namespace CamSistemDataLayer.Models
{
    public partial class Kullanici
    {
        public string KullaniciAdSoyadMail
        {
            get
            {
                return KullaniciAdi + " " + KullaniciSoyadi + " - " + KullaniciMail;
            }
        }
    }
}
