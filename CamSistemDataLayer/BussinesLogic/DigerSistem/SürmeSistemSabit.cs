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

            CamBilgileri camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "KAYAR CAM";
            camModel.Genislik = genislik;
            camModel.Yukseklik = (int)yukseklik;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "ORTA CAM";
            camModel.Genislik = genislik;
            camModel.Yukseklik = (int)yukseklik;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "SABİT CAM";
            camModel.Genislik = genislik;
            camModel.Yukseklik = (int)yukseklik;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }

        public static List<Profil> DigerMalzemeHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> digerMalzemeList = new List<Profil>();
            if (profilList == null) return digerMalzemeList;

            // Aksesuar / diğer malzeme şablon alanı:
            // Gerekirse profilList içinden ilgili kodlara göre ayrı hesaplar burada yapılabilir.
            foreach (Profil item in profilList)
            {
                digerMalzemeList.Add(item);
            }

            return digerMalzemeList;
        }
    }
}