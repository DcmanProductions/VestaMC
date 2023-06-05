// LFInteractive LLC. - All Rights Reserved
global using static Chase.Vesta.Server.Controllers.api.AuthenticationController;
using Chase.Vesta.Core.Controllers;
using CLMath;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Chase.Vesta.Core.Models.SettingsModel;

namespace Chase.Vesta.Server.Controllers.api;

[ApiController]
[Produces("application/json")]
[Route("/api/authentication")]
public class AuthenticationController : ControllerBase
{
    public static bool IsAuthenticated(HttpContext context)
    {
        IRequestCookieCollection cookies = context.Request.Cookies;

        return false;
    }

    public static bool IsLoggedIn(HttpContext context)
    {
        bool loggedin = false;
        if (context.Request.Cookies.TryGetValue("username", out string username) && context.Request.Cookies.TryGetValue("password", out string password))
        {
            AuthenticationSettingsModel Auth = ConfigurationController.Instance.Settings.Authentication;
            if (Auth.Username.Equals(username) && Auth.Password.Equals(password))
            {
                loggedin = true;
            }
        }
        return loggedin;
    }

    [Route("login")]
    public IActionResult Login([FromForm] string username, [FromForm] string password)
    {
        AuthenticationSettingsModel Auth = ConfigurationController.Instance.Settings.Authentication;
        if (Auth.Username.Equals(username) && Auth.Password.Equals(password))
        {
            return Ok(new { token = CLAESMath.EncryptStringAES(JsonConvert.SerializeObject(new { username, password })) });
        }
        return BadRequest(new
        {
            error = "Invalid username or password"
        });
    }
}