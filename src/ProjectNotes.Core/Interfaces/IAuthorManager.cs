using ProjectNotes.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNotes.Core.Interfaces
{
    public interface IAuthorManager
    {
        public Task<Author> AddAuthor(Author newAuthor);
    }
}
