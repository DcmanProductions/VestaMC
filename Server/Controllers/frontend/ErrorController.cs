﻿/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Microsoft.AspNetCore.Mvc;

namespace Chase.Vesta.Server.Controllers.frontend;

[Route("/error")]
public class ErrorController : Controller
{
    [Route("{code?}")]
    public IActionResult Index(int? code = 404)
    {
        ViewData["title"] = "Unexpected Error";
        switch (code)
        {
            case < 400:
                ViewData["title"] = "Successful Error";
                break;

            case 400:
                ViewData["title"] = "Bad Request";
                break;

            case 401:
                ViewData["title"] = "Unauthorized Access";
                break;

            case 402:
                ViewData["title"] = "Payment Required";
                break;

            case 403:
                ViewData["title"] = "Forbidden Access";
                break;

            case 404:
                ViewData["title"] = "Page Not Found";
                break;
        }
        ViewData["nav-page"] = -1;
        return View(code);
    }
}