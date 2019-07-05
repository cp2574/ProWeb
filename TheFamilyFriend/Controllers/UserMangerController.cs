using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.HelperModel;
using TheFamilyFriend.Models;

namespace TheFamilyFriend.Controllers
{
    [AuthorizeFilter(Roles ="admin")]

    public class UserMangerController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: UserManger
        public ActionResult Index()
        {
        
          return View();
        }
        
        public JsonResult GetuserArray() {
            var userlistUser = UserManager.Users.OrderBy(x => x.CreateTime).ToList();
            return Json(userlistUser);               
        }


        /// <summary>
        /// 修改皮肤
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public  JsonResult UpdateUserSkip(string newskip)
        {
            try
            {
                ApplicationUser user =  UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    user.Skip = newskip;
                    IdentityResult result =  UserManager.Update(user);
                    return Json(new { Issuccess = true, message = "修改成功！" });
                }
                else
                {
                    return Json(new { Issuccess = false,message = "用户异常，请联系管理员！" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { Issuccess = false,message = ex.Message });
            }
        }



        //[HttpPost]
        //public async Task<ActionResult> Edit(string id, AppUserViewModel appUserViewModel)
        //{
        //    ApplicationUser user = await UserManager.FindByIdAsync(id);
        //    if (user != null)
        //    {
        //        user.Email = appUserViewModel.Email;
        //        IdentityResult validEmail = await UserManager.UserValidator.ValidateAsync(user);
        //        if (!validEmail.Succeeded)
        //        {
        //            AddErrorsFromResult(validEmail);
        //        }
        //        else
        //        {

        //            user.Id = appUserViewModel.Id;
        //            user.Email = appUserViewModel.Email;
        //            user.EmailConfirmed = appUserViewModel.EmailConfirmed;
        //            user.PhoneNumber = appUserViewModel.PhoneNumber;
        //            user.PhoneNumberConfirmed = appUserViewModel.PhoneNumberConfirmed;
        //            user.TwoFactorEnabled = appUserViewModel.TwoFactorEnabled;
        //            user.LockoutEndDateUtc = appUserViewModel.LockoutEndDateUtc;
        //            user.LockoutEnabled = appUserViewModel.LockoutEnabled;
        //            user.AccessFailedCount = appUserViewModel.AccessFailedCount;
        //            user.UserName = appUserViewModel.UserName;
        //            user.QQ = appUserViewModel.QQ;
        //            user.QQLoginEnable = appUserViewModel.QQLoginEnable;
        //            user.WeChat = appUserViewModel.WeChat;
        //            user.WeChatLoginEnable = appUserViewModel.WeChatLoginEnable;
        //            user.EmpNo = appUserViewModel.EmpNo;
        //            user.IDCardNo = appUserViewModel.IDCardNo;

        //            IdentityResult result = await UserManager.UpdateAsync(user);

        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "用户木有发现");
        //    }
        //    return RedirectToAction("Index");
        //}



    }
}