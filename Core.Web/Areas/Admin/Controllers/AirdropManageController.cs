using Core.Application.Interfaces;
using Core.Application.ViewModels.BlockChain;
using Core.Application.ViewModels.System;
using Core.Application.ViewModels.Transfer;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Extensions;
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
    public class AirdropManageController : BaseController
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AirdropManageController> _logger;
        private readonly IUserService _userService;
        private readonly IAirdropService _airdropService;
        public AirdropManageController(
            ILogger<AirdropManageController> logger,
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
            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() != "admin")
                return Redirect("/logout");

            return View();
        }


        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _airdropService.GetAllPaging(keyword,"", page, pageSize);

            return new OkObjectResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() != "admin")
                return Redirect("/logout");

            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);
            else
            {
                try
                {
                    var result = await _airdropService.Approved(id);

                    return new OkObjectResult(new GenericResult(result.Success, result.Message));
                }
                catch (Exception ex)
                {
                    return new OkObjectResult(new GenericResult(false, ex.Message));
                }
            }
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() != "admin")
                return Redirect("/logout");

            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);
            else
            {
                try
                {
                    _airdropService.Rejected(id);
                    return new OkObjectResult(new GenericResult(true, "Reject Airdrop is success"));
                }
                catch (Exception ex)
                {
                    return new OkObjectResult(new GenericResult(false, ex.Message));
                }
            }
        }
    }
}
