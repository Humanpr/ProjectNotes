
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProjectNotes.Services
{
    public class UserExistanceChecker
    {
        private readonly ILogger<UserExistanceChecker> _logger;
        private readonly NotesDbContext context;

        public UserExistanceChecker(ILogger<UserExistanceChecker> logger,NotesDbContext context)
        {
            _logger = logger;
            this.context = context;
        }
        
        /// <summary>
        /// Returns true if user exists in Db table Authors
        /// </summary>
        /// <param name="nameidentifier"></param>
        /// <returns></returns>
        public async Task<bool> CheckUserWithNameIdentifier(string nameidentifier)
        {
            return await context.Authors.AnyAsync(a=>a.NameIdentifier==nameidentifier);
        }
        
    }
}
