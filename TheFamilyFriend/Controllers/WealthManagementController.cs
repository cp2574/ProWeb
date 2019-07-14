using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.HelperModel;
using TheFamilyFriend.Models;
namespace TheFamilyFriend.Controllers
{
    [AuthorizeFilter(Roles = "admin")]
    /// <summary>
    /// 账户管理
    /// </summary>
    public class WealthManagementController : BaseController
    {
        // GET: WealthManagement
        public ActionResult Index()
        {

            var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(MonthList());
            ViewBag.SumPrice = json;
            return View();
        }

        /// <summary>
        /// 消费录入
        /// </summary>
        /// <returns></returns>
        public ActionResult ConsumerEntry()
        {

            return View();
        }


        public JsonResult AddConsumer(ConsumptionExpenditure consumptionExpenditure) {
            try
            {
                using (var bd = new KinshipDb())
                {
                    bd.ConsumptionExpenditure.Add(consumptionExpenditure);
                    bd.SaveChanges();
                }
                return Json(new { succeed = true, message ="添加成功！" });
            }
            catch (Exception ex)
            {

                return Json(new { succeed= false, message=ex.Message});
            }

           
        }


        /// <summary>
        /// 消费清单
        /// </summary>
        /// <returns></returns>
        public ActionResult ConsumerList()
        {

            return View();
        }

        public JsonResult GetConsumerLists(int pageNumber, int pageSize)
        {

            using (var bd = new KinshipDb())
            {
                IQueryable<ConsumptionExpenditure> list = bd.ConsumptionExpenditure;
                if (!string.IsNullOrEmpty(Request["StartTime"]))
                {
                    DateTime StartTime = Convert.ToDateTime(Request["StartTime"]);
                    list = list.Where(x => x.RegisterTime >= StartTime);
                }
                if (!string.IsNullOrEmpty(Request["EndTime"]))
                {
                    DateTime StartTime = Convert.ToDateTime(Request["EndTime"]);
                    list = list.Where(x => x.RegisterTime <= StartTime);
                }

                var fenyelist = list.OrderByDescending(x => x.RegisterTime).Take(pageSize * pageNumber).Skip(pageSize * (pageNumber - 1)).ToList();
                var fenye = new
                {
                    total = list.Count(),//数据的总量
                    rows = fenyelist///分页返回的行数
                };
                return Json(fenye, JsonRequestBehavior.AllowGet);
            }
        }



        public List<decimal> MonthList()
        {         
            using (var bd = new KinshipDb())
            {

               string sqlstr = "select SUM(Money) as Money from[ConsumptionExpenditure] group by MONTH(RegisterTime)";

                return bd.Database.SqlQuery<decimal>(sqlstr).ToList();
            }
          
        }

    }
}