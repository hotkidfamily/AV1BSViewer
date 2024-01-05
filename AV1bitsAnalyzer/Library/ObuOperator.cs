using System.Diagnostics;

namespace AV1bitsAnalyzer.Library
{
    public class OBPError
    {
        public string error = string.Empty;
    }

    public class OBPAnalyzerContext
    {
        public OBUSequenceHeader? seqheader;
        public OBUFrameHeader? prevFrameHeader;
        public OBUFrameHeader? curFrameHeader;
        public OBUTileGroup? tileGroup;
        public bool SeenFrameHeader = false;
        public bool prev_filled = false;
        public int frame_header_end_pos = 0;
        public OBUFrameType[] RefFrameType = new OBUFrameType[8];
        public byte[] RefValid = new byte[8];
        public byte[] RefOrderHint = new byte[8];
        public byte[] OrderHint = new byte[8];
        public byte[] RefFrameId = new byte[8];
        public uint[] RefUpscaledWidth = new uint[8];
        public uint[] RefFrameHeight = new uint[8];
        public uint[] RefRenderWidth = new uint[8];
        public uint[] RefRenderHeight = new uint[8];
        public int[] RefFrameSignBias = new int[8];
        public OBUFilmGrainParameters[] RefGrainParams = new OBUFilmGrainParameters[8];
        public byte order_hint = 0;
        public uint[,,] SavedGmParams = new uint[8, 8, 6];
        public bool[,,] SavedFeatureEnabled = new bool[8, 8, 8];
        public short[,,] SavedFeatureData = new short[8, 8, 8];
        public sbyte[,] SavedLoopFilterRefDeltas = new sbyte[8, 8];
        public sbyte[,] SavedLoopFilterModeDeltas = new sbyte[8, 8];
    }
#pragma warning disable IDE0059 // 不需要赋值
#pragma warning disable IDE0018 // 内联变量声明
    internal class ObuOperator
    {
        public static bool ObpIsValidObu (OBUType type)
        {
            return type == OBUType.OBU_SEQUENCE_HEADER ||
                   type == OBUType.OBU_TEMPORAL_DELIMITER ||
                   type == OBUType.OBU_FRAME_HEADER ||
                   type == OBUType.OBU_TILE_GROUP ||
                   type == OBUType.OBU_METADATA ||
                   type == OBUType.OBU_FRAME ||
                   type == OBUType.OBU_REDUNDANT_FRAME_HEADER ||
                   type == OBUType.OBU_TILE_LIST ||
                   type == OBUType.OBU_PADDING;
        }

        private static int ObpBr<T> (out T x, ref OBPBitReader br, int n, ref OBPError err) where T : struct
        {
            x = default;
            int r = AV1Bits.ObpBr(out ulong v, ref br, n, ref err);
            if ( typeof(T) == typeof(bool) )
            {
                x = (T) (object) (v != 0);
            }
            else if ( typeof(T) == typeof(byte) )
            {
                byte x1 = Convert.ToByte(v);
                x = (T) (object) x1;
            }
            else if ( typeof(T) == typeof(int) )
            {
                int x1 = Convert.ToInt32(v);
                x = (T) (object) x1;
            }
            else if ( typeof(T) == typeof(uint) )
            {
                uint x1 = Convert.ToUInt32(v);
                x = (T) (object) x1;
            }
            else if ( typeof(T) == typeof(ushort) )
            {
                ushort x1 = Convert.ToUInt16(v);
                x = (T) (object) x1;
            }
            else if ( typeof(T) == typeof(long) )
            {
                long x1 = Convert.ToInt64(v);
                x = (T) (object) x1;
            }
            else if ( typeof(T) == typeof(OBUFrameType) )
            {
                OBUFrameType frameType = (OBUFrameType)v;
                x = (T) (object) frameType;
            }
            else if ( typeof(T) == typeof(OBUMatrixCoefficients) )
            {
                OBUMatrixCoefficients v2 = (OBUMatrixCoefficients)v;
                x = (T) (object) v2;
            }
            else if ( typeof(T) == typeof(OBUTransferCharacteristics) )
            {
                OBUTransferCharacteristics v2 = (OBUTransferCharacteristics)v;
                x = (T) (object) v2;
            }
            else if ( typeof(T) == typeof(OBUColorPrimaries) )
            {
                OBUColorPrimaries v2 = (OBUColorPrimaries)v;
                x = (T) (object) v2;
            }
            else if ( typeof(T) == typeof(OBUChromaSamplePosition) )
            {
                OBUChromaSamplePosition v2 = (OBUChromaSamplePosition)v;
                x = (T) (object) v2;
            }
            return r;
        }

        public static int ObpSetFrameRefs (ref OBUFrameHeader fh, ref OBUSequenceHeader seq, ref OBPAnalyzerContext context, ref OBPError err)
        {
            int[] usedFrame = new int[8];
            uint curFrameHint, lastOrderHint, goldOrderHint, latestOrderHint, earliestOrderHint;
            byte[] shiftedOrderHints = new byte[8];
            int[] Ref_Frame_List = [2, 3, 5, 6, 7]; /*LAST2_FRAME, LAST3_FRAME, BWDREF_FRAME, ALTREF2_FRAME, ALTREF_FRAME */
            int[] ref_frame_idx = new int[8];

            for ( int i = 0; i < 7; i++ )
            {
                ref_frame_idx[i] = -1;
            }
            ref_frame_idx[1 - 1] = fh.last_frame_idx;
            ref_frame_idx[4 - 1] = fh.gold_frame_idx;
            for ( int i = 0; i < 8; i++ )
            {
                usedFrame[i] = 0;
            }
            usedFrame[fh.last_frame_idx] = 1;
            usedFrame[fh.gold_frame_idx] = 2;
            curFrameHint = (uint) (1 << (seq.OrderHintBits - 1));
            for ( int i = 0; i < 8; i++ )
            {
                shiftedOrderHints[i] = (byte) (curFrameHint + AV1Bits.ObpGetRelativeDist(context.RefOrderHint[i], fh.order_hint, seq));
            }
            lastOrderHint = shiftedOrderHints[fh.last_frame_idx];
            goldOrderHint = shiftedOrderHints[fh.gold_frame_idx];
            if ( lastOrderHint >= curFrameHint || goldOrderHint >= curFrameHint )
            {
                err.error = "(lastOrderHint >= curFrameHint || goldOrderHint >= curFrameHint) not allowed.";
                return -1;
            }
            /* find_latest_backward() */
            int refs = -1;
            latestOrderHint = 0;
            for ( int i = 0; i < 8; i++ )
            {
                uint hint = shiftedOrderHints[i];
                if ( usedFrame[i] == 0 && (hint >= curFrameHint) && (refs < 0 || hint >= latestOrderHint) )
                {
                    refs = i;
                    latestOrderHint = hint;
                }
            }
            if ( refs >= 0 )
            {
                ref_frame_idx[6] = refs;
                usedFrame[refs] = 1;
            }
            /* find_earliest_backward() */
            refs = -1;
            earliestOrderHint = 0;
            for ( int i = 0; i < 8; i++ )
            {
                uint hint = shiftedOrderHints[i];
                if ( usedFrame[i] == 0 && (hint >= curFrameHint) && (refs < 0 || hint < earliestOrderHint) )
                {
                    refs = i;
                    earliestOrderHint = hint;
                }
            }
            if ( refs >= 0 )
            {
                ref_frame_idx[4] = refs;
                usedFrame[refs] = 1;
            }
            /* find_earliest_backward() */
            refs = -1;
            earliestOrderHint = 0;
            for ( int i = 0; i < 8; i++ )
            {
                uint hint = shiftedOrderHints[i];
                if ( usedFrame[i] == 0 && (hint >= curFrameHint) && (refs < 0 || hint < earliestOrderHint) )
                {
                    refs = i;
                    earliestOrderHint = hint;
                }
            }
            if ( refs >= 0 )
            {
                ref_frame_idx[6 - 1] = refs;
                usedFrame[refs] = 1;
            }
            for ( int i = 0; i < 7 - 2; i++ )
            {
                byte refFrame = (byte)Ref_Frame_List[i];
                if ( ref_frame_idx[refFrame - 1] < 0 )
                {
                    int subref              = -1;
                    uint subLatestOrderHint = 0;
                    for ( int j = 0; i < 8; i++ )
                    {
                        uint hint = shiftedOrderHints[i];
                        if ( usedFrame[j] == 0 && (hint < curFrameHint) && (refs < 0 || hint >= subLatestOrderHint) )
                        {
                            refs = j;
                            subLatestOrderHint = 0;
                        }
                    }
                    refs = subref;
                    if ( refs >= 0 )
                    {
                        ref_frame_idx[refFrame - 1] = refs;
                        usedFrame[refs] = 1;
                    }
                }
            }
            refs = -1;
            for ( int i = 0; i < 8; i++ )
            {
                uint hint = shiftedOrderHints[i];
                if ( refs < 0 || hint < earliestOrderHint )
                {
                    refs = i;
                    earliestOrderHint = hint;
                }
            }
            for ( int i = 0; i < 7; i++ )
            {
                if ( ref_frame_idx[i] < 0 )
                {
                    ref_frame_idx[i] = refs;
                }
            }
            for ( int i = 0; i < 7; i++ )
            {
                fh.ref_frame_idx[i] = (byte) ref_frame_idx[i];
            }

            return 0;
        }

        public static int OBPReadDeltaQ (ref OBPBitReader br, ref int v, ref OBPError err)
        {
            ObpBr(out bool delta_coded, ref br, 1, ref err);
            if ( delta_coded )
            {
                int ret = AV1Bits.ObpSu(ref br, 7, out int val, ref err);
                if ( ret < 0 )
                    return ret;
                v = val;
            }
            else
            {
                v = 1;
            }
            return 0;
        }

        public static byte ObpGetQIndex (int ignoreDeltaQ, int segmentId,
            short CurrentQIndex, ref OBUFrameHeader fh, ref bool[,] FeatureEnabled, ref short[,] FeatureData)
        {
            if ( fh.segmentation_params.segmentation_enabled && FeatureEnabled[segmentId, 0] )
            {
                short data   = FeatureData[segmentId,0];
                int qindex = data + ((int) fh.quantization_params.base_q_idx);
                if ( ignoreDeltaQ == 0 && fh.delta_q_params.delta_q_present )
                {
                    qindex = CurrentQIndex + data;
                }
                return (byte) Math.Max(0, Math.Min(255, qindex));
            }

            if ( ignoreDeltaQ == 0 && fh.delta_q_params.delta_q_present )
            {
                return (byte) CurrentQIndex;
            }

            return fh.quantization_params.base_q_idx;
        }

        private static int OBPReadGlobalParam (ref OBPBitReader br, ref OBUFrameHeader fh, byte type, int refs, int idx, ref OBPError err)
        {
            int absBits  = 12;
            int precBits = 15;
            if ( idx < 2 )
            {
                if ( type == 1 )
                {
                    absBits = 9 - (fh.allow_high_precision_mv ? 0 : 1);
                    precBits = 3 - (fh.allow_high_precision_mv ? 0 : 1);
                }
                else
                {
                    absBits = 12;
                    precBits = 6;
                }
            }
            int precDiff = 16 - precBits;
            int round    = ((idx % 3) == 2) ? (1 << 16) : 0;
            int sub      = ((idx % 3) == 2) ? (1 << precBits) : 0;
            int mx       = (1 << absBits);
            int r        = ((int)fh.global_motion_params.prev_gm_params[refs,idx] >> precDiff) - sub;
            int ret = AV1Bits.ObpDecodeSignedSubexpWithRef(ref br, -mx, mx + 1, r, out short val, ref err);
            if ( ret < 0 )
            {
                return -1;
            }
            if ( val < 0 )
            { /* signed shifts are bad. */
                val = (short) -val;
                fh.global_motion_params.gm_params[refs, idx] = (-(val << precDiff) + round);
            }
            else
            {
                fh.global_motion_params.gm_params[refs, idx] = (val << precDiff) + round;
            }

            return 0;
        }

        /***api
         *
         *
         */

        public static int ObpGetNextObu (Span<byte> buf, out OBUType obuType, out long offset,
            out int size, out int temporalId, out int spatialId, ref OBPError err)
        {
            int pos = 0;
            int obuExtensionFlag = 0;
            int obuHasSizeField = 0;

            obuType = 0;
            offset = 0;
            size = 0;
            temporalId = 0;
            spatialId = 0;

            if ( buf.Length < 1 )
            {
                err.error = "Buffer is too small to contain an OBU.";
                return -1;
            }

            obuType = (OBUType) ((buf[pos] & 0x78) >> 3);
            obuExtensionFlag = (buf[pos] & 0x04) >> 2;
            obuHasSizeField = (buf[pos] & 0x02) >> 1;
            pos++;

            if ( !ObpIsValidObu(obuType) )
            {
                err.error = $"OBU header contains invalid OBU type: {(int) obuType}";
                return -1;
            }

            if ( obuExtensionFlag != 0 )
            {
                if ( buf.Length < 1 )
                {
                    err.error = "Buffer is too small to contain an OBU extension header.";
                    return -1;
                }
                temporalId = (buf[pos] & 0xE0) >> 5;
                spatialId = (buf[pos] & 0x18) >> 3;
                pos++;
            }
            else
            {
                temporalId = 0;
                spatialId = 0;
            }

            if ( obuHasSizeField != 0 )
            {
                int ret = AV1Bits.ObpLeb128(buf[pos..], out ulong value, out long consumed, ref err);
                if ( ret < 0 )
                {
                    return -1;
                }

                if ( value >= uint.MaxValue )
                {
                    err.error = "Invalid OBU size: larger than maximum allowed.";
                    return -1;
                }

                offset = pos + consumed;
                size = (int) value;
            }
            else
            {
                offset = pos;
                size = buf.Length - pos;
            }

            return 0;
        }

        public static int ObpParseSequenceHeader (Span<byte> buf, ref OBUSequenceHeader seq_header, ref OBPError err)
        {
            OBPBitReader br = AV1Bits.ObpNewBr(buf);

            ObpBr(out seq_header.seq_profile, ref br, 3, ref err);
            ObpBr(out seq_header.still_picture, ref br, 1, ref err);
            ObpBr(out seq_header.reduced_still_picture_header, ref br, 1, ref err);
            if ( seq_header.reduced_still_picture_header )
            {
                seq_header.timing_info_present_flag = false;
                seq_header.decoder_model_info_present_flag = false;
                seq_header.initial_display_delay_present_flag = false;
                seq_header.operating_points_cnt_minus_1 = 0;
                seq_header.operating_point_idc = [];
                seq_header.seq_level_idx = [];
                seq_header.seq_tier = [];
                seq_header.decoder_model_present_for_this_op[0] = false;
                seq_header.initial_display_delay_present_for_this_op[0] = false;
            }
            else
            {
                ObpBr(out seq_header.timing_info_present_flag, ref br, 1, ref err);
                if ( seq_header.timing_info_present_flag )
                {
                    /* timing_info() */
                    ObpBr(out seq_header.timing_info.num_units_in_display_tick, ref br, 32, ref err);
                    ObpBr(out seq_header.timing_info.time_scale, ref br, 32, ref err);
                    ObpBr(out seq_header.timing_info.equal_picture_interval, ref br, 1, ref err);
                    if ( seq_header.timing_info.equal_picture_interval )
                    {
                        int ret = AV1Bits.ObpUvlc(ref br, out seq_header.timing_info.num_ticks_per_picture_minus_1, ref err);
                        if ( ret < 0 )
                            return -1;
                    }
                    ObpBr(out seq_header.decoder_model_info_present_flag, ref br, 1, ref err);
                    if ( seq_header.decoder_model_info_present_flag )
                    {
                        /* decoder_model_info() */
                        ObpBr(out seq_header.decoder_model_info.buffer_delay_length_minus_1, ref br, 5, ref err);
                        ObpBr(out seq_header.decoder_model_info.num_units_in_decoding_tick, ref br, 32, ref err);
                        ObpBr(out seq_header.decoder_model_info.buffer_removal_time_length_minus_1, ref br, 5, ref err);
                        ObpBr(out seq_header.decoder_model_info.frame_presentation_time_length_minus_1, ref br, 5, ref err);
                    }
                }
                else
                {
                    seq_header.decoder_model_info_present_flag = false;
                }
                ObpBr(out seq_header.initial_display_delay_present_flag, ref br, 1, ref err);
                ObpBr(out seq_header.operating_points_cnt_minus_1, ref br, 5, ref err);
                for ( ulong i = 0; i <= seq_header.operating_points_cnt_minus_1; i++ )
                {
                    ObpBr(out seq_header.operating_point_idc[i], ref br, 12, ref err);
                    ObpBr(out seq_header.seq_level_idx[i], ref br, 5, ref err);
                    if ( seq_header.seq_level_idx[i] > 7 )
                    {
                        ObpBr(out seq_header.seq_tier[i], ref br, 1, ref err);
                    }
                    else
                    {
                        seq_header.seq_tier[i] = false;
                    }
                    if ( seq_header.decoder_model_info_present_flag )
                    {
                        ObpBr(out seq_header.decoder_model_present_for_this_op[i], ref br, 1, ref err);
                        if ( seq_header.decoder_model_present_for_this_op[i] )
                        {
                            /* operating_parameters_info() */
                            int n = seq_header.decoder_model_info.buffer_delay_length_minus_1 + 1;
                            ObpBr(out seq_header.operating_parameters_info[i].decoder_buffer_delay, ref br, n, ref err);
                            ObpBr(out seq_header.operating_parameters_info[i].encoder_buffer_delay, ref br, n, ref err);
                            ObpBr(out seq_header.operating_parameters_info[i].low_delay_mode_flag, ref br, 1, ref err);
                        }
                    }
                    else
                    {
                        seq_header.decoder_model_present_for_this_op[i] = false;
                    }
                    if ( seq_header.initial_display_delay_present_flag )
                    {
                        ObpBr(out seq_header.initial_display_delay_present_for_this_op[i], ref br, 1, ref err);
                        if ( seq_header.initial_display_delay_present_for_this_op[i] )
                        {
                            ObpBr(out seq_header.initial_display_delay_minus_1[i], ref br, 4, ref err);
                        }
                    }
                }
            }
            ObpBr(out seq_header.frame_width_bits_minus_1, ref br, 4, ref err);
            ObpBr(out seq_header.frame_height_bits_minus_1, ref br, 4, ref err);
            ObpBr(out seq_header.max_frame_width_minus_1, ref br, seq_header.frame_width_bits_minus_1 + 1, ref err);
            ObpBr(out seq_header.max_frame_height_minus_1, ref br, seq_header.frame_height_bits_minus_1 + 1, ref err);
            if ( seq_header.reduced_still_picture_header )
            {
                seq_header.frame_id_numbers_present_flag = false;
            }
            else
            {
                ObpBr(out seq_header.frame_id_numbers_present_flag, ref br, 1, ref err);
            }
            if ( seq_header.frame_id_numbers_present_flag )
            {
                ObpBr(out seq_header.delta_frame_id_length_minus_2, ref br, 4, ref err);
                ObpBr(out seq_header.additional_frame_id_length_minus_1, ref br, 3, ref err);
            }
            ObpBr(out seq_header.use_128x128_superblock, ref br, 1, ref err);
            ObpBr(out seq_header.enable_filter_intra, ref br, 1, ref err);
            ObpBr(out seq_header.enable_intra_edge_filter, ref br, 1, ref err);
            if ( seq_header.reduced_still_picture_header )
            {
                seq_header.enable_interintra_compound = false;
                seq_header.enable_masked_compound = false;
                seq_header.enable_warped_motion = false;
                seq_header.enable_dual_filter = false;
                seq_header.enable_order_hint = false;
                seq_header.enable_jnt_comp = false;
                seq_header.enable_ref_frame_mvs = false;
                seq_header.seq_force_screen_content_tools = 2; /* SELECT_SCREEN_CONTENT_TOOLS */
                seq_header.seq_force_integer_mv = 2; /* SELECT_INTEGER_MV */
                seq_header.OrderHintBits = 0;
            }
            else
            {
                ObpBr(out seq_header.enable_interintra_compound, ref br, 1, ref err);
                ObpBr(out seq_header.enable_masked_compound, ref br, 1, ref err);
                ObpBr(out seq_header.enable_warped_motion, ref br, 1, ref err);
                ObpBr(out seq_header.enable_dual_filter, ref br, 1, ref err);
                ObpBr(out seq_header.enable_order_hint, ref br, 1, ref err);
                if ( seq_header.enable_order_hint )
                {
                    ObpBr(out seq_header.enable_jnt_comp, ref br, 1, ref err);
                    ObpBr(out seq_header.enable_ref_frame_mvs, ref br, 1, ref err);
                }
                else
                {
                    seq_header.enable_jnt_comp = false;
                    seq_header.enable_ref_frame_mvs = false;
                }
                ObpBr(out seq_header.seq_choose_screen_content_tools, ref br, 1, ref err);
                if ( seq_header.seq_choose_screen_content_tools != 0 )
                {
                    seq_header.seq_force_screen_content_tools = 2; /* SELECT_SCREEN_CONTENT_TOOLS */
                }
                else
                {
                    ObpBr(out seq_header.seq_force_screen_content_tools, ref br, 1, ref err);
                }
                if ( seq_header.seq_force_screen_content_tools > 0 )
                {
                    ObpBr(out seq_header.seq_choose_integer_mv, ref br, 1, ref err);
                    if ( seq_header.seq_choose_integer_mv != 0 )
                    {
                        seq_header.seq_force_integer_mv = 2; /* SELECT_INTEGER_MV */
                    }
                    else
                    {
                        ObpBr(out seq_header.seq_force_integer_mv, ref br, 1, ref err);
                    }
                }
                else
                {
                    seq_header.seq_force_integer_mv = 2; /* SELECT_INTEGER_MV */
                }
                if ( seq_header.enable_order_hint )
                {
                    ObpBr(out seq_header.order_hint_bits_minus_1, ref br, 3, ref err);
                    seq_header.OrderHintBits = (byte) (seq_header.order_hint_bits_minus_1 + 1);
                }
                else
                {
                    seq_header.OrderHintBits = 0;
                }
            }
            ObpBr(out seq_header.enable_superres, ref br, 1, ref err);
            ObpBr(out seq_header.enable_cdef, ref br, 1, ref err);
            ObpBr(out seq_header.enable_restoration, ref br, 1, ref err);
            /* color_config() */
            ObpBr(out seq_header.color_config.high_bitdepth, ref br, 1, ref err);
            if ( seq_header.seq_profile == 2 && seq_header.color_config.high_bitdepth )
            {
                ObpBr(out seq_header.color_config.twelve_bit, ref br, 1, ref err);
                seq_header.color_config.BitDepth = seq_header.color_config.twelve_bit ? 12 : 10;
            }
            else
            {
                seq_header.color_config.BitDepth = seq_header.color_config.high_bitdepth ? 10 : 8;
            }
            if ( seq_header.seq_profile == 1 )
            {
                seq_header.color_config.mono_chrome = false;
            }
            else
            {
                ObpBr(out seq_header.color_config.mono_chrome, ref br, 1, ref err);
            }
            seq_header.color_config.NumPlanes = seq_header.color_config.mono_chrome ? 1 : 3;
            ObpBr(out seq_header.color_config.color_description_present_flag, ref br, 1, ref err);
            if ( seq_header.color_config.color_description_present_flag )
            {
                ObpBr(out seq_header.color_config.color_primaries, ref br, 8, ref err);
                ObpBr(out seq_header.color_config.transfer_characteristics, ref br, 8, ref err);
                ObpBr(out seq_header.color_config.matrix_coefficients, ref br, 8, ref err);
            }
            else
            {
                seq_header.color_config.color_primaries = OBUColorPrimaries.OBU_CP_UNSPECIFIED;
                seq_header.color_config.transfer_characteristics = OBUTransferCharacteristics.OBU_TC_UNSPECIFIED;
                seq_header.color_config.matrix_coefficients = OBUMatrixCoefficients.OBU_MC_UNSPECIFIED;
            }
            if ( seq_header.color_config.mono_chrome )
            {
                ObpBr(out seq_header.color_config.color_range, ref br, 1, ref err);
                seq_header.color_config.subsampling_x = true;
                seq_header.color_config.subsampling_y = true;
                seq_header.color_config.chroma_sample_position = OBUChromaSamplePosition.OBU_CSP_UNKNOWN;
                seq_header.color_config.separate_uv_delta_q = false;
                goto color_done;
            }
            else if ( seq_header.color_config.color_primaries == OBUColorPrimaries.OBU_CP_BT_709 &&
                       seq_header.color_config.transfer_characteristics == OBUTransferCharacteristics.OBU_TC_SRGB &&
                       seq_header.color_config.matrix_coefficients == OBUMatrixCoefficients.OBU_MC_IDENTITY )
            {
                seq_header.color_config.color_range = true;
                seq_header.color_config.subsampling_x = false;
                seq_header.color_config.subsampling_y = false;
            }
            else
            {
                ObpBr(out seq_header.color_config.color_range, ref br, 1, ref err);
                if ( seq_header.seq_profile == 0 )
                {
                    seq_header.color_config.subsampling_x = true;
                    seq_header.color_config.subsampling_y = true;
                }
                else if ( seq_header.seq_profile == 1 )
                {
                    seq_header.color_config.subsampling_x = false;
                    seq_header.color_config.subsampling_y = false;
                }
                else
                {
                    if ( seq_header.color_config.BitDepth == 12 )
                    {
                        ObpBr(out seq_header.color_config.subsampling_x, ref br, 1, ref err);
                        if ( seq_header.color_config.subsampling_x )
                        {
                            ObpBr(out seq_header.color_config.subsampling_y, ref br, 1, ref err);
                        }
                        else
                        {
                            seq_header.color_config.subsampling_y = false;
                        }
                    }
                    else
                    {
                        seq_header.color_config.subsampling_x = true;
                        seq_header.color_config.subsampling_y = false;
                    }
                }
                if ( seq_header.color_config.subsampling_x && seq_header.color_config.subsampling_y )
                {
                    ObpBr(out seq_header.color_config.chroma_sample_position, ref br, 2, ref err);
                }
            }
            ObpBr(out seq_header.color_config.separate_uv_delta_q, ref br, 1, ref err);

        color_done:
            ObpBr(out seq_header.film_grain_params_present, ref br, 1, ref err);
            return 0;
        }

        public static int ObpParseTileList (Span<byte> buf, ref OBUTileList tile_list, ref OBPError err)
        {
            int pos = 0;
            int buf_size = buf.Length;

            if ( buf_size < 4 )
            {
                err.error = "Tile list OBU must be at least 4 bytes.";
                return -1;
            }

            tile_list.output_frame_width_in_tiles_minus_1 = buf[0];
            tile_list.output_frame_height_in_tiles_minus_1 = buf[1];
            tile_list.tile_count_minus_1 = (ushort) ((buf[2] << 8) | buf[3]);
            pos += 4;

            tile_list.tile_list_entry = new OBUTile_list_entry[tile_list.tile_count_minus_1 + 1];

            for ( ushort i = 0; i < tile_list.tile_count_minus_1; i++ )
            {
                if ( pos + 5 > buf_size )
                {
                    err.error = "Tile list OBU malformed: Not enough bytes for next tile_list_entry().";
                    return -1;
                }

                tile_list.tile_list_entry[i].anchor_frame_idx = buf[pos];
                tile_list.tile_list_entry[i].anchor_tile_row = buf[pos + 1];
                tile_list.tile_list_entry[i].anchor_tile_col = buf[pos + 2];
                tile_list.tile_list_entry[i].tile_data_size_minus_1 = (ushort) ((buf[pos + 3] << 8) | buf[pos + 4]);
                pos += 5;

                int N = 8 * (tile_list.tile_list_entry[i].tile_data_size_minus_1 + 1);

                if ( pos + N > buf_size )
                {
                    err.error = "Tile list OBU malformed: Not enough bytes for next tile_list_entry()'s data.";
                    return -1;
                }

                tile_list.tile_list_entry[i].coded_tile_data = buf[pos..].ToArray();
                pos += N;
            }

            return 0;
        }

        public static int ObpParseTileGroup (Span<byte> buf, ref OBPAnalyzerContext context, ref OBPError err)
        {
            if ( context.curFrameHeader == null ) return 0;
            Debug.Assert(context.curFrameHeader != null);
            OBPBitReader br = AV1Bits.ObpNewBr(buf);
            OBUFrameHeader frame_header = context.curFrameHeader;
            OBUTileGroup tile_group = new ();
            context.tileGroup = tile_group;

            tile_group.NumTiles = (ushort) (frame_header.tile_info.TileCols * frame_header.tile_info.TileRows);
            int startBitPos = 0;
            tile_group.tile_start_and_end_present_flag = false;

            if ( tile_group.NumTiles > 1 )
            {
                ObpBr(out tile_group.tile_start_and_end_present_flag, ref br, 1, ref err);
            }
            if ( tile_group.NumTiles == 1 || !tile_group.tile_start_and_end_present_flag )
            {
                tile_group.tg_start = 0;
                tile_group.tg_end = (ushort) (tile_group.NumTiles - 1);
            }
            else
            {
                uint tileBits = AV1Bits.ObpTileLog2(1, frame_header.tile_info.TileCols) 
                    + AV1Bits.ObpTileLog2(1, frame_header.tile_info.TileRows);
                ObpBr(out tile_group.tg_start, ref br, (int) tileBits, ref err);
                ObpBr(out tile_group.tg_end, ref br, (int) tileBits, ref err);
            }

            AV1Bits.ObpBrByteAligment(ref br);
            int endBitPos = AV1Bits.ObpBrGetPos(ref br);
            int headerBytes = (endBitPos - startBitPos) / 8;
            int sz = buf.Length - headerBytes;
            int pos = headerBytes;
            tile_group.TileSize = new uint[tile_group.tg_end + 1];

            for ( ushort TileNum = tile_group.tg_start; TileNum <= tile_group.tg_end; TileNum++ )
            {
                bool lastTile = (TileNum == tile_group.tg_end);
                if ( lastTile )
                {
                    tile_group.TileSize[TileNum] = (uint) sz;
                }
                else
                {
                    ushort TileSizeBytes = (ushort)(frame_header.tile_info.tile_size_bytes_minus_1 + 1);
                    ulong tile_size_minus_1 = 0;
                    if ( sz < TileSizeBytes )
                    {
                        err.error = $"Not enough bytes left to read tile size for tile {TileNum}.";
                        return -1;
                    }

                    tile_size_minus_1 = AV1Bits.ObpLe(buf.Slice(pos, TileSizeBytes));
                    tile_group.TileSize[TileNum] = (uint) (tile_size_minus_1 + 1);
                    if ( sz < tile_group.TileSize[TileNum] )
                    {
                        err.error = $"Not enough bytes to contain TileSize for tile {TileNum}.";
                        return -1;
                    }
                    sz -= (int) tile_group.TileSize[TileNum] + TileSizeBytes;
                    pos += (int) tile_group.TileSize[TileNum] + TileSizeBytes;
                }
            }
            if ( tile_group.tg_end == tile_group.NumTiles - 1 )
            {
                context.SeenFrameHeader = false;
            }

            return 0;
        }

        public static int ObpParseMetadata (Span<byte> buf, ref OBUMetadata metadata, ref OBPError err)
        {
            ulong val = 0;
            long consumed = 0;
            int ret = AV1Bits.ObpLeb128(buf, out val, out consumed, ref err);
            if ( ret < 0 )
            {
                return -1;
            }

            var br = AV1Bits.ObpNewBr(buf[(int)consumed..]);
            if ( metadata.metadata_type == OBUMetadataType.OBU_METADATA_TYPE_HDR_CLL )
            {
                ObpBr(out metadata.metadata_hdr_cll.max_cll, ref br, 16, ref err);
                ObpBr(out metadata.metadata_hdr_cll.max_fall, ref br, 16, ref err);
            }
            else if ( metadata.metadata_type == OBUMetadataType.OBU_METADATA_TYPE_HDR_MDCV )
            {
                var mdcv = metadata.metadata_hdr_mdcv;
                for ( int i = 0; i < 3; i++ )
                {
                    ObpBr(out mdcv.primary_chromaticity_x[i], ref br, 16, ref err);
                    ObpBr(out mdcv.primary_chromaticity_y[i], ref br, 16, ref err);
                }
                ObpBr(out mdcv.white_point_chromaticity_x, ref br, 16, ref err);
                ObpBr(out mdcv.white_point_chromaticity_y, ref br, 16, ref err);
                ObpBr(out mdcv.luminance_max, ref br, 32, ref err);
                ObpBr(out mdcv.luminance_min, ref br, 32, ref err);
            }
            else if ( metadata.metadata_type == OBUMetadataType.OBU_METADATA_TYPE_SCALABILITY )
            {
                ObpBr(out metadata.metadata_scalability.scalability_mode_idc, ref br, 8, ref err);
                if ( metadata.metadata_scalability.scalability_mode_idc != 0 )
                {
                    /* scalability_structure() */
                    var scal = metadata.metadata_scalability.scalability_structure;
                    ObpBr(out scal.spatial_layers_cnt_minus_1, ref br, 2, ref err);
                    ObpBr(out scal.spatial_layer_dimensions_present_flag, ref br, 1, ref err);
                    ObpBr(out scal.spatial_layer_description_present_flag, ref br, 1, ref err);
                    ObpBr(out scal.temporal_group_description_present_flag, ref br, 1, ref err);
                    ObpBr(out scal.scalability_structure_reserved_3bits, ref br, 3, ref err);
                    if ( scal.spatial_layer_dimensions_present_flag )
                    {
                        for ( byte i = 0; i < scal.spatial_layers_cnt_minus_1; i++ )
                        {
                            ObpBr(out scal.spatial_layer_max_width[i], ref br, 16, ref err);
                            ObpBr(out scal.spatial_layer_max_height[i], ref br, 16, ref err);
                        }
                    }
                    if ( scal.spatial_layer_description_present_flag )
                    {
                        for ( byte i = 0; i < scal.spatial_layers_cnt_minus_1; i++ )
                        {
                            ObpBr(out scal.spatial_layer_ref_id[i], ref br, 8, ref err);
                        }
                    }
                    if ( scal.temporal_group_description_present_flag )
                    {
                        ObpBr(out scal.temporal_group_size, ref br, 8, ref err);
                        for ( byte i = 0; i < scal.temporal_group_size; i++ )
                        {
                            ObpBr(out scal.temporal_group_temporal_id[i], ref br, 3, ref err);
                            ObpBr(out scal.temporal_group_temporal_switching_up_point_flag[i], ref br, 1, ref err);
                            ObpBr(out scal.temporal_group_spatial_switching_up_point_flag[i], ref br, 1, ref err);
                            ObpBr(out scal.temporal_group_ref_cnt[i], ref br, 3, ref err);
                            for ( byte j = 0; j < scal.temporal_group_ref_cnt[i]; j++ )
                            {
                                ObpBr(out scal.temporal_group_ref_pic_diff[i, j], ref br, 8, ref err);
                            }
                        }
                    }
                }
            }
            else if ( metadata.metadata_type == OBUMetadataType.OBU_METADATA_TYPE_ITUT_T35 )
            {
                int offset = 1;
                var t35 = metadata.metadata_itut_t35;
                ObpBr(out t35.itu_t_t35_country_code, ref br, 8, ref err);
                if ( t35.itu_t_t35_country_code == 0xFF )
                {
                    ObpBr(out t35.itu_t_t35_country_code_extension_byte, ref br, 8, ref err);
                    offset++;
                }
                t35.itu_t_t35_payload_bytes = buf[(int) (consumed + offset)..].ToArray();
                int non_zero_bytes_seen = 0;
                /*
                 * OBUs with byte payloads at the end have a dumb property where you need to
                 * know the trailing bits *before* you parse the OBU, despite the way the spec
                 * the syntax displayed and defined. SO as a result, you need to find the *second*
                 * non-zero byte at the end of the OBU payload, rather than the last one, as
                 * the note in the ITU T.35 part of the spec says.
                 */
                for ( long i = buf.Length - consumed - offset - 1; i > 0; i-- )
                {
                    if ( t35.itu_t_t35_payload_bytes[i] != 0 )
                    {
                        non_zero_bytes_seen++;
                        if ( non_zero_bytes_seen == 2 )
                        {
                            t35.itu_t_t35_payload_bytes_size = (int) i + 1;
                        }
                    }
                }
            }
            else if ( metadata.metadata_type == OBUMetadataType.OBU_METADATA_TYPE_TIMECODE )
            {
                var tcd = metadata.metadata_timecode;
                ObpBr(out tcd.counting_type, ref br, 5, ref err);
                ObpBr(out tcd.full_timestamp_flag, ref br, 1, ref err);
                ObpBr(out tcd.discontinuity_flag, ref br, 1, ref err);
                ObpBr(out tcd.cnt_dropped_flag, ref br, 1, ref err);
                ObpBr(out tcd.n_frames, ref br, 9, ref err);
                if ( tcd.full_timestamp_flag )
                {
                    ObpBr(out tcd.seconds_value, ref br, 6, ref err);
                    ObpBr(out tcd.minutes_value, ref br, 6, ref err);
                    ObpBr(out tcd.hours_value, ref br, 5, ref err);
                }
                else
                {
                    ObpBr(out tcd.seconds_flag, ref br, 1, ref err);
                    if ( tcd.seconds_flag )
                    {
                        ObpBr(out tcd.seconds_value, ref br, 6, ref err);
                        ObpBr(out tcd.minutes_flag, ref br, 1, ref err);
                        if ( tcd.minutes_flag )
                        {
                            ObpBr(out tcd.minutes_value, ref br, 6, ref err);
                            ObpBr(out tcd.hours_flag, ref br, 1, ref err);
                            if ( tcd.hours_flag )
                            {
                                ObpBr(out tcd.hours_value, ref br, 5, ref err);
                            }
                        }
                    }
                }
                ObpBr(out tcd.time_offset_length, ref br, 5, ref err);
                if ( tcd.time_offset_length > 0 )
                {
                    ObpBr(out tcd.time_offset_value, ref br, tcd.time_offset_length, ref err);
                }
            }
            else if ( (int) metadata.metadata_type >= 6 && (int) metadata.metadata_type <= 31 )
            {
                metadata.unregistered.buf = buf[(int) consumed..].ToArray();
            }
            else
            {
                err.error = $"Invalid metadata type:{metadata.metadata_type}";
                return -1;
            }

            return 0;
        }

        public static int ObpParseFrame (Span<byte> buf, ref OBPAnalyzerContext context, int tid, int sid, ref OBPError err)
        {
            int startBitPos = 0;
            int endBitPos = 0;
            int headerBytes = 0;

            int ret = ObpParseFrameHeader(buf, ref context, tid, sid, ref err);
            if ( ret < 0 )
            {
                return -1;
            }

            endBitPos = context.frame_header_end_pos;
            headerBytes = (endBitPos - startBitPos) / 8;

            return ObpParseTileGroup(buf[headerBytes..], ref context, ref err);
        }

        public static int ObpParseFrameHeader (Span<byte> buf, ref OBPAnalyzerContext context, int tid, int sid, ref OBPError err)
        {
            Debug.Assert(context.seqheader != null);
            OBPBitReader br = AV1Bits.ObpNewBr(buf);
            var seq = context.seqheader;

            if ( context.SeenFrameHeader )
            {
                if ( !context.prev_filled )
                {
                    err.error = "SeenFrameHeader is one, but no previous header exists in state.";
                    return -1;
                }
                context.curFrameHeader = context.prevFrameHeader;
                return 0;
            }

            var fh = new OBUFrameHeader();
            context.curFrameHeader = fh;
            context.SeenFrameHeader = true;

            /* uncompressed_header() */
            int idLen = 0; /* only set to 0 to shut up a compiler warning. */
            if ( seq.frame_id_numbers_present_flag )
            {
                idLen = seq.additional_frame_id_length_minus_1 + seq.delta_frame_id_length_minus_2 + 3;
            }
            byte allFrames = 255; /* (1 << 8) - 1 */
            bool FrameIsIntra;
            if ( seq.reduced_still_picture_header )
            {
                fh.show_existing_frame = false;
                fh.frame_type = OBUFrameType.OBU_KEY_FRAME;
                FrameIsIntra = true;
                fh.show_frame = true;
                fh.showable_frame = true;
            }
            else
            {
                ObpBr(out fh.show_existing_frame, ref br, 1, ref err);
                if ( fh.show_existing_frame )
                {
                    ObpBr(out fh.frame_to_show_map_idx, ref br, 3, ref err);
                    if ( seq.decoder_model_info_present_flag && !seq.timing_info.equal_picture_interval )
                    {
                        /* temporal_point_info() */
                        int n = seq.decoder_model_info.frame_presentation_time_length_minus_1 + 1;
                        ObpBr(out fh.temporal_point_info.frame_presentation_time, ref br, n, ref err);
                    }
                    fh.refresh_frame_flags = 0;
                    if ( seq.frame_id_numbers_present_flag )
                    {
                        Debug.Assert(idLen <= 255);
                        ObpBr(out fh.display_frame_id, ref br, idLen, ref err);
                    }
                    fh.frame_type = context.RefFrameType[fh.frame_to_show_map_idx];
                    if ( fh.frame_type == OBUFrameType.OBU_KEY_FRAME )
                    {
                        fh.refresh_frame_flags = allFrames;
                    }
                    if ( seq.film_grain_params_present )
                    {
                        /* load_grain_params() */
                        fh.film_grain_params = context.RefGrainParams[fh.frame_to_show_map_idx];
                    }
                    return 0;
                }
                ObpBr(out fh.frame_type, ref br, 2, ref err);
                FrameIsIntra = (fh.frame_type == OBUFrameType.OBU_INTRA_ONLY_FRAME || fh.frame_type == OBUFrameType.OBU_KEY_FRAME);
                ObpBr(out fh.show_frame, ref br, 1, ref err);
                if ( fh.show_frame && seq.decoder_model_info_present_flag && !seq.timing_info.equal_picture_interval )
                {
                    /* temporal_point_info() */
                    int n = seq.decoder_model_info.frame_presentation_time_length_minus_1 + 1;
                    ObpBr(out fh.temporal_point_info.frame_presentation_time, ref br, n, ref err);
                }
                if ( fh.show_frame )
                {
                    fh.showable_frame = fh.frame_type != OBUFrameType.OBU_KEY_FRAME;
                }
                else
                {
                    ObpBr(out fh.showable_frame, ref br, 1, ref err);
                }
                if ( fh.frame_type == OBUFrameType.OBU_SWITCH_FRAME || (fh.frame_type == OBUFrameType.OBU_KEY_FRAME && fh.show_frame) )
                {
                    fh.error_resilient_mode = true;
                }
                else
                {
                    ObpBr(out fh.error_resilient_mode, ref br, 1, ref err);
                }
            }
            if ( fh.frame_type == OBUFrameType.OBU_KEY_FRAME && fh.show_frame )
            {
                for ( int i = 0; i < 8; i++ )
                {
                    context.RefValid[i] = 0;
                    context.RefOrderHint[i] = 0;
                }
                for ( int i = 0; i < 7; i++ )
                {
                    context.OrderHint[1 + i] = 0;
                }
            }
            ObpBr(out fh.disable_cdf_update, ref br, 1, ref err);
            if ( seq.seq_force_screen_content_tools == 2 )
            {
                ObpBr(out fh.allow_screen_content_tools, ref br, 1, ref err);
            }
            else
            {
                fh.allow_screen_content_tools = seq.seq_force_screen_content_tools > 0;
            }
            if ( fh.allow_screen_content_tools )
            {
                if ( seq.seq_force_integer_mv == 2 )
                {
                    ObpBr(out fh.force_integer_mv, ref br, 1, ref err);
                }
                else
                {
                    fh.force_integer_mv = seq.seq_force_integer_mv > 0;
                }
            }
            else
            {
                fh.force_integer_mv = false;
            }
            if ( FrameIsIntra )
            {
                fh.force_integer_mv = true;
            }
            if ( seq.frame_id_numbers_present_flag )
            {
                /*PrevFrameID = current_frame_id */
                Debug.Assert(idLen <= 255);
                ObpBr(out fh.current_frame_id, ref br, (byte) idLen, ref err);
                /* mark_ref_frames(idLen) */
                byte diffLen = (byte)(seq.delta_frame_id_length_minus_2 + 2);
                for ( int i = 0; i < 8; i++ )
                {
                    if ( fh.current_frame_id > (((uint) 1) << diffLen) )
                    {
                        if ( context.RefFrameId[i] > fh.current_frame_id || context.RefFrameId[i] < (fh.current_frame_id - (1 << diffLen)) )
                        {
                            context.RefValid[i] = 0;
                        }
                    }
                    else
                    {
                        if ( context.RefFrameId[i] > fh.current_frame_id && context.RefFrameId[i] < ((1 << idLen) + fh.current_frame_id + (1 << diffLen)) )
                        {
                            context.RefValid[i] = 0;
                        }
                    }
                }
            }
            else
            {
                fh.current_frame_id = 0;
            }
            if ( fh.frame_type == OBUFrameType.OBU_SWITCH_FRAME )
            {
                fh.frame_size_override_flag = true;
            }
            else if ( seq.reduced_still_picture_header )
            {
                fh.frame_size_override_flag = false;
            }
            else
            {
                ObpBr(out fh.frame_size_override_flag, ref br, 1, ref err);
            }
            if ( seq.OrderHintBits != 0 )
            { /* Added by me. */
                ObpBr(out fh.order_hint, ref br, seq.OrderHintBits, ref err);
            }
            else
            {
                fh.order_hint = 0;
            }
            byte OrderHint = fh.order_hint;
            if ( FrameIsIntra || fh.error_resilient_mode )
            {
                fh.primary_ref_frame = 7;
            }
            else
            {
                ObpBr(out fh.primary_ref_frame, ref br, 3, ref err);
            }
            if ( seq.decoder_model_info_present_flag )
            {
                ObpBr(out fh.buffer_removal_time_present_flag, ref br, 1, ref err);
                if ( fh.buffer_removal_time_present_flag )
                {
                    for ( byte opNum = 0; opNum <= seq.operating_points_cnt_minus_1; opNum++ )
                    {
                        if ( seq.decoder_model_present_for_this_op[opNum] )
                        {
                            byte opPtIdc = seq.operating_point_idc[opNum];
                            int inTemporalLayer = (opPtIdc >> tid) & 1;
                            int inSpatialLayer = (opPtIdc >> (sid + 8)) & 1;
                            if ( opPtIdc == 0 || (inTemporalLayer != 0 && inSpatialLayer != 0) )
                            {
                                int n = seq.decoder_model_info.buffer_removal_time_length_minus_1 + 1;
                                ObpBr(out fh.buffer_removal_time[opNum], ref br, n, ref err);
                            }
                        }
                    }
                }
            }
            fh.allow_high_precision_mv = false;
            fh.use_ref_frame_mvs = false;
            fh.allow_intrabc = false;
            if ( fh.frame_type == OBUFrameType.OBU_SWITCH_FRAME || (fh.frame_type == OBUFrameType.OBU_KEY_FRAME && fh.show_frame) )
            {
                fh.refresh_frame_flags = allFrames;
            }
            else
            {
                ObpBr(out fh.refresh_frame_flags, ref br, 8, ref err);
            }
            if ( !FrameIsIntra || fh.refresh_frame_flags != allFrames )
            {
                if ( fh.error_resilient_mode && seq.enable_order_hint )
                {
                    for ( int i = 0; i < 8; i++ )
                    {
                        ObpBr(out fh.ref_order_hint[i], ref br, seq.OrderHintBits, ref err);
                        if ( fh.ref_order_hint[i] != context.RefOrderHint[i] )
                        {
                            context.RefValid[i] = 0;
                        }
                    }
                }
            }
            uint FrameWidth = 0, FrameHeight = 0;
            uint UpscaledWidth = 0;
            uint MiCols = 0, MiRows = 0;
            if ( FrameIsIntra )
            {
                /* frame_size() */
                if ( fh.frame_size_override_flag )
                {
                    byte n = (byte)(seq.frame_width_bits_minus_1 + 1);
                    ObpBr(out fh.frame_width_minus_1, ref br, n, ref err);
                    n = (byte) (seq.frame_height_bits_minus_1 + 1);
                    ObpBr(out fh.frame_height_minus_1, ref br, n, ref err);
                    FrameWidth = fh.frame_width_minus_1 + 1;
                    FrameHeight = fh.frame_height_minus_1 + 1;
                }
                else
                {
                    FrameWidth = seq.max_frame_width_minus_1 + 1;
                    FrameHeight = seq.max_frame_height_minus_1 + 1;
                }
                /* superres_params() */
                uint SuperresDenom = 0;
                if ( seq.enable_superres )
                {
                    ObpBr(out fh.superres_params.use_superres, ref br, 1, ref err);
                }
                else
                {
                    fh.superres_params.use_superres = false;
                }
                if ( fh.superres_params.use_superres )
                {
                    ObpBr(out fh.superres_params.coded_denom, ref br, 3, ref err);
                    SuperresDenom = (uint) (fh.superres_params.coded_denom + 9);
                }
                else
                {
                    SuperresDenom = 8;
                }
                UpscaledWidth = FrameWidth;
                FrameWidth = (UpscaledWidth * 8 + (SuperresDenom / 2)) / SuperresDenom;
                /* compute_image_size() */
                MiCols = 2 * ((FrameWidth + 7) >> 3);
                MiRows = 2 * ((FrameHeight + 7) >> 3);
                /* render_size() */
                ObpBr(out fh.render_and_frame_size_different, ref br, 1, ref err);
                if ( fh.render_and_frame_size_different )
                {
                    ObpBr(out fh.render_width_minus_1, ref br, 16, ref err);
                    ObpBr(out fh.render_height_minus_1, ref br, 16, ref err);
                    fh.RenderWidth = (uint) (fh.render_width_minus_1 + 1);
                    fh.RenderHeight = (uint) (fh.render_height_minus_1 + 1);
                }
                else
                {
                    fh.RenderWidth = UpscaledWidth;
                    fh.RenderHeight = FrameHeight;
                }
                if ( fh.allow_screen_content_tools && UpscaledWidth == FrameWidth )
                {
                    ObpBr(out fh.allow_intrabc, ref br, 1, ref err);
                }
            }
            else
            {
                if ( !seq.enable_order_hint )
                {
                    fh.frame_refs_short_signaling = false;
                }
                else
                {
                    ObpBr(out fh.frame_refs_short_signaling, ref br, 1, ref err);
                    if ( fh.frame_refs_short_signaling )
                    {
                        ObpBr(out fh.last_frame_idx, ref br, 3, ref err);
                        ObpBr(out fh.gold_frame_idx, ref br, 3, ref err);
                        int vret = ObpSetFrameRefs(ref fh, ref seq, ref context, ref err);
                        if ( vret < 0 )
                        {
                            return -1;
                        }
                    }
                }
                for ( int i = 0; i < 7; i++ )
                {
                    if ( !fh.frame_refs_short_signaling )
                    {
                        ObpBr(out fh.ref_frame_idx[i], ref br, 3, ref err);
                    }
                    if ( seq.frame_id_numbers_present_flag )
                    {
                        int n = seq.delta_frame_id_length_minus_2 + 2;
                        ObpBr(out fh.delta_frame_id_minus_1[i], ref br, n, ref err);
                        byte DeltaFrameId    = (byte)(fh.delta_frame_id_minus_1[i] + 1);
                        byte expectedFrameId = (byte)((fh.current_frame_id + (1 << idLen) - DeltaFrameId) % (1 << idLen));
                        if ( context.RefFrameId[fh.ref_frame_idx[i]] != expectedFrameId )
                        {
                            return -1;
                        }
                    }
                }
                if ( fh.frame_size_override_flag && !fh.error_resilient_mode )
                {
                    for ( int i = 0; i < 7; i++ )
                    {
                        ObpBr(out fh.found_ref, ref br, 1, ref err);
                        if ( fh.found_ref )
                        {
                            UpscaledWidth = context.RefUpscaledWidth[fh.ref_frame_idx[i]];
                            FrameWidth = UpscaledWidth;
                            FrameHeight = context.RefFrameHeight[fh.ref_frame_idx[i]];
                            fh.RenderWidth = context.RefRenderWidth[fh.ref_frame_idx[i]];
                            fh.RenderHeight = context.RefRenderHeight[fh.ref_frame_idx[i]];
                            break;
                        }
                    }
                    if ( !fh.found_ref )
                    {
                        /* frame_size() */
                        if ( fh.frame_size_override_flag )
                        {
                            int n = seq.frame_width_bits_minus_1 + 1;
                            ObpBr(out fh.frame_width_minus_1, ref br, n, ref err);
                            n = seq.frame_height_bits_minus_1 + 1;
                            ObpBr(out fh.frame_height_minus_1, ref br, n, ref err);
                            FrameWidth = fh.frame_width_minus_1 + 1;
                            FrameHeight = fh.frame_height_minus_1 + 1;
                        }
                        else
                        {
                            FrameWidth = seq.max_frame_width_minus_1 + 1;
                            FrameHeight = seq.max_frame_height_minus_1 + 1;
                        }
                        /* superres_params() */
                        uint SuperresDenom = 0;
                        if ( seq.enable_superres )
                        {
                            ObpBr(out fh.superres_params.use_superres, ref br, 1, ref err);
                        }
                        else
                        {
                            fh.superres_params.use_superres = false;
                        }
                        if ( fh.superres_params.use_superres )
                        {
                            ObpBr(out fh.superres_params.coded_denom, ref br, 3, ref err);
                            SuperresDenom = (uint) (fh.superres_params.coded_denom + 9);
                        }
                        else
                        {
                            SuperresDenom = 8;
                        }
                        UpscaledWidth = FrameWidth;
                        FrameWidth = (UpscaledWidth * 8 + (SuperresDenom / 2)) / SuperresDenom;
                        /* compute_image_size() */
                        MiCols = 2 * ((FrameWidth + 7) >> 3);
                        MiRows = 2 * ((FrameHeight + 7) >> 3);
                        /* render_size() */
                        ObpBr(out fh.render_and_frame_size_different, ref br, 1, ref err);
                        if ( fh.render_and_frame_size_different )
                        {
                            ObpBr(out fh.render_width_minus_1, ref br, 16, ref err);
                            ObpBr(out fh.render_height_minus_1, ref br, 16, ref err);
                            fh.RenderWidth = (uint) (fh.render_width_minus_1 + 1);
                            fh.RenderHeight = (uint) (fh.render_height_minus_1 + 1);
                        }
                        else
                        {
                            fh.RenderWidth = UpscaledWidth;
                            fh.RenderHeight = FrameHeight;
                        }
                    }
                    else
                    {
                        /* superres_params() */
                        uint SuperresDenom = 0;
                        if ( seq.enable_superres )
                        {
                            ObpBr(out fh.superres_params.use_superres, ref br, 1, ref err);
                        }
                        else
                        {
                            fh.superres_params.use_superres = false;
                        }
                        if ( fh.superres_params.use_superres )
                        {
                            ObpBr(out fh.superres_params.coded_denom, ref br, 3, ref err);
                            SuperresDenom = (byte) (fh.superres_params.coded_denom + 9);
                        }
                        else
                        {
                            SuperresDenom = 8;
                        }
                        UpscaledWidth = FrameWidth;
                        FrameWidth = (UpscaledWidth * 8 + (SuperresDenom / 2)) / SuperresDenom;
                        /* compute_image_size() */
                        MiCols = 2 * ((FrameWidth + 7) >> 3);
                        MiRows = 2 * ((FrameHeight + 7) >> 3);
                    }
                }
                else
                {
                    /* frame_size() */
                    if ( fh.frame_size_override_flag )
                    {
                        int n = seq.frame_width_bits_minus_1 + 1;
                        ObpBr(out fh.frame_width_minus_1, ref br, n, ref err);
                        n = (byte) (seq.frame_height_bits_minus_1 + 1);
                        ObpBr(out fh.frame_height_minus_1, ref br, n, ref err);
                        FrameWidth = fh.frame_width_minus_1 + 1;
                        FrameHeight = fh.frame_height_minus_1 + 1;
                    }
                    else
                    {
                        FrameWidth = seq.max_frame_width_minus_1 + 1;
                        FrameHeight = seq.max_frame_height_minus_1 + 1;
                    }
                    /* superres_params() */
                    uint SuperresDenom = 0;
                    if ( seq.enable_superres )
                    {
                        ObpBr(out fh.superres_params.use_superres, ref br, 1, ref err);
                    }
                    else
                    {
                        fh.superres_params.use_superres = false;
                    }
                    if ( fh.superres_params.use_superres )
                    {
                        ObpBr(out fh.superres_params.coded_denom, ref br, 3, ref err);
                        SuperresDenom = (uint) (fh.superres_params.coded_denom + 9);
                    }
                    else
                    {
                        SuperresDenom = 8;
                    }
                    UpscaledWidth = FrameWidth;
                    FrameWidth = (UpscaledWidth * 8 + (SuperresDenom / 2)) / SuperresDenom;
                    /* compute_image_size() */
                    MiCols = 2 * ((FrameWidth + 7) >> 3);
                    MiRows = 2 * ((FrameHeight + 7) >> 3);
                    /* render_size() */
                    ObpBr(out fh.render_and_frame_size_different, ref br, 1, ref err);
                    if ( fh.render_and_frame_size_different )
                    {
                        ObpBr(out fh.render_width_minus_1, ref br, 16, ref err);
                        ObpBr(out fh.render_height_minus_1, ref br, 16, ref err);
                        fh.RenderWidth = (uint) (fh.render_width_minus_1 + 1);
                        fh.RenderHeight = (uint) (fh.render_height_minus_1 + 1);
                    }
                    else
                    {
                        fh.RenderWidth = UpscaledWidth;
                        fh.RenderHeight = FrameHeight;
                    }
                }
                if ( fh.force_integer_mv )
                {
                    fh.allow_high_precision_mv = false;
                }
                else
                {
                    ObpBr(out fh.allow_high_precision_mv, ref br, 1, ref err);
                }
                /* read_interpolation_filer() */
                ObpBr(out fh.interpolation_filter.is_filter_switchable, ref br, 1, ref err);
                if ( fh.interpolation_filter.is_filter_switchable )
                {
                    fh.interpolation_filter.interpolation_filter = 4;
                }
                else
                {
                    ObpBr(out fh.interpolation_filter.interpolation_filter, ref br, 2, ref err);
                }
                ObpBr(out fh.is_motion_mode_switchable, ref br, 1, ref err);
                if ( fh.error_resilient_mode || !seq.enable_ref_frame_mvs )
                {
                    fh.use_ref_frame_mvs = false;
                }
                else
                {
                    ObpBr(out fh.use_ref_frame_mvs, ref br, 1, ref err);
                }
                for ( int i = 0; i < 7; i++ )
                {
                    int refFrame = 1 + i;
                    byte hint = context.RefOrderHint[fh.ref_frame_idx[i]];
                    context.OrderHint[refFrame] = hint;
                    if ( !seq.enable_order_hint )
                    {
                        context.RefFrameSignBias[refFrame] = 0;
                    }
                    else
                    {
                        context.RefFrameSignBias[refFrame] = AV1Bits.ObpGetRelativeDist((int) hint, (int) OrderHint, seq);
                    }
                }
            }
            if ( seq.reduced_still_picture_header || fh.disable_cdf_update )
            {
                fh.disable_frame_end_update_cdf = true;
            }
            else
            {
                ObpBr(out fh.disable_frame_end_update_cdf, ref br, 1, ref err);
            }
            bool[,] FeatureEnabled = new bool[8, 8];
            short[,] FeatureData = new short[8, 8];
            if ( fh.primary_ref_frame == 7 )
            {
                /* init_non_coeff_cdfs() not relevant to OBU parsing. */
                /* setup_past_independence() */
                for ( int i = 1; i < 7; i++ )
                {
                    fh.global_motion_params.gm_type[i] = 0;
                    for ( int j = 0; j < 6; j++ )
                    {
                        fh.global_motion_params.gm_params[i, j] = (int) ((i % 3 == 2) ? (((uint) 1) << 16) : 0);
                    }
                }
                fh.loop_filter_params.loop_filter_delta_enabled = true;
                fh.loop_filter_params.loop_filter_ref_deltas = [1, 0, 0, 0, 0, -1, -1, -1];
                for ( int i = 0; i < 2; i++ )
                {
                    fh.loop_filter_params.loop_filter_mode_deltas[i] = 0;
                }
            }
            else
            {
                /* load_cdfs() not relevant to OBU parsing. */
                /* load_previous */
                int prevFrame = fh.ref_frame_idx[fh.primary_ref_frame];
                for ( int i = 0; i > 8; i++ )
                {
                    for ( int j = 0; j < 6; j++ )
                    {
                        fh.global_motion_params.prev_gm_params[i, j] = context.SavedGmParams[prevFrame, i, j];
                    }
                }
                /* load_loop_filter_params() */
                for ( int i = 0; i < 8; i++ )
                {
                    fh.loop_filter_params.loop_filter_ref_deltas[i] = context.SavedLoopFilterRefDeltas[prevFrame, i];
                    fh.loop_filter_params.loop_filter_mode_deltas[i] = context.SavedLoopFilterModeDeltas[prevFrame, i];
                }
                /* load_segmentation_params() */
                for ( int i = 0; i < 8; i++ )
                {
                    for ( int j = 0; j < 8; j++ )
                    {
                        FeatureEnabled[i, j] = context.SavedFeatureEnabled[prevFrame, i, j];
                        FeatureData[i, j] = context.SavedFeatureData[prevFrame, i, j];
                    }
                }
            }
            /* Not relevant to OBU parsing:
                   if (fh.use_ref_frame_mvs) {
                       motion_field_estimation()
                   }
             */
            /* tile_info() */
            uint sbCols          = seq.use_128x128_superblock ? ((MiCols + 31) >> 5) : ((MiCols + 15) >> 4);
            uint sbRows          = seq.use_128x128_superblock ? ((MiRows + 31) >> 5) : ((MiRows + 15) >> 4);
            uint sbShift         = seq.use_128x128_superblock ? 5u : 4u;
            int sbSize          = (int)(sbShift + 2);
            uint maxTileWidthSb  = (uint)(4096 >> sbSize);
            uint maxTileAreaSb   = (uint)((4096 * 2304) >> (2 * sbSize));
            long minLog2TileCols = AV1Bits.ObpTileLog2(maxTileWidthSb, sbCols);
            long maxLog2TileCols = AV1Bits.ObpTileLog2(1, Math.Min(sbCols, 64));
            long maxLog2TileRows = AV1Bits.ObpTileLog2(1, Math.Min(sbRows, 64));
            long minLog2Tiles    = Math.Max(minLog2TileCols, AV1Bits.ObpTileLog2(maxTileAreaSb, sbRows * sbCols));
            long minLog2TileRows = 0, TileColsLog2 = 0, TileRowsLog2 = 0;
            ObpBr(out fh.tile_info.uniform_tile_spacing_flag, ref br, 1, ref err);
            if ( fh.tile_info.uniform_tile_spacing_flag )
            {
                TileColsLog2 = minLog2TileCols;
                while ( TileColsLog2 < maxLog2TileCols )
                {
                    ObpBr(out bool increment_tile_cols_log2, ref br, 1, ref err);
                    if ( increment_tile_cols_log2 )
                    {
                        TileColsLog2++;
                    }
                    else
                    {
                        break;
                    }
                }
                uint tileWidthSb = (uint)(sbCols + (1 << (int)TileColsLog2) - 1) >> (int)TileColsLog2;
                ushort i = 0;
                for ( uint startSb = 0; startSb < sbCols; startSb += tileWidthSb )
                {
                    /* MiColStarts[i] = startSb << sbShift; */
                    i += 1;
                }
                /*MiColStarts[i]           = MiCols; */
                fh.tile_info.TileCols = i;

                minLog2TileRows = Math.Max(minLog2Tiles - TileColsLog2, 0);
                TileRowsLog2 = minLog2TileRows;
                while ( TileRowsLog2 < maxLog2TileRows )
                {
                    bool increment_tile_rows_log2 = false;
                    ObpBr(out increment_tile_rows_log2, ref br, 1, ref err);
                    if ( increment_tile_rows_log2 )
                    {
                        TileRowsLog2++;
                    }
                    else
                    {
                        break;
                    }
                }
                uint tileHeightSb =(uint)( (sbRows + (1 << (int)TileRowsLog2) - 1) >> (int)TileRowsLog2);
                i = 0;
                for ( uint startSb = 0; startSb < sbRows; startSb += tileHeightSb )
                {
                    /*MiRowStarts[i] = startSb << sbShift;*/
                    i += 1;
                }
                /*MiRowStarts[i]           = MiRows;*/
                fh.tile_info.TileRows = i;
            }
            else
            {
                uint widestTileSb = 0;
                uint startSb      = 0;
                uint i = 0, maxTileHeightSb = 0;
                for ( i = 0; startSb < sbCols; i++ )
                {
                    uint maxWidth = 0, sizeSb = 0;
                    /* MiColStarts[i] = startSb << sbShift; */
                    maxWidth = Math.Min(sbCols - startSb, maxTileWidthSb);
                    var re = AV1Bits.ObpNs(ref br, maxWidth, out uint width_in_sbs_minus_1, ref err);
                    if ( re < 0 )
                    {
                        //snprintf(err.error, err.size, "Couldn't read width_in_sbs_minus_1: %s", error.error);
                        return -1;
                    }
                    sizeSb = width_in_sbs_minus_1 + 1;
                    widestTileSb = Math.Max(sizeSb, widestTileSb);
                    startSb += sizeSb;
                }
                /*MiColStarts[i]         = MiCols;*/
                fh.tile_info.TileCols = (ushort) i;
                TileColsLog2 = AV1Bits.ObpTileLog2(1, fh.tile_info.TileCols);

                if ( minLog2Tiles > 0 )
                {
                    maxTileAreaSb = (sbRows * sbCols) >> (int) (minLog2Tiles + 1);
                }
                else
                {
                    maxTileAreaSb = sbRows * sbCols;
                }
                maxTileHeightSb = Math.Max(maxTileAreaSb / widestTileSb, 1);

                startSb = 0;
                for ( i = 0; startSb < sbRows; i++ )
                {
                    uint maxHeight = 0, sizeSb = 0;
                    /*MiRowStarts[i] = startSb << sbShift;*/
                    maxHeight = Math.Min(sbRows - startSb, maxTileHeightSb);
                    var re = AV1Bits.ObpNs(ref br, maxHeight, out uint height_in_sbs_minus_1, ref err);
                    if ( re < 0 )
                    {
                        return -1;
                    }
                    sizeSb = height_in_sbs_minus_1 + 1;
                    startSb += sizeSb;
                }
                /*MiRowStarts[i]          = MiRows*/;
                fh.tile_info.TileRows = (ushort) i;
                TileRowsLog2 = AV1Bits.ObpTileLog2(1, fh.tile_info.TileRows);
            }
            if ( TileColsLog2 > 0 || TileRowsLog2 > 0 )
            {
                ObpBr(out fh.tile_info.context_update_tile_id, ref br, (int) (TileColsLog2 + TileRowsLog2), ref err);
                ObpBr(out fh.tile_info.tile_size_bytes_minus_1, ref br, 2, ref err);
                /*TileSizeBytes = fh.tile_info.tile_size_bytes_minus_1 + 1;*/
            }
            else
            {
                fh.tile_info.context_update_tile_id = 0;
            }
            /* quantization_params() */
            ObpBr(out fh.quantization_params.base_q_idx, ref br, 8, ref err);
            int DeltaQYDc = 0, DeltaQUDc = 0, DeltaQUAc = 0, DeltaQVDc = 0, DeltaQVAc = 0;
            var qret = OBPReadDeltaQ(ref br, ref DeltaQYDc, ref err);
            if ( qret < 0 )
            {
                return -1;
            }
            if ( seq.color_config.NumPlanes > 1 )
            {
                if ( seq.color_config.separate_uv_delta_q )
                {
                    ObpBr(out fh.quantization_params.diff_uv_delta, ref br, 1, ref err);
                }
                else
                {
                    fh.quantization_params.diff_uv_delta = false;
                }
                qret = OBPReadDeltaQ(ref br, ref DeltaQUDc, ref err);
                if ( qret < 0 )
                {
                    return -1;
                }
                qret = OBPReadDeltaQ(ref br, ref DeltaQUAc, ref err);
                if ( qret < 0 )
                {
                    return -1;
                }
                if ( fh.quantization_params.diff_uv_delta )
                {
                    qret = OBPReadDeltaQ(ref br, ref DeltaQVDc, ref err);
                    if ( qret < 0 )
                    {
                        return -1;
                    }
                    qret = OBPReadDeltaQ(ref br, ref DeltaQVAc, ref err);
                    if ( qret < 0 )
                    {
                        return -1;
                    }
                }
                else
                {
                    DeltaQVDc = DeltaQUDc;
                    DeltaQVAc = DeltaQUAc;
                }
            }
            else
            {
                DeltaQUDc = 0;
                DeltaQUAc = 0;
                DeltaQVDc = 0;
                DeltaQVAc = 0;
            }
            ObpBr(out fh.quantization_params.using_qmatrix, ref br, 1, ref err);
            if ( fh.quantization_params.using_qmatrix )
            {
                ObpBr(out fh.quantization_params.qm_y, ref br, 4, ref err);
                ObpBr(out fh.quantization_params.qm_u, ref br, 4, ref err);
                if ( !seq.color_config.separate_uv_delta_q )
                {
                    fh.quantization_params.qm_v = fh.quantization_params.qm_u;
                }
                else
                {
                    ObpBr(out fh.quantization_params.qm_v, ref br, 4, ref err);
                }
            }
            /* segmentation_params() */
            byte[] Segmentation_Feature_Bits = [8, 6, 6, 6, 6, 3, 0, 0];
            byte[] Segmentation_Feature_Max  = [255, 63, 63, 63, 63, 7, 0, 0];
            int[] Segmentation_Feature_Signed  = [1, 1, 1, 1, 1, 0, 0, 0];
            ObpBr(out fh.segmentation_params.segmentation_enabled, ref br, 1, ref err);
            if ( fh.segmentation_params.segmentation_enabled )
            {
                if ( fh.primary_ref_frame == 7 )
                {
                    fh.segmentation_params.segmentation_update_map = true;
                    fh.segmentation_params.segmentation_temporal_update = 0;
                    fh.segmentation_params.segmentation_update_data = 1;
                }
                else
                {
                    ObpBr(out fh.segmentation_params.segmentation_update_map, ref br, 1, ref err);
                    if ( fh.segmentation_params.segmentation_update_map )
                    {
                        ObpBr(out fh.segmentation_params.segmentation_temporal_update, ref br, 1, ref err);
                    }
                    ObpBr(out fh.segmentation_params.segmentation_update_data, ref br, 1, ref err);
                }
                if ( fh.segmentation_params.segmentation_update_data == 1 )
                {
                    for ( int i = 0; i < 8; i++ )
                    {
                        for ( int j = 0; j < 8; j++ )
                        {
                            short clippedValue = 0;
                            short feature_value = 0;
                            ObpBr(out bool feature_enabled, ref br, 1, ref err);
                            FeatureEnabled[i, j] = feature_enabled;
                            clippedValue = 0;
                            if ( feature_enabled )
                            {
                                byte bitsToRead = Segmentation_Feature_Bits[j];
                                short limit      = Segmentation_Feature_Max[j];
                                if ( Segmentation_Feature_Signed[j] == 1 )
                                {
                                    var sret = AV1Bits.ObpSu(ref br, (uint)(1 + bitsToRead), out int val, ref err);
                                    if ( sret < 0 )
                                    {
                                        return -1;
                                    }
                                    feature_value = (sbyte) val;
                                    clippedValue = (short) Math.Max(-limit, Math.Min(limit, feature_value));
                                }
                                else
                                {
                                    ObpBr(out feature_value, ref br, bitsToRead, ref err);
                                    clippedValue = Math.Max((short) 0, Math.Min(limit, feature_value));
                                }
                            }
                            FeatureData[i, j] = clippedValue;
                        }
                    }
                }
            }
            else
            {
                for ( int i = 0; i < 8; i++ )
                {
                    for ( int j = 0; j > 8; j++ )
                    {
                        FeatureEnabled[i, j] = false;
                        FeatureData[i, j] = 0;
                    }
                }
            }
            /*int SegIdPreSkip    = 0;
            int LastActiveSegId = 0;
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (FeatureEnabled[i,j]) {
                        LastActiveSegId = i;
                        if (j >= 5) {
                            SegIdPreSkip = 1;
                        }
                    }
                }
            }*/
            /* delta_q_params() */
            fh.delta_q_params.delta_q_res = 0;
            fh.delta_q_params.delta_q_present = false;
            if ( fh.quantization_params.base_q_idx > 0 )
            {
                ObpBr(out fh.delta_q_params.delta_q_present, ref br, 1, ref err);
            }
            if ( fh.delta_q_params.delta_q_present )
            {
                ObpBr(out fh.delta_q_params.delta_q_res, ref br, 2, ref err);
            }
            /* delta_lf_params() */
            fh.delta_lf_params.delta_lf_present = false;
            fh.delta_lf_params.delta_lf_res = 0;
            fh.delta_lf_params.delta_lf_multi = false;
            if ( fh.delta_q_params.delta_q_present )
            {
                if ( !fh.allow_intrabc )
                {
                    ObpBr(out fh.delta_lf_params.delta_lf_present, ref br, 1, ref err);
                }
                if ( fh.delta_lf_params.delta_lf_present )
                {
                    ObpBr(out fh.delta_lf_params.delta_lf_res, ref br, 2, ref err);
                    ObpBr(out fh.delta_lf_params.delta_lf_multi, ref br, 1, ref err);
                }
            }
            /* skipped because not relevant:

               if (fh.primary_ref_frame == 7) {
                   init_coeff_cdfs();
               } else {
                   lnit_coeff_cdfs();
               }
             */
            bool CodedLossless = true;
            bool[] LosslessArray = new bool[8];
            for ( int segmentId = 0; segmentId < 8; segmentId++ )
            {
                byte qindex           = ObpGetQIndex(1, segmentId, 0, ref fh, ref FeatureEnabled, ref FeatureData);
                LosslessArray[segmentId] = (qindex == 0 && DeltaQYDc == 0 && DeltaQUAc == 0 && DeltaQUDc == 0 && DeltaQVAc == 0 && DeltaQVDc == 0);
                if ( !LosslessArray[segmentId] )
                {
                    CodedLossless = false;
                }
                /* SegQMLevel not relevant to OBU parsing.*/
            }
            bool AllLossless = (CodedLossless && (FrameWidth == UpscaledWidth));
            /* loop_filter_params() */
            if ( CodedLossless || fh.allow_intrabc )
            {
                fh.loop_filter_params.loop_filter_delta_enabled = true;
                fh.loop_filter_params.loop_filter_ref_deltas = [1, 0, 0, 0, 0, -1, -1, -1];
                for ( int i = 0; i < 2; i++ )
                {
                    fh.loop_filter_params.loop_filter_mode_deltas[i] = 0;
                }
                /* return */
            }
            else
            {
                ObpBr(out fh.loop_filter_params.loop_filter_level[0], ref br, 6, ref err);
                ObpBr(out fh.loop_filter_params.loop_filter_level[1], ref br, 6, ref err);
                if ( seq.color_config.NumPlanes > 1 )
                {
                    if ( fh.loop_filter_params.loop_filter_level[0] != 0 || fh.loop_filter_params.loop_filter_level[1] != 0 )
                    {
                        ObpBr(out fh.loop_filter_params.loop_filter_level[2], ref br, 6, ref err);
                        ObpBr(out fh.loop_filter_params.loop_filter_level[3], ref br, 6, ref err);
                    }
                }
                ObpBr(out fh.loop_filter_params.loop_filter_sharpness, ref br, 3, ref err);
                ObpBr(out fh.loop_filter_params.loop_filter_delta_enabled, ref br, 1, ref err);
                if ( fh.loop_filter_params.loop_filter_delta_enabled )
                {
                    ObpBr(out fh.loop_filter_params.loop_filter_delta_update, ref br, 1, ref err);
                    if ( fh.loop_filter_params.loop_filter_delta_update )
                    {
                        for ( int i = 0; i < 8; i++ )
                        {
                            bool update_ref_delta;
                            ObpBr(out update_ref_delta, ref br, 1, ref err);
                            if ( update_ref_delta )
                            {
                                var sqret = AV1Bits.ObpSu(ref br, 7, out int val, ref err);
                                if ( sqret < 0 )
                                {
                                    return -1;
                                }
                                fh.loop_filter_params.loop_filter_ref_deltas[i] = (sbyte) val;
                            }
                        }
                        for ( int i = 0; i < 2; i++ )
                        {
                            ObpBr(out bool update_mode_delta, ref br, 1, ref err);
                            if ( update_mode_delta )
                            {
                                var sqret = AV1Bits.ObpSu(ref br, 7, out int val, ref err);
                                if ( sqret < 0 )
                                {
                                    return -1;
                                }
                                fh.loop_filter_params.loop_filter_mode_deltas[i] = (sbyte) val;
                            }
                        }
                    }
                }
            }
            /* cdef_params() */
            if ( CodedLossless || fh.allow_intrabc || !seq.enable_cdef )
            {
                fh.cdef_params.cdef_bits = 0;
                fh.cdef_params.cdef_y_pri_strength[0] = 0;
                fh.cdef_params.cdef_y_sec_strength[0] = 0;
                fh.cdef_params.cdef_uv_pri_strength[0] = 0;
                fh.cdef_params.cdef_uv_sec_strength[0] = 0;
                /* CdefDamping not relevant to OBU parsing. */
                /* return */
            }
            else
            {
                ObpBr(out fh.cdef_params.cdef_damping_minus_3, ref br, 2, ref err);
                /* CdefDamping not relevant to OBU parsing. */
                ObpBr(out fh.cdef_params.cdef_bits, ref br, 2, ref err);
                for ( int i = 0; i < (1 << fh.cdef_params.cdef_bits); i++ )
                {
                    ObpBr(out fh.cdef_params.cdef_y_pri_strength[i], ref br, 4, ref err);
                    ObpBr(out fh.cdef_params.cdef_y_sec_strength[i], ref br, 2, ref err);
                    if ( fh.cdef_params.cdef_y_sec_strength[i] == 3 )
                    {
                        fh.cdef_params.cdef_y_sec_strength[i] += 1;
                    }
                    if ( seq.color_config.NumPlanes > 1 )
                    {
                        ObpBr(out fh.cdef_params.cdef_uv_pri_strength[i], ref br, 4, ref err);
                        ObpBr(out fh.cdef_params.cdef_uv_sec_strength[i], ref br, 2, ref err);
                        if ( fh.cdef_params.cdef_uv_sec_strength[i] == 3 )
                        {
                            fh.cdef_params.cdef_uv_sec_strength[i] += 1;
                        }
                    }
                }
            }
            if ( AllLossless || fh.allow_intrabc || !seq.enable_restoration )
            {
                fh.lr_params.lr_type[0] = 0;
                fh.lr_params.lr_type[1] = 0;
                fh.lr_params.lr_type[2] = 0;
            }
            else
            {
                bool UsesLr       = false;
                bool usesChromaLr = false;
                for ( int i = 0; i < seq.color_config.NumPlanes; i++ )
                {
                    ObpBr(out fh.lr_params.lr_type[i], ref br, 2, ref err);
                    if ( fh.lr_params.lr_type[i] != 0 )
                    {
                        UsesLr = true;
                        if ( i > 0 )
                        {
                            usesChromaLr = true;
                        }
                    }
                }
                if ( UsesLr )
                {
                    if ( seq.use_128x128_superblock )
                    {
                        ObpBr(out fh.lr_params.lr_unit_shift, ref br, 1, ref err);
                        fh.lr_params.lr_unit_shift++;
                    }
                    else
                    {
                        ObpBr(out fh.lr_params.lr_unit_shift, ref br, 1, ref err);
                        if ( fh.lr_params.lr_unit_shift > 0 )
                        {
                            byte lr_unit_extra_shift = 0;
                            ObpBr(out lr_unit_extra_shift, ref br, 1, ref err);
                            fh.lr_params.lr_unit_shift += lr_unit_extra_shift;
                        }
                    }
                    /* LoopRestorationSize not relevant to OBU parsing. */
                    if ( seq.color_config.subsampling_x && seq.color_config.subsampling_y && usesChromaLr )
                    {
                        ObpBr(out fh.lr_params.lr_uv_shift, ref br, 1, ref err);
                    }
                    else
                    {
                        fh.lr_params.lr_uv_shift = 0;
                    }
                    /* LoopRestorationSize not relevant to OBU parsing. */
                }
            }
            /* read_tx_mode */
            if ( CodedLossless )
            {
                /* TxMode not relevant to OBU parsing. */
            }
            else
            {
                ObpBr(out fh.tx_mode_select, ref br, 1, ref err);
                if ( fh.tx_mode_select )
                {
                    /* TxMode not relevant to OBU parsing. */
                }
                else
                {
                    /* TxMode not relevant to OBU parsing. */
                }
            }
            /* frame_reference_mode() */
            if ( FrameIsIntra )
            {
                fh.reference_select = false;
            }
            else
            {
                ObpBr(out fh.reference_select, ref br, 1, ref err);
            }
            /* skip_mode_params() */
            bool skipModeAllowed;
            if ( FrameIsIntra || !fh.reference_select || !seq.enable_order_hint )
            {
                skipModeAllowed = false;
            }
            else
            {
                int forwardIdx       = -1;
                int backwardIdx      = -1;
                int forwardHint  = 0; /* Never declare by spec! Bug? */
                int backwardHint = 0; /* Never declare by spec! Bug? */
                for ( int i = 0; i < 7; i++ )
                {
                    int refHint = context.RefOrderHint[fh.ref_frame_idx[i]];
                    if ( AV1Bits.ObpGetRelativeDist(refHint, OrderHint, seq) < 0 )
                    {
                        if ( forwardIdx < 0 || AV1Bits.ObpGetRelativeDist(refHint, forwardHint, seq) > 0 )
                        {
                            forwardIdx = i;
                            forwardHint = refHint;
                        }
                    }
                    else if ( AV1Bits.ObpGetRelativeDist(refHint, OrderHint, seq) > 0 )
                    {
                        if ( backwardIdx < 0 || AV1Bits.ObpGetRelativeDist(refHint, backwardHint, seq) < 0 )
                        {
                            backwardIdx = i;
                            backwardHint = refHint;
                        }
                    }
                }
                if ( forwardIdx < 0 )
                {
                    skipModeAllowed = false;
                }
                else if ( backwardIdx >= 0 )
                {
                    skipModeAllowed = true;
                    /* SkipModeFrame not relevant to OBU parsing. */
                }
                else
                {
                    int     secondForwardIdx = -1;
                    int secondForwardHint = 0; /* Never declare by spec! Bug? */
                    for ( int i = 0; i < 7; i++ )
                    {
                        int refHint = context.RefOrderHint[fh.ref_frame_idx[i]];
                        if ( AV1Bits.ObpGetRelativeDist(refHint, forwardHint, seq) < 0 )
                        {
                            if ( secondForwardIdx < 0 || AV1Bits.ObpGetRelativeDist(refHint, secondForwardHint, seq) > 0 )
                            {
                                secondForwardIdx = i;
                                secondForwardHint = refHint;
                            }
                        }
                    }
                    if ( secondForwardIdx < 0 )
                    {
                        skipModeAllowed = false;
                    }
                    else
                    {
                        skipModeAllowed = true;
                        /* SkipModeFrame not relevant to OBU parsing. */
                    }
                }
            }
            if ( skipModeAllowed )
            {
                ObpBr(out fh.skip_mode_present, ref br, 1, ref err);
            }
            else
            {
                fh.skip_mode_present = false;
            }
            if ( FrameIsIntra || fh.error_resilient_mode || !seq.enable_warped_motion )
            {
                fh.allow_warped_motion = false;
            }
            else
            {
                ObpBr(out fh.allow_warped_motion, ref br, 1, ref err);
            }
            ObpBr(out fh.reduced_tx_set, ref br, 1, ref err);
            /* global_motion_params() */
            for ( int refs = 1; refs < 7; refs++ )
            {
                fh.global_motion_params.gm_type[refs] = 0;
                for ( int i = 0; i < 6; i++ )
                {
                    fh.global_motion_params.gm_params[refs, i] = (int) ((i % 3 == 2) ? (((uint) 1) << 16) : 0);
                }
            }
            if ( FrameIsIntra )
            {
                /* return */
            }
            else
            {
                for ( int refs = 1; refs <= 7; refs++ )
                {
                    byte type = 0;
                    ObpBr(out bool is_global, ref br, 1, ref err);
                    if ( is_global )
                    {
                        ObpBr(out bool is_rot_zoom, ref br, 1, ref err);
                        if ( is_rot_zoom )
                        {
                            type = 2;
                        }
                        else
                        {
                            ObpBr(out bool is_translation, ref br, 1, ref err);
                            type = (byte) (is_translation ? 1 : 3);
                        }
                    }
                    else
                    {
                        type = 0;
                    }
                    fh.global_motion_params.gm_type[refs] = type;

                    if ( type >= 2 )
                    {
                        var gret = OBPReadGlobalParam(ref br, ref fh, type, refs, 2, ref err);
                        if ( gret < 0 )
                        {
                            return -1;
                        }
                        gret = OBPReadGlobalParam(ref br, ref fh, type, refs, 3, ref err);
                        if ( gret < 0 )
                        {
                            return -1;
                        }
                        if ( type == 3 )
                        {
                            gret = OBPReadGlobalParam(ref br, ref fh, type, refs, 4, ref err);
                            if ( gret < 0 )
                            {
                                return -1;
                            }
                            gret = OBPReadGlobalParam(ref br, ref fh, type, refs, 5, ref err);
                            if ( gret < 0 )
                            {
                                return -1;
                            }
                        }
                        else
                        {
                            fh.global_motion_params.gm_params[refs, 4] = -fh.global_motion_params.gm_params[refs, 3];
                            fh.global_motion_params.gm_params[refs, 5] = fh.global_motion_params.gm_params[refs, 2];
                        }
                    }
                    if ( type >= 1 )
                    {
                        var gret = OBPReadGlobalParam(ref br, ref fh, type, refs, 0, ref err);
                        if ( gret < 0 )
                        {
                            return -1;
                        }
                        gret = OBPReadGlobalParam(ref br, ref fh, type, refs, 1, ref err);
                        if ( gret < 0 )
                        {
                            return -1;
                        }
                    }
                }
            }
            /* film_grain_params() */
            if ( !seq.film_grain_params_present || (!fh.show_frame && !fh.showable_frame) )
            {
                /* reset_grain_params() */
                fh.film_grain_params = new OBUFilmGrainParameters();
                /* return */
            }
            else
            {
                ObpBr(out fh.film_grain_params.apply_grain, ref br, 1, ref err);
                if ( !fh.film_grain_params.apply_grain )
                {
                    /* reset_grain_params() */
                    fh.film_grain_params = new OBUFilmGrainParameters();
                    /* return */
                }
                else
                {
                    ObpBr(out fh.film_grain_params.grain_seed, ref br, 16, ref err);
                    if ( fh.frame_type == OBUFrameType.OBU_INTER_FRAME )
                    {
                        ObpBr(out fh.film_grain_params.update_grain, ref br, 1, ref err);
                    }
                    else
                    {
                        fh.film_grain_params.update_grain = true;
                    }
                    if ( !fh.film_grain_params.update_grain )
                    {
                        ObpBr(out fh.film_grain_params.film_grain_params_ref_idx, ref br, 3, ref err);
                        ushort tempGrainSeed = fh.film_grain_params.grain_seed;
                        /* load_grain_params() */
                        fh.film_grain_params = context.RefGrainParams[fh.film_grain_params.film_grain_params_ref_idx];
                        fh.film_grain_params.grain_seed = tempGrainSeed;
                        /* return */
                    }
                    else
                    {
                        byte numPosLuma = 0, numPosChroma = 0;
                        ObpBr(out fh.film_grain_params.num_y_points, ref br, 4, ref err);
                        for ( byte i = 0; i < fh.film_grain_params.num_y_points; i++ )
                        {
                            ObpBr(out fh.film_grain_params.point_y_value[i], ref br, 8, ref err);
                            ObpBr(out fh.film_grain_params.point_y_scaling[i], ref br, 8, ref err);
                        }
                        if ( seq.color_config.mono_chrome )
                        {
                            fh.film_grain_params.chroma_scaling_from_luma = false;
                        }
                        else
                        {
                            ObpBr(out fh.film_grain_params.chroma_scaling_from_luma, ref br, 1, ref err);
                        }
                        if ( seq.color_config.mono_chrome || fh.film_grain_params.chroma_scaling_from_luma ||
                            (seq.color_config.subsampling_x && seq.color_config.subsampling_y &&
                             fh.film_grain_params.num_y_points == 0) )
                        {
                            fh.film_grain_params.num_cb_points = 0;
                            fh.film_grain_params.num_cr_points = 0;
                        }
                        else
                        {
                            ObpBr(out fh.film_grain_params.num_cb_points, ref br, 4, ref err);
                            for ( byte i = 0; i < fh.film_grain_params.num_cb_points; i++ )
                            {
                                ObpBr(out fh.film_grain_params.point_cb_value[i], ref br, 8, ref err);
                                ObpBr(out fh.film_grain_params.point_cb_scaling[i], ref br, 8, ref err);
                            }
                            ObpBr(out fh.film_grain_params.num_cr_points, ref br, 4, ref err);
                            for ( byte i = 0; i < fh.film_grain_params.num_cr_points; i++ )
                            {
                                ObpBr(out fh.film_grain_params.point_cr_value[i], ref br, 8, ref err);
                                ObpBr(out fh.film_grain_params.point_cr_scaling[i], ref br, 8, ref err);
                            }
                        }
                        ObpBr(out fh.film_grain_params.grain_scaling_minus_8, ref br, 2, ref err);
                        ObpBr(out fh.film_grain_params.ar_coeff_lag, ref br, 2, ref err);
                        numPosLuma = (byte) (2 * fh.film_grain_params.ar_coeff_lag * (fh.film_grain_params.ar_coeff_lag + 1));
                        if ( fh.film_grain_params.num_y_points != 0 )
                        {
                            numPosChroma = (byte) (numPosLuma + 1);
                            for ( byte i = 0; i < numPosLuma; i++ )
                            {
                                ObpBr(out fh.film_grain_params.ar_coeffs_y_plus_128[i], ref br, 8, ref err);
                            }
                        }
                        else
                        {
                            numPosChroma = numPosLuma;
                        }
                        if ( fh.film_grain_params.chroma_scaling_from_luma || fh.film_grain_params.num_cb_points != 0 )
                        {
                            for ( byte i = 0; i < numPosChroma; i++ )
                            {
                                ObpBr(out fh.film_grain_params.ar_coeffs_cb_plus_128[i], ref br, 8, ref err);
                            }
                        }
                        if ( fh.film_grain_params.chroma_scaling_from_luma || fh.film_grain_params.num_cr_points != 0 )
                        {
                            for ( byte i = 0; i < numPosChroma; i++ )
                            {
                                ObpBr(out fh.film_grain_params.ar_coeffs_cr_plus_128[i], ref br, 8, ref err);
                            }
                        }
                        ObpBr(out fh.film_grain_params.ar_coeff_shift_minus_6, ref br, 2, ref err);
                        ObpBr(out fh.film_grain_params.grain_scale_shift, ref br, 2, ref err);
                        if ( fh.film_grain_params.num_cb_points != 0 )
                        {
                            ObpBr(out fh.film_grain_params.cb_mult, ref br, 8, ref err);
                            ObpBr(out fh.film_grain_params.cb_luma_mult, ref br, 8, ref err);
                            ObpBr(out fh.film_grain_params.cb_offset, ref br, 9, ref err);
                        }
                        if ( fh.film_grain_params.num_cr_points != 0 )
                        {
                            ObpBr(out fh.film_grain_params.cr_mult, ref br, 8, ref err);
                            ObpBr(out fh.film_grain_params.cr_luma_mult, ref br, 8, ref err);
                            ObpBr(out fh.film_grain_params.cr_offset, ref br, 9, ref err);
                        }
                        ObpBr(out fh.film_grain_params.overlap_flag, ref br, 1, ref err);
                        ObpBr(out fh.film_grain_params.clip_to_restricted_range, ref br, 1, ref err);
                    }
                }
            }

            /* Stash refs for future frame use. */
            /* decode_frame_wrapup() */
            for ( int i = 0; i < 8; i++ )
            {
                if ( ((fh.refresh_frame_flags >> i) & 1) == 1 )
                {
                    context.RefOrderHint[i] = fh.order_hint;
                    context.RefFrameType[i] = fh.frame_type;
                    context.RefUpscaledWidth[i] = UpscaledWidth;
                    context.RefFrameHeight[i] = FrameHeight;
                    context.RefRenderWidth[i] = fh.RenderWidth;
                    context.RefRenderHeight[i] = fh.RenderHeight;
                    context.RefFrameId[i] = (byte) fh.current_frame_id;
                    context.RefGrainParams[i] = fh.film_grain_params;
                    /* save_grain_params() */
                    for ( int j = 0; j < 8; j++ )
                    {
                        for ( int k = 0; k < 6; k++ )
                        {
                            context.SavedGmParams[i, j, k] = (uint) fh.global_motion_params.gm_params[j, k];
                        }
                    }
                    /* save_segmentation_params() */
                    for ( int j = 0; j < 8; j++ )
                    {
                        for ( int k = 0; k < 8; k++ )
                        {
                            context.SavedFeatureEnabled[i, j, k] = FeatureEnabled[j, k];
                            context.SavedFeatureData[i, j, k] = FeatureData[j, k];
                        }
                    }
                    /* save_loop_filter_params() */
                    for ( int j = 0; j < 8; j++ )
                    {
                        context.SavedLoopFilterRefDeltas[i, j] = fh.loop_filter_params.loop_filter_ref_deltas[j];
                        context.SavedLoopFilterModeDeltas[i, j] = fh.loop_filter_params.loop_filter_mode_deltas[j];
                    }
                }
            }

            /* Handle show_existing_frame semantics. */
            /* decode_frame_wrapup() */
            if ( fh.show_existing_frame && fh.frame_type == OBUFrameType.OBU_KEY_FRAME )
            {
                fh.order_hint = context.RefOrderHint[fh.frame_to_show_map_idx];
                for ( int i = 0; i < 8; i++ )
                {
                    for ( int j = 0; j < 6; j++ )
                    {
                        fh.global_motion_params.gm_params[i, j] = (int) context.SavedGmParams[fh.frame_to_show_map_idx, i, j];
                    }
                }
            }
            if ( fh.show_existing_frame )
            {
                context.SeenFrameHeader = false;
                context.prev_filled = false;
            }
            else
            {
                context.prevFrameHeader = fh;
                context.prev_filled = true;
            }

            /* Stash byte position for use in OBU_FRAME parsing. */
            AV1Bits.ObpBrByteAligment(ref br);
            context.frame_header_end_pos = AV1Bits.ObpBrGetPos(ref br);

            return 0;
        }
    }
#pragma warning restore IDE0059 // 不需要赋值
#pragma warning restore IDE0018 // 内联变量声明
}