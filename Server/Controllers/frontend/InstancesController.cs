﻿/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Vesta.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Chase.Vesta.Server.Controllers.frontend;

[Route("/instances")]
public class InstancesController : Controller
{
    public IActionResult Index()
    {
        if (!IsLoggedIn(HttpContext))
        {
            return RedirectToAction("Login", "Authentication");
        }
        ViewData["title"] = "Instance";
        ViewData["nav-page"] = 1;
        if (Guid.TryParse(HttpContext.Session.GetString("selected-instance"), out Guid id) && InstanceController.Exists(id))
        {
            return View(InstanceController.Get(id));
        }
        return RedirectToAction("Create");
    }

    [Route("create")]
    public IActionResult Create()
    {
        if (!IsLoggedIn(HttpContext))
        {
            return RedirectToAction("Login", "Authentication");
        }
        ViewData["title"] = "Create Instance";
        ViewData["nav-page"] = 1;
        return View();
    }
}