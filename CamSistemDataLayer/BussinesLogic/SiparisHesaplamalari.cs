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

        public static List<CamBilgileri> CamYukseklikHesapla(int sistemId, int turId, int altSistemId, int boy, int en, int solEn, int adet)
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
                if (altsistem.Equals("Tek Camlı Sistem"))
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
            return camEntityList;
        }

        public static List<Profil> profilHesaplama(long siparisId, int en, int solEn, int boy, int adet,
            int? sistemIdOverride = null, int? altSistemIdOverride = null, int? sistemTurIdOverride = null)
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
                if (effectiveSistemId != null && effectiveAltSistemId != null && effectiveAltSistemId != -1 && effectiveSistemTurId != null && effectiveSistemTurId != -1)
                {
                    var join = sjRepo.FindBy(e => e.SistemId == effectiveSistemId && e.AltSistemId == effectiveAltSistemId && e.SistemTurId == effectiveSistemTurId).FirstOrDefault();
                    if (join != null) joinTablosuId = join.Id;
                }
                else if (effectiveSistemId != null && effectiveAltSistemId != null && effectiveAltSistemId != -1 && (effectiveSistemTurId == -1 || effectiveSistemTurId == null))
                {
                    var join = sjRepo.FindBy(e => e.SistemId == effectiveSistemId && e.AltSistemId == effectiveAltSistemId && e.SistemTurId == null).FirstOrDefault();
                    if (join != null) joinTablosuId = join.Id;
                }
                else if (effectiveSistemId != null
                    && (effectiveAltSistemId == -1 || effectiveAltSistemId == null)
                    && effectiveSistemTurId != null && effectiveSistemTurId != -1)
                {
                    var join = sjRepo.FindBy(e =>
                        e.SistemId == effectiveSistemId
                        && (e.AltSistemId == null || e.AltSistemId == -1)
                        && e.SistemTurId == effectiveSistemTurId
                    ).FirstOrDefault();

                    if (join != null)
                        joinTablosuId = join.Id;
                }

                else if (effectiveSistemId != null && (effectiveAltSistemId == -1 || effectiveAltSistemId == null) && (effectiveSistemTurId == -1 || effectiveSistemTurId == null))
                {
                    var join = sjRepo.FindBy(e => e.SistemId == effectiveSistemId && (e.AltSistemId == null || e.AltSistemId == -1) && (e.SistemTurId == null || e.SistemTurId == -1)).FirstOrDefault();
                    if (join != null) joinTablosuId = join.Id;
                }

                //join tablosından gelen id ile sistemprofildeki joinidsiyle eşleştirip listeyi çekeceğiz ve profil tablosundaki karşılıklarını alacağız.
                List<SistemProfilJoin> joinListesi = spjRepo.FindBy(e => e.SistemAltSistemJoinId == joinTablosuId).OrderBy(e => e.Id).ToList();
                List<Profil> tumProfilListesi = pRepo.GetAll().ToList();
                List<Profil> profilListesi = tumProfilListesi.Where(i => joinListesi.Any(e => e.ProfilId.Equals(i.Id))).ToList();

                List<Profil> profilListesiTekilleme = new List<Profil>();
                foreach (var item in profilListesi)
                {
                    if (item.ProfilKodu != null &&
                       (item.ProfilKodu.Contains("AP-101") || item.ProfilKodu.Contains("BC-108") || item.ProfilKodu.Contains("BC-107") || item.ProfilKodu.Contains("BC-103") || item.ProfilKodu.Contains("BC-102")
                        || item.ProfilKodu.Contains("RK-104") || item.ProfilKodu.Contains("G-106") || item.ProfilKodu.Contains("G-110") || item.ProfilKodu.Contains("G-111")
                        || item.ProfilKodu.Contains("G-112") || item.ProfilKodu.Contains("G-115") || item.ProfilKodu.Contains("G-116") || item.ProfilKodu.Contains("G-121")
                        || item.ProfilKodu.Contains("G-126") || item.ProfilKodu.Contains("G-127") || item.ProfilKodu.Contains("SS-134") || item.ProfilKodu.Contains("SS-133")
                        || item.ProfilKodu.Contains("SS-132") || item.ProfilKodu.Contains("SS-130") || item.ProfilKodu.Contains("SS-128") || item.ProfilKodu.Contains("SS-126")
                        || item.ProfilKodu.Contains("SS-124") || item.ProfilKodu.Contains("SS-121") || item.ProfilKodu.Contains("SS-118") || item.ProfilKodu.Contains("SS-117")
                        || item.ProfilKodu.Contains("SS-135") || item.ProfilKodu.Contains("SS-136") || item.ProfilKodu.Contains("SS-120") || item.ProfilKodu.Contains("T-2456") 
                        || item.ProfilKodu.Contains("T-2457") || item.ProfilKodu.Contains("T-2400") || item.ProfilKodu.Contains("KAR-4873") || item.ProfilKodu.Contains("KAR-4880") 
                        || item.ProfilKodu.Contains("KAR-4862")))
                    {
                        if (item.ProfilKodu.Split('-').Length > 2)
                        {
                            profilListesiTekilleme.Add(item);
                        }
                        if (joinTablosuId == 16 && item.ProfilKodu.Equals("G-126"))
                        {
                            profilListesiTekilleme.Add(item);
                        }
                        if (joinTablosuId == 13 && (item.ProfilKodu.Equals("G-110") || item.ProfilKodu.Equals("G-106")))
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
                    if (altSistem.Equals("Tek Camlı Sistem"))
                        list = GiyotinTemizlenebilirSistemTC.profilKesimOlcusuHesaplama(en, boy, adet, profilListesiTekilleme);
                    else
                        list = GiyotinTemizlenebilirSistem.profilKesimOlcusuHesaplama(en, boy, adet, profilListesiTekilleme);
                }
                if (sistem.Equals("Giyotin Sabit Sistem"))
                {
                    if (IsTekCamSecimi(altSistem, tur))
                        list = GiyotinSabitSistemTC.profilKesimOlcusuHesaplama(en, boy, adet, profilListesiTekilleme);
                    else
                        list = GiyotinSabitSistem.profilKesimOlcusuHesaplama(en, boy, adet, profilListesiTekilleme);
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

    }
}
