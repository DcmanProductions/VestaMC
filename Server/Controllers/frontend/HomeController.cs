// LFInteractive LLC. - All Rights Reserved
using Microsoft.AspNetCore.Mvc;

namespace Chase.WebDeploy.Server.Controllers.frontend;

[Route("/")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewData["title"] = "Dashboard";
        ViewData["nav-page"] = 0;
        return View();
    }
}