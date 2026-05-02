using CamSistemWebArayuz.ViewModels;
using CamSistemDataLayer.BussinesLogic;
using CamSistemDataLayer.Enums;
using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using CamSistemWebArayuz.Attributes;
using CamSistemWebArayuz.Models;
using CamSistemWebArayuz.Models.Sistemler;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// Optimizasyon alias
using OptInput = Optimizasyon.Input;
using OptFireStok = Optimizasyon.FireStok;
using OptStok = Optimizasyon.Stok;
using OptSiparis = Optimizasyon.Siparis;
using OptProfil = Optimizasyon.Profil;
using OptOptimizer = Optimizasyon.Optimizer;
using OptOutput = Optimizasyon.Output;

namespace CamSistemWebArayuz.Controllers
{
    [SessionController]
    [AuthLog(Roles = "SİPARİS")]
    public class SiparisController : Controller
    {
        SiparisRepo siparisRepo;
        MusteriRepo musteriRepo;
        RenkRepo renkRepo;
        SistemRepo sistemRepo;
        AltSistemRepo asRepo;
        SistemTurRepo stRepo;
        SistemAltSistemJoinRepo sjRepo;
        AksesuarRepo aksesuarRepo;
        SiparisAksesuarRepo siparisAksesuarRepo;
        SistemAksesuarJoinRepo sajRepo;
        SiparisEnBoyAdetRepo sebaRepo;
        SiparisCamRepo scRepo;
        SiparisStokRepo siparisStokRepo;
        ProfilBoyRepo profilBoyRepo;
        ProfilRepo profilRepo;

        #region Helpers (Admin / Role)
        private bool CurrentUserIsAdmin()
        {
            var currentUser = Session["CurrentUser"] as CamSistemDataLayer.Models.Kullanici;
            if (currentUser == null) return false;

            var kullaniciRolRepo = new CamSistemDataLayer.Repos.KullaniciRolRepo();
            var rolRepo = new CamSistemDataLayer.Repos.RolRepo();

            var kullaniciRol = kullaniciRolRepo.FindBy(x => x.KullaniciId == currentUser.Id).FirstOrDefault();
            if (kullaniciRol == null) return false;

            var rol = rolRepo.FindBy(x => x.Id == kullaniciRol.RolId).FirstOrDefault();
            if (rol == null) return false;

            return string.Equals(rol.RolAdi, "ADMIN", StringComparison.OrdinalIgnoreCase)
                || string.Equals(rol.RolAdi, "ADMİN", StringComparison.OrdinalIgnoreCase);
        }

        private string CurrentUserRoleAd()
        {
            var currentUser = Session["CurrentUser"] as CamSistemDataLayer.Models.Kullanici;
            if (currentUser == null) return "(kullanıcı null)";

            var kullaniciRolRepo = new CamSistemDataLayer.Repos.KullaniciRolRepo();
            var rolRepo = new CamSistemDataLayer.Repos.RolRepo();

            var kullaniciRol = kullaniciRolRepo.FindBy(x => x.KullaniciId == currentUser.Id).FirstOrDefault();
            if (kullaniciRol == null) return "(rol null)";

            var rol = rolRepo.FindBy(x => x.Id == kullaniciRol.RolId).FirstOrDefault();
            if (rol == null) return "(rol objesi null)";

            return rol.RolAdi;
        }
        #endregion

        #region Optimizasyon (Yeni sistem)
        private OptimizasyonHesap ParseKesimBicimiToHesap(string kesimBicimi, string kullanilanAlan, long siparisId, OptOutput output, int kullaniciId)
        {
            string[] parts = kesimBicimi.Split('#');
            if (parts.Length < 5) return null;
            int profilId, profilBoy, fireAtik, kesimAdet;
            if (!int.TryParse(parts[0].Trim(), out profilId) ||
                !int.TryParse(parts[1].Trim(), out profilBoy) ||
                !int.TryParse(parts[3].Trim(), out fireAtik) ||
                !int.TryParse(parts[4].Trim(), out kesimAdet))
                return null;
            return new OptimizasyonHesap
            {
                SiparisIds = siparisId.ToString(),
                ProfilId = profilId,
                ProfilBoy = profilBoy,
                KesilecekOlculer = parts[2],
                FireAtik = fireAtik,
                KesimAdet = kesimAdet,
                KullanilanAlan = kullanilanAlan,
                ToplamAtikUzunluk = (decimal)output.toplamAtikUzunluk,
                ToplamAtikAgirlik = (decimal)output.toplamAtikAgirlik,
                FireyeEklenenToplamUzunluk = (decimal)output.fireStogaEklenenToplamUzunluk,
                FireyeEklenenToplamAgirlik = (decimal)output.fireStogaEklenenToplamAgirlik,
                AsilStoktanKullanilanToplamUzunluk = (decimal)output.kullanilanToplamUzunlukAsil,
                AsilStoktanKullanilanToplamAgirlik = (decimal)output.kullanilanToplamAgirlikAsil,
                FiredenKullanilanToplamUzunluk = (decimal)output.kullanilanToplamUzunlukFire,
                FiredenKullanilanToplamAgirlik = (decimal)output.kullanilanToplamAgirlikFire,
                KayitTarih = DateTime.Now,
                KullaniciId = kullaniciId
            };
        }

        private OptOutput RunOptimizerForSiparis(List<long> siparisIds, bool fireKullanilsinMi)
        {
            // ImalatController.ImalataGonder içindeki kodun birebir aynısı
            OptInput input = new OptInput();
            OptFireStok fire = new OptFireStok();
            OptStok stok = new OptStok();

            AtikStokRepo asRepo = new AtikStokRepo();
            SabitRepo sabitRepo = new SabitRepo();
            StokRepo stokRepo = new StokRepo();
            ProfilRepo pRepo = new ProfilRepo();
            ProfilBoyRepo pbRepo = new ProfilBoyRepo();
            SiparisEnBoyAdetRepo sebaRepo = new SiparisEnBoyAdetRepo();

            // fire stok
            int fireMinDeger = (int)sabitRepo.FindBy(e => e.Id == 1).FirstOrDefault().SabitDeger;
            List<OptProfil> fireler = new List<OptProfil>();
            List<AtikStok> atikStok = asRepo.GetAll().ToList();

            fire.minDeger = fireMinDeger;
            foreach (AtikStok item in atikStok)
            {
                Optimizasyon.Profil profil = new Optimizasyon.Profil();
                profil.Adet = (int)item.Adet;
                profil.Boy = (int)item.Olcu;
                profil.Profil_Kod = (int)item.ProfilId;
                fireler.Add(profil);
            }
            fire.Fireler = fireler;
            input.FireStok = fire;

            // stok
            Dictionary<int, Dictionary<int, int>> stoktakiProfiller = new Dictionary<int, Dictionary<int, int>>();
            List<Optimizasyon.Profil> profiller = new List<Optimizasyon.Profil>();
            List<CamSistemDataLayer.Models.Stok> stokListesi = stokRepo.GetAll().ToList();

            foreach (CamSistemDataLayer.Models.Stok item in stokListesi)
            {
                Optimizasyon.Profil profil = new Optimizasyon.Profil();
                if (item.OzelOlcu == null)
                {
                    profil.Profil_Kod = (int)item.ProfilId;
                    int profilId = Convert.ToInt32(item.ProfilId);
                    profil.Gram = Convert.ToInt32(pRepo.FindBy(e => e.Id == profilId).FirstOrDefault().BirimAgirlik);
                    profil.Boy = (int)pbRepo.FindBy(e => e.Id == item.ProfilBoyId).FirstOrDefault().ProfilBoyu;
                    profil.Adet = (int)item.StokAdet;
                }
                else
                {
                    int profilId = Convert.ToInt32(item.ProfilId);
                    profil.Gram = Convert.ToInt32(pRepo.FindBy(e => e.Id == profilId).FirstOrDefault().BirimAgirlik);
                    profil.Profil_Kod = (int)item.ProfilId;
                    profil.Boy = (int)item.OzelOlcu;
                    profil.Adet = (int)item.StokAdet;
                }
                profiller.Add(profil);
            }

            List<CamSistemDataLayer.Models.Profil> profilIds = pRepo.GetAll().ToList();
            Dictionary<int, int> dicProfilBirimAgirlik = new Dictionary<int, int>();

            foreach (var item in profilIds)
            {
                Dictionary<int, int> dic = new Dictionary<int, int>();
                dicProfilBirimAgirlik[item.Id] = (int)item.BirimAgirlik;

                List<Optimizasyon.Profil> profilList = profiller.Where(e => e.Profil_Kod == item.Id).ToList();

                foreach (var item2 in profilList)
                {
                    if (!dic.ContainsKey(item2.Boy))
                        dic.Add(item2.Boy, item2.Adet);
                    else
                        dic[item2.Boy] += item2.Adet;
                }

                if (!stoktakiProfiller.ContainsKey(item.Id))
                    stoktakiProfiller.Add(item.Id, dic);
                else
                {
                    foreach (var pair in dic)
                    {
                        if (!stoktakiProfiller[item.Id].ContainsKey(pair.Key))
                            stoktakiProfiller[item.Id].Add(pair.Key, pair.Value);
                        else
                            stoktakiProfiller[item.Id][pair.Key] += pair.Value;
                    }
                }
            }

            stok.Stoktaki_Profiller = stoktakiProfiller;
            input.Stok = stok;
            input.ProfilBirimAgirlik = dicProfilBirimAgirlik;

            // Sipariş -> profiller
            List<Optimizasyon.Siparis> siparisler = new List<Optimizasyon.Siparis>();
            Dictionary<List<CamSistemDataLayer.Models.Profil>, long> spList = new Dictionary<List<CamSistemDataLayer.Models.Profil>, long>();

            foreach (var item in siparisIds)
            {
                List<SiparisEnBoyAdet> siparisAdet = sebaRepo.FindBy(e => e.SiparisId == item).ToList();
                foreach (var item2 in siparisAdet)
                {
                    var hesaplananProfiller = SiparisHesaplamalari.profilHesaplama(
                        item,
                        (int)(item2.GirilenEn ?? 0),
                        (int)(item2.GirilenSolEn ?? 0),
                        (int)(item2.GirilenBoy ?? 0),
                        (int)(item2.GirilenAdet ?? 0)
                    );

                    // Not: Bu yapı aynı sipariş için birden çok en/boy girilince aynı key tekrar ederse exception üretebilir.
                    // "Bozmadan" ilerlediğimiz için şimdilik dokunmuyoruz.
                    spList.Add(hesaplananProfiller, item);
                }
            }

            foreach (var item in spList)
            {
                Optimizasyon.Siparis siparis = new Optimizasyon.Siparis();
                List<Optimizasyon.Profil> profilListesi = new List<Optimizasyon.Profil>();

                foreach (var item2 in item.Key)
                {
                    Optimizasyon.Profil profil = new Optimizasyon.Profil();
                    profil.Adet = item2.KesimAdet;
                    profil.Boy = item2.KesimOlcusu;

                    // ImalatController’daki profil kod mapping (aynı)
                    if (item2.ProfilKodu.Contains("AP-101") || item2.ProfilKodu.Contains("BC-108") || item2.ProfilKodu.Contains("BC-107") || item2.ProfilKodu.Contains("BC-103") || item2.ProfilKodu.Contains("BC-102")
                        || item2.ProfilKodu.Contains("RK-104") || item2.ProfilKodu.Contains("G-106") || item2.ProfilKodu.Contains("G-110") || item2.ProfilKodu.Contains("G-111")
                        || item2.ProfilKodu.Contains("G-112") || item2.ProfilKodu.Contains("G-115") || item2.ProfilKodu.Contains("G-116") || item2.ProfilKodu.Contains("G-121")
                        || item2.ProfilKodu.Contains("G-126") || item2.ProfilKodu.Contains("G-127") || item2.ProfilKodu.Contains("SS-134") || item2.ProfilKodu.Contains("SS-133")
                        || item2.ProfilKodu.Contains("SS-132") || item2.ProfilKodu.Contains("SS-130") || item2.ProfilKodu.Contains("SS-128") || item2.ProfilKodu.Contains("SS-126")
                        || item2.ProfilKodu.Contains("SS-124") || item2.ProfilKodu.Contains("SS-121") || item2.ProfilKodu.Contains("SS-118") || item2.ProfilKodu.Contains("SS-117")
                        || item2.ProfilKodu.Contains("SS-135") || item2.ProfilKodu.Contains("SS-136") || item2.ProfilKodu.Contains("SS-120") || item2.ProfilKodu.Contains("T-2457") || item2.ProfilKodu.Contains("T-2456") || item2.ProfilKodu.Contains("T-2400")
                        || item2.ProfilKodu.Contains("KAR-4873") || item2.ProfilKodu.Contains("KAR-4862"))
                    {
                        if (item2.ProfilKodu.Split('-').Length > 2)
                        {
                            string[] split = item2.ProfilKodu.Split('-');
                            string merge = split[0] + "-" + split[1];
                            CamSistemDataLayer.Models.Profil pro = pRepo.FindBy(e => e.ProfilKodu.Equals(merge)).First();
                            profil.Profil_Kod = pro.Id;
                        }
                        else
                        {
                            profil.Profil_Kod = item2.Id;
                        }
                    }
                    else
                    {
                        profil.Profil_Kod = item2.Id;
                    }

                    profilListesi.Add(profil);
                }

                siparis.siparis_id = item.Value;
                siparis.profiller = profilListesi;
                siparis.siparis_adet = 1;
                siparisler.Add(siparis);
            }

            input.Siparisler = siparisler;

            // optimize
            Optimizasyon.Optimizer opti = new Optimizasyon.Optimizer(input, fireKullanilsinMi);
            opti.optimizeEt();
            return opti.output;
        }

        /// <summary>
        /// Optimizasyon sonuçlarını (kesimBicimiStok/kesimBicimiFireStok) veritabanına kaydeder.
        /// Tam veri içerir: KesilecekOlculer, FireAtik, KesimAdet ve tüm toplam alanları.
        /// </summary>
        private void SaveOptimizasyonHesaplar(OptOutput output, string siparisIdStr, int kullaniciId)
        {
            if (output == null) return;
            var repo = new OptimizasyonHesapRepo();

            if (output.kesimBicimiStok != null)
            {
                foreach (var item in output.kesimBicimiStok)
                {
                    try
                    {
                        // Format: profil_id#barSize#cuts#waste#count#missing
                        string[] split = item.Split('#');
                        if (split.Length < 6) continue;
                        repo.AddAndSave(new OptimizasyonHesap
                        {
                            SiparisIds = siparisIdStr,
                            ProfilId = int.Parse(split[0].Trim()),
                            ProfilBoy = int.Parse(split[1].Trim()),
                            KesilecekOlculer = split[2],
                            FireAtik = int.TryParse(split[3].Trim(), out int fa) ? fa : 0,
                            KesimAdet = int.TryParse(split[4].Trim(), out int ka) ? ka : 0,
                            KullanilanAlan = "Asıl Stok",
                            ToplamAtikUzunluk = (decimal)output.toplamAtikUzunluk,
                            ToplamAtikAgirlik = (decimal)output.toplamAtikAgirlik,
                            AsilStoktanKullanilanToplamUzunluk = (decimal)output.kullanilanToplamUzunlukAsil,
                            AsilStoktanKullanilanToplamAgirlik = (decimal)output.kullanilanToplamAgirlikAsil,
                            FiredenKullanilanToplamUzunluk = (decimal)output.kullanilanToplamUzunlukFire,
                            FiredenKullanilanToplamAgirlik = (decimal)output.kullanilanToplamAgirlikFire,
                            FireyeEklenenToplamUzunluk = (decimal)output.fireStogaEklenenToplamUzunluk,
                            FireyeEklenenToplamAgirlik = (decimal)output.fireStogaEklenenToplamAgirlik,
                            KayitTarih = DateTime.Now,
                            KullaniciId = kullaniciId
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("[SaveOptimizasyonHesaplar] Asıl stok satır hatası: " + ex.Message + " | Satır: " + item);
                    }
                }
            }

            if (output.kesimBicimiFireStok != null)
            {
                foreach (var item in output.kesimBicimiFireStok)
                {
                    try
                    {
                        // Format: profil_id#barSize#cuts#waste#count#missing
                        string[] split = item.Split('#');
                        if (split.Length < 6) continue;
                        repo.AddAndSave(new OptimizasyonHesap
                        {
                            SiparisIds = siparisIdStr,
                            ProfilId = int.Parse(split[0].Trim()),
                            ProfilBoy = int.Parse(split[1].Trim()),
                            KesilecekOlculer = split[2],
                            FireAtik = int.TryParse(split[3].Trim(), out int fa) ? fa : 0,
                            KesimAdet = int.TryParse(split[4].Trim(), out int ka) ? ka : 0,
                            KullanilanAlan = "Fire Stok",
                            ToplamAtikUzunluk = (decimal)output.toplamAtikUzunluk,
                            ToplamAtikAgirlik = (decimal)output.toplamAtikAgirlik,
                            AsilStoktanKullanilanToplamUzunluk = (decimal)output.kullanilanToplamUzunlukAsil,
                            AsilStoktanKullanilanToplamAgirlik = (decimal)output.kullanilanToplamAgirlikAsil,
                            FiredenKullanilanToplamUzunluk = (decimal)output.kullanilanToplamUzunlukFire,
                            FiredenKullanilanToplamAgirlik = (decimal)output.kullanilanToplamAgirlikFire,
                            FireyeEklenenToplamUzunluk = (decimal)output.fireStogaEklenenToplamUzunluk,
                            FireyeEklenenToplamAgirlik = (decimal)output.fireStogaEklenenToplamAgirlik,
                            KayitTarih = DateTime.Now,
                            KullaniciId = kullaniciId
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("[SaveOptimizasyonHesaplar] Fire stok satır hatası: " + ex.Message + " | Satır: " + item);
                    }
                }
            }
        }

        /// <summary>
        /// Belirtilen sipariş için OptimizasyonHesap kayıtlarını getirir.
        /// Kayıt yoksa optimizer'ı çalıştırıp kaydeder, sonra yeniden okur.
        /// </summary>
        private List<OptimizasyonHesap> GetOrRunOptimizasyonHesaps(long siparisId)
        {
            string siparisIdStr = siparisId.ToString();
            var optimizasyonHesapRepo = new OptimizasyonHesapRepo();

            List<OptimizasyonHesap> GetFiltered()
            {
                var all = optimizasyonHesapRepo.FindBy(e => e.SiparisIds != null && e.SiparisIds.Contains(siparisIdStr)).ToList();
                return all
                    .Where(x => x.SiparisIds.Split(',').Select(s => s.Trim()).Any(id => id == siparisIdStr))
                    .ToList();
            }

            var hesaps = GetFiltered();
            if (!hesaps.Any())
            {
                try
                {
                    var currentUser = (Kullanici)Session["CurrentUser"];
                    var output = RunOptimizerForSiparis(new List<long> { siparisId }, fireKullanilsinMi: false);
                    if (output != null)
                    {
                        SaveOptimizasyonHesaplar(output, siparisIdStr, currentUser?.Id ?? 0);
                        hesaps = GetFiltered();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("[GetOrRunOptimizasyonHesaps] Hata SiparisId=" + siparisId + ": " + ex.Message + "\n" + ex.StackTrace);
                }
            }
            return hesaps;
        }

        [HttpPost]
        [AuthLog(Roles = "SİPARİS,GORUNTULEME,IMALAT,ONAYLAMA")]
        public ActionResult OptimizasyonHesapla(long SiparisId)
        {
            try
            {
                var kayitlar = GetOrRunOptimizasyonHesaps(SiparisId)
                    .OrderByDescending(x => x.Id)
                    .ToList();
                return PartialView("_optimizasyonHesapGrid", kayitlar);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[OptimizasyonHesapla] Hata SiparisId=" + SiparisId + ": " + ex.Message);
                return PartialView("_optimizasyonHesapGrid", new List<OptimizasyonHesap>());
            }
        }
        #endregion

        #region Index / Liste
        [AuthLog(Roles = "SİPARİS,GORUNTULEME")]
        public ActionResult Index()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "DogrudanSiparisSayfasi";
            siparisRepo = new SiparisRepo();

            var currentUser = Session["CurrentUser"] as CamSistemDataLayer.Models.Kullanici;
            if (currentUser == null)
                return RedirectToAction("Login", "Login");

            var isAdmin = CurrentUserIsAdmin();

            var allSiparisler = siparisRepo.GetAll();

            var filtered = allSiparisler
                .Where(e =>
                    (e.DurumId != null &&
                     e.DurumId != (int)Durumlar.TeslimEdildi &&
                     e.DurumId != (int)Durumlar.Reddedildi) &&
                    (string.IsNullOrEmpty(e.SiparisTur) || e.SiparisTur != "Stoktan Gönderim")
                ).ToList();

            if (!isAdmin)
                filtered = filtered.Where(e => e.OlusturanKullaniciId == currentUser.Id).ToList();

            var allUsers = new KullaniciRepo().GetAll();

            var model = filtered.Select(e =>
            {
                var user = allUsers.FirstOrDefault(x => x.Id == e.OlusturanKullaniciId);
                return new SiparisGorunumViewModel
                {
                    Id = e.Id,
                    SistemTamami = e.SistemTamami,
                    DurumId = e.DurumId,
                    SistemId = e.SistemId,
                    SiparisTur = e.SiparisTur,
                    OlusturanKullaniciAdi = (user != null) ? user.KullaniciAdi + " " + user.KullaniciSoyadi : "-",
                    KayitTarihi = e.KayitTarihi
                };
            }).Take(300).ToList();

            return View(model);
        }
        #endregion

        #region Siparis Kaydet / Açıklama
        [HttpPost]
        [AuthLog(Roles = "YENIKAYIT,DUZENLEME")]
        public JsonResult AciklamaKaydet(long SiparisId, string Aciklama)
        {
            siparisRepo = new SiparisRepo();
            var siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
            if (siparis == null)
                return Json("NOK", JsonRequestBehavior.AllowGet);

            siparis.Aciklama = Aciklama;
            siparis.GuncellemeTarihi = DateTime.Now;
            siparisRepo.EditAndSave(siparis);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult SiparisKaydet()
        {
            CamKombinasyonRepo camKombinasyonRepo = new CamKombinasyonRepo();
            aksesuarRepo = new AksesuarRepo();
            musteriRepo = new MusteriRepo();
            renkRepo = new RenkRepo();
            sistemRepo = new SistemRepo();
            asRepo = new AltSistemRepo();
            stRepo = new SistemTurRepo();

            ViewBag.Musteriler = musteriRepo.FindBy(e => e.AktifMi == true);
            ViewBag.Renkler = renkRepo.FindBy(e => e.AktifMi == true);
            ViewBag.Sistemler = sistemRepo.FindBy(e => e.AktifMi == true);
            ViewBag.AltSistemler = asRepo.FindBy(e => e.AktifMi == true);
            ViewBag.SistemTurleri = stRepo.FindBy(e => e.AktifMi == true);
            ViewBag.Aksesuarlar = new MultiSelectList(aksesuarRepo.FindBy(e => e.AktifMi == true), "Id", "AksesuarAdi");
            ViewBag.CamKombinasyonList = camKombinasyonRepo.GetAll();

            Siparis siparis = new Siparis();
            return View(siparis);
        }

        [HttpPost]
        [AuthLog(Roles = "YENIKAYIT")]
        public JsonResult SistemAltturGetir(int SistemId)
        {
            try
            {
                sjRepo = new SistemAltSistemJoinRepo();
                asRepo = new AltSistemRepo();
                stRepo = new SistemTurRepo();

                var altSistemIdList = sjRepo.FindBy(x => x.SistemId == SistemId)
                                            .Select(x => x.AltSistemId)
                                            .ToList();

                var altSistemEntities = asRepo.FindBy(x => x.AktifMi == true && altSistemIdList.Contains(x.Id))
                                              .ToList();

                var altSistemListesi = altSistemEntities
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.AltSistemAdi
                    })
                    .ToList();

                var sistemTurEntities = stRepo.FindBy(x => x.AktifMi == true).ToList();
                var sistemTurListesi = sistemTurEntities
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.TurAdi
                    })
                    .ToList();

                return Json(new
                {
                    SistemTuruVarMi = sistemTurListesi.Any(),
                    SistemTurListesi = sistemTurListesi,
                    AltSistemListesi = altSistemListesi
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult SiparisKaydet(List<SiparisEnBoyAdet> enBoyAdet, Siparis siparis, SiparisCam siparisCam)
        {
            siparisAksesuarRepo = new SiparisAksesuarRepo();
            sebaRepo = new SiparisEnBoyAdetRepo();
            siparisRepo = new SiparisRepo();
            scRepo = new SiparisCamRepo();

            try
            {
                if (!ModelState.IsValid)
                    return Json("NOK", JsonRequestBehavior.AllowGet);

                if (siparis == null)
                    return Json("NOK", JsonRequestBehavior.AllowGet);

                if (enBoyAdet == null || enBoyAdet.Count == 0)
                    return Json("NOK", JsonRequestBehavior.AllowGet);

                siparis.IslemDurum = "İşlem Beklemede";
                siparis.KayitTarihi = DateTime.Now;
                siparis.DurumId = 1; // Onayda bekleyen

                var currentUser = Session["CurrentUser"] as Kullanici;
                if (currentUser == null)
                    return Json("LOGIN", JsonRequestBehavior.AllowGet);

                siparis.OlusturanKullaniciId = currentUser.Id;

                if (siparis.SiparisTur == "tur_demonte")
                    siparis.SiparisTur = "Demonte Gönderim";
                else if (siparis.SiparisTur == "tur_profil")
                    siparis.SiparisTur = "Profil Gönderim";

                Siparis siparisEntity = siparisRepo.SaveAndReturnEntity(siparis);

                // Cam kombinasyon
                if (siparisCam != null && !string.IsNullOrWhiteSpace(siparisCam.CamKombinasyon)
                    && !siparisCam.CamKombinasyon.Equals("Cam Kombinasyon Seçiniz..."))
                {
                    siparisCam.SiparisId = siparisEntity.Id;
                    scRepo.AddAndSave(siparisCam);
                }

                // Aksesuarlar
                if (siparis.SeciliAksesuarlar != null)
                {
                    foreach (var item in siparis.SeciliAksesuarlar)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            var aksesuar = new SiparisAksesuar
                            {
                                AksesuarId = Convert.ToInt32(item),
                                SiparisId = siparisEntity.Id
                            };

                            siparisAksesuarRepo.AddAndSave(aksesuar);
                        }
                    }
                }

                // En/Boy/Adet kayıtları
                foreach (var item in enBoyAdet)
                {
                    var enBoyAdetModel = new SiparisEnBoyAdet
                    {
                        GirilenAdet = item.GirilenAdet ?? 0,
                        GirilenSolEn = item.GirilenSolEn ?? 0,
                        GirilenBoy = item.GirilenBoy ?? 0,
                        GirilenEn = item.GirilenEn ?? 0,
                        GirilenKanatAdet = item.GirilenKanatAdet ?? 0,
                        SiparisId = siparisEntity.Id
                    };

                    sebaRepo.AddAndSave(enBoyAdetModel);
                }

                // PROFİL GÖNDERİM -> OPTİMİZASYON HESAPLA ve DB'ye yaz
                if (siparisEntity.SiparisTur == "Profil Gönderim")
                {
<<<<<<< HEAD
                    try
                    {
                        var output = RunOptimizerForSiparis(new List<long> { siparisEntity.Id }, fireKullanilsinMi: false);
                        if (output != null)
                        {
                            var currentUser2 = (Kullanici)Session["CurrentUser"];
                            SaveOptimizasyonHesaplar(output, siparisEntity.Id.ToString(), currentUser2?.Id ?? 0);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("[SiparisKaydet] Profil optimizasyon hatası SiparisId=" + siparisEntity.Id + ": " + ex.Message);
=======
                    var output = RunOptimizerForSiparis(new List<long> { siparisEntity.Id }, fireKullanilsinMi: false);
                    var optimizasyonRepo = new OptimizasyonHesapRepo();
                    var currentUser2 = (Kullanici)Session["CurrentUser"];

                    if (output != null && !output.hata)
                    {
                        // Asıl stok kesim listesini kaydet (kesim detayları dahil)
                        if (output.kesimBicimiStok != null)
                        {
                            foreach (var kbs in output.kesimBicimiStok)
                            {
                                var hesap = ParseKesimBicimiToHesap(kbs, "Asıl Stok", siparisEntity.Id, output, currentUser2.Id);
                                if (hesap != null) optimizasyonRepo.AddAndSave(hesap);
                            }
                        }

                        // Fire stok kesim listesini kaydet (kesim detayları dahil)
                        if (output.kesimBicimiFireStok != null)
                        {
                            foreach (var kbf in output.kesimBicimiFireStok)
                            {
                                var hesap = ParseKesimBicimiToHesap(kbf, "Fire Stok", siparisEntity.Id, output, currentUser2.Id);
                                if (hesap != null) optimizasyonRepo.AddAndSave(hesap);
                            }
                        }
>>>>>>> copilot/fix-optimization-issues
                    }
                }

                ViewBag.RecordResult = 1;
                ModelState.Clear();

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                ViewBag.RecordResult = 2;
                return Json("NOK", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Siparis Detay
        [AuthLog(Roles = "SİPARİS,GORUNTULEME")]
        public ActionResult SiparisDetayGoruntule(long SiparisId, bool raporMu)
        {
            // null-safe repo nesneleri
            var siparisRepo = new SiparisRepo();
            var sebaRepo = new SiparisEnBoyAdetRepo();
            var siparisTeklifRepo = new SiparisTeklifRepo();
            var siparisCamRepo = new SiparisCamRepo();
            var renkRepo = new RenkRepo();
            var aksesuarRepo = new AksesuarRepo();
            var siparisAksesuarRepo = new SiparisAksesuarRepo();
            var sabitRepo = new SabitRepo();
            var dosyaRepo = new DosyaRepo();

            var siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
            if (siparis == null)
                return Content("Sipariş bulunamadı!");

            ViewBag.SiparisDurumu = (siparis.DurumId == (int)Durumlar.Onaylandı || siparis.DurumId == (int)Durumlar.ImalataGonderildi || siparis.DurumId == (int)Durumlar.Sevkiyatta);

            List<SiparisEnBoyAdet> siparisAdet = sebaRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            List<SiparisTeklif> teklifListesi = siparisTeklifRepo.GetAll().ToList();

            List<SiparisEnBoyAdet> siparisTumDetay = new List<SiparisEnBoyAdet>();
            var siparisCam = siparisCamRepo.FindBy(e => e.SiparisId == siparis.Id).FirstOrDefault() ?? new SiparisCam { CamKombinasyon = "" };

            // Dosya listesi null-safe
            ViewBag.aciklamaDosyaList = null;
            if (!string.IsNullOrWhiteSpace(siparis.DosyaIds))
            {
                string[] aciklamaDosyaIds = siparis.DosyaIds.Split(',');
                var aciklamaDosyalari = new List<YuklenenDosyalar>();
                foreach (var split in aciklamaDosyaIds)
                {
                    if (!string.IsNullOrWhiteSpace(split))
                    {
                        long dosyaId;
                        if (long.TryParse(split.Trim(), out dosyaId))
                        {
                            var dosyalar = dosyaRepo.FindBy(e => e.Id == dosyaId).FirstOrDefault();
                            if (dosyalar != null)
                                aciklamaDosyalari.Add(dosyalar);
                        }
                    }
                }
                ViewBag.aciklamaDosyaList = aciklamaDosyalari;
            }

            List<int> altSistemId4Surme = new List<int> { 1012, 1013, 1014, 1015 };

            foreach (var item in siparisAdet)
            {
                SiparisEnBoyAdet ent = new SiparisEnBoyAdet();

                int girilenEn = item.GirilenEn ?? 0;
                int girilenSolEn = item.GirilenSolEn ?? 0;
                int girilenBoy = item.GirilenBoy ?? 0;
                int girilenAdet = item.GirilenAdet ?? 0;

                List<Profil> profilList = SiparisHesaplamalari.profilHesaplama(siparis.Id, girilenEn, girilenSolEn, girilenBoy, girilenAdet);
                List<CamBilgileri> camBilgileriList = SiparisHesaplamalari.CamYukseklikHesapla(
                    (int)(siparis.SistemId ?? 0),
                    (int)(siparis.SistemTurId ?? 0),
                    (int)(siparis.AltSistemId ?? 0),
                    girilenBoy,
                    girilenEn,
                    girilenSolEn,
                    girilenAdet
                );

                ProfilDetayBilgileri profilDetay = new ProfilDetayBilgileri();
                if (camBilgileriList != null)
                {
                    if (altSistemId4Surme.Contains((int)(siparis.AltSistemId ?? 0)))
                    {
                        profilDetay.ToplamAlan = camBilgileriList.Where(e => !string.IsNullOrWhiteSpace(e.CamAdi) && e.CamAdi.Contains("SAĞ")).Sum(e => e.Alanm2);
                        ent.camList = camBilgileriList.ToList();
                    }
                    else
                    {
                        profilDetay.ToplamAlan = camBilgileriList.Sum(e => e.Alanm2);
                        ent.camList = camBilgileriList.ToList();
                    }
                }
                else
                {
                    ent.camList = new List<CamBilgileri>();
                }

                profilDetay.ToplamPresKG = profilList.Sum(e => e.ToplamAgirlik);
                profilDetay.ToplamBoyaliKG = profilDetay.ToplamPresKG * 1.035;

                ent.camModel = profilDetay;
                ent.profilList = profilList;
                ent.GirilenAdet = item.GirilenAdet;
                ent.GirilenBoy = item.GirilenBoy;
                ent.GirilenEn = item.GirilenEn;
                ent.GirilenSolEn = item.GirilenSolEn;
                ent.SiparisId = item.SiparisId;
                ent.Id = item.Id;
                ent.siparisModel = siparis;
                ent.siparisCam = siparisCam;

                // ==== TEKLİF / MALİYET (SiparisTeklif) ====
                var teklifSatirlari = siparisTeklifRepo
                    .FindBy(t => t.SiparisEnBoyAdetId == item.Id)
                    .OrderBy(t => t.Id)
                    .ToList();

                // Bu 6 kalem zorunlu: eksikse teklifi yeniden üret
                bool zorunluEksikMi = false;
                string[] zorunlular = new[]
                {
                    "ALÜMİNYUM",
                    "CAM",
                    "AKSESUAR SETİ",
                    "İMALAT BEDELİ",
                    "SARF MALZEME BEDELİ",
                    "KAR PAYI"
                };

                if (teklifSatirlari == null || teklifSatirlari.Count == 0)
                {
                    zorunluEksikMi = true;
                }
                else
                {
                    foreach (var z in zorunlular)
                    {
                        if (!teklifSatirlari.Any(x => x.Malzeme != null &&
                                                      x.Malzeme.Trim().Equals(z, StringComparison.OrdinalIgnoreCase)))
                        {
                            zorunluEksikMi = true;
                            break;
                        }
                    }
                }

                if (zorunluEksikMi)
                {
                    var silinecekler = siparisTeklifRepo.FindBy(t => t.SiparisEnBoyAdetId == item.Id).ToList();
                    foreach (var s in silinecekler)
                        siparisTeklifRepo.DeleteAndSave(s);

                    var siparisAksesuarList = siparisAksesuarRepo.FindBy(a => a.SiparisId == siparis.Id).ToList();
                    List<Aksesuar> aksesuarEntities = null;
                    if (siparisAksesuarList != null && siparisAksesuarList.Count > 0)
                    {
                        var aksesuarIds = siparisAksesuarList.Select(x => x.AksesuarId).ToList();
                        aksesuarEntities = aksesuarRepo.FindBy(x => x.AktifMi == true && aksesuarIds.Contains(x.Id)).ToList();
                    }

                    var maliyetToplam = MaliyetHesapla.MaliyetHesaplama(
                        aksesuarEntities,
                        ent.camModel,
                        siparisCam != null ? siparisCam.CamKombinasyon : "",
                        item,
                        siparis.Id
                    );

                    if (maliyetToplam != null && maliyetToplam.MaliyetList != null)
                    {
                        foreach (var m in maliyetToplam.MaliyetList)
                        {
                            var yeni = new SiparisTeklif
                            {
                                SiparisEnBoyAdetId = item.Id,
                                Malzeme = m.Malzeme,
                                Birim = m.Birim,
                                Miktar = m.Miktar,
                                BirimFiyat = m.BirimFiyat,
                                ToplamTutar = m.ToplamTutar,
                                KayitTarihi = DateTime.Now
                            };

                            siparisTeklifRepo.AddAndSave(yeni);
                        }
                    }

                    teklifSatirlari = siparisTeklifRepo
                        .FindBy(t => t.SiparisEnBoyAdetId == item.Id)
                        .OrderBy(t => t.Id)
                        .ToList();
                }

                ent.teklifList = teklifSatirlari;

                if (teklifSatirlari != null && teklifSatirlari.Count > 0)
                {
                    decimal toplamMaliyet = teklifSatirlari.Sum(x => x.ToplamTutar ?? 0m);

                    decimal alan = 0m;
                    if (girilenEn > 0 && girilenBoy > 0)
                    {
                        int enToplam = girilenEn + (girilenSolEn > 0 ? girilenSolEn : 0);
                        alan = ((decimal)enToplam * (decimal)girilenBoy) / 1000000m;
                    }

                    ent.teklifToplamDetay = new SiparisTeklifToplamBilgisi
                    {
                        ToplamMaliyet = toplamMaliyet,
                        m2 = (alan > 0m ? (toplamMaliyet / alan) : 0m),
                        Teklif = toplamMaliyet
                    };
                }
                else
                {
                    ent.teklifToplamDetay = null;
                }

                siparisTumDetay.Add(ent);
            }

<<<<<<< HEAD
            // Optimizasyon kayıtlarını mevcut DB'den yükle (varsa ilk kayda ata, görünümde kullanılır)
            if (siparisTumDetay.Any())
            {
                try
                {
                    string siparisIdStr = siparis.Id.ToString();
                    var optimizasyonRepo = new OptimizasyonHesapRepo();
                    var optiKayitlar = optimizasyonRepo.FindBy(x => x.SiparisIds != null && x.SiparisIds.Contains(siparisIdStr)).ToList()
                        .Where(x => x.SiparisIds.Split(',').Select(s => s.Trim()).Any(id => id == siparisIdStr))
                        .OrderByDescending(x => x.Id)
                        .ToList();
                    siparisTumDetay[0].optimizasyonList = optiKayitlar;
                    ViewBag.optiVarMi = optiKayitlar.Any();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("[SiparisDetayGoruntule] Optimizasyon yükleme hatası: " + ex.Message);
                    siparisTumDetay[0].optimizasyonList = new List<OptimizasyonHesap>();
                    ViewBag.optiVarMi = false;
                }
            }
            else
            {
                ViewBag.optiVarMi = false;
            }
=======
            // Optimizasyon verilerini DB'den çekip modele ekle
            var optimizasyonHesapRepo = new OptimizasyonHesapRepo();
            string siparisIdStr = siparis.Id.ToString();
            var optimizasyonKayitlar = optimizasyonHesapRepo.GetAll()
                .Where(x => !string.IsNullOrWhiteSpace(x.SiparisIds) &&
                            x.SiparisIds.Split(',')
                                .Select(s => s.Trim())
                                .Any(id => id == siparisIdStr))
                .OrderByDescending(x => x.Id)
                .ToList();
            foreach (var ent in siparisTumDetay)
                ent.optimizasyonList = optimizasyonKayitlar;

            ViewBag.optiVarMi = optimizasyonKayitlar.Any();
>>>>>>> copilot/fix-optimization-issues

            if (string.IsNullOrWhiteSpace(siparis.Aciklama))
                ViewBag.SiparisAciklamasi = "";
            else
                ViewBag.SiparisAciklamasi = "<div class='alert alert-danger'><strong>" + SiparisId + " Nolu Sipariş Açıklaması:</strong> " + siparis.Aciklama + "</div>";

            ViewBag.raporMu = raporMu;
            ViewBag.minimumFire = sabitRepo.FindBy(e => e.Id == 1).FirstOrDefault()?.SabitDeger ?? 0;

            if (siparis.SistemId == 5 || siparis.SistemId == 2006 || siparis.SistemId == 2010)
                return PartialView("_siparisGiyotinSablon", siparisTumDetay);
            else
                return PartialView("_siparisDetaySablon", siparisTumDetay);
        }
        #endregion

        #region Durum / Onay / Fiş / Fiyat (Index.cshtml bunları çağırıyor)
        [AuthLog(Roles = "ONAYLAMA")]
        public ActionResult SiparisDurumGuncelle(long SiparisId, int DurumId)
        {
            try
            {
                siparisRepo = new SiparisRepo();
                Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
                Kullanici kullaniciModel = (Kullanici)Session["CurrentUser"];

                if (DurumId == (int)Durumlar.Onaylandı)
                {
                    siparis.OnayIptalKullaniciId = kullaniciModel.Id;
                    siparis.DurumId = (int)Durumlar.Onaylandı;
                    siparis.OnayIptalTarihi = DateTime.Now;
                    siparis.GuncellemeTarihi = DateTime.Now;
                    siparisRepo.EditAndSave(siparis);

                    return Json("OK", JsonRequestBehavior.AllowGet);
                }
                else if (DurumId == (int)Durumlar.Sevkiyatta)
                {
                    siparis.OnayIptalKullaniciId = kullaniciModel.Id;
                    siparis.DurumId = (int)Durumlar.Sevkiyatta;
                    siparis.GuncellemeTarihi = DateTime.Now;
                    siparisRepo.EditAndSave(siparis);

                    return Json("OK", JsonRequestBehavior.AllowGet);
                }

                //sevkiyat onaylandığında stoktan düşüm yapılacak.
                else if (DurumId == (int)Durumlar.SevkiyatOnaylandi)
                {
                    siparis.OnayIptalKullaniciId = kullaniciModel.Id;
                    siparis.DurumId = (int)Durumlar.SevkiyatOnaylandi;
                    siparis.GuncellemeTarihi = DateTime.Now;
                    siparisRepo.EditAndSave(siparis);

                    //stoktan düşüm burada yapılacak imalat kısmında düşüm kaldırıldı.
                    SiparisSevkiyatProfilRepo siparisSevkiyatProfilRepo = new SiparisSevkiyatProfilRepo();
                    SiparisSevkiyatAksesuarRepo siparisSevkiyatAksesuarRepo = new SiparisSevkiyatAksesuarRepo();
                    ProfilBoyRepo profilBoyRepo = new ProfilBoyRepo();
                    StokRepo stokRepo = new StokRepo();
                    StokAksesuarRepo stokAksesuarRepo = new StokAksesuarRepo();

                    List<SiparisSevkiyatProfil> siparisSevkiyatProfil = siparisSevkiyatProfilRepo.FindBy(e => e.SiparisEnBoyAdetId == SiparisId).ToList();
                    List<SiparisSevkiyatAksesuar> siparisSevkiyatAksesuar = siparisSevkiyatAksesuarRepo.FindBy(e => e.SiparisEnBoyAdetId == SiparisId).ToList();

                    foreach (var item in siparisSevkiyatProfil)
                    {
                        int profilBoy = Convert.ToInt32(item.ProfilBoy / 100);
                        ProfilBoy profilBoyEnt = profilBoyRepo.FindBy(e => e.ProfilBoyu == profilBoy).FirstOrDefault();
                        if (profilBoyEnt != null)
                        {
                            Stok stok = stokRepo.FindBy(e => e.ProfilBoyId == profilBoyEnt.Id && e.ProfilId == item.ProfilId).FirstOrDefault();
                            if (stok != null)
                            {
                                stok.StokAdet = stok.StokAdet - item.ProfilAdet;
                                stokRepo.EditAndSave(stok);
                            }
                        }
                    }

                    //Fire stok için de kontrol yapılacak 
                    if (siparis.SiparisTur == "Demonte Gönderim")
                    {

                    }


                    foreach (var item in siparisSevkiyatAksesuar)
                    {
                        StokAksesuar stokAksesuar = stokAksesuarRepo.FindBy(e => e.AksesuarId == item.AksesuarId).FirstOrDefault();
                        if (stokAksesuar != null)
                        {
                            int stokAdet = Convert.ToInt32(item.AksesuarAdet / 100);
                            stokAksesuar.StokAdet = stokAksesuar.StokAdet - stokAdet;
                            stokAksesuarRepo.EditAndSave(stokAksesuar);
                        }
                    }

                    return Json("OK", JsonRequestBehavior.AllowGet);
                }
                else if (DurumId == (int)Durumlar.Reddedildi)
                {
                    siparis.OnayIptalKullaniciId = kullaniciModel.Id;
                    siparis.DurumId = (int)Durumlar.Reddedildi;
                    siparis.OnayIptalTarihi = DateTime.Now;
                    siparis.GuncellemeTarihi = DateTime.Now;
                    siparisRepo.EditAndSave(siparis);

                    return Json("NOK", JsonRequestBehavior.AllowGet);
                }
                else if (DurumId == (int)Durumlar.TeslimEdildi)
                {
                    siparis.DurumId = (int)Durumlar.TeslimEdildi;
                    siparis.TeslimTarihi = DateTime.Now;
                    siparis.GuncellemeTarihi = DateTime.Now;
                    siparisRepo.EditAndSave(siparis);

                    return Json("OK", JsonRequestBehavior.AllowGet);
                }

                return Json("NOT", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "ONAYLAMA")]
        public ActionResult SiparisImalatDurumGuncelleme(long SiparisId, string Durum)
        {
            try
            {
                siparisRepo = new SiparisRepo();
                Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
                Kullanici kullaniciModel = (Kullanici)Session["CurrentUser"];

                string imalatDurum = string.Empty;
                switch (Durum)
                {
                    case "islembekleme": imalatDurum = "İŞLEM BEKLEMEDE"; break;
                    case "boyada": imalatDurum = "BOYADA"; break;
                    case "cambekleme": imalatDurum = "CAM BEKLİYOR"; break;
                    case "imalatbekleme": imalatDurum = "İMALATI BEKLİYOR"; break;
                    case "imalatta": imalatDurum = "İMALATTA"; break;
                    case "odemebekleme": imalatDurum = "ÖDEME BEKLİYOR"; break;
                    case "sevkiyatbekleme": imalatDurum = "SEVKİYAT BEKLİYOR"; break;
                    case "sevkedildi": imalatDurum = "SEVK EDİLDİ"; break;
                    case "disImalat": imalatDurum = "DIŞ İMALATTA"; break;
                    default:
                        break;
                }

                siparis.ImalatDurum = imalatDurum;
                siparis.GuncellemeTarihi = DateTime.Now;
                siparisRepo.EditAndSave(siparis);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [AuthLog(Roles = "ONAYLAMA,DUZENLEME")]
        public ActionResult SiparisDurumGuncelleFisGir(long SiparisId, int DurumId, string FisNo)
        {
            try
            {
                siparisRepo = new SiparisRepo();
                Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();

                if (DurumId == (int)Durumlar.TeslimEdildi)
                {
                    siparis.FisNo = FisNo;
                    siparis.DurumId = (int)Durumlar.TeslimEdildi;
                    siparis.TeslimTarihi = DateTime.Now;
                    siparis.GuncellemeTarihi = DateTime.Now;
                    siparisRepo.EditAndSave(siparis);

                    return Json("OK", JsonRequestBehavior.AllowGet);
                }

                return Json("NOT", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "ONAYLAMA,DUZENLEME")]
        public ActionResult SiparisIslemAl(long SiparisId, string FisNo)
        {
            try
            {
                siparisRepo = new SiparisRepo();
                Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();

                siparis.FisNo = FisNo;
                siparis.IslemDurum = "İşlem Onaylandı";
                siparis.GuncellemeTarihi = DateTime.Now;
                siparisRepo.EditAndSave(siparis);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "ONAYLAMA,DUZENLEME")]
        public ActionResult SiparisFisNoDuzenle(long SiparisId, string FisNo)
        {
            try
            {
                siparisRepo = new SiparisRepo();
                Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();

                siparis.FisNo = FisNo;
                siparis.GuncellemeTarihi = DateTime.Now;
                siparisRepo.EditAndSave(siparis);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "ONAYLAMA,DUZENLEME")]
        public ActionResult SiparisBirimFiyatGuncelle(long SiparisId, decimal BirimFiyat)
        {
            try
            {
                siparisRepo = new SiparisRepo();
                Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();

                siparis.SistemBirimFiyat = BirimFiyat;
                siparis.GuncellemeTarihi = DateTime.Now;
                siparisRepo.EditAndSave(siparis);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthLog(Roles = "ONAYLAMA,DUZENLEME")]
        public JsonResult SiparisToplamFiyatveToplamKgGuncelle(long SiparisId, string[] Bilgiler)
        {
            try
            {
                siparisRepo = new SiparisRepo();
                Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();

                //profil gönderiminde ve stoktan siparişte alü kg fiyatı güncelleniyor.
                if (Bilgiler[0] != "")
                    siparis.SistemBirimFiyat = Convert.ToDecimal(Bilgiler[0]);

                if (Bilgiler[1] != "")
                    siparis.ToplamAluKg = Convert.ToInt32(Convert.ToDouble(Bilgiler[1]) * 1000);

                if (Bilgiler[2] != "")
                    siparis.ToplamAluKgFiyat = Convert.ToDecimal(Bilgiler[2]);

                siparis.GuncellemeTarihi = DateTime.Now;
                siparisRepo.EditAndSave(siparis);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Silme
        [HttpPost]
        [AuthLog(Roles = "SILME")]
        public ActionResult SiparisSil(long SiparisId)
        {
            siparisAksesuarRepo = new SiparisAksesuarRepo();
            sebaRepo = new SiparisEnBoyAdetRepo();
            siparisRepo = new SiparisRepo();
            scRepo = new SiparisCamRepo();
            OptimizasyonHesapRepo optimizasyonHesapRepo = new OptimizasyonHesapRepo();

            try
            {
                var aksesuar = siparisAksesuarRepo.FindBy(e => e.SiparisId == SiparisId).ToList();
                foreach (var item in aksesuar)
                    siparisAksesuarRepo.DeleteAndSave(item);

                var optimizasyon = optimizasyonHesapRepo.FindBy(e => e.SiparisIds == SiparisId.ToString()).ToList();
                foreach (var item in optimizasyon)
                    optimizasyonHesapRepo.DeleteAndSave(item);

                var enBoyAdet = sebaRepo.FindBy(e => e.SiparisId == SiparisId).ToList();
                foreach (var item in enBoyAdet)
                    sebaRepo.DeleteAndSave(item);

                var siparisCam = scRepo.FindBy(e => e.SiparisId == SiparisId).FirstOrDefault();
                if (siparisCam != null)
                    scRepo.DeleteAndSave(siparisCam);

                var silinecekSiparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
                if (silinecekSiparis != null)
                    siparisRepo.DeleteAndSave(silinecekSiparis);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Detay yardımcıları
        public JsonResult ProfilDetayBilgisiGetir(int ProfilId, long SiparisId)
        {
            profilRepo = new ProfilRepo();
            siparisRepo = new SiparisRepo();
            var profil = profilRepo.FindBy(e => e.Id == ProfilId).FirstOrDefault();
            var siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
            string renk = "";

            if (siparis != null && siparis.Renk != null)
                renk = siparis.Renk.RenkAdi;

            if (profil == null)
                return Json(new { result = "NOK" }, JsonRequestBehavior.AllowGet);

            return Json(new { result = profil, siparisRenk = renk }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AksesuarDetayBilgisiGetir(int AksesuarId, long SiparisId)
        {
            aksesuarRepo = new AksesuarRepo();
            siparisRepo = new SiparisRepo();
            var aksesuar = aksesuarRepo.FindBy(e => e.Id == AksesuarId).FirstOrDefault();

            if (aksesuar == null)
                return Json(new { result = "NOK" }, JsonRequestBehavior.AllowGet);

            return Json(new { result = aksesuar }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region İndirme (Index.cshtml çağırıyor)
        // Not: Bunlar eski çalışan controller'dan taşındı.
        // Projede action isimleri / view'lar farklıysa uyarlamak gerekebilir.

        public FileResult SiparisIndir(string file)
        {
            string basePath = Server.MapPath("~/Assets/temp/");

            // Sanitize file parameter to prevent path traversal
            string safeFile = Path.GetFileName(file);
            if (string.IsNullOrWhiteSpace(safeFile))
                throw new ArgumentException("Geçersiz dosya adı.");

            string fullPath = Path.Combine(basePath, safeFile);
            if (!fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Geçersiz dosya yolu.");

            try
            {
                System.IO.File.Delete(fullPath);
            }
            catch { }

            long siparisId = Convert.ToInt64(safeFile.Split('_')[0]);
            excelKaydet(siparisId);

            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, safeFile);
        }

        [HttpGet]
        public ActionResult StoktanSiparisPdfYazdir(long SiparisId)
        {
            SabitRepo sabitRepo = new SabitRepo();
            AdresRepo adresRepo = new AdresRepo();
            SiparisStokSablon sablon = new SiparisStokSablon();
            decimal aluKgFiyat = Convert.ToDecimal(sabitRepo.FindBy(e => e.Id == 2).FirstOrDefault().SabitDeger) / 100;
            profilRepo = new ProfilRepo();
            profilBoyRepo = new ProfilBoyRepo();
            siparisStokRepo = new SiparisStokRepo();
            siparisRepo = new SiparisRepo();
            aksesuarRepo = new AksesuarRepo();
            musteriRepo = new MusteriRepo();

            List<SiparisStok> siparisStok = siparisStokRepo.FindBy(e => e.SiparisId == SiparisId).ToList();
            Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
            List<SiparisStokProfil> profilList = new List<SiparisStokProfil>();
            List<SiparisStokAksesuar> aksesuarList = new List<SiparisStokAksesuar>();

            if (siparis.SistemBirimFiyat != null)
                aluKgFiyat = (decimal)siparis.SistemBirimFiyat;

            foreach (var item in siparisStok.Where(e => e.ProfilId != null).ToList())
            {
                Profil profil = profilRepo.FindBy(e => e.Id == item.ProfilId).FirstOrDefault();
                SiparisStokProfil siparisStokProfil = new SiparisStokProfil();

                siparisStokProfil.Kodu = profil.ProfilKodu;
                siparisStokProfil.Adi = profil.ProfilAdi;
                siparisStokProfil.Kesit = profil.ProfilFoto;
                siparisStokProfil.BirimAgirlik = (double)profil.BirimAgirlik / 1000;
                siparisStokProfil.Birim = "BOY";
                siparisStokProfil.Renk = siparis.Renk.RenkAdi;
                siparisStokProfil.Miktar = (int)item.ProfilAdet;
                siparisStokProfil.Olcu = Convert.ToDouble(profilBoyRepo.FindBy(e => e.Id == item.ProfilBoyId).FirstOrDefault().ProfilBoyu) / 1000;
                siparisStokProfil.ToplamMetre = (double)(siparisStokProfil.Olcu * siparisStokProfil.Miktar);

                siparisStokProfil.ToplamKg = siparisStokProfil.BirimAgirlik * siparisStokProfil.ToplamMetre;
                siparisStokProfil.BirimFiyatKgM = aluKgFiyat;
                siparisStokProfil.ToplamTutar = siparisStokProfil.BirimFiyatKgM * (decimal)siparisStokProfil.ToplamKg;

                // sac boru aksesuarlarda görünecek
                if (!profil.ProfilKodu.Equals("SB-101"))
                {
                    profilList.Add(siparisStokProfil);
                }
                else
                {
                    SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                    siparisStokAksesuar.Kodu = profil.ProfilKodu;
                    siparisStokAksesuar.Adi = profil.ProfilAdi;
                    siparisStokAksesuar.Birim = "METRE";
                    siparisStokAksesuar.BirimFiyat = Convert.ToDecimal(sabitRepo.FindBy(e => e.Id == 6).FirstOrDefault().SabitDeger) / 100;
                    siparisStokAksesuar.Miktar = (decimal)siparisStokProfil.ToplamMetre;
                    siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.BirimFiyat * (decimal)siparisStokProfil.ToplamMetre;
                    aksesuarList.Add(siparisStokAksesuar);
                }
            }


            foreach (var item in siparisStok.Where(e => e.AksesuarId != null).ToList())
            {
                Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == item.AksesuarId).FirstOrDefault();
                SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                siparisStokAksesuar.Adi = aksesuar.AksesuarAdi;
                siparisStokAksesuar.Birim = aksesuar.AksesuarBirim;
                siparisStokAksesuar.BirimFiyat = (decimal)aksesuar.BirimFiyat;
                siparisStokAksesuar.Kodu = aksesuar.AksesuarKodu;
                siparisStokAksesuar.Miktar = (int)item.AksesuarAdet;

                siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.Miktar * siparisStokAksesuar.BirimFiyat;
                aksesuarList.Add(siparisStokAksesuar);
            }

            sablon.aksesuarList = aksesuarList;
            sablon.profilList = profilList;
            sablon.SiparisId = SiparisId;
            sablon.SiparisTarih = Convert.ToDateTime(siparis.TahminiTeslim);

            if (siparis.ToplamAluKg != null)
                sablon.ProfilToplamKg = (double)siparis.ToplamAluKg / 1000;
            else
                sablon.ProfilToplamKg = sablon.profilList.Sum(e => e.ToplamKg);

            if (siparis.ToplamAluKgFiyat != null)
                sablon.ProfilToplamTutar = (decimal)siparis.ToplamAluKgFiyat;
            else
                sablon.ProfilToplamTutar = sablon.profilList.Sum(e => e.ToplamTutar);

            sablon.AksesuarToplamTutar = aksesuarList.Sum(e => e.ToplamTutar);
            sablon.SirketAd = siparis.MusteriTamAdi;

            int adresId = (int)musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault().AdresId;
            Adres adres = adresRepo.FindBy(e => e.Id == adresId).FirstOrDefault();
            sablon.SirketAdres = adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;
            ViewBag.AluKg = aluKgFiyat;
            return PartialView("_stoktanSiparisSablon4Pdf", sablon);
        }

        public ActionResult SiparisPdfYazdir(long SiparisId)
        {
            SabitRepo sabitRepo = new SabitRepo();
            AdresRepo adresRepo = new AdresRepo();
            SistemAltSistemJoinRepo sistemAltSistemJoinRepo = new SistemAltSistemJoinRepo();
            SiparisStokSablon sablon = new SiparisStokSablon();
            decimal aluKgFiyat = Convert.ToDecimal(sabitRepo.FindBy(e => e.Id == 2).FirstOrDefault().SabitDeger) / 100;
            profilRepo = new ProfilRepo();
            profilBoyRepo = new ProfilBoyRepo();
            siparisStokRepo = new SiparisStokRepo();
            siparisRepo = new SiparisRepo();
            aksesuarRepo = new AksesuarRepo();
            musteriRepo = new MusteriRepo();
            sebaRepo = new SiparisEnBoyAdetRepo();
            siparisAksesuarRepo = new SiparisAksesuarRepo();
            scRepo = new SiparisCamRepo();

            List<SiparisStok> siparisStok = siparisStokRepo.FindBy(e => e.SiparisId == SiparisId).ToList();
            Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
            List<SiparisStokProfil> profilList = new List<SiparisStokProfil>();
            List<SiparisStokAksesuar> aksesuarList = new List<SiparisStokAksesuar>();
            List<SiparisAksesuar> siparisAksesuar = siparisAksesuarRepo.FindBy(e => e.SiparisId == SiparisId).ToList();
            SiparisCam siparisCam = scRepo.FindBy(e => e.SiparisId == SiparisId).FirstOrDefault();

            Musteri musteri = musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault();
            int adresId = (int)musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault().AdresId;
            Adres adres = adresRepo.FindBy(e => e.Id == adresId).FirstOrDefault();
            sablon.SirketAdres = adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;

            if (siparis.SistemBirimFiyat != null)
                aluKgFiyat = (decimal)siparis.SistemBirimFiyat;

            ViewBag.AluKg = aluKgFiyat;
            SistemAltSistemJoin sistemAltSistemJoin = sistemAltSistemJoinRepo.FindBy(e => e.SistemId == siparis.SistemId &&
                                                            e.AltSistemId == siparis.AltSistemId && e.SistemTurId == siparis.SistemTurId).FirstOrDefault();
            if (siparis.SiparisTur == "Profil Gönderim")
            {
                List<SiparisEnBoyAdet> enBoyList = sebaRepo.FindBy(e => e.SiparisId == SiparisId).ToList();
                SiparisTeklifRepo siparisTeklifRepo = new SiparisTeklifRepo();

                List<OptimizasyonHesap> optimizasyonHesaps = GetOrRunOptimizasyonHesaps(siparis.Id);

                List<int> profDist = optimizasyonHesaps.Select(e => (int)e.ProfilId).Distinct().ToList();
                List<ProfilGonderimSablonModel> profilGonderimList = new List<ProfilGonderimSablonModel>();

                foreach (var item in profDist)
                {
                    Dictionary<int, int> profilBoyDict = optimizasyonHesaps.Where(e => e.ProfilId == item).GroupBy(e => (int)e.ProfilBoy).ToDictionary(d => d.Key, d => d.Sum(e => (int)e.KesimAdet));
                    foreach (var pb in profilBoyDict)
                    {
                        ProfilGonderimSablonModel ent = new ProfilGonderimSablonModel();
                        ent.ProfilId = item;
                        ent.ProfilBoy = pb.Key;
                        ent.ProfilAdet = pb.Value;
                        profilGonderimList.Add(ent);
                    }
                }

                foreach (var item in profilGonderimList)
                {
                    Profil profil = profilRepo.FindBy(e => e.Id == item.ProfilId).FirstOrDefault();
                    SiparisStokProfil siparisStokProfil = new SiparisStokProfil();

                    siparisStokProfil.Kodu = profil.ProfilKodu;
                    siparisStokProfil.Adi = profil.ProfilAdi;
                    siparisStokProfil.Kesit = profil.ProfilFoto;
                    siparisStokProfil.BirimAgirlik = (double)profil.BirimAgirlik / 1000;
                    siparisStokProfil.Birim = "BOY";
                    siparisStokProfil.Renk = siparis.Renk.RenkAdi;
                    siparisStokProfil.Olcu = (double)item.ProfilBoy / 1000;
                    siparisStokProfil.Miktar = item.ProfilAdet;
                    siparisStokProfil.ToplamMetre = (double)(siparisStokProfil.Olcu * siparisStokProfil.Miktar);
                    siparisStokProfil.ToplamKg = siparisStokProfil.BirimAgirlik * siparisStokProfil.ToplamMetre;

                    siparisStokProfil.BirimFiyatKgM = aluKgFiyat;
                    siparisStokProfil.ToplamTutar = siparisStokProfil.BirimFiyatKgM * (decimal)siparisStokProfil.ToplamKg;

                    // sac boru aksesuarlarda görünecek
                    if (!profil.ProfilKodu.Equals("SB-101"))
                    {
                        profilList.Add(siparisStokProfil);
                    }
                    else
                    {
                        SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                        siparisStokAksesuar.Kodu = profil.ProfilKodu;
                        siparisStokAksesuar.Adi = profil.ProfilAdi;
                        siparisStokAksesuar.Birim = "METRE";
                        siparisStokAksesuar.BirimFiyat = Convert.ToDecimal(sabitRepo.FindBy(e => e.Id == 6).FirstOrDefault().SabitDeger) / 100;
                        siparisStokAksesuar.Miktar = (decimal)siparisStokProfil.ToplamMetre;
                        siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.BirimFiyat * (decimal)siparisStokProfil.ToplamMetre;
                        aksesuarList.Add(siparisStokAksesuar);
                    }
                }

                foreach (var item in siparisAksesuarRepo.FindBy(e => e.SiparisId == siparis.Id).ToList())
                {
                    Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == item.AksesuarId).FirstOrDefault();
                    SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                    siparisStokAksesuar.Adi = aksesuar.AksesuarAdi;
                    siparisStokAksesuar.Birim = aksesuar.AksesuarBirim;
                    siparisStokAksesuar.Kodu = aksesuar.AksesuarKodu;

                    List<long> enBoyAdetIds = enBoyList.Select(e => e.Id).ToList();
                    List<SiparisTeklif> siparisTeklifs = siparisTeklifRepo.GetAll().Where(e => enBoyAdetIds.Contains((long)e.SiparisEnBoyAdetId)).ToList();
                    List<SiparisTeklif> filteredList = siparisTeklifs.Where(e => e.Malzeme.Equals(aksesuar.AksesuarAdi)).ToList();

                    siparisStokAksesuar.BirimFiyat = (decimal)aksesuar.BirimFiyat;
                    if (item.BirimFiyat != null && item.BirimFiyat > 0)
                        siparisStokAksesuar.BirimFiyat = item.BirimFiyat.Value;

                    siparisStokAksesuar.Miktar = filteredList.Sum(e => (decimal)e.Miktar);//sorulacak

                    siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.Miktar * siparisStokAksesuar.BirimFiyat;
                    aksesuarList.Add(siparisStokAksesuar);
                }

                sablon.aksesuarList = aksesuarList;
                sablon.profilList = profilList;
                sablon.SiparisId = SiparisId;
                sablon.SiparisTarih = Convert.ToDateTime(siparis.TahminiTeslim);

                if (siparis.ToplamAluKg != null)
                    sablon.ProfilToplamKg = (double)siparis.ToplamAluKg / 1000;
                else
                    sablon.ProfilToplamKg = sablon.profilList.Where(e => !e.Kodu.Contains("DP-")).Sum(e => e.ToplamKg);

                if (siparis.ToplamAluKgFiyat != null)
                    sablon.ProfilToplamTutar = (decimal)siparis.ToplamAluKgFiyat;
                else
                    sablon.ProfilToplamTutar = sablon.profilList.Where(e => !e.Kodu.Contains("DP-")).Sum(e => e.ToplamTutar);

                sablon.AksesuarToplamTutar = aksesuarList.Sum(e => e.ToplamTutar);
                sablon.SirketAd = siparis.MusteriTamAdi;

                return PartialView("_stoktanSiparisSablon4Pdf", sablon);
            }
            else
            {
                Teklif4Pdf sablonPdf = new Teklif4Pdf();
                sablonPdf = SistemCiktisi.demonteGonderimi(SiparisId);

                return PartialView(sablonPdf.PartialAdi, sablonPdf);
            }
        }

        void excelStoktanKaydet(long siparisId)
        {
            string pathAfter = Server.MapPath("~/Assets/temp/") + siparisId + "_nolu_siparis";

            SabitRepo sabitRepo = new SabitRepo();
            AdresRepo adresRepo = new AdresRepo();
            SiparisStokSablon sablon = new SiparisStokSablon();
            decimal aluKgFiyat = Convert.ToDecimal(sabitRepo.FindBy(e => e.Id == 2).FirstOrDefault().SabitDeger) / 100;
            profilRepo = new ProfilRepo();
            profilBoyRepo = new ProfilBoyRepo();
            siparisStokRepo = new SiparisStokRepo();
            siparisRepo = new SiparisRepo();
            aksesuarRepo = new AksesuarRepo();
            musteriRepo = new MusteriRepo();

            List<SiparisStok> siparisStok = siparisStokRepo.FindBy(e => e.SiparisId == siparisId).ToList();
            Siparis siparis = siparisRepo.FindBy(e => e.Id == siparisId).FirstOrDefault();

            if (siparis.SistemBirimFiyat != null)
                aluKgFiyat = (decimal)siparis.SistemBirimFiyat;

            List<SiparisStokProfil> profilList = new List<SiparisStokProfil>();
            List<SiparisStokAksesuar> aksesuarList = new List<SiparisStokAksesuar>();
            foreach (var item in siparisStok.Where(e => e.ProfilId != null).ToList())
            {
                Profil profil = profilRepo.FindBy(e => e.Id == item.ProfilId).FirstOrDefault();

                SiparisStokProfil siparisStokProfil = new SiparisStokProfil();

                siparisStokProfil.Kodu = profil.ProfilKodu;
                siparisStokProfil.Adi = profil.ProfilAdi;
                siparisStokProfil.Kesit = profil.ProfilFoto;
                siparisStokProfil.BirimAgirlik = (double)profil.BirimAgirlik / 1000;
                siparisStokProfil.Birim = "BOY";
                siparisStokProfil.Renk = siparis.Renk.RenkAdi;
                siparisStokProfil.Miktar = (int)item.ProfilAdet;
                siparisStokProfil.Olcu = Convert.ToDouble(profilBoyRepo.FindBy(e => e.Id == item.ProfilBoyId).FirstOrDefault().ProfilBoyu) / 1000;
                siparisStokProfil.ToplamMetre = (double)(siparisStokProfil.Olcu * siparisStokProfil.Miktar);

                siparisStokProfil.ToplamKg = siparisStokProfil.BirimAgirlik * siparisStokProfil.ToplamMetre;
                siparisStokProfil.BirimFiyatKgM = aluKgFiyat;
                siparisStokProfil.ToplamTutar = siparisStokProfil.BirimFiyatKgM * (decimal)siparisStokProfil.ToplamKg;

                if (!profil.ProfilKodu.Equals("SB-101"))
                {
                    profilList.Add(siparisStokProfil);
                }
                else
                {
                    SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                    siparisStokAksesuar.Kodu = profil.ProfilKodu;
                    siparisStokAksesuar.Adi = profil.ProfilAdi;
                    siparisStokAksesuar.Birim = "METRE";
                    siparisStokAksesuar.BirimFiyat = Convert.ToDecimal(sabitRepo.FindBy(e => e.Id == 6).FirstOrDefault().SabitDeger) / 100;
                    siparisStokAksesuar.Miktar = (decimal)siparisStokProfil.ToplamMetre;
                    siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.BirimFiyat * (decimal)siparisStokProfil.ToplamMetre;
                    aksesuarList.Add(siparisStokAksesuar);
                }
            }

            foreach (var item in siparisStok.Where(e => e.AksesuarId != null).ToList())
            {
                Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == item.AksesuarId).FirstOrDefault();
                SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                siparisStokAksesuar.Adi = aksesuar.AksesuarAdi;
                siparisStokAksesuar.Birim = aksesuar.AksesuarBirim;
                siparisStokAksesuar.BirimFiyat = (decimal)aksesuar.BirimFiyat;
                siparisStokAksesuar.Kodu = aksesuar.AksesuarKodu;
                siparisStokAksesuar.Miktar = (int)item.AksesuarAdet;

                siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.Miktar * siparisStokAksesuar.BirimFiyat;
                aksesuarList.Add(siparisStokAksesuar);
            }

            sablon.aksesuarList = aksesuarList;
            sablon.profilList = profilList;
            sablon.SiparisId = siparisId;
            sablon.SiparisTarih = Convert.ToDateTime(siparis.TahminiTeslim);
            sablon.AksesuarToplamTutar = aksesuarList.Sum(e => e.ToplamTutar);
            sablon.SirketAd = siparis.MusteriTamAdi;

            if (siparis.ToplamAluKg != null)
                sablon.ProfilToplamKg = (double)siparis.ToplamAluKg / 1000;
            else
                sablon.ProfilToplamKg = sablon.profilList.Sum(e => e.ToplamKg);

            if (siparis.ToplamAluKgFiyat != null)
                sablon.ProfilToplamTutar = (decimal)siparis.ToplamAluKgFiyat;
            else
                sablon.ProfilToplamTutar = sablon.profilList.Sum(e => e.ToplamTutar);

            int adresId = (int)musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault().AdresId;
            Adres adres = adresRepo.FindBy(e => e.Id == adresId).FirstOrDefault();

            ViewBag.AluKg = aluKgFiyat;

            string path = Server.MapPath("~/Assets/sablonStokYeni.xlsx");

            ExcelPackage excel = new ExcelPackage(new FileInfo(path));
            ExcelWorksheet xlWorkSheet = excel.Workbook.Worksheets.First();
            xlWorkSheet.Cells.Style.Font.Name = "Arial";
            xlWorkSheet.Cells["A5"].Value = siparis.MusteriTamAdi;
            xlWorkSheet.Cells["D5"].Value = adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;
            xlWorkSheet.Cells["C6"].Value = siparisId;
            xlWorkSheet.Cells["G6"].Value = siparis.KayitTarihi;
            xlWorkSheet.Cells["K6"].Value = siparis.TeslimTarihi;

            int k = 0;
            int i = 8;
            int uniqueName = 0;
            foreach (var item in sablon.profilList)
            {
                k = k + 1;
                i = i + 1;

                xlWorkSheet.Row(i).Height = 34.5;
                //xlWorkSheet.Cells[i, 1].Value = k;
                //xlWorkSheet.Cells[i, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 1].Value = item.Kodu;
                xlWorkSheet.Cells[i, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 2].Value = item.Adi;
                xlWorkSheet.Cells[i, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                if (item.Kesit != null)
                {
                    var imagePath = Server.MapPath("/images/profilicons/" + item.Kesit);
                    Image img = Image.FromFile(imagePath);

                    int iColumnWidth = (int)((xlWorkSheet.Column(3).Width - 1) * 7) + 12;
                    int iColumnHeight = (int)(xlWorkSheet.Row(i).Height * 1.333);
                    int xOffset = iColumnWidth / 2 - img.Width / 2;
                    int yOffset = iColumnHeight / 2 - img.Height / 2;

                    var pic = xlWorkSheet.Drawings.AddPicture(uniqueName++.ToString(), img);
                    pic.SetPosition(i - 1, yOffset, 2, xOffset);
                }

                xlWorkSheet.Cells[i, 4].Value = item.BirimAgirlik;
                xlWorkSheet.Cells[i, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 5].Value = item.Birim;
                xlWorkSheet.Cells[i, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 6].Value = item.Renk;
                xlWorkSheet.Cells[i, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 7].Value = item.Olcu;
                xlWorkSheet.Cells[i, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 8].Value = item.Miktar;
                xlWorkSheet.Cells[i, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                xlWorkSheet.Cells[i, 9].Value = item.ToplamMetre;
                xlWorkSheet.Cells[i, 9].Style.Numberformat.Format = "0.00";
                xlWorkSheet.Cells[i, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 10].Value = item.ToplamKg + " Kg";
                xlWorkSheet.Cells[i, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 11].Value = item.BirimFiyatKgM;
                xlWorkSheet.Cells[i, 11].Style.Numberformat.Format = "#,##0.00 ₺";
                xlWorkSheet.Cells[i, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 12].Value = item.ToplamTutar;
                xlWorkSheet.Cells[i, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                xlWorkSheet.Cells[i, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[string.Format("A{0}:L{0}", i)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                xlWorkSheet.Cells[string.Format("A{0}:L{0}", i)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                xlWorkSheet.Cells[string.Format("A{0}:L{0}", i)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                xlWorkSheet.Cells[string.Format("A{0}:L{0}", i)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                if (k != sablon.profilList.Count)
                {
                    xlWorkSheet.InsertRow(i + 1, 1);
                    xlWorkSheet.Row(i + 1).Height = 34.5;
                }
            }

            xlWorkSheet.Cells[i + 1, 10].Value = Math.Round(sablon.ProfilToplamKg, 2) + " Kg";
            xlWorkSheet.Cells[i + 1, 10].Style.Font.Bold = true;
            xlWorkSheet.Cells[i + 1, 12].Value = sablon.ProfilToplamTutar;
            xlWorkSheet.Cells[i + 1, 12].Style.Numberformat.Format = "#,##0.00 ₺";
            xlWorkSheet.Cells[i + 1, 12].Style.Font.Bold = true;

            int x = 0;
            if (sablon.profilList.Count > 0)
                x = i + 3;
            else
                x = i + 4;
            int j = 0;

            foreach (var item in sablon.aksesuarList)
            {
                j = j + 1;
                k = k + 1;
                x = x + 1;

                xlWorkSheet.Row(x).Height = 25;
                //xlWorkSheet.Cells[x, 1].Value = k;
                //xlWorkSheet.Cells[x, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[x, 1].Value = item.Kodu;
                xlWorkSheet.Cells[x, 2].Value = item.Adi;
                xlWorkSheet.Cells[x, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[x, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                xlWorkSheet.Cells[string.Format("F{0}:J{0}", x)].Merge = true;
                xlWorkSheet.Cells[x, 4].Value = item.Birim;
                xlWorkSheet.Cells[x, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[x, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[x, 5].Value = item.Miktar;
                xlWorkSheet.Cells[x, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[x, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[x, 11].Value = item.BirimFiyat;
                xlWorkSheet.Cells[x, 11].Style.Numberformat.Format = "#,##0.00 ₺";
                xlWorkSheet.Cells[x, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[x, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[x, 12].Value = item.ToplamTutar;
                xlWorkSheet.Cells[x, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                xlWorkSheet.Cells[x, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[x, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                xlWorkSheet.Cells[string.Format("A{0}:L{0}", x)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                xlWorkSheet.Cells[string.Format("A{0}:L{0}", x)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                xlWorkSheet.Cells[string.Format("A{0}:L{0}", x)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                xlWorkSheet.Cells[string.Format("A{0}:L{0}", x)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                if (j != sablon.aksesuarList.Count)
                {
                    xlWorkSheet.InsertRow(x + 1, 1);
                    xlWorkSheet.Row(x + 1).Height = 34.5;
                }
            }

            //if (sablon.aksesuarList.Count < 1)
            //    x = x + 1;
            xlWorkSheet.Cells[x + 1, 12].Value = sablon.AksesuarToplamTutar;
            xlWorkSheet.Cells[x + 3, 12].Value = sablon.ProfilToplamTutar + sablon.AksesuarToplamTutar;
            xlWorkSheet.Cells[x + 4, 12].Value = Convert.ToDecimal(sablon.ProfilToplamTutar + sablon.AksesuarToplamTutar) * 20 / 100;
            xlWorkSheet.Cells[x + 5, 12].Value = (Convert.ToDecimal(sablon.ProfilToplamTutar + sablon.AksesuarToplamTutar) * 20 / 100) + sablon.ProfilToplamTutar + sablon.AksesuarToplamTutar;

            excel.SaveAs(new FileInfo(pathAfter + ".xlsx"));
            excel.Dispose();
        }

        void excelKaydet(long siparisId)
        {
            string pathAfter = Server.MapPath("~/Assets/temp/") + siparisId + "_nolu_siparis";

            siparisRepo = new SiparisRepo();
            sebaRepo = new SiparisEnBoyAdetRepo();
            scRepo = new SiparisCamRepo();
            siparisAksesuarRepo = new SiparisAksesuarRepo();
            musteriRepo = new MusteriRepo();
            aksesuarRepo = new AksesuarRepo();
            AdresRepo adresRepo = new AdresRepo();
            SabitRepo sabitRepo = new SabitRepo();
            SiparisTeklifRepo siparisTeklifRepo = new SiparisTeklifRepo();

            Siparis siparis = siparisRepo.FindBy(e => e.Id == siparisId).FirstOrDefault();
            SiparisCam siparisCam = scRepo.FindBy(e => e.SiparisId == siparisId).FirstOrDefault();
            List<SiparisAksesuar> siparisAksesuar = siparisAksesuarRepo.FindBy(e => e.SiparisId == siparisId).ToList();
            List<SiparisEnBoyAdet> enBoyList = sebaRepo.FindBy(e => e.SiparisId == siparisId).ToList();
            Musteri musteri = musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault();
            Adres adres = adresRepo.FindBy(e => e.Id == musteri.AdresId).FirstOrDefault();

            if (siparis.SiparisTur == "Profil Gönderim")
            {
                ViewBag.SiparisTur = "tur_profil";
                SiparisStokSablon sablon = new SiparisStokSablon();
                decimal aluKgFiyat = Convert.ToDecimal(sabitRepo.FindBy(e => e.Id == 2).FirstOrDefault().SabitDeger) / 100;
                ProfilRepo profilRepo = new ProfilRepo();
                siparisRepo = new SiparisRepo();
                aksesuarRepo = new AksesuarRepo();
                musteriRepo = new MusteriRepo();

                if (siparis.SistemBirimFiyat != null)
                    aluKgFiyat = (decimal)siparis.SistemBirimFiyat;

                List<SiparisStokProfil> profilList = new List<SiparisStokProfil>();
                List<SiparisStokAksesuar> aksesuarList = new List<SiparisStokAksesuar>();

                List<OptimizasyonHesap> optimizasyonHesaps = GetOrRunOptimizasyonHesaps(siparis.Id);

                List<int> profDist = optimizasyonHesaps.Select(e => (int)e.ProfilId).Distinct().ToList();
                List<ProfilGonderimSablonModel> profilGonderimList = new List<ProfilGonderimSablonModel>();

                foreach (var item in profDist)
                {
                    Dictionary<int, int> profilBoyDict = optimizasyonHesaps.Where(e => e.ProfilId == item).GroupBy(e => (int)e.ProfilBoy).ToDictionary(d => d.Key, d => d.Sum(e => (int)e.KesimAdet));

                    foreach (var pb in profilBoyDict)
                    {
                        ProfilGonderimSablonModel ent = new ProfilGonderimSablonModel();
                        ent.ProfilId = item;
                        ent.ProfilBoy = pb.Key;
                        ent.ProfilAdet = pb.Value;
                        profilGonderimList.Add(ent);
                    }

                }

                foreach (var item in profilGonderimList)
                {
                    Profil profil = profilRepo.FindBy(e => e.Id == item.ProfilId).FirstOrDefault();
                    SiparisStokProfil siparisStokProfil = new SiparisStokProfil();

                    siparisStokProfil.Kodu = profil.ProfilKodu;
                    siparisStokProfil.Adi = profil.ProfilAdi;
                    siparisStokProfil.Kesit = profil.ProfilFoto;
                    siparisStokProfil.BirimAgirlik = (double)profil.BirimAgirlik / 1000;
                    siparisStokProfil.Birim = "BOY";
                    siparisStokProfil.Renk = siparis.Renk.RenkAdi;
                    siparisStokProfil.Olcu = (double)item.ProfilBoy / 1000;
                    siparisStokProfil.Miktar = item.ProfilAdet;
                    siparisStokProfil.ToplamMetre = (double)(siparisStokProfil.Olcu * siparisStokProfil.Miktar);
                    siparisStokProfil.ToplamKg = siparisStokProfil.BirimAgirlik * siparisStokProfil.ToplamMetre;

                    siparisStokProfil.BirimFiyatKgM = aluKgFiyat;
                    siparisStokProfil.ToplamTutar = siparisStokProfil.BirimFiyatKgM * (decimal)siparisStokProfil.ToplamKg;

                    // sac boru aksesuarlarda görünecek
                    if (!profil.ProfilKodu.Equals("SB-101"))
                    {
                        profilList.Add(siparisStokProfil);
                    }
                    else
                    {
                        SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                        siparisStokAksesuar.Kodu = profil.ProfilKodu;
                        siparisStokAksesuar.Adi = profil.ProfilAdi;
                        siparisStokAksesuar.Birim = "METRE";
                        siparisStokAksesuar.BirimFiyat = Convert.ToDecimal(sabitRepo.FindBy(e => e.Id == 6).FirstOrDefault().SabitDeger) / 100;
                        siparisStokAksesuar.Miktar = (decimal)siparisStokProfil.ToplamMetre;
                        siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.BirimFiyat * (decimal)siparisStokProfil.ToplamMetre;
                        aksesuarList.Add(siparisStokAksesuar);
                    }
                }

                foreach (var item in siparisAksesuarRepo.FindBy(e => e.SiparisId == siparis.Id).ToList())
                {
                    Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == item.AksesuarId).FirstOrDefault();
                    SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                    siparisStokAksesuar.Adi = aksesuar.AksesuarAdi;
                    siparisStokAksesuar.Birim = aksesuar.AksesuarBirim;
                    siparisStokAksesuar.Kodu = aksesuar.AksesuarKodu;

                    List<long> enBoyAdetIds = enBoyList.Select(e => e.Id).ToList();
                    List<SiparisTeklif> siparisTeklifs = siparisTeklifRepo.GetAll().Where(e => enBoyAdetIds.Contains((long)e.SiparisEnBoyAdetId)).ToList();
                    List<SiparisTeklif> filteredList = siparisTeklifs.Where(e => e.Malzeme.Equals(aksesuar.AksesuarAdi)).ToList();

                    siparisStokAksesuar.BirimFiyat = (decimal)aksesuar.BirimFiyat;
                    if (item.BirimFiyat != null && item.BirimFiyat > 0)
                        siparisStokAksesuar.BirimFiyat = item.BirimFiyat.Value;

                    siparisStokAksesuar.Miktar = filteredList.Sum(e => (decimal)e.Miktar);//sorulacak

                    siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.Miktar * siparisStokAksesuar.BirimFiyat;
                    aksesuarList.Add(siparisStokAksesuar);
                }

                sablon.aksesuarList = aksesuarList;
                sablon.profilList = profilList;
                sablon.SiparisId = siparis.Id;
                sablon.SiparisTarih = Convert.ToDateTime(siparis.TahminiTeslim);
                sablon.ProfilToplamKg = sablon.profilList.Where(e => !e.Kodu.Contains("DP-")).Sum(e => e.ToplamKg);
                sablon.ProfilToplamTutar = sablon.profilList.Where(e => !e.Kodu.Contains("DP-")).Sum(e => e.ToplamTutar);
                sablon.AksesuarToplamTutar = aksesuarList.Sum(e => e.ToplamTutar);
                sablon.SirketAd = siparis.MusteriTamAdi;

                sablon.SirketAdres = adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;
                ViewBag.AluKg = aluKgFiyat;
                ViewBag.SiparisStokSablon = sablon;

                string path = Server.MapPath("~/Assets/sablonStokYeni.xlsx");

                ExcelPackage excel = new ExcelPackage(new FileInfo(path));
                ExcelWorksheet xlWorkSheet = excel.Workbook.Worksheets.First();

                xlWorkSheet.Cells.Style.Font.Name = "Arial";
                xlWorkSheet.Cells["A5"].Value = siparis.MusteriTamAdi;
                xlWorkSheet.Cells["D5"].Value = adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;
                xlWorkSheet.Cells["C6"].Value = siparisId;
                xlWorkSheet.Cells["G6"].Value = siparis.KayitTarihi;
                xlWorkSheet.Cells["K6"].Value = siparis.TeslimTarihi;

                int k = 0;
                int i = 8;
                List<string> tempKesitler = sablon.profilList.Select(e => e.Kesit).ToList();
                int repeatCount = 0;
                foreach (var item in sablon.profilList)
                {
                    k = k + 1;
                    i = i + 1;

                    xlWorkSheet.Row(i).Height = 34.5;
                    //xlWorkSheet.Cells[i, 1].Value = k;
                    //xlWorkSheet.Cells[i, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[i, 1].Value = item.Kodu;
                    xlWorkSheet.Cells[i, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[i, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i, 2].Value = item.Adi;
                    xlWorkSheet.Cells[i, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    if (item.Kesit != null)
                    {
                        var imagePath = Server.MapPath("/images/profilicons/" + item.Kesit);
                        Image img = Image.FromFile(imagePath);

                        int iColumnWidth = (int)((xlWorkSheet.Column(3).Width - 1) * 7) + 12;
                        int iColumnHeight = (int)(xlWorkSheet.Row(i).Height * 1.333);
                        int xOffset = iColumnWidth / 2 - img.Width / 2;
                        int yOffset = iColumnHeight / 2 - img.Height / 2;
                        string tempKesit = item.Kesit;
                        if (tempKesitler.Count(e => e.Equals(tempKesit)) > 1)
                        {
                            tempKesit += "_" + repeatCount++;
                        }
                        xlWorkSheet.Drawings.AddPicture(tempKesit, img).SetPosition(i - 1, yOffset, 2, xOffset);
                    }

                    xlWorkSheet.Cells[i, 4].Value = item.BirimAgirlik;
                    xlWorkSheet.Cells[i, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[i, 5].Value = item.Birim;
                    xlWorkSheet.Cells[i, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[i, 6].Value = item.Renk;
                    xlWorkSheet.Cells[i, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[i, 7].Value = item.Olcu;
                    xlWorkSheet.Cells[i, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[i, 8].Value = item.Miktar;
                    xlWorkSheet.Cells[i, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    xlWorkSheet.Cells[i, 9].Value = item.ToplamMetre;
                    xlWorkSheet.Cells[i, 9].Style.Numberformat.Format = "0.00";
                    xlWorkSheet.Cells[i, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[i, 10].Value = item.ToplamKg + " Kg";
                    xlWorkSheet.Cells[i, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[i, 11].Value = item.BirimFiyatKgM;
                    xlWorkSheet.Cells[i, 11].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[i, 12].Value = item.ToplamTutar;
                    xlWorkSheet.Cells[i, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[string.Format("A{0}:L{0}", i)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    xlWorkSheet.Cells[string.Format("A{0}:L{0}", i)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    xlWorkSheet.Cells[string.Format("A{0}:L{0}", i)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    xlWorkSheet.Cells[string.Format("A{0}:L{0}", i)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    if (k != sablon.profilList.Count)
                    {
                        xlWorkSheet.InsertRow(i + 1, 1);
                        xlWorkSheet.Row(i + 1).Height = 34.5;
                    }
                }

                repeatCount = 0;
                xlWorkSheet.Cells[i + 1, 10].Value = Math.Round(sablon.ProfilToplamKg, 2) + " Kg";
                xlWorkSheet.Cells[i + 1, 10].Style.Font.Bold = true;
                xlWorkSheet.Cells[i + 1, 12].Value = sablon.ProfilToplamTutar;
                xlWorkSheet.Cells[i + 1, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                xlWorkSheet.Cells[i + 1, 12].Style.Font.Bold = true;

                int x = 0;
                if (sablon.profilList.Count > 0)
                    x = i + 3;
                else
                    x = i + 4;
                int j = 0;

                foreach (var item in sablon.aksesuarList)
                {
                    j = j + 1;
                    k = k + 1;
                    x = x + 1;

                    xlWorkSheet.Row(x).Height = 25;
                    //xlWorkSheet.Cells[x, 1].Value = k;
                    //xlWorkSheet.Cells[x, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[x, 1].Value = item.Kodu;
                    xlWorkSheet.Cells[x, 2].Value = item.Adi;
                    xlWorkSheet.Cells[x, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[x, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    xlWorkSheet.Cells[string.Format("F{0}:J{0}", x)].Merge = true;
                    xlWorkSheet.Cells[x, 4].Value = item.Birim;
                    xlWorkSheet.Cells[x, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[x, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[x, 5].Value = item.Miktar;
                    xlWorkSheet.Cells[x, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[x, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[x, 11].Value = item.BirimFiyat;
                    xlWorkSheet.Cells[x, 11].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[x, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[x, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[x, 12].Value = item.ToplamTutar;
                    xlWorkSheet.Cells[x, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[x, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xlWorkSheet.Cells[x, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    xlWorkSheet.Cells[string.Format("A{0}:L{0}", x)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    xlWorkSheet.Cells[string.Format("A{0}:L{0}", x)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    xlWorkSheet.Cells[string.Format("A{0}:L{0}", x)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    xlWorkSheet.Cells[string.Format("A{0}:L{0}", x)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    if (j != sablon.aksesuarList.Count)
                    {
                        xlWorkSheet.InsertRow(x + 1, 1);
                        xlWorkSheet.Row(x + 1).Height = 34.5;
                    }
                }

                if (sablon.aksesuarList.Count < 1)
                    x = x + 1;
                xlWorkSheet.Cells[x + 1, 12].Value = sablon.AksesuarToplamTutar;
                xlWorkSheet.Cells[x + 3, 12].Value = sablon.ProfilToplamTutar + sablon.AksesuarToplamTutar;
                xlWorkSheet.Cells[x + 4, 12].Value = Convert.ToDecimal(sablon.ProfilToplamTutar + sablon.AksesuarToplamTutar) * 20 / 100;
                xlWorkSheet.Cells[x + 5, 12].Value = (Convert.ToDecimal(sablon.ProfilToplamTutar + sablon.AksesuarToplamTutar) * 20 / 100) + sablon.ProfilToplamTutar + sablon.AksesuarToplamTutar;

                excel.SaveAs(new FileInfo(pathAfter + ".xlsx"));
                excel.Dispose();
            }
            else
            {
                if (siparisCam == null)
                {
                    siparisCam = new SiparisCam();
                    siparisCam.CamKombinasyon = "";
                }

                Teklif4Pdf sablonPdf = new Teklif4Pdf();
                sablonPdf = SistemCiktisi.demonteGonderimi(siparisId);

                string path = Server.MapPath("~/Assets/" + sablonPdf.ExcelAdi);
                ExcelPackage excel = new ExcelPackage(new FileInfo(path));
                ExcelWorksheet xlWorkSheet = excel.Workbook.Worksheets.First();

                xlWorkSheet.Cells[14, 8].Value = sablonPdf.Tarih.ToShortDateString();
                //Firma bilgileri
                xlWorkSheet.Cells[15, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                xlWorkSheet.Cells[15, 3].Style.WrapText = true;
                xlWorkSheet.Cells[15, 3].Value = sablonPdf.Firma;

                xlWorkSheet.Cells[17, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                xlWorkSheet.Cells[17, 3].Style.WrapText = true;
                xlWorkSheet.Cells[17, 3].Value = sablonPdf.Adres;
                xlWorkSheet.Cells[19, 3].Value = sablonPdf.Telefon;
                xlWorkSheet.Cells[21, 9].Value = siparis.SiparisTur;

                // sipariş en boy adet kadar liste dolacak
                //şablonda liste 18.satırda başlıyor
                int i = 23;
                int p = 0;

                //hangi teklife göre kayıt atacağını bulacağız ona göre alan eklenecek ya da silinecek
                if (sablonPdf.DemonteList != null)
                {
                    foreach (Demonte item in sablonPdf.DemonteList)
                    {
                        p = p + 1;
                        i = i + 1;

                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        xlWorkSheet.Cells[i, 3].Value = item.ortak.UrunAciklama;
                        xlWorkSheet.Cells[i, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 3].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 4].Value = item.Motor;
                        xlWorkSheet.Cells[i, 4].AutoFitColumns();
                        xlWorkSheet.Cells[i, 4].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 5].Value = item.Kumanda;
                        xlWorkSheet.Cells[i, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 5].AutoFitColumns();
                        xlWorkSheet.Cells[i, 5].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 7].Value = item.AksesuarSet;
                        xlWorkSheet.Cells[i, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 7].AutoFitColumns();
                        xlWorkSheet.Cells[i, 7].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 6].Value = item.CamKombinasyon;
                        xlWorkSheet.Cells[i, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 6].AutoFitColumns();
                        xlWorkSheet.Cells[i, 6].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 8].Value = item.ortak.Adet;
                        xlWorkSheet.Cells[i, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 9].Value = Convert.ToDouble(item.En) / 1000;
                        xlWorkSheet.Cells[i, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 10].Value = Convert.ToDouble(item.Boy) / 1000;
                        xlWorkSheet.Cells[i, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 11].Value = Convert.ToDouble(item.ortak.Alan);
                        xlWorkSheet.Cells[i, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        xlWorkSheet.Cells[i, 12].Value = item.ortak.BirimFiyat;
                        xlWorkSheet.Cells[i, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                        xlWorkSheet.Cells[i, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        xlWorkSheet.Cells[i, 13].Value = item.ortak.ToplamTutar;
                        xlWorkSheet.Cells[i, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                        xlWorkSheet.Cells[i, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        if (p != enBoyList.Count)
                            xlWorkSheet.InsertRow(i + 1, 1);
                    }

                    xlWorkSheet.Cells[i + 1, 13].Value = Convert.ToDecimal(sablonPdf.Toplam);
                    xlWorkSheet.Cells[i + 1, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 1, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i + 2, 13].Value = Convert.ToDecimal(sablonPdf.KDV);
                    xlWorkSheet.Cells[i + 2, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i + 3, 13].Value = Convert.ToDecimal(sablonPdf.GenelToplam);
                    xlWorkSheet.Cells[i + 3, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 3, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                else if (sablonPdf.CamCatiList != null)
                {
                    foreach (CamCati item in sablonPdf.CamCatiList)
                    {
                        p = p + 1;
                        i = i + 1;

                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        xlWorkSheet.Cells[i, 3].Value = item.ortak.UrunAciklama;
                        xlWorkSheet.Cells[i, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 3].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 4].Value = item.CamKombinasyon;
                        xlWorkSheet.Cells[i, 4].AutoFitColumns();
                        xlWorkSheet.Cells[i, 4].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 5].Value = item.AksesuarSet;
                        xlWorkSheet.Cells[i, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 5].AutoFitColumns();
                        xlWorkSheet.Cells[i, 5].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 6].Value = item.OnYukseklik;
                        xlWorkSheet.Cells[i, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 6].AutoFitColumns();
                        xlWorkSheet.Cells[i, 6].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 7].Value = item.ArkaYukseklik;
                        xlWorkSheet.Cells[i, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 7].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 8].Value = item.ortak.Adet;
                        xlWorkSheet.Cells[i, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 9].Value = Convert.ToDouble(item.En) / 1000;
                        xlWorkSheet.Cells[i, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 10].Value = Convert.ToDouble(item.Boy) / 1000;
                        xlWorkSheet.Cells[i, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 11].Value = Convert.ToDouble(item.ortak.Alan);

                        xlWorkSheet.Cells[i, 12].Value = item.ortak.BirimFiyat;
                        xlWorkSheet.Cells[i, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                        xlWorkSheet.Cells[i, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        xlWorkSheet.Cells[i, 13].Value = item.ortak.ToplamTutar;
                        xlWorkSheet.Cells[i, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                        xlWorkSheet.Cells[i, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        if (p != enBoyList.Count)
                            xlWorkSheet.InsertRow(i + 1, 1);
                    }

                    xlWorkSheet.Cells[i + 1, 13].Value = Convert.ToDecimal(sablonPdf.Toplam);
                    xlWorkSheet.Cells[i + 1, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 1, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i + 2, 13].Value = Convert.ToDecimal(sablonPdf.KDV);
                    xlWorkSheet.Cells[i + 2, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i + 3, 13].Value = Convert.ToDecimal(sablonPdf.GenelToplam);
                    xlWorkSheet.Cells[i + 3, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 3, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                else if (sablonPdf.SurmeList != null)
                {
                    foreach (Surme item in sablonPdf.SurmeList)
                    {
                        p = p + 1;
                        i = i + 1;

                        xlWorkSheet.Cells[string.Format("C{0}:K{0}", i)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:K{0}", i)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:K{0}", i)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:K{0}", i)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        xlWorkSheet.Cells[i, 3].Value = item.ortak.UrunAciklama;
                        xlWorkSheet.Cells[i, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 3].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 4].Value = item.CamKombinasyon;
                        xlWorkSheet.Cells[i, 4].AutoFitColumns();
                        xlWorkSheet.Cells[i, 4].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 5].Value = item.AksesuarSet;
                        xlWorkSheet.Cells[i, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 5].AutoFitColumns();
                        xlWorkSheet.Cells[i, 5].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 6].Value = item.ortak.Adet;
                        xlWorkSheet.Cells[i, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 7].Value = Convert.ToDouble(item.En) / 1000;
                        xlWorkSheet.Cells[i, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 8].Value = Convert.ToDouble(item.Boy) / 1000;
                        xlWorkSheet.Cells[i, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 9].Value = Convert.ToDouble(item.ortak.Alan);

                        xlWorkSheet.Cells[i, 10].Value = item.ortak.BirimFiyat;
                        xlWorkSheet.Cells[i, 10].Style.Numberformat.Format = "#,##0.00 ₺";
                        xlWorkSheet.Cells[i, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        xlWorkSheet.Cells[i, 11].Value = item.ortak.ToplamTutar;
                        xlWorkSheet.Cells[i, 11].Style.Numberformat.Format = "#,##0.00 ₺";
                        xlWorkSheet.Cells[i, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        if (p != enBoyList.Count)
                            xlWorkSheet.InsertRow(i + 1, 1);
                    }

                    xlWorkSheet.Cells[i + 1, 11].Value = Convert.ToDecimal(sablonPdf.Toplam);
                    xlWorkSheet.Cells[i + 1, 11].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 1, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i + 2, 11].Value = Convert.ToDecimal(sablonPdf.KDV);
                    xlWorkSheet.Cells[i + 2, 11].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 2, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i + 3, 11].Value = Convert.ToDecimal(sablonPdf.GenelToplam);
                    xlWorkSheet.Cells[i + 3, 11].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 3, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                else if (sablonPdf.ZipPerdeList != null)
                {
                    foreach (ZipPerde item in sablonPdf.ZipPerdeList)
                    {
                        p = p + 1;
                        i = i + 1;

                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:M{0}", i)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        xlWorkSheet.Cells[i, 3].Value = item.ortak.UrunAciklama;
                        xlWorkSheet.Cells[i, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 3].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 4].Value = item.Motor;
                        xlWorkSheet.Cells[i, 4].AutoFitColumns();
                        xlWorkSheet.Cells[i, 4].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 5].Value = item.Kumanda;
                        xlWorkSheet.Cells[i, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 5].AutoFitColumns();
                        xlWorkSheet.Cells[i, 5].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 6].Value = item.Kumas;
                        xlWorkSheet.Cells[i, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 6].AutoFitColumns();
                        xlWorkSheet.Cells[i, 6].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 7].Value = item.AksesuarSet;
                        xlWorkSheet.Cells[i, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 7].AutoFitColumns();
                        xlWorkSheet.Cells[i, 7].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 8].Value = item.ortak.Adet;
                        xlWorkSheet.Cells[i, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 9].Value = Convert.ToDouble(item.En) / 1000;
                        xlWorkSheet.Cells[i, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 10].Value = Convert.ToDouble(item.Boy) / 1000;
                        xlWorkSheet.Cells[i, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 11].Value = Convert.ToDouble(item.ortak.Alan);

                        xlWorkSheet.Cells[i, 12].Value = item.ortak.BirimFiyat;
                        xlWorkSheet.Cells[i, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                        xlWorkSheet.Cells[i, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        xlWorkSheet.Cells[i, 13].Value = item.ortak.ToplamTutar;
                        xlWorkSheet.Cells[i, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                        xlWorkSheet.Cells[i, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        if (p != enBoyList.Count)
                            xlWorkSheet.InsertRow(i + 1, 1);
                    }

                    xlWorkSheet.Cells[i + 1, 13].Value = Convert.ToDecimal(sablonPdf.Toplam);
                    xlWorkSheet.Cells[i + 1, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 1, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i + 2, 13].Value = Convert.ToDecimal(sablonPdf.KDV);
                    xlWorkSheet.Cells[i + 2, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 2, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i + 3, 13].Value = Convert.ToDecimal(sablonPdf.GenelToplam);
                    xlWorkSheet.Cells[i + 3, 13].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 3, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                else if (sablonPdf.RuzgarKiriciList != null)
                {
                    foreach (RuzgarKirici item in sablonPdf.RuzgarKiriciList)
                    {
                        p = p + 1;
                        i = i + 1;

                        xlWorkSheet.Cells[string.Format("C{0}:L{0}", i)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:L{0}", i)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:L{0}", i)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        xlWorkSheet.Cells[string.Format("C{0}:L{0}", i)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        xlWorkSheet.Cells[i, 3].Value = item.ortak.UrunAciklama;
                        xlWorkSheet.Cells[i, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 3].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 4].Value = item.CamKombinasyon;
                        xlWorkSheet.Cells[i, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 4].AutoFitColumns();
                        xlWorkSheet.Cells[i, 4].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 5].Value = item.AksesuarSet;
                        xlWorkSheet.Cells[i, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 5].AutoFitColumns();
                        xlWorkSheet.Cells[i, 5].Style.WrapText = true;

                        xlWorkSheet.Cells[i, 6].Value = item.BaglantiSistem;
                        xlWorkSheet.Cells[i, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 6].Style.WrapText = true;


                        xlWorkSheet.Cells[i, 7].Value = item.ortak.Adet;
                        xlWorkSheet.Cells[i, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 8].Value = Convert.ToDouble(item.En) / 1000;
                        xlWorkSheet.Cells[i, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 9].Value = Convert.ToDouble(item.Boy) / 1000;
                        xlWorkSheet.Cells[i, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xlWorkSheet.Cells[i, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xlWorkSheet.Cells[i, 10].Value = Convert.ToDouble(item.ortak.Alan);

                        xlWorkSheet.Cells[i, 11].Value = item.ortak.BirimFiyat;
                        xlWorkSheet.Cells[i, 11].Style.Numberformat.Format = "#,##0.00 ₺";
                        xlWorkSheet.Cells[i, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        xlWorkSheet.Cells[i, 12].Value = item.ortak.ToplamTutar;
                        xlWorkSheet.Cells[i, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                        xlWorkSheet.Cells[i, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        if (p != enBoyList.Count)
                            xlWorkSheet.InsertRow(i + 1, 1);
                    }

                    xlWorkSheet.Cells[i + 1, 12].Value = Convert.ToDecimal(sablonPdf.Toplam);
                    xlWorkSheet.Cells[i + 1, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 1, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i + 2, 12].Value = Convert.ToDecimal(sablonPdf.KDV);
                    xlWorkSheet.Cells[i + 2, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 2, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xlWorkSheet.Cells[i + 3, 12].Value = Convert.ToDecimal(sablonPdf.GenelToplam);
                    xlWorkSheet.Cells[i + 3, 12].Style.Numberformat.Format = "#,##0.00 ₺";
                    xlWorkSheet.Cells[i + 3, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                else if (sablonPdf.PergolaList != null)
                {
                }

                excel.SaveAs(new FileInfo(pathAfter + ".xlsx"));
                excel.Dispose();
            }

        }

        [HttpPost]
        public JsonResult AciklamaDosyaYukleme()
        {
            string mesaj = "";
            long siparisId;

            if (!long.TryParse(Request.Form["siparisId"], out siparisId))
                return Json(new { result = "NOK", mesaj = "Sipariş ID alınamadı!" }, JsonRequestBehavior.AllowGet);

            siparisRepo = new SiparisRepo();
            var siparis = siparisRepo.FindBy(e => e.Id == siparisId).FirstOrDefault();
            if (siparis == null)
                return Json(new { result = "NOK", mesaj = "Sipariş bulunamadı!" }, JsonRequestBehavior.AllowGet);

            DosyaRepo dosyaRepo = new DosyaRepo();
            List<long> dosyaIdleri = new List<long>();
            string fName = "";

            if (Request.Files.Count > 0)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(siparis.DosyaIds))
                    {
                        string[] oncekiIdler = siparis.DosyaIds.Split(',');
                        foreach (var item in oncekiIdler)
                        {
                            long dosyaId;
                            if (long.TryParse(item.Trim(), out dosyaId))
                                dosyaIdleri.Add(dosyaId);
                        }
                    }

                    foreach (string fileName in Request.Files)
                    {
                        Guid uniqId = Guid.NewGuid();
                        HttpPostedFileBase file = Request.Files[fileName];
                        string[] splitFileName = file.FileName.Split('.');
                        if (splitFileName.Length < 2) continue;
                        fName = uniqId.ToString() + "." + splitFileName.Last();

                        if (file != null && file.ContentLength > 0)
                        {
                            var path = Path.Combine(Server.MapPath("~/Assets/yuklenenler"));
                            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                            var uploadpath = Path.Combine(path, fName);
                            file.SaveAs(uploadpath);

                            var dosya = dosyaRepo.SaveAndReturnEntity(new YuklenenDosyalar
                            {
                                DosyaAdi = fName,
                                DosyaYolu = "/Assets/yuklenenler/" + fName,
                                DosyaUzantisi = Path.GetExtension(file.FileName),
                                YuklenmeTarihi = DateTime.Now
                            });

                            dosyaIdleri.Add(dosya.Id);
                        }
                    }

                    siparis.DosyaIds = string.Join(",", dosyaIdleri);
                    siparisRepo.EditAndSave(siparis);

                    mesaj = Request.Files.Count.ToString();
                    return Json(new { result = "OK", mesaj = mesaj }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    mesaj = "<div class='alert alert-danger alert-dismissible' role='alert'> " +
                            "<button type = 'button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button>" +
                            " Dosya yükleme başarısız! Hata mesajı: " + ex.Message + "</div>";
                    return Json(new { result = "NOK", mesaj = mesaj }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                mesaj = "<div class='alert alert-warning alert-dismissible' role='alert'> " +
                        "<button type = 'button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button>" +
                        " Lütfen yüklemek istediğiniz dosya(ları) seçiniz! </div>";
                return Json(new { result = "WOK", mesaj = mesaj }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DosyaSil(long dosyaId, long siparisId, string dosyaTur)
        {
            DosyaRepo dosyaRepo = new DosyaRepo();
            var yuklenenDosyalar = dosyaRepo.FindBy(e => e.Id == dosyaId).FirstOrDefault();

            if (yuklenenDosyalar == null)
                return Json("NOK", JsonRequestBehavior.AllowGet);

            if (dosyaTur.Equals("Aciklama"))
            {
                siparisRepo = new SiparisRepo();
                var siparis = siparisRepo.FindBy(e => e.Id == siparisId).FirstOrDefault();
                if (siparis != null && !string.IsNullOrWhiteSpace(siparis.DosyaIds))
                {
                    string[] dosyaIds = siparis.DosyaIds.Split(',');
                    List<long> dosyaIdleri = new List<long>();
                    foreach (var item in dosyaIds)
                    {
                        long dosyaIdItem;
                        if (long.TryParse(item.Trim(), out dosyaIdItem) && dosyaIdItem != dosyaId)
                        {
                            dosyaIdleri.Add(dosyaIdItem);
                        }
                    }
                    siparis.DosyaIds = dosyaIdleri.Count > 0 ? string.Join(",", dosyaIdleri) : null;
                    siparisRepo.EditAndSave(siparis);
                }
            }

            var dosyaYolu = Server.MapPath("~/Assets/yuklenenler/" + yuklenenDosyalar.DosyaAdi);
            if (System.IO.File.Exists(dosyaYolu))
            {
                System.IO.File.Delete(dosyaYolu);
            }

            dosyaRepo.DeleteAndSave(yuklenenDosyalar);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ImalatKesim4ProfilGonderim (korundu)
        public JsonResult ImalatKesim4ProfilGonderim(
            List<string> hatalar,
            List<string> kesimBicimiStok,
            List<string> kesimBicimiFireStok,
            double toplamAtikUzunluk,
            double fireStogaEklenenToplam,
            double kullanilanToplamUzunlukAsil,
            double kullanilanToplamUzunlukFire,
            double fireStogaEklenenToplamAgirlik,
            double kullanilanToplamAgirlikAsil,
            double kullanilanToplamAgirlikFire,
            double toplamAtikAgirlik,
            List<long> siparisId)
        {
            ImalatModel model = new ImalatModel();
            profilRepo = new ProfilRepo();
            profilBoyRepo = new ProfilBoyRepo();
            StokRepo stokRepo = new StokRepo();
            List<OptimizasyonSonuc> listSonuc = new List<OptimizasyonSonuc>();
            SabitRepo sabitRepo = new SabitRepo();
            AtikStokRepo atikStokRepo = new AtikStokRepo();

            List<Stok> yetersizStokList = new List<Stok>();

            foreach (var item in kesimBicimiStok)
            {
                OptimizasyonSonuc ent = new OptimizasyonSonuc();
                string[] split = item.Split('#');
                int profilId = Convert.ToInt32(split[0].Trim());
                int profilBoy = Convert.ToInt32(split[1].Trim());
                int profilBoyId = profilBoyRepo.FindBy(e => e.ProfilBoyu == profilBoy).FirstOrDefault().Id;
                Profil profil = profilRepo.FindBy(e => e.Id == profilId).FirstOrDefault();

                ent.profil = profil;
                ent.KullanilacakOlcu = profilBoy;
                ent.KesileceklerOlcusu = split[2];
                ent.Adet = int.Parse(split[4]);
                ent.FireAtik = 0;
                if (split.Length > 4)
                    ent.FireAtik = int.Parse(split[3]);
                ent.KullanilanAlan = "Asıl Stok";
                ent.eksikAdet = int.Parse(split[5]);
                ent.mevcutStokMiktari = stokRepo.FindBy(e => e.ProfilId == profilId && e.ProfilBoyId == profilBoyId).FirstOrDefault().StokAdet.Value;

                listSonuc.Add(ent);
            }

            if (kesimBicimiFireStok != null)
            {
                foreach (var item in kesimBicimiFireStok)
                {
                    OptimizasyonSonuc ent = new OptimizasyonSonuc();
                    string[] split = item.Split('#');
                    int profilId = Convert.ToInt32(split[0].Trim());
                    int profilBoy = Convert.ToInt32(split[1].Trim());
                    Profil profil = profilRepo.FindBy(e => e.Id == profilId).FirstOrDefault();

                    ent.profil = profil;
                    ent.KullanilacakOlcu = profilBoy;
                    ent.KesileceklerOlcusu = split[2];
                    ent.Adet = int.Parse(split[4]);
                    ent.FireAtik = 0;
                    if (split.Length > 4)
                        ent.FireAtik = int.Parse(split[3]);
                    ent.KullanilanAlan = "Fire Stok";
                    ent.mevcutStokMiktari = atikStokRepo.FindBy(e => e.ProfilId == profilId && e.Olcu == profilBoy).FirstOrDefault().Adet.Value;

                    listSonuc.Add(ent);
                }
            }

            model.fireStogaEklenenToplam = Math.Round(double.Parse(fireStogaEklenenToplam.ToString()), 2);
            model.fireStogaEklenenToplamAgirlik = Math.Round(double.Parse(fireStogaEklenenToplamAgirlik.ToString()), 2);
            model.kullanilanToplamUzunlukAsil = Math.Round(double.Parse(kullanilanToplamUzunlukAsil.ToString()), 2);
            model.kullanilanToplamAgirlikAsil = Math.Round(double.Parse(kullanilanToplamAgirlikAsil.ToString()), 2);
            model.kullanilanToplamUzunlukFire = Math.Round(double.Parse(kullanilanToplamUzunlukFire.ToString()), 2);
            model.kullanilanToplamAgirlikFire = Math.Round(double.Parse(kullanilanToplamAgirlikFire.ToString()), 2);
            model.toplamAtikUzunluk = Math.Round(double.Parse(toplamAtikUzunluk.ToString()), 2);
            model.toplamAtikAgirlik = Math.Round(double.Parse(toplamAtikAgirlik.ToString()), 2);
            model.optiSonuc = listSonuc;
            model.yetersizStokList = yetersizStokList;
            ViewBag.minimumFire = sabitRepo.FindBy(e => e.Id == 1).FirstOrDefault().SabitDeger;

            //optimizasyon sonucu tabloya kaydediyoruz.
            OptimizasyonHesapRepo optimizasyonHesapRepo = new OptimizasyonHesapRepo();
            Kullanici kullaniciModel = (Kullanici)Session["CurrentUser"];
            foreach (var item in listSonuc)
            {
                OptimizasyonHesap hesap = new OptimizasyonHesap();
                hesap.SiparisIds = string.Join(", ", siparisId);
                hesap.KesilecekOlculer = item.KesileceklerOlcusu;
                hesap.ProfilBoy = item.KullanilacakOlcu;
                hesap.ProfilId = item.profil.Id;
                hesap.KesimAdet = item.Adet;
                hesap.FireAtik = item.FireAtik;

                hesap.ToplamAtikUzunluk = Math.Round(decimal.Parse(toplamAtikUzunluk.ToString()), 2);
                hesap.ToplamAtikAgirlik = Math.Round(decimal.Parse(toplamAtikAgirlik.ToString()), 2);
                hesap.AsilStoktanKullanilanToplamUzunluk = Math.Round(decimal.Parse(kullanilanToplamUzunlukAsil.ToString()), 2);
                hesap.AsilStoktanKullanilanToplamAgirlik = Math.Round(decimal.Parse(kullanilanToplamAgirlikAsil.ToString()), 2);
                hesap.FiredenKullanilanToplamUzunluk = Math.Round(decimal.Parse(kullanilanToplamUzunlukFire.ToString()), 2);
                hesap.FiredenKullanilanToplamAgirlik = Math.Round(decimal.Parse(kullanilanToplamAgirlikFire.ToString()), 2);
                hesap.FireyeEklenenToplamUzunluk = Math.Round(decimal.Parse(fireStogaEklenenToplam.ToString()), 2);
                hesap.FireyeEklenenToplamAgirlik = Math.Round(decimal.Parse(fireStogaEklenenToplamAgirlik.ToString()), 2);
                hesap.KullanilanAlan = item.KullanilanAlan;
                hesap.KayitTarih = DateTime.Now;
                hesap.KullaniciId = kullaniciModel.Id;

                optimizasyonHesapRepo.AddAndSave(hesap);
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}