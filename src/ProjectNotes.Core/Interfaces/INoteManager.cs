using ProjectNotes.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNotes.Core.Interfaces
{
    public interface INoteManager
    {
        public Task<IEnumerable<Note>> GetAuthorNotesByNameIdentifier(string nameIdentifier);
        public Task<Note> AddNote(Note newNote, string nameIdentifier);
    }
}
