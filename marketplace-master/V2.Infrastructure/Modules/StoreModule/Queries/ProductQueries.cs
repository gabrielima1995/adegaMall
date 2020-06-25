using Application.Modules.Products.Queries;
using Dapper;
using Domain.Events;
using Domain.Events.StoreModule;
using System;
using System.Collections.Generic;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.StoreModule.Queries
{
    public class ProductQueries :
        IProductQueries,
        IDomainEventHandler<ProductUpdatedEvent>
    {
        private static readonly Dictionary<string, Guid> _urlsToIdsTable = new Dictionary<string, Guid>();
        private static readonly object _lock = new object();

        public Guid? GetIdByFriendlyUrl(string friendlyUrl)
        {
            if (!_urlsToIdsTable.ContainsKey(friendlyUrl))
            {
                lock (_lock)
                {
                    using (var conn = MyDbConnection.Create())
                    {
                        var sql = "SELECT TOP 1 Id FROM Product WHERE FriendlyUrl = @friendlyUrl";
                        var id = conn.ExecuteScalar<Guid?>(sql, new { friendlyUrl });

                        if (id != null && !_urlsToIdsTable.ContainsKey(friendlyUrl))
                            _urlsToIdsTable.Add(friendlyUrl, id.Value);
                    }
                }
            }

            if (_urlsToIdsTable.ContainsKey(friendlyUrl))
                return _urlsToIdsTable[friendlyUrl];

            return null;
        }

        public IEnumerable<ProductSelectItem> ListForSelector(string sellerId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"SELECT P.Id, P.Title
                FROM Product P
                WHERE P.Deleted = 0 AND P.Store_SellerId = @sellerId
                ORDER BY P.Title";

                return conn.Query<ProductSelectItem>(sql, new { sellerId });
            }
        }

        public void Handle(ProductUpdatedEvent domainEvent)
        {
            foreach (var item in _urlsToIdsTable)
            {
                if (item.Value == domainEvent.Product.Id)
                {
                    _urlsToIdsTable.Remove(item.Key);
                    return;
                }
            }
        }
    }
}