using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using CamSistemWebArayuz.Attributes;
using System.Linq;
using System.Web.Mvc;

namespace CamSistemWebArayuz.Controllers
{
    [SessionController]
    [AuthLog(Roles = "FİYATLANDIRMA")]
    public class FiyatController : Controller
    {
        // GET: Fiyat     
        [AuthLog(Roles = "FİYATLANDIRMA,GORUNTULEME")]
        public ActionResult Cam()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "FiyatSayfasiCam";
            CamKombinasyonRepo camKombinasyonRepo = new CamKombinasyonRepo();

            return View(camKombinasyonRepo.GetAll());
        }

        [AuthLog(Roles = "FİYATLANDIRMA,GORUNTULEME")]
        public ActionResult Sistem()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "FiyatSayfasiSistem";
            SistemAltSistemJoinRepo sistemAltSistemJoinRepo = new SistemAltSistemJoinRepo();

            return View(sistemAltSistemJoinRepo.GetAll());
        }

        [HttpPost]
        [AuthLog(Roles = "DUZENLEME,YENIKAYIT,ONAYLAMA")]
        public JsonResult SistemFiyatla(int Id, decimal Fiyat)
        {
            SistemAltSistemJoinRepo sistemAltSistemJoinRepo = new SistemAltSistemJoinRepo();
            SistemAltSistemJoin sistemAltSistemJoin = sistemAltSistemJoinRepo.FindBy(e => e.Id == Id).FirstOrDefault();
            sistemAltSistemJoin.BirimFiyat = Fiyat;
            sistemAltSistemJoinRepo.EditAndSave(sistemAltSistemJoin);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [AuthLog(Roles = "DUZENLEME,YENIKAYIT,ONAYLAMA")]
        public JsonResult CamFiyatla(int Id, decimal Fiyat)
        {
            CamKombinasyonRepo camKombinasyonRepo = new CamKombinasyonRepo();
            CamKombinasyon camKombinasyon = camKombinasyonRepo.FindBy(e => e.Id == Id).FirstOrDefault();
            camKombinasyon.BirimFiyat = Fiyat;
            camKombinasyonRepo.EditAndSave(camKombinasyon);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        
    }
}