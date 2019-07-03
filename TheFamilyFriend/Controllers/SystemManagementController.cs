using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.Models;

namespace TheFamilyFriend.Controllers
{
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
        
        /// <summary>
        /// 菜单模块
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
     
    }
}