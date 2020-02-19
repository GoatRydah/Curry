using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Curry.DataAccess.Data.Repository.IRepository;
using Curry.Models;
using Curry.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Curry.Pages.Customer.Cart
{
    public class IndexModel : PageModel
    {
        public readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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
            }
        }
    }
}