using System;
using System.Threading.Tasks;
using ProjectNotes.Models;

public class SQLAuthorManager : IAuthorManager 
{
    private readonly NotesDbContext context;

    public SQLAuthorManager(NotesDbContext context)
    {
        this.context = context;
    }

    public async Task<Author> AddAuthor(Author newAuthor)
    {
        await context.Authors.AddAsync(newAuthor);
        await context.SaveChangesAsync();
        return newAuthor;
    }

}