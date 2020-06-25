using CSharpFunctionalExtensions;
using Dapper;
using NewContext.Core;
using NewContext.Domain.Languages;
using NewContext.Infrastructure.Util;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace NewContext.Infrastructure.Languages
{
    public static class LanguagesHelper
    {
        internal static IUnityContainer Container { get; set; }

        public static IReadOnlyList<string> ActiveLanguages
            => Container.Resolve<LanguagesHelperInternal>().ActiveLanguages;

        public static string DefaultLanguage
            => Container.Resolve<LanguagesHelperInternal>().DefaultLanguage;

        public static bool IsLanguageActive(Maybe<string> language)
            => Container.Resolve<LanguagesHelperInternal>().IsLanguageActive(language);
    }

    internal class LanguagesHelperInternal :
        IDomainEventHandler<LanguageActivated>,
        IDomainEventHandler<LanguageDeactivated>,
        IDomainEventHandler<LanguageSetAsDefault>
    {
        private const string CacheKey_ActiveLanguages = "LanguagesHelper_ActiveLanguages";
        private const string CacheKey_DefaultLanguage = "LanguagesHelper_DefaultLanguage";

        private readonly ICachingProvider cachingProvider;
        private readonly CommandsConnectionString connectionString;

        public LanguagesHelperInternal(ICachingProvider cachingProvider, CommandsConnectionString connectionString)
        {
            this.cachingProvider = cachingProvider;
            this.connectionString = connectionString;
        }

        internal IReadOnlyList<string> ActiveLanguages => cachingProvider.Get(CacheKey_ActiveLanguages, 10, () => GetActiveLanguages());

        internal string DefaultLanguage => cachingProvider.Get(CacheKey_DefaultLanguage, 10, () => GetDefaultLanguage());

        internal bool IsLanguageActive(Maybe<string> language)
        {
            if (language.HasNoValue)
                return false;

            var parsed = Language.Create(language.Value);

            if (parsed.IsFailure)
                return false;

            return ActiveLanguages.Contains(parsed.Value);
        }

        private string GetDefaultLanguage()
        {
            using (var conn = new SqlConnection(connectionString.Value))
            {
                var sql = "SELECT DefaultLanguage FROM nc.PlatformLanguage";
                var str = conn.QuerySingle<string>(sql);
                return str;
            }
        }

        private IReadOnlyList<string> GetActiveLanguages()
        {
            using (var conn = new SqlConnection(connectionString.Value))
            {
                var sql = "SELECT ActiveLanguages FROM nc.PlatformLanguage";
                var str = conn.QuerySingle<string>(sql);
                return str.Split(',').ToList().AsReadOnly();
            }
        }

        public Task HandleAsync(LanguageActivated domainEvent)
        {
            cachingProvider.Remove(CacheKey_ActiveLanguages);
            return Task.CompletedTask;
        }

        public Task HandleAsync(LanguageDeactivated domainEvent)
        {
            cachingProvider.Remove(CacheKey_ActiveLanguages);
            return Task.CompletedTask;
        }

        public Task HandleAsync(LanguageSetAsDefault domainEvent)
        {
            cachingProvider.Remove(CacheKey_DefaultLanguage);
            return Task.CompletedTask;
        }
    }
}
