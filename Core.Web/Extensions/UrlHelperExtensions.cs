using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Areas.Admin.Controllers;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, Guid userId, string code, string scheme)
        {
            var url = urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Admin/Account",
                null,
                protocol: scheme);

            url = $"{Uri.UnescapeDataString(url)}?userId={userId}&code={Uri.EscapeDataString(code)}";

            return url;
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, Guid userId, string code, string scheme)
        {
            var url = urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Admin/Account",
                null,
                protocol: scheme);

            url = $"{Uri.UnescapeDataString(url)}?userId={userId}&code={Uri.EscapeDataString(code)}";

            return url;
        }
    }
}
