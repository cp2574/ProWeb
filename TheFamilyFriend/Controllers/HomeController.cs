using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.HelperModel;
using TheFamilyFriend.Models;

namespace TheFamilyFriend.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {        
        [AllowAnonymous]
        public ActionResult Index()
        {
            //using (var KpDb = new KinshipDb())
            //{
            //    Personage person = new Personage();
            //    person.Name = "陈朋";
            //    person.Age = 23;
            //    person.Gender = "男";
            //    person.Height = 175;
            //    person.Premise = "湖北武汉";
            //    person.Hometown = "湖北阳新";
            //    person.Email = "860312862@qq.com";
            //    person.Birthday = Convert.ToDateTime("1995-09-21");
            //    person.Profession = "程序员";

            //    KpDb.Personage.Add(person);
            //    KpDb.SaveChanges();
            //}
            return View();
         }
        public ActionResult PersonInfo()
        {

            var user = User.Identity.Name;
            using (var KpDb = new KinshipDb())
            {

             var getPerson=KpDb.Personage.Single(x => x.UserName == user);
                
             return View(getPerson);
            }     
        }
        public ActionResult SetPersonInfo(Personage person)
        {

            var user = User.Identity.Name;
            using (var KpDb = new KinshipDb())
            {

                var getPerson = KpDb.Personage.Single(x => x.UserName == user);

                return View(getPerson);
            }
        }
        [HttpPost]
        public JsonResult Generate(Personage person) {
            using (var KpDb = new KinshipDb())
            {

                var getPerson = KpDb.Personage.Single(x => x.Name == person.Name);
                getPerson.Nickname = person.Nickname;
                getPerson.Phone = person.Phone;
                getPerson.Birthday = person.Birthday;
                getPerson.Blood = person.Blood;
                getPerson.Email = person.Email;
                getPerson.Gender = person.Gender;
                getPerson.Constellation = person.Constellation;
                if (KpDb.SaveChanges()>0)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
              
            }

        }

        [AllowAnonymous]
        public ActionResult Unauthorized() {

           return  View();
        }

    }
} 