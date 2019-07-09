using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TheFamilyFriend.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public Nullable<DateTime> CreateTime { get; set; }


        /// <summary>
        /// 头像
        /// </summary>       
        public string Avatar { get; set; }

        /// <summary>
        /// 主页皮肤
        /// </summary>       
        public string  Skip { get; set; }
    }

    //[NotMapped]
    //public class ApplicationRole : IdentityRole
    //{
    //    public ApplicationRole():base() {}
    //    public ApplicationRole(string roleName) : base(roleName) { }
    //    //[Display(Name = "角色描述")]
    //    //[StringLength(256, ErrorMessage = "{0}不能超过50个字符")]
    //    //public string RoleMark { get; set; }//角色表里新增加的字段
    //}




    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        //public new IDbSet<ApplicationRole> Roles { get; set; }//一定要重写这个方法，不然能用，网页中也能获取数据，就是代码里点不出来~~

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}