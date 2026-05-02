using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class DisSiparis
    {
        public Musteri Musteri
        {
            get
            {
                MusteriRepo mRepo = new MusteriRepo();
                return mRepo.FindBy(e => e.Id == MusteriId).FirstOrDefault();                
            }
        }

        public Tedarikci Tedarikci
        {
            get
            {
                TedarikRepo tedarikciRepo = new TedarikRepo();
                return tedarikciRepo.FindBy(e => e.Id == TedarikciId).FirstOrDefault();
            }
        }

        public string KullaniciTamAdi
        {
            get
            {
                KullaniciRepo kRepo = new KullaniciRepo();
                Kullanici kullanici = kRepo.FindBy(e => e.Id == OnayIptalKullaniciId).FirstOrDefault();
                if (kullanici == null)
                {
                    return "";
                }
                else
                {
                    return kullanici.KullaniciAdi + " " + kullanici.KullaniciSoyadi;
                }
            }
        }        
    }
}
