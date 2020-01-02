using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SecretCollect.Localization.Web.Models;
using System.Diagnostics;

namespace SecretCollect.Localization.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(IStringLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
