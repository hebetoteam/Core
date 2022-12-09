using Core.Application.Interfaces;
using Core.Application.ViewModels.Common;
using Core.Areas.Admin.Controllers;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Services;
using Core.Utilities.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Web.Areas.Admin.Controllers
{
    public class WalletTransactionController : BaseController
    {
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IBlockChainService _blockChainService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        public WalletTransactionController(
            IWalletTransactionService walletTransactionService,
            UserManager<AppUser> userManager,
            IEmailSender emailSender,
            IBlockChainService blockChainService
            )
        {
            _walletTransactionService = walletTransactionService;
            _userManager = userManager;
            _emailSender = emailSender;
            _blockChainService = blockChainService;
        }

        public IActionResult Index()
        {
            var enumTypes = ((WalletTransactionType[])Enum.
                GetValues(typeof(WalletTransactionType)))
                .Select(c => new EnumModel()
                {
                    Value = (int)c,
                    Name = c.GetDescription()
                }).ToList();

            ViewBag.WalletTransactionType = new SelectList(enumTypes, "Value", "Name");

            return View();
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int transactionId, int page, int pageSize)
        {
            var model = _walletTransactionService
                .GetAllPaging(keyword, IsAdmin ? null : CurrentUserId, page, pageSize, transactionId);
            
            return new OkObjectResult(model);
        }
    }
}
