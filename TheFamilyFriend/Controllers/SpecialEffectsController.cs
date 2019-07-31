using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheFamilyFriend.Controllers
{
    public class SpecialEffectsController : BaseController
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

        /// <summary>
        ///  鼠标移动特效
        /// </summary>
        /// <returns></returns>
        public ActionResult MouseMovement()
        {
            return View();
        }

        public ActionResult MoveLine() {


            return View();
        }
        public ActionResult Atar()
        {


            return View();
        }


        public ActionResult Flower()
        {


            return View();
        }

        public PartialViewResult Flowers(int n) {
            string action = "";
            switch (n)
            {
                case 1:
                    action = "_PartialFlower1";
                    break;
                default:
                    action = "_PartialFlower2";
                    break;
            }
            return PartialView(action);
        }
    }
}