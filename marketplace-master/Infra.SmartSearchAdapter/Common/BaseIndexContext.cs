using SmartSearch.LuceneNet;
using System;
using System.IO;

namespace Infra.SmartSearchAdapter.Common
{
    public class BaseIndexContext : PhysicalIndexContext
    {
        public string IndexPath { get; protected set; }

        public BaseIndexContext(string contextName)
            : base(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SearchIndexes", contextName), false) { }
    }
}