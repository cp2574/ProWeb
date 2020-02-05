using BaiDu_OCR;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.HelperModel;
using TheFamilyFriend.Models;

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

            if (User.IsInRole("Super"))
            {
                ViewBag.userlist = Newtonsoft.Json.JsonConvert.SerializeObject(UserManager.Users.OrderBy(x => x.CreateTime).Select(x => new { x.Id, x.RealName, x.Avatar, x.Lng, x.Lat, x.Roles }));
            }
            else
            {
                var memberIDs = RoleManager.FindByName("Super").Users.Select(x => x.UserId).ToArray();
                var members = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id)).ToList();
                members.Insert(0, UserManager.FindById(User.Identity.GetUserId()));
                ViewBag.userlist = Newtonsoft.Json.JsonConvert.SerializeObject(members);
            }
            var userID = User.Identity.GetUserId();
            ///先读出当前用户的完整表.
            var alluser = UserManager.FindById(userID);
            ViewBag.userposition = "[" + alluser.Lng + "," + alluser.Lng + "]";
            return View();
        }
        public ActionResult SetUserPosition(string Lng,string Lat,string Id,string Address)
        {
            try
            {
                ApplicationUser user = UserManager.FindById(Id);
                if (user != null)
                {
                    user.Address = Address;
                    user.Lng = Lng;
                    user.Lat = Lat;
                    IdentityResult result = UserManager.Update(user);
                    return Json(new Result { issucess = true, message = "修改成功！" });
                }
                else
                {
                    return Json(new Result { issucess = false, message = "用户异常，请联系管理员！" });
                }
            }
            catch (Exception ex)
            {
                return Json(new Result { issucess = false, message = ex.Message });
            }

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