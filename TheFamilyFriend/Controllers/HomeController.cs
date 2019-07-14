using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.HelperModel;
using TheFamilyFriend.HelperModel.SystemManger;
using TheFamilyFriend.Models;

namespace TheFamilyFriend.Controllers
{

    [Authorize]
    public class HomeController : BaseController
    {        
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
         }
        /// <summary>
        /// 下面是读取用户资料的代码
        /// </summary>
        /// <param name="disposing"></param>
        public ActionResult PersonInfo(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ApplicationUser user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ///定义一个EditUserViewModel模型
            var editUserViewModel = new EditUsers()
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                WX = user.WX,
                QQ = user.QQ,
                Address = user.Address,
                UserName = user.UserName,
                RealName = user.RealName,
                Avatar = user.Avatar,
                Gender = user.Gender ?? 0,
                Birthday = user.Birthday ?? DateTime.Now
            };
            return View(editUserViewModel);
        }

        /// <summary>
        //  /Edit 的POST方法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="editUserViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]      
        public async Task<ActionResult> PersonInfo(EditUsers editUserViewModel)
        {

            if (ModelState.IsValid && !string.IsNullOrEmpty(editUserViewModel.Id))
            {
                ///用模型传入的数据，赋值给User 模型
                ApplicationUser user = UserManager.FindById(editUserViewModel.Id);
                user.Email = editUserViewModel.Email;
                user.PhoneNumber = editUserViewModel.PhoneNumber;
                user.WX = editUserViewModel.WX;
                user.QQ = editUserViewModel.QQ;
                user.Address = editUserViewModel.Address;
                user.RealName = editUserViewModel.RealName;
                user.Gender = editUserViewModel.Gender;
                user.Birthday = editUserViewModel.Birthday;
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("PersonInfo", "Home",new { id= editUserViewModel.Id });
                   
                }
                AddErrors(result);
            }       
            return View(editUserViewModel);
        }


        /////// <summary>
        /////// 上传头像
        /////// </summary>
        //// GET: Upload
        [HttpGet]
        public ActionResult UploadHeaderPic(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ApplicationUser user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var uploadHeader = new UploadHeader()
            {
                Id = user.Id,
                Avatar = user.Avatar,
                RealName = user.RealName
            };

            return View(uploadHeader);
        }
        /////// <summary>
        /////// 上传头像
        /////// </summary>
        ////POST: Upload
        [HttpPost]
        //[Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadHeaderPic(string id, UploadHeader uploadHeader)

        {
            //int saveCount = 0; //定义变量

            if (ModelState.IsValid && !string.IsNullOrEmpty(id))
            {
                ApplicationUser user = UserManager.FindById(id);
                var files = Request.Files;
                if (files.Count > 0)
                {
                    var file = files[0];
                    string strFileSavePath = Request.MapPath("~/Content/Images/Avatar");  //定义上传地址
                    string strFileExtention = Path.GetExtension(file.FileName);
                    if (!Directory.Exists(strFileSavePath))
                    {
                        Directory.CreateDirectory(strFileSavePath);
                    }
                    ///这种方式，上传的为username+扩展名，相当于覆盖以前的图片
                    string filename = "";
                    if (!string.IsNullOrEmpty(user.RealName))
                    {
                        filename = user.RealName;
                    }
                    else
                    {
                        filename = user.UserName;
                    }
                    file.SaveAs(strFileSavePath + "/" + filename + strFileExtention);   //保存文件
                    user.Avatar = "/Content/Images/Avatar/" + filename + strFileExtention;   //给模型赋值
                }
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("UploadHeaderPic/" + id);
                }
                AddErrors(result);
            }
            return View(uploadHeader);


        }

        [AllowAnonymous]
        public ActionResult Unauthorized() {

           return  View();
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
} 