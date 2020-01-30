using Curry.DataAccess.Data.Repository.IRepository;
using Curry.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curry.DataAccess.Data.Repository
{
    public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
    {
        private readonly ApplicationDbContext _db;

        public MenuItemRepository(ApplicationDbContext db) : base (db)
        {
            _db = db;
        }

        public void Update(MenuItem menuItem)
        {
            var objFromDb = _db.MenuItem.FirstOrDefault(s => s.Id == menuItem.Id);

            objFromDb.Name = menuItem.Name;
            objFromDb.Price = menuItem.Price;
            objFromDb.Description = menuItem.Description;
            objFromDb.FoodTypeId = menuItem.FoodTypeId;
            objFromDb.CategoryId = menuItem.CategoryId;
            if (menuItem.Image != null)
                objFromDb.Image = menuItem.Image;

            _db.SaveChanges();
        }
    }
}
