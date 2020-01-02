using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecretCollect.Localization.SqlLocalizer.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SecretCollect.Localization.Web.Controllers
{
    public class CulturesController : Controller
    {
        private readonly LocalizationContext _context;

        public CulturesController(LocalizationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cultures = (await _context.SupportedCultures.Select(x => new { x.Id, x.Name }).ToArrayAsync()).Select(x => (x.Id, x.Name)).ToArray();

            return View(cultures);
        }
    }
}
