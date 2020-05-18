using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services
{
    public class StrService
    {
        private readonly IStringRepository _stringRepository;

        public StrService(IStringRepository stringRepository)
        {
            _stringRepository = stringRepository;
        }

        public Dictionary<string, Tuple<string, string>> GetDefaultStrings()
        {
            var strings = new Dictionary<string, Tuple<string, string>>();

            var stringArray = Encoding.UTF8.GetString(Resources.AppResources.strings_create_update).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (var str in stringArray)
            {
                var parts = str.Split('\t');
                if (!strings.ContainsKey(parts[0]))
                {
                    strings.Add(parts[0], new Tuple<string, string>(parts[1], parts[2]));
                }
            }

            return strings;
        }
    }
}