using SmartSearch.LuceneNet;
using SmartSearch.LuceneNet.Analysis;

namespace Infra.SmartSearchAdapter
{
    internal class DefaultIndexOptions : LuceneIndexOptions
    {
        public DefaultIndexOptions()
        {
            UseAnalyzerFactory(new BrazilianAnalyzerFactory());
        }
    }
}