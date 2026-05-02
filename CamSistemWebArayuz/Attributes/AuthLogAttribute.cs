using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace CamSistemWebArayuz.Attributes
{
    public class AuthLogAttribute : AuthorizeAttribute
    {
        public AuthLogAttribute()
        {
            View = "Home";
        }

        public string View { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            // base authorize sonucuna göre kontrol yapıyoruz
            IsUserAuthorized(filterContext);
        }

        private void IsUserAuthorized(AuthorizationContext authorizationContext)
        {
            // base authorize herhangi bir sonuç üretmediyse (izin varsa) devam etme
            if (authorizationContext.Result == null)
                return;

            // --- WHITELIST: bu sayfalar asla permission check'e takılmamalı (redirect loop'u önler) ---
            var controller = authorizationContext.RouteData.Values["controller"]?.ToString() ?? "";
            var action = authorizationContext.RouteData.Values["action"]?.ToString() ?? "";

            if (string.Equals(controller, "Permission", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(action, "Denied", StringComparison.OrdinalIgnoreCase))
            {
                authorizationContext.Result = null;
                return;
            }

            if (string.Equals(controller, "Login", StringComparison.OrdinalIgnoreCase))
            {
                authorizationContext.Result = null;
                return;
            }
            // -----------------------------------------------------------------------------------------

            if (!authorizationContext.HttpContext.User.Identity.IsAuthenticated)
                return;

            string kullaniciMail = authorizationContext.HttpContext.User.Identity.Name;

            string controllerAdi = controller;

            // controller -> sayfa adı map
            switch (controllerAdi)
            {
                case "Home": controllerAdi = "ANASAYFA"; break;
                case "Kullanici": controllerAdi = "KULLANİCİ"; break;
                case "Musteri": controllerAdi = "MUSTERİ"; break;
                case "Tedarik": controllerAdi = "TEDARİK"; break;
                case "Siparis": controllerAdi = "SİPARİS"; break;
                case "Imalat": controllerAdi = "IMALAT"; break;
                case "Stok": controllerAdi = "STOK"; break;
                case "Tanimlama": controllerAdi = "TANİMLAMA"; break;
                case "Fiyat": controllerAdi = "FİYATLANDIRMA"; break;
                case "Yetki": controllerAdi = "YETKİ"; break;
                case "Rapor": controllerAdi = "RAPOR"; break;
            }

            // Action'daki roller (yetkiler)
            if (string.IsNullOrWhiteSpace(Roles))
            {
                authorizationContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Permission", action = "Denied" }));
                return;
            }

            string[] roles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(x => x.Trim())
                                  .ToArray();

            // Repo init
            var kullaniciRepo = new KullaniciRepo();
            var kullaniciRolRepo = new KullaniciRolRepo();
            var rolSayfaYetkiRepo = new RolSayfaYetkiRepo();
            var sayfaRepo = new SayfaRepo();
            var yetkiRepo = new YetkiRepo();

            // Yetki id'leri
            List<int> yetkiIdleri = yetkiRepo.FindBy(e => roles.Contains(e.YetkiAdi))
                                             .Select(e => e.Id)
                                             .ToList();

            // Sayfa
            Sayfa sayfa = sayfaRepo.FindBy(e => e.SayfaAdi.Equals(controllerAdi)).FirstOrDefault();
            if (sayfa == null)
            {
                authorizationContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Permission", action = "Denied" }));
                return;
            }

            // Kullanıcı
            Kullanici kullanici = kullaniciRepo.FindBy(e => e.KullaniciMail.Equals(kullaniciMail)).FirstOrDefault();
            if (kullanici == null)
            {
                authorizationContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Permission", action = "Denied" }));
                return;
            }

            // Kullanıcı rolü
            KullaniciRol kullaniciRol = kullaniciRolRepo.FindBy(e => e.KullaniciId == kullanici.Id).FirstOrDefault();
            if (kullaniciRol == null)
            {
                authorizationContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Permission", action = "Denied" }));
                return;
            }

            // RolSayfaYetki kontrol
            List<RolSayfaYetki> roller = rolSayfaYetkiRepo.FindBy(e =>
                    e.RolId == kullaniciRol.RolId &&
                    e.SayfaId == sayfa.Id &&
                    e.YetkiId != null &&
                    yetkiIdleri.Contains((int)e.YetkiId)
                ).ToList();

            if (roller.Count > 0)
            {
                authorizationContext.Result = null; // izin ver
                return;
            }

            authorizationContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "Permission", action = "Denied" }));
        }
    }
}