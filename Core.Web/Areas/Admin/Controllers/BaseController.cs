using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Data.Entities;
using Core.Extensions;
using Core.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Core.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public virtual async Task<bool> VerifyCode(string authenticatorCode, UserManager<AppUser> userManager, AppUser appUser = null)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            appUser = appUser ?? await userManager.FindByIdAsync(userId);

            if (!appUser.TwoFactorEnabled) return true;

            return await userManager.VerifyTwoFactorTokenAsync(appUser, TokenOptions.DefaultAuthenticatorProvider, authenticatorCode);
        }

        protected string GetLoggedUserId()
        {
            return User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public string CurrentUserName
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userNameClaim = User.GetSpecificClaim("UserName");
                    return userNameClaim;
                }
                return "guess";
            }
        }

        public Guid CurrentUserId
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userIdClaim = User.GetSpecificClaim("UserId");
                    //userIdClaim = "0D8C0EE4-ED06-40B8-E951-08DAC9F33BE8";
                    if (!string.IsNullOrEmpty(userIdClaim))
                    {
                        return Guid.Parse(userIdClaim);
                    }
                }

                return Guid.Empty;
            }
        }

        public bool IsAdmin
        {
            get
            {
                bool isAdmin = false;
                if (User.Identity.IsAuthenticated)
                {
                    var roleName = User.GetSpecificClaim("RoleName");
                    isAdmin = roleName.ToLower().Contains("admin");
                }

                return isAdmin;
            }
        }

        public bool IsCustomer
        {
            get
            {
                bool isAdmin = false;
                if (User.Identity.IsAuthenticated)
                {
                    var roleName = User.GetSpecificClaim("RoleName");
                    isAdmin = roleName.ToLower().Contains("customer");
                }

                return isAdmin;
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

        public ActionResult RedirectHome()
        {
            return RedirectToAction("index", "home");
        }
    }
}
