using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.SurmeSistem.Isicam
{
    public static class _2arti2liSurmeSistem
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "SS-121-1":
                        item.ProfilAdi = "ALT KASA / SÜRME KASA";
                        item.KesimOlcusu = en - 37;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-120-1":
                        item.ProfilAdi = "ÜST KASA / SÜRME KASA";
                        item.KesimOlcusu = en - 37;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-127":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = boy - 21 + 11 - 4;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-123":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = boy - 20 - (24 + 30) - 4;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-122":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = boy - 55 + 11 - 7 - 4;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-124-1":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = Convert.ToInt32((double)(en - 237.76) / 4 - 28);
                        item.KesimAdet = adet * 8;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-126-1":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32(25.5 + 17.32);
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-130-1":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32(25.5 + 17.32) - 59;
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                }
                newProfilList.Add(item);
            }

            return newProfilList;
        }

        private static double profilToplamAgirlikHesaplama(int olcu, int adet, int agirlik) => olcu * adet * ((double)agirlik / 1000) / 1000;

        public static List<CamBilgileri> CamYukseklikHesapla(int boy, int en, int adet)
        {
            List<CamBilgileri> camEntityList = new List<CamBilgileri>();
            CamBilgileri camModel = new CamBilgileri();

            camModel.Adet = adet * 4;
            camModel.CamAdi = "ÇEKME KANAT";
            camModel.Genislik = Convert.ToInt32((double)(en - 237.76) / 4);
            camModel.Yukseklik = boy - (81 + 73);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
