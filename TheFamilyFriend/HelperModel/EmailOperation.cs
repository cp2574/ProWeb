using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TheFamilyFriend.Email
{
    /// <summary>
    /// 邮件操作
    /// </summary>
    public class EmailOperation
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <returns></returns>
        public static bool PushEmail(string str)
        {
            try
            {
                Email email = new Email();
                //读取配置
                var account = ConfigurationManager.AppSettings.Get("EmailAccount");
                if (account == null) email.mailFrom = "860312862@qq.com";
                var pwd = ConfigurationManager.AppSettings.Get("EmailPwd");
                if (pwd == null) email.mailPwd = "pgzychysuakobeef";
                var host = ConfigurationManager.AppSettings.Get("EmailHost");
                if (host == null) host = "smtp.qq.com"; 
                string mailToArrayStr=null;
                string[] mailToArray;
                if (mailToArrayStr == null)
                {
                    mailToArray = new string[] { "1784455078@qq.com" };
                }
                else
                {
                    mailToArray = mailToArrayStr.Split(';');
                }

                email.mailFrom = account;
                email.mailPwd = pwd;
                email.mailSubject = "问候";

                StringBuilder content = new StringBuilder();
                content.Append(str); //HTML拼接数据


                email.mailBody = content.ToString();//发送数据
                email.isbodyHtml = true;//指定格式
                email.host = host; //SMTP 服务器
                email.mailToArray = mailToArray;//收件人邮箱 
                bool ret = email.Send();//是否发送成功
                Console.WriteLine("PushEmail() 邮件是否发送成功：" + ret);
                return ret;
            }
            catch (Exception e)
            {
                Console.WriteLine("PushEmail() 邮件发送Error：" + e.Message + e.StackTrace);
                return false;
            }
        }
    }

    //Email发送类
    public class Email
    {
        #region 邮件
        /// <summary>
        /// 发送者
        /// </summary>
        public string mailFrom
        { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string[] mailToArray { get; set; }

        /// <summary>
        /// 抄送
        /// </summary>
        public string[] mailCcArray { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string mailSubject { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string mailBody { get; set; }

        /// <summary>
        /// 发件人密码
        /// </summary>
        public string mailPwd { get; set; }

        /// <summary>
        /// SMTP邮件服务器
        /// </summary>
        public string host { get; set; }

        /// <summary>
        /// 正文是否是html格式
        /// </summary>
        public bool isbodyHtml { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public string[] attachmentsPath { get; set; }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <returns></returns>
        public bool Send()
        {
            //使用指定的邮件地址初始化MailAddress实例
            MailAddress maddr = new MailAddress(mailFrom);
            //初始化MailMessage实例
            MailMessage myMail = new MailMessage();

            //向收件人地址集合添加邮件地址
            if (mailToArray != null)
            {
                for (int i = 0; i < mailToArray.Length; i++)
                {
                    myMail.To.Add(mailToArray[i].ToString());
                }
            }

            //向抄送收件人地址集合添加邮件地址
            if (mailCcArray != null)
            {
                for (int i = 0; i < mailCcArray.Length; i++)
                {
                    myMail.CC.Add(mailCcArray[i].ToString());
                }
            }

            //发件人地址
            myMail.From = maddr;

            //电子邮件的主题内容使用的编码
            myMail.SubjectEncoding = Encoding.Default;
            //电子邮件的标题
            myMail.Subject = mailSubject;
            //电子邮件正文的编码
            myMail.BodyEncoding = Encoding.Default;

            //电子邮件正文
            myMail.Body = mailBody;

            //电子邮件优先级
            myMail.Priority = MailPriority.High;

            //正文格式
            myMail.IsBodyHtml = isbodyHtml;

            //在有附件的情况下添加附件
            try
            {
                if (attachmentsPath != null && attachmentsPath.Length > 0)
                {
                    Attachment attachFile = null;
                    foreach (string path in attachmentsPath)
                    {
                        attachFile = new Attachment(path);
                        myMail.Attachments.Add(attachFile);
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception("在添加附件时有错误:" + err);
            }

            SmtpClient smtp = new SmtpClient();
            //指定发件人的邮件地址和密码以验证发件人身份
            smtp.Credentials = new System.Net.NetworkCredential(mailFrom, mailPwd); //设置SMTP邮件服务器
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Host = host;
            smtp.EnableSsl = true;
            //smtp.Port = 587;
            try
            {
                //将邮件发送到SMTP邮件服务器
                smtp.Send(myMail);
                return true;
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                return false;
            }
        }
        #endregion
    }
}
