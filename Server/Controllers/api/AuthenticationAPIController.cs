﻿/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

global using static Chase.Vesta.Server.Controllers.api.AuthenticationAPIController;
using Chase.Vesta.Core.Controllers;
using CLMath;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Chase.Vesta.Core.Models.SettingsModel;

namespace Chase.Vesta.Server.Controllers.api;

[ApiController]
[Produces("application/json")]
[Route("/api/authentication")]
public class AuthenticationAPIController : ControllerBase
{
    public static bool IsLoggedIn(HttpContext context)
    {
        bool loggedin = false;
        if (context.Request.Cookies.TryGetValue("access-token", out string? token))
        {
            JObject json = JObject.FromObject(JsonConvert.DeserializeObject(CLAESMath.DecryptStringAES(token)) ?? new { username = "", password = "" });
            AuthenticationSettingsModel Auth = ConfigurationController.Instance.Settings.Authentication;
            if (json.ContainsKey("username") && json.ContainsKey("password"))
            {
                string username = json.GetValue("username")?.ToObject<string>() ?? "";
                string password = json.GetValue("password")?.ToObject<string>() ?? "";

                if (Auth.Username.Equals(username) && Auth.Password.Equals(password))
                {
                    loggedin = true;
                }
            }
        }
        return loggedin;
    }

    [HttpPost("login")]
    public IActionResult Login([FromForm] string username, [FromForm] string? password = "")
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