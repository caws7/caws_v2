using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.GiyotinSistem
{
    public static class _3luTekCamGenisSeriSilinirSistem
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
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SP-102":
                        item.KesimOlcusu = en - 27;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-128":
                        item.KesimOlcusu = en - 205;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-129":
                        item.KesimOlcusu = en - 205;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "AP-101":
                        item.KesimOlcusu = en - 205 - 1;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "AP-101-5":
                        item.KesimOlcusu = en - 205 - 1;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-103":
                        item.KesimOlcusu = en - 206;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-104":
                        item.KesimOlcusu = en - 205;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-127":
                        item.KesimOlcusu = Convert.ToInt32(((double)(boy - 225.683) / 3) + 15.056 - 3.3 + (14 + 19.88));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-127-1"://üst cam
                        item.KesimOlcusu = Convert.ToInt32(((double)(boy - 225.683) / 3) + 15.056 - 3.3 + (14 + 19.88));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-127-2"://orta cam
                        item.KesimOlcusu = Convert.ToInt32(((double)(boy - 225.683) / 3) - 11.481 - 3.3 + (35.213 + 19.88));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-127-3"://alt cam
                        item.KesimOlcusu = Convert.ToInt32(((double)(boy - 225.683) / 3) - 3.572 - 3.3 + (33.033 + 21.88));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-107":
                        item.KesimOlcusu = boy - 150;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-108":
                        item.KesimOlcusu = boy - 150;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-109":
                        item.KesimOlcusu = boy - 150;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-110":
                        item.KesimOlcusu = boy - 150;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-110-1":
                        item.KesimOlcusu = boy - 150;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-110-2":
                        item.KesimOlcusu = Convert.ToInt32(((double)(boy - 225.683) / 3 + 15.056 - 3.3) + 5.985);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-105":
                        item.KesimOlcusu = en - 205;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SB-101":
                        item.KesimOlcusu = en - 71;
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

            camModel.Adet = adet;
            camModel.CamAdi = "KAYAR CAM";
            camModel.Genislik = en - 160 - 5;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 225.683) / 3 + 15.056 - 3.3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "ORTA CAM";
            camModel.Genislik = en - 160 - 5;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 225.683) / 3 - 11.481 - 3.3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "ALT SABİT CAM";
            camModel.Genislik = en - 160;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 225.683) / 3 - 3.572 - 3.3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
