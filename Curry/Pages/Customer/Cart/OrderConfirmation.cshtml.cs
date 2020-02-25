﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Curry.Pages.Customer.Cart
{
    public class OrderConfirmationModel : PageModel
    {
        [BindProperty]
        public int OrderId { get; set; }

        public void OnGet(int id)
        {
            OrderId = id;
        }
    }
}