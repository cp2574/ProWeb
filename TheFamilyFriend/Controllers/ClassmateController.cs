using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.Models;

namespace TheFamilyFriend.Controllers
{
    public class ClassmateController : Controller
    {
        // GET: Classmate
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Primary(int ? id)
        {
            using (var kingbd = new KinshipDb())
            {
                var personage = kingbd.Personage;
                List<Personage> list = new List<Personage>();
                switch (id)
                {
                    case 1:
                        list= personage.Where(x => x.Type == "小学同学" || x.UserName == "admin").ToList();
                        break;
                    case 2:
                        list = personage.Where(x => x.Type == "初中同学" || x.UserName == "admin").ToList();
                        break;
                    case 3:
                        list = personage.Where(x => x.Type == "高中同学" || x.UserName == "admin").ToList();
                        break;
                    case 4:
                        list = personage.Where(x => x.Type == "大学同学" || x.UserName == "admin").ToList();
                        break;
                    default:
                        list = personage.Where(x => x.Type == "小学同学" || x.UserName == "admin").ToList();
                        break;
                }
                return View("Primary", list);
             
            }
        }


        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Like(int ID)
        {
            using (var kindb = new KinshipDb())
            {
                try
                {
                    var person = kindb.Personage.Find(ID);
                    person.LikeGood = person.LikeGood + 1;
                    kindb.SaveChanges();
                    return Json(new { ReturnValue = true, Message = person.LikeGood });
                }
                catch (Exception)
                {
                    return Json(new { ReturnValue = false, Message = "数据异常！" });
                }
            }
        }


        /// <summary>
        /// 获取个人信息
        /// </summary>
        /// <param name="PseronId"></param>
        /// <returns></returns>
        public JsonResult redayPerson(int PseronId)
        {

            using (var kpbd = new KinshipDb())
            {
                return Json(kpbd.Personage.Find(PseronId));
            }
        }

        /// <summary>
        /// 修改个人信息
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Generate(Personage person)
        {
            try
            {
                using (var KpDb = new KinshipDb())
                {
                    var getPerson = KpDb.Personage.Single(x => x.Name == person.Name);
                    getPerson.Nickname = person.Nickname;
                    getPerson.Phone = person.Phone;
                    getPerson.Birthday = person.Birthday;
                    getPerson.Gender = person.Gender;
                    getPerson.Email = person.Email;
                    getPerson.Hometown = person.Hometown;
                    getPerson.Profession = person.Profession;
                    if (KpDb.SaveChanges() > 0)
                    {
                        return Json(new { returnValue = true, Message = "修改成功！" });
                    }
                    else
                    {
                        return Json(new { returnValue = true, Message = "修改失败！" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { returnValue = false, Message = ex.Message });
            }

        }



        [AllowAnonymous]
        /// <summary>
        /// 获取头像
        /// </summary>
        /// <returns></returns>
        public ActionResult HeadPortrait(int PersonId)
        {
            string imgpath;
            using (var kingbd = new KinshipDb())
            {
                var personage = kingbd.Personage.Find(PersonId);
                if (string.IsNullOrEmpty(personage.HeadPortrait) || !System.IO.File.Exists(personage.HeadPortrait))
                {
                    imgpath = Server.MapPath("/Content/Images/Avatar/defult.png");
                }
                else
                {
                    imgpath = personage.HeadPortrait;
                }
            }
            byte[] buff = System.IO.File.ReadAllBytes(imgpath);
            return File(buff, "image/jpeg");
        }




        /// <summary>
        /// 更新头像
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateHeadPortrait(int id)
        {
            var Files = HttpContext.Request.Files["HeadPortrait"];
            string suffix = Files.FileName.Substring(Files.FileName.LastIndexOf('.'));
            using (var KpDb = new KinshipDb())
            {
                Personage person = KpDb.Personage.Find(id);
                var savefile = Path.Combine(ConfigurationManager.AppSettings.Get("HeadPortrait").ToString(), person.Name + suffix);
                Files.SaveAs(savefile);
                person.HeadPortrait = savefile;
                KpDb.SaveChanges();
            }
            return Json("");
        }
    }
}