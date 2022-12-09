using Core.Application.Interfaces;
using Core.Application.ViewModels.BlockChain;
using Core.Application.ViewModels.System;
using Core.Application.ViewModels.Transfer;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Utilities.Constants;
using Core.Utilities.Dtos;
using Core.Utilities.Extensions;
using Core.Utilities.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Areas.Admin.Controllers
{
    public class AirdropController : BaseController
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AirdropController> _logger;
        private readonly IUserService _userService;
        private readonly IAirdropService _airdropService;
        public AirdropController(
            ILogger<AirdropController> logger,
            UserManager<AppUser> userManager,
            IUserService userService,
            IAirdropService airdropService
            )
        {
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
            _airdropService = airdropService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAirdrop([FromBody] AirdropViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.UserTelegramCommunity)
                    && string.IsNullOrWhiteSpace(model.UserTelegramChannel)
                    && string.IsNullOrWhiteSpace(model.UserFacebook))
                {
                    return new OkObjectResult(new GenericResult(false, "Missions cannot be left blank"));
                }


                var appUser = await _userManager.FindByIdAsync(CurrentUserId.ToString());
                if (appUser == null)
                    return new OkObjectResult(new GenericResult(false, "Account does not exist"));


                var isExistAirdrop = _airdropService.IsExistAirdrop(appUser.Id);
                if (isExistAirdrop)
                    return new OkObjectResult(new GenericResult(false, "End of quest confirmation"));


                model.AppUserId = appUser.Id;
                model.Status = AirdropStatus.Pending;

                _airdropService.Add(model);

                _airdropService.Save();

                return new OkObjectResult(new GenericResult(true, "Confirm airdrop is successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError("ConfirmAirdrop: {0}", ex.Message);

                return new OkObjectResult(new GenericResult(false, ex.Message));
            }
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _airdropService.GetAllPaging(keyword, CurrentUserName, page, pageSize);

            return new OkObjectResult(model);
        }
    }
}
