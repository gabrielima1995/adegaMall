using Application.Config;
using Dapper;
using Domain.Data.SqlScripts;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Entities.StoreModule;
using Domain.Entities.SubscriptionModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.StoreModule.CompleteProfile;
using Infrastructure.Caching;
using Infrastructure.Data.Context;
using PaymentIntegrator.Core.Subscriptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class SellerRepository : EfCrudRepository<Seller>, ISellerRepository
    {
        public SellerRepository(ScaffoldContext context) : base(context)
        {
        }

        public Seller GetByNickname(string nickname)
        {
            var cache = new CacheDefinition($"__SellerRepository_GetByNickname_{nickname}", new TimeSpan(0, 10, 0), CacheTimeoutType.Sliding);
            return CacheHelper.Get(Context.Set<Seller>().Where(x => x.Nickname == nickname), cache);
        }

        public Seller GetDetailsForAdmin(string sellerId)
        {
            Seller query()
            {
                using (var conn = MyDbConnection.Create())
                {
                    var sql = SqlScriptHelper.GetScript(ScriptName.SellerRepository_GetDetailsForAdmin);
                    var grid = conn.QueryMultiple(sql, new { sellerId });
                    var seller = grid.Read<Seller>().SingleOrDefault();

                    if (seller != null)
                    {
                        seller.Address = grid.Read<Address>().SingleOrDefault();
                        seller.BankAccount = grid.Read<BankAccount>().SingleOrDefault();
                        seller.Quotes = grid.Read<Quote, Category, SubCategory, Quote>((q, c, s) => { q.Seller = seller; q.Category = c; q.SubCategory = s; return q; }).ToList();
                        seller.Subscription = grid.Read<Subscription>().SingleOrDefault();

                        // Se o vendedor não completou o cadastro, pode não ter um registro de loja.
                        seller.Store = grid.Read<Store>().SingleOrDefault();
                        var products = grid.Read<Product, ProductCategory, Product>((p, c) => { p.Category = c; return p; }).ToList();

                        if (seller.Store != null)
                            seller.Store.Products = products;

                        seller.Orders = grid.Read<Order, Buyer, Address, Order>((o, b, a) => { o.Buyer = b; o.ShippingAddress = a; return o; }).ToList();
                        var orderProducts = grid.Read<OrderProduct>();

                        foreach (var order in seller.Orders)
                            order.Products = orderProducts.Where(x => x.Order_Id == order.Id).ToList();
                    }

                    return seller;
                }
            }

            var result = CacheHelper.Get(query);
            return result;
        }

        public Seller GetPublicProfileInfos(string sellerId, bool requireActiveSubscription)
        {
            var cache = new CacheDefinition($"__SellerRepository_GetPublicProfileInfos_{sellerId}", new TimeSpan(0, 5, 0));

            var result = CacheHelper.Get(Context.Set<Seller>()
                .Include(x => x.Address)
                .Include(x => x.Portfolio.SubCategories)
                .Include(x => x.Portfolio.SubCategories.Select(s => s.Category))
                .Include(x => x.Store)
                .Where(x => x.Id == sellerId && (requireActiveSubscription == false || (x.Subscription != null && (
                    x.Subscription.Status == SubscriptionStatus.Trialing ||
                    x.Subscription.Status == SubscriptionStatus.Paid ||
                    x.Subscription.Status == SubscriptionStatus.PendingPayment)))),
                cache);

            if (result != null)
                if (result.Portfolio != null)
                    result.Portfolio.SubCategories = result.Portfolio.SubCategories.Where(x => !x.Deleted && !x.Category.Deleted).ToList();

            return result;
        }

        public bool FriendlyUrlExists(string friendlyUrl)
        {
            return Context.Set<Seller>().Any(x => x.FriendlyUrl == friendlyUrl);
        }

        public Seller GetByFriendlyUrl(string friendlyUrl)
        {
            return CacheHelper
                .Get(Context.Set<Seller>()
                .Where(x => x.FriendlyUrl == friendlyUrl));
        }

        public Seller GetWithSubscription(string sellerId)
        {
            return CacheHelper.Get(Context.Set<Seller>()
                .Include(x => x.Subscription)
                .Where(x => x.Id == sellerId));
        }

        public List<SellerSelectorItem> ListForSelector()
        {
            return Context.Set<Seller>()
                .OrderBy(x => x.FullName)
                .Select(x => new SellerSelectorItem
                {
                    Id = x.Id,
                    Name = x.FullName
                })
                .ToList();
        }

        public List<Seller> ListForAdmin()
        {
            var requireModeration = MaaSConfig.Current.Seller.Moderation.Enabled;

            return CacheHelper.List(Context.Set<Seller>()
                .Include(x => x.Address)
                .Include(x => x.Orders)
                .Include(x => x.Orders.Select(o => o.Products))
                .Include(x => x.Quotes)
                .Include(x => x.Subscription)
                .Include(x => x.Portfolio)
                .Include(x => x.Portfolio.SubCategories)
                .Include(x => x.Portfolio.Items)
                .Include(x => x.Portfolio.Items.Select(item => item.SubCategories))
                .OrderBy(x => x.AlternativeName)
                .ThenBy(x => x.FullName));
        }

        public Seller GetWithLeadSalesProfile(string sellerId)
        {
            return CacheHelper.Get(Context.Set<Seller>()
                .Include(x => x.LeadSalesProfile)
                .Include(x => x.LeadSalesProfile.QuoteRequestsUnlocked)
                .Include(x => x.LeadSalesProfile.CreditsBundles)
                .Where(x => x.Id == sellerId));
        }

        public Seller GetWithCompleteLeadSalesProfile(string sellerId)
        {
            return CacheHelper.Get(Context.Set<Seller>()
                .Include(x => x.LeadSalesProfile)
                .Include(x => x.LeadSalesProfile.QuoteRequestsUnlocked)
                .Include(x => x.LeadSalesProfile.CreditsBundles)
                .Include(x => x.LeadSalesProfile.CreditsPackTransactions)
                .Include(x => x.LeadSalesProfile.CreditsPackTransactions.Select(y => y.CreditsBundle))
                .Where(x => x.Id == sellerId));
        }

        public bool ExistsByAlternativeNameEncoded(string alternativeNameEncoded, string ignoreSellerId)
        {
            var set = Context.Set<Seller>();

            return set.Any(i => i.AlternativeName == alternativeNameEncoded && i.Id != ignoreSellerId);
        }

        public bool ExistsByFriendlyUrl(string friendlyUrl, string ignoreSellerId)
        {
            var set = Context.Set<Seller>();

            return set.Any(i => i.FriendlyUrl == friendlyUrl && i.Id != ignoreSellerId);
        }

        public bool ExistsByNickname(string nickname, string ignoreSellerId)
        {
            var set = Context.Set<Seller>();
            return set.Any(i => i.Nickname == nickname && i.Id != ignoreSellerId);
        }

        public ReadOnlyCollection<string> ListSellerIdsPendingCheckForProfileComplete()
        {
            return Context.Set<Seller>()
                .Where(i => i.SellerCompleteProfileVersion != SellerCompleteProfile.CurrentVersion)
                .Select(seller => seller.Id)
                .ToList().AsReadOnly();
        }

        public ReadOnlyCollection<string> ListSellerIds()
        {
            return Context.Set<Seller>()
                .Select(seller => seller.Id)
                .ToList().AsReadOnly();
        }
    }
}