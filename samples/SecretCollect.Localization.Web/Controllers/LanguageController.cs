using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecretCollect.Localization.SqlLocalizer.Settings;
using System;
using System.Linq;

namespace SecretCollect.Localization.Web.Controllers
{
    [AllowAnonymous]
    public class LanguageController : Controller
    {
        private readonly ILogger _logger;
        private readonly IOptions<GlobalizationOptions> _globalizationSettings;
        private readonly IOptions<RequestLocalizationOptions> _requestLocalizationOptions;
        private readonly IUrlHelper _urlHelper;

        public LanguageController(
            IOptions<GlobalizationOptions> globalizationOptions,
            IOptions<RequestLocalizationOptions> requestLocalizationOptions,
            IActionContextAccessor actionContextAccessor, 
            IUrlHelperFactory urlHelperFactory, 
            ILogger<LanguageController> logger)
        {
            _globalizationSettings = globalizationOptions;
            _requestLocalizationOptions = requestLocalizationOptions;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _logger = logger;
        }

        public IActionResult Index(string lang = null, string returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(lang))
            {
                var uiCulture = _requestLocalizationOptions.Value.SupportedUICultures.SingleOrDefault(c => c.Name == lang);
                if (uiCulture != null)
                {
                    Response.Cookies.Append(
                        _globalizationSettings.Value.CookieName,
                        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(_requestLocalizationOptions.Value.DefaultRequestCulture.Culture, uiCulture)),
                        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), Domain = _globalizationSettings.Value.CookieDomain, HttpOnly = false }
                    );
                }
            }

            return _toAllowedUrl(returnUrl);
        }

        private IActionResult _toAllowedUrl(string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                if (_urlHelper.IsLocalUrl(returnUrl))
                {
                    _logger.LogDebug($"Redirecting to local url: {returnUrl}");
                    return new RedirectResult(returnUrl);
                }
                else if (Uri.TryCreate(returnUrl, UriKind.Absolute, out Uri retUri) && (Request.Host.Value?.Contains(retUri.Host) ?? false))
                {
                    _logger.LogDebug($"Redirecting to allowed domain: {returnUrl}");
                    return new RedirectResult(returnUrl);
                }

                _logger.LogWarning($"Invalid returnUrl ({returnUrl}), redirecting to backoffice dashboard.");
            }

            return new RedirectToActionResult("Index", "Home", null);
        }
    }
}
