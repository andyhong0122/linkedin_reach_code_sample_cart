using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models.Domain;
using Sabio.Models.Requests.ShoppingCart;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartApiController : BaseApiController
    {
        private ICartService _service = null;
        private IAuthenticationService<int> _authService = null;

        public CartApiController(
            IAuthenticationService<int> authService
            , ICartService service
            , ILogger<CartApiController> logger) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(CartAddRequest model)
        {

            int iCode = 201;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();

                int id = _service.AddToCart(model.ProviderServiceId, userId);
                response = new ItemResponse<int> { Item = id };
            }
            catch (Exception ex)
            {
                iCode = 500;
                Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(iCode, response);
        }


        [HttpGet]
        public ActionResult<ItemsResponse<List<ShoppingCart>>> Get()
        {
            int userId = _authService.GetCurrentUserId();

            int iCode = 200;
            BaseResponse response = null;

            try
            {
                List<ShoppingCart> aCart = _service.GetByCreatedBy(userId);

                if (aCart == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Cart is Empty");
                }
                else
                {
                    response = new ItemResponse<List<ShoppingCart>> { Item = aCart }; // 
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"ArgumentException Error:${ex.Message}");
            }
            return StatusCode(iCode, response);
        }


        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> DeleteItem(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                response = new SuccessResponse();
                _service.DeleteItem(id, _authService.GetCurrentUserId());
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }


        [HttpDelete]
        public ActionResult<SuccessResponse> DeleteCart()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                response = new SuccessResponse();
                _service.DeleteCart(_authService.GetCurrentUserId());
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

    }
}