/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Networking.Event;
using Chase.Vesta.Core;
using Chase.Vesta.Java.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Chase.VestaMC.Server.Controllers.api;

[ApiController]
[Produces("application/json")]
[Route("/api/java")]
public class JavaAPIController : ControllerBase
{
    private static Dictionary<string, DownloadProgressEventArgs> currentDownloadingProcesses = new();

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
    public async Task<IActionResult> DownloadJavaVersion(string version)
    {
        var manifests = await JavaController.GetJavaVersionManifests();
        await JavaController.InstallVersion(manifests.First(i => i.Version == version), (s, e) =>
        {
            currentDownloadingProcesses[version] = e;
        });
        return Ok(currentDownloadingProcesses);
    }

    [HttpDelete("{version}")]
    public IActionResult DeleteVersion(string version)
    {
        if (JavaController.IsJavaVersionInstalled(version))
        {
            Directory.Delete(JavaController.GetLocallyInstalledJavaVersion(version), true);
            return Ok();
        }
        return BadRequest(new { error = $"Java {version} is not installed!" });
    }

    [HttpGet("download/{version}/poll")]
    public IActionResult PollVersionProgress(string version)
    {
        if (currentDownloadingProcesses.TryGetValue(version, out var progress))
        {
            return Ok(progress);
        }
        return BadRequest(new { error = $"There is no download process for Java {version}" });
    }

    [HttpPost("upload")]
    public IActionResult UploadVersion([FromForm] IFormFile file)
    {
        string archive = Path.Combine(Values.Directories.TEMP, file.Name);
        using (FileStream fs = new(archive, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            file.CopyTo(fs);
        }

        FileInfo info = new(archive);
        string version = info.Name.Replace(info.Extension, "");
        JavaController.InstallLocalVersion(archive, JavaController.GetLocallyInstalledJavaVersion(version), version);
        return Ok();
    }
}