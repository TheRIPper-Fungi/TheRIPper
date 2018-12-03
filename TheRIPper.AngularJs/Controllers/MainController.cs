using Microsoft.AspNetCore.Mvc;

namespace TheRIPPer.Razor.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Index() {
            return View();
        }
    }
}