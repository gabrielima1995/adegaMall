using Newtonsoft.Json;

namespace V2.Application.Modules.StoreModule.Queries
{
    public class RentalScheduleTimelineResourceItem
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("title")]
        public string Title { get; private set; }

        protected RentalScheduleTimelineResourceItem()
        {
        }

        public static RentalScheduleTimelineResourceItem New(RentalScheduleTimelineProductDto product)
        {
            return new RentalScheduleTimelineResourceItem
            {
                Id = product.Id.ToString(),
                Title = product.Title
            };
        }

        public static RentalScheduleTimelineResourceItem New(string id, string title)
        {
            return new RentalScheduleTimelineResourceItem
            {
                Id = id,
                Title = title
            };
        }

        public static RentalScheduleTimelineResourceItem Store(string title)
        {
            return new RentalScheduleTimelineResourceItem
            {
                Id = "store",
                Title = title
            };
        }
    }
}