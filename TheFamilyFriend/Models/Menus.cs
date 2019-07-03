using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.Models
{
    [Table("Menus")]
    public class Menus
    {
        [Key]
        [DisplayName("主键")]
        public int Id { get; set; }
        [DisplayName("名称")]
        public string Name { get; set; }
        [DisplayName("控制器")]
        public string Controller { get; set; }
        [DisplayName("方法")]
        public string Action { get; set; }
        [DisplayName("级别")]
        public Nullable<int>Level { get; set; }
        [DisplayName("创建时间")]
        public Nullable<System.DateTime> CreateTime { get; set; }
        [DisplayName("图标")]
        public string Icon { get; set; }
        [Display]
        [DisplayName("父级")]
        public Nullable<int> parentID { get; set; }
    }
}