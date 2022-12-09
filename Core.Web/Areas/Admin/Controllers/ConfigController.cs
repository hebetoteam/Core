using Core.Application.Implementation;
using Core.Application.Interfaces;
using Core.Application.ViewModels.System;
using Core.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Core.Web.Areas.Admin.Controllers
{
    public class ConfigController : BaseController
    {
        public readonly IConfigService _configService;
        private readonly ITokenPriceHistoryService _tokenPriceHistoryService;

        public ConfigController(
            IConfigService configService,
            ITokenPriceHistoryService tokenPriceHistoryService
            )
        {
            _tokenPriceHistoryService = tokenPriceHistoryService;
            _configService = configService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _configService.GetById(id);

            return new OkObjectResult(model);
        }


        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _configService.GetAllPaging(keyword,page, pageSize);

            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult SaveEntity(ConfigViewModel model)
        {

            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            _configService.Update(model);

            if (model.Name.Equals("TOKEN_PRICE"))
            {
                _tokenPriceHistoryService.Add(decimal.Parse(model.Value, CultureInfo.InvariantCulture));
            }

            return new OkObjectResult(model);
        }
    }
}
