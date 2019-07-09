﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TheFamilyFriend.HelperModel;
using TheFamilyFriend.Models;

namespace TheFamilyFriend.Controllers
{
    [Authorize(Roles="admin")]
    public class SystemManagementController : Controller
    {
        // GET: SystemManagement

       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        
        /// <summary>
        /// 菜单模块
        /// </summary>
        /// <returns></returns>
        public ActionResult Menus()
        {
            return View();
        }
        public ActionResult AddMenus()
        {
            using (KinshipDb db = new KinshipDb())
            {
                if (ModelState.IsValid)
                {
               
                    List<Menus> MenuList = db.Menus.Where(T => T.Level < 3).OrderBy(t => t.Level).ToList();
                    List<SelectListItem> selectList = new List<SelectListItem>();
                    selectList.Add(new SelectListItem() { Value = "9999", Text = "无", Selected = false });
                    foreach (var item in MenuList)
                    {
                        SelectListItem sli = new SelectListItem() { Value = item.Id.ToString() };
                        switch (item.Level)
                        {
                            case 1:
                                sli.Text = "  " + item.Name;
                                break;
                            case 2:
                                sli.Text = item.Name;
                                break;
                            default:
                                break;
                        }
                        selectList.Add(sli);
                    }

                    SelectList sl = new SelectList(selectList, "Value", "Text");
                    ViewBag.ddmenu = sl;

                }
            }



            return View();
        }
        public JsonResult GetMenusList(int pageNumber, int pageSize)
        {

            using (var bd = new KinshipDb())
            {
                IQueryable<Menus> list = bd.Menus;
                //if (!string.IsNullOrEmpty(Request["StartTime"]))
                //{
                //    DateTime StartTime = Convert.ToDateTime(Request["StartTime"]);
                //    list = list.Where(x => x.RegisterTime >= StartTime);
                //}
                //if (!string.IsNullOrEmpty(Request["EndTime"]))
                //{
                //    DateTime StartTime = Convert.ToDateTime(Request["EndTime"]);
                //    list = list.Where(x => x.RegisterTime <= StartTime);
                //}

                var fenyelist = list.OrderByDescending(x => x.Id).Take(pageSize * pageNumber).Skip(pageSize * (pageNumber - 1)).ToList();
                var fenye = new
                {
                    total = list.Count(),//数据的总量
                    rows = fenyelist///分页返回的行数
                };
                return Json(fenye, JsonRequestBehavior.AllowGet);
            }
        }







        #region 用户模块
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
        public ActionResult UserList()
        {

            return View();
        }
        public JsonResult GetuserList()
        {
            var userlistUser = UserManager.Users.OrderBy(x => x.CreateTime).ToList();
            return Json(userlistUser);
        }

        /// <summary>
        /// 修改皮肤
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateUserSkip(string newskip)
        {
            try
            {
                ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    user.Skip = newskip;
                    IdentityResult result = UserManager.Update(user);
                    return Json(new { Issuccess = true, message = "修改成功！" });
                }
                else
                {
                    return Json(new { Issuccess = false, message = "用户异常，请联系管理员！" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { Issuccess = false, message = ex.Message });
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

        #endregion


        #region 角色模块

        // https://www.cnblogs.com/xiaodeng1979/articles/9563358.html

        public ActionResult RoleList()
        {

            return View();
        }
        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get { return _roleManager ?? Request.GetOwinContext().Get<ApplicationRoleManager>(); }
            set { _roleManager = value; }
        }

        public JsonResult GetroleList()
        {


            var roleList = RoleManager.Roles.ToList();

            var roles = from m in roleList
                        select new RolesViewModel
                        {
                            Id = m.Id,
                            Name = m.Name
                        };
            return Json(roles);
        }


        public ActionResult AddRole()
        {

            return View();
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddRole(AddRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var oneRole = await RoleManager.FindByNameAsync(model.Name);
                if (oneRole == null)
                {
                    var result1 = await RoleManager.CreateAsync(new IdentityRole(model.Name));

                    if (result1.Succeeded)
                    {
                        return RedirectToAction("RoleList");
                    }
                    AddErrors(result1);
                }
                return Content("<script>alert('角色已存在');location.href='/SystemManagement/AddRole';</script>");
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }
        public ActionResult EditRole()
        {

            return View();
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            IdentityRole role = RoleManager.FindById(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            var editModel = new RolesViewModel()
            {
                Name = role.Name
            };
            return View(editModel);
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRole(string id, RolesViewModel model)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(id))
            {
                var role = RoleManager.FindById(id);
                role.Name = model.Name;
                var result = await RoleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
              
            }
            return View(model);
        }
     



        //public async Task SetRole(string userName, string role)
        //{

        //    var oneRole = await RoleManager.FindByNameAsync(role);
        //    if (oneRole == null)
        //    {
        //        var result = await RoleManager.CreateAsync(new IdentityRole(role));
        //        if (!result.Succeeded)
        //        {
        //            //..
        //        }
        //    }
        //    var user = await UserManager.FindByNameAsync(userName);
        //    if (user != null)
        //    {
        //        var userRoles = UserManager.GetRoles(user.Id);
        //        if (!userRoles.Contains(role))
        //        {
        //            var result = await UserManager.AddToRoleAsync(user.Id, role);
        //            if (!result.Succeeded)
        //            {
        //                //..
        //            }
        //        }
        //    }
        //}



        #endregion








        #region 日志显示
        public ActionResult Logs(int? id)
        {
            using (KinshipDb db = new KinshipDb())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var model = db.Logs.Where(t => (int)t.Type == id).ToList();
                switch (id)
                {
                    case 0:
                        ViewBag.Title = "登录日志";
                        break;
                    case 1:
                        ViewBag.Title = "报错日志";
                        break;
                    case 2:
                        ViewBag.Title = "操作日志";
                        break;
                }
                return View("Logs", model);
            }

        }
        #endregion



        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}