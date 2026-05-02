using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace CamSistemWebArayuz.Roller
{
    public class RolYonetim : RoleProvider
    {
        private string pApplicationName;

        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }
        //public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            var asd = ApplicationName;
            KullaniciRepo kullaniciRepo = new KullaniciRepo();
            KullaniciRolRepo kullaniciRolRepo = new KullaniciRolRepo();
            RolSayfaYetkiRepo rolSayfaYetkiRepo = new RolSayfaYetkiRepo();
            SayfaRepo sayfaRepo = new SayfaRepo();

            Kullanici kullanici = kullaniciRepo.FindBy(e => e.KullaniciMail.Equals(username)).FirstOrDefault();
            KullaniciRol kullaniciRol = kullaniciRolRepo.FindBy(e => e.KullaniciId == kullanici.Id).FirstOrDefault();
            string[] sayfaListesi = { "ANASAYFA", "GORUNTULE" };

            if (kullaniciRol != null)
            {
                List<RolSayfaYetki> roller = rolSayfaYetkiRepo.FindBy(e => e.RolId == kullaniciRol.RolId).ToList();
                sayfaListesi = sayfaRepo.GetAll().ToList().Join(roller, e => e.Id, a => a.SayfaId, (e, a) => new { e.SayfaAdi }).Distinct().Select(e => e.SayfaAdi).ToArray();

                return sayfaListesi;
            }
            else
            {
                return sayfaListesi;
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}