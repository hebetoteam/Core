using System.Linq;
using System.Threading.Tasks;
using Core.Application.Interfaces;
using Core.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Core.Areas.Admin.Controllers
{
    public class NetworkController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IReportService _reportService;
        private readonly UserManager<AppUser> _userManager;


        public NetworkController(
            UserManager<AppUser> userManager,
            IUserService userService,
            IReportService reportService
            )
        {
            _userManager = userManager;
            _userService = userService;
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var userModel = await _userService.GetNetworkInfo(CurrentUserId.ToString());
            userModel.ReferalLink = $"{Request.Scheme}://{Request.Host}/register?sponsor={userModel.Sponsor}";
            return View(userModel);
        }

        public IActionResult GetMemberTreeNode(string parent)
        {
            if (string.IsNullOrEmpty(parent) || parent.Equals("#"))
            {
                var userId = CurrentUserId.ToString();
                parent = userId;
            }

            var model = _userService.GetMemberTreeAll(parent);
            return new ObjectResult(model);
        }

        public async Task<IActionResult> GetNetworkSummaryInfo()
        {
            var f1s = _reportService.GetAllBelowRef(CurrentUserId);

            var appUser = await _userManager.FindByIdAsync(CurrentUserId.ToString());

            return new ObjectResult(new
            {
                TotalNetwork = f1s.Count,
                TotalStaking = appUser.StakingAmount,
                TotalStakingAffiliate = appUser.StakingAffiliateAmount
            });
        }
    }
}


