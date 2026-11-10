using Microsoft.AspNetCore.Mvc;

namespace ModuloAdminBodegon.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index -> panel principal
        public IActionResult Index()
        {
            return View();
        }
    }
}