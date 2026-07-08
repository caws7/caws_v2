using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamSistemDataLayer.BussinesLogic
{
    public static class MaliyetHesapla
    {
        private static Sabitler FindSabit(List<Sabitler> tumSabitler, int legacyId, string aciklama)
        {
            var sabit = tumSabitler.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Aciklama) &&
                                                        x.Aciklama.Trim().Equals(aciklama, StringComparison.OrdinalIgnoreCase));
            if (sabit != null) return sabit;
            return tumSabitler.FirstOrDefault(x => x.Id == legacyId);
        }

        private static decimal GetSabitDeger(List<Sabitler> tumSabitler, int legacyId, string aciklama, bool divideBy100)
        {
            var sabit = FindSabit(tumSabitler, legacyId, aciklama);
            if (sabit?.SabitDeger == null) return 0m;

            var deger = Convert.ToDecimal(sabit.SabitDeger.Value);
            return divideBy100 ? (deger / 100m) : deger;
        }

        /// <summary>
        /// Bir sipariş kalemi için maliyet analizi hesaplar.
        /// Sabitler önce açıklamaya göre aranır, bulunamazsa eski Id karşılıklarına düşülür.
        /// Legacy eşlemeler: 2=Alüminyum, 3=İmalat, 4=Sarf, 8=Cam, 9=Aksesuar Seti, 10=Kar Oranı (sabit adı "KAR PAYI BİRİM FİYAT"). Not: 5 (KAR PAYI ORANI) artık kullanılmıyor.
        /// </summary>
        public static MaliyetToplam MaliyetHesaplama(
            List<Aksesuar> aksesuarEntities,
            ProfilDetayBilgileri camModel,
            string camKombinasyon,
            SiparisEnBoyAdet item,
            long siparisId)
        {
            var sabitRepo = new SabitRepo();
            var camKombinasyonRepo = new CamKombinasyonRepo();
            var maliyetList = new List<Maliyet>();
            var tumSabitler = sabitRepo.GetAll().ToList();

            // --- 1. ALÜMİNYUM ---
            decimal aluKgFiyat = GetSabitDeger(tumSabitler, 2, "ALÜMİNYUM BİRİM FİYAT", true);

            decimal aluMiktar = (decimal)(camModel?.ToplamPresKG ?? 0);
            maliyetList.Add(new Maliyet
            {
                Malzeme = "ALÜMİNYUM",
                Birim = "KG",
                Miktar = aluMiktar,
                BirimFiyat = aluKgFiyat,
                ToplamTutar = aluMiktar * aluKgFiyat
            });

            // --- 2. CAM ---
            decimal camBirimFiyat = 0m;
            string camBirim = "m2";
            decimal camMiktar = (decimal)(camModel?.ToplamAlan ?? 0);

            if (!string.IsNullOrWhiteSpace(camKombinasyon))
            {
                var kombRecord = camKombinasyonRepo
                    .FindBy(e => e.Kombinasyon == camKombinasyon)
                    .FirstOrDefault();
                if (kombRecord != null)
                {
                    camBirimFiyat = kombRecord.BirimFiyat ?? 0m;
                    if (!string.IsNullOrWhiteSpace(kombRecord.Birim))
                        camBirim = kombRecord.Birim;
                }
            }

            var camSabitFiyat = GetSabitDeger(tumSabitler, 8, "CAM BİRİM FİYAT", true);
            if (camSabitFiyat > 0m)
                camBirimFiyat = camSabitFiyat;

            maliyetList.Add(new Maliyet
            {
                Malzeme = "CAM",
                Birim = camBirim,
                Miktar = camMiktar,
                BirimFiyat = camBirimFiyat,
                ToplamTutar = camMiktar * camBirimFiyat
            });

            // --- 3. AKSESUAR SETİ ---
            int girilenAdet = item?.GirilenAdet ?? 1;
            if (girilenAdet < 1) girilenAdet = 1;

            decimal aksesuarToplamTutar = 0m;
            if (aksesuarEntities != null && aksesuarEntities.Count > 0)
            {
                foreach (var aks in aksesuarEntities)
                    aksesuarToplamTutar += (aks.BirimFiyat ?? 0m) * girilenAdet;
            }

            var aksesuarSetiSabitFiyat = GetSabitDeger(tumSabitler, 9, "AKSESUAR SETİ BİRİM FİYAT", true);
            if (aksesuarSetiSabitFiyat > 0m)
                aksesuarToplamTutar = aksesuarSetiSabitFiyat * girilenAdet;

            maliyetList.Add(new Maliyet
            {
                Malzeme = "AKSESUAR SETİ",
                Birim = "ADET",
                Miktar = girilenAdet,
                BirimFiyat = aksesuarToplamTutar / girilenAdet,
                ToplamTutar = aksesuarToplamTutar
            });

            // --- Sistem m2 alanı (İmalat ve Sarf için) ---
            decimal sistemM2 = 0m;
            if ((item?.GirilenEn ?? 0) > 0 && (item?.GirilenBoy ?? 0) > 0)
            {
                int enToplam = (item.GirilenEn ?? 0) + (item.GirilenSolEn ?? 0);
                sistemM2 = ((decimal)enToplam * (decimal)(item.GirilenBoy ?? 0)) / 1_000_000m;
            }
            if (sistemM2 == 0m)
                sistemM2 = (decimal)(camModel?.ToplamAlan ?? 0);

            // --- 4. İMALAT BEDELİ ---
            decimal imalatBirimFiyat = GetSabitDeger(tumSabitler, 3, "İMALAT BEDELİ", true);

            maliyetList.Add(new Maliyet
            {
                Malzeme = "İMALAT BEDELİ",
                Birim = "m2",
                Miktar = sistemM2,
                BirimFiyat = imalatBirimFiyat,
                ToplamTutar = sistemM2 * imalatBirimFiyat
            });

            // --- 5. SARF MALZEME BEDELİ ---
            decimal sarfBirimFiyat = GetSabitDeger(tumSabitler, 4, "SARF MALZEME BEDELİ", true);

            maliyetList.Add(new Maliyet
            {
                Malzeme = "SARF MALZEME BEDELİ",
                Birim = "m2",
                Miktar = sistemM2,
                BirimFiyat = sarfBirimFiyat,
                ToplamTutar = sistemM2 * sarfBirimFiyat
            });

            // --- 6. KAR ORANI ---
            // Kâr oranı, sabit tanımlamalardaki "KAR PAYI BİRİM FİYAT" (Id 10) değerinden
            // okunur ve yüzde (%) olarak uygulanır.
            decimal karOrani = GetSabitDeger(tumSabitler, 10, "KAR PAYI BİRİM FİYAT", false);

            decimal araToplamTutar = maliyetList.Sum(m => m.ToplamTutar);
            decimal karTutar = araToplamTutar * karOrani / 100m;
            decimal karBirimFiyat = araToplamTutar > 0m ? araToplamTutar / 100m : 0m;

            maliyetList.Add(new Maliyet
            {
                Malzeme = "KAR ORANI",
                Birim = "%",
                Miktar = karOrani,
                BirimFiyat = karBirimFiyat,
                ToplamTutar = karTutar
            });

            decimal toplamMaliyet = maliyetList.Sum(m => m.ToplamTutar);

            return new MaliyetToplam
            {
                MaliyetList = maliyetList,
                ToplamMaliyet = toplamMaliyet,
                M2 = sistemM2 > 0m ? toplamMaliyet / sistemM2 : 0m,
                Teklif = toplamMaliyet
            };
        }
    }
}
