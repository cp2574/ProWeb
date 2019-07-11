using Microsoft.AspNet.Identity;
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
using TheFamilyFriend.HelperModel.SystemManger;
using System.IO;

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

        #region 模块
        /// <summary>
        /// 菜单
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
        #endregion

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

        // <summary>
        /// 自己写的密码重置
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Super")]
        [AllowAnonymous]
        public ActionResult PasswordReset(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ApplicationUser user = UserManager.FindById(id);
            string code = UserManager.GeneratePasswordResetToken(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var resetPasswordViewModel = new PasswordRrestModel()
            {

                RealName = user.RealName,
                Password = user.PasswordHash,
                Code = code

            };
            return View(resetPasswordViewModel);
        }

        /// 重置密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Roles = "Super")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PasswordReset(string id, PasswordRrestModel resetPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordViewModel);
            }

            //User user = UserManager.FindById(id);
            string code = await UserManager.GeneratePasswordResetTokenAsync(id);
            //if (user == null)
            //{
            //  return View("Error");
            //}
            resetPasswordViewModel.Code = code;
            //resetPasswordViewModel.Password = "111111";

            var result = await UserManager.ResetPasswordAsync(id, resetPasswordViewModel.Code, resetPasswordViewModel.Password);
            if (result.Succeeded)
            {
                ViewBag.RPWMessagess = "重置成功";
                return RedirectToAction("UserList", "SystemManagement");
            }
            AddErrors(result);
            ViewBag.RPWMessagess = "失败";
            return View(resetPasswordViewModel);
        }

        /// <summary>
        /// 删除用户
        /// </summary>       
        public async Task<ActionResult> RemoveUser(string id) {
            ApplicationUser user = UserManager.FindById(id);
            await UserManager.DeleteAsync(user);
            return RedirectToAction("UserList", "SystemManagement");
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUser() {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(HelperModel.SystemManger.RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var _creatTime = DateTime.Now;
                var _headerPic = "/Content/Images/Avatar/defult.png";
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber, CreateTime = _creatTime, Avatar = _headerPic };
                var result = UserManager.FindByName(user.UserName);
                if (result == null)
                {
                    var results = await UserManager.CreateAsync(user, model.Password);
                    if (results.Succeeded)
                    {

                        return RedirectToAction("UserList");
                    }
                    AddErrors(results);
                }
                return Content("<script>alert('用户已存在');location.href='/SystemManagement/AddUser';</script>");                                        
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
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
                    file.SaveAs(strFileSavePath + "/" + user.RealName + strFileExtention);   //保存文件
                    user.Avatar = "/Content/Images/Avatar/" + user.RealName + strFileExtention;   //给模型赋值
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


        /// <summary>
        /// 下面是编辑用户资料的代码
        /// </summary>
        /// <param name="disposing"></param>
        [HttpGet]
        [Authorize(Roles = "Super")]
        public ActionResult EditUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            ///读出所有的部门名称分类，以下拉框的形式写入。
            //ViewBag.drolistDepart = (db.Departments).Select(g => new SelectListItem
            //{
            //    Text = g.DepartmentName,
            //    Value = g.Id,
            //    Selected = false
            //});


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
        ///Edit 的POST方法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="editUserViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Super")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(EditUsers editUserViewModel)
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
                    return RedirectToAction("UserList", "SystemManagement");
                }
                AddErrors(result);
            }
            /////读出所有的部门名称分类，以下拉框的形式写入。
            //ViewBag.drolistDepart = (db.Departments).Select(g => new SelectListItem
            //{
            //    Text = g.DepartmentName,
            //    Value = g.Id,
            //    Selected = false
            //});
            return View(editUserViewModel);
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
                            Name = m.Name,
                            Description= m.Description
                        };
            return Json(roles);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <returns></returns>
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
                    var result1 = await RoleManager.CreateAsync(new ApplicationRole() { Name= model.Name,Description= model.Description});

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
            ApplicationRole role = RoleManager.FindById(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            var editModel = new RolesViewModel()
            {
                Name = role.Name,
                Description =role.Description
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
                role.Description = model.Description;
                var result = await RoleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
              
            }
            return View(model);
        }



        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="disposing"></param>
        // POST: Menus/Delete/5
        [HttpGet]

        //[Authorize(Roles = "Super")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var role = RoleManager.FindById(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            var result = await RoleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                AddErrors(result);
            }
            return RedirectToAction("RoleList");
        }
        #endregion



        #region 角色与用户
        /// <summary>
        /// 用户到角色列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Super")]
        public ActionResult UserToRole(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var role = RoleManager.FindById(id);
            ViewBag.RoleName = role.Name;
            ViewBag.RoleId = id;
            if (role == null)
            {
                return HttpNotFound();
            }
            var memberIDs = role.Users.Select(x => x.UserId).ToArray();
            var members = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));
            var membersNo = UserManager.Users.Except(members);
            return View(membersNo);
        }
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
                var model = db.Logs.Where(t => (int)t.Type == id).OrderByDescending(x=>x.date).ToList();
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