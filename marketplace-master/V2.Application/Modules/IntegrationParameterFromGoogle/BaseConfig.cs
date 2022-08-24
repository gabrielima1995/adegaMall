using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2.Application.Modules.IntegrationParameterFromGoogle
{
    /// <summary>
    /// A data class for storing the info needed to authenticate API calls.
    /// </summary>
    public abstract class BaseConfig
    {
        public abstract string ConfigDir { get; set; }
        internal abstract string ConfigFile { get; set; }
    }
}
