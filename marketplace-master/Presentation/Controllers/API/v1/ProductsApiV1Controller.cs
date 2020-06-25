using Application.Auth;
using Domain.DTO;
using Domain.Services.StoreModule;
using Infra.Crosscutting.WebHelpers.WebApi;
using System;
using System.Web.Http;

namespace Presentation.Controllers.API.v1
{
    [AuthorizeApiToken]
    [RoutePrefix("api/v1/products")]
    public class ProductsApiV1Controller : ApiController
    {
        private readonly ProductService _productService;
        private readonly ApiTokenAuthorizedProvider _authorizedProvider;

        public ProductsApiV1Controller(
            ProductService productService,
            ApiTokenAuthorizedProvider authorizedProvider)
        {
            _productService = productService;
            _authorizedProvider = authorizedProvider;
        }

        [Route("{id:guid}")]
        public IHttpActionResult Put([FromUri]Guid id, [FromBody] ProductEditing model)
        {
            model.Id = id;

            return this.Result(_productService.Update(model, _authorizedProvider.Auth.Seller_Id));
        }

        [Route("ref:{ref}")]
        public IHttpActionResult PutByRef([FromUri]string @ref, [FromBody] ProductEditing model)
        {
            model.Id = Guid.Empty;
            model.CustomId = @ref;

            return this.Result(_productService.Update(model, _authorizedProvider.Auth.Seller_Id));
        }
    }
}