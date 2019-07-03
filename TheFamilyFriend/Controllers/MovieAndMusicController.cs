using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace TheFamilyFriend.Controllers
{
    public class MovieAndMusicController : Controller
    {
        // GET: MovieAndMusic
       public static  List<Music> Musiclist = new List<Music>();
        public ActionResult Index()
        {          
            string musicpath = Server.MapPath("/Music/歌曲清单.xml");
            if (System.IO.File.Exists(musicpath))
            {
                XDocument xdoc = XDocument.Load(musicpath);
                Musiclist = xdoc.Descendants("song").Select(x => 
                new Music { Id = Convert.ToInt32(x.Attribute("Id").Value), Path = x.Value ,Name= x.Value.Substring(x.Value.LastIndexOf('/')+1, x.Value.LastIndexOf(".")- x.Value.LastIndexOf('/')-1)}).ToList();
            }
            return View(Musiclist);
        }
        /// <summary>
        /// 切换歌曲
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public JsonResult Cut(int Id,string option)
        {
            Music music=null;
            string Message="";
            switch (option)
            {
                case "up":
                    music = Musiclist.OrderByDescending(x=>x.Id).FirstOrDefault(x => x.Id < Id);
                    Message = "亲，已经到顶咯";
                    break;
                case "down":
                    music = Musiclist.FirstOrDefault(x => x.Id > Id);
                    Message = "亲，已经到底咯";
                    break;
                case "now":
                    music = Musiclist.Find(x=>x.Id== Id);
                    Message = "亲，已经很努力加载咯";
                    break;
            }       
            if (music != null)
            {
                return Json(new {ReturnValue=true, Music=music } );
            }
            else
            {               
                return Json(new { ReturnValue = false,Message= Message });
            }          
        }

        public ActionResult MicroFilm() {


            return View();
        }

    }
    public class Music {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}