using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class AccessController : Controller
{

    private readonly ILogger<AccessController> _logger;

    public AccessController(ILogger<AccessController> logger)
    {
        _logger = logger;
    }

    public IActionResult Login()
    {
        return View();
    }
    
    public async Task LoginGoogle()
    {
        var authparam = new AuthenticationProperties() {RedirectUri="/Home" };
        await HttpContext.ChallengeAsync(scheme:"Google",authparam).ConfigureAwait(false);
    }

    [Authorize]
    public async Task<IActionResult> Logout() {
        await HttpContext.SignOutAsync();
        return RedirectToAction("AskUserLogoutMainAccount");
    }

    public IActionResult AskUserLogoutMainAccount() 
    {
        return View();
    }

}

