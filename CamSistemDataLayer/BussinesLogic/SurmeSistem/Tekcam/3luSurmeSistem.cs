using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.SurmeSistem.Tekcam
{
    public static class _3luSurmeSistem
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "SS-121":
                        item.ProfilAdi = "ALT KASA / SÜRME KASA";
                        item.KesimOlcusu = en - 37;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-121-1":
                        item.ProfilAdi = "ALT KASA / SÜRME KASA";
                        item.KesimOlcusu = en - 37;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-120-1":
                        item.ProfilAdi = "ÜST KASA / SÜRME KASA";
                        item.KesimOlcusu = en - 37;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-123":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = boy - 20 - (24 + 30) - 4;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-122":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME KASA";
                        item.KesimOlcusu = boy - 55 + 11 - 7 - 4;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-124":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = Convert.ToInt32((double)(en - 159.84) / 3 - 3 - 28);
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-124-1":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = Convert.ToInt32((double)(en - 159.84) / 3 - 3 - 28);
                        item.KesimAdet = adet * 4;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-126":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32((double)(25.5 + 17.32));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-126-1":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32((double)(25.5 + 17.32));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-130":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32((double)(25.5 + 17.32)) - 59;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-130-1":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32((double)(25.5 + 17.32)) - 59;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-124-2":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = Convert.ToInt32((double)(en - 159.84) / 3 - 3 - 28);
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-126-2":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ÇEKME KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32((double)(25.5 + 17.32));
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-134":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ORTA KANAT";
                        item.KesimOlcusu = Convert.ToInt32((double)(en - 159.84) / 3 - 3 - 28);
                        item.KesimAdet = (adet * 4) + (adet * 2);
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-134-1":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ORTA KANAT";
                        item.KesimOlcusu = Convert.ToInt32((double)(en - 159.84) / 3 - 3 - 28);
                        item.KesimAdet = (adet * 4) + (adet * 2);
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-134-2":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ORTA KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32((double)(25.5 + 17.32));
                        item.KesimAdet = (adet * 2) + (adet * 2);
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik);
                        break;
                    case "SS-134-3":
                        item.ProfilAdi = item.ProfilAdi + " / SÜRME ORTA KANAT";
                        item.KesimOlcusu = boy - Convert.ToInt32((double)(25.5 + 17.32)) - 59;
                        item.KesimAdet = adet * 2;
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

            camModel.Adet = adet * 2;
            camModel.CamAdi = "ÇEKME KANAT";
            camModel.Genislik = Convert.ToInt32((double)(en - 159.84) / 3) - 3;
            camModel.Yukseklik = boy - (81 + 73) - 3;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            camModel = new CamBilgileri();

            camModel.Adet = adet;
            camModel.CamAdi = "ORTA KANAT";
            camModel.Genislik = Convert.ToInt32((double)(en - 159.84) / 3) - 3;
            camModel.Yukseklik = boy - (81 + 73) - 3;
            camModel.Alanm2 = (double)camModel.Yukseklik * camModel.Adet * camModel.Genislik / 1000000;
            camEntityList.Add(camModel);

            return camEntityList;
        }
    }
}
