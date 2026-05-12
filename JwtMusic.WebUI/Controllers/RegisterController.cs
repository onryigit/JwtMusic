using Microsoft.AspNetCore.Mvc;

namespace JwtMusic.WebUI.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
