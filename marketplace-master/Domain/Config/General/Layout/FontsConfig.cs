using System.Collections.Generic;

namespace Application.Config.General.Layout
{
    public class FontsConfig : ExtendedDictionary<string, string>
    {
        public FontsConfig()
        {
            Extend(new Dictionary<string, string>()
            {
                { "$font_title", "'Open Sans', 'Arial', sans-serif" },
                { "$font_subtitle", "'Open Sans', 'Arial', sans-serif" },
                { "$font_text", "'Open Sans', 'Arial', sans-serif" },
                { "$font_btn", "'Open Sans', 'Arial', sans-serif" },

                /* hero unit */
                { "$home_hero_title_font", "$font_title" },
                { "$home_hero_slogan_font", "$font_title" },

                /* how it works */
                { "$howitworks_title_font", "$font_title" },
                { "$howitworks_subtitle_font", "$font_subtitle" },
                { "$howitworks_text_font", "$font_text" },

                /* best sellers */
                { "$bestsellers_title_font", "$font_title" },

                /* call to sellers */
                { "$callsellers_title_font", "$font_title" },

                /* nearby products */
                { "$nearbyproducts_title_font", "$font_title" },

                /* misc */
                { "$misc_title_font", "$font_title" },
            });
        }
    }
}