using Application.Config;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace V2.Application.Modules.SiteMap
{
    public class SiteMapAppService
    {
        private readonly IProductRepository productRepository;
        private readonly ISellerRepository sellerRepository;
        private readonly Lazy<IEnumerable<Seller>> sellers;

        public SiteMapAppService(IProductRepository productRepository, ISellerRepository sellerRepository)
        {
            this.productRepository = productRepository;
            this.sellerRepository = sellerRepository;
            sellers = new Lazy<IEnumerable<Seller>>(ListSellers);
        }

        public string GenerateSiteMapXml()
        {
            return GetSitemapDocument(GetSitemapNodes());
        }

        private IEnumerable<SitemapNode> GetSitemapNodes()
        {
            var sitemapNodes = new List<SitemapNode>
            {
                GetHomeNode()
            };

            sitemapNodes.AddRange(GetSellerNodes());
            sitemapNodes.AddRange(GetProductNodes());
            sitemapNodes.AddRange(GetRegistrationNodes());
            
            return sitemapNodes;
        }

        private static SitemapNode GetHomeNode()
        {
            return new SitemapNode
            {
                Frequency = SitemapFrequency.Weekly,
                Priority = 1.0,
                Url = MaaSConfig.Current.Environment.AppUrl
            };
        }

        private IEnumerable<SitemapNode> GetSellerNodes()
        {
            return sellers.Value.Select(seller => new SitemapNode
            {
                Priority = 0.9,
                Frequency = SitemapFrequency.Monthly,
                Url = MaaSConfig.Current.Environment.AppUrl + "p/" + (seller.FriendlyUrl ?? seller.Nickname)
            });
        }

        private IEnumerable<SitemapNode> GetProductNodes()
        {
            if (!MaaSConfig.Current.Product.Enabled)
                return Array.Empty<SitemapNode>();

            var nodes = new List<SitemapNode>();
            var products = ListProducts();

            nodes.AddRange(products.Select(product => new SitemapNode
            {
                Priority = 1,
                Frequency = SitemapFrequency.Weekly,
                Url = MaaSConfig.Current.Environment.AppUrl + "product/" + (product.FriendlyUrl ?? product.Id.ToString())
            }));

            if (MaaSConfig.Current.Search.Enabled && MaaSConfig.Current.Search.SearchType == Domain.Config.General.SearchType.Product)
            {
                var categories = products.Select(x => x.Category_Id).Distinct().Where(c => c.HasValue).ToList();
                nodes.AddRange(categories.Select(cat => new SitemapNode
                {
                    Priority = 0.5,
                    Frequency = SitemapFrequency.Weekly,
                    Url = MaaSConfig.Current.Environment.AppUrl + "products/search/?Category=" + cat.Value
                }));
            }

            return nodes;
        }

        private IEnumerable<SitemapNode> GetRegistrationNodes()
        {
            return new List<SitemapNode>
            {
                new SitemapNode
                {
                    Priority = 0.8,
                    Frequency = SitemapFrequency.Monthly,
                    Url = MaaSConfig.Current.Environment.AppUrl + "register/seller/",
                },
                new SitemapNode
                {
                    Priority = 0.8,
                    Frequency = SitemapFrequency.Monthly,
                    Url = MaaSConfig.Current.Environment.AppUrl + "register/buyer/",
                }
            };
        }

        private string GetSitemapDocument(IEnumerable<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.LastModified == null ? null : new XElement(
                        xmlns + "lastmod",
                        sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null ? null : new XElement(
                        xmlns + "changefreq",
                        sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(
                        xmlns + "priority",
                        sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }

            var document = new XDocument(root);
            return document.ToString();
        }

        private ReadOnlyCollection<Seller> ListSellers()
        {
            return sellerRepository.List()
                .Where(s => s.Enabled)
                .Where(s => s.SellerCompleteProfileMapped?.IsComplete ?? false)
                .ToList().AsReadOnly();
        }

        private ReadOnlyCollection<Product> ListProducts()
        {
            var sellerIds = new HashSet<string>(sellers.Value.Select(x => x.Id));
            return productRepository.List()
                .Where(p => p.IsBillingProduct != true)
                .Where(p => !p.Deleted && p.IsDisabled != true)
                .Where(p => !p.IsOutOfStock && sellerIds.Contains(p.Store_SellerId))
                .ToList().AsReadOnly();
        }
    }

    public class SitemapNode
    {
        public SitemapFrequency? Frequency { get; set; }
        public DateTime? LastModified { get; set; }
        public double? Priority { get; set; }
        public string Url { get; set; }
    }

    public enum SitemapFrequency
    {
        Never, Yearly, Monthly,
        Weekly, Daily, Hourly,
        Always
    }
}
