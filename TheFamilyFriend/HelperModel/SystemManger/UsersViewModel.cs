using System.ComponentModel.DataAnnotations;

namespace TheFamilyFriend.HelperModel.SystemManger
{
    /// <summary>
    /// 重置密码
    /// </summary>
    public class PasswordRrestModel
    {
        [Required]

        [Display(Name = "用户名")]
        public string RealName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
    /// <summary>
    /// 注册
    /// </summary>
    public class RegisterViewModel
    {
        [Display(Name = "电话(手机/固话)")]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }
    /// <summary>
    /// 上传头像
    /// </summary>
    public class UploadHeader
    {
        public string Id { get; set; }
        [Display(Name = "真实姓名")]
        public string RealName { get; set; }
        [Display(Name = "头像")]
        public virtual string Avatar { get; set; }
    }
}