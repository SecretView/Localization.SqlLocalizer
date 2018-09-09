using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecretCollect.Localization.SqlLocalizer.Data;
using SecretCollect.Localization.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretCollect.Localization.Web.Controllers
{
    public class KeysController : Controller
    {
        private readonly LocalizationContext _context;

        public KeysController(LocalizationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var baseKeys = await _context.LocalizationKeys.Select(k => k.Base).Distinct().ToArrayAsync();

            return View(baseKeys);
        }

        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            var results = await _context.LocalizationRecords
                .Where(r => r.Text.Contains(query))
                .Select(r => new
                {
                    Culture = r.Culture.Name,
                    Base = r.LocalizationKey.Base,
                    Key = r.LocalizationKey.Key,
                    Text = r.Text
                })
                .ToArrayAsync();
            var model = new SearchVM()
            {
                Query = query,
                Items = results.Select(r =>
                {
                    var space = 20;
                    var index = r.Text.IndexOf(query);
                    var start = Math.Max(0, index - space);
                    var end = Math.Min(r.Text.Length, index + query.Length + space);

                    var text = r.Text.Substring(start, end - start);
                    if (start > 0)
                        text = "..." + text;
                    if (end < r.Text.Length)
                        text = text + "...";
                    return new SearchResultItemVM
                    {
                        Base = r.Base,
                        Key = r.Key,
                        Culture = r.Culture,
                        Text = text
                    };
                }).ToArray()
            };
            return View(model);
        }

        public async Task<IActionResult> SubKey(string baseKey)
        {
            var model = new BaseKeyVM()
            {
                Base = baseKey,
                Keys = await _context.LocalizationKeys.Where(k => k.Base == baseKey).ToArrayAsync()
            };

            if (!model.Keys.Any())
                return RedirectToAction(nameof(Index));

            return View(model);
        }

        public IActionResult Add(string baseKey)
        {
            return View(new KeyVM()
            {
                Base = baseKey
            });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(KeyVM model)
        {
            if (model == null)
                return View(model);
            if (string.IsNullOrWhiteSpace(model.Base))
                return View(model);
            if (string.IsNullOrWhiteSpace(model.Main))
                return View(model);

            try
            {
                var key = new LocalizationKey()
                {
                    Base = model.Base,
                    Comment = model.Comment,
                    Key = model.Main
                };
                
                _context.Add(key);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Edit), new { baseKey = model.Base, mainKey = model.Main, saveResult = SaveResult.Success });
            }
            catch (DbUpdateException ex) when ((ex.InnerException as System.Data.SqlClient.SqlException)?.Number == 2601)
            {
                _context.ChangeTracker.Entries().ToList().ForEach(e => e.State = EntityState.Detached);
                model.SaveResult = SaveResult.Duplicate;
                return View(model);
            }
            catch (Exception)
            {
                model.SaveResult = SaveResult.UnknownFailure;
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(string baseKey, string mainKey, SaveResult saveResult = SaveResult.None)
        {
            var key = await _context.LocalizationKeys
                .Include(l => l.Records)
                .ThenInclude(l => l.Culture)
                .SingleOrDefaultAsync(k => k.Base == baseKey && k.Key == mainKey);
            if (key == null)
                return NotFound();

            var model = new KeyVM()
            {
                Base = key.Base,
                Comment = key.Comment,
                Id = key.Id,
                Main = key.Key,
                Translations = key.Records.Select(l => new LocVM()
                {
                    CultureId = l.Culture.Id,
                    CultureName = l.Culture.Name,
                    Translation = l.Text
                }).ToArray(),
                SaveResult = saveResult
            };
            var translationCultures = model.Translations.Select(t => t.CultureId).ToArray();
            var unfilledTranslations = await _context.SupportedCultures
                .Where(c => !translationCultures.Contains(c.Id))
                .Select(c => new LocVM()
                {
                    CultureId = c.Id,
                    CultureName = c.Name,
                    Translation = ""
                })
                .ToArrayAsync();

            model.Translations = model.Translations.Concat(unfilledTranslations).OrderBy(c => c.CultureName).ToArray();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string baseKey, string mainKey, KeyVM model)
        {
            var key = await _context.LocalizationKeys
                .Include(l => l.Records)
                .ThenInclude(l => l.Culture)
                .SingleOrDefaultAsync(k => k.Id == model.Id);
            if (key == null)
                return NotFound();

            if (model.Comment == LocalizationKey.DEFAULT_COMMENT)
                model.Comment = "";
            key.Comment = model.Comment;
            foreach (var modelRecord in model.Translations)
            {
                var dbRecord = key.Records.SingleOrDefault(r => r.Culture.Id == modelRecord.CultureId);
                if (dbRecord == null)
                {
                    dbRecord = new LocalizationRecord()
                    {
                        Culture = await _context.SupportedCultures.SingleOrDefaultAsync(c => c.Id == modelRecord.CultureId),
                        LocalizationKey = key,
                        Text = modelRecord.Translation,
                        Status = string.IsNullOrWhiteSpace(modelRecord.Translation) ? RecordStatus.New : RecordStatus.HumanTranslated
                    };
                    _context.LocalizationRecords.Add(dbRecord);
                    _context.Entry(dbRecord).State = EntityState.Added;
                }
                else
                {
                    dbRecord.Text = modelRecord.Translation;
                    dbRecord.Status = string.IsNullOrWhiteSpace(modelRecord.Translation) ? RecordStatus.New : RecordStatus.HumanTranslated;
                    _context.Entry(dbRecord).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { baseKey = key.Base, mainKey = key.Key, saveResult = SaveResult.Success });
        }

        public async Task<IActionResult> Missing()
        {
            var model = (await _context.SupportedCultures.ToArrayAsync())
                .Select(c => new MissingLocalizationsVM()
                {
                    CultureId = c.Id,
                    CultureName = c.Name,
                    LocalizationKeys = _context.GetMissingLocalizationsForCulture(c)
                })
                .ToArray();
            return View(model);
        }
    }
}
