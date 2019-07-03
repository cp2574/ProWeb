using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.HelperModel;

namespace TheFamilyFriend.Controllers
{
    [AuthorizeFilter(Users = "admin,rd1")]
    public class PeopleController :Controller
    {
        // GET: People
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Father()
        {
            return View();
        }
        public ActionResult Mother()
        {
            return View();
        }
        public ActionResult Sister()
        {
            return View();
        }
    }
}