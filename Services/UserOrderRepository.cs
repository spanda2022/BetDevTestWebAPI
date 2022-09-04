using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetDevTestAPI.Services;
using Microsoft.EntityFrameworkCore;
using BetDevTestAPI.Data;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using BetDevTestClassLibrary.Model;

namespace BetDevTestAPI.Services
{
    public class UserOrderRepository :IUserOrderRepository
    {

        private ApplicationDBContext _db;
      
        public UserOrderRepository(ApplicationDBContext db )
        {
            _db = db;
        }     
         public List<OrderHistory> ShowAllOrderHistory(string UserEmail)
        {
            List<OrderHistory> lstOrderHistory = _db.OrderHistory.Where(c => c.UserEmail == UserEmail).ToList();
            if (lstOrderHistory == null)
            {
                lstOrderHistory = new List<OrderHistory>();
            }
            return lstOrderHistory;            
        }

        public OrderHistory ShowSingleOrderDetails(string UserEmail, int orderId)
        {
            OrderHistory orderHistory = new OrderHistory();
            orderHistory = _db.OrderHistory.Where(c => c.UserEmail == UserEmail).FirstOrDefault(o=>o.Id==orderId);

            

            return orderHistory;
        }

        public Cart ShowShoppingCart(string UserEmail)
        {          
            Cart activeCart = _db.Cart.Where(c => c.UserEmail == UserEmail).Where(a => a.IsActive == true).FirstOrDefault();
            return activeCart;
        }

        public List<CartProductsView> ShowCartProducts( int cartId, string cartNo = "")
        {
           
                List<CartProductsView> cartProductsViews = new List<CartProductsView>();              
                List<CartProduct> cartProducts = _db.CartProducts.Where(cp => cp.CartId == cartId).ToList();
                foreach (CartProduct cp in cartProducts)
                {
                    cp.Product = _db.Products.Where(p => p.Id == cp.ProductId).FirstOrDefault();

                    CartProductsView cpv = new CartProductsView();
                    cpv.Id = cp.Id;
                    cpv.CartId = cp.CartId;
                    cpv.CartNo = cartNo;
                    cpv.ProductId = cp.Product.Id;
                    cpv.ProductName = cp.Product.Name;
                    cpv.Qty = cp.Qty;                                   
                    cpv.ProductImage = cp.Product.Image;
                    cpv.ProductPrice = cp.Product.Price;
                    cartProductsViews.Add(cpv);
                }
           
            return cartProductsViews;
        }

        public Cart AddProductToCart(string UserEmail, int productId, int Qty)
        {
            var product = _db.Products.Where(x => x.Quantity > 0).Where(b => b.IsActive == true).FirstOrDefault(c => c.Id == productId);
            Cart activeCart = _db.Cart.Where(c => c.UserEmail == UserEmail).Where(x => x.IsActive == true).FirstOrDefault();
            if(activeCart ==null)
            {
                    Cart cart = new Cart();
                    cart.CardNo = "Cart-" + DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Day.ToString() + DateTime.Today.Hour.ToString();
                    cart.UserEmail = UserEmail;
                    cart.IsActive = true;
                    cart.TotalCartPrice = 0;
                    _db.Cart.Add(cart);
                    _db.SaveChanges();
                    activeCart = _db.Cart.Where(c => c.UserEmail == UserEmail).Where(x => x.IsActive == true).FirstOrDefault();
                
            }
            if (product != null)
            {
                if (activeCart != null)
                {
                    CartProduct cartProducts = _db.CartProducts.Where(c => c.CartId == activeCart.Id).Where(b => b.ProductId == productId).FirstOrDefault();
                    if (cartProducts != null)
                    {

                        cartProducts.Qty += Qty;
                        product.Quantity -= Qty;
                        _db.SaveChanges();
                    }
                    else
                    {
                        CartProduct cartProduct = new CartProduct();
                        cartProduct.CartId = activeCart.Id;
                        cartProduct.ProductId = product.Id;
                        cartProduct.Qty = Qty;
                        activeCart.CartProducts.Add(cartProduct);

                        product.Quantity -= Qty;
                        if (product.Quantity == 0) product.IsActive = false;
                        _db.SaveChanges();
                    }
                }
                
            }            
            return activeCart;
        }
        public Cart RemoveProductFromCart(string UserEmail, int productId, int Qty,out CartProduct cartProduct)
        {
                Cart activeCart = _db.Cart.Where(c => c.UserEmail == UserEmail).Where(x => x.IsActive == true).FirstOrDefault();                            
                var product = _db.Products.Where(x => x.Quantity > 0).Where(b => b.IsActive == true).FirstOrDefault(c => c.Id == productId);
                cartProduct = _db.CartProducts.Where(c => c.CartId == activeCart.Id).Where(b => b.ProductId == productId).FirstOrDefault();
                if (activeCart != null)
                {
                    if (cartProduct != null)
                    {
                        if (cartProduct.Qty == Qty)
                        {
                            activeCart.CartProducts.Remove(cartProduct);
                            product.Quantity += Qty;
                            _db.SaveChanges();
                        }
                        else if (cartProduct.Qty > Qty)
                        {
                            cartProduct.Qty -= Qty;
                            product.Quantity += Qty;
                            _db.SaveChanges();
                        }
                    }
                }
                return activeCart;            
        }
      

        private decimal ValidatIfProductQtyIsAvailableInProductPoolAndCalculateCheckOutAmunt(int CartId, bool hasQtyAvailableForAllProductsAddedInCart)
        {
            
            hasQtyAvailableForAllProductsAddedInCart = true;
            List<CartProduct> cartProducts = _db.CartProducts.Where(c => c.CartId == CartId).ToList();

            List<Product> lstProduct = _db.Products.Where(c => c.IsActive == true).Where(a => a.Quantity > 0).ToList();
            //validate the qty
            foreach(CartProduct  cp in cartProducts)
            {
                Product p = lstProduct.Where(p => p.Id == cp.ProductId).FirstOrDefault();
                if (p.Quantity <= cp.Qty)
                {
                    cp.Qty = p.Quantity;

                    p.Quantity -= cp.Qty;
                    _db.SaveChanges();

                    hasQtyAvailableForAllProductsAddedInCart = false;
                }                
            }

            //calculate totamount
            var TotAmount = (from c in cartProducts
                             join p in lstProduct
                             on c.ProductId equals p.Id
                             select new { TotAmount = c.Qty * p.Price }).Sum(e => e.TotAmount);

            return (Decimal)TotAmount;
        }
        public IDictionary<int, decimal> CheckOut(string UserEmail, int CartId,out string respMsg)//,out bool mailSent)
        {
            IDictionary<int, decimal> checkoutResponse = new Dictionary<int, decimal>();
            respMsg = "";
            //mailSent = false;
            try
            {
                decimal TotCheckOutAmount = 0;               
                Cart activeCart = _db.Cart.Where(c => c.UserEmail == UserEmail).Where(a => a.IsActive == true).FirstOrDefault(b => b.Id == CartId);
                //validate if the products are available in the product backlog or not
                //calculate checkout amount
                bool hasQtyAvailableForAllProductsAddedInCart = true;

                TotCheckOutAmount = ValidatIfProductQtyIsAvailableInProductPoolAndCalculateCheckOutAmunt(CartId, hasQtyAvailableForAllProductsAddedInCart);
                if (hasQtyAvailableForAllProductsAddedInCart == true)
                {
                    //checkout
                    if (TotCheckOutAmount > 0)
                    {
                        //save OrderHistory
                        OrderHistory orderHistory = new OrderHistory();
                        orderHistory.OrderNo = "Ord_" + activeCart.CardNo + "_" + DateTime.Now.ToString();
                        orderHistory.OrderDate = DateTime.Now;
                        orderHistory.UserEmail = UserEmail;
                        orderHistory.CartId = activeCart.Id;
                        orderHistory.TotalPrice = TotCheckOutAmount;
                        _db.OrderHistory.Add(orderHistory);
                   //     _db.SaveChanges();

                        //make the cart deactive
                        activeCart.IsActive = false;
                        _db.SaveChanges();

                        checkoutResponse.Add(CartId, TotCheckOutAmount);
                    }
                    else                    
                        respMsg = "The Total Amount for the order cannot be 0.";                    
                }
                else                
                    respMsg = "Please Note that available quantity for some of the products added to the cart is less. Please check and confirm.";               
             
            }
            catch
            {
                respMsg = "Please Note that the checkout process was not successfull.";               
            }
            return checkoutResponse;
        }


        
        public bool SendEmail(string UserEmail, int cartId, string fromAddress, string smtpHost, string smtpPort)
        {
            
            bool mailSent = false;
            //send the mail only if the checkout is successfull
            Cart cart = _db.Cart.Where(c => c.Id == cartId).Where(c1 => c1.IsActive == false).FirstOrDefault();
            OrderHistory orderHistory = _db.OrderHistory.Where(o => o.CartId == cartId).FirstOrDefault();
            if (orderHistory != null && cart !=null)
            {
                using (MailMessage mm = new MailMessage(fromAddress, UserEmail))
                {
                    mm.Subject = "Order Confirmation For Order No:" + orderHistory.OrderNo;
                    mm.Body = "Please note that the Order with Order No: " + orderHistory.OrderNo + " has been successfully completed with total transaction amount of (ZAR): " + orderHistory.TotalPrice.ToString("0.00");

                    mm.IsBodyHtml = false;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = smtpHost;
                        smtp.EnableSsl = false;
                        smtp.UseDefaultCredentials = false;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Port = Int32.Parse( smtpPort);
                        smtp.Send(mm);
                        mailSent = true;
                    }
                    mm.Dispose();
                }
            }
                return mailSent;

        }

    }
}
