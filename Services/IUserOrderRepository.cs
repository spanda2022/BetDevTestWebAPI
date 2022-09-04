using BetDevTestClassLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BetDevTestAPI.Services
{
   public interface IUserOrderRepository
    {
        public List<OrderHistory> ShowAllOrderHistory(string UserEmail);

        public OrderHistory ShowSingleOrderDetails(string UserEmail, int orderId);

        public Cart ShowShoppingCart(string UserEmail);
        public Cart AddProductToCart(string UserEmail, int productid, int cartid);
        public Cart RemoveProductFromCart(string UserEmail, int productId, int Qty, out CartProduct cartProduct);

        public IDictionary<int, decimal> CheckOut(string UserEmail, int CartId, out string respMsg);
                                                                                                    
        public bool SendEmail(string UserEmail, int cartID, string fromAddress, string smtpHost, string smtpPort);
        public List<CartProductsView> ShowCartProducts(int cartId, string cartNo = "");



    }
}
