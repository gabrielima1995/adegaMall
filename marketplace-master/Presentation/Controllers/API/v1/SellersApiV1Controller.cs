using NewContext.Core;
using NewContext.Queries;
using NewContext.Queries.Sellers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Presentation.Controllers.API.v1
{
    [AuthorizeApiToken]
    [RoutePrefix("api/v1/sellers")]
    public class SellersApiV1Controller : ApiControllerBase
    {
        private readonly IMessagesDispatcher messages;

        public SellersApiV1Controller(IMessagesDispatcher messages)
        {
            this.messages = messages;
        }

        [Route("", Name = "sellers-list")]
        public async Task<IHttpActionResult> Get(int start = 0, int limit = 20, bool? enabled = null, bool? pending_moderation = null, string document = null, string gateway_id = null)
        {
            var result = await messages.QueryAsync(new ListSellersForApi(new Paging(start, limit), enabled, pending_moderation, document, gateway_id));

            var urlHelper = PagedUrl(start, limit, result.TotalCount);
            var routeValues = new { enabled, pending_moderation };

            var model = new PagedResult(result.Sellers, result.TotalCount,
                urlHelper.LinkToPreviousPage("sellers-list", routeValues),
                urlHelper.LinkToNextPage("sellers-list", routeValues));

            return Ok(model);
        }
    }
}