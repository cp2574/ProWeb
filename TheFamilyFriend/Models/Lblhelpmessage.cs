using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.Models
{
    [Table("Lblhelpmessage")]
    public class Lblhelpmessage
    {
        [Key]
        public int LblId { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
     
        public string  Title { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        
        public string Explain { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime  SignTime { get; set; }      
        /// <summary>
        /// 用户
        /// </summary>       
        public string UserName { get; set; }
    }
}