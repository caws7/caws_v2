using CamSistemDataLayer.Repos;
using System;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class SiparisSevkiyatProfil
    {
        public SiparisEnBoyAdet SiparisEnBoyAdet
        {
            get
            {
                SiparisEnBoyAdetRepo siparisEnBoyAdetRepo = new SiparisEnBoyAdetRepo();
                return siparisEnBoyAdetRepo.FindBy(e => e.Id == SiparisEnBoyAdetId).FirstOrDefault();
            }
        }
        public Profil Profil
        {
            get
            {
                ProfilRepo profilRepo = new ProfilRepo();
                return profilRepo.FindBy(e => e.Id == ProfilId).FirstOrDefault();
            }
            set { }
        }

        public Kullanici Kullanici
        {
            get
            {
                KullaniciRepo kullaniciRepo = new KullaniciRepo();
                return kullaniciRepo.FindBy(e => e.Id == KullaniciId).FirstOrDefault();
            }
        }

        public Decimal ToplamKgS
        {
            get
            {
                decimal toplamkg = Convert.ToDecimal(ProfilBoy * ProfilAdet * ((decimal)Profil.BirimAgirlik / 1000));
                return toplamkg;
            }
        }

        public string ToplamMetreS
        {
            get
            {
                decimal toplamM = (decimal)(ProfilBoy * ProfilAdet);
                return toplamM.ToString();
            }
        }
    }
}
