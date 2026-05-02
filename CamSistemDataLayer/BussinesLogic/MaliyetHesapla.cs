using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamSistemDataLayer.BussinesLogic
{
    public static class MaliyetHesapla
    {
        /// <summary>
        /// Bir sipariş kalemi için maliyet analizi hesaplar.
        /// Sabit tablosu kullanımı: Id=2 -> Alüminyum kg fiyatı (kuruş/100), Id=3 -> İmalat bedeli (kuruş/100 per m2), Id=4 -> Sarf malzeme bedeli (kuruş/100 per m2), Id=5 -> Kar payı yüzdesi
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

            // --- 1. ALÜMİNYUM ---
            decimal aluKgFiyat = 0m;
            var aluSabit = sabitRepo.FindBy(e => e.Id == 2).FirstOrDefault();
            if (aluSabit?.SabitDeger != null)
                aluKgFiyat = Convert.ToDecimal(aluSabit.SabitDeger) / 100m;

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
            decimal imalatBirimFiyat = 0m;
            var imalatSabit = sabitRepo.FindBy(e => e.Id == 3).FirstOrDefault();
            if (imalatSabit?.SabitDeger != null)
                imalatBirimFiyat = Convert.ToDecimal(imalatSabit.SabitDeger) / 100m;

            maliyetList.Add(new Maliyet
            {
                Malzeme = "İMALAT BEDELİ",
                Birim = "m2",
                Miktar = sistemM2,
                BirimFiyat = imalatBirimFiyat,
                ToplamTutar = sistemM2 * imalatBirimFiyat
            });

            // --- 5. SARF MALZEME BEDELİ ---
            decimal sarfBirimFiyat = 0m;
            var sarfSabit = sabitRepo.FindBy(e => e.Id == 4).FirstOrDefault();
            if (sarfSabit?.SabitDeger != null)
                sarfBirimFiyat = Convert.ToDecimal(sarfSabit.SabitDeger) / 100m;

            maliyetList.Add(new Maliyet
            {
                Malzeme = "SARF MALZEME BEDELİ",
                Birim = "m2",
                Miktar = sistemM2,
                BirimFiyat = sarfBirimFiyat,
                ToplamTutar = sistemM2 * sarfBirimFiyat
            });

            // --- 6. KAR PAYI ---
            decimal karPayiOran = 0m;
            var karSabit = sabitRepo.FindBy(e => e.Id == 5).FirstOrDefault();
            if (karSabit?.SabitDeger != null)
                karPayiOran = Convert.ToDecimal(karSabit.SabitDeger);

            decimal araToplamTutar = maliyetList.Sum(m => m.ToplamTutar);
            decimal karPayiTutar = araToplamTutar * karPayiOran / 100m;

            maliyetList.Add(new Maliyet
            {
                Malzeme = "KAR PAYI",
                Birim = "%",
                Miktar = karPayiOran,
                BirimFiyat = araToplamTutar > 0m ? araToplamTutar / 100m : 0m, // araToplamın 1%'i, karPayiTutar = Miktar(%) * BirimFiyat hesabına göre
                ToplamTutar = karPayiTutar
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
