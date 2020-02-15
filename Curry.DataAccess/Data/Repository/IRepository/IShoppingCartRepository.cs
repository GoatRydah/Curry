using Curry.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Curry.DataAccess.Data.Repository.IRepository
{
    public interface IShoppingCartRepository: IRepository<ShoppingCart>
    {
        int IncrementCount(ShoppingCart shoppingCart, int Count);
        int DecrementCount(ShoppingCart shoppingCart, int Count);
    }
}
