using Framework;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace Infra.Crosscutting.WebHelpers.WebApi
{
    public static class ApiControllerHelper
    {
        /// <summary>
        /// Cria um response com base nas informações de um operation result. Se for válido, retorna um status code OK, caso contrário, um Bad Request com a lista de erros
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ResponseMessageResult Result<T>(this ApiController controller, OperationResult<T> result, HttpStatusCode successStatusCode = HttpStatusCode.OK)
        {
            HttpResponseMessage response;

            if (result.OK)
            {
                response = controller.Request.CreateResponse(successStatusCode, result.Result);
            }
            else
            {
                response = controller.Request.CreateResponse(HttpStatusCode.BadRequest, new { errors = result.Errors, message = "Validation fail." });
            }

            return new ResponseMessageResult(response);
        }

        /// <summary>
        /// Cria um response com base nas informações de um operation result. Se for válido, retorna um status code NoContent, caso contrário, um Bad Request com a lista de erros
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ResponseMessageResult Result(this ApiController controller, OperationResult result, HttpStatusCode successStatusCode = HttpStatusCode.NoContent)
        {
            HttpResponseMessage response;

            if (result.OK)
            {
                response = controller.Request.CreateResponse(successStatusCode);
            }
            else
            {
                response = controller.Request.CreateResponse(HttpStatusCode.BadRequest, new { errors = result.Errors, message = "Validation fail." });
            }

            return new ResponseMessageResult(response);
        }
    }
}