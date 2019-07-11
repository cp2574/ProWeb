using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.Models
{
    public class RoleMenu
    {
        public int Id { get; set; }

        [Display(Name = "角色ID")]
        public string RoleId { get; set; }
        [Display(Name = "菜单Id")]
        public int MenuId { get; set; }

        ///（1为可用，2为不可用）
        [Display(Name = "是否可用")]
        public int IsAvailable { get; set; }

    }
}