using Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.System
{
    public class AirdropViewModel
    {
        public int Id { get; set; }

        public string UserTelegramChannel { get; set; }

        public string UserTelegramCommunity { get; set; }

        public string UserFacebook { get; set; }


        public AirdropStatus Status { get; set; }
        public string StatusName { get; set; }


        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public Guid AppUserId { get; set; }
        public string AppUserName { get; set; }
        public string Sponsor { get; set; }


        public  AppUserViewModel AppUser { set; get; }
    }
}
