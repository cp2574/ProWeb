using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.Models
{
    [Table("MenuInfo")]
    public class MenuInfo
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "菜单名称")]
        public string MenuName { get; set; }
        [Display(Name = "菜单地址")]
        public string MenuPath { get; set; }

        [Display(Name = "菜单图标")]
        public string MenuIcon { get; set; }
        /// <summary>
        /// 方法
        /// </summary>
        [Display(Name = "方法名")]
        public string MethodName { get; set; }

        /// <summary>
        /// 控制器
        /// </summary>
        [Display(Name = "控制器名")]
        public string ControllerName { get; set; }


        /// <summary>
        /// 为0则为父级菜单一级菜单，否则这里刚是他选择的上级菜单的ID值
        /// </summary>
        [Display(Name = "是否一级")]
        public int Popedomfatherid { get; set; }
        [Display(Name = "排序")]
        public int Sort { get; set; }


    }
}