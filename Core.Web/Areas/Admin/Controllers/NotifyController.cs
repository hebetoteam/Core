using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Core.Application.Interfaces;
using Core.Application.ViewModels.Blog;
using Core.Application.ViewModels.Common;
using Core.Application.ViewModels.Product;
using Core.Application.ViewModels.System;
using Core.Data.Enums;
using Core.Extensions;
using Core.Utilities.Constants;
using Core.Utilities.Dtos;
using Core.Utilities.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Core.Areas.Admin.Controllers
{
    public class NotifyController : BaseController
    {
        private readonly INotifyService _notifyService;

        public NotifyController(INotifyService notifyService)
        {
            _notifyService = notifyService;
        }

        public IActionResult Index()
        {
            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() != "admin")
                return Redirect("/logout");

            return View();
        }

        [HttpGet]
        public IActionResult AddEntity()
        {
            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() != "admin")
                return Redirect("/logout");

            return View();
        }

        [HttpGet]
        public IActionResult UpdateEntity(int id)
        {
            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() != "admin")
                return Redirect("/logout");

            var model = _notifyService.GetById(id);
            return View(model);
        }

        #region AJAX API

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _notifyService.GetAllPaging("", "", keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult SaveEntity(NotifyViewModel notifyVm)
        {
            try
            {
                var roleName = User.GetSpecificClaim("RoleName");
                if (roleName.ToLower() != "admin")
                    return Redirect("/logout");

                if (!ModelState.IsValid)
                    return new BadRequestObjectResult(ModelState.Values.SelectMany(v => v.Errors));
                else
                {
                    if (notifyVm.Id == 0)
                        _notifyService.Add(notifyVm);
                    else
                        _notifyService.Update(notifyVm);

                    _notifyService.Save();
                }

                return new OkObjectResult(new GenericResult(true));
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new GenericResult(true, ex.Message));
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() != "admin")
                return Redirect("/logout");

            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);
            else
            {
                _notifyService.Delete(id);
                _notifyService.Save();
                return new OkObjectResult(id);
            }
        }

        #endregion AJAX API
    }
}
