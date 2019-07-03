using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TheFamilyFriend.HelperModel;
using TheFamilyFriend.Models;

namespace TheFamilyFriend
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ////当model发生更改，执行以下
            Database.SetInitializer<KinshipDb>(null);

            System.Timers.Timer objTimer = new System.Timers.Timer();
            objTimer.Interval = 10800000;
            objTimer.Enabled = true;
            objTimer.Elapsed += new ElapsedEventHandler(TimerEvent);

          

        }
        /// <summary>
        /// 表示三小时更新一次天气
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void TimerEvent(object o,ElapsedEventArgs e) {
            string Weatherfilesave = Path.Combine(Server.MapPath("/Models"), "WeatherReport.xml");
            WeatherWeb.CreateXML(Weatherfilesave);
        }
        /// <summary>
        /// 程序异常，执行该代码
        /// </summary>
        protected void Application_Error()
        {
            try
            {

                HttpException lastErrorWrapper = Server.GetLastError() as HttpException;
                int HttpCode = lastErrorWrapper.GetHttpCode();
                switch (HttpCode)
                {
                    case 404://（未找到） 服务器找不到请求的网页
                    case 405:
                        Response.Redirect("~/404.html", true);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Response.Redirect("~/500.html", true);
            }
        }
    }

}
