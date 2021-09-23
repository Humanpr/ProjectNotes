using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectNotes.Models;

public class GetUserByNameIdentifierQuery: IRequest<Author>
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
        var author = await dbcontext.Authors.SingleOrDefaultAsync(a=>a.NameIdentifier==request.NameIdentifier);
        return author;
    }

}
