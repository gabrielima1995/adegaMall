using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2.Application.Modules.IntegrationParameterFromGoogle
{

    /// <summary>
    /// A data class for storing the contents of merchant-info.json.
    /// </summary>
    public class MerchantConfig : BaseConfig
    {
        public override string ConfigDir { get; set; }
        internal override string ConfigFile { get; set; }

        [Newtonsoft.Json.JsonPropertyAttribute("merchantId")]
        public ulong? MerchantId { get; set; }

        [Newtonsoft.Json.JsonPropertyAttribute("accountSampleUser")]
        public string AccountSampleUser { get; set; }

        [Newtonsoft.Json.JsonPropertyAttribute("accountSampleGoogleAdsCID")]
        public ulong? AccountSampleGoogleAdsCID { get; set; }

        public bool IsMCA { get; set; }
        public string WebsiteURL { get; set; }
    }
}

