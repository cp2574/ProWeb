using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.Models
{
    [Table("PictureType")]
    public partial class PictureType
    {
        [Key]
        public int PictureTypeId { get; set; }
         /// <summary>
         /// 类型名称
         /// </summary>
        public string PictureTypeName { get; set; }       
    }
}