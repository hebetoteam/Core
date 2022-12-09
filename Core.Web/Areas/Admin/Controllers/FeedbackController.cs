using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Core.Application.Interfaces;
using Core.Application.ViewModels.Blog;
using Core.Application.ViewModels.Common;
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
    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _feedbackService.GetAllPaging(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _feedbackService.GetById(id);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult ChangeType(int id)
        {
            var userFullName = User.GetSpecificClaim("FullName");

            _feedbackService.UpdateType(id, userFullName);
            _feedbackService.SaveChanges();

            return new OkObjectResult(true);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);
            else
            {
                _feedbackService.Delete(id);
                _feedbackService.SaveChanges();

                return new OkObjectResult(id);
            }
        }
    }
}
