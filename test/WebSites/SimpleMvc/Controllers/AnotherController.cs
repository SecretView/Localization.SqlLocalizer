// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace SimpleMvc.Controllers
{
    public class AnotherController : Controller
    {
        private readonly IStringLocalizer<AnotherController> _localizer;

        public AnotherController(IStringLocalizer<AnotherController> localizer)
        {
            _localizer = localizer;
        }

        public IActionResult FooBar()
        {
            var localization = _localizer["FOO_BAR"];
            return Content(localization);
        }

        public IActionResult FooBarBaz(string something)
        {
            if (something == null)
                return BadRequest();

            var localization = _localizer["FOO_BAR_BAZ", something];
            return Content(localization);
        }
    }
}
