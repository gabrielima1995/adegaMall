using Application.Config;
using Domain.Config.General;

namespace Domain.Config.ConfigMigrations
{
    internal class Migration_1_to_2 : ConfigMigrationBase
    {
        public override int ExpectedVersion => 1;

        public override int NewVersion => 2;

        public override void Migrate(MaaSConfig config)
        {
            MigrateSearch(config);
        }

        private void MigrateSearch(MaaSConfig config)
        {
            if (config.Portfolio.Enabled)
            {
                config.Search = new SearchConfig
                {
                    Enabled = true,
                    List = new SearchListConfig
                    {
                        Enabled = true
                    },
                    Map = new SearchMapConfig
                    {
                        Enabled = true,
                        SearchRange = 5000
                    },
                    RequireLogin = config.Portfolio.Search.RequireLogin,
                    ResultType = ResultType.Seller,
                    SearchFields = new SearchFieldsConfig
                    {
                        Category = true,
                        Location = true,
                        Query = true
                    },
                    SearchType = SearchType.Seller
                };
            }

            else if (config.Product.Enabled)
            {
                config.Search = new SearchConfig
                {
                    Enabled = true,
                    List = new SearchListConfig
                    {
                        Enabled = true
                    },
                    Map = new SearchMapConfig
                    {
                        Enabled = false,
                        SearchRange = 5000
                    },
                    RequireLogin = config.Product.Search.RequireLogin,
                    ResultType = ResultType.Product,
                    SearchFields = new SearchFieldsConfig
                    {
                        Category = true,
                        Location = false,
                        Query = true
                    },
                    SearchType = SearchType.Product
                };
            }
        }
    }
}
