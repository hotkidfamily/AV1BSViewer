using System.Diagnostics;
using System.Text;

namespace AV1bitsAnalyzer.Library
{
    using SpecTree = Queue<STItem>;
    /*********************************************
     * Various enums from the AV1 specification. *
     *********************************************/

#pragma warning disable CA1069 // 不应复制枚举值
    public enum OBULimited
    {
        REFS_PER_FRAME     = 7,      /* Number of reference frames that can be used for inter prediction */
        TOTAL_REFS_PER_FRAME = 8,      /* Number of reference frame types (including intra type) */
        BLOCK_SIZE_GROUPS    = 4,      /* Number of contexts when decoding y_mode */
        BLOCK_SIZES          = 22,     /* Number of different block sizes used */
        BLOCK_INVALID        = 22,     /* Sentinel value to mark partition choices that are not allowed */
        MAX_SB_SIZE          = 128,    /* Maximum size of a superblock in luma samples */
        MI_SIZE              = 4,      /* Smallest size of a mode info block in luma samples */
        MI_SIZE_LOG2         = 2,      /* Base 2 logarithm of smallest size of a mode info block */
        MAX_TILE_WIDTH       = 4096,   /* Maximum width of a tile in units of luma samples */
        MAX_TILE_AREA        = 4096 * 2304,  /* Maximum area of a tile in units of luma samples */
        MAX_TILE_ROWS        = 64,     /* Maximum number of tile rows */
        MAX_TILE_COLS        = 64,     /* Maximum number of tile columns */
        INTRABC_DELAY_PIXELS = 256,    /* Number of horizontal luma samples before intra block copy can be used */
        INTRABC_DELAY_SB64   = 4,      /* Number of 64 by 64 blocks before intra block copy can be used */
        NUM_REF_FRAMES       = 8,      /* Number of frames that can be stored for future reference */
        IS_INTER_CONTEXTS    = 4,      /* Number of contexts for is_inter */
        REF_CONTEXTS         = 3,      /* Number of contexts for single_ref, comp_ref, comp_bwdref, uni_comp_ref, uni_comp_ref_p1, and uni_comp_ref_p2 */
        MAX_SEGMENTS         = 8,      /* Number of segments allowed in segmentation map */
        SEGMENT_ID_CONTEXTS  = 3,      /* Number of contexts for segment_id */
        SEG_LVL_ALT_Q        = 0,      /* Index for quantizer segment feature */
        SEG_LVL_ALT_LF_Y_V   = 1,      /* Index for vertical luma loop filter segment feature */
        SEG_LVL_REF_FRAME    = 5,      /* Index for reference frame segment feature */
        SEG_LVL_SKIP         = 6,      /* Index for skip segment feature */
        SEG_LVL_GLOBALMV     = 7,      /* Index for global mv feature */
        SEG_LVL_MAX          = 8,      /* Number of segment features */
        PLANE_TYPES          = 2,      /* Number of different plane types (luma or chroma) */
        TX_SIZE_CONTEXTS     = 3,      /* Number of contexts for transform size */
        INTERP_FILTERS       = 3,      /* Number of values for interp_filter */
        INTERP_FILTER_CONTEXTS = 16,    /* Number of contexts for interp_filter */
        SKIP_MODE_CONTEXTS   = 3,      /* Number of contexts for decoding skip_mode */
        SKIP_CONTEXTS        = 3,      /* Number of contexts for decoding skip */
        PARTITION_CONTEXTS   = 4,      /* Number of contexts when decoding partition */
        TX_SIZES             = 5,      /* Number of square transform sizes */
        TX_SIZES_ALL         = 19,     /* Number of transform sizes (including non-square sizes) */
        TX_MODES             = 3,      /* Number of values for tx_mode */
        DCT_DCT              = 0,      /* Inverse transform rows with DCT and columns with DCT */
        ADST_DCT         = 1,      /* Inverse transform rows with DCT and columns with ADST */
        DCT_ADST         = 2,      /* Inverse transform rows with ADST and columns with DCT */
        ADST_ADST        = 3,      /* Inverse transform rows with ADST and columns with ADST */
        FLIPADST_DCT     = 4,      /* Inverse transform rows with DCT and columns with FLIPADST */
        DCT_FLIPADST     = 5,      /* Inverse transform rows with FLIPADST and columns with DCT */
        FLIPADST_FLIPADST = 6,      /* Inverse transform rows with FLIPADST and columns with FLIPADST */
        ADST_FLIPADST    = 7,      /* Inverse transform rows with FLIPADST and columns with ADST */
        FLIPADST_ADST    = 8,      /* Inverse transform rows with ADST and columns with FLIPADST */
        IDTX             = 9,      /* Inverse transform rows with identity and columns with identity */
        V_DCT            = 10,     /* Inverse transform rows with identity and columns with DCT */
        H_DCT            = 11,     /* Inverse transform rows with DCT and columns with identity */
        V_ADST           = 12,     /* Inverse transform rows with identity and columns with ADST */
        H_ADST           = 13,     /* Inverse transform rows with ADST and columns with identity */
        V_FLIPADST       = 14,     /* Inverse transform rows with identity and columns with FLIPADST */
        H_FLIPADST       = 15,     /* Inverse transform rows with FLIPADST and columns with identity */
        TX_TYPES         = 16,     /* Number of inverse transform types */
        MB_MODE_COUNT    = 17,     /* Number of values for YMode */
        INTRA_MODES      = 13,     /* Number of values for y_mode */
        UV_INTRA_MODES_CFL_NOT_ALLOWED = 13,     /* Number of values for uv_mode when chroma from luma is not allowed */
        UV_INTRA_MODES_CFL_ALLOWED = 14,     /* Number of values for uv_mode when chroma from luma is allowed */
        COMPOUND_MODES             = 8,      /* Number of values for compound_mode */
        COMPOUND_MODE_CONTEXTS     = 8,      /* Number of contexts for compound_mode */
        COMP_NEWMV_CTXS            = 5,      /* Number of new mv values used when constructing context for compound_mode */
        NEW_MV_CONTEXTS            = 6,      /* Number of contexts for new_mv */
        ZERO_MV_CONTEXTS           = 2,      /* Number of contexts for zero_mv */
        REF_MV_CONTEXTS            = 6,      /* Number of contexts for ref_mv */
        DRL_MODE_CONTEXTS          = 3,      /* Number of contexts for drl_mode */
        MV_CONTEXTS                = 2,      /* Number of contexts for decoding motion vectors including one for intra block copy */
        MV_INTRABC_CONTEXT         = 1,      /* Motion vector context used for intra block copy */
        MV_JOINTS                  = 4,      /* Number of values for mv_joint */
        MV_CLASSES                 = 11,     /* Number of values for mv_class */
        CLASS0_SIZE                = 2,      /* Number of values for mv_class0_bit */
        MV_OFFSET_BITS             = 10,     /* Maximum number of bits for decoding motion vectors */
        MAX_LOOP_FILTER            = 63,     /* Maximum value used for loop filtering */
        REF_SCALE_SHIFT            = 14,     /* Number of bits of precision when scaling reference frames */
        SUBPEL_BITS                = 4,      /* Number of bits of precision when choosing an inter prediction filter kernel */
        SUBPEL_MASK                = 15,     /* (1 << SUBPEL_BITS) - 1 */
        SCALE_SUBPEL_BITS          = 10,     /* Number of bits of precision when computing inter prediction locations */
        MV_BORDER                  = 128,    /* Value used when clipping motion vectors */
        PALETTE_COLOR_CONTEXTS     = 5,      /* Number of values for color contexts */
        PALETTE_MAX_COLOR_CONTEXT_HASH = 8,      /* Number of mappings between color context hash and color context */
        PALETTE_BLOCK_SIZE_CONTEXTS   = 7,      /* Number of values for palette block size */
        PALETTE_Y_MODE_CONTEXTS       = 3,      /* Number of values for palette Y plane mode contexts */
        PALETTE_UV_MODE_CONTEXTS      = 2,      /* Number of values for palette U and V plane mode contexts */
        PALETTE_SIZES                 = 7,      /* Number of values for palette_size */
        PALETTE_COLORS                = 8,      /* Number of values for palette_color */
        PALETTE_NUM_NEIGHBORS         = 3,      /* Number of neighbors considered within palette computation */
        DELTA_Q_SMALL                 = 3,      /* Value indicating alternative encoding of quantizer index delta values */
        DELTA_LF_SMALL                = 3,      /* Value indicating alternative encoding of loop filter delta values */
        QM_TOTAL_SIZE                 = 3344,   /* Number of values in the quantizer matrix */
        MAX_ANGLE_DELTA               = 3,      /* Maximum magnitude of AngleDeltaY and AngleDeltaUV */
        DIRECTIONAL_MODES             = 8,      /* Number of directional intra modes */
        ANGLE_STEP                    = 3,      /* Number of degrees of step per unit increase in AngleDeltaY or AngleDeltaUV */
        TX_SET_TYPES_INTRA            = 3,      /* Number of intra transform set types */
        TX_SET_TYPES_INTER            = 4,      /* Number of inter transform set types */
        WARPEDMODEL_PREC_BITS         = 16,     /* Internal precision of warped motion models */
        IDENTITY                      = 0,      /* Warp model is just an identity transform */
        TRANSLATION                   = 1,      /* Warp model is a pure translation */
        ROTZOOM                       = 2,      /* Warp model is a rotation + symmetric zoom + translation */
        AFFINE                        = 3,      /* Warp model is a general affine transform */
        GM_ABS_TRANS_BITS             = 12,     /* Number of bits encoded for translational components of global motion models, if part of a ROTZOOM or AFFINE model */
        GM_ABS_TRANS_ONLY_BITS       = 9,       /* Number of bits encoded for translational components of global motion models, if part of a TRANSLATION model */
        GM_ABS_ALPHA_BITS            = 12,      /* Number of bits encoded for non-translational components of global motion models */
        DIV_LUT_PREC_BITS            = 14,      /* Number of fractional bits of entries in divisor lookup table */
        DIV_LUT_BITS                 = 8,       /* Number of fractional bits for lookup in divisor lookup table */
        DIV_LUT_NUM                  = 257,     /* Number of entries in divisor lookup table */
        MOTION_MODES                 = 3,       /* Number of values for motion modes */
        SIMPLE                       = 0,       /* Use translation or global motion compensation */
        OBMC                         = 1,       /* Use overlapped block motion compensation */
        LOCALWARP                    = 2,       /* Use local warp motion compensation */
        LEAST_SQUARES_SAMPLES_MAX    = 8,       /* Largest number of samples used when computing a local warp */
        LS_MV_MAX                    = 256,     /* Largest motion vector difference to include in local warp computation */
        WARPEDMODEL_TRANS_CLAMP      = 1<<23,   /* Clamping value used for translation components of warp */
        WARPEDMODEL_NONDIAGAFFINE_CLAMP = 1<<13,  /* Clamping value used for matrix components of warp */
        WARPEDPIXEL_PREC_SHIFTS      = 1<<6,    /* Number of phases used in warped filtering */
        WARPEDDIFF_PREC_BITS         = 10,      /* Number of extra bits of precision in warped filtering */
        GM_ALPHA_PREC_BITS           = 15,      /* Number of fractional bits for sending non-translational warp model coefficients */
        GM_TRANS_PREC_BITS           = 6,       /* Number of fractional bits for sending translational warp model coefficients */
        GM_TRANS_ONLY_PREC_BITS      = 3,       /* Number of fractional bits used for pure translational warps */
        INTERINTRA_MODES             = 4,       /* Number of inter intra modes */
        MASK_MASTER_SIZE             = 64,      /* Size of MasterMask array */
        SEGMENT_ID_PREDICTED_CONTEXTS = 3,    /* Number of contexts for segment_id_predicted */
        FWD_REFS                  = 4,      /* Number of syntax elements for forward reference frames */
        BWD_REFS                  = 3,      /* Number of syntax elements for backward reference frames */
        SINGLE_REFS               = 7,      /* Number of syntax elements for single reference frames */
        UNIDIR_COMP_REFS          = 4,      /* Number of syntax elements for unidirectional compound reference frames */
        COMPOUND_TYPES            = 2,      /* Number of values for compound_type */
        CFL_JOINT_SIGNS           = 8,      /* Number of values for cfl_alpha_signs */
        CFL_ALPHABET_SIZE         = 16,     /* Number of values for cfl_alpha_u and cfl_alpha_v */
        COMP_INTER_CONTEXTS       = 5,      /* Number of contexts for comp_mode */
        COMP_REF_TYPE_CONTEXTS    = 5,      /* Number of contexts for comp_ref_type */
        CFL_ALPHA_CONTEXTS        = 6,      /* Number of contexts for cfl_alpha_u and cfl_alpha_v */
        INTRA_MODE_CONTEXTS       = 5,      /* Number of each of left and above contexts for intra_frame_y_mode */
        COMP_GROUP_IDX_CONTEXTS   = 6,      /* Number of contexts for comp_group_idx */
        COMPOUND_IDX_CONTEXTS     = 6,      /* Number of contexts for compound_idx */
        INTRA_EDGE_KERNELS        = 3,      /* Number of filter kernels for the intra edge filter */
        INTRA_EDGE_TAPS           = 5,      /* Number of kernel taps for the intra edge filter */
        FRAME_LF_COUNT            = 4,      /* Number of loop filter strength values */
        MAX_VARTX_DEPTH           = 2,      /* Maximum depth for variable transform trees */
        TXFM_PARTITION_CONTEXTS   = 21,     /* Number of contexts for txfm_split */
        REF_CAT_LEVEL             = 640,    /* Bonus weight for close motion vectors */
        MAX_REF_MV_STACK_SIZE     = 8,      /* Maximum number of motion vectors in the stack */
        MFMV_STACK_SIZE           = 3,      /* Stack size for motion field motion vectors */
        MAX_TX_DEPTH                 = 2,   /* Maximum times the transform can be split */
        WEDGE_TYPES                  = 16,  /* Number of directions for the wedge mask process */
        FILTER_BITS                  = 7,   /* Number of bits used in Wiener filter coefficients */
        WIENER_COEFFS                = 3,   /* Number of Wiener filter coefficients to read */
        SGRPROJ_PARAMS_BITS          = 4,   /* Number of bits needed to specify self guided filter set */
        SGRPROJ_PRJ_SUBEXP_K         = 4,   /* Controls how self guided deltas are read */
        SGRPROJ_PRJ_BITS             = 7,   /* Precision bits during self guided restoration */
        SGRPROJ_RST_BITS             = 4,   /* Restoration precision bits generated higher than source before projection */
        SGRPROJ_MTABLE_BITS          = 20,  /* Precision of mtable division table */
        SGRPROJ_RECIP_BITS           = 12,  /* Precision of division by n table */
        SGRPROJ_SGR_BITS             = 8,   /* Internal precision bits for core selfguided_restoration */
        EC_PROB_SHIFT                = 6,   /* Number of bits to reduce CDF precision during arithmetic coding */
        EC_MIN_PROB                  = 4,   /* Minimum probability assigned to each symbol during arithmetic coding */
        SELECT_SCREEN_CONTENT_TOOLS  = 2,   /* Value that indicates the allow_screen_content_tools syntax element is coded */
        SELECT_INTEGER_MV            = 2,   /* Value that indicates the force_integer_mv syntax element is coded */
        RESTORATION_TILESIZE_MAX     = 256, /* Maximum size of a loop restoration tile */
        MAX_FRAME_DISTANCE           = 31,  /* Maximum distance when computing weighted prediction */
        MAX_OFFSET_WIDTH             = 8,   /* Maximum horizontal offset of a projected motion vector */
        MAX_OFFSET_HEIGHT            = 0,   /* Maximum vertical offset of a projected motion vector */
        WARP_PARAM_REDUCE_BITS       = 6,   /* Rounding bitwidth for the parameters to the shear process */
        NUM_BASE_LEVELS         = 2,      /* Number of quantizer base levels */
        COEFF_BASE_RANGE        = 12,     /* The quantizer range above NUM_BASE_LEVELS above which the Exp-Golomb coding process is activated */
        BR_CDF_SIZE             = 4,      /* Number of values for coeff_br */
        SIG_COEF_CONTEXTS_EOB   = 4,      /* Number of contexts for coeff_base_eob */
        SIG_COEF_CONTEXTS_2D    = 26,     /* Context offset for coeff_base for horizontal-only or vertical-only transforms */
        SIG_COEF_CONTEXTS       = 42,     /* Number of contexts for coeff_base */
        SIG_REF_DIFF_OFFSET_NUM = 5,      /* Maximum number of context samples to be used in determining the context index for coeff_base and coeff_base_eob */
        SUPERRES_NUM            = 8,      /* Numerator for upscaling ratio */
        SUPERRES_DENOM_MIN      = 9,      /* Smallest denominator for upscaling ratio */
        SUPERRES_DENOM_BITS     = 3,      /* Number of bits sent to specify denominator of upscaling ratio */
        SUPERRES_FILTER_BITS    = 6,      /* Number of bits of fractional precision for upscaling filter selection */
        SUPERRES_FILTER_SHIFTS  = 1 << SUPERRES_FILTER_BITS,  /* Number of phases of upscaling filters */
        SUPERRES_FILTER_TAPS    = 8,      /* Number of taps of upscaling filters */
        SUPERRES_FILTER_OFFSET  = 3,      /* Sample offset for upscaling filters */
        SUPERRES_SCALE_BITS     = 14,     /* Number of fractional bits for computing position in upscaling */
        SUPERRES_SCALE_MASK     = (1 << 14) - 1,  /* Mask for computing position in upscaling */
        SUPERRES_EXTRA_BITS     = 8,      /* Difference in precision between SUPERRES_SCALE_BITS and SUPERRES_FILTER_BITS */
        TXB_SKIP_CONTEXTS       = 13,     /* Number of contexts for all_zero */
        EOB_COEF_CONTEXTS       = 9,      /* Number of contexts for eob_extra */
        DC_SIGN_CONTEXTS        = 3,      /* Number of contexts for dc_sign */
        LEVEL_CONTEXTS          = 21,     /* Number of contexts for coeff_br */
        TX_CLASS_2D          = 0,         /* Transform class for transform types performing non-identity transforms in both directions */
        TX_CLASS_HORIZ       = 1,         /* Transform class for transforms performing only a horizontal non-identity transform */
        TX_CLASS_VERT        = 2,         /* Transform class for transforms performing only a vertical non-identity transform */
        REFMVS_LIMIT         = (1 << 12) - 1,  /* Largest reference MV component that can be saved */
        INTRA_FILTER_SCALE_BITS = 4,      /* Scaling shift for intra filtering process */
        INTRA_FILTER_MODES   = 5,         /* Number of types of intra filtering */
        COEFF_CDF_Q_CTXS     = 4,         /* Number of selectable context types for the coeff() syntax structure */
        PRIMARY_REF_NONE     = 7,         /* Value of primary_ref_frame indicating that there is no primary reference frame */
        BUFFER_POOL_MAX_SIZE = 10,        /* Number of frames in buffer pool */
        LAST_FRAME = 1,
        LAST2_FRAME = 2,
        LAST3_FRAME = 3,
        GOLDEN_FRAME = 4,
        BWDREF_FRAME = 5,
        ALTREF2_FRAME = 6,
        ALTREF_FRAME = 7,
    }
#pragma warning restore CA1069 // 不应复制枚举值

    /*
     * OBU types.
     */

    public enum OBUType
    {
        OBU_SEQUENCE_HEADER = 1,
        OBU_TEMPORAL_DELIMITER = 2,
        OBU_FRAME_HEADER = 3,
        OBU_TILE_GROUP = 4,
        OBU_METADATA = 5,
        OBU_FRAME = 6,
        OBU_REDUNDANT_FRAME_HEADER = 7,
        OBU_TILE_LIST = 8,
        OBU_PADDING = 15,
    }

    /*
     * Metadata types for the Metadata OBU.
     */

    public enum OBUMetadataType
    {
        OBU_METADATA_TYPE_HDR_CLL = 1,
        OBU_METADATA_TYPE_HDR_MDCV = 2,
        OBU_METADATA_TYPE_SCALABILITY = 3,
        OBU_METADATA_TYPE_ITUT_T35 = 4,
        OBU_METADATA_TYPE_TIMECODE = 5,
        /* 6-31 Unregistered user private */
        /* 32 and greater Reserved for AOM use */
    }

    /*
     * Color primaries.
     *
     * These match ISO/IEC 23091-4/ITU-T H.273.
     */

    public enum OBUColorPrimaries
    {
        OBU_CP_BT_709 = 1,
        OBU_CP_UNSPECIFIED = 2,
        OBU_CP_BT_470_M = 4,
        OBU_CP_BT_470_B_G = 5,
        OBU_CP_BT_601 = 6,
        OBU_CP_SMPTE_240 = 7,
        OBU_CP_GENERIC_FILM = 8,
        OBU_CP_BT_2020 = 9,
        OBU_CP_XYZ = 10,
        OBU_CP_SMPTE_431 = 11,
        OBU_CP_SMPTE_432 = 12,
        OBU_CP_EBU_3213 = 22,
    }

    /*
     * Transfer characteristics.
     *
     * These match ISO/IEC 23091-4/ITU-T H.273.
     */

    public enum OBUTransferCharacteristics
    {
        OBU_TC_RESERVED_0 = 0,
        OBU_TC_BT_709 = 1,
        OBU_TC_UNSPECIFIED = 2,
        OBU_TC_RESERVED_3 = 3,
        OBU_TC_BT_470_M = 4,
        OBU_TC_BT_470_B_G = 5,
        OBU_TC_BT_601 = 6,
        OBU_TC_SMPTE_240 = 7,
        OBU_TC_LINEAR = 8,
        OBU_TC_LOG_100 = 9,
        OBU_TC_LOG_100_SQRT10 = 10,
        OBU_TC_IEC_61966 = 11,
        OBU_TC_BT_1361 = 12,
        OBU_TC_SRGB = 13,
        OBU_TC_BT_2020_10_BIT = 14,
        OBU_TC_BT_2020_12_BIT = 15,
        OBU_TC_SMPTE_2084 = 16,
        OBU_TC_SMPTE_428 = 17,
        OBU_TC_HLG = 18
    }

    /*
     * Color matrix coefficients.
     *
     * These match ISO/IEC 23091-4/ITU-T H.273.
     */

    public enum OBUMatrixCoefficients
    {
        OBU_MC_IDENTITY = 0,
        OBU_MC_BT_709 = 1,
        OBU_MC_UNSPECIFIED = 2,
        OBU_MC_RESERVED_3 = 3,
        OBU_MC_FCC = 4,
        OBU_MC_BT_470_B_G = 5,
        OBU_MC_BT_601 = 6,
        OBU_MC_SMPTE_240 = 7,
        OBU_MC_SMPTE_YCGCO = 8,
        OBU_MC_BT_2020_NCL = 9,
        OBU_MC_BT_2020_CL = 10,
        OBU_MC_SMPTE_2085 = 11,
        OBU_MC_CHROMAT_NCL = 12,
        OBU_MC_CHROMAT_CL = 13,
        OBU_MC_ICTCP = 14
    }

    /*
     * Chroma sample position.
     */

    public enum OBUChromaSamplePosition
    {
        OBU_CSP_UNKNOWN = 0,
        OBU_CSP_VERTICAL = 1,
        OBU_CSP_COLOCATED = 2
        /* 3 Reserved */
    }

    /*
     * Frame types.
     */

    public enum OBUFrameType
    {
        OBU_KEY_FRAME = 0,
        OBU_INTER_FRAME = 1,
        OBU_INTRA_ONLY_FRAME = 2,
        OBU_SWITCH_FRAME = 3
    }

    public struct OBUTimeInfo
    {
        public uint num_units_in_display_tick;
        public uint time_scale;
        public bool equal_picture_interval;
        public uint num_ticks_per_picture_minus_1;
        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"time_info"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"num_units_in_display_tick = {num_units_in_display_tick}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"time_scale =  {time_scale}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_logicalnode, $"equal_picture_interval = {equal_picture_interval}"));
            if ( equal_picture_interval )
            {
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"num_ticks_per_picture_minus_1 = {num_ticks_per_picture_minus_1}"));
            }
            return sb;
        }

        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n    - num_units_in_display_tick = {num_units_in_display_tick}");
            sb.Append($"\r\n    - time_scale =  {time_scale}");
            sb.Append($"\r\n    - equal_picture_interval = {equal_picture_interval}");
            if( equal_picture_interval )
            {
                sb.Append($"\r\n    - num_ticks_per_picture_minus_1 = {num_ticks_per_picture_minus_1}");
            }
            
            return sb.ToString();
        }
    }

    public struct OBUDecoderModelInfo
    {
        public byte buffer_delay_length_minus_1;
        public uint num_units_in_decoding_tick;
        public byte buffer_removal_time_length_minus_1;
        public byte frame_presentation_time_length_minus_1;
        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"decoder_mode_info"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"buffer_delay_length_minus_1 = {buffer_delay_length_minus_1}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"num_units_in_decoding_tick =  {num_units_in_decoding_tick}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"buffer_removal_time_length_minus_1 = {buffer_removal_time_length_minus_1}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"frame_presentation_time_length_minus_1 = {frame_presentation_time_length_minus_1}"));
            return sb;
        }

        public override string ToString ()
        {
            StringBuilder sb = new();
            sb.Append($"\r\n     - buffer_delay_length_minus_1 = {buffer_delay_length_minus_1}");
            sb.Append($"\r\n     - num_units_in_decoding_tick =  {num_units_in_decoding_tick}");
            sb.Append($"\r\n     - buffer_removal_time_length_minus_1 = {buffer_removal_time_length_minus_1}");
            sb.Append($"\r\n     - frame_presentation_time_length_minus_1 = {frame_presentation_time_length_minus_1}");

            return sb.ToString();
        }
    }

    public struct OBUOperaParametersInfo
    {
        public uint decoder_buffer_delay;
        public uint encoder_buffer_delay;
        public bool low_delay_mode_flag;

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"opera_parameters_info"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"decoder_buffer_delay = {decoder_buffer_delay}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"encoder_buffer_delay =  {encoder_buffer_delay}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"low_delay_mode_flag = {low_delay_mode_flag}"));
            return sb;
        }

        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n       - decoder_buffer_delay = {decoder_buffer_delay}");
            sb.Append($"\r\n       - encoder_buffer_delay =  {encoder_buffer_delay}");
            sb.Append($"\r\n       - low_delay_mode_flag = {low_delay_mode_flag}");

            return sb.ToString();
        }
    }

    public struct OBUColorConfig
    {
        public bool high_bitdepth;
        public bool twelve_bit;
        public int BitDepth;
        public bool mono_chrome;
        public int NumPlanes;
        public bool color_description_present_flag;
        public OBUColorPrimaries color_primaries;
        public OBUTransferCharacteristics transfer_characteristics;
        public OBUMatrixCoefficients matrix_coefficients;
        public bool color_range;
        public bool subsampling_x;
        public bool subsampling_y;
        public OBUChromaSamplePosition chroma_sample_position;
        public bool separate_uv_delta_q;
        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();

            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"ColorConfig"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"high_bitdepth = {high_bitdepth}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"twelve_bit = {twelve_bit}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"BitDepth = {BitDepth}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"mono_chrome = {mono_chrome}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"NumPlanes = {NumPlanes}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"color_description_present_flag = {color_description_present_flag}"));
            sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"color_primaries = {color_primaries}"));
            sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"transfer_characteristics = {transfer_characteristics}"));
            sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"matrix_coefficients = {matrix_coefficients}"));
            var desc = color_range?"full":"limited";
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"color_range = {desc}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"subsampling_x = {subsampling_x}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"subsampling_y = {subsampling_y}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"chroma_sample_position = {chroma_sample_position}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"separate_uv_delta_q = {separate_uv_delta_q}"));

            return sb;
        }
        public new readonly string ToString()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   |- high_bitdepth = {high_bitdepth}");
            sb.Append($"\r\n   |- twelve_bit = {twelve_bit}");
            sb.Append($"\r\n   |- BitDepth = {BitDepth}");
            sb.Append($"\r\n   |- mono_chrome = {mono_chrome}");
            sb.Append($"\r\n   |- NumPlanes = {NumPlanes}");
            sb.Append($"\r\n   |- color_description_present_flag = {color_description_present_flag}");
            sb.Append($"\r\n      - color_primaries = {color_primaries}");
            sb.Append($"\r\n      - transfer_characteristics = {transfer_characteristics}");
            sb.Append($"\r\n      - matrix_coefficients = {matrix_coefficients}");
            var desc = color_range?"full":"limited";
            sb.Append($"\r\n   |- color_range = {desc}");
            sb.Append($"\r\n   |- subsampling_x = {subsampling_x}");
            sb.Append($"\r\n   |- subsampling_y = {subsampling_y}");
            sb.Append($"\r\n   |- chroma_sample_position = {chroma_sample_position}");
            sb.Append($"\r\n   |_ separate_uv_delta_q = {separate_uv_delta_q}");

            return sb.ToString();
        }
    }

    public class OBUSequenceHeader
    {
        public byte seq_profile;
        public bool still_picture;
        public bool reduced_still_picture_header;
        public bool timing_info_present_flag;
        public OBUTimeInfo timing_info;
        public bool decoder_model_info_present_flag;
        public OBUDecoderModelInfo decoder_model_info;
        public bool initial_display_delay_present_flag;
        public byte operating_points_cnt_minus_1;
        public byte[] operating_point_idc = new byte[32];
        public byte[] seq_level_idx = new byte[32];
        public bool[] seq_tier = new bool[32];
        public bool[] decoder_model_present_for_this_op = new bool[32];
        public OBUOperaParametersInfo[] operating_parameters_info = new OBUOperaParametersInfo[32];
        public bool[] initial_display_delay_present_for_this_op = new bool[32];
        public byte[] initial_display_delay_minus_1 = new byte[32];
        public byte frame_width_bits_minus_1;
        public byte frame_height_bits_minus_1;
        public uint max_frame_width_minus_1;
        public uint max_frame_height_minus_1;
        public bool frame_id_numbers_present_flag;
        public byte delta_frame_id_length_minus_2;
        public byte additional_frame_id_length_minus_1;
        public bool use_128x128_superblock;
        public bool enable_filter_intra;
        public bool enable_intra_edge_filter;
        public bool enable_interintra_compound;
        public bool enable_masked_compound;
        public bool enable_warped_motion;
        public bool enable_dual_filter;
        public bool enable_order_hint;
        public bool enable_jnt_comp;
        public bool enable_ref_frame_mvs;
        public int seq_choose_screen_content_tools;
        public int seq_force_screen_content_tools;
        public int seq_choose_integer_mv;
        public int seq_force_integer_mv;
        public byte order_hint_bits_minus_1;
        public byte OrderHintBits;
        public bool enable_superres;
        public bool enable_cdef;
        public bool enable_restoration;
        public OBUColorConfig color_config;
        public bool film_grain_params_present;

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new();

            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_root, $"SequenceHeader"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"seq_profile = {seq_profile}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"still_picture = {still_picture}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"reduced_still_picture_v = {reduced_still_picture_header}"));
            if ( reduced_still_picture_header )
            {
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"timing_info_present_flag = {timing_info_present_flag}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"decoder_model_info_present_flag = {decoder_model_info_present_flag}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"initial_display_delay_present_flag = {initial_display_delay_present_flag}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"operating_points_cnt_minus_1 = {operating_points_cnt_minus_1}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"operating_point_idc = {operating_point_idc[0]}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"seq_level_idx = {seq_level_idx[0]}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"seq_level_idx = {seq_level_idx[0]}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"seq_tier = {seq_tier[0]}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"decoder_model_present_for_this_op = {decoder_model_present_for_this_op[0]}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"initial_display_delay_present_for_this_op = {initial_display_delay_present_for_this_op[0]}"));
            }
            else
            {
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"timing_info_present_flag = {timing_info_present_flag}"));
                if ( timing_info_present_flag )
                {
                    //sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"  - timing_info = ( {timing_info.ToString()} )"));
                    var t = timing_info.ToSpecTree();
                    foreach (var tr in t)
                    {
                        var t3 = tr;
                        t3.level += 1 + 1;
                        sb.Enqueue(t3);
                    }
                }

                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_logicalnode, $"decoder_model_info_present_flag = {decoder_model_info_present_flag}"));
                if ( decoder_model_info_present_flag )
                {
                    //sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"  - decoder_model_info = ( {decoder_model_info.ToString()} )"));
                    var t = decoder_model_info.ToSpecTree();
                    foreach ( var tr in t )
                    {
                        var t3 = tr;
                        t3.level += 2 + 1;
                        sb.Enqueue(t3);
                    }
                }

                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"initial_display_delay_present_flag = {initial_display_delay_present_flag}"));
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_logicalnode, $"operating_points_cnt_minus_1 = {operating_points_cnt_minus_1}"));
                for ( var i = 0; i <= operating_points_cnt_minus_1; i++ )
                {
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"operating_point_idc = {operating_point_idc[i]}"));
                    var level = seq_level_idx[i];
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"seq_level_idx = {level}"));
                    bool st = seq_tier[i];
                    if ( level < 7 )
                    {
                        st = false;
                    }
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_logicalnode, $"seq_tier = {st}"));
                    if ( decoder_model_info_present_flag )
                    {
                        sb.Enqueue(new STItem(3, STItemNoteType.NoteType_logicalnode, $"decoder_model_present_for_this_op = {decoder_model_present_for_this_op[i]}"));
                        if ( decoder_model_present_for_this_op[i] )
                        {
                            //sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"operating_parameters_info = ( {operating_parameters_info[i]} )"));
                            var t = operating_parameters_info[i].ToSpecTree();
                            foreach ( var tr in t )
                            {
                                var t3 = tr;
                                t3.level += 3 + 1;
                                sb.Enqueue(t3);
                            }
                        }
                    }
                    if ( initial_display_delay_present_flag )
                    {
                        sb.Enqueue(new STItem(3, STItemNoteType.NoteType_logicalnode, $"initial_display_delay_present_for_this_op = {initial_display_delay_present_for_this_op[i]}"));
                        if ( initial_display_delay_present_for_this_op[i] )
                        {
                            sb.Enqueue(new STItem(4, STItemNoteType.NoteType_node, $"initial_display_delay_minus_1 = {initial_display_delay_minus_1[i]}"));
                        }
                    }
                }

                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"frame_width_bits_minus_1 = {frame_width_bits_minus_1}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"frame_height_bits_minus_1 = {frame_height_bits_minus_1}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"max_frame_width_minus_1 = {max_frame_width_minus_1}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"max_frame_height_minus_1 = {max_frame_height_minus_1}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"frame_id_numbers_present_flag = {frame_id_numbers_present_flag}"));
                if ( frame_id_numbers_present_flag )
                {
                    sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"delta_frame_id_length_minus_2 = {delta_frame_id_length_minus_2}"));
                    sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"additional_frame_id_length_minus_1 = {additional_frame_id_length_minus_1}"));
                }
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"use_128x128_superblock = {use_128x128_superblock}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_filter_intra = {enable_filter_intra}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_intra_edge_filter = {enable_intra_edge_filter}"));
                if ( reduced_still_picture_header )
                {
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_interintra_compound = {enable_interintra_compound}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_masked_compound = {enable_masked_compound}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_warped_motion = {enable_warped_motion}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_dual_filter = {enable_dual_filter}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"enable_order_hint = {enable_order_hint}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_jnt_comp = {enable_jnt_comp}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_ref_frame_mvs = {enable_ref_frame_mvs}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"seq_force_screen_content_tools = {seq_force_screen_content_tools}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"seq_force_integer_mv = {seq_force_integer_mv}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"OrderHintBits = {OrderHintBits}"));
                }
                else
                {
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_interintra_compound = {enable_interintra_compound}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_masked_compound = {enable_masked_compound}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_warped_motion = {enable_warped_motion}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_dual_filter = {enable_dual_filter}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"enable_order_hint = {enable_order_hint}"));
                    sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"enable_jnt_comp = {enable_jnt_comp}"));
                    sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"enable_ref_frame_mvs = {enable_ref_frame_mvs}"));
                    sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"order_hint_bits_minus_1 = {order_hint_bits_minus_1}"));
                    sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"OrderHintBits = {OrderHintBits}"));
                    sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"seq_choose_screen_content_tools = {seq_choose_screen_content_tools}"));

                    if ( seq_choose_screen_content_tools > 0 )
                    {
                        sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS"));
                    }
                    else if ( seq_force_screen_content_tools > 0 )
                    {
                        sb.Enqueue(new STItem(2, STItemNoteType.NoteType_logicalnode, $"seq_force_screen_content_tools = {seq_force_screen_content_tools}"));
                        sb.Enqueue(new STItem(2, STItemNoteType.NoteType_logicalnode, $"seq_choose_integer_mv = {seq_choose_integer_mv}"));
                        if ( seq_choose_integer_mv > 0 )
                        {
                            sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"seq_force_integer_mv = SELECT_INTEGER_MV"));
                        }
                        else
                        {
                            sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"seq_force_integer_mv = {seq_force_integer_mv}"));
                        }
                    }
                }
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_superres = {enable_superres}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_cdef = {enable_cdef}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"enable_restoration = {enable_restoration}"));
                {
                    var v = color_config.ToSpecTree();
                    foreach ( var v2 in v )
                    {
                        var v3 = v2;
                        v3.level += 1;
                        sb.Enqueue(v3);
                    }
                }
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"film_grain_params_present = {film_grain_params_present}"));
            }
            return sb;
        }

        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"SequenceHeader");
            sb.Append($"\r\n |- seq_profile = {seq_profile}");
            sb.Append($"\r\n |- still_picture = {still_picture}");
            sb.Append($"\r\n |- reduced_still_picture_v = {reduced_still_picture_header}");
            if (reduced_still_picture_header )
            {
                sb.Append($"\r\n |- timing_info_present_flag = {timing_info_present_flag}");
                sb.Append($"\r\n |- decoder_model_info_present_flag = {decoder_model_info_present_flag}");
                sb.Append($"\r\n |- initial_display_delay_present_flag = {initial_display_delay_present_flag}");
                sb.Append($"\r\n |- operating_points_cnt_minus_1 = {operating_points_cnt_minus_1}");
                sb.Append($"\r\n |- operating_point_idc = {operating_point_idc[0]}");
                sb.Append($"\r\n |- seq_level_idx = {seq_level_idx[0]}");
                sb.Append($"\r\n |- seq_level_idx = {seq_level_idx[0]}");
                sb.Append($"\r\n |- seq_tier = {seq_tier[0]}");
                sb.Append($"\r\n |- decoder_model_present_for_this_op = {decoder_model_present_for_this_op[0]}");
                sb.Append($"\r\n |- initial_display_delay_present_for_this_op = {initial_display_delay_present_for_this_op[0]}");
            }
            else
            {
                sb.Append($"\r\n |- timing_info_present_flag = {timing_info_present_flag}");
                if ( timing_info_present_flag )
                {
                    sb.Append($"\r\n   - timing_info = ( {timing_info.ToString()} )");
                }

                sb.Append($"\r\n |- decoder_model_info_present_flag = {decoder_model_info_present_flag}");
                if ( decoder_model_info_present_flag )
                    sb.Append($"\r\n   - decoder_model_info = ( {decoder_model_info.ToString()} )");

                sb.Append($"\r\n |- initial_display_delay_present_flag = {initial_display_delay_present_flag}");
                sb.Append($"\r\n |- operating_points_cnt_minus_1 = {operating_points_cnt_minus_1}");
                sb.Append($"\r\n   --");
                for ( var i = 0; i <= operating_points_cnt_minus_1; i++ )
                {
                    sb.Append($"\r\n    - operating_point_idc = {operating_point_idc[i]}");
                    var level = seq_level_idx[i];
                    sb.Append($"\r\n    - seq_level_idx = {level}");
                    bool st = seq_tier[i];
                    if ( level < 7 )
                    {
                        st = false;
                    }
                    sb.Append($"\r\n    - seq_tier = {st}");
                    if ( decoder_model_info_present_flag )
                    {
                        sb.Append($"\r\n    - decoder_model_present_for_this_op = {decoder_model_present_for_this_op[i]}");
                        if ( decoder_model_present_for_this_op[i] )
                        {
                            sb.Append($"\r\n      - operating_parameters_info = ( {operating_parameters_info[i]} )");
                        }
                    }
                    if ( initial_display_delay_present_flag )
                    {
                        sb.Append($"\r\n    - initial_display_delay_present_for_this_op = {initial_display_delay_present_for_this_op[i]}");
                        if ( initial_display_delay_present_for_this_op[i] )
                        {
                            sb.Append($"\r\n      - initial_display_delay_minus_1 = {initial_display_delay_minus_1[i]}");
                        }
                    }
                }

                sb.Append($"\r\n |- frame_width_bits_minus_1 = {frame_width_bits_minus_1}");
                sb.Append($"\r\n |- frame_height_bits_minus_1 = {frame_height_bits_minus_1}");
                sb.Append($"\r\n |- max_frame_width_minus_1 = {max_frame_width_minus_1}");
                sb.Append($"\r\n |- max_frame_height_minus_1 = {max_frame_height_minus_1}");
                sb.Append($"\r\n |- frame_id_numbers_present_flag = {frame_id_numbers_present_flag}");
                if ( frame_id_numbers_present_flag )
                {
                    sb.Append($"\r\n    - delta_frame_id_length_minus_2 = {delta_frame_id_length_minus_2}");
                    sb.Append($"\r\n    - additional_frame_id_length_minus_1 = {additional_frame_id_length_minus_1}");
                }
                sb.Append($"\r\n |- use_128x128_superblock = {use_128x128_superblock}");
                sb.Append($"\r\n |- enable_filter_intra = {enable_filter_intra}");
                sb.Append($"\r\n |- enable_intra_edge_filter = {enable_intra_edge_filter}");
                if ( reduced_still_picture_header )
                {
                    sb.Append($"\r\n |- enable_interintra_compound = {enable_interintra_compound}");
                    sb.Append($"\r\n |- enable_masked_compound = {enable_masked_compound}");
                    sb.Append($"\r\n |- enable_warped_motion = {enable_warped_motion}");
                    sb.Append($"\r\n |- enable_dual_filter = {enable_dual_filter}");
                    sb.Append($"\r\n |- enable_order_hint = {enable_order_hint}");
                    sb.Append($"\r\n |- enable_jnt_comp = {enable_jnt_comp}");
                    sb.Append($"\r\n |- enable_ref_frame_mvs = {enable_ref_frame_mvs}");
                    sb.Append($"\r\n |- seq_force_screen_content_tools = {seq_force_screen_content_tools}");
                    sb.Append($"\r\n |- seq_force_integer_mv = {seq_force_integer_mv}");
                    sb.Append($"\r\n |- OrderHintBits = {OrderHintBits}");
                }
                else
                {
                    sb.Append($"\r\n |- enable_interintra_compound = {enable_interintra_compound}");
                    sb.Append($"\r\n |- enable_masked_compound = {enable_masked_compound}");
                    sb.Append($"\r\n |- enable_warped_motion = {enable_warped_motion}");
                    sb.Append($"\r\n |- enable_dual_filter = {enable_dual_filter}");
                    sb.Append($"\r\n |- enable_order_hint = {enable_order_hint}");
                    sb.Append($"\r\n   - enable_jnt_comp = {enable_jnt_comp}");
                    sb.Append($"\r\n   - enable_ref_frame_mvs = {enable_ref_frame_mvs}");
                    sb.Append($"\r\n |- seq_choose_screen_content_tools = {seq_choose_screen_content_tools}");

                    if ( seq_choose_screen_content_tools > 0 )
                    {
                        sb.Append($"\r\n   - seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS");
                    }
                    else if ( seq_force_screen_content_tools > 0 )
                    {
                        sb.Append($"\r\n   - seq_force_screen_content_tools = {seq_force_screen_content_tools}");
                        sb.Append($"\r\n      - seq_choose_integer_mv = {seq_choose_integer_mv}");
                        if ( seq_choose_integer_mv > 0 )
                        {
                            sb.Append($"\r\n        - seq_force_integer_mv = SELECT_INTEGER_MV");
                        }
                        else
                        {
                            sb.Append($"\r\n        - seq_force_integer_mv = {seq_force_integer_mv}");
                        }
                    }

                    if ( enable_order_hint )
                    {
                        sb.Append($"\r\n   - order_hint_bits_minus_1 = {order_hint_bits_minus_1}");
                        sb.Append($"\r\n   - OrderHintBits = {OrderHintBits}");
                    }
                }
                sb.Append($"\r\n |- enable_superres = {enable_superres}");
                sb.Append($"\r\n |- enable_cdef = {enable_cdef}");
                sb.Append($"\r\n |- enable_restoration = {enable_restoration}");
                sb.Append($"\r\n |- color_config = {color_config.ToString()}");
                sb.Append($"\r\n |_ film_grain_params_present = {film_grain_params_present}");
            }

            return sb.ToString();
        }
    }

    public class OBUFilmGrainParameters
    {
        public bool apply_grain;
        public ushort grain_seed;
        public bool update_grain;
        public byte film_grain_params_ref_idx;
        public byte num_y_points;
        public byte[] point_y_value = new byte[16];
        public byte[] point_y_scaling = new byte[16];
        public bool chroma_scaling_from_luma;
        public byte num_cb_points;
        public byte[] point_cb_value = new byte[16];
        public byte[] point_cb_scaling = new byte[16];
        public byte num_cr_points;
        public byte[] point_cr_value = new byte[16];
        public byte[] point_cr_scaling = new byte[16];
        public byte grain_scaling_minus_8;
        public byte ar_coeff_lag;
        public byte[] ar_coeffs_y_plus_128 = new byte[24];
        public byte[] ar_coeffs_cb_plus_128 = new byte[25];
        public byte[] ar_coeffs_cr_plus_128 = new byte[25];
        public byte ar_coeff_shift_minus_6;
        public byte grain_scale_shift;
        public byte cb_mult;
        public byte cb_luma_mult;
        public ushort cb_offset;
        public byte cr_mult;
        public byte cr_luma_mult;
        public ushort cr_offset;
        public bool overlap_flag;
        public bool clip_to_restricted_range;

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"FilmGrainParameters"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"apply_grain = {apply_grain}"));
            if ( apply_grain )
            {
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"grain_seed = {grain_seed}"));
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_logicalnode, $"update_grain = {update_grain}"));
                if ( !update_grain )
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"film_grain_params_ref_idx = {film_grain_params_ref_idx}"));
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_logicalnode, $"num_y_points = {num_y_points}"));
                if ( num_y_points > 0 )
                {
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_logicalnode, $"(point_y_value, point_y_scaling) ="));
                    for ( var i = 0; i < num_y_points; i++ )
                    {
                        sb.Enqueue(new STItem(4, STItemNoteType.NoteType_node, $"{point_y_value[i]},{point_y_scaling[i]}"));
                    }
                }
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"chroma_scaling_from_luma = {chroma_scaling_from_luma}"));
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"num_cb_points = {num_cb_points}"));
                if ( num_cb_points > 0 )
                {
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"(point_cb_value, point_cb_scaling) ="));
                    for ( var i = 0; i < num_cb_points; i++ )
                    {
                        sb.Enqueue(new STItem(4, STItemNoteType.NoteType_node, $"{point_cb_value[i]},{point_cb_scaling[i]}"));
                    }
                }
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"num_cr_points = {num_cr_points}"));
                if ( num_cr_points > 0 )
                {
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"(point_cr_value, point_cr_scaling) ="));
                    for ( var i = 0; i < num_cr_points; i++ )
                    {
                        sb.Enqueue(new STItem(4, STItemNoteType.NoteType_node, $"{point_cr_value[i]},{point_cr_scaling[i]}"));
                    }
                }
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"grain_scaling_minus_8 = {grain_scaling_minus_8}"));
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"ar_coeff_lag = {ar_coeff_lag}"));
                var numPosLuma = 2 * ar_coeff_lag * (ar_coeff_lag + 1) + 1;
                var numPosChroma = numPosLuma;
                if ( num_y_points > 0 )
                {
                    numPosChroma += 1;
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"ar_coeffs_y_plus_128 ="));
                    for ( var i = 0; i < numPosLuma; i++ )
                    {
                        sb.Enqueue(new STItem(4, STItemNoteType.NoteType_node, $"{ar_coeffs_y_plus_128[i]}"));
                    }
                }
                if ( chroma_scaling_from_luma || num_cb_points > 0 )
                {
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"ar_coeffs_cb_plus_128 ="));
                    for ( var i = 0; i < numPosChroma; i++ )
                    {
                        sb.Enqueue(new STItem(4, STItemNoteType.NoteType_node, $"{ar_coeffs_cb_plus_128[i]}"));
                    }
                }
                if ( chroma_scaling_from_luma || num_cr_points > 0 )
                {
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"ar_coeffs_cr_plus_128 ="));
                    for ( var i = 0; i < numPosChroma; i++ )
                    {
                        sb.Enqueue(new STItem(4, STItemNoteType.NoteType_node, $"{ar_coeffs_cr_plus_128[i]}"));
                    }
                }
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"ar_coeff_shift_minus_6 = {ar_coeff_shift_minus_6}"));
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"grain_scale_shift = {grain_scale_shift}"));

                if ( num_cb_points > 0 )
                {
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"cb_mult = {cb_mult}"));
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"cb_luma_mult = {cb_luma_mult}"));
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"cb_offset = {cb_offset}"));
                }

                if ( num_cr_points > 0 )
                {
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"cr_mult = {cr_mult}"));
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"cr_luma_mult = {cr_luma_mult}"));
                    sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"cr_offset = {cr_offset}"));
                }

                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"overlap_flag = {overlap_flag}"));
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"clip_to_restricted_range = {clip_to_restricted_range}"));
            }

            return sb;
        }

        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n    |- apply_grain = {apply_grain}");
            if( apply_grain )
            {
                sb.Append($"\r\n    |- grain_seed = {grain_seed}");
                sb.Append($"\r\n    |- update_grain = {update_grain}");
                if ( !update_grain )
                    sb.Append($"\r\n    |- film_grain_params_ref_idx = {film_grain_params_ref_idx}");
                sb.Append($"\r\n    |- num_y_points = {num_y_points}");
                if ( num_y_points > 0 )
                {
                    sb.Append($"\r\n    |- (point_y_value, point_y_scaling) =");
                    for ( var i = 0; i < num_y_points; i++ )
                    {
                        sb.Append($"{point_y_value[i]},{point_y_scaling[i]}");
                    }
                }
                sb.Append($"\r\n    |- chroma_scaling_from_luma = {chroma_scaling_from_luma}");
                sb.Append($"\r\n    |- num_cb_points = {num_cb_points}");
                if ( num_cb_points > 0 )
                {
                    sb.Append($"\r\n    |- (point_cb_value, point_cb_scaling) =");
                    for ( var i = 0; i < num_cb_points; i++ )
                    {
                        sb.Append($"{point_cb_value[i]},{point_cb_scaling[i]}");
                    }
                }
                sb.Append($"\r\n    |- num_cr_points = {num_cr_points}");
                if ( num_cr_points > 0 )
                {
                    sb.Append($"\r\n    |- (point_cr_value, point_cr_scaling) =");
                    for ( var i = 0; i < num_cr_points; i++ )
                    {
                        sb.Append($"{point_cr_value[i]},{point_cr_scaling[i]}");
                    }
                }
                sb.Append($"\r\n    |- grain_scaling_minus_8 = {grain_scaling_minus_8}");
                sb.Append($"\r\n    |- ar_coeff_lag = {ar_coeff_lag}");
                var numPosLuma = 2 * ar_coeff_lag * (ar_coeff_lag + 1) + 1;
                var numPosChroma = numPosLuma;
                if ( num_y_points > 0 )
                {
                    numPosChroma += 1;
                    sb.Append($"\r\n    |- ar_coeffs_y_plus_128 =");
                    for ( var i = 0; i < numPosLuma; i++ )
                    {
                        sb.Append($"{ar_coeffs_y_plus_128[i]}");
                    }
                }
                if ( chroma_scaling_from_luma || num_cb_points > 0 )
                {
                    sb.Append($"\r\n    |- ar_coeffs_cb_plus_128 =");
                    for ( var i = 0; i < numPosChroma; i++ )
                    {
                        sb.Append($"{ar_coeffs_cb_plus_128[i]}");
                    }
                }
                if ( chroma_scaling_from_luma || num_cr_points > 0 )
                {
                    sb.Append($"\r\n    |- ar_coeffs_cr_plus_128 =");
                    for ( var i = 0; i < numPosChroma; i++ )
                    {
                        sb.Append($"{ar_coeffs_cr_plus_128[i]}");
                    }
                }
                sb.Append($"\r\n    |- ar_coeff_shift_minus_6 = {ar_coeff_shift_minus_6}");
                sb.Append($"\r\n    |- grain_scale_shift = {grain_scale_shift}");

                if ( num_cb_points > 0 )
                {
                    sb.Append($"\r\n    |- cb_mult = {cb_mult}");
                    sb.Append($"\r\n    |- cb_luma_mult = {cb_luma_mult}");
                    sb.Append($"\r\n    |- cb_offset = {cb_offset}");
                }

                if ( num_cr_points > 0 )
                {
                    sb.Append($"\r\n    |- cr_mult = {cr_mult}");
                    sb.Append($"\r\n    |- cr_luma_mult = {cr_luma_mult}");
                    sb.Append($"\r\n    |- cr_offset = {cr_offset}");
                }

                sb.Append($"\r\n    |- overlap_flag = {overlap_flag}");
                sb.Append($"\r\n    |- clip_to_restricted_range = {clip_to_restricted_range}");
            }
            
            return sb.ToString ();
        }
    }

    public struct OBUTemporal_point_info
    {
        public uint frame_presentation_time;

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Temporal_point_info"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"frame_presentation_time = {frame_presentation_time}"));
            return sb;
        }

        public new readonly string ToString ()
        {
            return $"";
        }
    }

    public struct OBUSuperres_params
    {
        public bool use_superres;
        public byte coded_denom;

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Superres_params"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"use_superres = {use_superres}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"coded_denom =  {coded_denom}"));
            return sb;
        }

        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   - use_superres = {use_superres}");
            sb.Append($"\r\n   - coded_denom =  {coded_denom}");

            return sb.ToString();
        }
    }

    public struct OBUInterpolation_filter
    {
        public bool is_filter_switchable;
        public byte interpolation_filter;
        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Interpolation_filter"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"is_filter_switchable = {is_filter_switchable}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"interpolation_filter =  {interpolation_filter}"));
            return sb;
        }
        public override string ToString ()
        {
            StringBuilder sb = new();
            sb.Append($"\r\n   - is_filter_switchable = {is_filter_switchable}");
            sb.Append($"\r\n   - interpolation_filter =  {interpolation_filter}");

            return sb.ToString();
        }
    }

    public struct OBUTile_info
    {
        public bool uniform_tile_spacing_flag;
        public ushort TileRows;
        public ushort TileCols;
        public uint context_update_tile_id;
        public byte tile_size_bytes_minus_1;
        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Tile_info"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"uniform_tile_spacing_flag = {uniform_tile_spacing_flag}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"TileRows = {TileRows}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"TileCols = {TileCols}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"context_update_tile_id = {context_update_tile_id}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"tile_size_bytes_minus_1 = {tile_size_bytes_minus_1}"));
            return sb;
        }
        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n    - uniform_tile_spacing_flag = {uniform_tile_spacing_flag}");
            sb.Append($"\r\n    - TileRows = {TileRows}");
            sb.Append($"\r\n    - TileCols = {TileCols}");
            sb.Append($"\r\n    - context_update_tile_id = {context_update_tile_id}");
            sb.Append($"\r\n    - tile_size_bytes_minus_1 = {tile_size_bytes_minus_1}");

            return sb.ToString();
        }
    }

    public struct OBUQuantization_params
    {
        public byte base_q_idx;
        public bool diff_uv_delta;
        public bool using_qmatrix;
        public byte qm_y;
        public byte qm_u;
        public byte qm_v;
        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Quantization_params"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"base_q_idx = {base_q_idx}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"diff_uv_delta = {diff_uv_delta}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"using_qmatrix = {using_qmatrix}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"qm_y = {qm_y}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"qm_u = {qm_u}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"qm_v = {qm_v}"));
            return sb;
        }
        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n    - base_q_idx = {base_q_idx}");
            sb.Append($"\r\n    - diff_uv_delta = {diff_uv_delta}");
            sb.Append($"\r\n    - using_qmatrix = {using_qmatrix}");
            sb.Append($"\r\n    - qm_y = {qm_y}");
            sb.Append($"\r\n    - qm_u = {qm_u}");
            sb.Append($"\r\n    - qm_v = {qm_v}");

            return sb.ToString();
        }
    }

    public struct OBUSegmentation_params
    {
        public bool segmentation_enabled;
        public bool segmentation_update_map;
        public int segmentation_temporal_update;
        public int segmentation_update_data;
        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Segmentation_params"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"segmentation_enabled = {segmentation_enabled}"));
            sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"segmentation_update_map =  {segmentation_update_map}"));
            sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, $"segmentation_temporal_update = {segmentation_temporal_update}"));
            sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"segmentation_update_data =  {segmentation_update_data}"));
            return sb;
        }
        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   - segmentation_enabled = {segmentation_enabled}");
            sb.Append($"\r\n   - segmentation_update_map =  {segmentation_update_map}");
            sb.Append($"\r\n   - segmentation_temporal_update = {segmentation_temporal_update}");
            sb.Append($"\r\n   - segmentation_update_data =  {segmentation_update_data}");

            return sb.ToString();
        }
    }

    public struct OBUDelta_q_params
    {
        public bool delta_q_present;
        public byte delta_q_res;
        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Delta_q_params"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"delta_q_present = {delta_q_present}"));
            sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"delta_q_res =  {delta_q_res}"));
            return sb;
        }
        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   - delta_q_present = {delta_q_present}");
            sb.Append($"\r\n   - delta_q_res =  {delta_q_res}");

            return sb.ToString();
        }
    }

    public struct OBUDelta_lf_params
    {
        public bool delta_lf_present;
        public byte delta_lf_res;
        public bool delta_lf_multi;
        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Delta_lf_params"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"delta_lf_present = {delta_lf_present}"));
            if ( delta_lf_present )
            {
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"delta_lf_res =  {delta_lf_res}"));
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"delta_lf_multi =  {delta_lf_multi}"));
            }
            return sb;
        }
        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   - delta_lf_present = {delta_lf_present}");
            if(delta_lf_present)
            { 
                sb.Append($"\r\n   - delta_lf_res =  {delta_lf_res}");
                sb.Append($"\r\n   - delta_lf_multi =  {delta_lf_multi}");
            }

            return sb.ToString();
        }
    }

    public class OBULoop_filter_params
    {
        public byte[] loop_filter_level = new byte[4];
        public byte loop_filter_sharpness;
        public bool loop_filter_delta_enabled;
        public bool loop_filter_delta_update;
        public int[] update_ref_delta = new int[8];
        public sbyte[] loop_filter_ref_deltas = new sbyte[8];
        public int[] update_mode_delta = new int[8];
        public sbyte[] loop_filter_mode_deltas = new sbyte[8];

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Loop_filter_params"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"loop_filter_level = {TypeToString.ByteArray(loop_filter_level)}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"loop_filter_sharpness =  {loop_filter_sharpness}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"loop_filter_delta_enabled =  {loop_filter_delta_enabled}"));
            if ( loop_filter_delta_enabled )
            {
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"loop_filter_delta_update =  {loop_filter_delta_update}"));
                if ( loop_filter_delta_update )
                {
                    for ( var i = 0; i < (int) OBULimited.TOTAL_REFS_PER_FRAME; i++ )
                    {
                        sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, 
                            $"(update_ref_delta,loop_filter_ref_deltas) = {update_ref_delta[i]}, {loop_filter_ref_deltas[i]} "));
                    }
                    for ( var i = 0; i < 2; i++ )
                    {
                        sb.Enqueue(new STItem(3, STItemNoteType.NoteType_node, 
                            $"(update_mode_delta,loop_filter_mode_deltas) = {update_mode_delta[i]}, {loop_filter_mode_deltas[i]} "));
                    }
                }
            }
            return sb;
        }
        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   - loop_filter_level = {TypeToString.ByteArray(loop_filter_level)}");
            sb.Append($"\r\n   - loop_filter_sharpness =  {loop_filter_sharpness}");
            sb.Append($"\r\n   - loop_filter_delta_enabled =  {loop_filter_delta_enabled}");
            if ( loop_filter_delta_enabled )
            {
                sb.Append($"\r\n   - loop_filter_delta_update =  {loop_filter_delta_update}");
                if ( loop_filter_delta_update )
                {
                    for ( var i = 0; i < (int) OBULimited.TOTAL_REFS_PER_FRAME; i++ )
                    {
                        sb.Append($"\r\n   - (update_ref_delta,loop_filter_ref_deltas) = {update_ref_delta[i]}, {loop_filter_ref_deltas[i]} ");
                    }
                    for ( var i = 0; i < 2; i++ )
                    {
                        sb.Append($"\r\n   - (update_mode_delta,loop_filter_mode_deltas) = {update_mode_delta[i]}, {loop_filter_mode_deltas[i]} ");
                    }
                }
            }

            return sb.ToString();
        }
    }

    public class OBUCdef_params
    {
        public byte cdef_damping_minus_3;
        public byte cdef_bits;
        public byte[] cdef_y_pri_strength = new byte[8];
        public byte[] cdef_y_sec_strength = new byte[8];
        public byte[] cdef_uv_pri_strength = new byte[8];
        public byte[] cdef_uv_sec_strength = new byte[8];

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Cdef_params"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"cdef_damping_minus_3 = {cdef_damping_minus_3}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"cdef_bits = {cdef_bits} (y,uv(pri,sec))"));
            for ( var i = 0; i < (1 << cdef_bits); i++ )
            {
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"cdef{i} - ({cdef_y_pri_strength[i]},{cdef_y_sec_strength[i]}),({cdef_uv_pri_strength[i]},{cdef_uv_sec_strength[i]})"));
            }
            return sb;
        }
        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   - cdef_damping_minus_3 = {cdef_damping_minus_3}");
            sb.Append($"\r\n   - cdef_bits =  {cdef_bits}");

            for ( var i = 0; i < (1 << cdef_bits); i++ )
            {
                sb.Append($"\r\n   -- ");
                sb.Append($"\r\n     - cdef_y_pri_strength =  {cdef_y_pri_strength[i]}");
                sb.Append($"\r\n     - cdef_y_sec_strength =  {cdef_y_sec_strength[i]}");
                sb.Append($"\r\n     - cdef_uv_pri_strength =  {cdef_uv_pri_strength[i]}");
                sb.Append($"\r\n     - cdef_uv_sec_strength =  {cdef_uv_sec_strength[i]}");
            }

            return sb.ToString();
        }
    }

    public class OBULr_params
    {
        public byte[] lr_type = new byte[3];
        public byte lr_unit_shift;
        public int lr_uv_shift;

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Lr_params"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"lr_type = {TypeToString.ByteArray(lr_type)}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"lr_unit_shift =  {lr_unit_shift}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"lr_uv_shift =  {lr_uv_shift}"));
            return sb;
        }
        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   - lr_type = {TypeToString.ByteArray(lr_type)}");
            sb.Append($"\r\n   - lr_unit_shift =  {lr_unit_shift}");
            sb.Append($"\r\n   - lr_uv_shift =  {lr_uv_shift}");

            return sb.ToString();
        }
    }

    public class OBUGlobal_motion_params
    {
        public byte[] gm_type = new byte[8];
        public int[,] gm_params = new int[8, 6];
        public uint[,] prev_gm_params = new uint[8, 6];
        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"Global_motion_params"));
            for ( int i = 0; i < 8; i++ ) 
            {
                string d = gm_type[i] == (byte)OBULimited.IDENTITY? "IDENTITY"
                    :gm_type[i] == (byte)OBULimited.TRANSLATION? "TRANSLATION"
                    :gm_type[i] == (byte)OBULimited.ROTZOOM? "ROTZOOM"
                    :"AFFINE";
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"gm-params {i} ({d} - {gm_type[i]})"));
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, 
                    $"cur =  {gm_params[i, 0]},{gm_params[i, 1]},{gm_params[i, 2]},{gm_params[i, 3]},{gm_params[i, 4]},{gm_params[i, 5]}"));
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, 
                    $"prev = {prev_gm_params[i, 0]},{prev_gm_params[i, 1]},{prev_gm_params[i, 2]},{prev_gm_params[i, 3]},{prev_gm_params[i, 4]},{prev_gm_params[i, 5]}"));
            }
            return sb;
        }
        public override string ToString ()
        {
            StringBuilder sb = new();

            for(int i = 0; i < 8; i++ )
            {
                sb.Append($"\r\n  --");
                sb.Append($"\r\n   - gm_type = {gm_type[i]}");
                sb.Append($"\r\n   - gm_params =  {gm_params[i, 0]},{gm_params[i, 1]},{gm_params[i, 2]},{gm_params[i, 3]},{gm_params[i, 4]},{gm_params[i, 5]}");
                sb.Append($"\r\n   - prev_gm_params = {prev_gm_params[i, 0]},{prev_gm_params[i, 1]},{prev_gm_params[i, 2]},{prev_gm_params[i, 3]},{prev_gm_params[i, 4]},{prev_gm_params[i, 5]}");
            }
            return sb.ToString();
        }
    }

    public class OBUFrameHeader
    {
        public bool show_existing_frame;
        public byte frame_to_show_map_idx;
        public OBUTemporal_point_info temporal_point_info;
        public uint display_frame_id;
        public OBUFrameType frame_type;
        public bool show_frame;
        public bool showable_frame;
        public bool error_resilient_mode;
        public bool disable_cdf_update;
        public bool allow_screen_content_tools;
        public bool force_integer_mv;
        public uint current_frame_id;
        public bool frame_size_override_flag;
        public byte order_hint;
        public byte primary_ref_frame;
        public bool buffer_removal_time_present_flag;
        public uint[] buffer_removal_time = new uint[32];
        public byte refresh_frame_flags;
        public byte[] ref_order_hint = new byte[8];
        public uint frame_width_minus_1;
        public uint frame_height_minus_1;
        public OBUSuperres_params superres_params;
        public bool render_and_frame_size_different;
        public ushort render_width_minus_1;
        public ushort render_height_minus_1;
        public uint RenderWidth;
        public uint RenderHeight;
        public bool allow_intrabc;
        public bool frame_refs_short_signaling;
        public byte last_frame_idx;
        public byte gold_frame_idx;
        public byte[] ref_frame_idx = new byte[7];
        public byte[] delta_frame_id_minus_1 = new byte[7];
        public bool found_ref;
        public bool allow_high_precision_mv;
        public OBUInterpolation_filter interpolation_filter;
        public bool is_motion_mode_switchable;
        public bool use_ref_frame_mvs;
        public bool disable_frame_end_update_cdf;
        public OBUTile_info tile_info;
        public OBUQuantization_params quantization_params;
        public OBUSegmentation_params segmentation_params;
        public OBUDelta_q_params delta_q_params;
        public OBUDelta_lf_params delta_lf_params;
        public OBULoop_filter_params loop_filter_params = new();
        public OBUCdef_params cdef_params = new();
        public OBULr_params lr_params = new();
        public bool tx_mode_select;
        public bool skip_mode_present;
        public bool reference_select;
        public bool allow_warped_motion;
        public bool reduced_tx_set;
        public OBUGlobal_motion_params global_motion_params = new();
        public OBUFilmGrainParameters film_grain_params = new();

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            bool FrameIsIntra = frame_type == OBUFrameType.OBU_INTRA_ONLY_FRAME || frame_type == OBUFrameType.OBU_KEY_FRAME;

            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"FrameHeader"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"show_existing_frame = {show_existing_frame}"));
            // if ( show_existing_frame )
            {
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"frame_to_show_map_idx = {frame_to_show_map_idx}"));
            }
            //if ( h.decoder_model_info_present_flag && !h.timing_info.equal_picture_interval )
            {
                //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"temporal_point_info"));
                var v = temporal_point_info.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            //if ( h.frame_id_numbers_present_flag )
            {
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"display_frame_id = {display_frame_id}"));
            }
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"frame_type = {frame_type}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"show_frame = {show_frame}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"showable_frame = {showable_frame}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"error_resilient_mode = {error_resilient_mode}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"disable_cdf_update = {disable_cdf_update}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"allow_screen_content_tools = {allow_screen_content_tools}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"force_integer_mv = {force_integer_mv}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"current_frame_id = {current_frame_id}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"frame_size_override_flag = {frame_size_override_flag}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"order_hint = {order_hint}"));

            var prf = primary_ref_frame;
            if ( FrameIsIntra || error_resilient_mode )
            {
                prf = 0;
            }
            string primary_ref = primary_ref_frame == 7 ? "None" : prf.ToString();
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"primary_ref_frame = {primary_ref}"));

            //if ( h.decoder_model_info_present_flag )
            {
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"buffer_removal_time_present_flag = {buffer_removal_time_present_flag}"));
                if ( buffer_removal_time_present_flag )
                {
                    sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"buffer_removal_time  = {TypeToString.UintArray(buffer_removal_time)}"));
                }
            }

            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"refresh_frame_flags = 0x{refresh_frame_flags:X}, {TypeToString.Bits8(refresh_frame_flags)}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"ref_order_hint = {TypeToString.ByteArray(ref_order_hint)}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"frame_width_minus_1 = {frame_width_minus_1}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"frame_height_minus_1 = {frame_height_minus_1}"));
            {
                var v = superres_params.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"render_and_frame_size_different = {render_and_frame_size_different}"));
            if(render_and_frame_size_different)
            {
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"render_width_minus_1 = {render_width_minus_1}"));
                sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"render_height_minus_1 = {render_height_minus_1}"));
            }
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"RenderWidth = {RenderWidth}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"RenderHeight = {RenderHeight}"));
            if ( FrameIsIntra )
            {
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"allow_intrabc = {allow_intrabc}"));
            }
            else
            {
                /*if ( !h.enable_order_hint )
                {
                    sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"|- frame_refs_short_signaling = 0"));
                }
                else
                {
                }*/
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_logicalnode, $"frame_refs_short_signaling = {frame_refs_short_signaling}"));
                if ( frame_refs_short_signaling )
                {
                    sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"last_frame_idx = {last_frame_idx}"));
                    sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"gold_frame_idx = {gold_frame_idx}"));
                }

                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"ref_frame_idx = {TypeToString.ByteArray(ref_frame_idx)}"));
                sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"delta_frame_id_minus_1 = {TypeToString.ByteArray(delta_frame_id_minus_1)}"));
            }
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"found_ref = {found_ref}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"allow_high_precision_mv = {allow_high_precision_mv}"));
            {
                var v = interpolation_filter.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            //sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"interpolation_filter = {interpolation_filter}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"is_motion_mode_switchable = {is_motion_mode_switchable}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"use_ref_frame_mvs = {use_ref_frame_mvs}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"disable_frame_end_update_cdf = {disable_frame_end_update_cdf}"));
            //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"|- tile_info = {tile_info}"));
            {
                var v = tile_info.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"|- quantization_params = {quantization_params}"));
            {
                var v = quantization_params.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"|- segmentation_params = {segmentation_params}"));
            {
                var v = segmentation_params.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"|- delta_q_params = {delta_q_params}"));
            {
                var v = delta_q_params.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"|- delta_lf_params = {delta_lf_params}"));
            {
                var v = delta_lf_params.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"|- loop_filter_params = {loop_filter_params}"));
            {
                var v = loop_filter_params.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"|- cdef_params = {cdef_params}"));
            {
                var v = cdef_params.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            //sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"|- lr_params = {lr_params}"));
            {
                var v = lr_params.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"tx_mode_select = {tx_mode_select}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"skip_mode_present = {skip_mode_present}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"reference_select = {reference_select}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"allow_warped_motion = {allow_warped_motion}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"reduced_tx_set = {reduced_tx_set}"));
            {
                var v = global_motion_params.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }
            {
                var v = film_grain_params.ToSpecTree();
                foreach ( var v2 in v )
                {
                    var v3 = v2;
                    v3.level += 1;
                    sb.Enqueue(v3);
                }
            }

            return sb;
        }

        public override string ToString ()
        {
            bool FrameIsIntra = frame_type == OBUFrameType.OBU_INTRA_ONLY_FRAME || frame_type == OBUFrameType.OBU_KEY_FRAME;

            StringBuilder sb = new();
            sb.Append($"FrameHeader");
            sb.Append($"\r\n |- show_existing_frame = {show_existing_frame}");
            // if ( show_existing_frame )
            {
                sb.Append($"\r\n    - frame_to_show_map_idx = {frame_to_show_map_idx}");
            }
            //if ( h.decoder_model_info_present_flag && !h.timing_info.equal_picture_interval )
            {
                sb.Append($"\r\n |- temporal_point_info");
                sb.Append($"\r\n    - ( {temporal_point_info.ToString()} )");
            }
            //if ( h.frame_id_numbers_present_flag )
            {
                sb.Append($"\r\n |- display_frame_id = {display_frame_id}");
            }
            sb.Append($"\r\n |- frame_type = {frame_type}");
            sb.Append($"\r\n |- show_frame = {show_frame}");
            sb.Append($"\r\n |- showable_frame = {showable_frame}");
            sb.Append($"\r\n |- error_resilient_mode = {error_resilient_mode}");
            sb.Append($"\r\n |- disable_cdf_update = {disable_cdf_update}");
            sb.Append($"\r\n |- allow_screen_content_tools = {allow_screen_content_tools}");
            if ( FrameIsIntra )
                sb.Append($"\r\n |- force_integer_mv = {force_integer_mv}");
            sb.Append($"\r\n |- current_frame_id = {current_frame_id}");
            sb.Append($"\r\n |- frame_size_override_flag = {frame_size_override_flag}");
            sb.Append($"\r\n |- order_hint = {order_hint}");

            var prf = primary_ref_frame;
            if ( FrameIsIntra || error_resilient_mode )
            {
                prf = 0;
            }
            string primary_ref = primary_ref_frame == 7 ? "None" : prf.ToString();
            sb.Append($"\r\n |- primary_ref_frame = {primary_ref}");

            //if ( h.decoder_model_info_present_flag )
            {
                sb.Append($"\r\n |- buffer_removal_time_present_flag = {buffer_removal_time_present_flag}");
                if ( buffer_removal_time_present_flag )
                {
                    sb.Append($"\r\n    - buffer_removal_time  = {TypeToString.UintArray(buffer_removal_time)}");
                }
            }

            sb.Append($"\r\n |- refresh_frame_flags = 0x{refresh_frame_flags:X}, {TypeToString.Bits8(refresh_frame_flags)}");
            sb.Append($"\r\n |- ref_order_hint = {TypeToString.ByteArray(ref_order_hint)}");
            sb.Append($"\r\n |- frame_width_minus_1 = {frame_width_minus_1}");
            sb.Append($"\r\n |- frame_height_minus_1 = {frame_height_minus_1}");
            sb.Append($"\r\n |- superres_params = {superres_params}");
            sb.Append($"\r\n |- render_and_frame_size_different = {render_and_frame_size_different}");
            sb.Append($"\r\n |- render_width_minus_1 = {render_width_minus_1}");
            sb.Append($"\r\n |- render_height_minus_1 = {render_height_minus_1}");
            sb.Append($"\r\n |- RenderWidth = {RenderWidth}");
            sb.Append($"\r\n |- RenderHeight = {RenderHeight}");
            if ( FrameIsIntra )
            {
                sb.Append($"\r\n |- allow_intrabc = {allow_intrabc}");
            }
            else
            {
                /*if ( !h.enable_order_hint )
                {
                    sb.Append($"\r\n |- frame_refs_short_signaling = 0");
                }
                else*/
                {
                    sb.Append($"\r\n |- frame_refs_short_signaling = {frame_refs_short_signaling}");
                    if ( frame_refs_short_signaling )
                    {
                        sb.Append($"\r\n |- last_frame_idx = {last_frame_idx}");
                        sb.Append($"\r\n |_ gold_frame_idx = {gold_frame_idx}");
                    }

                    sb.Append($"\r\n |- ref_frame_idx = {TypeToString.ByteArray(ref_frame_idx)}");
                    sb.Append($"\r\n |- delta_frame_id_minus_1 = {TypeToString.ByteArray(delta_frame_id_minus_1)}");
                }
            }
            sb.Append($"\r\n |- found_ref = {found_ref}");
            sb.Append($"\r\n |- allow_high_precision_mv = {allow_high_precision_mv}");
            sb.Append($"\r\n |- interpolation_filter = {interpolation_filter}");
            sb.Append($"\r\n |- is_motion_mode_switchable = {is_motion_mode_switchable}");
            sb.Append($"\r\n |- use_ref_frame_mvs = {use_ref_frame_mvs}");
            sb.Append($"\r\n |- disable_frame_end_update_cdf = {disable_frame_end_update_cdf}");
            sb.Append($"\r\n |- tile_info = {tile_info}");
            sb.Append($"\r\n |- quantization_params = {quantization_params}");
            sb.Append($"\r\n |- segmentation_params = {segmentation_params}");
            sb.Append($"\r\n |- delta_q_params = {delta_q_params}");
            sb.Append($"\r\n |- delta_lf_params = {delta_lf_params}");
            sb.Append($"\r\n |- loop_filter_params = {loop_filter_params}");
            sb.Append($"\r\n |- cdef_params = {cdef_params}");
            sb.Append($"\r\n |- lr_params = {lr_params}");
            sb.Append($"\r\n |- tx_mode_select = {tx_mode_select}");
            sb.Append($"\r\n |- skip_mode_present = {skip_mode_present}");
            sb.Append($"\r\n |- reference_select = {reference_select}");
            sb.Append($"\r\n |- allow_warped_motion = {allow_warped_motion}");
            sb.Append($"\r\n |- reduced_tx_set = {reduced_tx_set}");
            sb.Append($"\r\n |- global_motion_params = {global_motion_params}");
            sb.Append($"\r\n |- film_grain_params = {film_grain_params}");

            return sb.ToString();
        }
    }

    public class OBUTileGroup
    {
        public ushort NumTiles;
        public bool tile_start_and_end_present_flag;
        public ushort tg_start;
        public ushort tg_end;
        public uint[] TileSize = new uint[1];

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();

            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"TileGroup"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"NumTiles = {NumTiles}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"tile_start_and_end_present_flag = {tile_start_and_end_present_flag}"));
            sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"tg_start = {tg_start}"));
            sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"tg_end = {tg_end}"));
            sb.Enqueue(new STItem(2, STItemNoteType.NoteType_node, $"[] TileSize = {TypeToString.UintArray(TileSize)}"));

            return sb;
        }

        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"TileGroup\r\n");

            sb.Append($" |- NumTiles = {NumTiles}\r\n");
            sb.Append($" |- tile_start_and_end_present_flag = {tile_start_and_end_present_flag}\r\n");
            sb.Append($"   - tg_start = {tg_start}\r\n");
            sb.Append($"   - tg_end = {tg_end}\r\n");
            sb.Append($"   - [] TileSize = { TypeToString.UintArray(TileSize) }\r\n");

            return sb.ToString();
        }
    }

    public class OBUTile_list_entry
    {
        public byte anchor_frame_idx;
        public byte anchor_tile_row;
        public byte anchor_tile_col;
        public ushort tile_data_size_minus_1;
        public byte[]? coded_tile_data;
        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();

            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"anchor_frame_idx = {anchor_frame_idx}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"anchor_tile_row = {anchor_tile_row}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"anchor_tile_col = {anchor_tile_col}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"tile_data_size_minus_1 = {tile_data_size_minus_1}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"[] coded_tile_data = {coded_tile_data?.Length}"));

            return sb;
        }
        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   - anchor_frame_idx = {anchor_frame_idx}\r\n");
            sb.Append($"\r\n   - anchor_tile_row = {anchor_tile_row}\r\n");
            sb.Append($"\r\n   - anchor_tile_col = {anchor_tile_col}\r\n");
            sb.Append($"\r\n   - tile_data_size_minus_1 = {tile_data_size_minus_1}\r\n");
            sb.Append($"\r\n   - [] coded_tile_data = {coded_tile_data?.Length}\r\n");

            return sb.ToString();
        }
    }

    public class OBUTileList
    {
        public byte output_frame_width_in_tiles_minus_1;
        public byte output_frame_height_in_tiles_minus_1;
        public ushort tile_count_minus_1;
        public OBUTile_list_entry[]? tile_list_entry;

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"output_frame_width_in_tiles_minus_1 = {output_frame_width_in_tiles_minus_1}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"output_frame_height_in_tiles_minus_1 = {output_frame_height_in_tiles_minus_1}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"tile_count_minus_1 = {tile_count_minus_1}"));
            for ( var i = 0; i <= tile_count_minus_1; i++ )
                sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"tile_list_entry = {tile_list_entry![i]}"));
            return sb;
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   - output_frame_width_in_tiles_minus_1 = {output_frame_width_in_tiles_minus_1}");
            sb.Append($"\r\n   - output_frame_height_in_tiles_minus_1 = {output_frame_height_in_tiles_minus_1}");
            sb.Append($"\r\n   - tile_count_minus_1 = {tile_count_minus_1}");
            for ( var i = 0; i <= tile_count_minus_1; i++ )
                sb.Append($"\r\n   - tile_list_entry = {tile_list_entry![i]}");

            return sb.ToString();
        }
    }

    public struct OBUMetadata
    {
        // Define the structure for OBP metadata fields
        public OBUMetadataType metadata_type;

        public OBUMetadataHdrCll metadata_hdr_cll;
        public OBUMetadataHdrMdcv metadata_hdr_mdcv;
        public OBUMetadataScalability metadata_scalability;
        public OBUMetadataItutT35 metadata_itut_t35;
        public OBUMetadataTimecode metadata_timecode;
        public OBUUnregisteredMetadata unregistered;

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_root, "MetaData"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"metadata_type = ${metadata_type}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"metadata_hdr_cll = ${metadata_hdr_cll}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"metadata_hdr_mdcv = ${metadata_hdr_mdcv}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"metadata_scalability = ${metadata_scalability}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"metadata_itut_t35 = ${metadata_itut_t35}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"metadata_timecode = ${metadata_timecode}"));
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"unregistered = ${unregistered}"));
            return sb;
        }

        public override string ToString ()
        {
            StringBuilder sb = new ();

            sb.Append("MetaData");
            sb.Append($"\r\n metadata_type = ${metadata_type}");
            sb.Append($"\r\n metadata_hdr_cll = ${metadata_hdr_cll}");
            sb.Append($"\r\n metadata_hdr_mdcv = ${metadata_hdr_mdcv}");
            sb.Append($"\r\n metadata_scalability = ${metadata_scalability}");
            sb.Append($"\r\n metadata_itut_t35 = ${metadata_itut_t35}");
            sb.Append($"\r\n metadata_timecode = ${metadata_timecode}");
            sb.Append($"\r\n unregistered = ${unregistered}");

            return sb.ToString();
        }
    }

    public struct OBUMetadataHdrCll
    {
        public ushort max_cll;
        public ushort max_fall;

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(1, STItemNoteType.NoteType_node, $"\r\n   - max_cll,max_fall = {max_cll},{max_fall}"));
            return sb;
        }

        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   - max_cll,max_fall = {max_cll},{max_fall}");

            return sb.ToString();
        }
    }

    public class OBUMetadataHdrMdcv
    {
        public ushort[] primary_chromaticity_x = new ushort[3];
        public ushort[] primary_chromaticity_y = new ushort[3];
        public ushort white_point_chromaticity_x;
        public ushort white_point_chromaticity_y;
        public uint luminance_max;
        public uint luminance_min;

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"primary_chromaticity_x,primary_chromaticity_y = ({primary_chromaticity_x[0]},{primary_chromaticity_y[0]}),({primary_chromaticity_x[1]},{primary_chromaticity_y[1]}),({primary_chromaticity_x[2]},{primary_chromaticity_y[2]})"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"white_point_chromaticity_x,white_point_chromaticity_y = ({white_point_chromaticity_x},{white_point_chromaticity_y})"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"luminance_max,luminance_min = ({luminance_max},{luminance_min})"));
            return sb;
        }

        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n   - primary_chromaticity_x,primary_chromaticity_y = ({primary_chromaticity_x[0]},{primary_chromaticity_y[0]}),({primary_chromaticity_x[1]},{primary_chromaticity_y[1]}),({primary_chromaticity_x[2]},{primary_chromaticity_y[2]})");
            sb.Append($"\r\n   - white_point_chromaticity_x,white_point_chromaticity_y = ({white_point_chromaticity_x},{white_point_chromaticity_y})");
            sb.Append($"\r\n   - luminance_max,luminance_min = ({luminance_max},{luminance_min})");

            return sb.ToString();
        }
    }

    public struct OBUMetadataScalability
    {
        public byte scalability_mode_idc;
        public OBUScalabilityStructure scalability_structure;
    }

    public class OBUScalabilityStructure
    {
        public byte spatial_layers_cnt_minus_1;
        public bool spatial_layer_dimensions_present_flag;
        public bool spatial_layer_description_present_flag;
        public bool temporal_group_description_present_flag;
        public byte scalability_structure_reserved_3bits;
        public ushort[] spatial_layer_max_width = new ushort[3];
        public ushort[] spatial_layer_max_height = new ushort[3];
        public byte[] spatial_layer_ref_id = new byte[3];
        public byte temporal_group_size;
        public byte[] temporal_group_temporal_id = new byte[256];
        public bool[] temporal_group_temporal_switching_up_point_flag = new bool[256];
        public bool[] temporal_group_spatial_switching_up_point_flag = new bool[256];
        public byte[] temporal_group_ref_cnt = new byte[256];
        public byte[,] temporal_group_ref_pic_diff = new byte[256, 8];
    }

    public struct OBUMetadataItutT35
    {
        public byte itu_t_t35_country_code;
        public byte itu_t_t35_country_code_extension_byte;
        public byte[]? itu_t_t35_payload_bytes;
        public int itu_t_t35_payload_bytes_size;
    }

    public struct OBUMetadataTimecode
    {
        public byte counting_type;
        public bool full_timestamp_flag;
        public bool discontinuity_flag;
        public bool cnt_dropped_flag;
        public ushort n_frames;
        public bool seconds_flag;
        public byte seconds_value;
        public bool minutes_flag;
        public byte minutes_value;
        public bool hours_flag;
        public byte hours_value;
        public byte time_offset_length;
        public byte time_offset_value;

        public SpecTree ToSpecTree ()
        {
            SpecTree sb = new ();
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"counting_type = {counting_type}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"full_timestamp_flag = {full_timestamp_flag}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"discontinuity_flag = {discontinuity_flag}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"cnt_dropped_flag = {cnt_dropped_flag}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"n_frames = {n_frames}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"seconds_flag = {seconds_flag}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"seconds_value = {seconds_value}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"minutes_flag = {minutes_flag}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"minutes_value = {minutes_value}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"hours_flag = {hours_flag}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"hours_value = {hours_value}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"time_offset_length = {time_offset_length}"));
            sb.Enqueue(new STItem(0, STItemNoteType.NoteType_node, $"time_offset_value = {time_offset_value}"));
            return sb;
        }

        public override string ToString ()
        {
            StringBuilder sb = new();

            sb.Append($"\r\n    |- counting_type = {counting_type}");
            sb.Append($"\r\n    |- full_timestamp_flag = {full_timestamp_flag}");
            sb.Append($"\r\n    |- discontinuity_flag = {discontinuity_flag}");
            sb.Append($"\r\n    |- cnt_dropped_flag = {cnt_dropped_flag}");
            sb.Append($"\r\n    |- n_frames = {n_frames}");
            sb.Append($"\r\n    |- seconds_flag = {seconds_flag}");
            sb.Append($"\r\n    |- seconds_value = {seconds_value}");
            sb.Append($"\r\n    |- minutes_flag = {minutes_flag}");
            sb.Append($"\r\n    |- minutes_value = {minutes_value}");
            sb.Append($"\r\n    |- hours_flag = {hours_flag}");
            sb.Append($"\r\n    |- hours_value = {hours_value}");
            sb.Append($"\r\n    |- time_offset_length = {time_offset_length}");
            sb.Append($"\r\n    |_ time_offset_value = {time_offset_value}");

            return sb.ToString ();
        }
    }

    public class OBUUnregisteredMetadata
    {
        public byte[]? buf;
    }

    /************************************
     * Functions from AV1 specification. *
     ************************************/

    public class OBPBitReader
    {
        public byte[] Buffer = new byte[1];
        public int BufferPosition;
        public ulong BitBuffer;
        public int BitsInBuffer;
    }

    internal class AV1Bits
    {
        public static OBPBitReader ObpNewBr (Span<byte> buf)
        {
            OBPBitReader ret = new()
            {
                Buffer = buf.ToArray(),
                BufferPosition = 0,
                BitBuffer = 0,
                BitsInBuffer = 0
            };

            return ret;
        }

        public static void ObpBrByteAligment (ref OBPBitReader br)
        {
            br.BitsInBuffer -= (byte) (br.BitsInBuffer % 8);
        }

        public static int ObpBrGetPos (ref OBPBitReader br)
        {
            return (br.BufferPosition * 8) - ((int) br.BitsInBuffer);
        }

        public static int ObpBr (out ulong x, ref OBPBitReader br, int n, ref OBPError err)
        {
            x = 0;
            do
            {
                int bytesNeeded = (n - br.BitsInBuffer + (1 << 3) - 1) >> 3;
                if ( bytesNeeded > (br.Buffer.Length - br.BufferPosition) )
                {
                    err.error = "Ran out of bytes in buffer.";
                    return -1;
                }
                x = ObpBrUnchecked(ref br, n);
            } while ( false );

            return 0;
        }

        public static ulong ObpBrUnchecked (ref OBPBitReader br, int n)
        {
            Debug.Assert(n <= 63);

            while ( n > br.BitsInBuffer )
            {
                br.BitBuffer <<= 8;
                br.BitBuffer |= br.Buffer[br.BufferPosition];
                br.BitsInBuffer += 8;
                br.BufferPosition++;

                if ( br.BitsInBuffer > 56 )
                {
                    if ( n <= br.BitsInBuffer )
                        break;

                    if ( n <= 64 )
                        return (ObpBrUnchecked(ref br, 32) << 32) | ObpBrUnchecked(ref br, n - 32);
                }
            }

            br.BitsInBuffer -= n;
            return (br.BitBuffer >> br.BitsInBuffer) & ((((ulong) 1) << n) - 1);
        }

        public static int ObpLeb128 (Span<byte> buf, out ulong value, out long consumed, ref OBPError err)
        {
            value = 0;
            consumed = 0;

            for ( ulong i = 0; i < 8; i++ )
            {
                if ( (consumed + 1) > buf.Length )
                {
                    err.error = "Buffer too short to read leb128 value.";
                    return -1;
                }

                byte b = buf[(int)consumed];
                value |= ((ulong) (b & 0x7F)) << (int) (i * 7);
                consumed++;

                if ( (b & 0x80) != 0x80 )
                    break;
            }

            return 0;
        }

        public static int ObpUvlc (ref OBPBitReader br, out uint value, ref OBPError err)
        {
            uint leadingZeroes = 0;
            value = 0;
            while ( leadingZeroes < 32 )
            {
                if ( ObpBr(out ulong b, ref br, 1, ref err) != 0 )
                    return -1;
                if ( b != 0 )
                    break;
                leadingZeroes++;
            }
            if ( leadingZeroes == 32 )
            {
                err.error = "Invalid VLC.";
                return -1;
            }
            if ( ObpBr(out ulong val, ref br, (byte) leadingZeroes, ref err) != 0 )
                return -1;
            value = (uint) (val + ((1u << (int) leadingZeroes) - 1u));
            return 0;
        }

        public static int ObpGetRelativeDist (int a, int b, OBUSequenceHeader seq)
        {
            int diff, m;

            if ( !seq.enable_order_hint )
                return 0;

            diff = a - b;
            m = 1 << (seq.OrderHintBits - 1);
            diff = (diff & (m - 1)) - (diff & m);

            return diff;
        }

        public static uint ObpTileLog2 (uint blkSize, uint target)
        {
            uint k;
            for ( k = 0; (blkSize << (int) k) < target; k++ )
            {
            }
            return k;
        }

        public static uint ObpFloorLog2 (uint a)
        {
            uint s = 0;
            uint x = a;
            while ( x != 0 )
            {
                x >>= 1;
                s++;
            }
            return s - 1;
        }

        public static ulong ObpLe (Span<byte> buf)
        {
            ulong t = 0;
            int pos = 0;
            for ( byte i = 0; i < buf.Length; i++ )
            {
                byte byteVal = buf[pos];
                t += ((ulong) byteVal) << (i * 8);
                pos++;
            }
            return t;
        }

        public static int ObpNs (ref OBPBitReader br, uint n, out uint outValue, ref OBPError err)
        {
            uint w = ObpFloorLog2(n) + 1;
            uint m = ((uint)1 << (int)w) - n;
            ulong v;
            ulong extraBit;
            outValue = 0;

            Debug.Assert(w - 1 <= 32);
            if ( ObpBr(out v, ref br, (byte) (w - 1), ref err) != 0 )
                return -1;
            if ( v < m )
            {
                outValue = (uint) v;
                return 0;
            }
            if ( ObpBr(out extraBit, ref br, 1, ref err) != 0 )
                return -1;
            outValue = (uint) ((v << 1) - m + extraBit);
            return 0;
        }

        public static int ObpSu (ref OBPBitReader br, uint n, out int outValue, ref OBPError err)
        {
            ulong value;
            ulong signMask;
            byte v = (byte)n;
            outValue = 0;

            if ( ObpBr(out value, ref br, v, ref err) != 0 )
                return -1;
            signMask = ((uint) 1) << (int) (n - 1);
            if ( (value & signMask) != 0 )
            {
                value -= 2 * signMask;
            }
            outValue = (int) value;
            return 0;
        }

        public static int ObpDecodeSubexp (OBPBitReader br, int numSyms, out uint outValue, ref OBPError err)
        {
            int i = 0;
            int mk = 0;
            int k = 3;

            outValue = 0;

            while ( true )
            {
                int b2 = i != 0 ? k + i - 1 : k;
                int a = 1 << b2;
                if ( numSyms <= mk + 3 * a )
                {
                    int ret = ObpNs(ref br, (uint)(numSyms - mk), out uint val, ref err);
                    if ( ret < 0 )
                    {
                        return -1;
                    }
                    outValue = val;
                    return 0;
                }
                else
                {
                    if ( ObpBr(out ulong subexpMoreBits, ref br, 1, ref err) != 0 )
                        return -1;
                    if ( subexpMoreBits != 0 )
                    {
                        i++;
                        mk += a;
                    }
                    else
                    {
                        Debug.Assert(b2 <= 255);
                        if ( ObpBr(out ulong subexpBits, ref br, (byte) b2, ref err) != 0 )
                            return -1;
                        outValue = (uint) (subexpBits + (uint) mk);
                        return 0;
                    }
                }
            }
        }

        public static int ObpsInverseRecenter (int r, uint v)
        {
            if ( (ulong) v > (ulong) (2 * r) )
            {
                return (int) v;
            }
            else if ( (v & 1) != 0 )
            {
                return r - ((int) (v + 1) >> 1);
            }
            else
            {
                return r + ((int) v >> 1);
            }
        }

        public static int ObpDecodeUnsignedSubexpWithRef (OBPBitReader br, int mx, int r, out short outValue, ref OBPError err)
        {
            outValue = 0;
            int ret = ObpDecodeSubexp(br, mx, out uint v, ref err);
            if ( ret < 0 )
            {
                return -1;
            }
            if ( r < 0 )
            {
                if ( -(-r << 1) <= mx )
                {
                    outValue = (short) ObpsInverseRecenter(r, v);
                    return 0;
                }
            }
            if ( (r << 1) <= mx )
            {
                outValue = (short) ObpsInverseRecenter(r, v);
                return 0;
            }
            else
            {
                outValue = (short) (mx - 1 - ObpsInverseRecenter(mx - 1 - r, v));
                return 0;
            }
        }

        public static int ObpDecodeSignedSubexpWithRef (ref OBPBitReader br, int low, int high, int r, out short outValue, ref OBPError err)
        {
            outValue = 0;
            int ret = ObpDecodeUnsignedSubexpWithRef(br, high - low, r - low, out short val, ref err);
            if ( ret < 0 )
            {
                return -1;
            }
            outValue = (short) (val + low);
            return 0;
        }
    }
}