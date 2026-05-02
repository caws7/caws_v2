using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.RuzgarKirici
{
    public static class PistonluSistem
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            ProfilRepo profilRepo = new ProfilRepo();
            List<Profil> newProfilList = new List<Profil>();
            int kapaliBoy = Convert.ToInt32((double)(boy / 2) + 155);
            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "RK-101":
                        item.KesimOlcusu = en - 46;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "RK-102":
                        item.KesimOlcusu = Convert.ToInt32(kapaliBoy - 85);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "RK-103":
                        item.KesimOlcusu = en - 13;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "RK-104-1":
                        item.ProfilAdi = "Hareketli Cam Çerçeve Pistonlu";
                        item.KesimOlcusu = en - 95;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "RK-106":
                        item.KesimOlcusu = ((boy - kapaliBoy) + 266) - 33 - 8;
                        item.KesimAdet = adet * 2;
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
            int kapaliBoy = Convert.ToInt32((double)(boy / 2) + 155);

            camModel.Adet = adet;
            camModel.CamAdi = "KAYAR CAM";
            camModel.Genislik = en - 48;
            camModel.Yukseklik = Convert.ToInt32((double)((boy - kapaliBoy) - 27.6) + 35.5);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "SABİT CAM";
            camModel.Genislik = en - 50;
            camModel.Yukseklik = Convert.ToInt32((double)(kapaliBoy - 14.5 - 25.5));
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
