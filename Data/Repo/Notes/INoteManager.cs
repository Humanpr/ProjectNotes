using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectNotes.Models;

public interface INoteManager
{
    public Task<IEnumerable<Note>> GetAuthorNotesByNameIdentifier(string nameIdentifier);
    public Task<Note> AddNote(Note newNote, string nameIdentifier);
}