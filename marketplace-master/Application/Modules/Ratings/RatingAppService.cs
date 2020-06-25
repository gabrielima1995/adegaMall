using Application.Config;
using Application.Modules.Ratings.Models;
using Application.Sellers;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Ratings;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.Ratings
{
    public class UserRatingAppService : IUserRatingAppService
    {
        private const string cacheKeyPrefix = "RATING_APP_CACHE_";

        private readonly IUserRatingsRepository ratingsRepository;
        private readonly ISellerRepository sellerRepository;
        private readonly IBuyerRepository buyerRepository;
        private readonly IQuoteRepository quoteRepository;
        private readonly IOrderRepository orderRepository;
        private readonly ICache cache;

        public UserRatingAppService(
            IUserRatingsRepository ratingsRepository,
            ISellerRepository sellerRepository,
            IBuyerRepository buyerRepository,
            IQuoteRepository quoteRepository,
            IOrderRepository orderRepository,
            ICache cache)
        {
            this.ratingsRepository = ratingsRepository;
            this.sellerRepository = sellerRepository;
            this.buyerRepository = buyerRepository;
            this.quoteRepository = quoteRepository;
            this.orderRepository = orderRepository;
            this.cache = cache;
        }

        public OperationResult CompleteRating(string ratingId, string comment, int rating)
        {
            var userRating = ratingsRepository.GetById(ratingId);

            if (userRating.IsCompleted)
                return OperationResult.Fail("Rating already completed");

            userRating.IsCompleted = true;

            if (userRating.PairedRating.HasValue)
            {
                var pairedRating = ratingsRepository.GetById(userRating.PairedRating.Value.ToString());
                pairedRating.IsPairCompleted = true;
                ratingsRepository.Update(pairedRating);
            }

            userRating.Comment = comment;
            userRating.Rating = Math.Min(Math.Max(rating, 1), 5);

            string cacheKey = cacheKeyPrefix + "AVG_" + userRating.RatingTargetId;

            if (cache.Contains(cacheKey))
                cache.Remove(cacheKey);

            return ratingsRepository.Update(userRating);
        }

        public double GetAverageRating(string userId, out int ratingsCount)
        {
            var cacheKey = cacheKeyPrefix + "AVG_" + userId;

            if (cache.Contains(cacheKey))
            {
                var tuple = (Tuple<double, int>)cache.Get(cacheKey);
                ratingsCount = tuple.Item2;
                return tuple.Item1;
            }
            else
            {
                var list = ratingsRepository.ListRatingsReceivedByUser(userId);
                ratingsCount = list.Count;

                double average = 0;

                if (ratingsCount > 0)
                    average = list.Average(x => x.Rating.Value);

                cache.Add(cacheKey, new Tuple<double, int>(average, ratingsCount));
                return average;
            }
        }

        public List<UserRating> GetLastRatings(int amount, string userId)
        {
            var list = ratingsRepository.ListRecentByUser(userId, 0, amount);
            return list;
        }

        public UserRatingsOverviewModel GetSellerRatingsInfo(string sellerId, int page)
        {
            var person = sellerRepository.GetById(sellerId);
            var resultsPerPage = 10;

            if (person == null)
                return null;

            var ratings = ratingsRepository.ListRecentByUser(sellerId, page * resultsPerPage, resultsPerPage);
            var average = GetAverageRating(sellerId, out int count);
            var list = new List<UserRatingViewModel>();

            foreach (var rating in ratings)
            {
                var buyer = buyerRepository.GetById(rating.RatingSenderId);
                if (buyer != null)
                {
                    var tempModel = new UserRatingViewModel
                    {
                        CreatedAt = rating.CreatedAt,
                        RatingMessage = rating.Comment,
                        RatingValue = rating.Rating.GetValueOrDefault(),
                        SenderAvatar = buyer.Avatar ?? MaaSConfig.Current.Layout.Images.DefaultAvatar,
                        SenderName = buyer.Name,
                    };
                    tempModel.SenderAvatar = SellerHelper.GetAvatarUrl(tempModel.SenderAvatar, 80, 80);
                    list.Add(tempModel);
                }
            }

            var model = new UserRatingsOverviewModel
            {
                SellerAvatarUrl = person.Avatar ?? MaaSConfig.Current.Layout.Images.DefaultAvatar,
                SellerName = person.AlternativeName ?? person.FullName,
                SellerAverageRating = average,
                LatestRatingsList = list,
                TotalRatingsCount = count,
            };
            model.SellerAvatarUrl = SellerHelper.GetAvatarUrl(model.SellerAvatarUrl, 300, 300);
            return model;
        }

        public OperationResult HideRating(string ratingId)
        {
            var rating = ratingsRepository.GetById(ratingId);
            rating.Hidden = true;
            return ratingsRepository.Update(rating);
        }

        public OperationResult<Guid> NewPendingRating(string senderId, string targetId, string targetRole, string orderId = null, string quoteId = null)
        {
            Quote quote = null;
            Order order = null;
            Guid? orderGuid = null;
            Guid? quoteGuid = null;

            if (!string.IsNullOrWhiteSpace(quoteId))
            {
                quoteGuid = new Guid(quoteId);
                var existingRating = ratingsRepository.GetByQuoteId(quoteId, targetId);

                if (existingRating != null && existingRating.RatingSenderId == senderId)
                    return OperationResult<Guid>.Fail("Error", "Rating already created for this quote");

                quote = quoteRepository.GetById(quoteGuid.Value);
            }

            if (!string.IsNullOrWhiteSpace(orderId))
            {
                orderGuid = new Guid(orderId);
                var existingRating = ratingsRepository.GetByOrderId(orderId, targetId);

                if (existingRating != null)
                    return OperationResult<Guid>.Fail("Error", "Rating already created for this order");

                order = orderRepository.GetById(orderGuid.Value);
            }

            var rating = new UserRating
            {
                RatingSenderId = senderId,
                RatingTargetId = targetId,
                Hidden = false,
                OrderId = orderGuid,
                Order = order,
                QuoteId = quoteGuid,
                Quote = quote,
            };

            var domainEvent = new PendingRatingCreatedEvent
            {
                RatingId = rating.Id,
                Sender = targetRole == "Buyer" ? sellerRepository.GetById(senderId) : (Person)buyerRepository.GetById(senderId),
                Target = targetRole == "Seller" ? sellerRepository.GetById(targetId) : (Person)buyerRepository.GetById(targetId),
                TargetRole = targetRole == "Buyer" ? PendingRatingCreatedEvent.RatingTargetRole.BUYER : PendingRatingCreatedEvent.RatingTargetRole.SELLER,
            };

            rating.AddDomainEvent(domainEvent);
            var result = ratingsRepository.Add(rating);

            if (result.OK)
                return rating.Id;

            return OperationResult<Guid>.Fail("error", result.Errors.First().Value);
        }

        public OperationResult AssignRatingPairs(string ratingId1, string ratingId2)
        {
            var rating1 = ratingsRepository.GetById(ratingId1);
            var rating2 = ratingsRepository.GetById(ratingId2);

            if (rating1 == null || rating2 == null)
                return OperationResult.Fail("Unable to find one or more of the specified ratings");

            if (rating1.IsCompleted || rating1.PairedRating.HasValue || rating2.IsCompleted || rating2.PairedRating.HasValue)
                return OperationResult.Fail("Rating completed or already paired");

            rating1.PairedRating = rating2.Id;
            rating2.PairedRating = rating1.Id;

            ratingsRepository.Update(rating1);
            ratingsRepository.Update(rating2);

            return OperationResult.Success;
        }
    }
}