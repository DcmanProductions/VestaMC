/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Networking.Event;
using Chase.Vesta.Java.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Chase.VestaMC.Server.Controllers.api;

[ApiController]
[Produces("application/json")]
[Route("/api/java")]
public class JavaAPIController : ControllerBase
{
    private static Dictionary<int, DownloadProgressEventArgs> currentDownloadingProcesses = new();

    [HttpGet("versions")]
    public async Task<IActionResult> GetVersions()
    {
        return Ok(await JavaController.GetJavaVersionManifests());
    }

    [HttpGet("versions/local")]
    public IActionResult GetLocalVersions()
    {
        return Ok(JavaController.GetLocallyInstalledJavaVersions());
    }

    [HttpGet("download/{version}")]
    public async Task<IActionResult> DownloadJavaVersion(int version)
    {
        var manifests = await JavaController.GetJavaVersionManifests();
        await JavaController.InstallVersion(manifests.First(i => i.Version == version), (s, e) =>
        {
            currentDownloadingProcesses[version] = e;
        });
        return Ok(currentDownloadingProcesses);
    }

    [HttpDelete("{version}")]
    public IActionResult DeleteVersion(int version)
    {
        if (JavaController.IsJavaVersionInstalled(version))
        {
            Directory.Delete(JavaController.GetLocallyInstalledJavaVersion(version), true);
            return Ok();
        }
        return BadRequest(new { error = $"Java {version} is not installed!" });
    }

    [HttpGet("download/{version}/poll")]
    public IActionResult PollVersionProgress(int version)
    {
        if (currentDownloadingProcesses.TryGetValue(version, out var progress))
        {
            return Ok(progress);
        }
        return BadRequest(new { error = $"There is no download process for Java {version}" });
    }
}