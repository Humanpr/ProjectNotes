using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectNotes.Core.Entities;
using ProjectNotes.Core.Interfaces;
using ProjectNotes.Infrastructure.Services;

namespace ProjectNotes.WebUi.Controllers
{
        public class NotesController : Controller
        {
            public record NoteDTO(string Title, string Text);
            private readonly ILogger<HomeController> _logger;
            private readonly INoteManager noteManager;
            private readonly NoteTweeter tweeter;
            private string NameIdentifier;

            public NotesController(ILogger<HomeController> logger, INoteManager noteManager, NoteTweeter tweeter)
            {
                _logger = logger;
                this.noteManager = noteManager;
                this.tweeter = tweeter;
            }
            [Authorize]
            [HttpGet]
            public IActionResult AddNote()
            {
                return View();
            }

            [Authorize]
            [HttpPost]
            public async Task<IActionResult> AddNote(NoteDTO note)
            {
                NameIdentifier = User.Claims.SingleOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                //TODO add model validation error
                if (ModelState.IsValid)
                {
                    await noteManager.AddNote(new Note { Title = note.Title, Text = note.Text }, NameIdentifier);
                    return RedirectToAction("AddNote");
                }
                return RedirectToAction("AddNote");
            }

            [Authorize]
            [HttpGet]
            public async Task<int> TweetNote(int id)
            {
                await tweeter.TweetNote(id);
                return id;
            }

        }
    }
