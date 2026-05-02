using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using CamSistemWebArayuz.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CamSistemWebArayuz.Controllers
{
    [SessionController]
    [AuthLog(Roles = "KULLANİCİ")]
    public class KullaniciController : Controller
    {
        KullaniciRepo kRepo;

        // GET: Kullanici
        [AuthLog(Roles = "KULLANİCİ,GORUNTULEME")]
        public ActionResult Index()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "KullaniciSayfasi";
            kRepo = new KullaniciRepo();

            return View(kRepo.GetAll());
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT,GORUNTULEME")]
        public ActionResult KullaniciKaydet()
        {
            return PartialView("KullaniciKaydet");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult KullaniciKaydet(Kullanici KullaniciModel)//modelde tanımlandığı gibi değişken isimleri verilir
        {
            kRepo = new KullaniciRepo();
            try
            {
                if (ModelState.IsValid)
                {
                    KullaniciModel.KullaniciSoyadi = KullaniciModel.KullaniciSoyadi.ToUpper();
                    KullaniciModel.AktifMi = true;
                    KullaniciModel.KayitTarihi = DateTime.Now;
                    kRepo.AddAndSave(KullaniciModel);

                    ViewBag.RecordResult = 1;

                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.RecordResult = 2;
                return RedirectToAction("Index");
            }
        }

        [AuthLog(Roles = "SILME")]
        public ActionResult KullaniciSil(int Id)
        {
            try
            {
                kRepo = new KullaniciRepo();
                KullaniciRolRepo kullaniciRolRepo = new KullaniciRolRepo();
                List<KullaniciRol> kullaniciRols = kullaniciRolRepo.FindBy(e => e.KullaniciId == Id).ToList();
                if (kullaniciRols != null)
                {
                    foreach (var item in kullaniciRols)
                    {
                        kullaniciRolRepo.DeleteAndSave(item);
                    }
                }

                kRepo.DeleteAndSave(kRepo.FindBy(e => e.Id == Id).FirstOrDefault());

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult KullaniciDuzenle(int Id)
        {
            kRepo = new KullaniciRepo();
            Kullanici kullanici = kRepo.FindBy(e => e.Id == Id).FirstOrDefault();

            return View(kullanici);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult KullaniciDuzenle(Kullanici KullaniciModel)
        {
            kRepo = new KullaniciRepo();
            if (ModelState.IsValid)
            {
                KullaniciModel.GuncellemeTarihi = DateTime.Now;
                KullaniciModel.KullaniciSoyadi = KullaniciModel.KullaniciSoyadi.ToUpper();
                kRepo.EditAndSave(KullaniciModel);
                ViewBag.RecordResult = 1;
                ModelState.Clear();

                return RedirectToAction("Index", "Kullanici");
            }

            return View("Index");
        }
    }
}