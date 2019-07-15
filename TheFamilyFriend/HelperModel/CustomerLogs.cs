using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheFamilyFriend.Models;

namespace TheFamilyFriend.HelperModel
{
    public class CustomerLogs
    {
        /// <summary>
        /// 获取登录日志
        /// </summary>
        /// <param name="model">登录模型</param>
        /// <param name="result">结果</param>
        /// <param name="url">请求路径</param>
        internal static void LoginLogs(LoginViewModel model, Result result,string url)
        {
            using (var kbd = new KinshipDb())
            {
                //登录日志
                kbd.Logs.Add(new TLogs()
                {
                    Account = model.UserName,
                    code = model.UserName,
                    IP = Host.IPAddress,
                    Url = url,
                    Result = result.message,
                    Type = logtype.登陆日志,
                    date = DateTime.Now
                });
                kbd.SaveChanges();
            }
        }
        /// <summary>
        /// 获取报错日志
        /// </summary>
        /// <param name="code"></param>
        /// <param name="result"></param>
        internal static void ErrorLogs(string name, string code, Result result,string url)
        {
            using (var kbd = new KinshipDb())
            {
                //登录日志
                kbd.Logs.Add(new TLogs()
                {
                    Account = name,
                    code = code,
                    IP = Host.IPAddress,
                    Url = url,
                    Result = result.message,
                    Type = logtype.报错日志,
                    date = DateTime.Now
                });
                kbd.SaveChanges();
            }

        }
    }

}