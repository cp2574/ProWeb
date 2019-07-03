using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using TheFamilyFriend.cn.com.webxml.www;

namespace TheFamilyFriend.HelperModel
{
    public class WeatherWeb
    {
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 实况
        /// </summary>
        public string Scene { get; set; }
        /// <summary>
        /// 紫外线
        /// </summary>
        public string Ultraviolet { get; set; }
        /// <summary>
        /// 地理位置
        /// </summary>
        public string Geographic { get; set; }

        /// <summary>
        /// 基本天气情况
        /// </summary>
        public List<Baseweather> Mangerweather { get; set; }


        public static void CreateXML(string filesave) {
            WeatherWebService web = new WeatherWebService();
            string[] CityWeatherInfo = web.getWeatherbyCityName("武汉");
            XDocument Createxdoc = new XDocument();
            XElement root = new XElement("Weather",
                    new XElement("Province", CityWeatherInfo[0]),
                    new XElement("City", CityWeatherInfo[1]),
                    new XElement("Code", CityWeatherInfo[2]),
                    new XElement("Scene", CityWeatherInfo[10]),
                    new XElement("Ultraviolet", CityWeatherInfo[11]),
                    new XElement("Geographic", CityWeatherInfo[22]));
            root.Add(new XElement("Baseweather",
                   new XElement("Temperature", CityWeatherInfo[5]),
                   new XElement("TimeAndweather", CityWeatherInfo[6]),
                   new XElement("Wind", CityWeatherInfo[7]),
                   new XElement("Gif", CityWeatherInfo[8])));
            root.Add(new XElement("Baseweather",
            new XElement("Temperature", CityWeatherInfo[12]),
            new XElement("TimeAndweather", CityWeatherInfo[13]),
            new XElement("Wind", CityWeatherInfo[14]),
            new XElement("Gif", CityWeatherInfo[15])));
            root.Add(new XElement("Baseweather",
            new XElement("Temperature", CityWeatherInfo[17]),
            new XElement("TimeAndweather", CityWeatherInfo[18]),
            new XElement("Wind", CityWeatherInfo[19]),
            new XElement("Gif", CityWeatherInfo[20])));
            Createxdoc.Add(root); Createxdoc.Save(filesave);



        }

    }
    public class Baseweather
    {

        /// <summary>
        /// 气温
        /// </summary>
        public string Temperature { get; set; }
        /// <summary>
        /// 时间和天气
        /// </summary>
        public string TimeAndweather { get; set; }
        /// <summary>
        /// 风向
        /// </summary>
        public string Wind { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Gif { get; set; }
    }

}