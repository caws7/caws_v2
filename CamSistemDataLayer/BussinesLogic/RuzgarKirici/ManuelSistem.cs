using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamSistemDataLayer.BussinesLogic.RuzgarKirici
{
    public static class ManuelSistem
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            ProfilRepo profilRepo = new ProfilRepo();
            List<Profil> newProfilList = new List<Profil>();
            int kapaliBoy = Convert.ToInt32((double)(boy / 2) + 162);
            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "RK-101":
                        item.KesimOlcusu = en - 44 - 2;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "RK-102":
                        item.KesimOlcusu = Convert.ToInt32((boy - kapaliBoy) + 267 - 34 - 8);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "RK-103":
                        item.KesimOlcusu = en - 12 - 2;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "RK-104":
                        item.KesimOlcusu = en - 95 - 2;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "RK-104-1":
                        item.KesimOlcusu = en - 95 - 2;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "RK-104-2":
                        item.KesimOlcusu = Convert.ToInt32((boy - kapaliBoy) + 267 - 34 - 8);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "RK-105":
                        item.KesimOlcusu = en - 60;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "DP-101":
                        item.KesimOlcusu = en - 90;
                        double rk103ba = (double)profilRepo.FindBy(e => e.ProfilKodu.Equals("RK-103")).FirstOrDefault().BirimAgirlik / 1000;
                        double rk104ba = (double)profilRepo.FindBy(e => e.ProfilKodu.Equals("RK-104")).FirstOrDefault().BirimAgirlik / 1000;
                        double rk105ba = (double)profilRepo.FindBy(e => e.ProfilKodu.Equals("RK-105")).FirstOrDefault().BirimAgirlik / 1000;
                        double dp101ba = (double)item.BirimAgirlik / 1000;
                        double h19 = 12.15;
                        double g19 = (adet * (((boy - kapaliBoy) - 27.6) + 34.5) * (en - 52)) / 1000000;
                        double i19 = g19 * h19;
                        double i14 = ((boy - kapaliBoy + 267 - 34) * (adet * 2) * rk104ba) / 1000;
                        double i13 = ((en - 95) * adet * rk104ba) / 1000;
                        double i12 = ((en - 12) * adet * rk103ba) / 1000;
                        double i15 = ((en - 60) * adet * rk105ba) / 1000;
                        double i21 = i12 + i13 + i14 + i19;
                        double i22 = i21 + 2;
                        double i23 = i15;
                        double i24 = i22 - i23;
                        double h16 = dp101ba;
                        double g24 = (i24 / h16) * 1000;
                        double f16 = en - 90;
                        item.KesimAdet = Convert.ToInt32(Math.Floor(g24 / f16));
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "DP-102":
                        item.KesimOlcusu = en - 90;
                        item.KesimAdet = 1;
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
            int kapaliBoy = Convert.ToInt32((double)(boy / 2) + 162);

            camModel.Adet = adet;
            camModel.CamAdi = "KAYAR CAM";
            camModel.Genislik = en - 52;
            camModel.Yukseklik = Convert.ToInt32((double)((boy - kapaliBoy) - 27.6) + 34.5);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "SABİT CAM";
            camModel.Genislik = en - 52 - 5;
            camModel.Yukseklik = Convert.ToInt32((double)(kapaliBoy - 14.5 - 38.5));
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
