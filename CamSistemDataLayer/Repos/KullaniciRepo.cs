using CamSistemDataLayer.Models;
using CamSistemDataLayer.Models.GenericRepo;
using System.ComponentModel;
using System.Linq;

namespace CamSistemDataLayer.Repos
{
    [DataObject(true)]
    public class KullaniciRepo : GenericRepository<Kullanici>
    {
        public IQueryable<Kullanici> GetTumKullanicilar()
        {
            return base.GetAll();
        }

        public bool KullaniciVarMi(string KullaniciAdi, string Password)
        {
            return FindBy(e => e.KullaniciMail == KullaniciAdi && e.Sifre == Password && e.AktifMi == true).SingleOrDefault() != null;
        }
    }
}
