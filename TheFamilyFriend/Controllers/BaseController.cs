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
    //[Authorize]
    public class BaseController : Controller
    {
        protected KinshipDb db = new KinshipDb();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

     
        protected ApplicationRoleManager RoleManager
        {
            get { return _roleManager ?? Request.GetOwinContext().Get<ApplicationRoleManager>(); }
            set { _roleManager = value; }
        }
        protected ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        protected ApplicationUserManager UserManager
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

        

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Request.IsAuthenticated)
            {

                //List<MenuInfo> UserMenus = Session["menus"] as List<MenuInfo>;
                List<MenuInfo> UserMenus = new List<MenuInfo>();
                //if (UserMenus == null)
                //{

                    ///获取当前用户的id
                    var userID = User.Identity.GetUserId();
                    ///先读出当前用户表对应的所有的ROLES表，但只有rolename.
                    var rolses = UserManager.GetRoles(userID);
                    ///先读出当前用户的完整表.
                    var alluser = UserManager.FindById(userID);                  
                    ///先读出当前所有的ROLES表.
                    var allrolses = RoleManager.Roles.ToList();


                    ///匹配这个用户ID的角色列表ID出来
                    var user_roleid = allrolses.Where(x => rolses.Contains(x.Name)).Select(n => n.Id).ToList();
                    //var rolses = RoleManager.Roles.ToList();
                    //这里是所有角色，不是当前用户的角色
                    //var roleIds = rolses.Select(m => m.id).ToList();
                    var role_menus = db.RoleMenu.Where(m => user_roleid.Contains(m.RoleId)).ToList();//查询出角色关联表的所有数据
                                                                                                      //var role_menus = db.RoleMenus.Where(m =>m.RoleId== rolses.Select(x=>x.id) rolses.Contains(m.RoleId)).ToList();//查询出角色关联表的所有数据
                    var menuIds = role_menus.Select(n => n.MenuId).ToArray();
                    var menus = db.MenuInfo.OrderBy(x => x.Sort).Where(m => menuIds.Contains(m.Id)).ToList();//查询出菜单数据
                                                                                                              ///下面进行表的读成菜单样式
                                                                                                              ///定义一个变量result的MenuInfo列表变量
                    var result333 = new List<MenuInfo>();
                    ///读出所有的一级菜单
                    var level0 = menus.Where(m => m.Popedomfatherid == 0).ToList();
                    ///对所有一级菜单进行一次循环。 foreah (var xx in level0) 这种一样
                    level0.ForEach(item =>
                    {
                        ///定义一个children字菜单变量，当他的Popedomfatherid=当前循环的ID,取出当前的所有字菜单
                        var children = menus.Where(m => m.Popedomfatherid == item.Id).ToList();
                        /////给子菜单名字前面加上几个---
                        //children.ForEach(m => m.MenuName = "------------" + m.MenuName);
                        /////为新定义的result变量增加一个 一级菜单
                        result333.Add(item);
                        ///为新定义的result变量增加一个 所有的字菜单
                        result333.AddRange(children);
                    });
                    //Session["menus"] = result333;
                    UserMenus = result333;
               // }
                ViewBag.Menu = UserMenus;
                base.OnActionExecuted(filterContext);

            }
            else
            {
                ViewBag.Menu = new List<MenuInfo>();
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

    }
}