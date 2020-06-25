using Domain.Interfaces.Repositories;
using Domain.UserConfig;
using Framework;
using Infrastructure.Data.Context;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class UserConfigRepository : IUserConfigRepository
    {
        private readonly ScaffoldContext _dbContext;

        public UserConfigRepository(ScaffoldContext dbContext)
        {
            _dbContext = dbContext;
        }

        public UserConfiguration ReadConfig()
        {
            var dbConfig = _dbContext.Set<UserConfigurationRaw>().FirstOrDefault();
            if (dbConfig == null)
                return null;

            var result = JsonConvert.DeserializeObject<UserConfiguration>(dbConfig.RawConfig);
            return result;
        }

        public OperationResult SaveConfig(UserConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException();

            var dbSet = _dbContext.Set<UserConfigurationRaw>();

            var dbConfig = dbSet.FirstOrDefault();

            if (dbConfig == null)
            {
                dbConfig = new UserConfigurationRaw() { Id = 1 };
                dbSet.Add(dbConfig);

                _dbContext.Entry(dbConfig).State = System.Data.Entity.EntityState.Added;
                _dbContext.SaveChanges();
            }


            dbConfig.RawConfig = config.ToJson();
            _dbContext.Entry(dbConfig).State = System.Data.Entity.EntityState.Modified;
            var result = _dbContext.SaveChanges();

            return result >= 0 ? OperationResult.Success : OperationResult.Fail("Error setting User Config");
        }
    }
}
