using CamSistemDataLayer.BussinesLogic.DigerSistem;
using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamSistemDataLayer.BussinesLogic
{
    public class SiparisHesaplamalari
    {
        static SistemRepo sistemRepo;
        static AltSistemRepo asRepo;
        static SistemTurRepo tRepo;

        public static List<CamBilgileri> CamYukseklikHesapla(int sistemId, int turId, int altSistemId, int boy, int en, int solEn, int adet, int kanatAdedi = 0)
        {
            if (sistemId <= 0) return new List<CamBilgileri>();

            sistemRepo = new SistemRepo();
            asRepo = new AltSistemRepo();
            tRepo = new SistemTurRepo();

            var sistemEntity = sistemRepo.FindBy(e => e.Id == sistemId).FirstOrDefault();
            if (sistemEntity == null) return new List<CamBilgileri>();
            string sistem = sistemEntity.SistemAdi;

            string tur = "";
            string altsistem = "";

            if (turId > 0)
            {
                var turEntity = tRepo.FindBy(e => e.Id == turId).FirstOrDefault();
                if (turEntity != null) tur = turEntity.TurAdi;
            }
            if (altSistemId > 0)
            {
                var altSistemEntity = asRepo.FindBy(e => e.Id == altSistemId).FirstOrDefault();
                if (altSistemEntity != null) altsistem = altSistemEntity.AltSistemAdi;
            }

            List<CamBilgileri> camEntityList = new List<CamBilgileri>();
            CamBilgileri camModel = new CamBilgileri();

            if (sistem.Equals("Giyotin Temizlenebilir Sistem"))
            {
                if (IsTekCamSecimi(altsistem, tur))
                    camEntityList = GiyotinTemizlenebilirSistemTC.CamYukseklikHesapla(boy, en, adet);
                else
                    camEntityList = GiyotinTemizlenebilirSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sabit Sistem"))
            {
                if (IsTekCamSecimi(altsistem, tur))
                    camEntityList = GiyotinSabitSistemTC.CamYukseklikHesapla(boy, en, adet);
                else
                    camEntityList = GiyotinSabitSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (IsSurmeSistem(sistem))
            {
                camEntityList = SürmeSistemSabit.CamYukseklikHesapla(en, boy, kanatAdedi > 0 ? kanatAdedi : 1, adet);
            }
            return camEntityList;
        }

        public static List<Profil> profilHesaplama(long siparisId, int en, int solEn, int boy, int adet,
            int? sistemIdOverride = null, int? altSistemIdOverride = null, int? sistemTurIdOverride = null,
            int kanatAdedi = 0)
        {
            SiparisRepo sRepo = new SiparisRepo();
            SiparisEnBoyAdetRepo sebaRepo = new SiparisEnBoyAdetRepo();
            SistemAltSistemJoinRepo sjRepo = new SistemAltSistemJoinRepo();
            ProfilRepo pRepo = new ProfilRepo();
            SistemProfilJoinRepo spjRepo = new SistemProfilJoinRepo();
            Siparis siparis = sRepo.FindBy(e => e.Id == siparisId).FirstOrDefault();

            sistemRepo = new SistemRepo();
            asRepo = new AltSistemRepo();
            tRepo = new SistemTurRepo();

            if (siparis != null)
            {
                // Use per-row system overrides when provided
                int? effectiveSistemId = sistemIdOverride.HasValue && sistemIdOverride.Value > 0 ? sistemIdOverride : siparis.SistemId;
                int? effectiveAltSistemId = altSistemIdOverride.HasValue && altSistemIdOverride.Value > 0 ? altSistemIdOverride : siparis.AltSistemId;
                int? effectiveSistemTurId = sistemTurIdOverride.HasValue && sistemTurIdOverride.Value > 0 ? sistemTurIdOverride : siparis.SistemTurId;

                // Normalize 0 to null: form sends 0 for "not selected", treat same as null/-1
                if (effectiveAltSistemId.HasValue && effectiveAltSistemId.Value == 0) effectiveAltSistemId = null;
                if (effectiveSistemTurId.HasValue && effectiveSistemTurId.Value == 0) effectiveSistemTurId = null;

                int joinTablosuId = 0;
                if (effectiveSistemId != null)
                {
                    SistemAltSistemJoin join = null;
                    bool hasAltSistem = effectiveAltSistemId != null && effectiveAltSistemId != -1;
                    bool hasSistemTur = effectiveSistemTurId != null && effectiveSistemTurId != -1;

                    if (hasAltSistem && hasSistemTur)
                    {
                        join = sjRepo.FindBy(e =>
                            e.SistemId == effectiveSistemId
                            && e.AltSistemId == effectiveAltSistemId
                            && e.SistemTurId == effectiveSistemTurId
                        ).FirstOrDefault();
                    }

                    if (join == null && hasAltSistem)
                    {
                        join = sjRepo.FindBy(e =>
                            e.SistemId == effectiveSistemId
                            && e.AltSistemId == effectiveAltSistemId
                            && (e.SistemTurId == null || e.SistemTurId == -1)
                        ).FirstOrDefault();
                    }

                    if (join == null && hasSistemTur)
                    {
                        join = sjRepo.FindBy(e =>
                            e.SistemId == effectiveSistemId
                            && (e.AltSistemId == null || e.AltSistemId == -1)
                            && e.SistemTurId == effectiveSistemTurId
                        ).FirstOrDefault();
                    }

                    if (join == null)
                    {
                        join = sjRepo.FindBy(e =>
                            e.SistemId == effectiveSistemId
                            && (e.AltSistemId == null || e.AltSistemId == -1)
                            && (e.SistemTurId == null || e.SistemTurId == -1)
                        ).FirstOrDefault();
                    }

                    if (join != null)
                        joinTablosuId = join.Id;
                }

                //join tablosından gelen id ile sistemprofildeki joinidsiyle eşleştirip listeyi çekeceğiz ve profil tablosundaki karşılıklarını alacağız.
                List<SistemProfilJoin> joinListesi = spjRepo.FindBy(e => e.SistemAltSistemJoinId == joinTablosuId).OrderBy(e => e.Id).ToList();
                List<Profil> tumProfilListesi = pRepo.GetAll().ToList();
                List<Profil> profilListesi = tumProfilListesi.Where(i => joinListesi.Any(e => e.ProfilId.Equals(i.Id))).ToList();

                List<Profil> profilListesiTekilleme = new List<Profil>();
                foreach (var item in profilListesi)
                {
                    var profilKodu = ResolveKar4880ProfilKodu(item);
                    if (!string.Equals(item.ProfilKodu, profilKodu, StringComparison.Ordinal))
                    {
                        item.ProfilKodu = profilKodu;
                    }

                    if (profilKodu != null &&
                       (profilKodu.Contains("AP-101") || profilKodu.Contains("BC-108") || profilKodu.Contains("BC-107") || profilKodu.Contains("BC-103") || profilKodu.Contains("BC-102")
                        || profilKodu.Contains("RK-104") || profilKodu.Contains("G-106") || profilKodu.Contains("G-110") || profilKodu.Contains("G-111")
                        || profilKodu.Contains("G-112") || profilKodu.Contains("G-115") || profilKodu.Contains("G-116") || profilKodu.Contains("G-121")
                        || profilKodu.Contains("G-126") || profilKodu.Contains("G-127") || profilKodu.Contains("SS-134") || profilKodu.Contains("SS-133")
                        || profilKodu.Contains("SS-132") || profilKodu.Contains("SS-130") || profilKodu.Contains("SS-128") || profilKodu.Contains("SS-126")
                        || profilKodu.Contains("SS-124") || profilKodu.Contains("SS-121") || profilKodu.Contains("SS-118") || profilKodu.Contains("SS-117")
                        || profilKodu.Contains("SS-135") || profilKodu.Contains("SS-136") || profilKodu.Contains("SS-120") || profilKodu.Contains("T-2456")
                        || profilKodu.Contains("T-2457") || profilKodu.Contains("T-2400") || profilKodu.Contains("KAR-4873") || profilKodu.Contains("KAR-4880")
                        || profilKodu.Contains("KAR-4862")))
                    {
                        if (profilKodu.Split('-').Length > 2)
                        {
                            profilListesiTekilleme.Add(item);
                        }
                        if (joinTablosuId == 16 && profilKodu.Equals("G-126"))
                        {
                            profilListesiTekilleme.Add(item);
                        }
                        if (joinTablosuId == 13 && (profilKodu.Equals("G-110") || profilKodu.Equals("G-106")))
                        {
                            profilListesiTekilleme.Add(item);
                        }
                    }
                    else
                    {
                        profilListesiTekilleme.Add(item);
                    }

                }

                string sistem = sistemRepo.FindBy(e => e.Id == effectiveSistemId).FirstOrDefault()?.SistemAdi ?? "";
                string tur = "";
                string altSistem = "";

                // Sistem türü kontrol ve atama
                if (effectiveSistemTurId != -1)
                {
                    var turEntity = tRepo.FindBy(e => e.Id == effectiveSistemTurId).FirstOrDefault();
                    tur = turEntity != null ? turEntity.TurAdi : "";
                }
                // Alt sistem kontrol ve atama
                if (effectiveAltSistemId != -1)
                {
                    var altSistemEntity = asRepo.FindBy(e => e.Id == effectiveAltSistemId).FirstOrDefault();
                    altSistem = altSistemEntity != null ? altSistemEntity.AltSistemAdi : "";
                }

                //buraya yeni yapılan classlar çağırılcak.
                List<Profil> list = new List<Profil>();

                if (sistem.Equals("Giyotin Temizlenebilir Sistem"))
                {
                    if (IsTekCamSecimi(altSistem, tur))
                        list = GiyotinTemizlenebilirSistemTC.profilKesimOlcusuHesaplama(en, boy, adet, profilListesiTekilleme);
                    else
                        list = GiyotinTemizlenebilirSistem.profilKesimOlcusuHesaplama(en, boy, adet, profilListesiTekilleme);
                }
                else if (sistem.Equals("Giyotin Sabit Sistem"))
                {
                    if (IsTekCamSecimi(altSistem, tur))
                        list = GiyotinSabitSistemTC.profilKesimOlcusuHesaplama(en, boy, adet, profilListesiTekilleme);
                    else
                        list = GiyotinSabitSistem.profilKesimOlcusuHesaplama(en, boy, adet, profilListesiTekilleme);
                }
                else if (IsSurmeSistem(sistem))
                {
                    var surmeProfilList = SürmeSistemSabit.profilKesimOlcusuHesaplama(
                        en, boy, kanatAdedi > 0 ? kanatAdedi : 1, adet, profilListesiTekilleme);
                    list = SürmeSistemSabit.DigerMalzemeHesaplama(
                        en, boy, kanatAdedi > 0 ? kanatAdedi : 1, adet, surmeProfilList);
                }
                return list;
            }

            return null;
        }

        private static bool IsTekCamliAltSistem(string altSistemAdi)
        {
            if (string.IsNullOrWhiteSpace(altSistemAdi))
                return false;

            var normalized = new string(
                altSistemAdi
                    .Trim()
                    .Replace("ı", "i")
                    .Replace("İ", "I")
                    .ToUpperInvariant()
                    .Select(c => char.IsLetterOrDigit(c) ? c : ' ')
                    .ToArray());

            var tokens = normalized
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].StartsWith("TEKCAM"))
                    return true;

                if (tokens[i].Equals("TEK") &&
                    i + 1 < tokens.Length &&
                    tokens[i + 1].StartsWith("CAM"))
                    return true;
            }

            return false;
        }

        private static bool IsTekCamSecimi(string altSistemAdi, string sistemTurAdi)
        {
            if (IsTekCamliAltSistem(altSistemAdi))
                return true;

            if (!string.IsNullOrWhiteSpace(altSistemAdi) || string.IsNullOrWhiteSpace(sistemTurAdi))
                return false;

            return IsTekCamliAltSistem(sistemTurAdi);
        }

        private static bool IsSurmeSistem(string sistemAdi)
        {
            return string.Equals(NormalizeText(sistemAdi), "SURME SISTEM", StringComparison.Ordinal);
        }

        private static string ResolveKar4880ProfilKodu(Profil item)
        {
            var profilKodu = item?.ProfilKodu?.Trim();
            if (!string.Equals(profilKodu, "KAR-4880", StringComparison.OrdinalIgnoreCase))
                return profilKodu;

            var normalizedProfilAdi = NormalizeText(item?.ProfilAdi);
            if (normalizedProfilAdi.Contains("DIKEY") && normalizedProfilAdi.Contains("HAREKETLI") && normalizedProfilAdi.Contains("CAM") && normalizedProfilAdi.Contains("ADAPTOR"))
                return "KAR-4880-1";

            if (normalizedProfilAdi.Contains("DIKEY") && normalizedProfilAdi.Contains("SABIT") && normalizedProfilAdi.Contains("CAM") && normalizedProfilAdi.Contains("ADAPTOR"))
                return "KAR-4880-2";

            if (normalizedProfilAdi.Contains("YATAY") && normalizedProfilAdi.Contains("CAM") && normalizedProfilAdi.Contains("ADAPTOR"))
                return "KAR-4880-3";

            return profilKodu;
        }

        private static string NormalizeText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value
                .Trim()
                .ToUpperInvariant()
                .Replace('Ç', 'C')
                .Replace('Ğ', 'G')
                .Replace('İ', 'I')
                .Replace('Ö', 'O')
                .Replace('Ş', 'S')
                .Replace('Ü', 'U');
        }

    }
}
