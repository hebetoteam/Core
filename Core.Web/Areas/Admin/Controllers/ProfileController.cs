using Core.Application.Interfaces;
using Core.Application.ViewModels.System;
using Core.Areas.Admin.Controllers;
using Core.Data.Entities;
using Core.Extensions;
using Core.Utilities.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Util;
using System;
using System.Threading.Tasks;

namespace Core.Web.Areas.Admin.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly AddressUtil _addressUtil = new AddressUtil();
        public ProfileController(
            UserManager<AppUser> userManager,
            IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userModel = await _userService.GetById(CurrentUserId.ToString());

            userModel.ReferalLink = $"{Request.Scheme}://{Request.Host}/register?sponsor={userModel.Sponsor}";

            return View(userModel);
        }
    }
}
