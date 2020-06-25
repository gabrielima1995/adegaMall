using Microsoft.EntityFrameworkCore;
using NewContext.Core;
using NewContext.Domain.Languages;
using NewContext.Infrastructure.Data.Mappers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NewContext.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IMessagesDispatcher messages;

        public AppDbContext(DbContextOptions options, IMessagesDispatcher messages) : base(options)
        {
            this.messages = messages;
        }

        public DbSet<PlatformLanguage> PlatformLanguage { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var mapperInterface = typeof(IMapper);

            var mappers = mapperInterface.Assembly.GetTypes()
                .Where(t => t.IsClass && mapperInterface.IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t) as IMapper);

            foreach (var mapper in mappers)
                mapper.Map(modelBuilder);
        }

        public override int SaveChanges()
        {
            var result = base.SaveChanges();
            messages.DispatchDomainEvents(this);
            return result;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            messages.DispatchDomainEvents(this);
            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await messages.DispatchDomainEventsAsync(this);
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            await messages.DispatchDomainEventsAsync(this);
            return result;
        }
    }
}
