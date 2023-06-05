﻿/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Microsoft.AspNetCore.Mvc;

namespace Chase.Vesta.Server.Controllers.frontend;

[Route("/")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (!IsLoggedIn(HttpContext))
        {
            return RedirectToAction("Login", "Authentication");
        }

        ViewData["title"] = "Dashboard";
        ViewData["nav-page"] = 0;
        return View();
    }
}