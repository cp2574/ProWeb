﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.HelperModel;

namespace TheFamilyFriend.Controllers
{
    [Authorize(Roles = "Super,admin,Relative")]
    public class PeopleController : BaseController
    {
        // GET: People
        public ActionResult Index()
        {

            ViewBag.ServerPicture = ConfigurationManager.AppSettings.Get("ServerPicture");
            return View();
        }
        public ActionResult Parents()
        {
            return View();
        }      
        public ActionResult Sister()
        {
            return View();
        }
    }
}