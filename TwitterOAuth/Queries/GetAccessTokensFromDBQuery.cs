using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectNotes.TwitterOAuth.Queries
{
    public class GetAccessTokensFromDBQuery:IRequest<string>
    {
        public readonly string userId;

        public GetAccessTokensFromDBQuery(string userId)
        {
            this.userId = userId;
        }
    }

    public class GetAccessTokensFromDBQueryHandler : IRequestHandler<GetAccessTokensFromDBQuery, string>
    {
        private readonly NotesDbContext notesDbContext;

        public GetAccessTokensFromDBQueryHandler(NotesDbContext notesDbContext)
        {
            this.notesDbContext = notesDbContext;
        }
        public async Task<string> Handle(GetAccessTokensFromDBQuery request, CancellationToken cancellationToken)
        {
            var userId = request.userId;
            var user = await notesDbContext.Authors.SingleOrDefaultAsync(a=>a.NameIdentifier==userId);

            var access_token = user.accesstoken;
            var access_token_secret = user.accesstokensecret;

            return access_token + " " + access_token_secret;
        }
    }
}
