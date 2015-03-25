using System.Web.Mvc;

namespace _02AfterSPA.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// In SPA apps these are typically just to serve templates 
        /// </summary>
        /// <remarks>
        /// Why not use static? Because with controllers I can [Authorize] to restrict access and
        /// load templates with localized text, etc.
        /// </remarks>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}