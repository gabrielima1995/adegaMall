using Application.Config;
using Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domain.Config.ConfigMigrations
{
    public class ConfigMigrator
    {
        private readonly IRepository<ApplicationConfig> _appConfigRepository;

        public ConfigMigrator(IRepository<ApplicationConfig> appConfigRepository)
        {
            _appConfigRepository = appConfigRepository;
        }

        public OperationResult EnsureLatestVersion()
        {
            if (!NeedsMigration())
                return OperationResult.Success;

            var result = Migrate();
            return result;
        }

        private MaaSConfig GetConfigClone()
        {
            var settings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            var json = MaaSConfig.Current.ToJson();
            var clone = JsonConvert.DeserializeObject<MaaSConfig>(json, settings);

            return clone;
        }

        private List<IConfigMigration> ListMigrations()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var nspace = "Domain.Config.ConfigMigrations";
            var interfaceType = typeof(IConfigMigration);

            var classes = assembly.GetTypes()
                .Where(t => String.Equals(t.Namespace, nspace, StringComparison.Ordinal))
                .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();

            var instances = classes
                .Select(x => Activator.CreateInstance(x) as IConfigMigration)
                .OrderBy(x => x.NewVersion)
                .ToList();

            return instances;
        }

        private OperationResult Migrate()
        {
            var config = GetConfigClone();

            var migrationsResult = RunMigrations(config);
            if (!migrationsResult.OK) return migrationsResult;

            var updateResult = UpdateDatabase(config);
            if (!updateResult.OK) return updateResult;

            var reloadResult = ReloadConfig(updateResult.Result);
            if (!reloadResult.OK) return reloadResult;

            return OperationResult.Success;
        }

        private bool NeedsMigration()
        {
            var latestMigration = ListMigrations().OrderByDescending(x => x.NewVersion).First();
            return MaaSConfig.Current.Version != latestMigration.NewVersion;
        }

        private OperationResult ReloadConfig(ApplicationConfig applicationConfig)
        {
            try
            {
                MaaSConfig.Load(applicationConfig);
                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return new ErrorDictionary($"{nameof(ConfigMigrator)}.{nameof(UpdateDatabase)}", ex.Message);
            }
        }

        private OperationResult RunMigrations(MaaSConfig config)
        {
            var migrationList = ListMigrations()
                .Where(x => x.ExpectedVersion >= config.Version)
                .OrderBy(x => x.ExpectedVersion)
                .ToList();

            foreach (var m in migrationList)
            {
                if (config.Version != m.ExpectedVersion)
                    return new ErrorDictionary(m.GetType().Name, $"Expected version is {m.ExpectedVersion}, found {config.Version}.");

                if (!m.CanMigrate())
                    return new ErrorDictionary(m.GetType().Name, "CanMigrate returned false.");

                var validationResult = new MaaSConfigValidator().Validate(config);
                if (!validationResult.IsValid)
                {
                    var errors = new ErrorDictionary($"{nameof(ConfigMigrator)}.{nameof(RunMigrations)}", "Validation failed before migrations could run.");
                    errors.Merge(new ErrorDictionary(validationResult));
                    return errors;
                }

                try
                {
                    m.Migrate(config);
                }
                catch (Exception ex)
                {
                    return new ErrorDictionary(m.GetType().Name, ex.Message);
                }

                validationResult = new MaaSConfigValidator().Validate(config);
                if (!validationResult.IsValid)
                {
                    var errors = new ErrorDictionary($"{nameof(ConfigMigrator)}.{nameof(RunMigrations)}", "Validation failed after migrations ran.");
                    errors.Merge(new ErrorDictionary(validationResult));
                    return errors;
                }

                config.Version = m.NewVersion;
            }

            return OperationResult.Success;
        }

        private OperationResult<ApplicationConfig> UpdateDatabase(MaaSConfig config)
        {
            try
            {
                var appConfig = _appConfigRepository.Query().FirstOrDefault() ?? new ApplicationConfig();
                var serializedConfig = config.ToJson();

                appConfig.MaaSConfig = serializedConfig;

                if (appConfig.Id == 0) _appConfigRepository.Insert(appConfig);
                else _appConfigRepository.Update(appConfig);

                _appConfigRepository.SaveChanges();

                return appConfig;
            }
            catch (Exception ex)
            {
                return new ErrorDictionary($"{nameof(ConfigMigrator)}.{nameof(UpdateDatabase)}", ex.Message);
            }
        }
    }
}
