using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheFamilyFriend.HelperModel
{
    public class AuthorizeFilterAttribute:AuthorizeAttribute
    {
   
        /// <summary>
        /// 授权的逻辑处理, 授权成功true,否则false      可根据实际需求处理
        /// </summary>
        /// <param name="httpContext">http请求</param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

           
            if (base.AuthorizeCore(httpContext))
            {
                return true;
            }
            else
            {
                return false;
            }
        
        //if (httpContext == null)
        //{
        //    throw new ArgumentNullException("HttpContext");
        //}
        //if (!httpContext.User.Identity.IsAuthenticated)
        //{
        //    return false;
        //}
        //if (Roles == null)
        //{
        //    return true;
        //}   
        //if (Roles.Length>0)
        //{
        //    return true;
        //}
        //if (httpContext.User.IsInRole(Roles))
        //{
        //    return true;
        //}
        //return false;
    }
            /// <summary>
            /// 授权失败跳转页面
            /// </summary>
            /// <param name="filterContext"></param>
            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {            
            if(string.IsNullOrEmpty( filterContext.HttpContext.User.Identity.Name))
            {
                filterContext.HttpContext.Response.Redirect("/Account/Login");
            }
            else
            {
                filterContext.HttpContext.Response.Redirect("/Home/Unauthorized");
            }              
                ///继承AuthorizeAttribute默认方法
                //base.HandleUnauthorizedRequest(filterContext);
            }      
    }
}