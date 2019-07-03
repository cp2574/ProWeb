using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheFamilyFriend.Controllers
{
    public class SpecialEffectsController : Controller
    {
        // GET: SpecialEffects
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ThreeDShine()
        {
            return View();
        }
        /// <summary>
        /// 雪花
        /// </summary>
        /// <returns></returns>
        public ActionResult Snowflake()
        {
            return View();
        }
        

        /// <summary>
        ///  烟花
        /// </summary>
        /// <returns></returns>
        public ActionResult FireWorks()
        {
            return View();
        }
        /// <summary>
        ///  炫酷文字
        /// </summary>
        /// <returns></returns>
        public ActionResult Bildschirmtext()
        {
            return View();
        }
    }
}