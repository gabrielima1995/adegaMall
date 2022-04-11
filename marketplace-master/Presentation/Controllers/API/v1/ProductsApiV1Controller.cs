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

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Put([FromUri]Guid id, [FromBody] ProductEditing model)
        {
            model.Id = id;

            return this.Result(_productService.Update(model, _authorizedProvider.Auth.Seller_Id));
        }

        [HttpPut]
        [Route("ref:{ref}")]
        public IHttpActionResult PutByRef([FromUri]string @ref, [FromBody] ProductEditing model)
        {
            model.Id = Guid.Empty;
            model.CustomId = @ref;

            return this.Result(_productService.Update(model, _authorizedProvider.Auth.Seller_Id));
        }

        [HttpGet]
        [Route("{id:guid}", Name = "Product by id")]
        public IHttpActionResult Get([FromUri]Guid id)
        {
            var product = _productService.GetById(id);

            if (product == null) return NotFound();
            if (_authorizedProvider.Auth.HasSeller &&
                _authorizedProvider.Auth.Seller_Id != product.Store_SellerId) return BadRequest();

            return Ok(new ProductGetting(product));
        }

        [HttpGet]
        [Route("ref:{ref}", Name = "Product by ref")]
        public IHttpActionResult GetByRef([FromUri]string @ref)
        {
            var product = _productService.GetByRef(@ref);

            if (product == null) return NotFound();
            if (_authorizedProvider.Auth.HasSeller &&
                _authorizedProvider.Auth.Seller_Id != product.Store_SellerId) return BadRequest();

            return Ok(new ProductGetting(product));
        }
    }
}