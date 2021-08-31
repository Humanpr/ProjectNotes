using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectNotes.Models;

namespace ProjectNotes.Controllers
{
    public class NotesController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public NotesController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        [Authorize]
        public IActionResult AddNote()
        {
            return View();
        }

    }
}
