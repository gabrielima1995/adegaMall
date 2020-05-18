using Framework;
using Infra.Crosscutting.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services
{
    public class GlobalizationService : ServiceBase<Str>
    {
        private readonly IRepository<Str> _repository;

        public GlobalizationService(IRepository<Str> repository)
            : base(repository)
        {
            _repository = repository;
        }

        private void AddLang(IList<Str> strs, Lang lang, string label, string value, bool forceUpdate)
        {
            var str = strs.FirstOrDefault(i => i.Label == label && i.Lang == lang);

            if (str == null)
            {
                str = new Str
                {
                    Label = label,
                    Lang = lang,
                    Value = value
                };

                strs.Add(str);

                _repository.Insert(str);
            }
            else
            {
                if (forceUpdate)
                {
                    str.Value = value;

                    _repository.Update(str);
                }
            }
        }

        public IList<Str> SaveAllLangs(string label, string value, bool forceUpdate = false)
        {
            var strs = _repository
                .Query()
                .Where(i => i.Label == label)
                .ToList();

            AddLang(strs, Lang.en_US, label, value, forceUpdate);
            AddLang(strs, Lang.pt_BR, label, value, forceUpdate);

            _repository.SaveChanges();

            return strs;
        }
    }
}
