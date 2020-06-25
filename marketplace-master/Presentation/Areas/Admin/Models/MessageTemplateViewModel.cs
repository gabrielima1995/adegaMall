using Application.Config;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Messages;

namespace Presentation.Areas.Admin.Models
{
    public class MessageTemplateViewModel
    {
        private readonly IReadOnlyDictionary<string, MessageSpecification> _specifications;
        private readonly IReadOnlyDictionary<string, MessageTemplate> _templates;

        public IEnumerable<MessageSpecification> Specifications => _specifications.Select(i => i.Value);

        public IList<string> Langs { get; }

        public MessageTemplateViewModel(
            MessagesContext context,
            IList<string> langs)
        {
            _specifications = GetSpecificationsForActiveModules(context);
            _templates = context.Templates;
            Langs = langs;
        }

        private IReadOnlyDictionary<string, MessageSpecification> GetSpecificationsForActiveModules(MessagesContext context)
        {
            var config = MaaSConfig.Current;
            var ret = new Dictionary<string, MessageSpecification>(context.Specifications.Count);

            foreach (var item in context.Specifications)
            {
                switch (item.Value.Module)
                {
                    case "Quote":
                        if (config.Quote.Enabled)
                            ret.Add(item.Key, item.Value);
                        break;

                    case "Store":
                        if (config.Product.Enabled)
                            ret.Add(item.Key, item.Value);
                        break;

                    case "Subscription":
                        if (config.Subscription.Enabled)
                            ret.Add(item.Key, item.Value);
                        break;

                    case "Account":
                    default:
                        ret.Add(item.Key, item.Value);
                        break;
                }
            }

            return ret;
        }
    }
}