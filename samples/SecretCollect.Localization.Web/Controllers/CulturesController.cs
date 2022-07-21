using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SecretCollect.Localization.SqlLocalizer.Data;
using SecretCollect.Localization.Web.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SecretCollect.Localization.Web.Controllers
{
    public class CulturesController : Controller
    {
        private readonly LocalizationContext _context;
        private readonly IStringLocalizer<CulturesController> _localizer;

        public CulturesController(LocalizationContext context, IStringLocalizer<CulturesController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var cultures = (await _context.SupportedCultures.Select(x => new { x.Id, x.Name }).ToArrayAsync()).Select(x => (x.Id, x.Name)).ToArray();

            return View(cultures);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new CultureVM
            {
                OtherCultures =
                (
                    await _context.SupportedCultures
                        .Select(x => new { x.Id, x.Name })
                        .ToArrayAsync()
                )
                .Select(x => (x.Id, x.Name))
                .ToArray()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(CultureVM model)
        {
            if (!ModelState.IsValid)
            {
                model.OtherCultures =
                    (
                        await _context.SupportedCultures
                            .Select(x => new { x.Id, x.Name })
                            .ToArrayAsync()
                    )
                    .Select(x => (x.Id, x.Name))
                    .ToArray();
                return View(model);
            }

            var (isValid, name) = _validateCultureName(model.Name);
            if (!isValid)
            {
                ModelState.AddModelError(nameof(model.Name), _localizer["NAME_IS_NOT_A_VALID_CULTURE_NAME"]);
                model.OtherCultures =
                    (
                        await _context.SupportedCultures
                            .Select(x => new { x.Id, x.Name })
                            .ToArrayAsync()
                    )
                    .Select(x => (x.Id, x.Name))
                    .ToArray();
                return View(model);
            }

            try
            {
                var culture = new SupportedCulture()
                {
                    Name = model.Name,
                    IsSupported = model.IsSupported
                };

                _context.Add(culture);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Edit), new { id = culture.Id, saveResult = SaveResult.Success });
            }
            catch (DbUpdateException ex) when (( ex.InnerException as Microsoft.Data.SqlClient.SqlException )?.Number == 2601)
            {
                _context.ChangeTracker.Entries().ToList().ForEach(e => e.State = EntityState.Detached);
                model.SaveResult = SaveResult.Duplicate;
                model.OtherCultures =
                    (
                        await _context.SupportedCultures
                            .Select(x => new { x.Id, x.Name })
                            .ToArrayAsync()
                    )
                    .Select(x => (x.Id, x.Name))
                    .ToArray();
                return View(model);
            }
            catch (Exception)
            {
                model.SaveResult = SaveResult.UnknownFailure;
                model.OtherCultures =
                (
                    await _context.SupportedCultures
                        .Select(x => new { x.Id, x.Name })
                        .ToArrayAsync()
                )
                .Select(x => (x.Id, x.Name))
                .ToArray();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id, SaveResult saveResult = SaveResult.None)
        {
            if (id == null)
                return NotFound();

            var model = await _context.SupportedCultures
                .Where(x => x.Id == id)
                .Select(x => new CultureVM()
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsSupported = x.IsSupported,
                    FallbackCultureId = x.FallbackCultureId
                })
                .SingleOrDefaultAsync();

            if (model == null)
                return NotFound();

            model.SaveResult = saveResult;
            model.OtherCultures =
                (
                    await _context.SupportedCultures
                        .Select(x => new { x.Id, x.Name })
                        .ToArrayAsync()
                )
                .Select(x => (x.Id, x.Name))
                .Where(x => x.Id != id.Value)
                .ToArray();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid? id, CultureVM model)
        {
            if (id == null)
                return NotFound();

            if (!ModelState.IsValid || model.Id != id)
            {
                model.OtherCultures =
                    (
                        await _context.SupportedCultures
                            .Select(x => new { x.Id, x.Name })
                            .ToArrayAsync()
                    )
                    .Select(x => (x.Id, x.Name))
                    .Where(x => x.Id != id.Value)
                    .ToArray();
                return View(model);
            }

            var (isValid, name) = _validateCultureName(model.Name);
            if (!isValid)
            {
                ModelState.AddModelError(nameof(model.Name), _localizer["NAME_IS_NOT_A_VALID_CULTURE_NAME"]);
                model.OtherCultures =
                    (
                        await _context.SupportedCultures
                            .Select(x => new { x.Id, x.Name })
                            .ToArrayAsync()
                    )
                    .Select(x => (x.Id, x.Name))
                    .Where(x => x.Id != id.Value)
                    .ToArray();
                return View(model);
            }

            try
            {
                var culture = new SupportedCulture()
                {
                    Id = model.Id,
                    Name = model.Name,
                    IsSupported = model.IsSupported,
                    FallbackCultureId = model.FallbackCultureId
                };

                _context.Update(culture);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Edit), new { id = culture.Id, saveResult = SaveResult.Success });
            }
            catch (DbUpdateException ex) when (( ex.InnerException as Microsoft.Data.SqlClient.SqlException )?.Number == 2601)
            {
                _context.ChangeTracker.Entries().ToList().ForEach(e => e.State = EntityState.Detached);
                model.SaveResult = SaveResult.Duplicate;
                model.OtherCultures =
                    (
                        await _context.SupportedCultures
                            .Select(x => new { x.Id, x.Name })
                            .ToArrayAsync()
                    )
                    .Select(x => (x.Id, x.Name))
                    .Where(x => x.Id != id.Value)
                    .ToArray();
                return View(model);
            }
            catch (Exception)
            {
                model.SaveResult = SaveResult.UnknownFailure;
                model.OtherCultures =
                (
                    await _context.SupportedCultures
                        .Select(x => new { x.Id, x.Name })
                        .ToArrayAsync()
                )
                .Select(x => (x.Id, x.Name))
                .Where(x => x.Id != id.Value)
                .ToArray();
                return View(model);
            }
        }


        private (bool IsValid, string Name) _validateCultureName(string name)
        {
            try
            {
                var culture = CultureInfo.CreateSpecificCulture(name);
                if (culture == CultureInfo.InvariantCulture)
                    return (false, string.Empty);

                return (true, culture.Name);
            }
            catch (CultureNotFoundException)
            {
                return (false, string.Empty);
            }
        }
    }
}
