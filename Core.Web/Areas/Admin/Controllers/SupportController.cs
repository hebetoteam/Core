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
    public class SupportController : BaseController
    {
        private readonly ISupportService _SupportService;
        private readonly IBlogService _blogService;

        public SupportController(
            ISupportService SupportService,
            IBlogService blogService)
        {
            _blogService = blogService;
            _SupportService = SupportService;
        }

        [Route("support")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            string appUserId = User.GetSpecificClaim("UserId");
            var model = _SupportService.GetAllPaging(keyword, appUserId, page, pageSize);
            return new OkObjectResult(model);
        }

        #region AJAX API

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _SupportService.GetById(id);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult BlogDetail(int id)
        {
            var model = _blogService.GetById(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveEntity(SupportViewModel supportVm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new BadRequestObjectResult(ModelState.Values.SelectMany(v => v.Errors));
                else
                {
                    string appUserId = User.GetSpecificClaim("UserId");
                    supportVm.AppUserId = new Guid(appUserId);
                    _SupportService.Add(supportVm);
                    _SupportService.Save();
                }

                return new OkObjectResult(new GenericResult(true));
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new GenericResult(true, ex.Message));
            }
        }

        #endregion AJAX API
    }
}
