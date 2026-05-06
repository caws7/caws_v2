using CamSistemDataLayer.Repos;
using System;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class Musteri
    {
        public string AdSoyadSirketAdi
        {
            get
            {
                return MusteriAdi + " " + MusteriSoyadi + " - " + SirketAdi;
            }
        }

        public string AdSoyad
        {
            get
            {
                return MusteriAdi + " " + MusteriSoyadi;
            }
        }

        public Adres Adres
        {
            get
            {
                try
                {
                    AdresRepo adresRepo = new AdresRepo();
                    return adresRepo.FindBy(e => e.Id == AdresId).FirstOrDefault();
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("[Musteri.Adres] Hata AdresId=" + AdresId + ": " + ex.Message); return null; }
            }
        }
    }
}
