using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.GiyotinSistem
{
    public static class _3luGiyotinIsicamliVasistasliSistem
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "SP-101":
                        item.KesimOlcusu = en - 25;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SP-102":
                        item.KesimOlcusu = en - 27;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-101":
                        item.KesimOlcusu = en - 217;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-102":
                        item.KesimOlcusu = en - 217;
                        item.KesimAdet = adet * 3;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-106":
                        item.KesimOlcusu = Convert.ToInt32((((double)(boy - 278.221) / 3) + 34.398 - 3.3) + (14 + 20));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-106-1"://dikey baza üst cam
                        item.KesimOlcusu = Convert.ToInt32((((double)(boy - 278.221) / 3) + 34.398 - 3.3) + (14 + 20));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-106-2"://orta cam
                        item.KesimOlcusu = Convert.ToInt32((((double)(boy - 278.221) / 3) + 26.88 - 3.3) + (21.98 + 20));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-107":
                        item.KesimOlcusu = boy - 150;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-108":
                        item.KesimOlcusu = boy - 150;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-109":
                        item.KesimOlcusu = Convert.ToInt32(boy - ((((double)(boy - 278.221) / 3 - 61.249 - 3.3) + (68.416 * 2)) + 147));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-110":
                        item.KesimOlcusu = Convert.ToInt32(boy - ((((double)(boy - 278.221) / 3 - 61.249 - 3.3) + (68.416 * 2)) + 147));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-110-1":
                        item.KesimOlcusu = Convert.ToInt32(boy - ((((double)(boy - 278.221) / 3 - 61.249 - 3.3) + (68.416 * 2)) + 147));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-110-2":
                        item.KesimOlcusu = Convert.ToInt32((((double)(boy - 278.221) / 3) + 34.398 - 3.3) - 5.807);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-111":
                        item.KesimOlcusu = en - 30;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-111-1":
                        item.KesimOlcusu = en - 30;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-111-2":
                        item.KesimOlcusu = Convert.ToInt32((((double)(boy - 278.221) / 3) - 61.249 - 3.3) + (68.416 * 2));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-112":
                        item.KesimOlcusu = en - 30 - 42;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-112-1":
                        item.KesimOlcusu = en - 30 - 42;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-112-2":
                        item.KesimOlcusu = Convert.ToInt32(((((double)(boy - 278.221) / 3) - 61.249 - 3.3) + (68.416 * 2)) - 39);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-113":
                        item.KesimOlcusu = en - 195;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-114":
                        item.KesimOlcusu = en - 30 - 42 - 300;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-115":
                        item.KesimOlcusu = en - 30 - 42 - 85;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-115-1":
                        item.KesimOlcusu = en - 30 - 42 - 85;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-115-2":
                        item.KesimOlcusu = Convert.ToInt32(((((double)(boy - 278.221) / 3) - 61.249 - 3.3) + (68.416 * 2)) - 39 - 85); ;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SB-101":
                        item.KesimOlcusu = en - 71;
                        item.KesimAdet = adet;
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

            camModel.Adet = adet;
            camModel.CamAdi = "KAYAR CAM";
            camModel.Genislik = en - 172 - 5;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 278.221) / 3 + 34.398 - 3.3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "ORTA CAM";
            camModel.Genislik = en - 172 - 5;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 278.221) / 3 + 26.88 - 3.3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "VASİSTAS CAM";
            camModel.Genislik = en - 30 - 42 - 95;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 278.221) / 3 - 61.249 - 3.3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
