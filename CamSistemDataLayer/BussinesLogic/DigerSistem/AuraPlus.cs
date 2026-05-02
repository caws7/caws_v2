using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.DigerSistem
{
    public static class AuraPlus
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "T-2401":
                        item.KesimOlcusu = en - 30;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2402":
                        item.KesimOlcusu = Convert.ToInt32(en - (double)0.5);
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2466":
                        item.KesimOlcusu = en - 214;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2467":
                        item.KesimOlcusu = en - 200;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2455":
                        item.KesimOlcusu = en - 200;
                        item.KesimAdet = adet * 3;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2468":
                        item.KesimOlcusu = Convert.ToInt32(en - (double)194.6);
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2457-1":
                        item.KesimOlcusu = Convert.ToInt32((double)(boy - 206 + 28) / 3 - 3) + 24;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2457-2":
                        item.KesimOlcusu = Convert.ToInt32(((double)(boy - 210) / 3 - 3) + 19.56);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2498":
                        item.KesimOlcusu = boy - 151;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2452":
                        item.KesimOlcusu = boy - 182 - (Convert.ToInt32((double)(boy - 210) / 3 - 3) + 36 + 4) - 6;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2494":
                        item.KesimOlcusu = Convert.ToInt32((double)(boy - 210) / 3 - 3) + 36 + 4;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2451":
                        item.KesimOlcusu = boy - 182 - (Convert.ToInt32((double)(boy - 210) / 3 - 3) + 36 + 4) - 6;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2456-1":
                        item.KesimOlcusu = Convert.ToInt32(((double)(boy - 206 + 28) / 3 - 3) + 31 - 31.7 + 3 + 6);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2456-2":
                        item.KesimOlcusu = boy - 182 - (Convert.ToInt32((double)(boy - 210) / 3 - 3) + 36 + 4) - 6;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2493":
                        item.KesimOlcusu = Convert.ToInt32((double)(boy - 210) / 3 - 3) + 19 + 3;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2400-1":
                        item.KesimOlcusu = boy - 151;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2490":
                        item.KesimOlcusu = en - 195;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2495":
                        item.KesimOlcusu = en - 45;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SB-101":
                        item.KesimOlcusu = en - 84;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "T-2400-2":
                        item.KesimOlcusu = boy - 151;
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
            camModel.Genislik = en - 159;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 206 + 28) / 3 - 3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "ORTA CAM";
            camModel.Genislik = en - 159;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 210) / 3 - 3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();
            camModel.Adet = adet;
            camModel.CamAdi = "SABİT CAM";
            camModel.Genislik = en - 159;
            camModel.Yukseklik = Convert.ToInt32((double)(boy - 210) / 3 - 3);
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
