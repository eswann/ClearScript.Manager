using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClearScript.Manager.WebDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View("JSql");
        }

        public ActionResult JSql()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
