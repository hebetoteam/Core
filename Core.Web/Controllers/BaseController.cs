using Core.Data.Entities;
using Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public virtual async Task<bool> VerifyCode(
            string authenticatorCode, UserManager<AppUser> userManager, AppUser appUser = null)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            appUser = appUser ?? await userManager.FindByIdAsync(userId);

            if (!appUser.TwoFactorEnabled) return true;

            return await userManager.VerifyTwoFactorTokenAsync(
                appUser, TokenOptions.DefaultAuthenticatorProvider, authenticatorCode);
        }

        public string CurrentUserId
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    //return "9231ffc0-53bf-43e2-22b7-08da7aadb8a7";
                    return User.GetSpecificClaim("UserId");
                }
                return null;

            }
        }

        public string CurrentUserName
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    //return "9231ffc0-53bf-43e2-22b7-08da7aadb8a7";
                    return User.Identity.Name;
                }
                return null;

            }
        }

        public bool IsLeader
        {
            get
            {
                bool isAdmin = false;
                if (User.Identity.IsAuthenticated)
                {
                    var roleName = User.GetSpecificClaim("RoleName");
                    isAdmin = roleName.ToLower().Contains("leader");
                }

                return isAdmin;
            }
        }

        public string UserName
        {

            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userNameClaim = User.GetSpecificClaim("UserName");
                    return userNameClaim;
                }

                return null;
            }

        }
    }
}
