using CamSistemDataLayer.Models;
using System.Collections.Generic;

namespace CamSistemDataLayer.BussinesLogic.DigerSistem
{
    public static class SürmeSistemSabit
    {
        public static List<Profil> profilKesimOlcusuHesaplama(int en, int yukseklik, int kanatAdedi, int sistemAdedi, List<Profil> profilList)
        {
            List<Profil> newProfilList = new List<Profil>();
            if (profilList == null) return newProfilList;

            int toplamKanatAdedi = kanatAdedi * sistemAdedi;

            foreach (Profil item in profilList)
            {
                switch ((item?.ProfilKodu ?? "").Trim())
                {
                    // Profil kesim şablon alanı:
                    // Aşağıdaki case bloklarını profil kodlarına göre çoğaltıp
                    // kesim ölçüsü / adet formüllerini;
                    // en, yukseklik, kanatAdedi, sistemAdedi ve toplamKanatAdedi
                    // değişkenlerini kullanarak doldurabilirsiniz.

                    case "SURME-PROFIL-1":
                        item.KesimOlcusu = en;
                        item.KesimAdet = sistemAdedi;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;

                    case "SURME-PROFIL-2":
                        item.KesimOlcusu = yukseklik;
                        item.KesimAdet = toplamKanatAdedi;
                        item.ToplamAgirlik = profilToplamAgirlikHesaplama(item.KesimOlcusu, item.KesimAdet, (item.BirimAgirlik ?? 0));
                        break;
                }

                newProfilList.Add(item);
            }

            return newProfilList;
        }

        private static double profilToplamAgirlikHesaplama(int olcu, int adet, int agirlik)
            => olcu * adet * ((double)agirlik / 1000) / 1000;

        public static List<CamBilgileri> CamYukseklikHesapla(int yukseklik, int en, int kanatAdedi, int sistemAdedi)
        {
            List<CamBilgileri> camEntityList = new List<CamBilgileri>();
            int toplamKanatAdedi = kanatAdedi * sistemAdedi;

            // Cam hesap şablon alanı:
            // Aşağıdaki ölçü ve adet formüllerini sürme sistem profillerine göre;
            // en, yukseklik, kanatAdedi, sistemAdedi ve toplamKanatAdedi
            // değişkenlerini kullanarak güncelleyebilirsiniz.
            int genislik = en;

            camEntityList.Add(CreateCamBilgisi("KAYAR CAM", genislik, yukseklik, toplamKanatAdedi));
            camEntityList.Add(CreateCamBilgisi("ORTA CAM", genislik, yukseklik, sistemAdedi));
            camEntityList.Add(CreateCamBilgisi("SABİT CAM", genislik, yukseklik, sistemAdedi));

            return camEntityList;
        }

        public static List<Profil> DigerMalzemeHesaplama(int en, int yukseklik, int kanatAdedi, int sistemAdedi, List<Profil> profilList)
        {
            // Aksesuar / diğer malzeme şablon alanı:
            // Gerekirse profilList içinden ilgili kodlara göre ayrı hesaplar burada yapılabilir.
            // Örnek toplam kullanım:
            // int toplamKanatAdedi = kanatAdedi * sistemAdedi;
            return profilList ?? new List<Profil>();
        }

        private static CamBilgileri CreateCamBilgisi(string camAdi, int genislik, int yukseklik, int adet)
        {
            return new CamBilgileri
            {
                Adet = adet,
                CamAdi = camAdi,
                Genislik = genislik,
                Yukseklik = yukseklik,
                Alanm2 = (double)yukseklik * adet * genislik / 1000000
            };
        }
    }
}