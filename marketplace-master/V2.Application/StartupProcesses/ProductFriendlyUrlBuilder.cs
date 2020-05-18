using Domain.Interfaces.Repositories;
using System;
using System.IO;
using System.Linq;

namespace V2.Application.StartupProcesses
{
    public class FriendlyUrlBuilder
    {
        private const string FileName = "startup_friendlyurlbuilder_ok.txt";

        private readonly IUnitOfWork _unitOfWork;

        public FriendlyUrlBuilder(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Run()
        {
            if (!ShouldRun())
                return;

            BuildFriendlyUrlForAllProducts();
            BuildFriendlyUrlForAllSellers();
            BuildFriendlyUrlForAllPortfolioItems();
            RegisterOkFile();
        }

        private string BuildFriendlyUrl(string name)
        {
            return name
                .ReplaceDiacritics()
                .Hyphenize();
        }

        private void BuildFriendlyUrlForAllProducts()
        {
            var products = _unitOfWork.ProductRepository.List();

            foreach (var p in products)
            {
                Guid guidTest;
                if (p.FriendlyUrl != null && !Guid.TryParse(p.FriendlyUrl, out guidTest))
                    continue;

                var url = BuildFriendlyUrl(p.Title);

                if (products.Any(x => x.FriendlyUrl == url && x.Id != p.Id))
                {
                    var uniqueTitle = p.Id.ToString().Substring(0, 6) + " " + p.Title;
                    url = BuildFriendlyUrl(uniqueTitle);
                }

                p.FriendlyUrl = url;
                _unitOfWork.ProductRepository.Update(p);
            }

            _unitOfWork.Save();
        }

        private void BuildFriendlyUrlForAllSellers()
        {
            var sellers = _unitOfWork.SellerRepository.List();

            foreach (var s in sellers)
            {
                Guid guidTest;
                if (s.FriendlyUrl != null && !Guid.TryParse(s.FriendlyUrl, out guidTest))
                    continue;

                s.GenerateFriendlyUrl();

                if (sellers.Any(x => x.FriendlyUrl == s.FriendlyUrl && x.Id != s.Id))
                {
                    s.GenerateFriendlyUrl(s.Id.Substring(0, 6));
                }

                _unitOfWork.SellerRepository.Update(s);
            }

            _unitOfWork.Save();
        }

        private void BuildFriendlyUrlForAllPortfolioItems()
        {
            var items = _unitOfWork.PortfolioRepository.GetQueryable().ToList();

            foreach (var p in items)
            {
                Guid guidTest;
                if (p.FriendlyUrl != null && !Guid.TryParse(p.FriendlyUrl, out guidTest))
                    continue;

                p.GenerateFriendlyUrl();

                if (items.Any(x => x.FriendlyUrl == p.FriendlyUrl && x.Id != p.Id))
                {
                    p.GenerateFriendlyUrl(p.Id.ToString().Substring(0, 6));
                }

                _unitOfWork.PortfolioRepository.Update(p);
            }

            _unitOfWork.Save();
        }

        private string GetFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
        }

        private void RegisterOkFile()
        {
            using (var stream = File.OpenWrite(GetFilePath()))
            {
                using (var writer = new StreamWriter(stream))
                {
                    var now = DateTime.UtcNow;
                    var msg = $"ProductFriendlyUrlBuilder ran successfully at {now.ToShortDateString()} {now.ToLongTimeString()}";
                    writer.WriteLine(msg);
                }
            }
        }

        private bool ShouldRun()
        {
            return !File.Exists(GetFilePath());
        }
    }
}