using Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Extensions;
using System.Security.Claims;
using Core.Utilities.Constants;
using Core.Application.ViewModels.System;

namespace Core.Web.Controllers.Components
{
    public class AjaxNotifyViewComponent : ViewComponent
    {
        private INotifyService _notifyService;

        public AjaxNotifyViewComponent(INotifyService notifyService)
        {
            _notifyService = notifyService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var notify = _notifyService.GetDashboard();
            return View(notify);
        }
    }
}
