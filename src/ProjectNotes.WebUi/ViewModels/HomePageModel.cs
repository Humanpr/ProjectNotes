using ProjectNotes.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectNotes.WebUi.ViewModels
{
    public class HomePageModel
    {
        public string AuthorName { get; set; }
        public IEnumerable<Note> AuthorNotes { get; set; } = new List<Note>();
    }
}
