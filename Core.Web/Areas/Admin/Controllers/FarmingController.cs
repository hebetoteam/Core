using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Application.Interfaces;
using Core.Application.ViewModels.BlockChain;
using Core.Application.ViewModels.System;
using Core.Application.ViewModels.Valuesshare;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Extensions;
using Core.Utilities.Constants;
using Core.Utilities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Newtonsoft.Json;

namespace Core.Areas.Admin.Controllers
{
    public class FarmingController : BaseController
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IBlockChainService _blockChainService;
        private readonly ITRONService _tronService;
        private readonly ILogger<FarmingController> _logger;
        private readonly AddressUtil _addressUtil = new AddressUtil();

        public FarmingController(
            ILogger<FarmingController> logger,
            ITRONService tronService,
            UserManager<AppUser> userManager,
            IUserService userService,
            IBlockChainService blockChainService)
        {
            _logger = logger;
            _tronService = tronService;
            _userManager = userManager;
            _blockChainService = blockChainService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
