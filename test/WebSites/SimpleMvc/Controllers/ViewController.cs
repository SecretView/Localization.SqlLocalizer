// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace SimpleMvc.Controllers
{
    public class ViewController : Controller
    {
        public IActionResult SelectListUsingEnum() => View();
    }
}
