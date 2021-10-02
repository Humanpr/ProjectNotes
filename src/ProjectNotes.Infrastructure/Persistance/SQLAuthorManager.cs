using ProjectNotes.Core.Entities;
using ProjectNotes.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNotes.Infrastructure.Persistance
{
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
}
