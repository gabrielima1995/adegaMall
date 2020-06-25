using System.Web.Http;

namespace Presentation.Controllers.API.v1
{
    [AllowAnonymous]
    [RoutePrefix("api/v1/ping")]
    public class PingApiV1Controller : ApiController
    {
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok("pong");
        }
    }

    [AllowAnonymous]
    [RoutePrefix("api/v1/ex")]
    public class ExApiV1Controller : ApiController
    {
        [Route("")]
        public IHttpActionResult Get()
        {
            var a = 1;
            var b = 0;

            return Ok(a / b);
        }
    }
}