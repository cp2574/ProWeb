using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.Models
{
    public class RolesViewModel
    {
        public string Id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "角色名")]
        public string Name { get; set; }


    }
    public class AddRoleViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "角色名称")]
        public string Name { get; set; }
    }
}