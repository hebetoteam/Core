using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Core.Web.Controllers.Components
{
    public class AjaxPagingViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
