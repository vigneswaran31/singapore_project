using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Controllers
{
    public class ErrorController : Controller
    {

        //Return Error page with General Error.
        public ViewResult General(string message)
        {
            ViewBag.ErrorMessage = message;
            return View("Error");
        }

        //Return Error page with 404 Error.
        public ActionResult HttpError404(string message)
        {
            ViewBag.ErrorMessage = "The page cannot be found.";
            return View("Error");
        }

        //Return Error page with 500 Error.
        public ActionResult HttpError500(string message)
        {
            ViewBag.ErrorMessage = message;
            return View("Error");
        }
	}
}