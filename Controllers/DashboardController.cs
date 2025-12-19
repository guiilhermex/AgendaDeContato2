using Microsoft.AspNetCore.Mvc;

namespace AgendaContato.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
