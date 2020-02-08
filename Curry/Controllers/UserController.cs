using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Curry.DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Curry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitOfWork.ApplicationUser.GetAll() });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(s => s.Id == id);
            if (objFromDb == null)
                return Json(new { success = false, message="Error while Locking or Unlocking User."});

            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //Lock User
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                //Unlock user for 100 years
                objFromDb.LockoutEnd = DateTime.Now.AddYears(100);
            }

            _unitOfWork.Save();
            return Json(new { success = true, message = "Lock/Unlock successful." });
        }
    }
}