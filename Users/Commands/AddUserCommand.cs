using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectNotes.Models;

public class AddUserCommand : IRequest<Author> {
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