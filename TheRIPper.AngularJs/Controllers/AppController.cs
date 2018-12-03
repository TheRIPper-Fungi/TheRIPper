using Microsoft.AspNetCore.Mvc;

namespace TheRIPPer.Razor.Controllers
{
    public class AppController : Controller
    {
        public IActionResult Index() {
            return View();
        }

        public ActionResult Home() {
            var id = User.Identity;

            return View();
        }


        public ActionResult Background() {
            return View();
        }
    }
}