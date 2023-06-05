// LFInteractive LLC. - All Rights Reserved
using Microsoft.AspNetCore.Mvc;

namespace Chase.Vesta.Server.Controllers.frontend;

[Route("/authentication")]
public class AuthenticationController : Controller
{
    [Route("login")]
    public IActionResult Login()
    {
        if (IsLoggedIn(HttpContext))
        {
            return RedirectToAction("Index", "Home");
        }
        ViewData["title"] = "Login";
        ViewData["hide-nav"] = true;
        return View();
    }
}