using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Curry.DataAccess.Data.Repository.IRepository;
using Curry.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Curry.Pages.Admin.MenuItems
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UpsertModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public MenuItemVM MenuItemObj { get; set; }

        public IActionResult OnGet(int? id)
        {
            MenuItemObj = new MenuItemVM
            {
                CategoryList =
                _unitOfWork.Category.GetCategoryListOrDropdown(),
                FoodTypeList =
                _unitOfWork.FoodType.GetFoodTypeListOrDropDown(),
                menuItem = new Models.MenuItem()
            };
            


            if (id != null) //editing
            {
                MenuItemObj.menuItem = _unitOfWork.MenuItem.GetFirstOrDefault(u => u.Id == id);
                if (MenuItemObj == null)
                    return NotFound();
            } //creating handled on post method

            return Page();
        }

        public IActionResult OnPost()
        {
            //Find path to wwwroot
            string webRoutePath = _hostingEnvironment.WebRootPath;
            //Grab the file(s) from the form
            var files = HttpContext.Request.Form.Files;

            if (!ModelState.IsValid)
                return Page();

            if (MenuItemObj.menuItem.Id == 0) //New Category
            {
                //rename the file
                string fileName = Guid.NewGuid().ToString();

                //upload file to path
                var uploads = Path.Combine(webRoutePath, @"images\menuItems");

                //preserce extension (.jpg, .png, etc)
                var extension = Path.GetExtension(files[0].FileName);

                //append new name to extension
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                MenuItemObj.menuItem.Image = @"\images\menuItems\" + fileName + extension;

                _unitOfWork.MenuItem.Add(MenuItemObj.menuItem);
            }
            else //edit
            {
                var objFromDb = _unitOfWork.MenuItem.Get(MenuItemObj.menuItem.Id);
                //were there any files submitted with the post
                if (files.Count > 0)
                {
                    //rename the file
                    string fileName = Guid.NewGuid().ToString();

                    //upload file to path
                    var uploads = Path.Combine(webRoutePath, @"images\menuItems");

                    //preserce extension (.jpg, .png, etc)
                    var extension = Path.GetExtension(files[0].FileName);

                    var imagePath = Path.Combine(webRoutePath, objFromDb.Image.Trim('\\'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);

                        //append new name to extension
                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        MenuItemObj.menuItem.Image = @"\images\menuItems\" + fileName + extension;
                    }
                    else
                    {
                        MenuItemObj.menuItem.Image = objFromDb.Image;
                    }
                }
                _unitOfWork.MenuItem.Update(MenuItemObj.menuItem);
            }

            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}