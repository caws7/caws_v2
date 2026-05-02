using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.DigerSistem
{
    public static class WintentSistem
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "WT-101":
                        item.ProfilAdi = item.ProfilAdi;
                        item.KesimOlcusu = en - 44;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "WT-102":
                        item.ProfilAdi = item.ProfilAdi;
                        item.KesimOlcusu = en - 54;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SB-102":
                        item.ProfilAdi = item.ProfilAdi;
                        item.KesimOlcusu = en - 86;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
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
            camModel.CamAdi = "KUMAŞ";
            camModel.Genislik = en - 99;
            camModel.Yukseklik = (1 * 2 * 1000) + 150;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
