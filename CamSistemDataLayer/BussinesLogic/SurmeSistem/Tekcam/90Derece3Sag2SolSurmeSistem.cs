using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.SurmeSistem.Tekcam
{
    public static class _90Derece3Sag2SolSurmeSistem
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int solEn, int sagEn, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "SS-121-1":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = solEn - 19;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-120-1":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = solEn - 19;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-121-2":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = sagEn - 19;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-120-2":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = sagEn - 19;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-129":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = boy - 20 - (24 + 30) - 4;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-123":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = boy - 20 - (24 + 30) - 4;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-122":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = boy - 55 + 11 - 7 - 4;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-124-1":
                        item.ProfilAdi = item.ProfilAdi + " / SOL KÖŞE - SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = Convert.ToInt32((double)(solEn - 155.3) / 2 - 3 - 28);
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-126-1":
                        item.ProfilAdi = item.ProfilAdi + " / SOL KÖŞE - SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32(25.5 + 17.32);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-130-1":
                        item.ProfilAdi = item.ProfilAdi + " / SOL KÖŞE - SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32(25.5 + 17.32) - 59;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-124-2":
                        item.ProfilAdi = item.ProfilAdi + " / SAĞ KÖŞE - SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = Convert.ToInt32((double)(sagEn - 155.55) / 3 - 3 - 28);
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-126-2":
                        item.ProfilAdi = item.ProfilAdi + " / SAĞ KÖŞE - SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32(25.5 + 17.32);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-130-2":
                        item.ProfilAdi = item.ProfilAdi + " / SAĞ KÖŞE - SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32(25.5 + 17.32) - 59;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-124-3":
                        item.ProfilAdi = item.ProfilAdi + " / SAĞ KÖŞE - SÜRME ORTA KANAT";
                        item.KesimOlcusu = Convert.ToInt32((double)(sagEn - 155.55) / 3 - 3 - 28);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-126-3":
                        item.ProfilAdi = item.ProfilAdi + " / SAĞ KÖŞE - SÜRME ORTA KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32(25.5 + 17.32);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-134-4"://sağ alt baza
                        item.ProfilAdi = item.ProfilAdi + " / SAĞ KÖŞE - SÜRME ORTA KANAT";
                        item.KesimOlcusu = Convert.ToInt32((double)(sagEn - 155.55) / 3 - 3 - 28);
                        item.KesimAdet = (adet * 4) + (adet * 2);
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-134-5"://sol alt baza
                        item.ProfilAdi = item.ProfilAdi + " / SAĞ KÖŞE - SÜRME ORTA KANAT";
                        item.KesimOlcusu = Convert.ToInt32((double)(solEn - 155.3) / 2 - 3 - 28);
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-134-2":
                        item.ProfilAdi = item.ProfilAdi + " / SAĞ KÖŞE - SÜRME ORTA KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32(25.5 + 17.32);
                        item.KesimAdet = (adet * 2) + (adet * 2) + (adet * 2);
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                    case "SS-134-3":
                        item.ProfilAdi = item.ProfilAdi + " / SAĞ KÖŞE - SÜRME ORTA KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32(25.5 + 17.32) - 59;
                        item.KesimAdet = (adet * 2) + (adet * 2);
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

            camModel.Adet = adet * 2;
            camModel.CamAdi = "SOL ÇEKME KANAT";
            camModel.Genislik = Convert.ToInt32((double)(solEn - 155.3) / 2) - 3;
            camModel.Yukseklik = boy - (81 + 73) - 3;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();

            camModel.Adet = adet * 2;
            camModel.CamAdi = "SAĞ ÇEKME KANAT";
            camModel.Genislik = Convert.ToInt32((double)(sagEn - 155.55) / 3) - 3;
            camModel.Yukseklik = boy - (81 + 73) - 3;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();

            camModel.Adet = adet * 1;
            camModel.CamAdi = "SAĞ ORTA KANAT";
            camModel.Genislik = Convert.ToInt32((double)(sagEn - 155.55) / 3) - 3;
            camModel.Yukseklik = boy - (81 + 73) - 3;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
