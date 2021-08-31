using System;
using System.Threading.Tasks;
using ProjectNotes.Models;

public interface IAuthorManager 
{
    public Task<Author> AddAuthor(Author newAuthor);
}