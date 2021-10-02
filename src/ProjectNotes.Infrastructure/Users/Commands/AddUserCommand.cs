using MediatR;
using ProjectNotes.Core.Entities;
using ProjectNotes.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectNotes.Infrastructure.Users.Commands
{
    public class AddUserCommand : IRequest<Author>
    {
        public Author NewUser { get; set; }
    }

    public class AddUserHandler : IRequestHandler<AddUserCommand, Author>
    {
        private readonly IAuthorManager authorManager;

        public AddUserHandler(IAuthorManager authorManager)
        {
            this.authorManager = authorManager;
        }
        public async Task<Author> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            return await authorManager.AddAuthor(request.NewUser);
        }
    }
}
