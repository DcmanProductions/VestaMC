/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.VestaMC.Modded.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Chase.VestaMC.Server.Controllers.api;

[ApiController]
[Produces("application/json")]
[Route("/api/forge")]
public class ForgeAPIController : ControllerBase
{
    [HttpGet("versions")]
    public async Task<IActionResult> GetForgeVersions([FromQuery] string mc)
    {
        return Ok(await ForgeVersionController.GetForgeLoaderVersions(mc));
    }

    [HttpGet("version/{ID}")]
    public async Task<IActionResult> GetForgeVersionByID([FromRoute] string id, [FromQuery] string mc)
    {
        return Ok(await ForgeVersionController.GetForgeLoaderVersionByID(mc, id));
    }
}