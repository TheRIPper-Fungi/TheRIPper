using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TheRIPper.UI.NoDatabase.Controllers
{
    public class AppController : Controller
    {
        public IActionResult Home() {
            SessionManagement.SessionMethods.Set<DateTime>(HttpContext.Session, "DateTime", DateTime.Now, true, null);
            return View();
        }

        public IActionResult Background() {
            var dt = SessionManagement.SessionMethods.Get<DateTime>(HttpContext.Session, "DateTime", true, null);
            return View();
        }

        public IActionResult Contact() {
            return View();
        }

    }
}