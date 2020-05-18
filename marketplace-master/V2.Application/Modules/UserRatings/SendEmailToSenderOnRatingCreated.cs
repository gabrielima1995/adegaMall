using Application.Config;
using Domain.Events;
using V2.Application.Messages;
using V2.Application.Modules.UserRatings.Messages;
using V2.Application.Modules.UserRatings.Messages.Specs;

namespace V2.Application.Modules.UserRatings
{
    public class SendEmailToSenderOnRatingCreated : IDomainEventHandler<PendingRatingCreatedEvent>
    {
        public void Handle(PendingRatingCreatedEvent domainEvent)
        {
            var target = domainEvent.Target;
            var sender = domainEvent.Sender;

            if (domainEvent.TargetRole == PendingRatingCreatedEvent.RatingTargetRole.BUYER)
            {
                MessagesContext.Current.GenerateAndSend<RatingRequestedToSellerMessageSpecification, RatingRequestMessageModel>(new RatingRequestMessageModel()
                {
                    RatedName = target.Name,
                    RaterName = sender.Name,
                    RatingId = domainEvent.RatingId.ToString(),
                }, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress, new[] { sender.Email });
            }
            else
            {
                MessagesContext.Current.GenerateAndSend<RatingRequestedToBuyerMessageSpecification, RatingRequestMessageModel>(new RatingRequestMessageModel()
                {
                    RatedName = target.Name,
                    RaterName = sender.Name,
                    RatingId = domainEvent.RatingId.ToString(),
                }, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress, new[] { sender.Email });
            }
        }
    }
}