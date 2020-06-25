using System.Collections.Generic;

namespace Application.Config.General.Layout
{
    public class StylesConfig : ExtendedDictionary<string, string>
    {
        public StylesConfig()
        {
            Extend(new Dictionary<string, string>
            {
                /* Border radius */
                { "$radius", "4px" },

                /* Spacing */
                { "$default_margin", "15px" },
                { "$default_padding", "15px" },

                /* Menu mobile */
                { "$menu_mobile_width", "80%" },
            });
        }
    }
}