using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.DigerSistem
{
    public static class GiyotinTemizlenebilirSistemTC
    {
        private const string Kar4880 = "KAR-4880";
        private const string Kar4880DikeySabit = "KAR-4880-2";
        private const string Kar4880DikeyHareketli = "KAR-4880-1";
        private const string Kar4880Yatay = "KAR-4880-3";

        private const string DikeySabitCamAdaptoru = "Dikey Sabit Cam Adaptörü";
        private const string DikeyHareketliCamAdaptoru = "Dikey Hareketli Cam Adaptörü";
        private const string YatayCamAdaptoru = "Yatay Cam Adaptörü";

        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();

            foreach (Profil item in profilList)
            {
                switch (ResolveProfilKodu(item))
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
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4862-1":
                        item.KesimOlcusu = (int)((((boy - 207 + 11) / 3.0) - 6) + 36);
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4863":
                        item.KesimOlcusu = (int)(boy - 151);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4861":
                        item.KesimOlcusu = (int)(boy - 151);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4869":
                        item.KesimOlcusu = (int)(boy - 173 - 6 - ((((boy - 207 + 11) / 3.0) - 6) + 31));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4873-1":
                        item.KesimOlcusu = (int)((((boy - 207 + 11) / 3.0) - 6) + 5.3);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4873-2":
                        item.KesimOlcusu = (int)(boy - 173 - ((((boy - 207 + 11) / 3) - 6) + 28 + 3) - 6);
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

                    case "KAR-4864":
                        item.KesimOlcusu = en - 195;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4868":
                        item.KesimOlcusu = (int)((((boy - 207 + 11) / 3) - 6) + 28 + 3);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4877":
                        item.KesimOlcusu = (int)(boy - 173 - 6 - ((((boy - 207 + 11) / 3.0) - 6) + 31));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4876":
                        item.KesimOlcusu = (int)((((boy - 207 + 11) / 3.0) - 6) + 31.7);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4865":
                        item.KesimOlcusu = en - 200;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4867":
                        item.KesimOlcusu = en - 195;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4882":
                        item.KesimOlcusu = en - 71;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4880-1":
                        item.KesimOlcusu = (int)((((boy - 207 + 11) / 3.0) - 6) + 36);
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4880-2":
                        item.KesimOlcusu = (int)((((boy - 207 + 11) / 3.0) - 6) + 31.7);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "KAR-4880-3":
                        item.KesimOlcusu = en - 240;
                        item.KesimAdet = adet * 6;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                }

                newProfilList.Add(item);
            }

            return newProfilList;
        }

        private static double profilToplamAgirlikHesaplama(int olcu, int adet, int agirlik)
            => olcu * adet * ((double)agirlik / 1000) / 1000;

        private static string ResolveProfilKodu(Profil item)
        {
            string profilKodu = item?.ProfilKodu?.Trim() ?? "";
            if (!profilKodu.Equals(Kar4880, StringComparison.OrdinalIgnoreCase))
                return profilKodu;

            string profilAdi = item?.ProfilAdi?.Trim() ?? "";
            if (profilAdi.Equals(DikeySabitCamAdaptoru, StringComparison.OrdinalIgnoreCase))
                return Kar4880DikeySabit;

            if (profilAdi.Equals(DikeyHareketliCamAdaptoru, StringComparison.OrdinalIgnoreCase))
                return Kar4880DikeyHareketli;

            if (profilAdi.Equals(YatayCamAdaptoru, StringComparison.OrdinalIgnoreCase))
                return Kar4880Yatay;

            return profilKodu;
        }

        public static List<CamBilgileri> CamYukseklikHesapla(int boy, int en, int adet)
        {
            List<CamBilgileri> camEntityList = new List<CamBilgileri>();
            CamBilgileri camModel = new CamBilgileri();

            camModel.Adet = adet;
            camModel.CamAdi = "KAYAR CAM";
            camModel.Genislik = en - 170;
            camModel.Yukseklik = Convert.ToInt32(((double)(boy - 207 + 11) / 3 - 16));
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "ORTA CAM";
            camModel.Genislik = en - 170;
            camModel.Yukseklik = Convert.ToInt32(((double)(boy - 207 + 11) / 3 - 16));
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "SABİT CAM";
            camModel.Genislik = en - 170;
            camModel.Yukseklik = Convert.ToInt32(((double)(boy - 207 + 11) / 3 - 16));
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}