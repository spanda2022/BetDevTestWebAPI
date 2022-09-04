using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetDevTestAPI.Services;
using Microsoft.EntityFrameworkCore;
using BetDevTestAPI.Data;
using BetDevTestClassLibrary.Model;

namespace BetDevTestAPI.Services
{
    public class ProductRepository :IProductRepository
    {
        private ApplicationDBContext _db;
        public ProductRepository(ApplicationDBContext db)
        {
            _db = db;
        }
      
        public List<Product> ListProducts()
        {
            
            
            List<Product> lstProduct  = _db.Products.Where(c => c.IsActive ==true).Where(a=>a.Quantity > 0).ToList();

            return lstProduct;
        }

        public List<Product> AddProduct(Product p)
        {
            
            _db.Products.Add(p);
            _db.SaveChanges();
            List<Product> lstProduct = _db.Products.Where(c => c.IsActive == true).Where(a => a.Quantity > 0).ToList();

            return lstProduct;
        }

        
        public List<Product> DeleteProduct(int productId)
        {  
           Product p = _db.Products.Where(p => p.Id == productId).Where(p1 => p1.IsActive == true).FirstOrDefault();
                if (p != null)
                {   
                    _db.Products.Remove(p);
                    _db.SaveChanges();
                }
            List<Product> lstProduct = _db.Products.Where(c => c.IsActive == true).Where(a => a.Quantity > 0).ToList();
            return lstProduct;
        }

        public Product GetProductDetails(int prodId)
        {
            var product = _db.Products.Where(a=>a.IsActive == true).Where(x=>x.Quantity > 0).FirstOrDefault(c => c.Id == prodId);

            return product;
        }
    }
}