using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.HelperModel;

namespace TheFamilyFriend.Controllers
{
    [AuthorizeFilter(Roles = "admin")]
    public class MailboxController : Controller
    {
        // GET: Mailbox
        public ActionResult Index()
        {
            //Email.EmailOperation.PushEmail("刘冲");
            return View();
        }
    }
}