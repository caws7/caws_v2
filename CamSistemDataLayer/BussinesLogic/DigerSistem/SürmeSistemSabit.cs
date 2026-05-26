using CamSistemDataLayer.Models;
using System;
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
            List<Profil> profilList,
            string kasaTipi = null)
        {
            List<Profil> newProfilList = new List<Profil>();
            if (profilList == null) return newProfilList;

            int toplamKanatAdedi = kanatAdedi * sistemAdedi;

            foreach (Profil item in profilList)
            {
                var profilKodu = (item?.ProfilKodu ?? "").Trim();
                if (!KasaTipineGoreProfilGosterilsinMi(profilKodu, kasaTipi))
                    continue;

                switch (profilKodu)
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

                    case "KAR-4328":
                        item.KesimOlcusu = en - 32;
                        item.KesimAdet = sistemAdedi * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(
                            item.KesimOlcusu,
                            item.KesimAdet,
                            (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4327":
                        item.KesimOlcusu = yukseklik - 5;
                        item.KesimAdet = sistemAdedi * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(
                            item.KesimOlcusu,
                            item.KesimAdet,
                            (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4324":
                        item.KesimOlcusu = yukseklik - 94;
                        item.KesimAdet = sistemAdedi * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(
                            item.KesimOlcusu,
                            item.KesimAdet,
                            (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4323":
                        item.KesimOlcusu = yukseklik - 80;
                        item.KesimAdet = sistemAdedi * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(
                            item.KesimOlcusu,
                            item.KesimAdet,
                            (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4322":
                        item.KesimOlcusu = yukseklik - 80;
                        item.KesimAdet = sistemAdedi * (( kanatAdedi * 2) - 2 );
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(
                            item.KesimOlcusu,
                            item.KesimAdet,
                            (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4320":
                        item.KesimOlcusu = ((en - 196) / kanatAdedi) + 2 ;
                        item.KesimAdet = sistemAdedi * (kanatAdedi * 2);
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

        private static bool KasaTipineGoreProfilGosterilsinMi(string profilKodu, string kasaTipi)
        {
            if (string.IsNullOrWhiteSpace(profilKodu))
                return true;

            bool kasa3eOzel = string.Equals(profilKodu, "KAR-4326", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(profilKodu, "KAR-4325", StringComparison.OrdinalIgnoreCase);
            bool kasa5eOzel = string.Equals(profilKodu, "KAR-4328", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(profilKodu, "KAR-4327", StringComparison.OrdinalIgnoreCase);

            if (!kasa3eOzel && !kasa5eOzel)
                return true;

            var normalizedKasaTipi = (kasaTipi ?? string.Empty)
                .Trim()
                .Replace("ı", "i")
                .Replace("İ", "I")
                .Replace("ü", "u")
                .Replace("Ü", "U")
                .ToLowerInvariant();

            if (normalizedKasaTipi.Contains("3 lu"))
                return kasa3eOzel;

            if (normalizedKasaTipi.Contains("5 li"))
                return kasa5eOzel;

            return true;
        }

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
            double camYukseklik = yukseklik - 155;
            int camGenislik = ((en - 196) / kanatAdedi);

            camEntityList.Add(CreateCamBilgisi("CAM", camGenislik, (int)camYukseklik, toplamKanatAdedi));

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