
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectNotes.Models;

public class SQLNoteManager : INoteManager
{
    private readonly NotesDbContext context;

    public SQLNoteManager(NotesDbContext context)
    {
        this.context = context;
    }

    public async Task<Note> AddNote(Note newNote,string nameIdentifier)
    {
        var author = await context.Authors.SingleOrDefaultAsync(a => a.NameIdentifier == nameIdentifier);
        newNote.Author = author;
        newNote.CreatedDate = DateTime.Now;
        await context.Notes.AddAsync(newNote);
        await context.SaveChangesAsync();
        return newNote;
    }

    /// <summary>
    /// Returns notes of user matching with passed NameIdentifier.
    /// </summary>
    /// <param name="nameIdentifier"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Note>> GetAuthorNotesByNameIdentifier(string nameIdentifier)
    {
        var author = await context.Authors.Include(a => a.Notes).SingleOrDefaultAsync(author => author.NameIdentifier == nameIdentifier);
        return author.Notes;
    }
}