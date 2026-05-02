using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CamSistemWebArayuz.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            TempData["loader"] = null;
            TempData["loader"] = "Lütfen bekleyiniz...";
            return View();
        }

        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            ProfilRepo profilRepo = new ProfilRepo();
            ProfilBoyRepo profilBoyRepo = new ProfilBoyRepo();
            StokRepo stokRepo = new StokRepo();
            List<Profil> profils = profilRepo.GetAll().ToList();
            List<ProfilBoy> profilBoys = profilBoyRepo.GetAll().ToList();
            List<Stok> stoks = stokRepo.GetAll().ToList();

            KullaniciRepo kRepo = new KullaniciRepo();

            if (kRepo.KullaniciVarMi(Username, Password))
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    Username,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(60 * 12),
                    true,
                    "asdQWE**123/4",
                    FormsAuthentication.FormsCookiePath);

                string hash = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);

                cookie.Expires = DateTime.MinValue;
                if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;

                FormsAuthentication.SetAuthCookie(Username, true);

                // Kullanıcı kaydı
                var kullanici = kRepo.FindBy(e => e.KullaniciMail == Username).FirstOrDefault();
                Session["CurrentUser"] = kullanici;

                // Layout'ta kullanılan alanlar
                Session["KullaniciMail"] = kullanici?.KullaniciMail;
                Session["KullaniciAdSoyad"] = kullanici == null
                    ? "-"
                    : (kullanici.KullaniciAdi + " " + kullanici.KullaniciSoyadi);

                // Kullanıcı Rolü (KullaniciRol -> Rol tablosundan rol adını çek)
                string rolAdi = "-";
                if (kullanici != null)
                {
                    var krRepo = new KullaniciRolRepo();
                    var rolRepo = new RolRepo();

                    var kullaniciRol = krRepo.FindBy(x => x.KullaniciId == kullanici.Id).FirstOrDefault();
                    if (kullaniciRol != null)
                    {
                        var rol = rolRepo.FindBy(r => r.Id == kullaniciRol.RolId).FirstOrDefault();
                        if (rol != null)
                        {
                            rolAdi = rol.RolAdi; // Eğer derleme hatası verirse: Rol modelindeki doğru alan adı ile değiştir (Ad / RoleName / RolAd vb.)
                        }
                    }
                }

                Session["KullaniciRol"] = rolAdi;

                Response.Cookies.Add(cookie);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Hata = "Kullanıcı bulunamadı! Kullanıcı adı veya şifre bilgilerinizi kontrol ediniz.";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session["CurrentUser"] = null;
            Session["KullaniciMail"] = null;
            Session["KullaniciAdSoyad"] = null;
            Session["KullaniciRol"] = null;

            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Login", "Login");
        }
    }
}