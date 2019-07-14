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
    [Authorize(Roles="Super")]
    public class SystemManagementController : BaseController
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
        [Authorize(Roles = "Super")]
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


        /// <summary>
        /// 添加用户到角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Super")]
        public ActionResult AddToRole(string userId, string roleName, string roleId)
        {
            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(roleName))
            {
                UserManager.AddToRole(userId, roleName);
            }
            return RedirectToAction("UserToRole/" + roleId);
        }
        /// <summary>
        /// 查看角色中的用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ///
        [Authorize(Roles = "Super")]
        public ActionResult ViewRoleUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var role = RoleManager.FindById(id);
            ViewBag.RoleName = role.Name;
            var memberIDs = role.Users.Select(x => x.UserId).ToArray();
            var members = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));
            return View(members);
        }

        [Authorize(Roles = "Super")]
        /// <summary>
        ///  解除用户与角色的关系
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult RemoveRoleUser(string roleName, string userId)
        {
            if (string.IsNullOrWhiteSpace(roleName) && string.IsNullOrWhiteSpace(userId))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var result = UserManager.RemoveFromRole(userId, roleName);
            var role = RoleManager.FindByName(roleName);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return Content("<script>alert('移除失败，请联系相关人员');location.href='/SystemManagement/ViewRoleUser?id ='"+ role.Id + ";</script>");
            }
            return RedirectToAction("ViewRoleUser", new { id = role.Id });
        }


        #endregion

        #region 菜单模块
        KinshipDb db = new KinshipDb();
        /// <summary>
        /// 菜单列表
        /// </summary>
        [HttpGet]
        public ActionResult MenuList(string SeachColumnString)
        {
            ///读出所有的一级菜单
            ViewBag.drolistmenu = db.MenuInfo.Where(m => m.Popedomfatherid == 0).OrderBy(x => x.Sort);
            ///读出所有的现有的菜单名称，以下拉框的形式写入。

            var menus = db.MenuInfo.Where(m => m.Popedomfatherid == 0).OrderBy(x => x.Sort).Select(g => new SelectListItem
            {
                Text = g.MenuName,
                Value = g.Id.ToString(),
                Selected = false
            }).ToList();
            menus.Insert(0, new SelectListItem { Value = "0", Text = "请选择", Selected = true });
            ViewBag.menuInfo = menus;

            ///下面是开始显示菜单的列表了
            ///读出所有的菜单列表
            var data = db.MenuInfo.ToList();
            ///定义一个变量result的MenuInfo列表变量
            var result = new List<MenuInfo>();
            ///读出所有的一级菜单
            var level0 = data.Where(m => m.Popedomfatherid == 0).ToList();
            ///当一级菜单索引选择有的时候，一级菜单就仅为我们选择的这个

            if (!string.IsNullOrEmpty(SeachColumnString))
            {
                var xxx = int.Parse(SeachColumnString);
                level0 = level0.Where(m => m.Id == xxx).ToList();
            }

            ///对所有一级菜单进行一次循环。 foreah (var xx in level0) 这种一样
            level0.ForEach(item =>
            {
                ///定义一个children字菜单变量，当他的Popedomfatherid=当前循环的ID,取出当前的所有字菜单
                var children = data.Where(m => m.Popedomfatherid == item.Id).ToList();
                ///给子菜单名字前面加上几个---
                children.ForEach(m => m.MenuName = "------------" + m.MenuName);
                ///为新定义的result变量增加一个 一级菜单
                result.Add(item);
                ///为新定义的result变量增加一个 所有的字菜单
                result.AddRange(children);
            });
            ///ViewBag.list传值
            ViewBag.List = result;

            return View();

        }

        /// <summary>
        /// 菜单列表
        /// </summary>
        /// <param name="disposing"></param>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MenuList([Bind(Include = "MenuName,MenuPath,MenuIcon,MethodName,ControllerName,Popedomfatherid,Sort")] MenuInfo model)
        {
        
                if (ModelState.IsValid)
                {

                    db.MenuInfo.Add(model);
                    await db.SaveChangesAsync();
                    return RedirectToAction("MenuList");
                }
                ///读出所有的现有的菜单名称，以下拉框的形式写入。

                var menus = db.MenuInfo.Where(m => m.Popedomfatherid == 0).OrderBy(x => x.Sort).Select(g => new SelectListItem
                {
                    Text = g.MenuName,
                    Value = g.Id.ToString(),
                    Selected = false
                }).ToList();

                menus.Insert(0, new SelectListItem { Value = "0", Text = "请选择", Selected = true });
                ViewBag.menuInfo = menus;
                ///读出所有的一级菜单
                ViewBag.drolistmenu = db.MenuInfo.Where(m => m.Popedomfatherid == 0).OrderBy(x => x.Sort);
                return View(model);
           

         

        }

        /// <summary>
        /// 菜单授权
        /// </summary>
        /// <param name="disposing"></param>
        [HttpGet]
        public ActionResult AuthorityMenu(string Id,string SeachColumnString)
        {

            if (string.IsNullOrWhiteSpace(Id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var role = RoleManager.FindById(Id);
            ViewBag.RoleName = role.Name;
            ViewBag.RoleId = Id;
            if (role == null)
            {
                return HttpNotFound();
            }
            ///读出所有的一级菜单
            ViewBag.drolistmenu = db.MenuInfo.Where(m => m.Popedomfatherid == 0).OrderBy(x => x.Sort);

            ///下面是开始显示菜单的列表了
            ///读出所有的菜单列表
            var data = db.MenuInfo.ToList();
            ///定义一个变量result的MenuInfo列表变量
            var result = new List<MenuInfo>();
            ///读出所有的一级菜单
            var level0 = data.Where(m => m.Popedomfatherid == 0).ToList();
            ///当一级菜单索引选择有的时候，一级菜单就仅为我们选择的这个
            if (!string.IsNullOrEmpty(SeachColumnString))
            {
                var xxx = int.Parse(SeachColumnString);
                level0 = level0.Where(m => m.Id == xxx).ToList();
            }

            ///对所有一级菜单进行一次循环。 foreah (var xx in level0) 这种一样
            level0.ForEach(item =>
            {
                ///定义一个children字菜单变量，当他的Popedomfatherid=当前循环的ID,取出当前的所有字菜单
                var children = data.Where(m => m.Popedomfatherid == item.Id).ToList();
                ///给子菜单名字前面加上几个---
                children.ForEach(m => m.MenuName = "------------" + m.MenuName);
                ///为新定义的result变量增加一个 一级菜单
                result.Add(item);
                ///为新定义的result变量增加一个 所有的字菜单
                result.AddRange(children);
            });
            ///ViewBag.list传值
            ViewBag.List = result;

            return View();

        }


        [HttpPost]
        public ActionResult AuthorityMenu(string RoleId, string [] meunid)
        {
            ///如Id为null，返回错误请求
            if (RoleId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ////删除包含有该角色Id的所有记录
            //删除所有的数据，就是清空表原有数据
            List<RoleMenu> xx = db.RoleMenu.Where(m => m.RoleId == RoleId).ToList();
            db.RoleMenu.RemoveRange(xx);
            ///将所有List C中的菜单ID与ROLEID一起写入表中
            if (meunid != null)
            {
                foreach (var id2 in meunid)
                {
                    RoleMenu roleMenu = new RoleMenu();
                    roleMenu.RoleId = RoleId;
                    roleMenu.MenuId = Convert.ToInt32(id2);
                    roleMenu.IsAvailable = 1;
                    db.RoleMenu.Add(roleMenu);
                }
            }
            db.SaveChanges();
            return RedirectToAction("AuthorityMenu", new { Id = RoleId });
         
        }



        /// <summary>
        /// 返回当前的角色所拥有的菜单
        /// </summary>
        /// <param name="disposing"></param>
        [HttpGet]
        public ActionResult ShowRoleMenu(string Id, string SeachColumnString)
        {
            ///是否登录
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }


            if (string.IsNullOrWhiteSpace(Id))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var role = RoleManager.FindById(Id);
            ViewBag.RoleName = role.Name;
            ViewBag.RoleId = Id;
            if (role == null)
            {
                return HttpNotFound();
            }
            var role_menus = db.RoleMenu.Where(m =>m.RoleId== role.Id).ToList();//查询出角色关联表的所有数据
            //var role_menus = db.RoleMenus.Where(m =>m.RoleId== rolses.Select(x=>x.id) rolses.Contains(m.RoleId)).ToList();//查询出角色关联表的所有数据
            var menuIds = role_menus.Select(n => n.MenuId).ToArray();
            var menus = db.MenuInfo.OrderBy(x => x.Sort).Where(m => menuIds.Contains(m.Id)).ToList();//查询出菜单数据
            ///下面进行表的读成菜单样式
            ///定义一个变量result的MenuInfo列表变量
            var result = new List<MenuInfo>();
            ///读出所有的一级菜单
            var level0 = menus.Where(m => m.Popedomfatherid == 0).ToList();
            ViewBag.drolistmenu = level0;


            if (!string.IsNullOrEmpty(SeachColumnString))
            {
                var xxx = int.Parse(SeachColumnString);
                level0 = level0.Where(m => m.Id == xxx).ToList();
            }
           
            ///对所有一级菜单进行一次循环。 foreah (var xx in level0) 这种一样
            level0.ForEach(item =>
            {
                ///定义一个children字菜单变量，当他的Popedomfatherid=当前循环的ID,取出当前的所有字菜单
                var children = menus.Where(m => m.Popedomfatherid == item.Id).ToList();
                /////为新定义的result变量增加一个 一级菜单
                children.ForEach(m => m.MenuName = "------------" + m.MenuName);
                result.Add(item);
                ///为新定义的result变量增加一个 所有的字菜单
                result.AddRange(children);
            });
            return View(result);
        }



        [Authorize(Roles = "Super")]
        /// <summary>
        ///  解除菜单与角色的关系
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult RemoveRoleMenu(string RoleId, string MenuId)
        {
            if (string.IsNullOrWhiteSpace(RoleId) && string.IsNullOrWhiteSpace(MenuId))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            using (var dbs=new KinshipDb())
            {
                int MenuIds = Convert.ToInt32(MenuId);
                var rolemenu= dbs.RoleMenu.FirstOrDefault(x => x.RoleId ==RoleId && x.MenuId == MenuIds);
                dbs.RoleMenu.Remove(rolemenu);
              
                if (dbs.SaveChanges()>0)
                {
                    return RedirectToAction("ShowRoleMenu", new { Id = RoleId });
                }
                else
                {
                    return Content("<script>alert('移除失败，请联系相关人员');location.href='/SystemManagement/ShowRoleMenu?id ='" + RoleId + ";</script>");
                }
            }
        }









        /// <summary>
        /// 返回当前用户所属的角色所拥有的菜单
        /// </summary>
        /// <param name="disposing"></param>
        [HttpGet]
        //public ActionResult ShowRoleMenu()
        //{
        //    ///是否登录
        //    if (!Request.IsAuthenticated)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    ///获取当前用户的id
        //    var userID = User.Identity.GetUserId();
        //    ///先读出当前用户表对应的所有的ROLES表，但只有rolename.
        //    var rolses = UserManager.GetRoles(userID);
        //    ///先读出当前用户的完整表.
        //    var alluser = UserManager.FindById(userID);
        //    ///先读出当前所有的ROLES表.
        //    var allrolses = RoleManager.Roles.ToList();
        //    ///皮配出这个用户ID的角色列表ID出来
        //    var user_roleid = allrolses.Where(x => rolses.Contains(x.Name)).Select(n => n.Id).ToList();
        //    //var rolses = RoleManager.Roles.ToList();
        //    //这里是所有角色，不是当前用户的角色
        //    //var roleIds = rolses.Select(m => m.id).ToList();
        //    var role_menus = db.RoleMenu.Where(m => user_roleid.Contains(m.RoleId)).ToList();//查询出角色关联表的所有数据
        //    //var role_menus = db.RoleMenus.Where(m =>m.RoleId== rolses.Select(x=>x.id) rolses.Contains(m.RoleId)).ToList();//查询出角色关联表的所有数据
        //    var menuIds = role_menus.Select(n => n.MenuId).ToArray();
        //    var menus = db.MenuInfo.OrderBy(x => x.Sort).Where(m => menuIds.Contains(m.Id)).ToList();//查询出菜单数据
        //    ///下面进行表的读成菜单样式
        //    ///定义一个变量result的MenuInfo列表变量
        //    var result = new List<MenuInfo>();
        //    ///读出所有的一级菜单
        //    var level0 = menus.Where(m => m.Popedomfatherid == 0).ToList();
        //    ///对所有一级菜单进行一次循环。 foreah (var xx in level0) 这种一样
        //    level0.ForEach(item =>
        //    {
        //        ///定义一个children字菜单变量，当他的Popedomfatherid=当前循环的ID,取出当前的所有字菜单
        //        var children = menus.Where(m => m.Popedomfatherid == item.Id).ToList();
        //        /////为新定义的result变量增加一个 一级菜单
        //        result.Add(item);
        //        ///为新定义的result变量增加一个 所有的字菜单
        //        result.AddRange(children);
        //    });
        //    //ViewBag.List = result;

        //    return View(result);

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