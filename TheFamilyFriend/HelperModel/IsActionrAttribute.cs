using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TheFamilyFriend.Models;

namespace TheFamilyFriend.HelperModel
{
    /// <summary>
    /// 权限过滤器
    /// </summary>
    public class IsActionrAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { set; get; }

        /// <summary>
        /// 权限动作
        /// </summary>
        public actionType Action { set; get; }

        /// <summary>
        /// 返回结果形式
        /// </summary>
        public resultType result { set; get; } = resultType.Content;

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    bool isPermission = false;
        //    string cookieName = FormsAuthentication.FormsCookieName;//读取登录授权Cookies的名称
        //    HttpCookie authCookie = filterContext.HttpContext.Request.Cookies[cookieName];//接收这个Cookies
        //    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        //    TUserRoles userRole = JsonConvert.DeserializeObject<TUserRoles>(authTicket.UserData);
        //    if (userRole.role.RoleName != "管理员")
        //    {
        //        List<TPermission> permissionlist = HttpRuntime.Cache.Get("searchDataPermissions") as List<TPermission>;
        //        using (DbBaseContext db = new DbBaseContext())
        //        {
        //            if (permissionlist == null)//缓存里没有  
        //            {
        //                permissionlist = db.Permissions.Include(t => t.menu).Include(t => t.role).ToList();
        //                HttpRuntime.Cache.Insert("searchDataPermissions", permissionlist);
        //            }

        //            TPermission permission = permissionlist.Where(t => (t.role.Id == userRole.role.Id && t.menu.Name == MenuName)).FirstOrDefault();
        //            if (permission != null)
        //            {
        //                switch (Action)
        //                {
        //                    case actionType.add:
        //                    case actionType.edit:
        //                        isPermission = permission.iWrite;
        //                        break;
        //                    case actionType.delete:
        //                        isPermission = permission.iDelete;
        //                        break;
        //                    case actionType.audit:
        //                        isPermission = permission.iAudit;
        //                        break;
        //                }
        //            }
        //            if (!isPermission)
        //            {
        //                switch (result)
        //                {
        //                    case resultType.Content:
        //                        filterContext.Result = new ContentResult { Content = "<script>alert('你没有操作权限,请找管理员申请操作权限，谢谢!!');history.go(-1);</script>" };
        //                        break;
        //                    case resultType.Json:
        //                        {
        //                            Result resultjs = new Result();
        //                            resultjs.issucess = false;
        //                            resultjs.message = "你没有操作权限,请找管理员申请操作权限，谢谢!!";
        //                            filterContext.Result = new JsonResult
        //                            {
        //                                Data = resultjs
        //                            };

        //                        }
        //                        break;
        //                    default:
        //                        filterContext.Result = new ContentResult { Content = "<script>alert('你没有操作权限,请找管理员申请操作权限，谢谢!!');history.go(-1);</script>" };
        //                        break;
        //                }

        //            }
        //            //记录操作日志
        //            db.Logs.Add(new TLogs()
        //            {
        //                Account = userRole.user.Account,
        //                code = filterContext.RouteData.Values["action"].ToString(),
        //                IP = Host.IPAddress,
        //                Url = filterContext.HttpContext.Request.Url.ToString(),
        //                Result = "获取" + MenuName + "菜单的" + Action + "权限" + (isPermission ? "成功" : "失败"),
        //                Type = logtype.操作日志,
        //                date = DateTime.Now
        //            });
        //            db.SaveChanges();
        //        }
        //    }

        //    base.OnActionExecuting(filterContext);

        //}
    }

    public enum actionType
    {
        add,
        edit,
        delete,
        audit
    }

    public enum resultType
    {
        Json,
        Content
    }
}