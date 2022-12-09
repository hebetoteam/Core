using Core.Application.Interfaces;
using Core.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Web.Hubs
{

    public class LuckyHub<TUser, TRole> : BaseHub
        where TUser : IdentityUser
        where TRole : IdentityRole
    {
        private readonly ILogger<LuckyHub<TUser, TRole>> _logger;
        private readonly IUserService _userService;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;


        private static DateTime StartTime = DateTime.UtcNow;// time game star


        private static bool isInit = false;

        public LuckyHub(IUserService userService,
            RoleManager<AppRole> roleManager,
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            ILogger<LuckyHub<TUser, TRole>> logger)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            await Init();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            await base.OnDisconnectedAsync(exception);
        }

        public async Task Init()
        {
            if (isInit)
                return;


            _logger.LogInformation("Start Init Lucky Info");

            isInit = true;


            await RunGame();
        }


        public async Task RunGame()
        {
            await SendAllAsync("luckyMembers",null);

            Thread.Sleep(2000);

            await RunGame();
        }

        void dbTimeout()
        {
            Thread.Sleep(1000);
        }

        public async Task OnUserConnected()
        {
            if (isInit)
            {


                //await SendClient("join", gameInfo);

            }

        }

        public Task Send(string message)
        {
            return SendAllAsync("Send", message);
        }
    }
}
