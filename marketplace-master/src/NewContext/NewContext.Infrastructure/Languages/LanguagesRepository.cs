using Microsoft.EntityFrameworkCore;
using NewContext.Application.Languages;
using NewContext.Domain.Languages;
using NewContext.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace NewContext.Infrastructure.Languages
{
    public class LanguagesRepository : ILanguagesRepository
    {
        private readonly AppDbContext dbContext;

        public LanguagesRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddSingleAsync(PlatformLanguage platformLanguage)
        {
            if (await ExistsAsync())
                throw new InvalidOperationException("The application can have only one PlatformLanguage instance in the database.");

            await dbContext.AddAsync(platformLanguage);
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync()
        {
            return await dbContext.PlatformLanguage.AnyAsync();
        }

        public async Task<PlatformLanguage> GetAsync()
        {
            return await dbContext.PlatformLanguage.SingleOrDefaultAsync();
        }

        public async Task UpdateAsync(PlatformLanguage platformLanguage)
        {
            dbContext.Attach(platformLanguage).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }
    }
}
