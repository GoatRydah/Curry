using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Curry.DataAccess.Data.Repository.IRepository;
using Curry.Models;


namespace Curry.Pages.Customer.Home
{
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DetailsModel(IUnitOfWork UnitOfWork)
        {
            _unitOfWork = UnitOfWork;
        }

        [BindProperty]
        public ShoppingCart ShoppingCartObj { get; set; }


        public void OnGet(int id)
        {
            ShoppingCartObj = new ShoppingCart()
            {
                MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault
                (includeProperties: "Category,FoodType", filter: c => c.Id == id),
                MenuItemId = id
            };

        }
    }
}