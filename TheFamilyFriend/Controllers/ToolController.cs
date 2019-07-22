using BaiDu_OCR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheFamilyFriend.Controllers
{
    [AuthorizeAttribute]
    public class ToolController : BaseController
    {
        // GET: Tool
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult WenZiMessage() {
            try
            {
                var Files = HttpContext.Request.Files["Img"];
                string root = ConfigurationManager.AppSettings.Get("WenZiImg").ToString();
                if(!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
                var savefile = Path.Combine(root, Files.FileName);
                Files.SaveAs(savefile);
                string Message = BaiduAipOcr.TextDiscern(savefile);
                return Json(new { Message });
            }
            catch (Exception ex)
            {
                return Json(new { Message=ex.Message });
            }
        }

        public ActionResult Signature() {

            return View();
        }
        /// <summary>
        /// 电子地图
        /// </summary>
        /// <returns></returns>
        public ActionResult Map()
        {

            return View();
        }

        /// <summary>
        /// 邮箱
        /// </summary>
        /// <returns></returns>
        public ActionResult Mailbox()
        {

            return View();
        }
        /// <summary>
        /// 日历
        /// </summary>
        /// <returns></returns>
        public ActionResult Calendar()
        {

            return View();
        }

    }
}