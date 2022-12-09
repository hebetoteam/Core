using Core.Application.Interfaces;
using Core.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Core.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public HomeController(
            )
        {
        }

        [Route("home")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
