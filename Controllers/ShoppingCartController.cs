using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetDevTestAPI.Services;
using Microsoft.Extensions.Logging;
using BetDevTestAPI.Data;
using Microsoft.Extensions.Configuration;
using BetDevTestClassLibrary.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BetDevTestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private IUserOrderRepository _userOrderRepository;
        private IConfiguration _configuration { get; }

        private ApplicationDBContext _db;

        public ShoppingCartController(ILogger<ShoppingCartController> logger, IConfiguration configuration, IUserOrderRepository userOrderRepository, ApplicationDBContext db)
        {
            _logger = logger;
            _userOrderRepository = userOrderRepository;
            _db = db;
            _configuration = configuration;

        }
        [HttpGet("ShowCartProducts")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ShowCartProducts(int cartId,string cartNo="")
        {
            try
            {
              
                    List<CartProductsView> lstActiveCartProduct = _userOrderRepository.ShowCartProducts(cartId, cartNo);
                    if (lstActiveCartProduct != null)
                        return Ok(lstActiveCartProduct);
                    else
                        return BadRequest("CartProduct is empty");
                
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error ShowCartProducts");
            }
        }



        [HttpGet("ShowShoppingCart")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ShowShoppingCart(string UserEmail)
        {
            try
            {
                
                 Cart lsActiveOrders = _userOrderRepository.ShowShoppingCart(UserEmail);
                if (lsActiveOrders!=null)
                    return Ok(lsActiveOrders);
                else
                    return BadRequest("Shopping Cart is empty");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error ShowShoppingCart");
            }
        }

        [HttpPost("AddProductToCart")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult AddProductToCart(string UserEmail,int productId, int Qty)
        {
            try
            {
                Cart lsActiveOrders = _userOrderRepository.AddProductToCart(UserEmail, productId, Qty);
                if (lsActiveOrders!=null)
                    return Ok("Successfully Added product to cart.");
                
                else
                    return BadRequest("Active Cart is empty");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error AddProductToCart");
            }
        }


        [HttpPost("RemoveProductFromCart")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult RemoveProductFromCart(string UserEmail, int productId, int Qty)
        {
            try
            {
                CartProduct cartProduct = new CartProduct();
                Cart lsActiveOrders = _userOrderRepository.RemoveProductFromCart(UserEmail, productId, Qty ,out cartProduct);
                if (cartProduct != null)
                    return Ok("Successfully removed product to cart.");
                
                else
                    return BadRequest("Cart Product not found");

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error RemoveProductFromCart");
            }            
        }

        [HttpGet("ShowAllOrderHistory")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ShowAllOrderHistory(string UserEmail)
        {
            try
            {
                List<OrderHistory> lstOrderHistory = _userOrderRepository.ShowAllOrderHistory(UserEmail);
                if (lstOrderHistory.Count > 0)
                    return Ok(lstOrderHistory.ToList());
                else
                    return BadRequest("OrderHistory is empty");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error ShowAllOrderHistory");
            }
        }

        [HttpGet("ShowSingleOrderDetails")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ShowSingleOrderDetails(string UserEmail,int orderId)
        {
            try
            {
               var orderHistory = _userOrderRepository.ShowSingleOrderDetails(UserEmail,orderId);
                if (orderHistory !=null)
                    return Ok(orderHistory);
                else
                    return BadRequest("OrderHistory is empty");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error ShowSingleOrderDetails");
            }
        }


        [HttpPost("CheckOut")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult CheckOut(string UserEmail, int CartId)
        {
           
            string respMsg = "";
            bool mailSent = false;
            try
            {
                IDictionary<int, decimal> checkOutInstance = _userOrderRepository.CheckOut(UserEmail, CartId, out respMsg);//,out mailSent);

                if (respMsg.Length == 0)
                {
                    //mail config
                    string smtpHost = _configuration["smtpHost"];
                    string smtpFromAddress = _configuration["smtpFromAddress"];
                    string smtpPort = _configuration["smptPort"];
                    mailSent= _userOrderRepository.SendEmail(UserEmail, CartId,smtpFromAddress,smtpHost,smtpPort);
                    if(mailSent)
                        return Ok("CheckOut happened successfully. Please check the confirmation email on your mailbox");
                    else
                        return Ok("Checkout was successfull where as there is problem sending the confirmation mail.");
                }
                else
                    return Ok(respMsg);
            }
            catch
            {
                if(respMsg.Length==0 && mailSent==false)
                    return Ok("Checkout was successfull where as there is problem sending the confirmation mail.");
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error in Checkout process.");
            }
        }

       
        //Only for test purpose
        [HttpPost("SendMail")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult SendMail(string UserEmail)
        {
            try
            {

                //mail config
                string smtpHost = _configuration["smtpHost"];
                string smtpFromAddress = _configuration["smtpFromAddress"];
                string smtpPort = _configuration["smptPort"];
                _userOrderRepository.SendEmail(UserEmail, 1, smtpFromAddress, smtpHost, smtpPort);
                return Ok("mail sent");
            }
            catch
            {
                return Ok("Checkout was successfull where as there is problem sending the confirmation mail.");
            }
        }
    }
}
