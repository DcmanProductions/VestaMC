// LFInteractive LLC. - All Rights Reserved
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers.frontend;

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