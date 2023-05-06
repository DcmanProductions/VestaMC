// LFInteractive LLC. - All Rights Reserved
using Chase.WebDeploy.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Chase.WebDeploy.Server.Controllers.frontend;

[Route("/instances")]
public class InstancesController : Controller
{
    public IActionResult Index()
    {
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
        ViewData["title"] = "Create Instance";
        ViewData["nav-page"] = 1;
        return View();
    }
}