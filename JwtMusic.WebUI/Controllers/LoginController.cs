using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebUI.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
