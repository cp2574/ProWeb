using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TheFamilyFriend.HelperModel;
using TheFamilyFriend.Models;

namespace TheFamilyFriend.Controllers
{
  
    public class BosomFriendController : Controller
    {
        /// <summary>
        /// 上传的位置信息
        /// </summary>
       private readonly string Upload = ConfigurationManager.AppSettings.Get("Upload").ToString();

        // GET: BosomFriend
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        /// <summary>
        /// 小学同学
        /// </summary>
        /// <returns></returns>
        public ActionResult Primary()
        {
            using (var kingbd = new KinshipDb())
            {
                var personage = kingbd.Personage.Where(x => x.Type == "小学同学" || x.UserName == "admin").ToList();
                return View(personage);
            }
            
        }
      
        [AuthorizeFilter(Users = "rd1,admin")]
        /// <summary>
        /// 初中同学
        /// </summary>
        /// <returns></returns>
        public ActionResult Middle()
        {
            using (var kingbd=new KinshipDb())
            {
             var personage=kingbd.Personage.Where(x => x.Type == "初中同学" || x.UserName== "admin").ToList();
             return View(personage);
            }
        }
        [AllowAnonymous]
        /// <summary>
        /// 头像
        /// </summary>
        /// <returns></returns>
        public ActionResult HeadPortrait(int PersonId)
        {
            string imgpath;
            using (var kingbd = new KinshipDb())
            {
                var personage = kingbd.Personage.Find(PersonId);
                if(string.IsNullOrEmpty(personage.HeadPortrait)||!System.IO.File.Exists(personage.HeadPortrait))
                {
                    imgpath = Server.MapPath("/Content/Images/defult.png");
                }
                else
                {
                    imgpath = personage.HeadPortrait;
                } 
            }
            byte[] buff = System.IO.File.ReadAllBytes(imgpath);
            return File(buff, "image/jpeg");
        }

        [AuthorizeFilter(Users = "admin")]
        /// <summary>
        /// 高中同学
        /// </summary>
        /// <returns></returns>
        public ActionResult Senior()
        {
            using (var kingbd = new KinshipDb())
            {
                var personage = kingbd.Personage.Where(x => x.Type == "高中同学" || x.UserName == "admin").ToList();
                return View(personage);
            }
        }


        [AuthorizeFilter(Users = "admin")]
        /// <summary>
        /// 大学同学
        /// </summary>
        /// <returns></returns>
        public ActionResult University()
        {
            using (var kingbd = new KinshipDb())
            {
                var personage = kingbd.Personage.Where(x => x.Type == "大学同学" || x.UserName == "admin").ToList();
                return View(personage);
            }
        }

        [AuthorizeFilter(Users="admin")]
        /// <summary>
        /// 特殊人
        /// </summary>
        /// <returns></returns>
        public ActionResult Special()
        {
            using (var kingbd = new KinshipDb())
            {
                var personage = kingbd.Personage.Where(x => x.Type == "同事" || x.UserName == "admin").ToList();
                return View(personage);
            }
        }

        /// <summary>
        /// 添加新人
        /// </summary>
        /// <returns></returns>
        public ActionResult NewPeople()
        {
            NewPeopenType();
            return View();
        }
        [HttpPost]
        public JsonResult CreateNewpeopen(Personage person) {
            using (var KpDb = new KinshipDb())
            {

                if (KpDb.Personage.Where(x=>x.Name== person.Name).FirstOrDefault()==null)
                {

                    //KpDb.Personage.Add(new Personage()
                    //{
                    //    Name = person.Name,
                    //    Nickname = person.Nickname,
                    //    Phone = person.Phone,
                    //    Birthday = person.Birthday,
                    //    Blood = person.Blood,
                    //    Email = person.Email,
                    //    Gender = person.Gender,
                    //    Constellation = person.Constellation,
                    //    Type = person.Type,
                    //});
                    person.LikeGood = 1;
                    KpDb.Personage.Add(person);
                    if (KpDb.SaveChanges() > 0)
                    {
                        return Json(new { ReturnValue = true, message = isType(person.Type),UserId=person.PersonId });
                    }
                    else
                    {
                        return Json(new { ReturnValue = false, message = "新增失败" });
                    }
                }
                else
                {
                    return Json(new { ReturnValue = false, message = "已存在该用户！" });
                }
              
            }         
        }
        public void NewPeopenType() {
            List<SelectListItem> sele = new List<SelectListItem>() {
            new SelectListItem() { Text = "小学同学", Value = "小学同学" },
            new SelectListItem() { Text = "初中同学", Value = "初中同学" },
            new SelectListItem() { Text = "高中同学", Value = "高中同学" },
            new SelectListItem() { Text = "大学同学", Value = "大学同学" },
            };

            ViewBag.TypePeopen =sele;
        }
        public string isType(string TypeName) {
            string typevalue = "";
            switch (TypeName)
            {
                case "小学同学":
                    typevalue= "Primary";
                    break;

                case "初中同学":
                    typevalue = "Middle";
                    break;
                case "高中同学":
                    typevalue = "Senior";
                    break;
                case "大同学学":
                    typevalue = "University";
                    break;
                case "同事":
                    typevalue = "University";
                    break;

            }
            return typevalue;
        }


        /// <summary>
        /// 更新头像
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateHeadPortrait(int id) {
            var Files = HttpContext.Request.Files["HeadPortrait"];
            string suffix = Files.FileName.Substring(Files.FileName.LastIndexOf('.'));
            using (var KpDb = new KinshipDb())
            {
                Personage person = KpDb.Personage.Find(id);
                var savefile=Path.Combine(ConfigurationManager.AppSettings.Get("HeadPortrait").ToString(), person.Name + suffix);
                Files.SaveAs(savefile);
                person.HeadPortrait = savefile;
                KpDb.SaveChanges();
            }
            return Json("");
        }
        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]       
        public JsonResult Like(int ID) {
            using (var kindb=new KinshipDb())
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
                    return Json(new { ReturnValue = false,Message = "数据异常！" });
                }           
            }            
        }

        public JsonResult redayPerson(int PseronId) {

            using (var kpbd=new KinshipDb())
            {           
                return Json(kpbd.Personage.Find(PseronId));
            }       
        }

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
                return Json(new { returnValue=false,Message= ex.Message});
            }

        }


        #region 别针模块
        public ActionResult tag()
        {
            return View(getLblhelpmessage());
        }
        /// <summary>
        /// 用户所拥有的别针
        /// </summary>
        /// <returns></returns>
        public List<Lblhelpmessage> getLblhelpmessage()
        {
            var user = User.Identity.Name;
            using (var kindb = new KinshipDb())
            {
                return kindb.Lblhelpmessage.Where(x => x.UserName == user).ToList();
            }
        }
        /// <summary>
        /// 删除别针
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult DelectLbl(int Id)
        {

            using (var kinbd = new KinshipDb())
            {
                kinbd.Lblhelpmessage.Remove(kinbd.Lblhelpmessage.Find(Id));
                if (kinbd.SaveChanges() > 0)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }



        }
        /// <summary>
        /// 新增别针
        /// </summary>
        /// <param name="lbl"></param>
        /// <returns></returns>
        public JsonResult AddLbl(Lblhelpmessage lbl)
        {

            using (var kinbd = new KinshipDb())
            {
                lbl.UserName = User.Identity.Name;
                lbl.SignTime = DateTime.Now;
                kinbd.Lblhelpmessage.Add(lbl);
                if (kinbd.SaveChanges() > 0)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
        }
        /// <summary>
        /// 更新别针
        /// </summary>
        /// <param name="lbl"></param>
        /// <returns></returns>
        public JsonResult UpdateLbl(Lblhelpmessage lbl)
        {

            using (var kinbd = new KinshipDb())
            {

                var Lblhelpmessage = kinbd.Lblhelpmessage.Find(lbl.LblId);
                Lblhelpmessage.Title = lbl.Title;
                Lblhelpmessage.Explain = lbl.Explain;
                Lblhelpmessage.SignTime = DateTime.Now;
                if (kinbd.SaveChanges() > 0)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
        }
        #endregion

        #region 图片模块
        public ActionResult Picture()
        {

            ViewBag.ServerPicture = ConfigurationManager.AppSettings.Get("ServerPicture");
            using (var bd = new KinshipDb())
            {
                return View(bd.PictureType.ToList());
            }
        }

        public JsonResult ShowPicture() {
          
            using (var bd = new KinshipDb())
            {
                var list = bd.Picture.Where(x => x.UserName == "admin").Select(x => new { x.PicturePath, x.PictureTypeId }).ToList();
              return Json(list);
            }           
        }

        public JsonResult ShowSingerTypePicture(int PictureTypeId)
        {
            using (var bd = new KinshipDb())
            {
                if (PictureTypeId==0)
                {
                    var list = bd.Picture.Where(x => x.UserName == "admin").Select(x => new { x.PicturePath, x.PictureTypeId }).ToList();
                    return Json(list);
                }
                else
                {
                    var list = bd.Picture.Where(x => x.UserName == "admin" && x.PictureTypeId == PictureTypeId).Select(x => new { x.PicturePath, x.PictureTypeId }).ToList();
                    return Json(list);
                }
          
            }
        }



        [AuthorizeFilter]
        public ActionResult UploadPicture()
        {
            using (var bd = new KinshipDb())
            {
                return View(bd.PictureType.ToList());
            }
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public JsonResult BatchUpload()
        {
            int PictureTypeId = Convert.ToInt32(Request["PictureTypeId"].Split(',')[0]);
            bool isSavedSuccessfully = true;
            int count = 0;
            string msg = "";
            string fileName = "";
            string fileExtension = "";
            string filePath = "";
            string fileNewName = "";
            try
            {

                string directoryPath = Path.Combine(Upload, User.Identity.Name);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                List<Picture> list = new List<Picture>();
                foreach (string f in Request.Files)
                {

                    HttpPostedFileBase file = Request.Files[count];
                    if (file != null && file.ContentLength > 0)
                    {

                        fileName = file.FileName;
                        fileExtension = Path.GetExtension(fileName);
                        string fileExtensionToLower = fileExtension.ToLower();
                        if (fileExtensionToLower == ".jpg" || fileExtensionToLower == ".png" || fileExtensionToLower == ".jpeg")
                        {
                            //fileNewName = Guid.NewGuid().ToString() + fileExtension;
                            fileNewName = User.Identity.Name + "_" + Guid.NewGuid().ToString() + fileExtensionToLower;
                            filePath = Path.Combine(directoryPath, fileNewName);
                            count++;
                            list.Add(new Picture()
                            {
                                PictureName = fileNewName,
                                PictureTypeId = PictureTypeId,
                                PicturePath = User.Identity.Name + "/" + fileNewName,
                                UserName = User.Identity.Name,
                                UploadTime = DateTime.Now,
                                PersonId = null
                            });
                            //file.SaveAs(filePath);
                            Thumbnail.MakeThumbnail(file.InputStream, filePath);
                        }
                    }
                }
                if (count > 0)
                {
                    using (var bd = new KinshipDb())
                    {
                        bd.Picture.AddRange(list);
                        bd.SaveChanges();

                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                isSavedSuccessfully = false;
            }

            return Json(new
            {
                Result = isSavedSuccessfully,
                Count = count,
                Message = msg
            });
        }
        #endregion
    }
}