using System.Text.RegularExpressions;
using Fenrus.Services;
using Jint;
using Microsoft.AspNetCore.Mvc;

namespace Fenrus.Controllers;

/// <summary>
/// Controller for app 
/// </summary>
[Route("/apps")]
public class DashboardAppController: Controller
{
    /// <summary>
    /// Gets an apps icon
    /// </summary>
    /// <param name="name">The app name</param>
    /// <returns>the app icon</returns>
    [HttpGet("{name}/{iconFile}")]
    [ResponseCache(Duration = 7 * 24 * 60 * 60)]
    public IActionResult Icon([FromRoute] string name, [FromRoute] string iconFile)
    {
        var app = AppService.GetByName(name);
        if (string.IsNullOrEmpty(app?.Icon))
            return new NotFoundResult();

        string appIcon = Path.Combine(app.FullPath, app.Icon);
        if(System.IO.File.Exists(appIcon) == false)
            return new NotFoundResult();

        var image = System.IO.File.OpenRead(appIcon);
        string type = "image/" + appIcon.Substring(appIcon.LastIndexOf(".", StringComparison.Ordinal) + 1);
        if (type == "image/svg")
            type = "image/svg+xml";
        return File(image, type);
    }

    /// <summary>
    /// Gets the status of a smart app
    /// </summary>
    /// <param name="name">The app name</param>
    /// <returns>the app status</returns>
    [HttpGet("{name}/{uid}/status")]
    public IActionResult Status([FromRoute] string name, [FromRoute] Guid uid)
    {
        var app = AppService.GetByName(name);
        if (app?.IsSmart != true)
            return new NotFoundResult();

        string codeFile = Path.Combine(app.FullPath, "code.js");
        if (System.IO.File.Exists(codeFile) == false)
            return new NotFoundResult();

        return Content("");
        
        string code = System.IO.File.ReadAllText(codeFile);
        code = string.Join("\n", code.Split("\n")[2..^3]).Trim();
        code = "function " + Regex.Replace(code, @"(?<=(^|[\s]+))(await|async)", string.Empty);


        var engine = new Engine(options =>
        {
        });

        engine.Evaluate(code);
        var result = engine.Invoke("status", new
        {
            url = "https://github.com/revenz/Fenrus/"
        });
        return Content(result.ToString());
    }    
}