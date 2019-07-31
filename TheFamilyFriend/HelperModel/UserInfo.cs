using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using TheFamilyFriend.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.IO;
using System;
using System.Drawing;

namespace TheFamilyFriend.HelperModel
{
    public class UserInfo
    {

        public static string  strFileSavePath = ConfigurationManager.AppSettings.Get("HeadPortrait").ToString();
        public static string [] GetUser(string id) {
            
            string skip =""; string Avatar = null;
            var manage = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser= manage.FindById(id);
            if (currentUser != null)
            {
                if (!string.IsNullOrEmpty(currentUser.Skip))
                {
                    skip = currentUser.Skip;
                }
                if (!string.IsNullOrEmpty(currentUser.Avatar))
                {
                    string url = ImgToBase64String(Path.Combine(strFileSavePath, currentUser.Avatar));

                    if (url != null)
                    {
                        Avatar = url;
                    }
                }
              
            }
            return new string[] { skip, Avatar };
        }






        /// <summary>
        /// 图片转为base64编码的字符串
        /// </summary>
        /// <param name="Imagefilename"></param>
        /// <returns></returns>
        public static string ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null; 
            }
        }

        /// <summary>
        /// base64编码的字符串转为图片
        /// </summary>
        /// <param name="strbase64"></param>
        /// <returns></returns>
        public Bitmap Base64StringToImage(string strbase64)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(strbase64);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);

                //bmp.Save("test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmp.Save("test.bmp", ImageFormat.Bmp);
                //bmp.Save("test.gif", ImageFormat.Gif);
                //bmp.Save("test.png", ImageFormat.Png);
                ms.Close();
                return bmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}