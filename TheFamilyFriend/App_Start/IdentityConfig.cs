using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using TheFamilyFriend.Models;

namespace TheFamilyFriend
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 在此处插入电子邮件服务可发送电子邮件。
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 在此处插入 SMS 服务可发送短信。
            return Task.FromResult(0);
        }
    }

    // 配置此应用程序中使用的应用程序用户管理器。UserManager 在 ASP.NET Identity 中定义，并由此应用程序使用。
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // 配置用户名的验证逻辑
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // 配置密码的验证逻辑
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,//指定有效的密码最小长度

                RequireNonLetterOrDigit = false,//当为True时，有效的密码必须包含一个字符，它既不是数字也不是字母
                RequireDigit = false,//当为True时，有效密码必须包含数字
                RequireLowercase = false,//当为True时，有效密码必须包含一个小写字符
                RequireUppercase = false,//当为True时，有效密码必须包含一个大写字符
            };

            // 配置用户锁定默认值
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // 注册双重身份验证提供程序。此应用程序使用手机和电子邮件作为接收用于验证用户的代码的一个步骤
            // 你可以编写自己的提供程序并将其插入到此处。
            manager.RegisterTwoFactorProvider("电话代码", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "你的安全代码是 {0}"
            });
            manager.RegisterTwoFactorProvider("电子邮件代码", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "安全代码",
                BodyFormat = "你的安全代码是 {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // 配置要在此应用程序中使用的应用程序登录管理器。
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
    // 配置此应用程序中使用的应用程序角色管理器。RoleManager 在 ASP.NET Identity 中定义，并由此应用程序使用。
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
        : base(roleStore)
        { }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
        }
    }


    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext> //如果模型改变就删除原有数据库，重新创建一个数据库，同时，已有的数据将会丢失。
    {
        protected override void Seed(ApplicationDbContext context)   //建立种子方法，当模型改变需要重新建一个数据库的时候，用默认的数据填充数据库。
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(); //取得userManager ,或者使用new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            //创建角色列表
            var roles = new List<ApplicationRole>
                  {
                  new ApplicationRole { Name="admin",Description="系统超级管理员"},
                  new ApplicationRole { Name="user",Description="一般用户"},
                  };
            foreach (var role in roles)  //遍历列表，如果数据库中不存在列表中某个角色，就添加角色
            {
                var _role = roleManager.FindByName(role.Name);
                if (_role == null)
                {
                    var roleResult = roleManager.Create(role); //创建角色。
                }
            }
            base.Seed(context);   //运行父类的方法。
        }

    }


}
