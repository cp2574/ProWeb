using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TheFamilyFriend
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new AuthorizeAttribute());
            filters.Add(new HandleErrorAttribute());           
        }
    }

    #region 会出现重定向bug,解决方式注释掉 Global对过滤器的引用

    /// <summary>
        /// 需要登录才能进行操作
        /// </summary>
    public class PermissionRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool result = filterContext.ActionDescriptor.IsDefined(typeof(NoPermissionRequiredAttribute), true);
            if (result) return;
            if (HttpContext.Current.Session["LoginInfo"] == null)
            {

                //判断是否是ajax提交
                if (filterContext.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
                {
                    //如果是ajax提交返回错误码
                    HttpContext.Current.Response.StatusCode = 499;
                }
                else
                {
                    filterContext.HttpContext.Response.Redirect("/Account/Login");
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }


    //在不需要的方法上面打上标记

    public class NoPermissionRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

    }

    #endregion





  public class LoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
             //判断Action描述标签中是否有AllowAnonymous特性
         if(filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true))
            {
                return;
            }

            var a = HttpContext.Current.Session["LoginInfo"];
            if (HttpContext.Current.Session["LoginInfo"] == null)
            {
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }
            base.OnActionExecuting(filterContext);

        }
    }




    public class BaseController : Controller
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (filterContext.HttpContext.Session["username"] == null)
                filterContext.HttpContext.Response.Redirect("/Account/Login");
            base.OnActionExecuting(filterContext); }


    }
}
