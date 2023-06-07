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
[Route("/api/fabric")]
public class FabricAPIController : ControllerBase
{
    [HttpGet("versions")]
    public async Task<IActionResult> GetFabricVersions()
    {
        return Ok(await FabricVersionController.GetFabricLoaderVersions());
    }

    [HttpGet("version/{ID}")]
    public async Task<IActionResult> GetFabricVersionByID([FromRoute] string id)
    {
        return Ok(await FabricVersionController.GetFabricLoaderByID(id));
    }
}