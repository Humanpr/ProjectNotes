using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectNotes.ViewModels;

namespace ProjectNotes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INoteManager noteManager;

        public HomeController(ILogger<HomeController> logger,INoteManager noteManager)
        {
            _logger = logger;
            this.noteManager = noteManager;
        }

        public async Task<IActionResult> Index()
        { 
            _logger.LogError("Someone did something");
            HomePageModel model = new HomePageModel();
            
            if (User.Identity.IsAuthenticated)
            {
                model.AuthorName = User.Claims.SingleOrDefault(c => c.Type == "name").Value;
                model.AuthorNotes = await noteManager.GetAuthorNotesByNameIdentifier(User.Claims.SingleOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value);
                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult Error()
        {
            return View();
        }

    }
}
