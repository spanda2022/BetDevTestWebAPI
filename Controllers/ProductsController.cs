using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BetDevTestClassLibrary.Model;
using BetDevTestAPI.Services;
using BetDevTestAPI.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BetDevTestAPI.Controllers
{
  //  [Authorize]    
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {       
        private readonly ILogger<ProductsController> _logger;
        private IProductRepository _ProductRepository;
        
        public ProductsController(ILogger<ProductsController> logger,IProductRepository productRepository)
        {
            _logger = logger;
            _ProductRepository = productRepository;         
          
        }

        [HttpGet("ListProducts")]   
        //[Authorize(AuthenticationSchemes  JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ListProducts()
        {
            //if (!User.Identity.IsAuthenticated)
            //    return StatusCode(StatusCodes.Status401Unauthorized, "Error ListProducts");
            try
            {
                
                
                List<Product> lstProduct  = _ProductRepository.ListProducts();
                if (lstProduct.Count > 0)
                    return Ok(lstProduct.ToList());
                else
                    return BadRequest("No Product in the Pool");
            }
            catch 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error ListProducts");
            }
        }

        [HttpGet("GetProductDetails")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetProductDetails(int productId)
        {
            try
            {
                Product product = _ProductRepository.GetProductDetails(productId);
                if (product != null)
                    return Ok(product);
                else
                    return BadRequest("Product details in not available");

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error GetProductDetails");
            }
        }

        [HttpPost("AddProduct")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult AddProduct(string pName,int pQty,decimal price,string imagePath)
        {
            try
            {                
                //create a bew product object
                Product pNew = new Product();
                pNew.Name = pName;                
                pNew.Quantity = pQty;
                pNew.Price = price;
                pNew.Image = imagePath;
                pNew.IsActive = true;
                List<Product> lstProduct = _ProductRepository.AddProduct(pNew);
                if (lstProduct.Count > 0)
                    return Ok(lstProduct.ToList());
                else
                    return BadRequest("No Product in the Pool");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding product");
            }
        }



        [HttpPost("DeleteProduct")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult DeleteProduct(int productId)
        {
            try
            {
                List<Product> lstProduct = _ProductRepository.DeleteProduct(productId);
                if (lstProduct.Count > 0)
                    return Ok(lstProduct.ToList());
                else
                    return BadRequest("No Product in the Pool to be deleted");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error =deleting product");
            }
        }



    }
}
