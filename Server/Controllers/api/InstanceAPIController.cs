/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Vesta.Core.Controllers;
using Chase.Vesta.Vesta.Models;
using Chase.VestaMC.Modded.Data;
using Chase.VestaMC.Vesta.Events;
using Microsoft.AspNetCore.Mvc;

namespace Chase.VestaMC.Server.Controllers.api;

[ApiController]
[Produces("application/json")]
[Route("/api/instances")]
public class InstanceAPIController : ControllerBase
{
    private static Dictionary<Guid, InstallingInstanceEventArgs> InstallationProgress = new();

    [HttpGet]
    public IActionResult GetInstances()
    {
        return Ok();
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public IActionResult CreateInstance([FromForm] string name, [FromForm] int java, [FromForm] string minecraftVersion, [FromForm] SupportedModloaders modloader, [FromForm] string? modloaderVersion, [FromForm] IFormFile? javaArchive, [FromForm] string? javaArchiveVersion)
    {
        InstanceModel instance = new()
        {
            Name = name,
            MinecraftVersion = minecraftVersion,
            ModLoader = modloader,
            ModLoaderVersion = modloaderVersion,
            JavaSettings = new()
            {
                JavaVersion = java.ToString(),
            }
        };
        if (InstanceController.TryAdd(instance))
        {
            InstanceController.InstallInstance(instance, (s, e) =>
            {
                InstallationProgress[instance.Id] = e;
            });
            return Ok(new { id = instance.Id });
        }
        return BadRequest(new { error = "Unable to create instance!" });
    }
}