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
using Core.Data.Enums;
using Core.Extensions;
using Core.Utilities.Constants;
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
    public class SupportManageController : BaseController
    {
        private readonly ISupportService _SupportService;

        public SupportManageController(ISupportService SupportService)
        {
            _SupportService = SupportService;
        }

        public IActionResult Index()
        {
            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() != "admin")
                return Redirect("/logout");

            return View();
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() != "admin")
                return Redirect("/logout");

            var model = _SupportService.GetAllPaging(keyword, "", page, pageSize);
            return new OkObjectResult(model);
        }

        #region AJAX API

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() != "admin")
                return Redirect("/logout");

            var model = _SupportService.GetById(id);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult SaveEntity(SupportViewModel supportVm)
        {
            var roleName = User.GetSpecificClaim("RoleName");
            if (roleName.ToLower() != "admin")
                return Redirect("/logout");

            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState.Values.SelectMany(v => v.Errors));
            else
            {
                _SupportService.Update(supportVm);
                _SupportService.Save();

                return new OkObjectResult(supportVm);
            }
        }

        #endregion AJAX API
    }
}
