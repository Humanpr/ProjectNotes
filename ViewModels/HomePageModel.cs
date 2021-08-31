using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectNotes.Models;

namespace ProjectNotes.ViewModels
{
    public class HomePageModel
    {        
        public string AuthorName { get; set; }
        public IEnumerable<Note> AuthorNotes { get; set; } = new List<Note>();
    }
}
