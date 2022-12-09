using Core.Application.Interfaces;
using Core.Application.ViewModels.BlockChain;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Utilities.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Core.Areas.Admin.Controllers
{
    public class RequestStatusController : BaseController
    {
        private readonly ITicketTransactionService _ticketTransactionService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RequestStatusController> _logger;
        private readonly IUserService _userService;
        public RequestStatusController(
            ILogger<RequestStatusController> logger,
            UserManager<AppUser> userManager,
            IUserService userService,
            ITicketTransactionService ticketTransactionService
            )
        {
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
            _ticketTransactionService = ticketTransactionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _ticketTransactionService
                .GetAllPaging(keyword, CurrentUserName, page, pageSize);

            return new OkObjectResult(model);
        }
    }
}
