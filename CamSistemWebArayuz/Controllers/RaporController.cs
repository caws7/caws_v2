using CamSistemDataLayer.Enums;
using CamSistemDataLayer.Repos;
using CamSistemWebArayuz.Attributes;
using System.Linq;
using System.Web.Mvc;

namespace CamSistemWebArayuz.Controllers
{
    [SessionController]
    [AuthLog(Roles = "RAPOR")]
    public class RaporController : Controller
    {
        SiparisRepo siparisRepo;

        // GET: Rapor
        [AuthLog(Roles = "RAPOR,GORUNTULEME")]
        public ActionResult TamamlananSiparisListesi()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "TamamlananRaporSayfasi";
            siparisRepo = new SiparisRepo();

            return View(siparisRepo.FindBy(e => e.DurumId == (int)Durumlar.TeslimEdildi).ToList());
        }

        [AuthLog(Roles = "RAPOR,GORUNTULEME")]
        public ActionResult IptalEdilenSiparisListesi()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "IptalEdilenRaporSayfasi";
            siparisRepo = new SiparisRepo();

            return View(siparisRepo.FindBy(e => e.DurumId == (int)Durumlar.Reddedildi).ToList());
        }
    }
}