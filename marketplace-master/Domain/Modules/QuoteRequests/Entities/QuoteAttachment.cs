using Domain.Core;
using Domain.Entities;
using FluentValidation;
using System;

namespace Domain.QuoteRequests
{
    public class QuoteAttachment : BaseEntity<Guid>
    {
        public Guid QuoteId { get; set; }

        public Guid? QuoteAnswerId { get; set; }

        public string AttachmentUrl { get; set; }

        public QuoteAttachment() { }

        public QuoteAttachment(Guid id, Guid quoteId, Guid? quoteAnswerId, string attachmentUrl)
        {
            Id = id;
            QuoteId = quoteId;
            QuoteAnswerId = quoteAnswerId;
            AttachmentUrl = attachmentUrl;

            CheckInvariants();
        }

        public void CheckInvariants()
        {
            var result = new QuoteAttachmentValidator().Validate(this);
            if (!result.IsValid) throw new InvalidEntityException(GetType(), result);
        }
    }

    internal class QuoteAttachmentValidator : AbstractValidator<QuoteAttachment>
    {
        public QuoteAttachmentValidator()
        {
            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty);

            RuleFor(x => x.QuoteId)
                .NotEqual(Guid.Empty);

            RuleFor(x => x.QuoteAnswerId)
                .NotEqual(Guid.Empty);

            RuleFor(x => x.AttachmentUrl)
                .Must(BeValidUrl);
        }

        private bool BeValidUrl(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }
    }
}
