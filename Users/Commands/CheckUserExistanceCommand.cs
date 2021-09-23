using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

public class CheckUserExistanceCommand : IRequest<bool>
{
    public readonly string nameIdentifier;

    public CheckUserExistanceCommand(string nameIdentifier)
    {
        this.nameIdentifier = nameIdentifier;
    }
}

public class CheckUserExistanceCommandHandler : IRequestHandler<CheckUserExistanceCommand, bool>
{
    private readonly NotesDbContext notesDbContext;

    public CheckUserExistanceCommandHandler(NotesDbContext notesDbContext)
    {
        this.notesDbContext = notesDbContext;
    }
    public async Task<bool> Handle(CheckUserExistanceCommand request, CancellationToken cancellationToken)
    {
        return await notesDbContext.Authors.AnyAsync(a => a.NameIdentifier == request.nameIdentifier);
    }
}
