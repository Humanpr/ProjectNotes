using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectNotes.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectNotes.Infrastructure.Users.Queries
{
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
}
