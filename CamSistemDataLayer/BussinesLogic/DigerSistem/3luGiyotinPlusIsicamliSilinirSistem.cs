using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.DigerSistem
{
    public static class _3luGiyotinPlusIsicamliSilinirSistem
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "SP-101":
                        item.KesimOlcusu = en - 27;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SP-102":
                        item.KesimOlcusu = en - 27;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-101":
                        item.KesimOlcusu = en - 208;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-102":
                        item.KesimOlcusu = en - 208;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-143":
                        item.KesimOlcusu = en - 196;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-106-2"://orta cam
                        item.KesimOlcusu = Convert.ToInt32(((double)((boy - 131.63) / 3) - 31.227 - 3) + 32.84 + 2);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-106-4"://haraketli cam
                        item.KesimOlcusu = Convert.ToInt32(((double)((boy - 131.63) / 3) - 12.227 - 3) + 35.88 + 2);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-107":
                        item.KesimOlcusu = boy - (155 + 15);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-108":
                        item.KesimOlcusu = boy - 155 - Convert.ToInt32((((double)((boy - 131.63) / 3) - 31.227 - 3) + 48.852)) + 6 - 7 - 16 + 2 - 28 - 16 - 2 - 9 + 10;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-137":
                        item.KesimOlcusu = Convert.ToInt32((((double)((boy - 131.63) / 3) - 31.227 - 3) + 48.852)) + 6 - 7 - 16 + 2;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-140":
                        item.KesimOlcusu = boy - 155 - Convert.ToInt32((((double)((boy - 131.63) / 3) - 31.227 - 3) + 48.852)) + 6 - 7 - 16 + 2 - 28 - 16 - 2 - 9 + 10;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-110-1":
                        item.KesimOlcusu = Convert.ToInt32(((double)((boy - 131.63) / 3) - 12.227 - 3) - 25.556 + 21.406);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-110-2":
                        item.KesimOlcusu = Convert.ToInt32(((double)((boy - 131.63) / 3) - 12.227 - 3) + ((double)((boy - 131.63) / 3) - 31.227 - 3) - 0.586 - 25.929 + 21.406 - 8);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-136":
                        item.KesimOlcusu = Convert.ToInt32(((double)((boy - 131.63) / 3) - 31.227 - 3) + 29.945) + 2;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-133":
                        item.KesimOlcusu = en - 203;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-134":
                        item.KesimOlcusu = en - 196;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "G-132":
                        item.KesimOlcusu = en - 30;
                        item.KesimAdet = adet;
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
            camModel.Genislik = en - 162 - 5;
            camModel.Yukseklik = Convert.ToInt32((double)((boy - 131.63) / 3) - 12.227 - 3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "ORTA CAM";
            camModel.Genislik = en - 162 - 5;
            camModel.Yukseklik = Convert.ToInt32((double)((boy - 131.63) / 3) - 31.227 - 3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "SABİT CAM";
            camModel.Genislik = en - 162 + 5;
            camModel.Yukseklik = Convert.ToInt32((double)((boy - 131.63) / 3) - 31.227 - 3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
