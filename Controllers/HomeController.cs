using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectNotes.Models;
using ProjectNotes.ViewModels;

namespace ProjectNotes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INoteManager noteManager;

        public HomeController(ILogger<HomeController> logger,INoteManager noteManager)
        {
            _logger = logger;
            this.noteManager = noteManager;
        }

        public async Task<IActionResult> Index()
        {
            
            HomePageModel model = new HomePageModel();
            
            if (User.Identity.IsAuthenticated)
            {
                model.AuthorName = User.Claims.SingleOrDefault(c => c.Type == "name").Value;
                model.AuthorNotes = await noteManager.GetAuthorNotesByNameIdentifier(User.Claims.SingleOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value);
                return View(model);
            }
            else
            {
                return View(model);
            }
        }

    }
}
