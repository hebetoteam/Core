using Core.Application.ViewModels;
using Core.Application.ViewModels.Blog;
using Core.Application.ViewModels.Common;
using Core.Application.ViewModels.Product;
using Core.Application.ViewModels.System;
using System.Collections.Generic;

namespace Core.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            HomeBlogs = new List<BlogViewModel>();
        }

        public List<BlogViewModel> HomeBlogs { get; set; }
        public NotifyViewModel Notify { get; set; }
    }
}
