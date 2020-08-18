using System;

namespace Framework.Settings
{
    public class EndpointSettings : ResilienceStrategySettings
    {
        private string _key;

        public string Key
        {
            get => _key = (!string.IsNullOrEmpty(_key) ? _key : Guid.NewGuid().ToString());
            set => _key = value;
        }

        public string Url { get; set; }
    }
}