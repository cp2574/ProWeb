using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheFamilyFriend.cn.com.webxml.www;
using TheFamilyFriend.HelperModel;
using System.Xml.Linq;
using System.Configuration;

namespace TheFamilyFriend.Controllers
{
    public class WeatherReportController : BaseController
    {
        // GET: WeatherReport
        public ActionResult Index()
        {
            
            string Weatherfilesave = Path.Combine(Server.MapPath("/"), "WeatherReport.xml");

            WeatherWeb weath = new WeatherWeb();

            DateTime dt = System.IO.File.GetCreationTime(Weatherfilesave);


            if (!System.IO.File.Exists(Weatherfilesave))
            {
                WeatherWeb.CreateXML(Weatherfilesave);
            }
            else
            {
               DateTime time= System.IO.File.GetLastWriteTime(Weatherfilesave);
               TimeSpan t = Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(time.ToShortDateString());
                if (t.TotalDays>1)
                {
                    System.IO.File.Delete(Weatherfilesave);
                    WeatherWeb.CreateXML(Weatherfilesave);
                } 
            }
            XDocument xdoc = XDocument.Load(Weatherfilesave);
            weath.Province = xdoc.Descendants("Province").FirstOrDefault().Value;
            weath.Scene = xdoc.Descendants("Scene").FirstOrDefault().Value;
            weath.Geographic = xdoc.Descendants("Geographic").FirstOrDefault().Value;
            weath.Code = xdoc.Descendants("Code").FirstOrDefault().Value;
            weath.City = xdoc.Descendants("City").FirstOrDefault().Value;
            weath.Ultraviolet = xdoc.Descendants("Ultraviolet").FirstOrDefault().Value;         
            weath.Mangerweather = xdoc.Descendants("Baseweather").Select(x =>
               new Baseweather
               {
                   Temperature = x.Element("Temperature").Value,
                   TimeAndweather = x.Element("TimeAndweather").Value,
                   Gif = x.Element("Gif").Value,
                   Wind = x.Element("Wind").Value
               }).ToList();

            return View(weath);
        }
    }




    //public class WeatherWeb {
    //  public const string City = "湖北";   
    // /// <summary>
    // /// 获取指定的省份包含城市信息
    // /// </summary>
    //  public static void  GetSingleWeather()
    //  {
    //        WeatherWebServiceSoapClient web = new WeatherWebServiceSoapClient();
    //        string [] CityInfo =web.getSupportCity(City);

           
    //    }

    //    /// <summary>
    //    /// 获取指定的省份的城市(包含城市信息
    //    /// </summary>
    //    /// <param name="CityName">指定的洲或国内的省份，若为ALL或空则表示返回全部城市</param>
    //  public static void GetSingleWeather(string CityName)
    //  {
    //        WeatherWebServiceSoapClient web = new WeatherWebServiceSoapClient();
    //        web.getSupportCity(CityName);
    //   }
    //    /// <summary>
    //    /// 获取所有的省、直辖市、港澳台
    //    /// </summary>
    //    public static void GetAllProvince()
    //    {
    //        WeatherWebServiceSoapClient web = new WeatherWebServiceSoapClient();
    //        web.getSupportProvince();
    //    }
    //    /// <summary>
    //    /// 城市或地区名称查询获得未来三天内天气情况、现在的天气实况、天气和生活指数
    //    /// <param name="CityName">指定的城市或地区</param>
    //    /// </summary>
    //    public static void GetWeatherbyCityName(string CityName)
    //    {
    //        WeatherWebServiceSoapClient web = new WeatherWebServiceSoapClient();
    //        string[] CityWeatherInfo = web.getWeatherbyCityName(CityName);
    //    }
    //}
}