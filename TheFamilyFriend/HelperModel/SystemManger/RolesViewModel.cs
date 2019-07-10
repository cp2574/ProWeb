using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.HelperModel.SystemManger
{
    public class RolesViewModel
    {
        public string Id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "角色名")]
        public string Name { get; set; }
        [Display(Name = "描述")]
        public string Description { get; set; }
    }
    public class AddRoleViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "角色名称")]
        public string Name { get; set; }
         [Display(Name = "描述")]
        public string Description { get; set; }
    }
}