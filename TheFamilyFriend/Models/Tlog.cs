using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheFamilyFriend.Models
{
    [Table("TLogs")]
    public class TLogs
    {
        [Key]
        [DisplayName("主键")]
        public int Id { get; set; }
        [DisplayName("登录名")]
        public string Account { get; set; }

        [DisplayName("相关值")]
        public string code { get; set; }

        [DisplayName("登陆IP")]
        public string IP { get; set; }

        [DisplayName("操作网址")]
        public string Url { get; set; }

        [DisplayName("结果")]
        public string Result { get; set; }

        [DisplayName("备注")]
        public string Mark { get; set; }

        [DisplayName("操作时间")]
        public DateTime date { get; set; }

        [DisplayName("日志类型")]
        public logtype Type { get; set; }
    }

   public enum logtype{
        登陆日志,
        报错日志,
        操作日志 }
}
