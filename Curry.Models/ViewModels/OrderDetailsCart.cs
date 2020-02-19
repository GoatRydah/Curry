using System;
using System.Collections.Generic;
using System.Text;

namespace Curry.Models.ViewModels
{
    public class OrderDetailsCart
    {
        public OrderHeader OrderHeader { get; set; }
        public List<ShoppingCart> ShoppingCart { get; set; }
    }
}
