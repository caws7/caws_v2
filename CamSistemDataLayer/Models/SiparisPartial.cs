using CamSistemDataLayer.Repos;
using System.Collections.Generic;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class Siparis
    {
        public string MusteriTamAdi
        {
            get
            {
                MusteriRepo mRepo = new MusteriRepo();
                Musteri musteri = mRepo.FindBy(e => e.Id == MusteriId).FirstOrDefault();
                if (musteri != null)
                    return musteri.AdSoyadSirketAdi;
                else
                    return "";
            }
        }

        public string MusteriAdres
        {
            get
            {
                MusteriRepo mRepo = new MusteriRepo();
                Musteri musteri = mRepo.FindBy(e => e.Id == MusteriId).FirstOrDefault();
                if (musteri != null)
                    return musteri.Adres.AcikAdres + " " + musteri.Adres.PostaKodu + " " + musteri.Adres.Ilce + " - " + musteri.Adres.Il + " / " + musteri.Adres.Ulke;
                else
                    return "";
            }
        }

        public string RenkAdi { get; set; }

        public Renk Renk
        {
            get
            {
                RenkRepo renkRepo = new RenkRepo();
                return renkRepo.FindBy(e => e.Id == RenkId).FirstOrDefault();
            }
        }
        public string Motor { get; set; }
        public string Aksesuar { get; set; }
        public IList<string> SeciliAksesuarlar { get; set; }
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

        public string SistemTamami
        {
            get
            {
                SistemRepo sRepo = new SistemRepo();
                SistemTurRepo stRepo = new SistemTurRepo();
                AltSistemRepo asRepo = new AltSistemRepo();
                string retVal = "";

                if (SistemId != null && SistemId != -1)
                {
                    var sistem = sRepo.FindBy(e => e.Id == SistemId).FirstOrDefault();
                    if (sistem != null)
                        retVal = sistem.SistemAdi;
                }
                if (SistemTurId != -1 && SistemTurId != null)
                {
                    var sistemTur = stRepo.FindBy(e => e.Id == SistemTurId).FirstOrDefault();
                    if (sistemTur != null)
                        retVal = retVal + " / " + sistemTur.TurAdi;
                }
                if (AltSistemId != -1 && AltSistemId != null)
                {
                    var altSistem = asRepo.FindBy(e => e.Id == AltSistemId).FirstOrDefault();
                    if (altSistem != null)
                        retVal = retVal + " / " + altSistem.AltSistemAdi;
                }
                return retVal;
            }
        }

        // ------- EKLEDİK: Detaylar Listesi --------
        public List<SiparisEnBoyAdet> enBoyAdetList
        {
            get
            {
                SiparisEnBoyAdetRepo sebaRepo = new SiparisEnBoyAdetRepo();
                return sebaRepo.FindBy(e => e.SiparisId == Id).ToList();
            }
        }
        //-------------------------------------------

        public bool SevkiyatVarMi
        {
            get
            {
                SevkiyatRepo sevkiyatRepo = new SevkiyatRepo();
                Sevkiyat sevkiyat = sevkiyatRepo.FindBy(e => e.SiparisId == Id).FirstOrDefault();
                if (sevkiyat == null)
                    return false;
                else
                    return true;
            }
        }
    }
}