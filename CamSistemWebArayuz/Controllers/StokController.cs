using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using CamSistemWebArayuz.Attributes;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CamSistemWebArayuz.Controllers
{
    [SessionController]
    [AuthLog(Roles = "STOK")]
    public class StokController : Controller
    {
        StokRepo stokRepo;
        StokAksesuarRepo aksesuarStokRepo;
        AtikStokRepo atikStokRepo;

        // GET: Stok
        [AuthLog(Roles = "STOK,GORUNTULEME")]
        public ActionResult Index()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "ProfilStokSayfasi";
            stokRepo = new StokRepo();
            return View(stokRepo.GetAll());
        }
        
        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult StokKaydet()
        {
            ProfilBoyRepo pbRepo = new ProfilBoyRepo();
            ProfilRepo pRepo = new ProfilRepo();

            ViewBag.ProfilBoyList = pbRepo.GetAll();
            ViewBag.ProfilList = pRepo.GetAll().ToList().Where(e => e.ProfilKodu.ToString().Count(c => c == '-') < 2).ToList();

            return View();
        }

        [HttpPost]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult StokKaydet(Stok stok)
        {
            //buraya tabloya kaydetme yapılacak. Eğer o profil ve boy varsa update olacak yoksa insert olacak
            stokRepo = new StokRepo();
            Stok stokEnt;
            if (stok.OzelOlcu != null)
            {
                stokEnt = stokRepo.FindBy(e => e.ProfilId == stok.ProfilId && e.OzelOlcu == stok.OzelOlcu).FirstOrDefault();
            }
            else
            {
                stokEnt = stokRepo.FindBy(e => e.ProfilId == stok.ProfilId && e.ProfilBoyId == stok.ProfilBoyId).FirstOrDefault();
            }

            if (stokEnt != null)
            {
                stokEnt.StokAdet = stok.StokAdet + stokEnt.StokAdet;
                stokRepo.EditAndSave(stokEnt);

                return Json("UPDATE", JsonRequestBehavior.AllowGet);
            }
            else
            {
                Stok newEnt = new Stok
                {
                    ProfilBoyId = stok.ProfilBoyId,
                    ProfilId = stok.ProfilId,
                    OzelOlcu = stok.OzelOlcu,
                    StokAdet = stok.StokAdet
                };

                stokRepo.AddAndSave(newEnt);
                return Json("INSERT", JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "YENIKAYIT,DUZENLEME")]
        public ActionResult StokDus(long StokId, int StokAdet)
        {
            stokRepo = new StokRepo();
            Stok stokEnt = stokRepo.FindBy(e => e.Id == StokId).FirstOrDefault();
            stokEnt.StokAdet = stokEnt.StokAdet - StokAdet;
            stokRepo.EditAndSave(stokEnt);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [AuthLog(Roles = "SILME,DUZENLEME")]
        public ActionResult StokSil(int Id)
        {
            try
            {
                stokRepo = new StokRepo();
                stokRepo.DeleteAndSave(stokRepo.FindBy(e => e.Id == Id).FirstOrDefault());

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: AksesuarStok
        [AuthLog(Roles = "STOK,GORUNTULEME")]
        public ActionResult Aksesuar()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "AksesuarStokSayfasi";
            aksesuarStokRepo = new StokAksesuarRepo();
            return View(aksesuarStokRepo.GetAll());
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult AksesuarStokKaydet()
        {
            AksesuarRepo aksesuarRepo = new AksesuarRepo();

            ViewBag.AksesuarList = aksesuarRepo.GetAll();

            return View();
        }

        [HttpPost]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult AksesuarStokKaydet(StokAksesuar stokAksesuar)
        {
            //buraya tabloya kaydetme yapılacak. Eğer o profil ve boy varsa update olacak yoksa insert olacak
            aksesuarStokRepo = new StokAksesuarRepo();
            StokAksesuar stokAksesuarEnt;

            stokAksesuarEnt = aksesuarStokRepo.FindBy(e => e.AksesuarId == stokAksesuar.AksesuarId).FirstOrDefault();         
            if (stokAksesuarEnt != null)
            {
                stokAksesuarEnt.StokAdet = stokAksesuarEnt.StokAdet + stokAksesuar.StokAdet;
                aksesuarStokRepo.EditAndSave(stokAksesuarEnt);

                return Json("UPDATE", JsonRequestBehavior.AllowGet);
            }
            else
            {
                StokAksesuar newEnt = new StokAksesuar
                {
                    AksesuarId = stokAksesuar.AksesuarId,
                    StokAdet = stokAksesuar.StokAdet
                };

                aksesuarStokRepo.AddAndSave(newEnt);
                return Json("INSERT", JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "YENIKAYIT,DUZENLEME,ONAYLAMA")]
        public ActionResult AksesuarStokDus(long StokId, int StokAdet)
        {
            aksesuarStokRepo = new StokAksesuarRepo();
            StokAksesuar stokAksesuarEnt = aksesuarStokRepo.FindBy(e => e.Id == StokId).FirstOrDefault();
            stokAksesuarEnt.StokAdet = stokAksesuarEnt.StokAdet - StokAdet;
            aksesuarStokRepo.EditAndSave(stokAksesuarEnt);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [AuthLog(Roles = "SILME")]
        public ActionResult AksesuarStokSil(int Id)
        {
            try
            {
                aksesuarStokRepo = new StokAksesuarRepo();
                aksesuarStokRepo.DeleteAndSave(aksesuarStokRepo.FindBy(e => e.Id == Id).FirstOrDefault());

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: FireStok
        [AuthLog(Roles = "STOK,GORUNTULEME")]
        public ActionResult Fire()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "FireStokSayfasi";
            atikStokRepo = new AtikStokRepo();
            return View(atikStokRepo.GetAll());
        }

        [AuthLog(Roles = "YENIKAYIT,DUZENLEME,ONAYLAMA")]
        public ActionResult FireStokDus(long StokId, int StokAdet)
        {
            atikStokRepo = new AtikStokRepo();
            AtikStok stokEnt = atikStokRepo.FindBy(e => e.Id == StokId).FirstOrDefault();
            stokEnt.Adet = stokEnt.Adet - StokAdet;
            atikStokRepo.EditAndSave(stokEnt);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult FireStokKaydet()
        {
            ProfilRepo pRepo = new ProfilRepo();
            ViewBag.ProfilList = pRepo.GetAll().ToList().Where(e => e.ProfilKodu.ToString().Count(c => c == '-') < 2).ToList();

            return View();
        }

        [HttpPost]
        [AuthLog(Roles = "YENIKAYIT")]
        public JsonResult FireStokKaydet(AtikStok fireStok)
        {
            //buraya tabloya kaydetme yapılacak. Eğer o profil ve boy varsa update olacak yoksa insert olacak
            atikStokRepo = new AtikStokRepo();
            AtikStok atikStokEnt;
            atikStokEnt = atikStokRepo.FindBy(e => e.ProfilId == fireStok.ProfilId && e.Olcu == fireStok.Olcu).FirstOrDefault();
           

            if (atikStokEnt != null)
            {
                atikStokEnt.Adet = fireStok.Adet + atikStokEnt.Adet;
                atikStokRepo.EditAndSave(atikStokEnt);

                return Json("UPDATE", JsonRequestBehavior.AllowGet);
            }
            else
            {
                AtikStok newEnt = new AtikStok
                {
                    ProfilId = fireStok.ProfilId,
                    Olcu = fireStok.Olcu,
                    Adet = fireStok.Adet
                };

                atikStokRepo.AddAndSave(newEnt);
                return Json("INSERT", JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "SILME")]
        public ActionResult FireStokSil(int Id)
        {
            try
            {
                atikStokRepo = new AtikStokRepo();
                atikStokRepo.DeleteAndSave(atikStokRepo.FindBy(e => e.Id == Id).FirstOrDefault());

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

    }
}