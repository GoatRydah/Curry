using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Curry.DataAccess.Data.Repository.IRepository;
using Curry.Models;
using Curry.Models.ViewModels;
using Curry.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;

namespace Curry.Pages.Customer.Cart
{
    public class SummaryModel : PageModel
    {
        public readonly IUnitOfWork _unitOfWork;

        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public OrderDetailsCart OrderDetailsCartVM { get; set; }

        public void OnGet()
        {
            OrderDetailsCartVM = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader(),
                ShoppingCart = new List<ShoppingCart>()
            };

            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                IEnumerable<ShoppingCart> Cart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value);

                if (Cart != null)
                {
                    OrderDetailsCartVM.ShoppingCart = Cart.ToList();
                }

                foreach (var item in OrderDetailsCartVM.ShoppingCart)
                {
                    item.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == item.MenuItemId);
                    OrderDetailsCartVM.OrderHeader.OrderTotal += (item.MenuItem.Price * item.Count);
                }

                //add sales tax to value
                OrderDetailsCartVM.OrderHeader.OrderTotal += OrderDetailsCartVM.OrderHeader.OrderTotal * SD.SalesTaxPercent;

                ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value);
                OrderDetailsCartVM.OrderHeader.DeliveryName = applicationUser.FullName;
                OrderDetailsCartVM.OrderHeader.DeliveryTime = DateTime.Now;
                OrderDetailsCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            }
        }

        public IActionResult OnPost(string stripeToken)
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var Claim = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsCartVM.ShoppingCart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == Claim.Value).ToList();

            OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.OrderDate = DateTime.Now;
            OrderDetailsCartVM.OrderHeader.UserId = Claim.Value;
            OrderDetailsCartVM.OrderHeader.Status = SD.StatusSubmitted;
            OrderDetailsCartVM.OrderHeader.DeliveryTime = Convert.ToDateTime(OrderDetailsCartVM.OrderHeader.DeliveryDate.ToShortDateString() + " " + OrderDetailsCartVM.OrderHeader.DeliveryTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _unitOfWork.OrderHeader.Add(OrderDetailsCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var item in OrderDetailsCartVM.ShoppingCart)
            {
                item.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = OrderDetailsCartVM.OrderHeader.Id,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                OrderDetailsCartVM.OrderHeader.OrderTotal += (orderDetails.Count * orderDetails.Price) * (1 + SD.SalesTaxPercent);
                _unitOfWork.OrderDetails.Add(orderDetails);
            }
            OrderDetailsCartVM.OrderHeader.OrderTotal = Convert.ToDouble(String.Format("{0:.##}", OrderDetailsCartVM.OrderHeader.OrderTotal));
            _unitOfWork.ShoppingCart.RemoveRange(OrderDetailsCartVM.ShoppingCart);
            HttpContext.Session.SetInt32(SD.ShoppingCart, 0);
            _unitOfWork.Save();

            if (stripeToken != null)
            {
                var options = new ChargeCreateOptions
                {
                    //Amount is in cents
                    Amount = Convert.ToInt32(OrderDetailsCartVM.OrderHeader.OrderTotal * 100 * SD.SalesTaxRate),
                    Currency = "usd",
                    Description = "Order ID : " + OrderDetailsCartVM.OrderHeader.Id,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);

                OrderDetailsCartVM.OrderHeader.TransactionId = charge.Id;

                if (charge.Status.ToLower() == "Succeeded")
                {
                    //send confirmation email here
                    OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                }
                else
                {
                    OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
            }
            _unitOfWork.Save();

            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = OrderDetailsCartVM.OrderHeader.Id });
        }

        public IActionResult OnPostPlus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToPage("/Customer/Cart/Index");
        }

        public IActionResult OnPostMinus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);

            if (cart.Count == 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();

                var cnt = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ShoppingCart, cnt);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
                _unitOfWork.Save();
            }
            return RedirectToPage("/Customer/Cart/Index");
        }

        public IActionResult OnPostRemove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();

            var cnt = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ShoppingCart, cnt);

            return RedirectToPage("/Customer/Cart/Index");
        }
    }
}