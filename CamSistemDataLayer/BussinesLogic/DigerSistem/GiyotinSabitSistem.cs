using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.DigerSistem
{
    public static class GiyotinSabitSistem
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();

            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "KAR-4871":
                        item.KesimOlcusu = en - 30;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4872":
                        item.KesimOlcusu = en;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4874":
                        item.KesimOlcusu = en - 200;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4866":
                        item.KesimOlcusu = en - 205;
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4875":
                        item.KesimOlcusu = en - 200;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4862-1":
                        item.KesimOlcusu = (int)((((boy - 207) / 3.0) - 3) + 36);
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4862-2":
                        item.KesimOlcusu = (int)((((boy - 207) / 3.0) - 3) + 43);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4861":
                        // Not: (int) cast 151.5'i kırpar. Yuvarlamak istersen Convert.ToInt32 kullanabilirsin.
                        item.KesimOlcusu = (int)(boy - 151.5);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4863":
                        item.KesimOlcusu = (int)(boy - 151.5);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4869":
                        item.KesimOlcusu = (int)(boy - 151.5);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4878":
                        item.KesimOlcusu = (int)(boy - 151.5);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4873-1":
                        item.KesimOlcusu = (int)(boy - (((boy - 207) / 3) - 3) - 39 - 25 - 136 - 3);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4873-2":
                        item.KesimOlcusu = (int)((((boy - 207) / 3) - 3) + 4);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4860":
                        item.KesimOlcusu = en - 45;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4870":
                        item.KesimOlcusu = en - 204;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4882":
                        item.KesimOlcusu = en - 71;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                }

                // Böylece hangi case olursa olsun unutma olmaz:
                newProfilList.Add(item);
            }

            return newProfilList;
        }

        private static double profilToplamAgirlikHesaplama(int olcu, int adet, int agirlik)
            => olcu * adet * ((double)agirlik / 1000) / 1000;

        public static List<CamBilgileri> CamYukseklikHesapla(int boy, int en, int adet)
        {
            List<CamBilgileri> camEntityList = new List<CamBilgileri>();

            double yukseklik = ((boy - 207) / 3.0) - 3;
            int genislik = en - 156 - 6;

            // KAYAR CAM
            CamBilgileri camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "KAYAR CAM";
            camModel.Genislik = genislik;
            camModel.Yukseklik = (int)yukseklik;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            // ORTA CAM
            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "ORTA CAM";
            camModel.Genislik = genislik;
            camModel.Yukseklik = (int)yukseklik;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            // SABİT CAM
            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "SABİT CAM";
            camModel.Genislik = genislik;
            camModel.Yukseklik = (int)yukseklik;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}