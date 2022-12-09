using Core.Application.ViewModels.Blog;
using Core.Application.ViewModels.Common;
using Core.Application.ViewModels.Valuesshare;
using Core.Data.Enums;
using System;
using System.Collections.Generic;

namespace Core.Application.ViewModels.System
{
    public class StatementUserViewModel
    {
        public StatementUserViewModel()
        {
            AppUsers = new List<AppUserViewModel>();
        }

        public int TotalMember { get; set; }

        public decimal TotalBNBBalance { get; set; }

        public decimal TotalUSDTBalance { get; set; }

        public List<AppUserViewModel> AppUsers { set; get; }
    }
}
