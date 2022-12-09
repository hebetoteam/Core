using Core.Data.Entities;
using Core.Utilities.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Core.Application.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AntiForgeryAddress : Attribute, IAuthorizationFilter
    {
        public const string ConnectedAddress = "ConnectedAddress";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var connectedAddress = context.HttpContext.Session.Get<string>(ConnectedAddress);

            var result = context.HttpContext.Request.Headers.TryGetValue(ConnectedAddress, out var requestedAddress);

            if (!result)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }

            if (connectedAddress != requestedAddress)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
        }
    }
}
