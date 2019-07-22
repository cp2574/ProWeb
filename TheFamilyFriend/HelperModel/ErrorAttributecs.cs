using Common;
using Microsoft.AspNet.Identity;
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
    /// 报错过滤器
    /// </summary>
    public class ErrorAttributecs : HandleErrorAttribute
    {
        //在程序中任何地方出现异常都会执行  
        public override void OnException(ExceptionContext filterContext)
        {
            //获取异常对象  
            Exception ex = filterContext.Exception;

            //string cookieName = FormsAuthentication.FormsCookieName;//读取登录授权Cookies的名称
            //HttpCookie authCookie = filterContext.HttpContext.Request.Cookies[cookieName];//接收这个Cookies
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //string UserName = authTicket.Name;
            string UserName = filterContext.HttpContext.User.Identity.Name;
            //记录错误日志
            using (KinshipDb db = new KinshipDb()) {
                db.Logs.Add(new TLogs() {
                    Account= UserName,
                    code=filterContext.RouteData.Values["action"].ToString(),
                    IP= Host.IPAddress,
                    Url= filterContext.HttpContext.Request.Url.ToString(),
                    Result=ex.Message,
                    Type=logtype.报错日志,
                    date=DateTime.Now
                });
                db.SaveChanges();
            }
            //导向友好错误界面  
            //   filterContext.Result = new RedirectResult("/Home/Index");
            string message = "走丢啦,详情请看日志！";
            filterContext.Result = new  ContentResult { Content = "<script>alert('" + message.Replace("\r", "").Replace("\n", "").Replace("\t", "") + "');history.go(-1);</script>" };
            //重要！！告诉系统异常已处理！！如果没有这个步骤，系统还是会按照正常的异常处理流程走  
            filterContext.ExceptionHandled = true;
            //base.OnException(filterContext);  
        }

    }
}