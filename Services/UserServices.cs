
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectNotes.Models;

namespace ProjectNotes.Services
{
    public class UserServices
    {
        private readonly ILogger<UserServices> _logger;
        private readonly IAuthorManager authorManager;

        public UserServices(ILogger<UserServices> logger,IAuthorManager authorManager)
        {
            _logger = logger;
            this.authorManager = authorManager;
        }

        public async Task<Author> AddUser(Author newAuthor)
        {
            return await authorManager.AddAuthor(newAuthor);
        }

    }
}
