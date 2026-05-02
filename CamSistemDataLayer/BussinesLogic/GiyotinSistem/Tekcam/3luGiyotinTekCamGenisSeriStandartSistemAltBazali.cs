using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.GiyotinSistem
{
    public static class _3luGiyotinTekCamGenisSeriStandartSistemAltBazali
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
                        item.KesimOlcusu = en - 26;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-128":
                        item.KesimOlcusu = en - 217;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-129":
                        item.KesimOlcusu = en - 217;
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-105":
                        item.KesimOlcusu = en - 217;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "AP-101":
                        item.KesimOlcusu = en - 217;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "AP-101-5":
                        item.KesimOlcusu = en - 217;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-127":
                        item.KesimOlcusu = Convert.ToInt32((double)(boy - 208.852) / 3 + 9.8082 - 3.3 + (14 + 19.88));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-127-1"://dikey baza üst cam
                        item.KesimOlcusu = Convert.ToInt32((double)(boy - 208.852) / 3 + 9.8082 - 3.3 + (14 + 19.88));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-127-2"://dikey baza üst cam
                        item.KesimOlcusu = Convert.ToInt32((double)(boy - 208.852) / 3 + 1.1484 - 3.3 + (19.88 + 21.88));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-127-3"://Dikey Baza Alt Cam
                        item.KesimOlcusu = Convert.ToInt32(((double)(boy - 208.852) / 3) - 10.9566 - 3.3 + (21.88 + 31.986));
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
                        item.KesimOlcusu = Convert.ToInt32(((double)(boy - 208.852) / 3 + 9.8082 - 3.3) + ((double)(boy - 208.852) / 3 + 1.1484 - 3.3) - (27.734 + 2.754));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-110-1":
                        item.KesimOlcusu = Convert.ToInt32(((double)(boy - 208.852) / 3 + 9.8082 - 3.3) + ((double)(boy - 208.852) / 3 + 1.1484 - 3.3) - (27.734 + 2.754));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-110-2":
                        item.KesimOlcusu = Convert.ToInt32(((double)(boy - 208.852) / 3 + 9.8082 - 3.3) + 22.707 - 27.734);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "G-130":
                        item.KesimOlcusu = en - 195;
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
            camModel.Genislik = en - 172 - 5;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 208.852) / 3 + 9.8082 - 3.3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "ORTA CAM";
            camModel.Genislik = en - 172 - 5;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 208.852) / 3 + 1.1484 - 3.3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "SABİT CAM";
            camModel.Genislik = en - 172;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 208.852) / 3 - 10.9566 - 3.3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
