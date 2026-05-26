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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

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
        // Her kesimden önce ve sonra eklenen bıçak payı (mm)
        private const int BICHAK_PAYI = 4;
        private const string KAR_PAYI_MALZEME = "KAR PAYI";
        private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string SacBoruProfilKodu = "SB-101";

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

        #region Exception Handler
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var ex = filterContext.Exception;
                string hataTipi = ex?.GetType().Name + ": " + ex?.Message;
                if (ex?.InnerException != null)
                    hataTipi += " --> " + ex.InnerException.Message;
                System.Diagnostics.Debug.WriteLine("[SiparisController.OnException] Hata: " + hataTipi + "\n" + ex?.StackTrace);
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.HttpContext.Response.StatusCode = 200;
                filterContext.Result = new ContentResult
                {
                    Content = "<div class='alert alert-danger' style='margin:20px;'>" +
                              "<strong>Sipariş detayı yüklenirken bir hata oluştu.</strong><br/>" +
                              "Lütfen sayfayı yenileyip tekrar deneyin.<br/>" +
                              "<details style='margin-top:8px;'>" +
                              "<summary style='cursor:pointer;color:#a94442;font-size:12px;'>Hata Detayı (Geliştirici)</summary>" +
                              "<pre style='font-size:11px;white-space:pre-wrap;background:#f9f2f4;padding:8px;margin-top:6px;border-radius:3px;'>" +
                              System.Web.HttpUtility.HtmlEncode(hataTipi) + "</pre></details>" +
                              "<button class='btn btn-default' style='margin-top:10px;' onclick='$(\"#showDuzenleModal\").modal(\"hide\")'>Kapat</button>" +
                              "</div>",
                    ContentType = "text/html; charset=utf-8",
                    ContentEncoding = Encoding.UTF8
                };
            }
            else
            {
                base.OnException(filterContext);
            }
        }
        #endregion

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
                    profil.Gram = Convert.ToInt32(pRepo.FindBy(e => e.Id == profilId).FirstOrDefault()?.BirimAgirlik ?? 0);
                    profil.Boy = (int)pbRepo.FindBy(e => e.Id == item.ProfilBoyId).FirstOrDefault().ProfilBoyu;
                    profil.Adet = (int)item.StokAdet;
                }
                else
                {
                    int profilId = Convert.ToInt32(item.ProfilId);
                    profil.Gram = Convert.ToInt32(pRepo.FindBy(e => e.Id == profilId).FirstOrDefault()?.BirimAgirlik ?? 0);
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
                dicProfilBirimAgirlik[item.Id] = Convert.ToInt32(item.BirimAgirlik ?? 0);

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
                        (int)(item2.GirilenAdet ?? 0),
                        item2.SistemId,
                        item2.AltSistemId,
                        item2.SistemTurId,
                        kanatAdedi: item2.GirilenKanatAdet ?? 1,
                        kasaTipiOverride: item2.KasaTipi
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
                        || item2.ProfilKodu.Contains("KAR-4873") || item2.ProfilKodu.Contains("KAR-4862") || item2.ProfilKodu.Contains("KAR-4880"))
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
                        string kesilecekOlculer = split[2];
                        int profilBoy = int.Parse(split[1].Trim());
                        repo.AddAndSave(new OptimizasyonHesap
                        {
                            SiparisIds = siparisIdStr,
                            ProfilId = int.Parse(split[0].Trim()),
                            ProfilBoy = profilBoy,
                            KesilecekOlculer = kesilecekOlculer,
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
                        string kesilecekOlculer = split[2];
                        int profilBoy = int.Parse(split[1].Trim());
                        repo.AddAndSave(new OptimizasyonHesap
                        {
                            SiparisIds = siparisIdStr,
                            ProfilId = int.Parse(split[0].Trim()),
                            ProfilBoy = profilBoy,
                            KesilecekOlculer = kesilecekOlculer,
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

        private static string BuildProfilOlcuPozKey(int profilId, int kesimOlcusu)
        {
            return profilId + "#" + kesimOlcusu;
        }

        private Tuple<Dictionary<string, Queue<string>>, Dictionary<int, string>> BuildPozLookupForSiparis(long siparisId)
        {
            var pozOlcuQueueMap = new Dictionary<string, Queue<string>>();
            var profilDefaultPozMap = new Dictionary<int, string>();

            try
            {
                var siparis = new SiparisRepo().FindBy(e => e.Id == siparisId).FirstOrDefault();
                if (siparis == null)
                {
                    return Tuple.Create(pozOlcuQueueMap, profilDefaultPozMap);
                }

                var satirlar = new SiparisEnBoyAdetRepo()
                    .FindBy(e => e.SiparisId == siparisId)
                    .OrderBy(e => e.Id)
                    .ToList();

                int pozNo = 0;
                foreach (var satir in satirlar)
                {
                    pozNo++;
                    var pozLabel = "POZ" + pozNo;

                    List<Profil> profilList;
                    try
                    {
                        profilList = SiparisHesaplamalari.profilHesaplama(
                            siparisId,
                            satir.GirilenEn ?? 0,
                            satir.GirilenSolEn ?? 0,
                            satir.GirilenBoy ?? 0,
                            satir.GirilenAdet ?? 0,
                            satir.SistemId,
                            satir.AltSistemId,
                            satir.SistemTurId,
                            kanatAdedi: satir.GirilenKanatAdet ?? 1,
                            kasaTipiOverride: satir.KasaTipi
                        ) ?? new List<Profil>();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("[BuildPozLookupForSiparis] profilHesaplama hatası satirId=" + satir.Id + ": " + ex.Message);
                        continue;
                    }

                    foreach (var profil in profilList)
                    {
                        if (!profilDefaultPozMap.ContainsKey(profil.Id))
                        {
                            profilDefaultPozMap[profil.Id] = pozLabel;
                        }

                        var key = BuildProfilOlcuPozKey(profil.Id, profil.KesimOlcusu);
                        if (!pozOlcuQueueMap.ContainsKey(key))
                        {
                            pozOlcuQueueMap[key] = new Queue<string>();
                        }

                        var kesimAdet = profil.KesimAdet > 0 ? profil.KesimAdet : 1;
                        for (int i = 0; i < kesimAdet; i++)
                        {
                            pozOlcuQueueMap[key].Enqueue(pozLabel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[BuildPozLookupForSiparis] Hata SiparisId=" + siparisId + ": " + ex.Message);
            }

            return Tuple.Create(pozOlcuQueueMap, profilDefaultPozMap);
        }

        [HttpPost]
        [AuthLog(Roles = "SİPARİS,GORUNTULEME,IMALAT,ONAYLAMA")]
        public ActionResult OptimizasyonHesapla(long SiparisId)
        {
            try
            {
                var sabitRepo = new SabitRepo();
                ViewBag.minimumFire = sabitRepo.FindBy(e => e.Id == 1).FirstOrDefault()?.SabitDeger ?? 0;
                var pozLookup = BuildPozLookupForSiparis(SiparisId);
                ViewBag.PozOlcuQueueMap = pozLookup.Item1.ToDictionary(k => k.Key, v => v.Value.ToList());
                ViewBag.ProfilDefaultPozMap = pozLookup.Item2;

                var kayitlar = GetOrRunOptimizasyonHesaps(SiparisId)
                    .OrderByDescending(x => x.Id)
                    .ToList();
                string html = RenderPartialViewToString("_optimizasyonHesapGrid", kayitlar);
                return Content(html, "text/html; charset=utf-8", Encoding.UTF8);
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

                // Backward compat: set order-level system from first row's per-row system data when provided
                var firstRow = enBoyAdet.FirstOrDefault();
                if (firstRow != null && firstRow.SistemId.HasValue && firstRow.SistemId.Value > 0)
                {
                    siparis.SistemId = firstRow.SistemId;
                    siparis.AltSistemId = firstRow.AltSistemId;
                    siparis.SistemTurId = firstRow.SistemTurId;
                }

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
                        GirilenEn3 = item.GirilenEn3 ?? 0,
                        SistemId = item.SistemId,
                        AltSistemId = item.AltSistemId,
                        SistemTurId = item.SistemTurId,
                        KasaTipi = item.KasaTipi,
                        SiparisId = siparisEntity.Id
                    };

                    sebaRepo.AddAndSave(enBoyAdetModel);
                }

                // Sipariş oluşturulduğunda optimizasyonu çalıştır (tüm sipariş türleri için)
                GetOrRunOptimizasyonHesaps(siparisEntity.Id);

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
        [HttpPost]
        public ActionResult SiparisDetayGoruntule(long SiparisId, bool raporMu)
        {
            try
            {
                return SiparisDetayGoruntuleInternal(SiparisId, raporMu);
            }
            catch (Exception ex)
            {
                string hataTipi = ex.GetType().Name + ": " + ex.Message;
                // Inner exception da varsa ekle
                if (ex.InnerException != null)
                    hataTipi += " --> " + ex.InnerException.Message;
                System.Diagnostics.Debug.WriteLine("[SiparisDetayGoruntule] Hata SiparisId=" + SiparisId + ": " + hataTipi + "\n" + ex.StackTrace);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 200;
                return Content(
                    "<div class='alert alert-danger' style='margin:20px;'>" +
                    "<strong>Sipariş detayı yüklenirken bir hata oluştu.</strong><br/>" +
                    "Lütfen sayfayı yenileyip tekrar deneyin.<br/>" +
                    "<details style='margin-top:8px;'>" +
                    "<summary style='cursor:pointer;color:#a94442;font-size:12px;'>Hata Detayı (Geliştirici)</summary>" +
                    "<pre style='font-size:11px;white-space:pre-wrap;background:#f9f2f4;padding:8px;margin-top:6px;border-radius:3px;'>" +
                    System.Web.HttpUtility.HtmlEncode(hataTipi) + "</pre></details>" +
                    "<button class='btn btn-default' style='margin-top:8px;' onclick='$(\"#showDuzenleModal\").modal(\"hide\")'>Kapat</button>" +
                    "</div>",
                    "text/html; charset=utf-8", Encoding.UTF8);
            }
        }

        private ActionResult SiparisDetayGoruntuleInternal(long SiparisId, bool raporMu)
        {
            EnsureUtf8HtmlResponse();
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
                return Content("Sipariş bulunamadı!", "text/html; charset=utf-8", Encoding.UTF8);

            ViewBag.SiparisDurumu = (siparis.DurumId == (int)Durumlar.Onaylandı || siparis.DurumId == (int)Durumlar.ImalataGonderildi || siparis.DurumId == (int)Durumlar.Sevkiyatta);

            List<SiparisEnBoyAdet> siparisAdet;
            try
            {
                siparisAdet = sebaRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            }
            catch (Exception exSeba)
            {
                System.Diagnostics.Debug.WriteLine("[SiparisDetayGoruntule] SiparisEnBoyAdet sorgu hatası SiparisId=" + SiparisId + ": " + exSeba.GetType().Name + ": " + exSeba.Message + (exSeba.InnerException != null ? " --> " + exSeba.InnerException.Message : ""));
                return Content(
                    "<div class='alert alert-danger' style='margin:20px;'>" +
                    "<strong>Sipariş detayı yüklenirken bir hata oluştu.</strong><br/>" +
                    "Veritabanı şeması güncel olmayabilir. Lütfen yöneticinizle iletişime geçin.<br/>" +
                    "<details style='margin-top:8px;'>" +
                    "<summary style='cursor:pointer;color:#a94442;font-size:12px;'>Hata Detayı (Geliştirici)</summary>" +
                    "<pre style='font-size:11px;white-space:pre-wrap;background:#f9f2f4;padding:8px;margin-top:6px;border-radius:3px;'>" +
                    System.Web.HttpUtility.HtmlEncode(exSeba.GetType().Name + ": " + exSeba.Message + (exSeba.InnerException != null ? " --> " + exSeba.InnerException.Message : "")) + "</pre></details>" +
                    "<button class='btn btn-default' style='margin-top:10px;' onclick='$(\"#showDuzenleModal\").modal(\"hide\")'>Kapat</button>" +
                    "</div>",
                    "text/html; charset=utf-8", Encoding.UTF8);
            }

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
                int girilenKanatAdet = item.GirilenKanatAdet ?? 1;

                // Use per-row system data when available, fall back to order-level system
                int effectiveSistemId = (item.SistemId.HasValue && item.SistemId.Value > 0) ? item.SistemId.Value : (int)(siparis.SistemId ?? 0);
                int effectiveAltSistemId = (item.AltSistemId.HasValue && item.AltSistemId.Value > 0) ? item.AltSistemId.Value : (int)(siparis.AltSistemId ?? 0);
                int effectiveSistemTurId = (item.SistemTurId.HasValue && item.SistemTurId.Value > 0) ? item.SistemTurId.Value : (int)(siparis.SistemTurId ?? 0);

                List<Profil> profilList = new List<Profil>();
                try
                {
                    profilList = SiparisHesaplamalari.profilHesaplama(
                        siparis.Id, girilenEn, girilenSolEn, girilenBoy, girilenAdet,
                        item.SistemId, item.AltSistemId, item.SistemTurId,
                        kanatAdedi: girilenKanatAdet,
                        kasaTipiOverride: item.KasaTipi) ?? new List<Profil>();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("[SiparisDetayGoruntule] profilHesaplama hatası item.Id=" + item.Id + ": " + ex.GetType().Name + ": " + ex.Message + (ex.InnerException != null ? " --> " + ex.InnerException.Message : ""));
                }

                List<CamBilgileri> camBilgileriList = new List<CamBilgileri>();
                try
                {
                    camBilgileriList = SiparisHesaplamalari.CamYukseklikHesapla(
                        effectiveSistemId,
                        effectiveSistemTurId,
                        effectiveAltSistemId,
                        girilenBoy,
                        girilenEn,
                        girilenSolEn,
                        girilenAdet,
                        girilenKanatAdet
                    ) ?? new List<CamBilgileri>();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("[SiparisDetayGoruntule] CamYukseklikHesapla hatası item.Id=" + item.Id + ": " + ex.GetType().Name + ": " + ex.Message + (ex.InnerException != null ? " --> " + ex.InnerException.Message : ""));
                }

                ProfilDetayBilgileri profilDetay = new ProfilDetayBilgileri();
                if (camBilgileriList != null)
                {
                    if (altSistemId4Surme.Contains(effectiveAltSistemId))
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
                ent.GirilenKanatAdet = item.GirilenKanatAdet;
                ent.GirilenBoy = item.GirilenBoy;
                ent.GirilenEn = item.GirilenEn;
                ent.GirilenSolEn = item.GirilenSolEn;
                ent.SiparisId = item.SiparisId;
                ent.Id = item.Id;
                ent.SistemId = item.SistemId;
                ent.AltSistemId = item.AltSistemId;
                ent.SistemTurId = item.SistemTurId;
                ent.KasaTipi = item.KasaTipi;
                ent.siparisModel = siparis;
                ent.siparisCam = siparisCam;

                // ==== TEKLİF / MALİYET (SiparisTeklif) ====
                try
                {
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
                        var aksesuarIds = siparisAksesuarList
                            .Where(x => x.AksesuarId.HasValue)
                            .Select(x => x.AksesuarId.Value)
                            .ToList();
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
                }
                catch (Exception exTeklif)
                {
                    System.Diagnostics.Debug.WriteLine("[SiparisDetayGoruntule] Teklif/Maliyet hatası SiparisEnBoyAdetId=" + item.Id + ": " + exTeklif.GetType().Name + ": " + exTeklif.Message + (exTeklif.InnerException != null ? " --> " + exTeklif.InnerException.Message : ""));
                    ent.teklifList = new List<SiparisTeklif>();
                    ent.teklifToplamDetay = null;
                }

                siparisTumDetay.Add(ent);
            }

            // Optimizasyon verilerini DB'den çekip modele ekle
            var optimizasyonHesapRepo = new OptimizasyonHesapRepo();
            string siparisIdStr = siparis.Id.ToString();
            List<OptimizasyonHesap> optimizasyonKayitlar = new List<OptimizasyonHesap>();
            try
            {
                optimizasyonKayitlar = optimizasyonHesapRepo
                    .FindBy(e => e.SiparisIds != null && e.SiparisIds.Contains(siparisIdStr))
                    .ToList()
                    .Where(x => x.SiparisIds.Split(',').Select(s => s.Trim()).Any(id => id == siparisIdStr))
                    .OrderByDescending(x => x.Id)
                    .ToList();
            }
            catch (Exception exOpti)
            {
                System.Diagnostics.Debug.WriteLine("[SiparisDetayGoruntule] Optimizasyon sorgu hatası SiparisId=" + siparis.Id + ": " + exOpti.GetType().Name + ": " + exOpti.Message + (exOpti.InnerException != null ? " --> " + exOpti.InnerException.Message : ""));
            }
            foreach (var ent in siparisTumDetay)
                ent.optimizasyonList = optimizasyonKayitlar;

            ViewBag.optiVarMi = optimizasyonKayitlar.Any();

            if (string.IsNullOrWhiteSpace(siparis.Aciklama))
                ViewBag.SiparisAciklamasi = "";
            else
                ViewBag.SiparisAciklamasi = "<div class='alert alert-danger'><strong>" + SiparisId + " Nolu Sipariş Açıklaması:</strong> " + siparis.Aciklama + "</div>";

            ViewBag.raporMu = raporMu;
            try
            {
                ViewBag.minimumFire = sabitRepo.FindBy(e => e.Id == 1).FirstOrDefault()?.SabitDeger ?? 0;
            }
            catch (Exception exSabit)
            {
                System.Diagnostics.Debug.WriteLine("[SiparisDetayGoruntule] minimumFire sorgu hatası: " + exSabit.GetType().Name + ": " + exSabit.Message);
                ViewBag.minimumFire = 0;
            }

            // Select view: check order-level SistemId AND per-row SistemIds for Giyotin Sabit Sistem
            var giyotinIds = new[] { 5, 2006, 2010 };
            bool isGiyotinSabit = (siparis.SistemId.HasValue && giyotinIds.Contains(siparis.SistemId.Value))
                || siparisTumDetay.Any(e => e.SistemId.HasValue && giyotinIds.Contains(e.SistemId.Value));
            string viewName = isGiyotinSabit ? "_siparisGiyotinSablon" : "_siparisDetaySablon";

            string html = RenderPartialViewToString(viewName, siparisTumDetay);
            return Content(html, "text/html; charset=utf-8", Encoding.UTF8);
        }

        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new Utf8StringWriter())
            {
                var viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                if (viewResult == null || viewResult.View == null)
                    throw new InvalidOperationException("Görünüm bulunamadı: " + viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        private void EnsureUtf8HtmlResponse()
        {
            Response.ContentType = "text/html; charset=utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "utf-8";
        }

        private sealed class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
        #endregion

        #region Durum / Onay / Fiş / Fiyat (Index.cshtml bunları çağırıyor)
        [AuthLog(Roles = "ONAYLAMA")]
        public ActionResult SiparisDurumGuncelle(long SiparisId, int DurumId)
        {
            try
            {
                if (SiparisId <= 0 || !Enum.IsDefined(typeof(Durumlar), DurumId))
                    return Json("NOT", JsonRequestBehavior.AllowGet);

                siparisRepo = new SiparisRepo();
                Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
                if (siparis == null)
                    return Json("NOT", JsonRequestBehavior.AllowGet);

                Kullanici kullaniciModel = Session["CurrentUser"] as Kullanici;
                if (kullaniciModel == null)
                    return Json("NOT", JsonRequestBehavior.AllowGet);

                if (DurumId == (int)Durumlar.Onaylandı)
                {
                    siparis.OnayIptalKullaniciId = kullaniciModel.Id;
                    siparis.DurumId = (int)Durumlar.Onaylandı;
                    siparis.OnayIptalTarihi = DateTime.Now;
                    siparis.GuncellemeTarihi = DateTime.Now;
                    siparisRepo.EditAndSave(siparis);

                    // Henüz optimizasyon yapılmamışsa şimdi çalıştır
                    GetOrRunOptimizasyonHesaps(SiparisId);

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
                if (SiparisId <= 0 || string.IsNullOrWhiteSpace(FisNo))
                    return Json("NOT", JsonRequestBehavior.AllowGet);

                siparisRepo = new SiparisRepo();
                Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
                if (siparis == null)
                    return Json("NOT", JsonRequestBehavior.AllowGet);

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
                if (SiparisId <= 0 || string.IsNullOrWhiteSpace(FisNo))
                    return Json("NOT", JsonRequestBehavior.AllowGet);

                siparisRepo = new SiparisRepo();
                Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
                if (siparis == null)
                    return Json("NOT", JsonRequestBehavior.AllowGet);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "ONAYLAMA,DUZENLEME")]
        public JsonResult SiparisTeklifSatiriGuncelle(long TeklifId, string BirimFiyat, string Miktar, string ToplamTutar)
        {
            try
            {
                var siparisTeklifRepo = new SiparisTeklifRepo();
                var sebaRepo = new SiparisEnBoyAdetRepo();

                var teklifSatiri = siparisTeklifRepo.FindBy(x => x.Id == TeklifId).FirstOrDefault();
                if (teklifSatiri == null)
                    return Json(new { Result = "ERR", Message = "Teklif satırı bulunamadı." }, JsonRequestBehavior.AllowGet);

                var birimFiyatDeger = ParseDecimalFlexible(BirimFiyat);
                if (!birimFiyatDeger.HasValue || birimFiyatDeger.Value < 0m)
                    return Json(new { Result = "ERR", Message = "Birim fiyat geçersiz." }, JsonRequestBehavior.AllowGet);

                var miktarDeger = ParseDecimalFlexible(Miktar);
                var toplamTutarDeger = ParseDecimalFlexible(ToplamTutar);

                if (miktarDeger.HasValue && miktarDeger.Value >= 0m)
                    teklifSatiri.Miktar = miktarDeger.Value;

                teklifSatiri.BirimFiyat = birimFiyatDeger.Value;

                var seciliMiktar = teklifSatiri.Miktar ?? 0m;
                bool karPayiSatiri = string.Equals((teklifSatiri.Malzeme ?? "").Trim(), KAR_PAYI_MALZEME, StringComparison.OrdinalIgnoreCase);

                if (karPayiSatiri && toplamTutarDeger.HasValue && toplamTutarDeger.Value >= 0m)
                {
                    teklifSatiri.ToplamTutar = toplamTutarDeger.Value;
                    if (seciliMiktar > 0m)
                        teklifSatiri.BirimFiyat = toplamTutarDeger.Value / seciliMiktar;
                }
                else
                {
                    teklifSatiri.ToplamTutar = seciliMiktar * birimFiyatDeger.Value;
                }

                siparisTeklifRepo.EditAndSave(teklifSatiri);

                var satirlar = siparisTeklifRepo
                    .FindBy(x => x.SiparisEnBoyAdetId == teklifSatiri.SiparisEnBoyAdetId)
                    .ToList();

                var toplamMaliyet = satirlar.Sum(x => x.ToplamTutar ?? 0m);
                decimal alan = 0m;

                if (teklifSatiri.SiparisEnBoyAdetId.HasValue)
                {
                    var seba = sebaRepo.FindBy(x => x.Id == teklifSatiri.SiparisEnBoyAdetId.Value).FirstOrDefault();
                    if (seba != null && (seba.GirilenEn ?? 0) > 0 && (seba.GirilenBoy ?? 0) > 0)
                    {
                        var enToplam = (seba.GirilenEn ?? 0) + (seba.GirilenSolEn ?? 0);
                        alan = ((decimal)enToplam * (decimal)(seba.GirilenBoy ?? 0)) / 1000000m;
                    }
                }

                return Json(new
                {
                    Result = "OK",
                    BirimFiyat = teklifSatiri.BirimFiyat ?? 0m,
                    Miktar = teklifSatiri.Miktar ?? 0m,
                    ToplamTutar = teklifSatiri.ToplamTutar ?? 0m,
                    ToplamMaliyet = toplamMaliyet,
                    M2 = (alan > 0m ? (toplamMaliyet / alan) : 0m),
                    Teklif = toplamMaliyet
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERR", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private static decimal? ParseDecimalFlexible(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            decimal parsed;
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("tr-TR"), out parsed))
                return parsed;
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsed))
                return parsed;
            return null;
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

        string GetExportTempPath()
        {
            string tempPath = Server.MapPath("~/Assets/temp/");
            if (!Directory.Exists(tempPath))
            {
                try
                {
                    Directory.CreateDirectory(tempPath);
                }
                catch (Exception ex)
                {
                    throw new IOException("İndirme için geçici klasör oluşturulamadı: " + tempPath, ex);
                }
            }

            return tempPath;
        }

        string BuildAdresMetni(Adres adres)
        {
            if (adres == null)
                return "";

            string adresMetni = string.Format("{0} {1} {2} - {3} / {4}",
                adres.AcikAdres,
                adres.PostaKodu,
                adres.Ilce,
                adres.Il,
                adres.Ulke).Trim();

            return SanitizeExcelText(adresMetni).Trim();
        }

        string SanitizeExcelText(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            StringBuilder builder = new StringBuilder(value.Length);
            foreach (char ch in value)
            {
                if (XmlConvert.IsXmlChar(ch))
                    builder.Append(ch);
            }

            return builder.ToString();
        }

        decimal GetSabitDegerOrDefault(SabitRepo sabitRepo, int sabitId, decimal defaultValue = 0)
        {
            try
            {
                var sabit = sabitRepo.FindBy(e => e.Id == sabitId).FirstOrDefault();
                return sabit?.SabitDeger != null ? Convert.ToDecimal(sabit.SabitDeger) / 100 : defaultValue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[GetSabitDegerOrDefault] Hata SabitId=" + sabitId + ": " + ex.Message);
                return defaultValue;
            }
        }

        ExcelPackage CreateExcelPackage(string templatePath, string worksheetName)
        {
            if (!System.IO.File.Exists(templatePath))
                throw new FileNotFoundException("Excel şablonu bulunamadı: " + templatePath, templatePath);

            return new ExcelPackage(new FileInfo(templatePath));
        }

        bool TryCreateFallbackExcel(string fullPath, long siparisId, Exception ex)
        {
            try
            {
                siparisRepo = siparisRepo ?? new SiparisRepo();
                musteriRepo = musteriRepo ?? new MusteriRepo();
                AdresRepo adresRepo = new AdresRepo();

                Siparis siparis = siparisRepo.FindBy(e => e.Id == siparisId).FirstOrDefault();
                Musteri musteri = null;
                Adres adres = null;

                if (siparis?.MusteriId != null)
                    musteri = musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault();

                if (musteri?.AdresId != null)
                    adres = adresRepo.FindBy(e => e.Id == musteri.AdresId).FirstOrDefault();

                using (ExcelPackage excel = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Siparis");
                    worksheet.Cells["A1"].Value = "Sipariş No";
                    worksheet.Cells["B1"].Value = siparisId;
                    worksheet.Cells["A2"].Value = "Müşteri";
                    worksheet.Cells["B2"].Value = SanitizeExcelText(siparis?.MusteriTamAdi ?? "");
                    worksheet.Cells["A3"].Value = "Adres";
                    worksheet.Cells["B3"].Value = BuildAdresMetni(adres);
                    worksheet.Cells["A5"].Value = "Sipariş çıktısı eksik veriler güvenli şekilde atlanarak oluşturuldu.";
                    worksheet.Cells["A6"].Value = "Hata";
                    worksheet.Cells["B6"].Value = SanitizeExcelText(ex?.Message ?? "Bilinmeyen hata");
                    worksheet.Cells.AutoFitColumns();
                    excel.SaveAs(new FileInfo(fullPath));
                }

                return true;
            }
            catch (Exception fallbackEx)
            {
                System.Diagnostics.Debug.WriteLine("[TryCreateFallbackExcel] Hata SiparisId=" + siparisId + ": " + fallbackEx.Message + "\n" + fallbackEx.StackTrace);
                return false;
            }
        }

        ActionResult DownloadExcelFile(string file, Action<long> exportAction)
        {
            string basePath = GetExportTempPath();

            string requestedFile = Path.GetFileName(file ?? string.Empty);
            string safeFile = Path.GetFileName(HttpUtility.UrlDecode(requestedFile ?? string.Empty));
            if (string.IsNullOrWhiteSpace(safeFile))
                return new HttpStatusCodeResult(400, "Geçersiz dosya adı.");

            if (!string.Equals(Path.GetExtension(safeFile), ".xlsx", StringComparison.OrdinalIgnoreCase))
                return new HttpStatusCodeResult(400, "Yalnızca Excel dosyası indirilebilir.");

            string fullPath = Path.Combine(basePath, safeFile);
            if (!fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                return new HttpStatusCodeResult(400, "Geçersiz dosya yolu.");

            try
            {
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[DownloadExcelFile] Var olan dosya silinemedi: " + fullPath + " | " + ex.Message);
            }

            string[] fileParts = safeFile.Split('_');
            if (fileParts.Length == 0 || string.IsNullOrWhiteSpace(fileParts[0]) || !long.TryParse(fileParts[0], out long siparisId))
                return new HttpStatusCodeResult(400, "Geçersiz sipariş dosya formatı.");

            try
            {
                exportAction(siparisId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[DownloadExcelFile] Excel export hatası SiparisId=" + siparisId + ": " + ex.Message + "\n" + ex.StackTrace);
                return new HttpStatusCodeResult(500, "Excel dosyası şablon ile oluşturulamadı.");
            }

            if (!System.IO.File.Exists(fullPath))
                return HttpNotFound("İndirilecek dosya oluşturulamadı.");

            Response.TrySkipIisCustomErrors = true;
            Response.AppendHeader("X-Content-Type-Options", "nosniff");
            return File(fullPath, ExcelMimeType, safeFile);
        }

        void TryAddProfilKesitPicture(ExcelWorksheet xlWorkSheet, int rowIndex, string kesit, string pictureName)
        {
            if (string.IsNullOrWhiteSpace(kesit))
                return;

            try
            {
                var imagePath = Server.MapPath("/images/profilicons/" + kesit);
                if (!System.IO.File.Exists(imagePath))
                    return;

                using (Image img = Image.FromFile(imagePath))
                {
                    int iColumnWidth = (int)((xlWorkSheet.Column(3).Width - 1) * 7) + 12;
                    int iColumnHeight = (int)(xlWorkSheet.Row(rowIndex).Height * 1.333);
                    int xOffset = Math.Max(0, iColumnWidth / 2 - img.Width / 2);
                    int yOffset = Math.Max(0, iColumnHeight / 2 - img.Height / 2);
                    xlWorkSheet.Drawings.AddPicture(pictureName, img).SetPosition(rowIndex - 1, yOffset, 2, xOffset);
                }
            }
            catch
            {
                // Geçersiz/bozuk görsel veya path hatası durumunda Excel üretimine devam edilir.
            }
        }

        public ActionResult SiparisIndir(string file)
        {
            return DownloadExcelFile(file, excelKaydet);
        }

        public ActionResult StoktanIndir(string file)
        {
            return DownloadExcelFile(file, excelStoktanKaydet);
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

            var musteri = musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault();
            Adres adres = null;
            if (musteri?.AdresId != null)
                adres = adresRepo.FindBy(e => e.Id == musteri.AdresId).FirstOrDefault();

            sablon.SirketAdres = BuildAdresMetni(adres);
            ViewBag.AluKg = aluKgFiyat;
            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "utf-8";
            return PartialView("_stoktanSiparisSablon4Pdf", sablon);
        }

        SiparisStokSablon BuildSiparisSablon(long siparisId, out Siparis siparis, out decimal aluKgFiyat)
        {
            siparisRepo = new SiparisRepo();
            sebaRepo = new SiparisEnBoyAdetRepo();
            siparisAksesuarRepo = new SiparisAksesuarRepo();
            siparisStokRepo = new SiparisStokRepo();
            musteriRepo = new MusteriRepo();
            aksesuarRepo = new AksesuarRepo();
            AdresRepo adresRepo = new AdresRepo();
            SabitRepo sabitRepo = new SabitRepo();
            SiparisTeklifRepo siparisTeklifRepo = new SiparisTeklifRepo();
            ProfilRepo profilRepo = new ProfilRepo();
            profilBoyRepo = new ProfilBoyRepo();

            siparis = siparisRepo.FindBy(e => e.Id == siparisId).FirstOrDefault();
            if (siparis == null)
                throw new InvalidOperationException("Sipariş bulunamadı.");

            long localSiparisId = siparis.Id;
            long? localMusteriId = siparis.MusteriId;

            Musteri musteri = musteriRepo.FindBy(e => e.Id == localMusteriId).FirstOrDefault();
            Adres adres = null;
            if (musteri?.AdresId != null)
                adres = adresRepo.FindBy(e => e.Id == musteri.AdresId).FirstOrDefault();

            SiparisStokSablon sablon = new SiparisStokSablon();
            aluKgFiyat = GetSabitDegerOrDefault(sabitRepo, 2);
            if (siparis.SistemBirimFiyat != null)
                aluKgFiyat = (decimal)siparis.SistemBirimFiyat;

            List<SiparisStokProfil> profilList = new List<SiparisStokProfil>();
            List<SiparisStokAksesuar> aksesuarList = new List<SiparisStokAksesuar>();
            List<SiparisStok> siparisStokList = siparisStokRepo.FindBy(e => e.SiparisId == localSiparisId).ToList();

            List<OptimizasyonHesap> optimizasyonHesaps = new List<OptimizasyonHesap>();
            try
            {
                optimizasyonHesaps = GetOrRunOptimizasyonHesaps(siparis.Id) ?? new List<OptimizasyonHesap>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[BuildSiparisSablon] Optimizasyon verisi alınamadı SiparisId=" + siparis.Id + ": " + ex.Message + "\n" + ex.StackTrace);
            }

            List<ProfilGonderimSablonModel> profilGonderimList = optimizasyonHesaps
                .Where(e => e != null && e.ProfilId != null && e.ProfilBoy != null && e.KesimAdet != null)
                .GroupBy(e => new { ProfilId = e.ProfilId.Value, ProfilBoy = e.ProfilBoy.Value })
                .Select(g => new ProfilGonderimSablonModel
                {
                    ProfilId = g.Key.ProfilId,
                    ProfilBoy = g.Key.ProfilBoy,
                    ProfilAdet = g.Sum(x => x.KesimAdet ?? 0)
                })
                .OrderBy(e => e.ProfilId)
                .ThenBy(e => e.ProfilBoy)
                .ToList();

            if (profilGonderimList.Count > 0)
            {
                foreach (var item in profilGonderimList)
                {
                    Profil profil = profilRepo.FindBy(e => e.Id == item.ProfilId).FirstOrDefault();
                    if (profil == null)
                        continue;

                    SiparisStokProfil siparisStokProfil = new SiparisStokProfil();
                    siparisStokProfil.Kodu = SanitizeExcelText(profil.ProfilKodu);
                    siparisStokProfil.Adi = SanitizeExcelText(profil.ProfilAdi);
                    siparisStokProfil.Kesit = SanitizeExcelText(profil.ProfilFoto);
                    siparisStokProfil.BirimAgirlik = (double)(profil.BirimAgirlik ?? 0) / 1000;
                    siparisStokProfil.Birim = "BOY";
                    siparisStokProfil.Renk = SanitizeExcelText(siparis.Renk?.RenkAdi ?? "");
                    siparisStokProfil.Olcu = (double)item.ProfilBoy / 1000;
                    siparisStokProfil.Miktar = item.ProfilAdet;
                    siparisStokProfil.ToplamMetre = (double)(siparisStokProfil.Olcu * siparisStokProfil.Miktar);
                    siparisStokProfil.ToplamKg = siparisStokProfil.BirimAgirlik * siparisStokProfil.ToplamMetre;
                    siparisStokProfil.BirimFiyatKgM = aluKgFiyat;
                    siparisStokProfil.ToplamTutar = siparisStokProfil.BirimFiyatKgM * (decimal)siparisStokProfil.ToplamKg;

                    if (!string.Equals(profil.ProfilKodu, SacBoruProfilKodu, StringComparison.OrdinalIgnoreCase))
                    {
                        profilList.Add(siparisStokProfil);
                    }
                    else
                    {
                        SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                        siparisStokAksesuar.Kodu = SanitizeExcelText(profil.ProfilKodu);
                        siparisStokAksesuar.Adi = SanitizeExcelText(profil.ProfilAdi);
                        siparisStokAksesuar.Birim = "METRE";
                        siparisStokAksesuar.BirimFiyat = GetSabitDegerOrDefault(sabitRepo, 6);
                        siparisStokAksesuar.Miktar = (decimal)siparisStokProfil.ToplamMetre;
                        siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.BirimFiyat * (decimal)siparisStokProfil.ToplamMetre;
                        aksesuarList.Add(siparisStokAksesuar);
                    }
                }
            }
            else
            {
                foreach (var item in siparisStokList.Where(e => e.ProfilId != null).ToList())
                {
                    Profil profil = profilRepo.FindBy(e => e.Id == item.ProfilId).FirstOrDefault();
                    int? profilAdet = item.ProfilAdet;
                    if (profil == null || profilAdet == null)
                        continue;

                    var profilBoy = item.ProfilBoyId != null ? profilBoyRepo.FindBy(e => e.Id == item.ProfilBoyId).FirstOrDefault() : null;
                    SiparisStokProfil siparisStokProfil = new SiparisStokProfil();
                    siparisStokProfil.Kodu = SanitizeExcelText(profil.ProfilKodu);
                    siparisStokProfil.Adi = SanitizeExcelText(profil.ProfilAdi);
                    siparisStokProfil.Kesit = SanitizeExcelText(profil.ProfilFoto);
                    siparisStokProfil.BirimAgirlik = (double)(profil.BirimAgirlik ?? 0) / 1000;
                    siparisStokProfil.Birim = "BOY";
                    siparisStokProfil.Renk = SanitizeExcelText(siparis.Renk?.RenkAdi ?? "");
                    siparisStokProfil.Miktar = profilAdet.Value;
                    siparisStokProfil.Olcu = Convert.ToDouble(profilBoy?.ProfilBoyu ?? 0) / 1000;
                    siparisStokProfil.ToplamMetre = (double)(siparisStokProfil.Olcu * siparisStokProfil.Miktar);
                    siparisStokProfil.ToplamKg = siparisStokProfil.BirimAgirlik * siparisStokProfil.ToplamMetre;
                    siparisStokProfil.BirimFiyatKgM = aluKgFiyat;
                    siparisStokProfil.ToplamTutar = siparisStokProfil.BirimFiyatKgM * (decimal)siparisStokProfil.ToplamKg;

                    if (!string.Equals(profil.ProfilKodu, SacBoruProfilKodu, StringComparison.OrdinalIgnoreCase))
                    {
                        profilList.Add(siparisStokProfil);
                    }
                    else
                    {
                        SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                        siparisStokAksesuar.Kodu = SanitizeExcelText(profil.ProfilKodu);
                        siparisStokAksesuar.Adi = SanitizeExcelText(profil.ProfilAdi);
                        siparisStokAksesuar.Birim = "METRE";
                        siparisStokAksesuar.BirimFiyat = GetSabitDegerOrDefault(sabitRepo, 6);
                        siparisStokAksesuar.Miktar = (decimal)siparisStokProfil.ToplamMetre;
                        siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.BirimFiyat * (decimal)siparisStokProfil.ToplamMetre;
                        aksesuarList.Add(siparisStokAksesuar);
                    }
                }
            }

            List<SiparisEnBoyAdet> enBoyList = sebaRepo.FindBy(e => e.SiparisId == siparisId).ToList();
            List<long> enBoyAdetIds = enBoyList.Select(e => e.Id).ToList();
            List<SiparisTeklif> siparisTeklifs = enBoyAdetIds.Count > 0
                ? siparisTeklifRepo.FindBy(e => e.SiparisEnBoyAdetId != null && enBoyAdetIds.Contains((long)e.SiparisEnBoyAdetId)).ToList()
                : new List<SiparisTeklif>();

            foreach (var item in siparisAksesuarRepo.FindBy(e => e.SiparisId == localSiparisId).ToList())
            {
                Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == item.AksesuarId).FirstOrDefault();
                if (aksesuar == null)
                    continue;

                SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                siparisStokAksesuar.Adi = SanitizeExcelText(aksesuar.AksesuarAdi);
                siparisStokAksesuar.Birim = SanitizeExcelText(aksesuar.AksesuarBirim);
                siparisStokAksesuar.Kodu = SanitizeExcelText(aksesuar.AksesuarKodu);
                siparisStokAksesuar.BirimFiyat = aksesuar.BirimFiyat ?? 0;
                if (item.BirimFiyat != null && item.BirimFiyat > 0)
                    siparisStokAksesuar.BirimFiyat = item.BirimFiyat.Value;

                decimal teklifMiktar = siparisTeklifs
                    .Where(e => string.Equals((e.Malzeme ?? string.Empty).Trim(), (aksesuar.AksesuarAdi ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase))
                    .Sum(e => e.Miktar ?? 0);
                decimal stokMiktar = siparisStokList
                    .Where(e => e.AksesuarId == item.AksesuarId)
                    .Sum(e => (decimal)(e.AksesuarAdet ?? 0));
                if (teklifMiktar <= 0)
                {
                    if (stokMiktar > 0)
                        System.Diagnostics.Debug.WriteLine("[BuildSiparisSablon] Aksesuar miktarı stoktan alındı. SiparisId=" + siparisId + ", AksesuarId=" + item.AksesuarId);
                    else
                        System.Diagnostics.Debug.WriteLine("[BuildSiparisSablon] Aksesuar miktarı 0. SiparisId=" + siparisId + ", AksesuarId=" + item.AksesuarId);
                }

                siparisStokAksesuar.Miktar = teklifMiktar > 0 ? teklifMiktar : stokMiktar;
                siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.Miktar * siparisStokAksesuar.BirimFiyat;
                aksesuarList.Add(siparisStokAksesuar);
            }

            sablon.aksesuarList = aksesuarList.OrderBy(e => e.Kodu ?? string.Empty).ThenBy(e => e.Adi ?? string.Empty).ToList();
            sablon.profilList = profilList.OrderBy(e => e.Kodu ?? string.Empty).ThenBy(e => e.Olcu).ToList();
            sablon.SiparisId = siparisId;
            DateTime? siparisTarih = siparis.TahminiTeslim ?? siparis.KayitTarihi;
            if (siparisTarih == null)
                System.Diagnostics.Debug.WriteLine("[BuildSiparisSablon] Sipariş tarihi bulunamadı, DateTime.Now kullanılacak. SiparisId=" + siparisId);
            sablon.SiparisTarih = siparisTarih ?? DateTime.Now;

            if (siparis.ToplamAluKg != null)
                sablon.ProfilToplamKg = (double)siparis.ToplamAluKg / 1000;
            else
                sablon.ProfilToplamKg = sablon.profilList.Where(e => !(e.Kodu ?? string.Empty).Contains("DP-")).Sum(e => e.ToplamKg);

            if (siparis.ToplamAluKgFiyat != null)
                sablon.ProfilToplamTutar = (decimal)siparis.ToplamAluKgFiyat;
            else
                sablon.ProfilToplamTutar = sablon.profilList.Where(e => !(e.Kodu ?? string.Empty).Contains("DP-")).Sum(e => e.ToplamTutar);

            sablon.AksesuarToplamTutar = sablon.aksesuarList.Sum(e => e.ToplamTutar);
            sablon.SirketAd = SanitizeExcelText(siparis.MusteriTamAdi);
            sablon.SirketAdres = BuildAdresMetni(adres);
            return sablon;
        }

        public ActionResult SiparisPdfYazdir(long SiparisId)
        {
            SiparisStokSablon sablon = BuildSiparisSablon(SiparisId, out _, out decimal aluKgFiyat);
            ViewBag.AluKg = aluKgFiyat;

            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "utf-8";
            return PartialView("_stoktanSiparisSablon4Pdf", sablon);
        }

        void excelStoktanKaydet(long siparisId)
        {
            string pathAfter = Path.Combine(GetExportTempPath(), siparisId + "_nolu_siparis");

            SabitRepo sabitRepo = new SabitRepo();
            AdresRepo adresRepo = new AdresRepo();
            SiparisStokSablon sablon = new SiparisStokSablon();
            decimal aluKgFiyat = GetSabitDegerOrDefault(sabitRepo, 2);
            profilRepo = new ProfilRepo();
            profilBoyRepo = new ProfilBoyRepo();
            siparisStokRepo = new SiparisStokRepo();
            siparisRepo = new SiparisRepo();
            aksesuarRepo = new AksesuarRepo();
            musteriRepo = new MusteriRepo();

            List<SiparisStok> siparisStok = siparisStokRepo.FindBy(e => e.SiparisId == siparisId).ToList();
            Siparis siparis = siparisRepo.FindBy(e => e.Id == siparisId).FirstOrDefault();
            if (siparis == null)
                throw new InvalidOperationException("Sipariş bulunamadı.");

            if (siparis.SistemBirimFiyat != null)
                aluKgFiyat = (decimal)siparis.SistemBirimFiyat;

            List<SiparisStokProfil> profilList = new List<SiparisStokProfil>();
            List<SiparisStokAksesuar> aksesuarList = new List<SiparisStokAksesuar>();
            foreach (var item in siparisStok.Where(e => e.ProfilId != null).ToList())
            {
                Profil profil = profilRepo.FindBy(e => e.Id == item.ProfilId).FirstOrDefault();
                int? profilAdet = item.ProfilAdet;
                if (profil == null || profilAdet == null)
                    continue;

                var profilBoy = item.ProfilBoyId != null ? profilBoyRepo.FindBy(e => e.Id == item.ProfilBoyId).FirstOrDefault() : null;

                SiparisStokProfil siparisStokProfil = new SiparisStokProfil();

                siparisStokProfil.Kodu = SanitizeExcelText(profil.ProfilKodu);
                siparisStokProfil.Adi = SanitizeExcelText(profil.ProfilAdi);
                siparisStokProfil.Kesit = SanitizeExcelText(profil.ProfilFoto);
                siparisStokProfil.BirimAgirlik = (double)(profil.BirimAgirlik ?? 0) / 1000;
                siparisStokProfil.Birim = "BOY";
                siparisStokProfil.Renk = SanitizeExcelText(siparis.Renk?.RenkAdi ?? "");
                siparisStokProfil.Miktar = profilAdet.Value;
                siparisStokProfil.Olcu = Convert.ToDouble(profilBoy?.ProfilBoyu ?? 0) / 1000;
                siparisStokProfil.ToplamMetre = (double)(siparisStokProfil.Olcu * siparisStokProfil.Miktar);

                siparisStokProfil.ToplamKg = siparisStokProfil.BirimAgirlik * siparisStokProfil.ToplamMetre;
                siparisStokProfil.BirimFiyatKgM = aluKgFiyat;
                siparisStokProfil.ToplamTutar = siparisStokProfil.BirimFiyatKgM * (decimal)siparisStokProfil.ToplamKg;

                if (!string.Equals(profil.ProfilKodu, SacBoruProfilKodu, StringComparison.OrdinalIgnoreCase))
                {
                    profilList.Add(siparisStokProfil);
                }
                else
                {
                    SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                    siparisStokAksesuar.Kodu = SanitizeExcelText(profil.ProfilKodu);
                    siparisStokAksesuar.Adi = SanitizeExcelText(profil.ProfilAdi);
                    siparisStokAksesuar.Birim = "METRE";
                    siparisStokAksesuar.BirimFiyat = GetSabitDegerOrDefault(sabitRepo, 6);
                    siparisStokAksesuar.Miktar = (decimal)siparisStokProfil.ToplamMetre;
                    siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.BirimFiyat * (decimal)siparisStokProfil.ToplamMetre;
                    aksesuarList.Add(siparisStokAksesuar);
                }
            }

            foreach (var item in siparisStok.Where(e => e.AksesuarId != null).ToList())
            {
                Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == item.AksesuarId).FirstOrDefault();
                if (aksesuar == null) continue;
                SiparisStokAksesuar siparisStokAksesuar = new SiparisStokAksesuar();
                siparisStokAksesuar.Adi = SanitizeExcelText(aksesuar.AksesuarAdi);
                siparisStokAksesuar.Birim = SanitizeExcelText(aksesuar.AksesuarBirim);
                siparisStokAksesuar.BirimFiyat = aksesuar.BirimFiyat ?? 0;
                siparisStokAksesuar.Kodu = SanitizeExcelText(aksesuar.AksesuarKodu);
                siparisStokAksesuar.Miktar = item.AksesuarAdet != null ? (int)item.AksesuarAdet : 0;

                siparisStokAksesuar.ToplamTutar = siparisStokAksesuar.Miktar * siparisStokAksesuar.BirimFiyat;
                aksesuarList.Add(siparisStokAksesuar);
            }

            sablon.aksesuarList = aksesuarList.OrderBy(e => e.Kodu ?? string.Empty).ThenBy(e => e.Adi ?? string.Empty).ToList();
            sablon.profilList = profilList.OrderBy(e => e.Kodu ?? string.Empty).ThenBy(e => e.Olcu).ToList();
            sablon.SiparisId = siparisId;
            sablon.SiparisTarih = Convert.ToDateTime(siparis.TahminiTeslim);
            sablon.AksesuarToplamTutar = aksesuarList.Sum(e => e.ToplamTutar);
            sablon.SirketAd = SanitizeExcelText(siparis.MusteriTamAdi);

            if (siparis.ToplamAluKg != null)
                sablon.ProfilToplamKg = (double)siparis.ToplamAluKg / 1000;
            else
                sablon.ProfilToplamKg = sablon.profilList.Where(e => !(e.Kodu ?? string.Empty).Contains("DP-")).Sum(e => e.ToplamKg);

            if (siparis.ToplamAluKgFiyat != null)
                sablon.ProfilToplamTutar = (decimal)siparis.ToplamAluKgFiyat;
            else
                sablon.ProfilToplamTutar = sablon.profilList.Where(e => !(e.Kodu ?? string.Empty).Contains("DP-")).Sum(e => e.ToplamTutar);

            var musteri = musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault();
            Adres adres = null;
            if (musteri?.AdresId != null)
                adres = adresRepo.FindBy(e => e.Id == musteri.AdresId).FirstOrDefault();
            sablon.SirketAdres = BuildAdresMetni(adres);

            ViewBag.AluKg = aluKgFiyat;

            string path = Server.MapPath("~/Assets/sablonStokYeni.xlsx");

            ExcelPackage excel = CreateExcelPackage(path, "Siparis");
            ExcelWorksheet xlWorkSheet = excel.Workbook.Worksheets.First();
            xlWorkSheet.Cells.Style.Font.Name = "Arial";
            xlWorkSheet.Cells["A5"].Value = SanitizeExcelText(siparis.MusteriTamAdi);
            xlWorkSheet.Cells["D5"].Value = BuildAdresMetni(adres);
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
                xlWorkSheet.Cells[i, 1].Value = SanitizeExcelText(item.Kodu);
                xlWorkSheet.Cells[i, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 2].Value = SanitizeExcelText(item.Adi);
                xlWorkSheet.Cells[i, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                xlWorkSheet.Cells[i, 4].Value = item.BirimAgirlik;
                xlWorkSheet.Cells[i, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 5].Value = item.Birim;
                xlWorkSheet.Cells[i, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 6].Value = SanitizeExcelText(item.Renk);
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
                // Picture is added after InsertRow to avoid EPPlus ArgumentException when
                // adjusting existing drawing positions during row insertion.
                TryAddProfilKesitPicture(xlWorkSheet, i, item.Kesit, uniqueName++.ToString());
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
                xlWorkSheet.Cells[x, 1].Value = SanitizeExcelText(item.Kodu);
                xlWorkSheet.Cells[x, 2].Value = SanitizeExcelText(item.Adi);
                xlWorkSheet.Cells[x, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[x, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                xlWorkSheet.Cells[string.Format("F{0}:J{0}", x)].Merge = true;
                xlWorkSheet.Cells[x, 4].Value = SanitizeExcelText(item.Birim);
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

        void excelKaydet(long siparisId)
        {
            string pathAfter = Path.Combine(GetExportTempPath(), siparisId + "_nolu_siparis");

            SiparisStokSablon sablon = BuildSiparisSablon(siparisId, out Siparis siparis, out decimal aluKgFiyat);
            ViewBag.AluKg = aluKgFiyat;

            string path = Server.MapPath("~/Assets/sablonStokYeni.xlsx");

            ExcelPackage excel = CreateExcelPackage(path, "Siparis");
            ExcelWorksheet xlWorkSheet = excel.Workbook.Worksheets.First();
            xlWorkSheet.Cells.Style.Font.Name = "Arial";
            xlWorkSheet.Cells["A5"].Value = SanitizeExcelText(siparis.MusteriTamAdi);
            xlWorkSheet.Cells["D5"].Value = sablon.SirketAdres;
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
                xlWorkSheet.Cells[i, 1].Value = SanitizeExcelText(item.Kodu);
                xlWorkSheet.Cells[i, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 2].Value = SanitizeExcelText(item.Adi);
                xlWorkSheet.Cells[i, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                xlWorkSheet.Cells[i, 4].Value = item.BirimAgirlik;
                xlWorkSheet.Cells[i, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 5].Value = item.Birim;
                xlWorkSheet.Cells[i, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[i, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[i, 6].Value = SanitizeExcelText(item.Renk);
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
                // Picture is added after InsertRow to avoid EPPlus ArgumentException when
                // adjusting existing drawing positions during row insertion.
                TryAddProfilKesitPicture(xlWorkSheet, i, item.Kesit, uniqueName++.ToString());
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
                xlWorkSheet.Cells[x, 1].Value = SanitizeExcelText(item.Kodu);
                xlWorkSheet.Cells[x, 2].Value = SanitizeExcelText(item.Adi);
                xlWorkSheet.Cells[x, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                xlWorkSheet.Cells[x, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                xlWorkSheet.Cells[string.Format("F{0}:J{0}", x)].Merge = true;
                xlWorkSheet.Cells[x, 4].Value = SanitizeExcelText(item.Birim);
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
