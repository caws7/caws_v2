using CamSistemDataLayer.Enums;
using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using CamSistemWebArayuz.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CamSistemWebArayuz.Controllers
{
    [SessionController]
    [AuthLog(Roles = "TEDARİK")]
    public class TedarikController : Controller
    {
        SiparisRepo siparisRepo;
        TedarikRepo tedarikRepo;
        CamTedarikRepo camTedarikRepo;
        BoyaTedarikRepo boyaTedarikRepo;
        DisTedarikRepo disTedarikRepo;
        private object tedarik;

        // GET: Tedarik
        [AuthLog(Roles = "TEDARİK,GORUNTULEME")]
        public ActionResult Index()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "TedarikSayfasi";
            tedarikRepo = new TedarikRepo();

            return View(tedarikRepo.GetAll());
        }

        [AuthLog(Roles = "TEDARİK,GORUNTULEME")]
        public ActionResult Cam()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "CamTedarikSayfasi";
            siparisRepo = new SiparisRepo();

            //onaylanan siparişlere bilgiler girilecek
            return View(siparisRepo.FindBy(e => e.DurumId == (int)Durumlar.Onaylandı || e.DurumId == (int)Durumlar.ImalataGonderildi || e.DurumId == (int)Durumlar.Sevkiyatta).ToList());
        }

        [AuthLog(Roles = "TEDARİK,GORUNTULEME")]
        public ActionResult Boya()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "BoyaTedarikSayfasi";
            siparisRepo = new SiparisRepo();

            //onaylanan siparişlere bilgiler girilecek
            return View(siparisRepo.FindBy(e => e.DurumId == (int)Durumlar.Onaylandı || e.DurumId == (int)Durumlar.ImalataGonderildi || e.DurumId == (int)Durumlar.Sevkiyatta).ToList());
        }

        [AuthLog(Roles = "TEDARİK,GORUNTULEME")]
        public ActionResult Dis()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "DisTedarikSayfasi";
            siparisRepo = new SiparisRepo();

            //onaylanan siparişlere bilgiler girilecek
            return View(siparisRepo.FindBy(e => e.DurumId == (int)Durumlar.Onaylandı || e.DurumId == (int)Durumlar.ImalataGonderildi || e.DurumId == (int)Durumlar.Sevkiyatta).ToList());
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult TedarikKaydet()
        {
            return PartialView("TedarikKaydet");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult TedarikKaydet(Tedarikci TedarikModel)
        {
            tedarikRepo = new TedarikRepo();
            try
            {
                if (ModelState.IsValid)
                {
                    TedarikModel.KayitTarihi = DateTime.Now;
                    tedarikRepo.AddAndSave(TedarikModel);

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

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult CamTedarikKaydet(long SiparisId)
        {
            TempData["DosyaAds"] = null;
            ViewBag.SiparisId = SiparisId;
            tedarikRepo = new TedarikRepo();
            ViewBag.tedarikciler = tedarikRepo.FindBy(e => e.TedarikciTuru.Equals("Cam"));
            return View("CamTedarikKaydet");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult CamTedarikKaydet(CamTedarik CamTedarikModel)
        {
            siparisRepo = new SiparisRepo();
            camTedarikRepo = new CamTedarikRepo();
            try
            {
                if (ModelState.IsValid)
                {
                    List<string> ads = TempData["DosyaAds"] as List<string>;
                    List<long> ids = new List<long>();
                    DosyaRepo dosyaRepo = new DosyaRepo();
                    if (ads != null)
                    {
                        foreach (var item in ads)
                        {
                            YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.DosyaAdi.Equals(item)).FirstOrDefault();
                            if (yuklenenDosyalar != null)
                                ids.Add(yuklenenDosyalar.Id);
                        }
                        if (ids.Count > 0)
                            CamTedarikModel.DosyaIds = string.Join(", ", ids);
                    }

                    CamTedarikModel.KayitTarihi = DateTime.Now;
                    CamTedarik camModel = camTedarikRepo.SaveAndReturnEntity(CamTedarikModel);

                    Siparis siparis = siparisRepo.FindBy(e => e.Id == camModel.SiparisId).FirstOrDefault();
                    siparis.CamTedarikId = camModel.Id;
                    siparisRepo.EditAndSave(siparis);

                    ViewBag.RecordResult = 1;

                    ModelState.Clear();
                    TempData["DosyaAds"] = null;
                    return RedirectToAction("Index", "Siparis");
                }
                return RedirectToAction("Index", "Siparis");
            }
            catch (Exception)
            {
                ViewBag.RecordResult = 2;
                return RedirectToAction("Index", "Siparis");
            }
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult DisTedarikKaydet(long SiparisId)
        {
            TempData["DosyaAds"] = null;
            ViewBag.SiparisId = SiparisId;
            tedarikRepo = new TedarikRepo();
            ViewBag.tedarikciler = tedarikRepo.FindBy(e => e.TedarikciTuru.Equals("Dis"));
            return View("DisTedarikKaydet");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult DisTedarikKaydet(DisTedarik DisTedarikModel)
        {
            siparisRepo = new SiparisRepo();
            disTedarikRepo = new DisTedarikRepo();
            try
            {
                if (ModelState.IsValid)
                {
                    List<string> ads = TempData["DosyaAds"] as List<string>;
                    List<long> ids = new List<long>();
                    DosyaRepo dosyaRepo = new DosyaRepo();

                    if (ads != null)
                    {
                        foreach (var item in ads)
                        {
                            YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.DosyaAdi.Equals(item)).FirstOrDefault();
                            if (yuklenenDosyalar != null)
                                ids.Add(yuklenenDosyalar.Id);
                        }
                        if (ids.Count > 0)
                            DisTedarikModel.DosyaIds = string.Join(", ", ids);
                    }

                    DisTedarikModel.KayitTarihi = DateTime.Now;
                    DisTedarik disModel = disTedarikRepo.SaveAndReturnEntity(DisTedarikModel);

                    Siparis siparis = siparisRepo.FindBy(e => e.Id == disModel.SiparisId).FirstOrDefault();
                    siparis.DisTedarikId = disModel.Id;
                    siparisRepo.EditAndSave(siparis);

                    ViewBag.RecordResult = 1;

                    ModelState.Clear();
                    TempData["DosyaAds"] = null;

                    return RedirectToAction("Index", "Siparis");
                }
                return RedirectToAction("Index", "Siparis");
            }
            catch (Exception)
            {
                ViewBag.RecordResult = 2;
                return RedirectToAction("Index", "Siparis");
            }
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult BoyaTedarikKaydet(long SiparisId)
        {
            TempData["DosyaAds"] = null;
            ViewBag.SiparisId = SiparisId;
            tedarikRepo = new TedarikRepo();
            ViewBag.tedarikciler = tedarikRepo.FindBy(e => e.TedarikciTuru.Equals("Boya"));
            return View("BoyaTedarikKaydet");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult BoyaTedarikKaydet(BoyaTedarik BoyaTedarikModel)
        {
            siparisRepo = new SiparisRepo();
            boyaTedarikRepo = new BoyaTedarikRepo();
            try
            {
                if (ModelState.IsValid)
                {
                    List<string> ads = TempData["DosyaAds"] as List<string>;
                    List<long> ids = new List<long>();
                    DosyaRepo dosyaRepo = new DosyaRepo();

                    if (ads != null)
                    {
                        foreach (var item in ads)
                        {
                            YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.DosyaAdi.Equals(item)).FirstOrDefault();
                            if (yuklenenDosyalar != null)
                                ids.Add(yuklenenDosyalar.Id);
                        }
                        if (ids.Count > 0)
                            BoyaTedarikModel.DosyaIds = string.Join(", ", ids);
                    }
                    BoyaTedarikModel.KayitTarihi = DateTime.Now;
                    BoyaTedarik boyaModel = boyaTedarikRepo.SaveAndReturnEntity(BoyaTedarikModel);

                    Siparis siparis = siparisRepo.FindBy(e => e.Id == boyaModel.SiparisId).FirstOrDefault();
                    siparis.BoyaTedarikId = boyaModel.Id;
                    siparisRepo.EditAndSave(siparis);

                    ViewBag.RecordResult = 1;

                    ModelState.Clear();
                    TempData["DosyaAds"] = null;

                    return RedirectToAction("Index", "Siparis");
                }
                return RedirectToAction("Index", "Siparis");
            }
            catch (Exception)
            {
                ViewBag.RecordResult = 2;
                return RedirectToAction("Index", "Siparis");
            }
        }

        [AuthLog(Roles = "SILME")]
        public ActionResult TedarikSil(int Id)
        {
            try
            {
                tedarikRepo = new TedarikRepo();
                tedarikRepo.DeleteAndSave(tedarikRepo.FindBy(e => e.Id == Id).FirstOrDefault());

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "SILME")]
        public ActionResult CamTedarikSil(long Id)
        {
            try
            {
                DosyaRepo dosyaRepo = new DosyaRepo();
                siparisRepo = new SiparisRepo();
                camTedarikRepo = new CamTedarikRepo();
                CamTedarik camTedarik = camTedarikRepo.FindBy(e => e.SiparisId == Id).FirstOrDefault();

                if (camTedarik != null && camTedarik.DosyaIds != null)
                {
                    string[] ids = camTedarik.DosyaIds.Split(',');
                    foreach (var item in ids)
                    {
                        long dosyaId = Convert.ToInt64(item.Trim());
                        YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.Id == dosyaId).FirstOrDefault();
                        //fiziksel dosyayı sil
                        System.IO.File.Delete(Server.MapPath("~" + yuklenenDosyalar.DosyaYolu));
                        //tablodan sil
                        dosyaRepo.DeleteAndSave(yuklenenDosyalar);
                    }
                }

                camTedarikRepo.DeleteAndSave(camTedarik);
                Siparis siparis = siparisRepo.FindBy(e => e.Id == Id).FirstOrDefault();
                siparis.CamTedarikId = null;
                siparisRepo.EditAndSave(siparis);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "SILME")]
        public ActionResult BoyaTedarikSil(long Id)
        {
            try
            {
                DosyaRepo dosyaRepo = new DosyaRepo();
                siparisRepo = new SiparisRepo();
                boyaTedarikRepo = new BoyaTedarikRepo();
                BoyaTedarik boyaTedarik = boyaTedarikRepo.FindBy(e => e.SiparisId == Id).FirstOrDefault();

                if (boyaTedarik != null && boyaTedarik.DosyaIds != null)
                {
                    string[] ids = boyaTedarik.DosyaIds.Split(',');
                    foreach (var item in ids)
                    {
                        long dosyaId = Convert.ToInt64(item.Trim());
                        YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.Id == dosyaId).FirstOrDefault();
                        //fiziksel dosyayı sil
                        System.IO.File.Delete(Server.MapPath("~" + yuklenenDosyalar.DosyaYolu));
                        //tablodan sil
                        dosyaRepo.DeleteAndSave(yuklenenDosyalar);
                    }
                }

                boyaTedarikRepo.DeleteAndSave(boyaTedarik);
                Siparis siparis = siparisRepo.FindBy(e => e.Id == Id).FirstOrDefault();
                siparis.BoyaTedarikId = null;
                siparisRepo.EditAndSave(siparis);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "SILME")]
        public ActionResult DisTedarikSil(long Id)
        {
            try
            {
                DosyaRepo dosyaRepo = new DosyaRepo();
                siparisRepo = new SiparisRepo();
                disTedarikRepo = new DisTedarikRepo();
                DisTedarik disTedarik = disTedarikRepo.FindBy(e => e.SiparisId == Id).FirstOrDefault();

                if (disTedarik != null && disTedarik.DosyaIds != null)
                {
                    string[] ids = disTedarik.DosyaIds.Split(',');
                    foreach (var item in ids)
                    {
                        long dosyaId = Convert.ToInt64(item.Trim());
                        YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.Id == dosyaId).FirstOrDefault();
                        //fiziksel dosyayı sil
                        System.IO.File.Delete(Server.MapPath("~" + yuklenenDosyalar.DosyaYolu));
                        //tablodan sil
                        dosyaRepo.DeleteAndSave(yuklenenDosyalar);
                    }
                }

                disTedarikRepo.DeleteAndSave(disTedarik);
                Siparis siparis = siparisRepo.FindBy(e => e.Id == Id).FirstOrDefault();
                siparis.DisTedarikId = null;
                siparisRepo.EditAndSave(siparis);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult TedarikDuzenle(int Id)
        {
            tedarikRepo = new TedarikRepo();
            Tedarikci tedarikci = tedarikRepo.FindBy(e => e.Id == Id).FirstOrDefault();

            return View(tedarikci);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult TedarikDuzenle(Tedarikci TedarikModel)
        {
            tedarikRepo = new TedarikRepo();
            if (ModelState.IsValid)
            {
                tedarikRepo.EditAndSave(TedarikModel);
                ViewBag.RecordResult = 1;
                ModelState.Clear();

                return RedirectToAction("Index", "Tedarik");
            }

            return View("Index");
        }

        [HttpGet]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult CamTedarikDuzenle(int SiparisId)
        {
            tedarikRepo = new TedarikRepo();
            camTedarikRepo = new CamTedarikRepo();

            ViewBag.tedarikciler = tedarikRepo.FindBy(e => e.TedarikciTuru.Equals("Cam"));
            CamTedarik camTedarik = camTedarikRepo.FindBy(e => e.SiparisId == SiparisId).FirstOrDefault();

            //viewbag ile yüklenen dosya listesi tutulacak arayüzde tablo gösterilecek ve indirme/silme işlemleri olacak
            DosyaRepo dosyaRepo = new DosyaRepo();
            List<YuklenenDosyalar> dosyaList = new List<YuklenenDosyalar>();
            if (camTedarik.DosyaIds != null)
            {
                string[] dosyaIds = camTedarik.DosyaIds.Split(',');
                foreach (var item in dosyaIds)
                {
                    long dosyaId = Convert.ToInt64(item.Trim());
                    YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.Id == dosyaId).FirstOrDefault();
                    if (yuklenenDosyalar != null)
                        dosyaList.Add(yuklenenDosyalar);
                }
            }
            ViewBag.yuklenenDosyaList = dosyaList;

            return View(camTedarik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult CamTedarikDuzenle(CamTedarik CamTedarikModel)
        {
            camTedarikRepo = new CamTedarikRepo();
            if (ModelState.IsValid)
            {
                List<string> ads = TempData["DosyaAds"] as List<string>;
                List<long> ids = new List<long>();
                DosyaRepo dosyaRepo = new DosyaRepo();
                if (ads != null)
                {
                    foreach (var item in ads)
                    {
                        YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.DosyaAdi.Equals(item)).FirstOrDefault();
                        if (yuklenenDosyalar != null)
                            ids.Add(yuklenenDosyalar.Id);
                    }
                }

                if (CamTedarikModel.DosyaIds != null)
                {
                    //mevcutta yüklenmiş dosya varsa onları da listeye ekle
                    string[] dosyaIds = CamTedarikModel.DosyaIds.Split(',');
                    if (dosyaIds.Length > 0)
                    {
                        foreach (var item in dosyaIds)
                        {
                            long _dosyaId = Convert.ToInt64(item.Trim());
                            ids.Add(_dosyaId);
                        }
                    }
                }
                if (ids.Count > 0)
                    CamTedarikModel.DosyaIds = string.Join(", ", ids);

                camTedarikRepo.EditAndSave(CamTedarikModel);
                ViewBag.RecordResult = 1;
                ModelState.Clear();
                TempData["DosyaAds"] = null;

                return RedirectToAction("Cam", "Tedarik");
            }

            return View("Cam");
        }

        [HttpGet]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult DisTedarikDuzenle(int SiparisId)
        {
            tedarikRepo = new TedarikRepo();
            disTedarikRepo = new DisTedarikRepo();

            ViewBag.tedarikciler = tedarikRepo.FindBy(e => e.TedarikciTuru.Equals("Dis"));
            DisTedarik disTedarik = disTedarikRepo.FindBy(e => e.SiparisId == SiparisId).FirstOrDefault();

            //viewbag ile yüklenen dosya listesi tutulacak arayüzde tablo gösterilecek ve indirme/silme işlemleri olacak
            DosyaRepo dosyaRepo = new DosyaRepo();
            List<YuklenenDosyalar> dosyaList = new List<YuklenenDosyalar>();
            if (disTedarik.DosyaIds != null)
            {
                string[] dosyaIds = disTedarik.DosyaIds.Split(',');
                foreach (var item in dosyaIds)
                {
                    long dosyaId = Convert.ToInt64(item.Trim());
                    YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.Id == dosyaId).FirstOrDefault();
                    if (yuklenenDosyalar != null)
                        dosyaList.Add(yuklenenDosyalar);
                }
            }
            ViewBag.yuklenenDosyaList = dosyaList;

            return View(disTedarik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult DisTedarikDuzenle(DisTedarik DisTedarikModel)
        {
            disTedarikRepo = new DisTedarikRepo();
            if (ModelState.IsValid)
            {
                List<string> ads = TempData["DosyaAds"] as List<string>;
                List<long> ids = new List<long>();
                DosyaRepo dosyaRepo = new DosyaRepo();
                if (ads != null)
                {
                    foreach (var item in ads)
                    {
                        YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.DosyaAdi.Equals(item)).FirstOrDefault();
                        if (yuklenenDosyalar != null)
                            ids.Add(yuklenenDosyalar.Id);
                    }
                }

                if (DisTedarikModel.DosyaIds != null)
                {
                    //mevcutta yüklenmiş dosya varsa onları da listeye ekle
                    string[] dosyaIds = DisTedarikModel.DosyaIds.Split(',');
                    if (dosyaIds.Length > 0)
                    {
                        foreach (var item in dosyaIds)
                        {
                            long _dosyaId = Convert.ToInt64(item.Trim());
                            ids.Add(_dosyaId);
                        }
                    }
                }
                if (ids.Count > 0)
                    DisTedarikModel.DosyaIds = string.Join(", ", ids);

                disTedarikRepo.EditAndSave(DisTedarikModel);
                ViewBag.RecordResult = 1;
                ModelState.Clear();
                TempData["DosyaAds"] = null;

                return RedirectToAction("Dis", "Tedarik");
            }

            return RedirectToAction("Dis", "Tedarik");
        }

        [HttpGet]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult BoyaTedarikDuzenle(int SiparisId)
        {
            tedarikRepo = new TedarikRepo();
            boyaTedarikRepo = new BoyaTedarikRepo();

            ViewBag.tedarikciler = tedarikRepo.FindBy(e => e.TedarikciTuru.Equals("Boya"));
            BoyaTedarik boyaTedarik = boyaTedarikRepo.FindBy(e => e.SiparisId == SiparisId).FirstOrDefault();

            //viewbag ile yüklenen dosya listesi tutulacak arayüzde tablo gösterilecek ve indirme/silme işlemleri olacak
            DosyaRepo dosyaRepo = new DosyaRepo();
            List<YuklenenDosyalar> dosyaList = new List<YuklenenDosyalar>();

            if (boyaTedarik.DosyaIds != null)
            {
                string[] dosyaIds = boyaTedarik.DosyaIds.Split(',');
                foreach (var item in dosyaIds)
                {
                    long dosyaId = Convert.ToInt64(item.Trim());
                    YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.Id == dosyaId).FirstOrDefault();
                    if (yuklenenDosyalar != null)
                        dosyaList.Add(yuklenenDosyalar);
                }
            }
            ViewBag.yuklenenDosyaList = dosyaList;

            return View(boyaTedarik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult BoyaTedarikDuzenle(BoyaTedarik BoyaTedarikModel)
        {
            boyaTedarikRepo = new BoyaTedarikRepo();
            if (ModelState.IsValid)
            {
                List<string> ads = TempData["DosyaAds"] as List<string>;
                List<long> ids = new List<long>();
                DosyaRepo dosyaRepo = new DosyaRepo();
                if (ads != null)
                {
                    foreach (var item in ads)
                    {
                        YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.DosyaAdi.Equals(item)).FirstOrDefault();
                        if (yuklenenDosyalar != null)
                            ids.Add(yuklenenDosyalar.Id);
                    }
                }

                if (BoyaTedarikModel.DosyaIds != null)
                {
                    //mevcutta yüklenmiş dosya varsa onları da listeye ekle
                    string[] dosyaIds = BoyaTedarikModel.DosyaIds.Split(',');
                    if (dosyaIds.Length > 0)
                    {
                        foreach (var item in dosyaIds)
                        {
                            long _dosyaId = Convert.ToInt64(item.Trim());
                            ids.Add(_dosyaId);
                        }
                    }

                }
                if (ids.Count > 0)
                    BoyaTedarikModel.DosyaIds = string.Join(", ", ids);

                boyaTedarikRepo.EditAndSave(BoyaTedarikModel);
                ViewBag.RecordResult = 1;
                ModelState.Clear();
                TempData["DosyaAds"] = null;

                return RedirectToAction("Boya", "Tedarik");
            }

            return View("Boya");
        }

        [HttpPost]
        [AuthLog(Roles = "SILME")]
        public JsonResult DosyaSil(long DosyaId, long TedarikTurId, string TedarikTur)
        {
            DosyaRepo dosyaRepo = new DosyaRepo();
            List<long> ids = new List<long>();
            YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.Id == DosyaId).FirstOrDefault();
            System.IO.File.Delete(Server.MapPath("~/Assets/yuklenenler/" + yuklenenDosyalar.DosyaAdi));
            dosyaRepo.DeleteAndSave(yuklenenDosyalar);

            if (TedarikTur.Equals("Boya"))
            {
                boyaTedarikRepo = new BoyaTedarikRepo();
                BoyaTedarik boyaTedarik = boyaTedarikRepo.FindBy(e => e.Id == TedarikTurId).FirstOrDefault();
                string[] boyaDosyaIds = boyaTedarik.DosyaIds.Split(',');
                foreach (var item in boyaDosyaIds)
                {
                    long _dosyaId = Convert.ToInt64(item.Trim());
                    if (DosyaId != _dosyaId)
                        ids.Add(_dosyaId);
                }

                if (ids.Count > 0)
                    boyaTedarik.DosyaIds = string.Join(", ", ids);
                else
                    boyaTedarik.DosyaIds = null;
                boyaTedarikRepo.EditAndSave(boyaTedarik);
            }
            else if (TedarikTur.Equals("Cam"))
            {
                camTedarikRepo = new CamTedarikRepo();
                CamTedarik camTedarik = camTedarikRepo.FindBy(e => e.Id == TedarikTurId).FirstOrDefault();
                string[] boyaDosyaIds = camTedarik.DosyaIds.Split(',');
                foreach (var item in boyaDosyaIds)
                {
                    long _dosyaId = Convert.ToInt64(item.Trim());
                    if (DosyaId != _dosyaId)
                        ids.Add(_dosyaId);
                }

                if (ids.Count > 0)
                    camTedarik.DosyaIds = string.Join(", ", ids);
                else
                    camTedarik.DosyaIds = null;
                camTedarikRepo.EditAndSave(camTedarik);
            }
            else if (TedarikTur.Equals("Dis"))
            {
                disTedarikRepo = new DisTedarikRepo();
                DisTedarik disTedarik = disTedarikRepo.FindBy(e => e.Id == TedarikTurId).FirstOrDefault();
                string[] boyaDosyaIds = disTedarik.DosyaIds.Split(',');
                foreach (var item in boyaDosyaIds)
                {
                    long _dosyaId = Convert.ToInt64(item.Trim());
                    if (DosyaId != _dosyaId)
                        ids.Add(_dosyaId);
                }
                if (ids.Count > 0)
                    disTedarik.DosyaIds = string.Join(", ", ids);
                else
                    disTedarik.DosyaIds = null;
                disTedarikRepo.EditAndSave(disTedarik);
            }
            else if (TedarikTur.Equals("Sevkiyat"))
            {
                SevkiyatRepo sevkiyatRepo = new SevkiyatRepo();
                Sevkiyat sevkiyat = sevkiyatRepo.FindBy(e => e.Id == TedarikTurId).FirstOrDefault();
                string[] dosyaIds = sevkiyat.DosyaIds.Split(',');
                foreach (var item in dosyaIds)
                {
                    long _dosyaId = Convert.ToInt64(item.Trim());
                    if (DosyaId != _dosyaId)
                        ids.Add(_dosyaId);
                }
                if (ids.Count > 0)
                    sevkiyat.DosyaIds = string.Join(", ", ids);
                else
                    sevkiyat.DosyaIds = null;
                sevkiyatRepo.EditAndSave(sevkiyat);
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthLog(Roles = "DUZENLEME,YENIKAYIT")]
        public JsonResult DosyaYuklemeKaydet()
        {
            DosyaRepo dosyaRepo = new DosyaRepo();
            bool isSavedSuccessfully = true;
            string fName = "";
            List<string> dosyaAds = TempData["DosyaAds"] as List<string>;
            if (dosyaAds == null)
                dosyaAds = new List<string>();
            try
            {
                foreach (string fileName in Request.Files)
                {
                    Guid uniqId = Guid.NewGuid();
                    HttpPostedFileBase file = Request.Files[fileName];
                    string[] fileName1 = file.FileName.Split('.');
                    fName = uniqId.ToString() + "." + fileName1[1];
                    if (file != null && file.ContentLength > 0)
                    {
                        var path = Path.Combine(Server.MapPath("~/Assets/yuklenenler"));
                        string pathString = Path.Combine(path.ToString());
                        bool isExists = Directory.Exists(pathString);
                        if (!isExists) Directory.CreateDirectory(pathString);
                        var uploadpath = string.Format("{0}\\{1}", pathString, fName);
                        file.SaveAs(uploadpath);

                        YuklenenDosyalar dosya = dosyaRepo.SaveAndReturnEntity(new YuklenenDosyalar
                        {
                            DosyaAdi = fName,
                            DosyaYolu = "/Assets/yuklenenler/" + fName,
                            DosyaUzantisi = file.ContentType,
                            YuklenmeTarihi = DateTime.Now

                        });

                        dosyaAds.Add(dosya.DosyaAdi);
                    }
                }

                TempData["DosyaAds"] = dosyaAds;
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;

                return Json(new { Message = "Error in saving file. Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            if (isSavedSuccessfully)
            {
                return Json(fName);
            }
            else
            {
                return Json(new { Message = "Error in saving file" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AuthLog(Roles = "SILME")]
        public JsonResult DosyaYuklemeSil(string ImageName)
        {
            DosyaRepo dosyaRepo = new DosyaRepo();
            bool isSavedSuccessfully = true;
            try
            {
                List<string> ads = TempData["DosyaAds"] as List<string>;

                if (ads.Contains(ImageName))
                {
                    System.IO.File.Delete(Server.MapPath("~/Assets/yuklenenler/" + ImageName));
                    YuklenenDosyalar yuklenenDosyalar = dosyaRepo.FindBy(e => e.DosyaAdi.Equals(ImageName)).FirstOrDefault();
                    dosyaRepo.DeleteAndSave(yuklenenDosyalar);
                    ads.Remove(ImageName);
                }

                TempData["DosyaAds"] = ads;
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
                return null;
            }
            if (isSavedSuccessfully)
            {
                return Json(ImageName);
            }
            else
            {
                return Json("EX");
            }
        }

    }
}