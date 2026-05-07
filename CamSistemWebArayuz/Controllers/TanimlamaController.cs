using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using CamSistemWebArayuz.Attributes;
using CamSistemWebArayuz.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CamSistemWebArayuz.Controllers
{
    [SessionController]
    [AuthLog(Roles = "TANİMLAMA")]
    public class TanimlamaController : Controller
    {
        RenkRepo renkRepo;
        SistemRepo sistemRepo;
        SistemTurRepo sistemTurRepo;
        AltSistemRepo altSistemRepo;
        SistemAltSistemJoinRepo sjRepo;
        ProfilRepo profilRepo;
        AksesuarRepo aksesuarRepo;
        CamKombinasyonRepo camKombinasyonRepo;
        SabitRepo sabitRepo;

        private void EnsureMaliyetSabitleri()
        {
            var gerekliSabitler = new List<Tuple<int?, string, int>>
            {
                Tuple.Create<int?, string, int>(2, "ALÜMİNYUM BİRİM FİYAT", 0),
                Tuple.Create<int?, string, int>(3, "İMALAT BEDELİ", 0),
                Tuple.Create<int?, string, int>(4, "SARF MALZEME BEDELİ", 0),
                Tuple.Create<int?, string, int>(5, "KAR PAYI ORANI", 0),
                Tuple.Create<int?, string, int>(8, "CAM BİRİM FİYAT", 0),
                Tuple.Create<int?, string, int>(9, "AKSESUAR SETİ BİRİM FİYAT", 0),
                Tuple.Create<int?, string, int>(10, "KAR PAYI BİRİM FİYAT", 0)
            };

            var tumSabitler = sabitRepo.GetAll().ToList();
            foreach (var sabit in gerekliSabitler)
            {
                var aciklamaIle = tumSabitler.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Aciklama) &&
                                                                   x.Aciklama.Trim().Equals(sabit.Item2, StringComparison.OrdinalIgnoreCase));
                var idIle = sabit.Item1.HasValue ? tumSabitler.FirstOrDefault(x => x.Id == sabit.Item1.Value) : null;
                var mevcut = aciklamaIle ?? idIle;

                if (mevcut == null)
                {
                    var yeni = new Sabitler { Aciklama = sabit.Item2, SabitDeger = sabit.Item3 };
                    sabitRepo.AddAndSave(yeni);
                    tumSabitler.Add(yeni);
                }
                else if (string.IsNullOrWhiteSpace(mevcut.Aciklama))
                {
                    mevcut.Aciklama = sabit.Item2;
                    sabitRepo.EditAndSave(mevcut);
                }
            }
        }

        // GET: Tanimlama
        [AuthLog(Roles = "TANİMLAMA,GORUNTULEME")]
        public ActionResult Renk()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "TanimlamaSayfasiRenk";
            renkRepo = new RenkRepo();

            return View(renkRepo.FindBy(x => x.AktifMi == true));
        }

        [AuthLog(Roles = "TANİMLAMA,GORUNTULEME")]
        public ActionResult Sistem()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "TanimlamaSayfasiSistem";

            TanimalamalarModel multiModel = new TanimalamalarModel();
            sistemRepo = new SistemRepo();
            sistemTurRepo = new SistemTurRepo();
            altSistemRepo = new AltSistemRepo();
            sjRepo = new SistemAltSistemJoinRepo();

            ViewBag.Sistemler = sistemRepo.FindBy(x => x.AktifMi == true);
            ViewBag.AltSistemler = altSistemRepo.FindBy(x => x.AktifMi == true);
            ViewBag.SistemTurleri = sistemTurRepo.FindBy(x => x.AktifMi == true);
            multiModel.SistemJoinModel = sjRepo.FindBy(x => x.AktifMi == true);
            multiModel.SistemModel = sistemRepo.FindBy(x => x.AktifMi == true);
            multiModel.SistemTurModel = sistemTurRepo.FindBy(x => x.AktifMi == true);
            multiModel.AltSistemModel = altSistemRepo.FindBy(x => x.AktifMi == true);

            return View(multiModel);
        }

        [AuthLog(Roles = "TANİMLAMA,GORUNTULEME")]
        public ActionResult Profil()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "TanimlamaSayfasiProfil";
            profilRepo = new ProfilRepo();

            return View(profilRepo.GetAll().ToList().Where(e => e.ProfilKodu.ToString().Count(c => c == '-') < 2).ToList());
        }

        [AuthLog(Roles = "TANİMLAMA,GORUNTULEME")]
        public ActionResult Aksesuar()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "TanimlamaSayfasiAksesuar";
            aksesuarRepo = new AksesuarRepo();

            return View(aksesuarRepo.FindBy(x => x.AktifMi == true));
        }

        [AuthLog(Roles = "TANİMLAMA,GORUNTULEME")]
        public ActionResult Cam()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "TanimlamaSayfasiCam";
            camKombinasyonRepo = new CamKombinasyonRepo();

            return View(camKombinasyonRepo.GetAll());
        }

        [AuthLog(Roles = "TANİMLAMA,GORUNTULEME")]
        public ActionResult Sabitler()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "TanimlamaSayfasiSabit";
            sabitRepo = new SabitRepo();
            EnsureMaliyetSabitleri();

            return View(sabitRepo.GetAll());
        }

        [HttpPost]
        [AuthLog(Roles = "DUZENLEME")]
        public JsonResult SabitGuncelle(int SabitId, int SabitDeger)
        {
            sabitRepo = new SabitRepo();
            Sabitler sabit = sabitRepo.FindBy(e => e.Id == SabitId).FirstOrDefault();
            sabit.SabitDeger = SabitDeger;
            sabitRepo.EditAndSave(sabit);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthLog(Roles = "DUZENLEME")]
        public JsonResult YeniCam(string[] CamBilgi)
        {
            camKombinasyonRepo = new CamKombinasyonRepo();
            CamKombinasyon cam = new CamKombinasyon();

            cam.Kombinasyon = CamBilgi[0];
            cam.Birim = CamBilgi[1];
            cam.BirimFiyat = 0;
            camKombinasyonRepo.AddAndSave(cam);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthLog(Roles = "DUZENLEME")]
        public JsonResult CamDuzenle(int Id, string[] CamBilgi)
        {
            camKombinasyonRepo = new CamKombinasyonRepo();

            CamKombinasyon cam = camKombinasyonRepo.FindBy(e => e.Id == Id).FirstOrDefault();
            cam.Kombinasyon = CamBilgi[0];
            cam.Birim = CamBilgi[1];
            camKombinasyonRepo.EditAndSave(cam);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthLog(Roles = "SILME")]
        public JsonResult CamSil(int Id)
        {
            camKombinasyonRepo = new CamKombinasyonRepo();

            CamKombinasyon cam = camKombinasyonRepo.FindBy(e => e.Id == Id).FirstOrDefault();
            camKombinasyonRepo.DeleteAndSave(cam);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [AuthLog(Roles = "SILME")]
        public ActionResult AksesuarSil(int Id)
        {
            try
            {
                StokAksesuarRepo stokAksesuarRepo = new StokAksesuarRepo();
                aksesuarRepo = new AksesuarRepo();
                List<StokAksesuar> stokAksesuarList = stokAksesuarRepo.FindBy(e => e.AksesuarId == Id).ToList();
                foreach (var item in stokAksesuarList)
                {
                    stokAksesuarRepo.DeleteAndSave(item);
                }

                Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == Id).FirstOrDefault();
                //if (aksesuar.AksesuarGorsel != null && aksesuar.AksesuarGorsel != "")
                //{
                //    System.IO.File.Delete(Server.MapPath("~/images/aksesuaricons/" + aksesuar.AksesuarGorsel));
                //}
                aksesuar.AktifMi = false;
                aksesuarRepo.EditAndSave(aksesuar);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult YeniAksesuar()
        {
            aksesuarRepo = new AksesuarRepo();
            //string[] splitNo = aksesuarRepo.FindBy(e => e.AksesuarKodu.Contains("AS-")).ToList().OrderByDescending(e => e.AksesuarKodu).FirstOrDefault().AksesuarKodu.Split('-');
            //ViewBag.AksesuarKod = splitNo[0] + "-" + Convert.ToString(Convert.ToInt32(splitNo[1]) + 1);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult YeniAksesuar(Aksesuar aksesuar, string ImageName, string AksesuarTuru)
        {
            aksesuarRepo = new AksesuarRepo();

            if (AksesuarTuru.Equals("tur_motor"))
            {
                aksesuar.Motor = true;
                aksesuar.Kumanda = null;
            }
            else if (AksesuarTuru.Equals("tur_kumanda"))
            {
                aksesuar.Kumanda = true;
                aksesuar.Motor = null;
            }

            aksesuar.AksesuarGorsel = ImageName;
            if (aksesuar.BirimFiyat == null)
                aksesuar.BirimFiyat = 0;
            aksesuar.AktifMi = true;
            aksesuarRepo.AddAndSave(aksesuar);

            return RedirectToAction("Aksesuar");
        }

        [HttpGet]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult AksesuarDuzenle(int Id)
        {
            aksesuarRepo = new AksesuarRepo();

            return View(aksesuarRepo.FindBy(e => e.Id == Id && e.AktifMi == true).FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult AksesuarDuzenle(Aksesuar aksesuar, string AksesuarTuru)
        {
            aksesuarRepo = new AksesuarRepo();

            if (AksesuarTuru.Equals("tur_motor"))
            {
                aksesuar.Motor = true;
                aksesuar.Kumanda = null;
            }
            else if (AksesuarTuru.Equals("tur_kumanda"))
            {
                aksesuar.Kumanda = true;
                aksesuar.Motor = null;
            }

            string dosyaAd = TempData["YuklenenDosyaAd"] as string;
            if (aksesuar.AksesuarGorsel != null && aksesuar.AksesuarGorsel != "" && dosyaAd != null && aksesuar.AksesuarGorsel != dosyaAd)
            {
                System.IO.File.Delete(Server.MapPath("~/images/aksesuaricons/" + aksesuar.AksesuarGorsel));
                aksesuar.AksesuarGorsel = dosyaAd;
            }

            aksesuarRepo.EditAndSave(aksesuar);
            TempData["YuklenenDosyaAd"] = null;

            return RedirectToAction("Aksesuar");
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult YeniProfil()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult YeniProfil(Profil profil, string ImageName)
        {
            profilRepo = new ProfilRepo();
            profil.ProfilFoto = ImageName;
            profil.AktifMi = true;
            Profil prof = profilRepo.SaveAndReturnEntity(profil);

            //yeni profil tanımlamalarında stok için boş kayıt atılacak
            StokRepo stokRepo = new StokRepo();
            ProfilBoyRepo profilBoyRepo = new ProfilBoyRepo();
            List<ProfilBoy> profilBoys = profilBoyRepo.GetAll().ToList();
            foreach (var boy in profilBoys)
            {
                Stok stok = new Stok();
                stok.ProfilId = prof.Id;
                stok.ProfilBoyId = boy.Id;
                stok.StokAdet = 0;

                stokRepo.AddAndSave(stok);
            }

            return RedirectToAction("Profil");
        }

        [HttpGet]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult ProfilDuzenle(int Id)
        {
            profilRepo = new ProfilRepo();

            return View(profilRepo.FindBy(e => e.Id == Id && e.AktifMi == true).FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult ProfilDuzenle(Profil profil)
        {
            string dosyaAd = TempData["YuklenenDosyaAd"] as string;
            profilRepo = new ProfilRepo();

            if (profil.ProfilFoto != null && profil.ProfilFoto != "" && dosyaAd != null && profil.ProfilFoto != dosyaAd)
            {
                System.IO.File.Delete(Server.MapPath("~/images/profilicons/" + profil.ProfilFoto));
                profil.ProfilFoto = dosyaAd;
            }
            profilRepo.EditAndSave(profil);
            TempData["YuklenenDosyaAd"] = null;

            return RedirectToAction("Profil");
        }

        [AuthLog(Roles = "SILME")]
        public ActionResult ProfilSil(int Id)
        {
            try
            {
                StokRepo stokRepo = new StokRepo();
                List<Stok> stokList = stokRepo.FindBy(e => e.ProfilId == Id).ToList();
                foreach (var item in stokList)
                {
                    stokRepo.DeleteAndSave(item);
                }

                profilRepo = new ProfilRepo();
                Profil profil = profilRepo.FindBy(e => e.Id == Id).FirstOrDefault();
                profil.AktifMi = false;
                //if (profil.ProfilFoto != null && profil.ProfilFoto != "")
                //{
                //    System.IO.File.Delete(Server.MapPath("~/images/profilicons/" + profil.ProfilFoto));
                //}
                profilRepo.EditAndSave(profil);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult YeniRenkEkle(string[] RenkBilgisi)
        {
            try
            {
                renkRepo = new RenkRepo();
                Renk renk = new Renk();
                renk.RenkAdi = RenkBilgisi[0];
                renk.RenkKodu = RenkBilgisi[1];
                renk.AktifMi = true;
                renkRepo.AddAndSave(renk);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "SILME")]
        public ActionResult RenkSil(int RenkId)
        {
            try
            {
                renkRepo = new RenkRepo();
                Renk renk = renkRepo.FindBy(e => e.Id == RenkId).FirstOrDefault();
                renk.AktifMi = false;
                renkRepo.EditAndSave(renk);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult RenkDuzenle(int RenkId, string[] RenkBilgisi)
        {
            try
            {
                renkRepo = new RenkRepo();
                Renk renk = renkRepo.FindBy(e => e.Id == RenkId).FirstOrDefault();
                renk.RenkAdi = RenkBilgisi[0];
                renk.RenkKodu = RenkBilgisi[1];
                renkRepo.EditAndSave(renk);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AuthLog(Roles = "YENIKAYIT")]
        public JsonResult YeniSistem(string SistemAdi, string SistemTur)
        {
            sistemRepo = new SistemRepo();
            sistemTurRepo = new SistemTurRepo();
            altSistemRepo = new AltSistemRepo();

            switch (SistemTur)
            {
                case "Sistem":
                    Sistem sistemEnt = new Sistem();
                    sistemEnt.SistemAdi = SistemAdi;
                    sistemEnt.AktifMi = true;
                    sistemRepo.AddAndSave(sistemEnt);
                    break;
                case "SistemTur":
                    SistemTur sistemTurEnt = new SistemTur();
                    sistemTurEnt.TurAdi = SistemAdi;
                    sistemTurEnt.AktifMi = true;
                    sistemTurRepo.AddAndSave(sistemTurEnt);
                    break;
                case "AltSistem":
                    AltSistem altSistemEnt = new AltSistem();
                    altSistemEnt.AltSistemAdi = SistemAdi;
                    altSistemEnt.AktifMi = true;
                    altSistemRepo.AddAndSave(altSistemEnt);
                    break;
                default:
                    break;
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthLog(Roles = "DUZENLEME")]
        public JsonResult SistemDuzenle(int SistemId, string SistemAdi, string SistemTur)
        {
            sistemRepo = new SistemRepo();
            sistemTurRepo = new SistemTurRepo();
            altSistemRepo = new AltSistemRepo();

            switch (SistemTur)
            {
                case "Sistem":
                    Sistem sistemEnt = sistemRepo.FindBy(e => e.Id == SistemId && e.AktifMi == true).FirstOrDefault();
                    sistemEnt.SistemAdi = SistemAdi;
                    sistemRepo.EditAndSave(sistemEnt);
                    break;
                case "SistemTur":
                    SistemTur sistemTurEnt = sistemTurRepo.FindBy(e => e.Id == SistemId && e.AktifMi == true).FirstOrDefault();
                    sistemTurEnt.TurAdi = SistemAdi;
                    sistemTurRepo.EditAndSave(sistemTurEnt);
                    break;
                case "AltSistem":
                    AltSistem altSistemEnt = altSistemRepo.FindBy(e => e.Id == SistemId && e.AktifMi == true).FirstOrDefault();
                    altSistemEnt.AltSistemAdi = SistemAdi;
                    altSistemRepo.EditAndSave(altSistemEnt);
                    break;
                default:
                    break;
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthLog(Roles = "SILME")]
        public JsonResult SistemSil(int SistemId, string SistemTur)
        {
            sistemRepo = new SistemRepo();
            sistemTurRepo = new SistemTurRepo();
            altSistemRepo = new AltSistemRepo();

            switch (SistemTur)
            {
                case "Sistem":
                    Sistem sistem = sistemRepo.FindBy(e => e.Id == SistemId).FirstOrDefault();
                    sistem.AktifMi = false;
                    sistemRepo.EditAndSave(sistem);
                    break;
                case "SistemTur":
                    SistemTur sistemTur = sistemTurRepo.FindBy(e => e.Id == SistemId).FirstOrDefault();
                    sistemTur.AktifMi = false;
                    sistemTurRepo.EditAndSave(sistemTur);
                    break;
                case "AltSistem":
                    AltSistem altSistem = altSistemRepo.FindBy(e => e.Id == SistemId).FirstOrDefault();
                    altSistem.AktifMi = false;
                    altSistemRepo.EditAndSave(altSistem);
                    break;
                default:
                    break;
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult SistemBaglamaKaydet(int sistemler, int sistemTurleri, int[] altSistemler)
        {
            sjRepo = new SistemAltSistemJoinRepo();

            foreach (var item in altSistemler)
            {
                SistemAltSistemJoin model = new SistemAltSistemJoin();
                model.SistemId = sistemler;
                model.SistemTurId = sistemTurleri;
                model.AltSistemId = item;
                model.BirimFiyat = 0;
                model.AktifMi = true;

                sjRepo.AddAndSave(model);
            }

            return RedirectToAction("Sistem");
        }

        [HttpPost]
        [AuthLog(Roles = "SILME")]
        public JsonResult BaglamaSil(int JoinId)
        {
            sjRepo = new SistemAltSistemJoinRepo();
            SistemAltSistemJoin sistemAltSistemJoin = sjRepo.FindBy(e => e.Id == JoinId).FirstOrDefault();
            sistemAltSistemJoin.AktifMi = false;
            sjRepo.EditAndSave(sistemAltSistemJoin);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DosyaYuklemeKaydet(string ImageType)
        {
            if (!ImageType.Equals("undefined"))
            {
                string image = "";
                bool isSavedSuccessfully = true;
                string yuklemeYeri = null;
                string dosyaAd = TempData["YuklenenDosyaAd"] as string;

                if (dosyaAd == null)
                    dosyaAd = "";
                try
                {
                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileName];

                        if (file != null && file.ContentLength > 0)
                        {
                            /* ---- Start ---- */
                            /* ---- Injection içerikleri parse etmemesi için eklendi. ----*/
                            if (!DataProvider.IsImage(file))
                            {
                                isSavedSuccessfully = false;
                                return Json("ErrorUnknowFileContent");
                            }
                            /* ---- End ---- */
                            image = file.FileName;
                            dosyaAd = file.FileName;
                            if (ImageType == "A")
                            {
                                yuklemeYeri = Path.Combine(Server.MapPath("~/images/aksesuaricons/"), image);
                            }
                            else if (ImageType == "P")
                            {
                                yuklemeYeri = Path.Combine(Server.MapPath("~/images/profilicons/"), image);
                            }
                            //burada image resize edilir. Excel çıktısında kolon uzunluğu sabit 34.5 point 1.333 ile çarpılınca pixel verir. genişlik 16point
                            WebImage img = new WebImage(file.InputStream);
                            int excelCellHeight = 46 - 1;
                            int excelCellWidth = 116 - 1;
                            if (img.Width > excelCellWidth || img.Height > excelCellHeight)
                            {
                                img.Resize(excelCellWidth, excelCellHeight);
                                img.Save(@yuklemeYeri);
                            }
                            else
                            {
                                //uzunluklar excel için ideal doğrudan kaydet
                                img.Save(@yuklemeYeri);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    isSavedSuccessfully = false;

                    return Json(new { Message = "Error in saving file" }, JsonRequestBehavior.AllowGet);
                }
                if (isSavedSuccessfully)
                {
                    TempData["YuklenenDosyaAd"] = dosyaAd;
                    return Json(image);
                }
                else
                {
                    return Json(new { Message = "Error in saving file" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("KodBos", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AuthLog(Roles = "SILME")]
        public JsonResult DosyaYuklemeSil(string ImageName, string ImageType)
        {
            bool isSavedSuccessfully = true;
            try
            {
                if (ImageType == "A")
                {

                    System.IO.File.Delete(Server.MapPath("~/images/aksesuaricons/" + ImageName));
                }
                else if (ImageType == "P")
                {
                    System.IO.File.Delete(Server.MapPath("~/images/profilicons/" + ImageName));
                }
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
