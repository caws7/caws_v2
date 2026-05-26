using CamSistemDataLayer.Models;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.DigerSistem
{
    public static class SürmeSistemSabit
    {
        public static List<Profil> profilKesimOlcusuHesaplama(
            int en,
            int yukseklik,
            int kanatAdedi,
            int sistemAdedi,
            List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            if (profilList == null) return newProfilList;

            int toplamKanatAdedi = kanatAdedi * sistemAdedi;

            foreach (Profil item in profilList)
            {
                switch ((item?.ProfilKodu ?? "").Trim())
                {
                    // Profil kesim şablon alanı:
                    // Burada her profil için ölçü hangi girdiye bağlıysa ona göre hesap yazılmalı.
                    // toplam adet hesabında:
                    // - kanat bazlı parçalarda toplamKanatAdedi
                    // - sistem bazlı parçalarda sistemAdedi
                    // kullanılabilir.


                    // Örnek: sistem başına 1 kez kullanılan profil
                    case "KAR-4326":
                        item.KesimOlcusu = en- 32;
                        item.KesimAdet = sistemAdedi * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(
                            item.KesimOlcusu,
                            item.KesimAdet,
                            (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4325":
                        item.KesimOlcusu = yukseklik - 5;
                        item.KesimAdet = sistemAdedi * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(
                            item.KesimOlcusu,
                            item.KesimAdet,
                            (item.BirimAgirlik ?? 0));
                        break;

                    case "SURME-PROFIL-1":
                        item.KesimOlcusu = en;
                        item.KesimAdet = toplamKanatAdedi;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(
                            item.KesimOlcusu,
                            item.KesimAdet,
                            (item.BirimAgirlik ?? 0));
                        break;

                    case "SURME-PROFIL-2":
                        item.KesimOlcusu = yukseklik;
                        item.KesimAdet = toplamKanatAdedi;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(
                            item.KesimOlcusu,
                            item.KesimAdet,
                            (item.BirimAgirlik ?? 0));
                        break;

                }

                newProfilList.Add(item);
            }

            return newProfilList;
        }

        private static double profilToplamAgirlikHesaplama(int olcu, int adet, int agirlik)
            => olcu * adet * ((double)agirlik / 1000) / 1000;

        public static List<CamBilgileri> CamYukseklikHesapla(
            int en,
            int yukseklik,
            int kanatAdedi,
            int sistemAdedi)
        {
            List<CamBilgileri> camEntityList = new List<CamBilgileri>();

            int toplamKanatAdedi = kanatAdedi * sistemAdedi;

            // Cam hesap şablon alanı:
            // Nihai formüller profile göre burada düzenlenecek.
            double camYukseklik = yukseklik;
            int camGenislik = en;

            camEntityList.Add(CreateCamBilgisi("KAYAR CAM", camGenislik, (int)camYukseklik, toplamKanatAdedi));
            camEntityList.Add(CreateCamBilgisi("ORTA CAM", camGenislik, (int)camYukseklik, toplamKanatAdedi));
            camEntityList.Add(CreateCamBilgisi("SABİT CAM", camGenislik, (int)camYukseklik, toplamKanatAdedi));

            return camEntityList;
        }

        public static List<Profil> DigerMalzemeHesaplama(
            int en,
            int yukseklik,
            int kanatAdedi,
            int sistemAdedi,
            List<Profil> profilList)
        {
            // Aksesuar / diğer malzeme şablon alanı:
            // Örn:
            // - kanat başına kullanılan malzeme => kanatAdedi * sistemAdedi
            // - sistem başına kullanılan malzeme => sistemAdedi
            // - çevre ölçüsüne bağlı malzeme => en / yukseklik ile hesaplanır

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