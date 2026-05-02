using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.DigerSistem
{
    public static class BioklimatikSistem
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int boy, int adet, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            foreach (Profil item in profilList)
            {
                switch (item.ProfilKodu)
                {
                    case "BC-102":
                        item.KesimOlcusu = boy - 320;
                        item.KesimAdet = adet*2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-102-1":
                        item.KesimOlcusu = boy - 320;
                        item.KesimAdet = adet*2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-102-2":
                        item.KesimOlcusu = en - 320;
                        item.KesimAdet = adet*2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-103":
                        item.KesimOlcusu = boy - 62;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-103-1":
                        item.KesimOlcusu = boy - 62;
                        item.KesimAdet = adet*2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-103-2":
                        item.KesimOlcusu = en-62;
                        item.KesimAdet = adet*2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-109":
                        item.KesimOlcusu = en - 62-38;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-110":
                        item.KesimOlcusu = en - 62 - 38-1;
                        item.KesimAdet = adet;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-108":
                        item.KesimOlcusu = en;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-108-1":
                        item.KesimOlcusu = en;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-108-2":
                        item.KesimOlcusu = boy;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-106":
                        item.KesimOlcusu = boy - 62-284;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-105":
                        item.KesimOlcusu = boy - 62 - 284-128;
                        item.KesimAdet = adet * 2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-101":
                        item.KesimOlcusu = en - 360;
                        item.KesimAdet = adet * (int)((double)(boy-388)/234);
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-107":
                        item.KesimOlcusu = 2500 +250;//açılır ayak yüksekliği sabit 2500
                        item.KesimAdet = adet*2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-107-1":
                        item.KesimOlcusu = 2500 + 250;//açılır ayak yüksekliği sabit 2500
                        item.KesimAdet = adet*2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                    case "BC-107-2":
                        item.KesimOlcusu = 2500 + 250;//cephe ayak yüksekliği sabit 2500
                        item.KesimAdet = adet*2;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (int)item.BirimAgirlik, item.ProfilKodu);
                        break;
                }
                newProfilList.Add(item);
            }

            return newProfilList;
        }

        private static double profilToplamAgirlikHesaplama(int olcu, int adet, int agirlik, string pk) => olcu * adet * ((double)agirlik / 1000) / 1000;

        public static List<CamBilgileri> CamYukseklikHesapla(int boy, int en, int adet)
        {
            List<CamBilgileri> camEntityList = new List<CamBilgileri>();
            CamBilgileri camModel = new CamBilgileri();


            return camEntityList;
        }
    }
}
