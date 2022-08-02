using Application.Auth;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using Framework;
using Infra.Crosscutting.WebHelpers.WebApi;
using System;
using System.Net;
using System.Web.Http;

namespace Presentation.Controllers.API.v1
{
    //[AuthorizeApiToken]
    [AllowAnonymous]
    [RoutePrefix("api/v1/tokens")]
    public class ApiTokensApiV1Controller : ApiController
    {
        private readonly ApiTokenService _apiTokenService;
        private readonly IApiTokenRepository _apiTokenRepository;
        private readonly ISellerRepository _sellerRepository;

        public ApiTokensApiV1Controller( ApiTokenService apiTokenService, IApiTokenRepository apiTokenRepository, ISellerRepository sellerRepository)
        {
            _apiTokenService = apiTokenService;
            _apiTokenRepository = apiTokenRepository;
            _sellerRepository = sellerRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{id:Guid}", Name = "Token list")]
        public IHttpActionResult List([FromUri]string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("O id do seller deve ser informado na url.");
            }

            var tokenList = _apiTokenRepository.ListByActive(id);

            return Ok(tokenList);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("", Name = "Token Create")]
        public IHttpActionResult Post([FromBody] ApiTokenRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.seller_id_or_email))
            {
                return BadRequest("O seller não foi informado no corpo da requisição.");
            }

            var seller = _sellerRepository.GetById(model.seller_id_or_email);
            if (seller == null)
            {
                return BadRequest("O seller não foi encontrado.");
            }

            var name = model.name ?? seller.Name;

            var apiToken = _apiTokenService.GenerateForApi(model.name, seller.Id);
            if (apiToken == null)
            {
                return InternalServerError();
            }

            return Ok( new ApiTokenResponse(apiToken.Token) );
        }

        [AllowAnonymous]
        [HttpDelete]
        [Route("token:{token}")]
        public IHttpActionResult Delete([FromUri] string @token)
        {
            var apiToken = _apiTokenRepository.GetByToken(token);

            return this.Result(_apiTokenService.Revoke(apiToken.Id));
        }

        [HttpGet]
        [Route("token:{token}")]
        public IHttpActionResult Exists([FromUri] string @token)
        {
            var apiToken = _apiTokenRepository.GetByToken(token);
            if (apiToken == null)
            {
                return InternalServerError();
            }
            else
            {
                return Ok();
            }
        }

    }

}