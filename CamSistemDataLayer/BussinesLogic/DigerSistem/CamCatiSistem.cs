using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamSistemDataLayer.BussinesLogic.DigerSistem
{
    public static class CamCatiSistem
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int solEn, int sagEn, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "KAR-4871":
                        item.KesimOlcusu = Convert.ToInt32((double)(solEn - 30) / 0.994);
                        item.KesimAdet = adet + 1;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "CT-202":
                        item.KesimOlcusu = Convert.ToInt32((double)(solEn - 157) / 0.994) + 45;
                        item.KesimAdet = adet + 1;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "CT-203":
                        item.KesimOlcusu = Convert.ToInt32((double)(solEn - 157) / 0.994) + 55;
                        item.KesimAdet = adet + 1;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "CT-207":
                        item.KesimOlcusu = Convert.ToInt32((double)((solEn - 157) / 0.994)) + 55;
                        item.KesimAdet = adet + 1 - 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "CT-209":
                        item.KesimOlcusu = Convert.ToInt32((double)((solEn - 157) / 0.994)) + 10 - 15;
                        item.KesimAdet = (adet + 1) - (adet + 1 - 2);
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "CT-204":
                        item.KesimOlcusu = sagEn;
                        item.KesimAdet = 1;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "CT-205":
                        item.KesimOlcusu = sagEn;
                        item.KesimAdet = 1;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "CT-107":
                        item.KesimOlcusu = boy - 4 - 47;
                        item.KesimAdet = Convert.ToInt32(((double)sagEn / 2900) + 1.4);
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "CT-208":
                        item.KesimOlcusu = Convert.ToInt32((double)(solEn - 157) / 0.994);
                        item.KesimAdet = adet + 1;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "CT-206":
                        item.KesimOlcusu = Convert.ToInt32(((double)(sagEn - ((adet + 1) * 61)) / adet) - 1.5);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                }
                newProfilList.Add(item);
            }

            return newProfilList;
        }

        private static double profilToplamAgirlikHesaplama(int olcu, int adet, int agirlik) => olcu * adet * ((double)agirlik / 1000) / 1000;

        public static List<CamBilgileri> CamYukseklikHesapla(int boy, int solEn, int sagEn, int adet)
        {
            List<CamBilgileri> camEntityList = new List<CamBilgileri>();
            CamBilgileri camModel = new CamBilgileri();
            int kapaliBoy = Convert.ToInt32((double)(boy / 2) + 162);

            camModel.Adet = adet;
            camModel.CamAdi = "CAM ÖLÇÜSÜ";
            camModel.Genislik = Convert.ToInt32((double)(solEn - 157) / 0.994) + 55;
            camModel.Yukseklik = Convert.ToInt32(((sagEn - (double)((adet + 1) * 61)) / adet) + 18 + 18);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
