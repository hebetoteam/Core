using Microsoft.AspNetCore.SignalR;
using Nethereum.JsonRpc.Client;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using Core.Extensions;
using Core.Application.ViewModels.Hubs;

namespace Core.Web.Hubs
{
    public class BaseHub : Hub
    {
        public string CurrentUserName
        {
            get
            {
                if (Context.User.Identity.IsAuthenticated)
                {
                    var userNameClaim = Context.User.GetSpecificClaim("UserName");
                    return userNameClaim;
                }
                return "guess";
            }
        }

        public Guid CurrentUserId
        {
            get
            {
                if (Context.User.Identity.IsAuthenticated)
                {
                    var userIdClaim = Context.User.GetSpecificClaim("UserId");
                    if (!string.IsNullOrEmpty(userIdClaim))
                    {
                        return Guid.Parse(userIdClaim);
                    }
                }

                return Guid.Empty;
            }
        }

        public bool IsAdmin
        {
            get
            {
                bool isAdmin = false;
                if (Context.User.Identity.IsAuthenticated)
                {
                    var roleName = Context.User.GetSpecificClaim("RoleName");
                    isAdmin = roleName.ToLower().Contains("admin");
                }

                return isAdmin;
            }
        }

        public async Task SendErrorAsync(string description)
        {
            Console.WriteLine($"Warning: Send Client {description}");
            await Clients.Client(Context.ConnectionId).SendAsync("error", JsonConvert.SerializeObject(new BaseErrorModel
            {
                Error = $"{description}"
            }));
        }

        public async Task SendAllAsync(string method, object data)
        {
            await Clients.All.SendAsync(method, JsonConvert.SerializeObject(data));
        }

        public async Task SendClient(string method, object data)
        {

            await Clients.Client(Context.ConnectionId).SendAsync(method, JsonConvert.SerializeObject(data));
        }

        public async Task SendGlobalErrorAsync(string description, string exception)
        {
            Console.WriteLine($"Warning: Send All {description}");
            await Clients.All.SendAsync("error", $"{description} - {exception}");
        }


        public async Task AddGroupAsync(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task SendGroupAsync(string roomName, string method, object data)
        {
            await Clients.Group(roomName).SendAsync(method, JsonConvert.SerializeObject(data));
        }

        public async Task RemoveFromGroupAsync(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);


        }
    }
}
