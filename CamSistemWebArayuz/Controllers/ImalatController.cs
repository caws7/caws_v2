using CamSistemDataLayer.BussinesLogic;
using CamSistemDataLayer.Enums;
using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using CamSistemWebArayuz.Attributes;
using CamSistemWebArayuz.Models;
using Optimizasyon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CamSistemWebArayuz.Controllers
{
    [SessionController]
    [AuthLog(Roles = "IMALAT")]
    public class ImalatController : Controller
    {
        SiparisRepo sRepo;
        StokRepo stokRepo;
        // GET: Imalat
        [AuthLog(Roles = "IMALAT,GORUNTULEME")]
        public ActionResult Index()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "ImalatSayfasi";

            sRepo = new SiparisRepo();

            return View(sRepo.FindBy(e => e.DurumId == (int)Durumlar.Onaylandı && e.SiparisTur.Equals("Demonte Gönderim") && e.SistemId != 2));
        }

        [AuthLog(Roles = "IMALAT,GORUNTULEME")]
        public ActionResult ImalatKesimGoruntule(List<string> hatalar, List<string> kesimBicimiStok, List<string> kesimBicimiFireStok,
            string toplamAtikUzunluk, string fireStogaEklenenToplam, string kullanilanToplamUzunlukAsil, string kullanilanToplamUzunlukFire, string fireStogaEklenenToplamAgirlik,
            string kullanilanToplamAgirlikAsil, string kullanilanToplamAgirlikFire, string toplamAtikAgirlik)
        {
            stokRepo = new StokRepo();
            sRepo = new SiparisRepo();
            ImalatModel model = new ImalatModel();
            ProfilRepo pRepo = new ProfilRepo();
            ProfilBoyRepo pbRepo = new ProfilBoyRepo();
            List<OptimizasyonSonuc> listSonuc = new List<OptimizasyonSonuc>();
            SabitRepo sabitRepo = new SabitRepo();
            AtikStokRepo atikStokRepo = new AtikStokRepo();

            List<CamSistemDataLayer.Models.Stok> yetersizStokList = new List<CamSistemDataLayer.Models.Stok>();
            List<CamSistemDataLayer.Models.Profil> kesilenProfilList = new List<CamSistemDataLayer.Models.Profil>();
            foreach (var item in kesimBicimiStok)
            {
                //profil_id#Kesilen Profil#Elde edilen profiller#fire#kesim adeti#eksik adet
                OptimizasyonSonuc ent = new OptimizasyonSonuc();
                string[] split = item.Split('#');
                int profilId = Convert.ToInt32(split[0].Trim());
                int profilBoy = Convert.ToInt32(split[1].Trim());
                int profilBoyId = pbRepo.FindBy(e => e.ProfilBoyu == profilBoy).FirstOrDefault().Id;
                CamSistemDataLayer.Models.Profil profil = pRepo.FindBy(e => e.Id == profilId).FirstOrDefault();

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
                    //profil_id#Kesilen Profil#Elde edilen profiller#fire#kesim adeti
                    OptimizasyonSonuc ent = new OptimizasyonSonuc();
                    string[] split = item.Split('#');
                    int profilId = Convert.ToInt32(split[0].Trim());
                    int profilBoy = Convert.ToInt32(split[1].Trim());
                    CamSistemDataLayer.Models.Profil profil = pRepo.FindBy(e => e.Id == profilId).FirstOrDefault();

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

            model.fireStogaEklenenToplam = Math.Round(double.Parse(fireStogaEklenenToplam, System.Globalization.CultureInfo.InvariantCulture), 2);
            model.fireStogaEklenenToplamAgirlik = Math.Round(double.Parse(fireStogaEklenenToplamAgirlik, System.Globalization.CultureInfo.InvariantCulture), 2);
            model.kullanilanToplamUzunlukAsil = Math.Round(double.Parse(kullanilanToplamUzunlukAsil, System.Globalization.CultureInfo.InvariantCulture), 2);
            model.kullanilanToplamAgirlikAsil = Math.Round(double.Parse(kullanilanToplamAgirlikAsil, System.Globalization.CultureInfo.InvariantCulture), 2);
            model.kullanilanToplamUzunlukFire = Math.Round(double.Parse(kullanilanToplamUzunlukFire, System.Globalization.CultureInfo.InvariantCulture), 2);
            model.kullanilanToplamAgirlikFire = Math.Round(double.Parse(kullanilanToplamAgirlikFire, System.Globalization.CultureInfo.InvariantCulture), 2);
            model.toplamAtikUzunluk = Math.Round(double.Parse(toplamAtikUzunluk, System.Globalization.CultureInfo.InvariantCulture), 2);
            model.toplamAtikAgirlik = Math.Round(double.Parse(toplamAtikAgirlik, System.Globalization.CultureInfo.InvariantCulture), 2);
            model.optiSonuc = listSonuc;
            model.yetersizStokList = yetersizStokList;
            ViewBag.minimumFire = sabitRepo.FindBy(e => e.Id == 1).FirstOrDefault().SabitDeger;

            //optimizasyon sonucu temp listeye kaydediyoruz.
            Kullanici kullaniciModel = (Kullanici)Session["CurrentUser"];
            List<OptimizasyonHesap> optimizasyonSonucList = new List<OptimizasyonHesap>();
            string siparisAciklama = "";
            List<long> ids = TempData["siparisIds"] as List<long>;
            foreach (var item in ids)
            {
                string aciklama = sRepo.FindBy(e => e.Id == item).FirstOrDefault().Aciklama;
                siparisAciklama += "<div class='alert alert-danger'><strong>" + item + " Nolu Sipariş Açıklaması:</strong> " + aciklama + "</div>";
            }
            foreach (var item in listSonuc)
            {
                OptimizasyonHesap hesap = new OptimizasyonHesap();
                hesap.SiparisIds = string.Join(", ", ids);
                TempData["siparisIds"] = null;
                TempData["siparisIds"] = ids;
                hesap.KesilecekOlculer = item.KesileceklerOlcusu;
                hesap.ProfilBoy = item.KullanilacakOlcu;
                hesap.ProfilId = item.profil.Id;
                hesap.KesimAdet = item.Adet;
                hesap.FireAtik = item.FireAtik;

                hesap.ToplamAtikUzunluk = Math.Round(decimal.Parse(toplamAtikUzunluk, System.Globalization.CultureInfo.InvariantCulture), 2);
                hesap.ToplamAtikAgirlik = Math.Round(decimal.Parse(toplamAtikAgirlik, System.Globalization.CultureInfo.InvariantCulture), 2);
                hesap.AsilStoktanKullanilanToplamUzunluk = Math.Round(decimal.Parse(kullanilanToplamUzunlukAsil, System.Globalization.CultureInfo.InvariantCulture), 2);
                hesap.AsilStoktanKullanilanToplamAgirlik = Math.Round(decimal.Parse(kullanilanToplamAgirlikAsil, System.Globalization.CultureInfo.InvariantCulture), 2);
                hesap.FiredenKullanilanToplamUzunluk = Math.Round(decimal.Parse(kullanilanToplamUzunlukFire, System.Globalization.CultureInfo.InvariantCulture), 2);
                hesap.FiredenKullanilanToplamAgirlik = Math.Round(decimal.Parse(kullanilanToplamAgirlikFire, System.Globalization.CultureInfo.InvariantCulture), 2);
                hesap.FireyeEklenenToplamUzunluk = Math.Round(decimal.Parse(fireStogaEklenenToplam, System.Globalization.CultureInfo.InvariantCulture), 2);
                hesap.FireyeEklenenToplamAgirlik = Math.Round(decimal.Parse(fireStogaEklenenToplamAgirlik, System.Globalization.CultureInfo.InvariantCulture), 2);
                hesap.KullanilanAlan = item.KullanilanAlan;
                hesap.KayitTarih = DateTime.Now;
                hesap.KullaniciId = kullaniciModel.Id;

                optimizasyonSonucList.Add(hesap);
            }


            ViewBag.SiparisAciklamasi = siparisAciklama;
            TempData["kaydedilecekOptiSonuc"] = optimizasyonSonucList;

            return PartialView("_imalatKesimListesi", model);
        }

        [AuthLog(Roles = "ONAYLAMA")]
        public JsonResult ImalataGonder(List<long> siparisIds, bool fireKullanilsinMi, bool stoktanDussunMu)
        {
            Input input = new Input();
            FireStok fire = new FireStok();
            Optimizasyon.Stok stok = new Optimizasyon.Stok();
            AtikStokRepo asRepo = new AtikStokRepo();
            SabitRepo sabitRepo = new SabitRepo();
            stokRepo = new StokRepo();
            ProfilRepo pRepo = new ProfilRepo();
            ProfilBoyRepo pbRepo = new ProfilBoyRepo();
            SiparisRepo siparisRepo = new SiparisRepo();
            SiparisEnBoyAdetRepo sebaRepo = new SiparisEnBoyAdetRepo();


            //fire stok
            int fireMinDeger = (int)sabitRepo.FindBy(e => e.Id == 1).FirstOrDefault().SabitDeger;
            List<Optimizasyon.Profil> fireler = new List<Optimizasyon.Profil>();

            List<AtikStok> atikStok = asRepo.GetAll().ToList();

            fire.minDeger = fireMinDeger;
            foreach (AtikStok item in atikStok)
            {
                Optimizasyon.Profil profil = new Optimizasyon.Profil();
                profil.Adet = (int)item.Adet;
                profil.Boy = (int)item.Olcu;

                //profil idsi gönderip id alacağız
                profil.Profil_Kod = (int)item.ProfilId;
                fireler.Add(profil);
            }
            fire.Fireler = fireler;
            input.FireStok = fire;

            //stok
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
                        dic[item2.Boy] += item2.Adet; // veya üzerine yazabilirsin: dic[item2.Boy] = item2.Adet;
                }

                if (!stoktakiProfiller.ContainsKey(item.Id))
                    stoktakiProfiller.Add(item.Id, dic);
                else
                {
                    // Daha önce varsa, mevcut dictionary ile yeni dic içini birleştir:
                    foreach (var pair in dic)
                    {
                        if (!stoktakiProfiller[item.Id].ContainsKey(pair.Key))
                            stoktakiProfiller[item.Id].Add(pair.Key, pair.Value);
                        else
                            stoktakiProfiller[item.Id][pair.Key] += pair.Value; // veya üzerine yaz: stoktakiProfiller[item.Id][pair.Key] = pair.Value;
                    }
                }
            }
            stok.Stoktaki_Profiller = stoktakiProfiller;
            input.Stok = stok;
            input.ProfilBirimAgirlik = dicProfilBirimAgirlik;

            //Sipariş
            List<Optimizasyon.Siparis> siparisler = new List<Optimizasyon.Siparis>();
            Dictionary<List<CamSistemDataLayer.Models.Profil>, long> spList = new Dictionary<List<CamSistemDataLayer.Models.Profil>, long>();

            foreach (var item in siparisIds)
            {
                List<SiparisEnBoyAdet> siparisAdet = sebaRepo.FindBy(e => e.SiparisId == item).ToList();
                foreach (var item2 in siparisAdet)
                {
                    // Profil hesaplama
                    var hesaplananProfiller = SiparisHesaplamalari.profilHesaplama(
                        item, // <--- SiparişID
                        (int)item2.GirilenEn,
                        (int)item2.GirilenSolEn,
                        (int)item2.GirilenBoy,
                        (int)item2.GirilenAdet
                    );
                    System.Diagnostics.Debug.WriteLine($"Siparis={item} - Profil Listesi Adedi: {hesaplananProfiller.Count}");
                    if (hesaplananProfiller.Count == 0)
                        System.Diagnostics.Debug.WriteLine("PROFİL YOK! HATALI!");
                    
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
                            String[] split = item2.ProfilKodu.Split('-');
                            String merge = split[0] + "-" + split[1];
                            CamSistemDataLayer.Models.Profil pro = pRepo.FindBy(e => e.ProfilKodu.Equals(merge)).First();
                            profil.Profil_Kod = pro.Id;
                        }
                        else
                            profil.Profil_Kod = item2.Id;
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


            //input
            Optimizer opti = new Optimizer(input, fireKullanilsinMi);
            opti.optimizeEt();

            //output
            TempData["output"] = null;
            TempData["siparisIds"] = null;
            TempData["stokDusmeDurum"] = null;

            TempData["output"] = opti.output;
            TempData["siparisIds"] = siparisIds;
            TempData["stokDusmeDurum"] = stoktanDussunMu;

            return Json(new { Result = opti.output, Fire = false }, JsonRequestBehavior.AllowGet);
        }

        [AuthLog(Roles = "ONAYLAMA,YENIKAYIT,DUZENLEME")]
        public JsonResult ImalatOnayla()
        {
            //imalata onayla butonuna basılınca optimizasyon sonucunu tabloya kaydediyoruz.
            OptimizasyonHesapRepo optimizasyonHesapRepo = new OptimizasyonHesapRepo();
            List<OptimizasyonHesap> optimizasyonSonucList = TempData["kaydedilecekOptiSonuc"] as List<OptimizasyonHesap>;
            foreach (var item in optimizasyonSonucList)
            {
                optimizasyonHesapRepo.AddAndSave(item);
            }

            //Stok düşümü sevkiyat onaylanınca olacağından buradaki manuel olarak false durumuna çekildi.
            bool stokDurum = false;//Convert.ToBoolean(TempData["stokDusmeDurum"]);
            List<long> siparisIds = TempData["siparisIds"] as List<long>;
            sRepo = new SiparisRepo();
            if (stokDurum)
            {
                Output output = TempData["output"] as Output;

                ProfilBoyRepo bRepo = new ProfilBoyRepo();
                AtikStokRepo fireRepo = new AtikStokRepo();
                stokRepo = new StokRepo();

                foreach (var item in output.stoktanKullanilanProfiller)
                {
                    int profilId = Convert.ToInt32(item.Profil_Kod);
                    ProfilBoy profilBoy = bRepo.FindBy(e => e.ProfilBoyu == item.Boy).FirstOrDefault();
                    CamSistemDataLayer.Models.Stok stok = null;
                    if (profilBoy != null)
                        stok = stokRepo.FindBy(e => e.ProfilId == profilId && e.ProfilBoyId == profilBoy.Id).FirstOrDefault();
                    else
                        stok = stokRepo.FindBy(e => e.ProfilId == profilId && e.OzelOlcu == item.Boy).FirstOrDefault();

                    stok.StokAdet = stok.StokAdet - item.Adet;
                    stokRepo.EditAndSave(stok);
                }

                foreach (var item in output.fireStoktanKullanilanProfiller)
                {
                    int profilId = Convert.ToInt32(item.Profil_Kod);
                    AtikStok fire = fireRepo.FindBy(e => e.ProfilId == profilId && e.Olcu == item.Boy).FirstOrDefault();

                    fire.Adet = fire.Adet - item.Adet;
                    fireRepo.EditAndSave(fire);
                }

                foreach (var item in output.fireStogaEklenecekProfiller)
                {
                    int profilId = Convert.ToInt32(item.Profil_Kod);
                    AtikStok fire = fireRepo.FindBy(e => e.ProfilId == profilId && e.Olcu == item.Boy).FirstOrDefault();

                    if (fire == null)
                    {
                        AtikStok stok = new AtikStok();
                        stok.Adet = item.Adet;
                        stok.Olcu = item.Boy;
                        stok.ProfilId = Convert.ToInt32(item.Profil_Kod);
                        fireRepo.AddAndSave(stok);
                    }
                    else
                    {
                        fire.Adet = fire.Adet + item.Adet;
                        fireRepo.EditAndSave(fire);
                    }
                }

                foreach (var item in siparisIds)
                {
                    CamSistemDataLayer.Models.Siparis siparis = sRepo.FindBy(e => e.Id == item).FirstOrDefault();
                    siparis.DurumId = (int)Durumlar.ImalataGonderildi;
                    siparis.GuncellemeTarihi = DateTime.Now;
                    sRepo.EditAndSave(siparis);
                }
                TempData["loader"] = "Lütfen bekleyiniz...";

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            else
            {
                foreach (var item in siparisIds)
                {
                    CamSistemDataLayer.Models.Siparis siparis = sRepo.FindBy(e => e.Id == item).FirstOrDefault();
                    siparis.DurumId = (int)Durumlar.ImalataGonderildi;
                    siparis.GuncellemeTarihi = DateTime.Now;
                    sRepo.EditAndSave(siparis);
                }

                TempData["stokDusmeDurum"] = null;
                return Json("NOK", JsonRequestBehavior.AllowGet);
            }
        }
    }
}