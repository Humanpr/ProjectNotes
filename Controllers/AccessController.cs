using System;
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
    
    public IActionResult LoginGoogle()
    {
        return View();
    }
}

