using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetDevTestClassLibrary.Model;

namespace BetDevTestAPI.Services
{
   public interface IProductRepository
    {
        public List<Product> ListProducts();
        public Product GetProductDetails(int prodId);
        public List<Product> AddProduct(Product p);
        public List<Product> DeleteProduct(int productId);
    }
}
