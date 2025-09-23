using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AL.Areas.Admin.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Admin/Error
        public ActionResult ErrorView()
        {
            return View();
        }
    }
}