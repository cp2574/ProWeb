using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.Models
{
    [Table("Personage")]
    public partial class Personage
    {
        [Key]
        public int PersonId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [StringLength(50)]
        public string  Name { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(50)]
        public string Nickname { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [StringLength(10)]
        public string Gender { get; set; }
        public int? Age { get; set; }
        public int ?Height { get; set; }
        public int? Weight { get; set; }
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(100)]        
        public string HeadPortrait { get; set; }
        /// <summary>
        /// 所在地
        /// </summary>
        [StringLength(200)]       
        public string Premise { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [StringLength(50)]
        public string Email { get; set; }
        /// <summary>
        /// 个人说明
        /// </summary>
           
        [StringLength(400)]
        public string Description { get; set; }

        /// <summary>
        /// 职业
        /// </summary>
        [StringLength(50)]
        public string Profession { get; set; }        
        /// <summary>
        /// 签名
        /// </summary>
        [StringLength(100)]
        public string Signature { get; set; }
        /// <summary>
        /// 爱好
        /// </summary>

        [StringLength(400)]
        public string Hobby { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        [StringLength(50)]
        public string Company { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        [StringLength(400)]
        public string Tag { get; set; }
        /// <summary>
        /// 家乡
        /// </summary>
        [StringLength(50)]
        public string Hometown{ get; set; }
        /// <summary>
        /// 婚否
        /// </summary>
        [StringLength(10)]
        public string Ismarried   { get; set; }
        /// <summary>
        /// 血型
        /// </summary>
        [StringLength(10)]
        public string Blood { get; set; }
        /// <summary>
        /// 星座
        /// </summary>
        [StringLength(10)]
        public string Constellation { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        [StringLength(12)]
        public string Phone { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [StringLength(12)]
        public string UserName { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        [StringLength(12)]
        public string Type { get; set; }
        /// <summary>
        /// 点赞
        /// </summary>
        public int LikeGood { get; set; }      
    }
}