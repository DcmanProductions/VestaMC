/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.VestaMC.Minecraft.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Chase.VestaMC.Server.Controllers.api;

[ApiController]
[Produces("application/json")]
[Route("/api/minecraft")]
public class MinecraftAPIController : ControllerBase
{
    [HttpGet("versions")]
    public async Task<IActionResult> GetMinecraftVersions()
    {
        return Ok(await MinecraftVersionController.GetMinecraftVersions());
    }

    [HttpGet("versions/release")]
    public async Task<IActionResult> GetMinecraftReleaseVersions()
    {
        return Ok(await MinecraftVersionController.GetMinecraftReleaseVersions());
    }

    [HttpGet("versions/snapshot")]
    public async Task<IActionResult> GetMinecraftSnapshotVersions()
    {
        return Ok(await MinecraftVersionController.GetMinecraftSnapshotVersions());
    }

    [HttpGet("version/{ID}")]
    public async Task<IActionResult> GetMinecraftVersionByID(string id)
    {
        return Ok(await MinecraftVersionController.GetMinecraftVersionByID(id));
    }
}