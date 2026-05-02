using System.Web.Mvc;

namespace CamSistemWebArayuz.Controllers
{
    public class PermissionController : Controller
    {
        // GET: Permission
        public ActionResult Error()
        {
            return PartialView("Error");
        }

        public ActionResult Denied()
        {
            TempData["RecordResult"] = "4";
            
            return RedirectToAction("Index", "Home");
        }
    }
}