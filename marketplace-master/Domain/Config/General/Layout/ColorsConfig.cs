using System.Collections.Generic;

namespace Application.Config.General.Layout
{
    public class ColorsConfig : ExtendedDictionary<string, string>
    {
        public ColorsConfig()
        {
            Extend(new Dictionary<string, string>()
            {
                /* Colors */
                { "$primary_color", "#3498DB" },
                { "$secondary_color", "#2C3E50" },

                { "$black_color", "#2980B9" },
                { "$gray_color", "#E74C3C" },
                { "$white_color", "#FFFFFF" },

                { "$default_color", "#363636" },
                { "$success_color", "green" },
                { "$info_color", "lightblue" },
                { "$warning_color", "lightyellow" },
                { "$danger_color", "rgb(181, 16, 16)" },

                /* Backgrounds */
                { "$bg_1", "#F5F5F5" },
                { "$bg_2", "lightgray" },
                { "$bg_3", "#F0F0F0" },
                { "$bg_image_1_color", "white" },
                { "$bg_image_2_color", "white" },
                { "$bg_image_3_color", "white" },
                { "$bg_card", "white" },

                /* Box */
                { "$box_background", "white" },

                { "$box_portfolio_background", "#bfbfbf" },
                { "$box_portfolio_color", "#363636" },

                /* Menu mobile */
                { "$menu_mobile_bg", "#363636" },
                { "$menu_mobile_color", "#cccccc" },

                /* paleta de cores */
                // https://color.adobe.com/pt/Flat-UI-color-theme-2469224
                { "$color_bg1", "white" }, // bg principal
                { "$color_bg2", "#EEEEEE" }, // bg secundário
                { "$color_accent1", "#3498DB" }, // botões principais
                { "$color_accent2", "#2C3E50" }, // botões secundários, rodapé
                { "$color_contrast", "#E74C3C" }, // contraste, call to action

                /* home do ecommerce */
                { "$ecommerce_home_search_bg", "$color_bg2" },

                /* cores de texto (tendem a não mudar) */
                { "$color_title", "darken($color_accent1, 40%)" },
                { "$color_subtitle", "darken($color_accent1, 30%)" },
                { "$color_text", "#363636" },
                { "$color_anchor", "$color_accent1" },

                /* cores das sobras do título principal e slogan (ambos na home) */
                { "$title_shadow_color", "#333" },
                { "$slogan_shadow_color", "#333" },

                /* botões */
                { "$btn_primary_bg", "#3498DB" },
                { "$btn_primary_color", "#FFF" },
                { "$btn_secondary_bg", "#2980B9" },
                { "$btn_secondary_color", "#FFF" },
                { "$btn_highlight_bg", "#E74C3C" },
                { "$btn_highlight_color", "#FFF" },
                { "$btn_danger_bg", "#E74C3C" },
                { "$btn_danger_color", "#FFF" },

                /* menu fixo */
                { "$menu_fixed_bg", "$color_bg1" },
                { "$menu_fixed_color", "$color_text" },
                { "$menu_fixed_hover_color", "$color_accent1" },
                { "$menu_fixed_btn_bg", "$color_accent1" },
                { "$menu_fixed_btn_color", "$color_bg1" },

                /* menu variação inicial da home (antes da rolagem) */
                { "$menu_home_bg", "transparent" },
                { "$menu_home_color", "$color_bg1" },
                { "$menu_home_btn_bg", "$color_accent1" },
                { "$menu_home_btn_color", "$color_bg1" },

                /* hero unit */
                { "$home_hero_bg", "$bg_image_1" },
                { "$home_hero_title_color", "white" },
                { "$home_hero_slogan_color", "white" },
                { "$home_hero_main_btn_bg", "$color_accent1" },
                { "$home_hero_main_btn_color", "$color_bg1" },
                { "$home_hero_second_btn_bg", "$color_accent2" },
                { "$home_hero_second_btn_color", "$color_bg1" },

                /* how it works */
                { "$howitworks_bg", "$color_bg2" },
                { "$howitworks_title_color", "$color_title" },
                { "$howitworks_subtitle_color", "$color_subtitle" },
                { "$howitworks_text_color", "$color_text" },

                /* best sellers */
                { "$bestsellers_bg", "$color_bg1" },
                { "$bestsellers_title_color", "$color_title" },

                /* call to sellers */
                { "$callsellers_bg", "$color_accent2" },
                { "$callsellers_title_color", "$color_bg1" },
                { "$callsellers_btn_bg", "$color_contrast" },
                { "$callsellers_btn_color", "$color_bg1" },
                { "$callsellers_box_bg", "$color_bg1" },
                { "$callsellers_box_color", "$color_accent2" },

                /* nearby products */
                { "$nearbyproducts_bg", "$color_accent2" },
                { "$nearbyproducts_title_color", "$color_bg1" },
                { "$nearbyproducts_btn_bg", "$color_contrast" },
                { "$nearbyproducts_btn_color", "$color_bg1" },
                { "$nearbyproducts_box_bg", "$color_bg1" },
                { "$nearbyproducts_box_color", "$color_accent2" },

                /* misc */
                { "$misc_bg", "$color_bg1" },
                { "$misc_title_color", "$color_title" },

                /* footer */
                { "$footer_bg", "darken($color_accent2, 10%)" },
                { "$footer_color", "$color_bg2" },
                { "$footer_title_color", "$color_bg2" },
                { "$footer_anchor_color", "$color_bg2" },
                { "$line_footer_color", "$color_bg2" },
                { "$line_footer_width", "1px" },
            });
        }
    }
}