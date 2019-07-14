using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lemony.SystemInfo;
using TheFamilyFriend.HelperModel;
using System.Timers;

namespace TheFamilyFriend.Controllers
{
    [Authorize]
    public class ReportController : BaseController
    {
        // GET: Report
        public ActionResult Index()
        {        
            ViewBag.GetDrives = SystemInfo.GetDrives();
            return View();
        }
        public JsonResult GetDisk() {
            var GetDrives = SystemInfo.GetDrives();
            return Json(GetDrives);
        }
        public ActionResult CpuLoad()
        {
            System.Timers.Timer cupTimer = new System.Timers.Timer();
            cupTimer.Interval = 1500;
            cupTimer.Enabled = true;
            cupTimer.Elapsed += new ElapsedEventHandler(cupTimerEvent);
            return View();
        }
        SystemInfo sys = new SystemInfo();
        public void cupTimerEvent(object o, ElapsedEventArgs e)
        {
            var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<TheFamilyFriend.Center.CpuHub>();
            hubContext.Clients.All.addNewMessageToPage(sys.CpuLoad);
        }

    }
}