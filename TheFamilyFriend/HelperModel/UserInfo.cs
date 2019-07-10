using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using TheFamilyFriend.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TheFamilyFriend.HelperModel
{
    public class UserInfo
    {


        public static string [] GetUser(string id) {

            string skip =""; string Avatar = "~/Content/Images/Avatar/defult.png";
            
            var manage = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser= manage.FindById(id);
            if (currentUser != null)
            {
                if (!string.IsNullOrEmpty(currentUser.Skip))
                {
                    skip = currentUser.Skip;
                }
                if (!string.IsNullOrEmpty(currentUser.Avatar))
                {
                    Avatar = currentUser.Avatar;
                }
              
            }
            return new string[] { skip, Avatar };
        }
     }
}