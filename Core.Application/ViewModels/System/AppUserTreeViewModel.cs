using Core.Data.Enums;
using System;
using System.Collections.Generic;

namespace Core.Application.ViewModels.System
{
    public class AppUserTreeViewModel
    {
        public AppUserTreeViewModel()
        {
            children = new List<AppUserTreeViewModel>();
        }

        public string id { get; set; }

        public Guid userid { get; set; }

        public string text { get; set; }
        public string icon { get; set; }
        public AppUserTreeData data { get; set; }
        public AppUserTreeState state { get; set; }

        public List<AppUserTreeViewModel> children { get; set; }
    }

    public class AppUserTreeState
    {
        public bool opened { get; set; } = true;
    }

    public class AppUserTreeData
    {
        public Guid rootId { get; set; }

        public string sponsor { get; set; }

        public Guid? referralId { get; set; }

        public decimal investBalance { get; set; }

        public decimal lockBalance { get; set; }

        public decimal affiliateBalance { get; set; }

        public decimal stakingBalance { get; set; }

        public string email { set; get; }

        public bool isSystem { get; set; } = false;

        public bool emailConfirmed { get; set; }

        public DateTime dateCreated { get; set; }

        public Status status { get; set; }

        public decimal InvestBotBalance { get; set; }
    }

    public class AppUserTreeViewModelAjax
    {
        public AppUserTreeViewModelAjax()
        {

        }

        public Guid id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }

        public bool children { get; set; }

        public string type { get; set; }

        public int rank { get; set; }


    }

    public class TreeViewUserViewModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public decimal TotalInvestChild { get; set; }

        public List<AppUserTreeViewModel> TreeView { get; set; }
    }
}
