using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;
using System.Collections.Generic;

namespace Infrastructure.Data.Repositories
{
    public class MessageTemplateRepository : EfCrudRepository<MessageTemplate>, IMessageTemplateRepository
    {
        private readonly ScaffoldContext _context;

        public MessageTemplateRepository(ScaffoldContext context) : base(context)
        {
            _context = context;
        }

        public MessageTemplate Get(string key, string lang)
        {
            return Get(i => i.Key == key && i.Lang == lang);
        }

        public List<MessageTemplate> List()
        {
            return base.List();
        }
    }
}