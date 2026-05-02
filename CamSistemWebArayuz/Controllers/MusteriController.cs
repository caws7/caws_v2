using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using CamSistemWebArayuz.Attributes;
using CamSistemWebArayuz.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CamSistemWebArayuz.Controllers
{
    [SessionController]
    [AuthLog(Roles = "MUSTERİ")]
    public class MusteriController : Controller
    {
        MusteriRepo mRepo;
        AdresRepo aRepo;

        // GET: Musteri
        [AuthLog(Roles = "MUSTERİ,GORUNTULEME")]
        public ActionResult Index()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "MusteriSayfasi";
            mRepo = new MusteriRepo();

            return View(mRepo.GetAll());
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT,GORUNTULEME")]
        public ActionResult MusteriKaydet()
        {
            return PartialView("MusteriKaydet");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult MusteriKaydet(Musteri MusteriModel, Adres AdresModel)//modelde tanımlandığı gibi değişken isimleri verilir
        {
            aRepo = new AdresRepo();
            mRepo = new MusteriRepo();

            try
            {
                if (ModelState.IsValid)
                {
                    AdresModel.KayitTarihi = DateTime.Now;
                    Adres adresEntity = aRepo.SaveAndReturnEntity(AdresModel);

                    MusteriModel.MusteriSoyadi = MusteriModel.MusteriSoyadi.ToUpper();
                    MusteriModel.KayitTarihi = DateTime.Now;
                    MusteriModel.AdresId = adresEntity.Id;
                    mRepo.AddAndSave(MusteriModel);

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
        public ActionResult MusteriSil(int MusteriId)
        {
            try
            {
                aRepo = new AdresRepo();
                mRepo = new MusteriRepo();

                int adresId = Convert.ToInt32(mRepo.FindBy(e => e.Id == MusteriId).FirstOrDefault().AdresId);
                Adres adresEntity = aRepo.FindBy(e => e.Id == adresId).FirstOrDefault();
                aRepo.DeleteAndSave(adresEntity);

                Musteri musteriEntity = mRepo.FindBy(e => e.Id == MusteriId).FirstOrDefault();
                mRepo.DeleteAndSave(musteriEntity);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult MusteriDuzenle(int Id)
        {
            aRepo = new AdresRepo();
            mRepo = new MusteriRepo();

            MusteriveAdresModel multiModel = new MusteriveAdresModel();

            Musteri musteri = mRepo.FindBy(e => e.Id == Id).FirstOrDefault();
            Adres adres = aRepo.FindBy(e => e.Id == musteri.AdresId).FirstOrDefault();

            multiModel.AdresModel = adres;
            multiModel.MusteriModel = musteri;
            return View(multiModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult MusteriDuzenle(Musteri MusteriModel, Adres AdresModel)//modelde tanımlandığı gibi değişken isimleri verilir
        {
            aRepo = new AdresRepo();
            mRepo = new MusteriRepo();

            if (ModelState.IsValid)
            {
                AdresModel.GuncellemeTarihi = DateTime.Now;
                aRepo.EditAndSave(AdresModel);

                MusteriModel.GuncellemeTarihi = DateTime.Now;
                MusteriModel.AdresId = AdresModel.Id;
                mRepo.EditAndSave(MusteriModel);

                ViewBag.RecordResult = 1;
                ModelState.Clear();

                return RedirectToAction("Index", "Musteri");
            }

            return View("Index");
        }
    }
}