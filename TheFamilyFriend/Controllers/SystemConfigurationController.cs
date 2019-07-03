using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace TheFamilyFriend.Controllers
{
    public class SystemConfigurationController : Controller
    {
        // GET: SystemConfiguration
        public ActionResult Index()
        {
            ViewBag.ServerPicture = ConfigurationManager.AppSettings.Get("ServerPicture");


            return View();
        }

        public JsonResult loadstate() {

            ConfigurationManager.RefreshSection("appSettings"); //强制刷新配置文件
            var BackMuisc = ConfigurationManager.AppSettings.Get("BackgroundMusic");          
            return Json(BackMuisc);
        }
        public JsonResult BackMuisc(string  state)
        {
            try
            {
                var BackMuisc = ConfigurationManager.AppSettings.Get("BackgroundMusic");
                if (BackMuisc!=state)
                {
                    AppSettings("BackgroundMusic", state);
                    return Json(new { ReturnValue = true });
                }
                else
                {
                    return Json(new { ReturnValue = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ReturnValue = false, Message = ex.Message });
            }
          
        }


        public JsonResult SetServerPicture(string state)
        {
            try
            {
                var BackMuisc = ConfigurationManager.AppSettings.Get("ServerPicture");
                if (BackMuisc != state)
                {
                    AppSettings("ServerPicture", state);
                    return Json(new { ReturnValue = true });
                }
                else
                {
                    return Json(new { ReturnValue = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ReturnValue = false, Message = ex.Message });
            }

        }


        public void AppSettings(string name,string state) {

            System.Configuration.Configuration cfa = WebConfigurationManager.OpenWebConfiguration("~");
            cfa.AppSettings.Settings[name].Value = state;//修改值
            cfa.Save();//保存
            ConfigurationManager.RefreshSection("appSettings"); //强制刷新配置文件
            
        }
    }
}