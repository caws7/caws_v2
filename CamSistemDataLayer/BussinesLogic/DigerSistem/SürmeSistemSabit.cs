using CamSistemDataLayer.Models;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.DigerSistem
{
    public static class SürmeSistemSabit
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            if (profilList == null) return newProfilList;

            foreach (Profil item in profilList)
            {
                switch ((item?.ProfilKodu ?? "").Trim())
                {
                    // Profil kesim şablon alanı:
                    // Aşağıdaki case bloklarını profil kodlarına göre çoğaltıp
                    // kesim ölçüsü / adet formüllerini doldurabilirsiniz.

                    case "SURME-PROFIL-1":
                        item.KesimOlcusu = en;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "SURME-PROFIL-2":
                        item.KesimOlcusu = boy;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                }

                newProfilList.Add(item);
            }

            return newProfilList;
        }

        private static double profilToplamAgirlikHesaplama(int olcu, int adet, int agirlik)
            => olcu * adet * ((double)agirlik / 1000) / 1000;

        public static List<CamBilgileri> CamYukseklikHesapla(int boy, int en, int adet)
        {
            List<CamBilgileri> camEntityList = new List<CamBilgileri>();

            // Cam hesap şablon alanı:
            // Aşağıdaki ölçü formüllerini sürme sistem profillerine göre güncelleyebilirsiniz.
            double yukseklik = boy;
            int genislik = en;

            camEntityList.Add(CreateCamBilgisi("KAYAR CAM", genislik, (int)yukseklik, adet));
            camEntityList.Add(CreateCamBilgisi("ORTA CAM", genislik, (int)yukseklik, adet));
            camEntityList.Add(CreateCamBilgisi("SABİT CAM", genislik, (int)yukseklik, adet));

            return camEntityList;
        }

        public static List<Profil> DigerMalzemeHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            // Aksesuar / diğer malzeme şablon alanı:
            // Gerekirse profilList içinden ilgili kodlara göre ayrı hesaplar burada yapılabilir.
            return profilList ?? new List<Profil>();
        }

        private static CamBilgileri CreateCamBilgisi(string camAdi, int genislik, int yukseklik, int adet)
        {
            return new CamBilgileri
            {
                Adet = adet,
                CamAdi = camAdi,
                Genislik = genislik,
                Yukseklik = yukseklik,
                Alanm2 = (double)yukseklik * adet * genislik / 1000000
            };
        }
    }
}