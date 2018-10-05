// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace SimpleMvc.Controllers
{
    public class SimpleController : Controller
    {
        private readonly IStringLocalizer<SimpleController> _localizer;

        public SimpleController(IStringLocalizer<SimpleController> localizer)
        {
            _localizer = localizer;
        }

        public IActionResult HelloWorld()
        {
            var localization = _localizer["HELLO_WORLD"];
            return Content(localization);
        }

        public IActionResult HelloPerson(string person)
        {
            if (person == null)
                return BadRequest();

            var localization = _localizer["HELLO_PERSON", person];
            return Content(localization);
        }
    }
}
