using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectNotes.Core.Entities;
using ProjectNotes.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectNotes.Infrastructure.Users.Queries
{
    public class GetUserByNameIdentifierQuery : IRequest<Author>
    {
        public string NameIdentifier { get; init; }
        public GetUserByNameIdentifierQuery(string NameIdentifier)
        {
            this.NameIdentifier = NameIdentifier;
        }

    }

    public class GetUserByNameIdentifierQueryHandler : IRequestHandler<GetUserByNameIdentifierQuery, Author>
    {
        private readonly NotesDbContext dbcontext;

        public GetUserByNameIdentifierQueryHandler(NotesDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        public async Task<Author> Handle(GetUserByNameIdentifierQuery request, CancellationToken cancellationToken)
        {
            var author = await dbcontext.Authors.SingleOrDefaultAsync(a => a.NameIdentifier == request.NameIdentifier);
            return author;
        }

    }

}
