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
using System.Data.Entity.Migrations;

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
        //[Required]
        //[StringLength(20, ErrorMessage = "{0}少于{2}个字符", MinimumLength = 2)]
        [Display(Name = "姓名")]
        public string RealName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public virtual Nullable<DateTime> CreateTime { get; set; }
        /// <summary>
        /// 头像
        /// </summary>     
         [StringLength(200)]
        [Display(Name = "头像")]
        public virtual  string Avatar { get; set; }
        /// <summary>
        /// 主页皮肤
        /// </summary>     
        [StringLength(10)]
        [Display(Name = "主题")]
        public virtual string  Skip { get; set; }
        [StringLength(50)]
        [Display(Name = "微信")]
        public virtual string WX { get; set; }
        [StringLength(15)]
        public virtual string QQ { get; set; }
        [Display(Name = "性别")]
        public virtual int? Gender { get; set; }
        [StringLength(200)]
        public virtual string Address { get; set; }
        
        [Display(Name = "出生日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual Nullable<DateTime> Birthday { get; set; }
        


    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }


        [Display(Name = "角色描述")]
        [StringLength(50, ErrorMessage = "{0}不能超过50个字符")]
        public string Description { get; set; }

    }



    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


       // 静态构造函数。MSDN：静态构造函数用于初始化任何静态数据，或用于执行仅需执行一次的特定操作。在创建第一个实例或引用任何静态成员之前，将自动调用静态构造函数。   当程序部署在服务器上时，当第一次登陆的时候，就执行这个初始化器，并填充数据库。
        static ApplicationDbContext()  //静态构造函数不需要有public 或private  修饰符。
        {
            //设置数据库初始化器，它就在应用程序运行的时候加载。
            //在初始化器中需要建立一个管理员角色和一个具有管理员角色的账户。
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());  //在System.Data.Entity 命名空间下面。

          }


        }
}