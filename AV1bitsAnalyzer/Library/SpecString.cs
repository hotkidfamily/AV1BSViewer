using System.Text;

namespace AV1bitsAnalyzer.Library
{
    internal class SpecString
    {
        public static string[] ToMetadataStrings (OBUMetadata v)
        {
            string[] strings = [
                "MetaData",
                $"metadata_type = ${v.metadata_type}",
                $"metadata_hdr_cll = ${v.metadata_hdr_cll}",
                $"metadata_hdr_mdcv = ${v.metadata_hdr_mdcv}",
                $"metadata_scalability = ${v.metadata_scalability}",
                $"metadata_itut_t35 = ${v.metadata_itut_t35}",
                $"metadata_timecode = ${v.metadata_timecode}",
                $"unregistered = ${v.unregistered}",
            ];
            return strings;
        }

        public static string ToFrameHeaderString(OBUFrameHeader v, OBUSequenceHeader h)
        {
            StringBuilder sb = new();

            bool FrameIsIntra = v.frame_type == OBUFrameType.OBU_INTRA_ONLY_FRAME || v.frame_type == OBUFrameType.OBU_KEY_FRAME;

            sb.Append($"FrameHeader\r\n");

            sb.Append($" |- show_existing_frame = {v.show_existing_frame}\r\n");
           // if ( v.show_existing_frame )
            {
                sb.Append($"  |- frame_to_show_map_idx = {v.frame_to_show_map_idx}\r\n");
            }
            if ( h.decoder_model_info_present_flag && !h.timing_info.equal_picture_interval )
            {
                //sb.Append($"if( h.decoder_model_info_present_flag && !h.timing_info.equal_picture_interval)\r\n");
                sb.Append($" |- temporal_point_info\r\n");
                sb.Append($"     - ( {v.temporal_point_info.ToString()} )\r\n");
            }
            if ( h.frame_id_numbers_present_flag )
            {
                sb.Append($" |- display_frame_id = {v.display_frame_id}\r\n");
            }
            sb.Append($" |- frame_type = {v.frame_type}\r\n");
            sb.Append($" |- show_frame = {v.show_frame}\r\n");
            sb.Append($" |- showable_frame = {v.showable_frame}\r\n");
            bool error_resilient_mode = v.error_resilient_mode;
            if ( v.frame_type == OBUFrameType.OBU_SWITCH_FRAME || (v.frame_type == OBUFrameType.OBU_KEY_FRAME && v.show_frame) )
                error_resilient_mode = true;
            sb.Append($" |- error_resilient_mode = {error_resilient_mode}\r\n");
            sb.Append($" |- disable_cdf_update = {v.disable_cdf_update}\r\n");
            sb.Append($" |- allow_screen_content_tools = {v.allow_screen_content_tools}\r\n");
            if ( FrameIsIntra )
                sb.Append($" |- force_integer_mv = {v.force_integer_mv}\r\n");
            sb.Append($" |- current_frame_id = {v.current_frame_id}\r\n");
            sb.Append($" |- frame_size_override_flag = {v.frame_size_override_flag}\r\n");
            sb.Append($" |- order_hint = {v.order_hint}\r\n");

            var primary_ref_frame = v.primary_ref_frame;
            if ( FrameIsIntra || v.error_resilient_mode )
            {
                primary_ref_frame = 0;
            }
            sb.Append($" |- primary_ref_frame = {primary_ref_frame}\r\n");

            if ( h.decoder_model_info_present_flag )
            {
                sb.Append($" |- buffer_removal_time_present_flag = {v.buffer_removal_time_present_flag}\r\n");
                if ( v.buffer_removal_time_present_flag )
                {
                    sb.Append($"    - buffer_removal_time  = {TypeToString.UintArray(v.buffer_removal_time)}\r\n");
                }
            }

            sb.Append($" |- refresh_frame_flags = {v.refresh_frame_flags}\r\n");
            sb.Append($" |- ref_order_hint = {TypeToString.ByteArray(v.ref_order_hint)}\r\n");
            sb.Append($" |- frame_width_minus_1 = {v.frame_width_minus_1}\r\n");
            sb.Append($" |- frame_height_minus_1 = {v.frame_height_minus_1}\r\n");
            sb.Append($" |- superres_params = {v.superres_params}\r\n");
            sb.Append($" |- render_and_frame_size_different = {v.render_and_frame_size_different}\r\n");
            sb.Append($" |- render_width_minus_1 = {v.render_width_minus_1}\r\n");
            sb.Append($" |- render_height_minus_1 = {v.render_height_minus_1}\r\n");
            sb.Append($" |- RenderWidth = {v.RenderWidth}\r\n");
            sb.Append($" |- RenderHeight = {v.RenderHeight}\r\n");
            if ( FrameIsIntra )
            {
                sb.Append($" |- allow_intrabc = {v.allow_intrabc}\r\n");
            }
            else
            {
                if ( !h.enable_order_hint )
                {
                    sb.Append($" |- frame_refs_short_signaling = 0\r\n");
                }
                else
                {
                    sb.Append($" |- frame_refs_short_signaling = {v.frame_refs_short_signaling}\r\n");
                    if ( v.frame_refs_short_signaling )
                    {
                        sb.Append($" |- last_frame_idx = {v.last_frame_idx}\r\n");
                        sb.Append($" L gold_frame_idx = {v.gold_frame_idx}\r\n");
                    }

                    sb.Append($" |- ref_frame_idx = {TypeToString.ByteArray(v.ref_frame_idx)}\r\n");
                    sb.Append($" |- delta_frame_id_minus_1 = {TypeToString.ByteArray(v.delta_frame_id_minus_1)}\r\n");
                }
            }
            sb.Append($" |- found_ref = {v.found_ref}\r\n");
            sb.Append($" |- allow_high_precision_mv = {v.allow_high_precision_mv}\r\n");
            sb.Append($" |- interpolation_filter = {v.interpolation_filter}\r\n");
            sb.Append($" |- is_motion_mode_switchable = {v.is_motion_mode_switchable}\r\n");
            sb.Append($" |- use_ref_frame_mvs = {v.use_ref_frame_mvs}\r\n");
            sb.Append($" |- disable_frame_end_update_cdf = {v.disable_frame_end_update_cdf}\r\n");
            sb.Append($" |- tile_info = {v.tile_info}\r\n");
            sb.Append($" |- quantization_params = {v.quantization_params}\r\n");
            sb.Append($" |- segmentation_params = {v.segmentation_params}\r\n");
            sb.Append($" |- delta_q_params = {v.delta_q_params}\r\n");
            sb.Append($" |- delta_lf_params = {v.delta_lf_params}\r\n");
            sb.Append($" |- loop_filter_params = {v.loop_filter_params}\r\n");
            sb.Append($" |- cdef_params = {v.cdef_params}\r\n");
            sb.Append($" |- lr_params = {v.lr_params}\r\n");
            sb.Append($" |- tx_mode_select = {v.tx_mode_select}\r\n");
            sb.Append($" |- skip_mode_present = {v.skip_mode_present}\r\n");
            sb.Append($" |- reference_select = {v.reference_select}\r\n");
            sb.Append($" |- allow_warped_motion = {v.allow_warped_motion}\r\n");
            sb.Append($" |- reduced_tx_set = {v.reduced_tx_set}\r\n");
            sb.Append($" |- global_motion_params = {v.global_motion_params}\r\n");
            sb.Append($" |- film_grain_params = {v.film_grain_params}\r\n");

            return sb.ToString();
        }

        public static string ToSeqHeaderString (OBUSequenceHeader v)
        {
            StringBuilder sb = new();

            sb.Append($"SequenceHeader\r\n");
            sb.Append($" |- seq_profile = {v.seq_profile}\r\n");
            sb.Append($" |- still_picture = {v.still_picture}\r\n");
            sb.Append($" |- reduced_still_picture_v = {v.reduced_still_picture_header}\r\n");
            if(v.reduced_still_picture_header)
            {
                sb.Append($" |- timing_info_present_flag = {v.timing_info_present_flag}\r\n");
                sb.Append($" |- decoder_model_info_present_flag = {v.decoder_model_info_present_flag}\r\n");
                sb.Append($" |- initial_display_delay_present_flag = {v.initial_display_delay_present_flag}\r\n");
                sb.Append($" |- operating_points_cnt_minus_1 = {v.operating_points_cnt_minus_1}\r\n");
                sb.Append($" |- operating_point_idc = {v.operating_point_idc[0]}\r\n");
                sb.Append($" |- seq_level_idx = {v.seq_level_idx[0]}\r\n");
                sb.Append($" |- seq_level_idx = {v.seq_level_idx[0]}\r\n");
                sb.Append($" |- seq_tier = {v.seq_tier[0]}\r\n");
                sb.Append($" |- decoder_model_present_for_this_op = {v.decoder_model_present_for_this_op[0]}\r\n");
                sb.Append($" |- initial_display_delay_present_for_this_op = {v.initial_display_delay_present_for_this_op[0]}\r\n");
            }
            else
            {
                sb.Append($" |- timing_info_present_flag = {v.timing_info_present_flag}\r\n");
                if ( v.timing_info_present_flag )
                {
                    sb.Append($"   - timing_info = ( {v.timing_info.ToString()} )\r\n");
                }

                sb.Append($" |- decoder_model_info_present_flag = {v.decoder_model_info_present_flag}\r\n");
                if ( v.decoder_model_info_present_flag )
                    sb.Append($"   - decoder_model_info = ( {v.decoder_model_info.ToString()} )\r\n");

                sb.Append($" |- initial_display_delay_present_flag = {v.initial_display_delay_present_flag}\r\n");
                sb.Append($" |- operating_points_cnt_minus_1 = {v.operating_points_cnt_minus_1}\r\n");
                for(var i= 0; i <= v.operating_points_cnt_minus_1; i++ )
                {
                    sb.Append($" |- operating_point_idc = {v.operating_point_idc[i]}\r\n");
                    var level = v.seq_level_idx[i];
                    sb.Append($" |- seq_level_idx = {level}\r\n");
                    bool seq_tier = v.seq_tier[i];
                    if (level < 7 )
                    {
                        seq_tier = false;
                    }
                    sb.Append($" |- seq_tier = {seq_tier}\r\n");
                    if ( v.decoder_model_info_present_flag )
                    {
                        sb.Append($" |- decoder_model_present_for_this_op = {v.decoder_model_present_for_this_op[i]}\r\n");
                        if ( v.decoder_model_present_for_this_op[i] )
                        {
                            sb.Append($"   - operating_parameters_info = ( {v.operating_parameters_info[i]} )\r\n");
                        }
                    }
                    if ( v.initial_display_delay_present_flag )
                    {
                        sb.Append($" |- initial_display_delay_present_for_this_op = {v.initial_display_delay_present_for_this_op[i]}\r\n");
                        if ( v.initial_display_delay_present_for_this_op[i] )
                        {
                            sb.Append($" |- initial_display_delay_minus_1 = {v.initial_display_delay_minus_1[i]}\r\n");
                        }
                    }
                }

                sb.Append($" |- frame_width_bits_minus_1 = {v.frame_width_bits_minus_1}\r\n");
                sb.Append($" |- frame_height_bits_minus_1 = {v.frame_height_bits_minus_1}\r\n");
                sb.Append($" |- max_frame_width_minus_1 = {v.max_frame_width_minus_1}\r\n");
                sb.Append($" |- max_frame_height_minus_1 = {v.max_frame_height_minus_1}\r\n");
                sb.Append($" |- frame_id_numbers_present_flag = {v.frame_id_numbers_present_flag}\r\n");
                if( v.frame_id_numbers_present_flag )
                {
                    sb.Append($"    - delta_frame_id_length_minus_2 = {v.delta_frame_id_length_minus_2}\r\n");
                    sb.Append($"    - additional_frame_id_length_minus_1 = {v.additional_frame_id_length_minus_1}\r\n");
                }
                sb.Append($" |- use_128x128_superblock = {v.use_128x128_superblock}\r\n");
                sb.Append($" |- enable_filter_intra = {v.enable_filter_intra}\r\n");
                sb.Append($" |- enable_intra_edge_filter = {v.enable_intra_edge_filter}\r\n");
                if( v.reduced_still_picture_header )
                {
                    sb.Append($" |- enable_interintra_compound = {v.enable_interintra_compound}\r\n");
                    sb.Append($" |- enable_masked_compound = {v.enable_masked_compound}\r\n");
                    sb.Append($" |- enable_warped_motion = {v.enable_warped_motion}\r\n");
                    sb.Append($" |- enable_dual_filter = {v.enable_dual_filter}\r\n");
                    sb.Append($" |- enable_order_hint = {v.enable_order_hint}\r\n");
                    sb.Append($" |- enable_jnt_comp = {v.enable_jnt_comp}\r\n");
                    sb.Append($" |- enable_ref_frame_mvs = {v.enable_ref_frame_mvs}\r\n");
                    sb.Append($" |- seq_force_screen_content_tools = {v.seq_force_screen_content_tools}\r\n");
                    sb.Append($" |- seq_force_integer_mv = {v.seq_force_integer_mv}\r\n");
                    sb.Append($" |- OrderHintBits = {v.OrderHintBits}\r\n");
                }
                else
                {
                    sb.Append($" |- enable_interintra_compound = {v.enable_interintra_compound}\r\n");
                    sb.Append($" |- enable_masked_compound = {v.enable_masked_compound}\r\n");
                    sb.Append($" |- enable_warped_motion = {v.enable_warped_motion}\r\n");
                    sb.Append($" |- enable_dual_filter = {v.enable_dual_filter}\r\n");
                    sb.Append($" |- enable_order_hint = {v.enable_order_hint}\r\n");
                    sb.Append($"   - enable_jnt_comp = {v.enable_jnt_comp}\r\n");
                    sb.Append($"   - enable_ref_frame_mvs = {v.enable_ref_frame_mvs}\r\n");
                    sb.Append($" |- seq_choose_screen_content_tools = {v.seq_choose_screen_content_tools}\r\n");

                    if( v.seq_choose_screen_content_tools > 0 )
                    {
                        sb.Append($"   - seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS\r\n");
                    }
                    else if ( v.seq_force_screen_content_tools > 0 )
                    {
                        sb.Append($"   - seq_force_screen_content_tools = {v.seq_force_screen_content_tools}\r\n");
                        sb.Append($"      - seq_choose_integer_mv = {v.seq_choose_integer_mv}\r\n");
                        if ( v.seq_choose_integer_mv > 0 )
                        {
                            sb.Append($"        - seq_force_integer_mv = SELECT_INTEGER_MV\r\n");
                        }
                        else
                        {
                            sb.Append($"        - seq_force_integer_mv = {v.seq_force_integer_mv}\r\n");
                        }
                    }

                    if ( v.enable_order_hint )
                    {
                        sb.Append($"   - order_hint_bits_minus_1 = {v.order_hint_bits_minus_1}\r\n");
                        sb.Append($"   - OrderHintBits = {v.OrderHintBits}\r\n");
                    }
                }
                sb.Append($" |- enable_superres = {v.enable_superres}\r\n");
                sb.Append($" |- enable_cdef = {v.enable_cdef}\r\n");
                sb.Append($" |- enable_restoration = {v.enable_restoration}\r\n");
                sb.Append($" |- color_config = {v.color_config.ToString()}\r\n");
                sb.Append($" |_ film_grain_params_present = {v.film_grain_params_present}\r\n");
            }

            return sb.ToString();
        }
    }
}