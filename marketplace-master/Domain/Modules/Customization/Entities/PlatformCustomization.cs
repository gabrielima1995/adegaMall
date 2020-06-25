using Domain.Entities;
using Domain.Events.Customization;
using System;

namespace Domain.Modules.Customization.Entities
{
    public class PlatformCustomization : BaseEntity<Guid>
    {
        public PlatformCustomization()
        {
            Id = Guid.Empty;
        }

        public string HtmlHead { get; private set; }

        public string HtmlBodyStart { get; private set; }

        public string HtmlBodyEnd { get; private set; }

        public static PlatformCustomization Create()
        {
            return new PlatformCustomization();
        }

        public void SetHtmlHead(string htmlHead)
        {
            HtmlHead = htmlHead;
            AddDomainEvent(new PlatformCustomizationUpdatedEvent());
        }

        public void SetHtmlBodyStart(string htmlBodyStart)
        {
            HtmlBodyStart = htmlBodyStart;
            AddDomainEvent(new PlatformCustomizationUpdatedEvent());
        }

        public void SetHtmlBodyEnd(string htmlBodyEnd)
        {
            HtmlBodyEnd = htmlBodyEnd;
            AddDomainEvent(new PlatformCustomizationUpdatedEvent());
        }
    }
}
