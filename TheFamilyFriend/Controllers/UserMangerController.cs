using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.HelperModel;
using TheFamilyFriend.Models;

namespace TheFamilyFriend.Controllers
{
    [AuthorizeFilter(Roles ="admin")]

    public class UserMangerController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: UserManger
        public ActionResult Index()
        {
        
          return View();
        }
        
        public JsonResult GetuserArray() {
            var userlistUser = UserManager.Users.OrderBy(x => x.CreateTime).ToList();
            return Json(userlistUser);               
        }

       
    }
}