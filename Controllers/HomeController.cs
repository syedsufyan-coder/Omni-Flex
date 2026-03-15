using Microsoft.AspNetCore.Mvc;

namespace OmniFlex.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
