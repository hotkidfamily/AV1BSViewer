using System.Text;

namespace AV1bitsAnalyzer.Library
{
    internal class SpecString
    {
        public static string ToFrameHeaderString(OBUFrameHeader v, OBUSequenceHeader h)
        {
            bool FrameIsIntra = v.frame_type == OBUFrameType.OBU_INTRA_ONLY_FRAME || v.frame_type == OBUFrameType.OBU_KEY_FRAME;

            StringBuilder sb = new();
            sb.Append($"FrameHeader");
            sb.Append($"\r\n |- show_existing_frame = {v.show_existing_frame}");
            // if ( v.show_existing_frame )
            {
                sb.Append($"\r\n    - frame_to_show_map_idx = {v.frame_to_show_map_idx}");
            }
            if ( h.decoder_model_info_present_flag && !h.timing_info.equal_picture_interval )
            {
                sb.Append($"\r\n |- temporal_point_info");
                sb.Append($"\r\n    - ( {v.temporal_point_info.ToString()} )");
            }
            if ( h.frame_id_numbers_present_flag )
            {
                sb.Append($"\r\n |- display_frame_id = {v.display_frame_id}");
            }
            sb.Append($"\r\n |- frame_type = {v.frame_type}");
            sb.Append($"\r\n |- show_frame = {v.show_frame}");
            sb.Append($"\r\n |- showable_frame = {v.showable_frame}");
            sb.Append($"\r\n |- error_resilient_mode = {v.error_resilient_mode}");
            sb.Append($"\r\n |- disable_cdf_update = {v.disable_cdf_update}");
            sb.Append($"\r\n |- allow_screen_content_tools = {v.allow_screen_content_tools}");
            if ( FrameIsIntra )
                sb.Append($"\r\n |- force_integer_mv = {v.force_integer_mv}");
            sb.Append($"\r\n |- current_frame_id = {v.current_frame_id}");
            sb.Append($"\r\n |- frame_size_override_flag = {v.frame_size_override_flag}");
            sb.Append($"\r\n |- order_hint = {v.order_hint}");

            var primary_ref_frame = v.primary_ref_frame;
            if ( FrameIsIntra || v.error_resilient_mode )
            {
                primary_ref_frame = 0;
            }
            sb.Append($"\r\n |- primary_ref_frame = {primary_ref_frame}");

            if ( h.decoder_model_info_present_flag )
            {
                sb.Append($"\r\n |- buffer_removal_time_present_flag = {v.buffer_removal_time_present_flag}");
                if ( v.buffer_removal_time_present_flag )
                {
                    sb.Append($"\r\n    - buffer_removal_time  = {TypeToString.UintArray(v.buffer_removal_time)}");
                }
            }

            sb.Append($"\r\n |- refresh_frame_flags = {v.refresh_frame_flags}");
            sb.Append($"\r\n |- ref_order_hint = {TypeToString.ByteArray(v.ref_order_hint)}");
            sb.Append($"\r\n |- frame_width_minus_1 = {v.frame_width_minus_1}");
            sb.Append($"\r\n |- frame_height_minus_1 = {v.frame_height_minus_1}");
            sb.Append($"\r\n |- superres_params = {v.superres_params}");
            sb.Append($"\r\n |- render_and_frame_size_different = {v.render_and_frame_size_different}");
            sb.Append($"\r\n |- render_width_minus_1 = {v.render_width_minus_1}");
            sb.Append($"\r\n |- render_height_minus_1 = {v.render_height_minus_1}");
            sb.Append($"\r\n |- RenderWidth = {v.RenderWidth}");
            sb.Append($"\r\n |- RenderHeight = {v.RenderHeight}");
            if ( FrameIsIntra )
            {
                sb.Append($"\r\n |- allow_intrabc = {v.allow_intrabc}");
            }
            else
            {
                if ( !h.enable_order_hint )
                {
                    sb.Append($"\r\n |- frame_refs_short_signaling = 0");
                }
                else
                {
                    sb.Append($"\r\n |- frame_refs_short_signaling = {v.frame_refs_short_signaling}");
                    if ( v.frame_refs_short_signaling )
                    {
                        sb.Append($"\r\n |- last_frame_idx = {v.last_frame_idx}");
                        sb.Append($"\r\n |_ gold_frame_idx = {v.gold_frame_idx}");
                    }

                    sb.Append($"\r\n |- ref_frame_idx = {TypeToString.ByteArray(v.ref_frame_idx)}");
                    sb.Append($"\r\n |- delta_frame_id_minus_1 = {TypeToString.ByteArray(v.delta_frame_id_minus_1)}");
                }
            }
            sb.Append($"\r\n |- found_ref = {v.found_ref}");
            sb.Append($"\r\n |- allow_high_precision_mv = {v.allow_high_precision_mv}");
            sb.Append($"\r\n |- interpolation_filter = {v.interpolation_filter}");
            sb.Append($"\r\n |- is_motion_mode_switchable = {v.is_motion_mode_switchable}");
            sb.Append($"\r\n |- use_ref_frame_mvs = {v.use_ref_frame_mvs}");
            sb.Append($"\r\n |- disable_frame_end_update_cdf = {v.disable_frame_end_update_cdf}");
            sb.Append($"\r\n |- tile_info = {v.tile_info}");
            sb.Append($"\r\n |- quantization_params = {v.quantization_params}");
            sb.Append($"\r\n |- segmentation_params = {v.segmentation_params}");
            sb.Append($"\r\n |- delta_q_params = {v.delta_q_params}");
            sb.Append($"\r\n |- delta_lf_params = {v.delta_lf_params}");
            sb.Append($"\r\n |- loop_filter_params = {v.loop_filter_params}");
            sb.Append($"\r\n |- cdef_params = {v.cdef_params}");
            sb.Append($"\r\n |- lr_params = {v.lr_params}");
            sb.Append($"\r\n |- tx_mode_select = {v.tx_mode_select}");
            sb.Append($"\r\n |- skip_mode_present = {v.skip_mode_present}");
            sb.Append($"\r\n |- reference_select = {v.reference_select}");
            sb.Append($"\r\n |- allow_warped_motion = {v.allow_warped_motion}");
            sb.Append($"\r\n |- reduced_tx_set = {v.reduced_tx_set}");
            sb.Append($"\r\n |- global_motion_params = {v.global_motion_params}");
            sb.Append($"\r\n |- film_grain_params = {v.film_grain_params}");


            return sb.ToString();
        }

        public static string ToSeqHeaderString (OBUSequenceHeader v)
        {
            StringBuilder sb = new();

            sb.Append($"SequenceHeader");
            sb.Append($"\r\n |- seq_profile = {v.seq_profile}");
            sb.Append($"\r\n |- still_picture = {v.still_picture}");
            sb.Append($"\r\n |- reduced_still_picture_v = {v.reduced_still_picture_header}");
            if ( v.reduced_still_picture_header )
            {
                sb.Append($"\r\n |- timing_info_present_flag = {v.timing_info_present_flag}");
                sb.Append($"\r\n |- decoder_model_info_present_flag = {v.decoder_model_info_present_flag}");
                sb.Append($"\r\n |- initial_display_delay_present_flag = {v.initial_display_delay_present_flag}");
                sb.Append($"\r\n |- operating_points_cnt_minus_1 = {v.operating_points_cnt_minus_1}");
                sb.Append($"\r\n |- operating_point_idc = {v.operating_point_idc[0]}");
                sb.Append($"\r\n |- seq_level_idx = {v.seq_level_idx[0]}");
                sb.Append($"\r\n |- seq_level_idx = {v.seq_level_idx[0]}");
                sb.Append($"\r\n |- seq_tier = {v.seq_tier[0]}");
                sb.Append($"\r\n |- decoder_model_present_for_this_op = {v.decoder_model_present_for_this_op[0]}");
                sb.Append($"\r\n |- initial_display_delay_present_for_this_op = {v.initial_display_delay_present_for_this_op[0]}");
            }
            else
            {
                sb.Append($"\r\n |- timing_info_present_flag = {v.timing_info_present_flag}");
                if ( v.timing_info_present_flag )
                {
                    sb.Append($"\r\n   - timing_info = ( {v.timing_info.ToString()} )");
                }

                sb.Append($"\r\n |- decoder_model_info_present_flag = {v.decoder_model_info_present_flag}");
                if ( v.decoder_model_info_present_flag )
                    sb.Append($"\r\n   - decoder_model_info = ( {v.decoder_model_info.ToString()} )");

                sb.Append($"\r\n |- initial_display_delay_present_flag = {v.initial_display_delay_present_flag}");
                sb.Append($"\r\n |- operating_points_cnt_minus_1 = {v.operating_points_cnt_minus_1}");
                for ( var i = 0; i <= v.operating_points_cnt_minus_1; i++ )
                {
                    sb.Append($"\r\n    - operating_point_idc = {v.operating_point_idc[i]}");
                    var level = v.seq_level_idx[i];
                    sb.Append($"\r\n    - seq_level_idx = {level}");
                    bool seq_tier = v.seq_tier[i];
                    if ( level < 7 )
                    {
                        seq_tier = false;
                    }
                    sb.Append($"\r\n    - seq_tier = {seq_tier}");
                    if ( v.decoder_model_info_present_flag )
                    {
                        sb.Append($"\r\n    - decoder_model_present_for_this_op = {v.decoder_model_present_for_this_op[i]}");
                        if ( v.decoder_model_present_for_this_op[i] )
                        {
                            sb.Append($"\r\n      - operating_parameters_info = ( {v.operating_parameters_info[i]} )");
                        }
                    }
                    if ( v.initial_display_delay_present_flag )
                    {
                        sb.Append($"\r\n    - initial_display_delay_present_for_this_op = {v.initial_display_delay_present_for_this_op[i]}");
                        if ( v.initial_display_delay_present_for_this_op[i] )
                        {
                            sb.Append($"\r\n      - initial_display_delay_minus_1 = {v.initial_display_delay_minus_1[i]}");
                        }
                    }
                }

                sb.Append($"\r\n |- frame_width_bits_minus_1 = {v.frame_width_bits_minus_1}");
                sb.Append($"\r\n |- frame_height_bits_minus_1 = {v.frame_height_bits_minus_1}");
                sb.Append($"\r\n |- max_frame_width_minus_1 = {v.max_frame_width_minus_1}");
                sb.Append($"\r\n |- max_frame_height_minus_1 = {v.max_frame_height_minus_1}");
                sb.Append($"\r\n |- frame_id_numbers_present_flag = {v.frame_id_numbers_present_flag}");
                if ( v.frame_id_numbers_present_flag )
                {
                    sb.Append($"\r\n    - delta_frame_id_length_minus_2 = {v.delta_frame_id_length_minus_2}");
                    sb.Append($"\r\n    - additional_frame_id_length_minus_1 = {v.additional_frame_id_length_minus_1}");
                }
                sb.Append($"\r\n |- use_128x128_superblock = {v.use_128x128_superblock}");
                sb.Append($"\r\n |- enable_filter_intra = {v.enable_filter_intra}");
                sb.Append($"\r\n |- enable_intra_edge_filter = {v.enable_intra_edge_filter}");
                if ( v.reduced_still_picture_header )
                {
                    sb.Append($"\r\n |- enable_interintra_compound = {v.enable_interintra_compound}");
                    sb.Append($"\r\n |- enable_masked_compound = {v.enable_masked_compound}");
                    sb.Append($"\r\n |- enable_warped_motion = {v.enable_warped_motion}");
                    sb.Append($"\r\n |- enable_dual_filter = {v.enable_dual_filter}");
                    sb.Append($"\r\n |- enable_order_hint = {v.enable_order_hint}");
                    sb.Append($"\r\n |- enable_jnt_comp = {v.enable_jnt_comp}");
                    sb.Append($"\r\n |- enable_ref_frame_mvs = {v.enable_ref_frame_mvs}");
                    sb.Append($"\r\n |- seq_force_screen_content_tools = {v.seq_force_screen_content_tools}");
                    sb.Append($"\r\n |- seq_force_integer_mv = {v.seq_force_integer_mv}");
                    sb.Append($"\r\n |- OrderHintBits = {v.OrderHintBits}");
                }
                else
                {
                    sb.Append($"\r\n |- enable_interintra_compound = {v.enable_interintra_compound}");
                    sb.Append($"\r\n |- enable_masked_compound = {v.enable_masked_compound}");
                    sb.Append($"\r\n |- enable_warped_motion = {v.enable_warped_motion}");
                    sb.Append($"\r\n |- enable_dual_filter = {v.enable_dual_filter}");
                    sb.Append($"\r\n |- enable_order_hint = {v.enable_order_hint}");
                    sb.Append($"\r\n   - enable_jnt_comp = {v.enable_jnt_comp}");
                    sb.Append($"\r\n   - enable_ref_frame_mvs = {v.enable_ref_frame_mvs}");
                    sb.Append($"\r\n |- seq_choose_screen_content_tools = {v.seq_choose_screen_content_tools}");

                    if ( v.seq_choose_screen_content_tools > 0 )
                    {
                        sb.Append($"\r\n   - seq_force_screen_content_tools = SELECT_SCREEN_CONTENT_TOOLS");
                    }
                    else if ( v.seq_force_screen_content_tools > 0 )
                    {
                        sb.Append($"\r\n   - seq_force_screen_content_tools = {v.seq_force_screen_content_tools}");
                        sb.Append($"\r\n      - seq_choose_integer_mv = {v.seq_choose_integer_mv}");
                        if ( v.seq_choose_integer_mv > 0 )
                        {
                            sb.Append($"\r\n        - seq_force_integer_mv = SELECT_INTEGER_MV");
                        }
                        else
                        {
                            sb.Append($"\r\n        - seq_force_integer_mv = {v.seq_force_integer_mv}");
                        }
                    }

                    if ( v.enable_order_hint )
                    {
                        sb.Append($"\r\n   - order_hint_bits_minus_1 = {v.order_hint_bits_minus_1}");
                        sb.Append($"\r\n   - OrderHintBits = {v.OrderHintBits}");
                    }
                }
                sb.Append($"\r\n |- enable_superres = {v.enable_superres}");
                sb.Append($"\r\n |- enable_cdef = {v.enable_cdef}");
                sb.Append($"\r\n |- enable_restoration = {v.enable_restoration}");
                sb.Append($"\r\n |- color_config = {v.color_config.ToString()}");
                sb.Append($"\r\n |_ film_grain_params_present = {v.film_grain_params_present}");
            }

            return sb.ToString();
        }
    }
}