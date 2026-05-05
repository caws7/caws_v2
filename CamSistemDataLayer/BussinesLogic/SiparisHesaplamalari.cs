using CamSistemDataLayer.BussinesLogic.DigerSistem;
using CamSistemDataLayer.BussinesLogic.GiyotinSistem;
using CamSistemDataLayer.BussinesLogic.GiyotinSistem.Isicam;
using CamSistemDataLayer.BussinesLogic.GiyotinSistem.Tekcam;
using CamSistemDataLayer.BussinesLogic.RuzgarKirici;
using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
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
            sistemRepo = new SistemRepo();
            asRepo = new AltSistemRepo();
            tRepo = new SistemTurRepo();

            string sistem = sistemRepo.FindBy(e => e.Id == sistemId).FirstOrDefault().SistemAdi;

            string tur = "";
            string altsistem = "";

            if (turId != -1)
                tur = tRepo.FindBy(e => e.Id == turId).FirstOrDefault().TurAdi;
            if (altSistemId != -1)
                altsistem = asRepo.FindBy(e => e.Id == altSistemId).FirstOrDefault().AltSistemAdi;

            List<CamBilgileri> camEntityList = new List<CamBilgileri>();
            CamBilgileri camModel = new CamBilgileri();

            if (sistem.Equals("Giyotin Temizlenebilir Sistem"))
            {
                camEntityList = GiyotinTemizlenebilirSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sabit Sistem"))
            {
                camEntityList = GiyotinSabitSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("3'lü Standart Sistem"))
            {
                camEntityList = _3luGiyotinIsicamStandartSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("3'lü Klasik"))
            {
                camEntityList = _3luGiyotinIsicamliKlasikSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("2'li Klasik"))
            {
                camEntityList = _2liGiyotinIsicamliKlasikSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("3'lü Alt Bazalı Standart Sistem"))
            {
                camEntityList = _3luGiyotinIsicamliStandartSistemAltBazali.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("3'lü Vasistaslı Sistem"))
            {
                camEntityList = _3luGiyotinIsicamliVasistasliSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("3'lü Vasistaslı Silinebilir Sistem"))
            {
                camEntityList = _3luGiyotinIsicamliVasistasliSilinirSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("3'lü Alt Cam Sabit Vasistaslı Silinir Sistem"))
            {
                camEntityList = _3luGiyotinIsicamliVasistasliSilinirSistemAltCamSabit.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("3'lü Silinebilir Sistem"))
            {
                camEntityList = _3luGiyotinIsicamliSilinirSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("3'lü Alt Bazalı Silinir Sistem"))
            {
                camEntityList = _3luGiyotinIsicamliSilinirSistemAltBazali.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("2'li Standart Sistem"))
            {
                camEntityList = _2liGiyotinIsicamliStandartSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("2'li Silinebilir Sistem"))
            {
                camEntityList = _2liGiyotinIsicamliSilinirSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("2'li Alt Bazalı Standart Sistem"))
            {
                camEntityList = _2liGiyotinIsicamliStandartSistemAltBazali.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("2'li Vasistaslı Sistem"))
            {
                camEntityList = _2liGiyotinIsicamliVasistasliSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Geniş Sistem") && altsistem.Equals("2'li Vasistaslı Sistem"))
            {
                camEntityList = _2liGiyotinTekCamGenisSeriVasistasliSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Sistem") && altsistem.Equals("2'li Standart Sistem"))
            {
                camEntityList = _2liGiyotinTekCamStandartSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Sistem") && altsistem.Equals("2'li Alt Bazalı Standart Sistem"))
            {
                camEntityList = _2liGiyotinTekCamStandartSistemAltBazali.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Geniş Sistem") && altsistem.Equals("2'li Alt Bazalı Standart Sistem"))
            {
                camEntityList = _2liGiyotinTekCamGenisSeriStandartSistemAltBazali.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Geniş Sistem") && altsistem.Equals("3'lü Alt Bazalı Standart Sistem"))
            {
                camEntityList = _3luGiyotinTekCamGenisSeriStandartSistemAltBazali.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Geniş Sistem") && altsistem.Equals("2'li Standart Sistem"))
            {
                camEntityList = _2liGiyotinTekCamGenisSeriStandartSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Geniş Sistem") && altsistem.Equals("3'lü Standart Sistem"))
            {
                camEntityList = _3luTekCamGenisSeriStandartSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Geniş Sistem") && altsistem.Equals("3'lü Klasik"))
            {
                camEntityList = _3luTekCamGenisSeriKlasikSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Geniş Sistem") && altsistem.Equals("3'lü Vasistaslı Silinebilir Sistem"))
            {
                camEntityList = _3luGiyotinTekCamGenisSeriVasistasliSilinirSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Sistem") && altsistem.Equals("3'lü Standart Sistem"))
            {
                camEntityList = _3luGiyotinTekCamStandartSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Sistem") && altsistem.Equals("3'lü Alt Bazalı Standart Sistem"))
            {
                camEntityList = _3luGiyotinTekCamStandartSistemAltBazali.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Sistem") && altsistem.Equals("3'lü Alt Sabit Değişken Ölçülü Standart Sistem"))
            {
                camEntityList = _3luGiyotinTekCamStandartSistemAltBazali.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Sistem") && altsistem.Equals("4'lü Standart Sistem"))
            {
                camEntityList = _4luGiyotinTekCamStandartSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("4'lü Standart Sistem"))
            {
                camEntityList = _4luGiyotinIsicamliStandartSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Bioklimatik Sistem"))//cam yok ?
            {
                camEntityList = BioklimatikSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Plus Sistem") && tur.Equals("Isı Camlı Sistem") && altsistem.Equals("3'lü Silinebilir Sistem"))
            {
                camEntityList = _3luGiyotinPlusIsicamliSilinirSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Plus Sistem") && tur.Equals("Tek Camlı Sistem") && altsistem.Equals("3'lü Silinebilir Sistem"))
            {
                camEntityList = _3luGiyotinPlusTekcamSilinirSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Rüzgar Kırıcı Sistem") && altsistem.Equals("Manuel Sistem"))
            {
                camEntityList = ManuelSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Rüzgar Kırıcı Sistem") && altsistem.Equals("Pistonlu Sistem"))
            {
                camEntityList = PistonluSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Rüzgar Kırıcı Sistem") && tur.Equals("Profilsiz Sistem") && altsistem.Equals("Pistonlu Sistem"))
            {
                camEntityList = ProfilsizPistonluSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Rüzgar Kırıcı Sistem") && altsistem.Equals("Motorlu Sistem"))
            {
                camEntityList = MotorluSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Zip Perde Sistem"))
            {
                camEntityList = ZipPerdeSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Wintent"))
            {
                camEntityList = WintentSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Sürme Sistem"))
            {
                camEntityList = surmeSistemCam(tur, altsistem, boy, en, solEn, adet);
            }
            else if (sistem.Equals("Sürme Sistem Eşiksiz"))
            {
                camEntityList = surmeSistemEsiksizCam(tur, altsistem, boy, en, solEn, adet);
            }
            else if (sistem.Equals("Cam Çatı Sistem"))
            {
                camEntityList = CamCatiSistem.CamYukseklikHesapla(boy, solEn, en, adet);
            }
            else if (sistem.Equals("Giyotin Sistem") && tur.Equals("Tek Camlı Sistem") && altsistem.Equals("Griyajlı"))
            {
                camEntityList = GioXTekCamGriyajli.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Giyotin Silinir Sistem"))
            {
                camEntityList = GiyotinSilinirSistem.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Aura Plus"))
            {
                camEntityList = AuraPlus.CamYukseklikHesapla(boy, en, adet);
            }
            else if (sistem.Equals("Aura Silinebilir Sistem"))
            {
                camEntityList = AuraSilinebilirSistem.CamYukseklikHesapla(boy, en, adet);
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

                int joinTablosuId = 0;
                if (effectiveSistemId != null && effectiveAltSistemId != null && effectiveAltSistemId != -1 && effectiveSistemTurId != null && effectiveSistemTurId != -1)
                {
                    joinTablosuId = sjRepo.FindBy(e => e.SistemId == effectiveSistemId && e.AltSistemId == effectiveAltSistemId && e.SistemTurId == effectiveSistemTurId).FirstOrDefault().Id;
                }
                else if (effectiveSistemId != null && effectiveAltSistemId != null && effectiveAltSistemId != -1 && (effectiveSistemTurId == -1 || effectiveSistemTurId == null))
                {
                    joinTablosuId = sjRepo.FindBy(e => e.SistemId == effectiveSistemId && e.AltSistemId == effectiveAltSistemId && e.SistemTurId == null).FirstOrDefault().Id;
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
                    joinTablosuId = sjRepo.FindBy(e => e.SistemId == effectiveSistemId && (e.AltSistemId == null || e.AltSistemId == -1) && (e.SistemTurId == null || e.SistemTurId == -1)).FirstOrDefault().Id;
                }

                //join tablosından gelen id ile sistemprofildeki joinidsiyle eşleştirip listeyi çekeceğiz ve profil tablosundaki karşılıklarını alacağız.
                List<SistemProfilJoin> joinListesi = spjRepo.FindBy(e => e.SistemAltSistemJoinId == joinTablosuId).OrderBy(e => e.Id).ToList();
                List<Profil> tumProfilListesi = pRepo.GetAll().ToList();
                List<Profil> profilListesi = tumProfilListesi.Where(i => joinListesi.Any(e => e.ProfilId.Equals(i.Id))).ToList();

                List<Profil> profilListesiTekilleme = new List<Profil>();
                foreach (var item in profilListesi)
                {
                    if (item.ProfilKodu.Contains("AP-101") || item.ProfilKodu.Contains("BC-108") || item.ProfilKodu.Contains("BC-107") || item.ProfilKodu.Contains("BC-103") || item.ProfilKodu.Contains("BC-102")
                        || item.ProfilKodu.Contains("RK-104") || item.ProfilKodu.Contains("G-106") || item.ProfilKodu.Contains("G-110") || item.ProfilKodu.Contains("G-111")
                        || item.ProfilKodu.Contains("G-112") || item.ProfilKodu.Contains("G-115") || item.ProfilKodu.Contains("G-116") || item.ProfilKodu.Contains("G-121")
                        || item.ProfilKodu.Contains("G-126") || item.ProfilKodu.Contains("G-127") || item.ProfilKodu.Contains("SS-134") || item.ProfilKodu.Contains("SS-133")
                        || item.ProfilKodu.Contains("SS-132") || item.ProfilKodu.Contains("SS-130") || item.ProfilKodu.Contains("SS-128") || item.ProfilKodu.Contains("SS-126")
                        || item.ProfilKodu.Contains("SS-124") || item.ProfilKodu.Contains("SS-121") || item.ProfilKodu.Contains("SS-118") || item.ProfilKodu.Contains("SS-117")
                        || item.ProfilKodu.Contains("SS-135") || item.ProfilKodu.Contains("SS-136") || item.ProfilKodu.Contains("SS-120") || item.ProfilKodu.Contains("T-2456") 
                        || item.ProfilKodu.Contains("T-2457") || item.ProfilKodu.Contains("T-2400") || item.ProfilKodu.Contains("KAR-4873") || item.ProfilKodu.Contains("KAR-4862"))
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
                    list = GiyotinTemizlenebilirSistem.profilKesimOlcusuHesaplama(en, boy, adet, profilListesiTekilleme);
                }
                if (sistem.Equals("Giyotin Sabit Sistem"))
                {
                    list = GiyotinSabitSistem.profilKesimOlcusuHesaplama(en, boy, adet, profilListesiTekilleme);
                }
                return list;
            }

            return null;
        }

        static List<CamBilgileri> surmeSistemCam(string turAdi, string altSistemAdi, int boy, int en, int solEn, int adet)
        {
            switch (turAdi + altSistemAdi)
            {
                case ("Isı Camlı Sistem" + "2'li Sürme Sistem"): return SurmeSistem.Isicam._2liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "3'lü Sürme Sistem"): return SurmeSistem.Isicam._3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "4'lü Sürme Sistem"): return SurmeSistem.Isicam._4luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "5'li Sürme Sistem"): return SurmeSistem.Isicam._5liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "2 + 2'li Sürme Sistem"): return SurmeSistem.Isicam._2arti2liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "3 + 2'li Sürme Sistem"): return SurmeSistem.Isicam._3arti2liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "3 + 3'lü Sürme Sistem"): return SurmeSistem.Isicam._3arti3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "4 + 3'lü Sürme Sistem"): return SurmeSistem.Isicam._4arti3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "4 + 4'lü Sürme Sistem"): return SurmeSistem.Isicam._4arti4luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "5 + 5'li Sürme Sistem"): return SurmeSistem.Isicam._5arti5liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "90 Derece 3-Sağ 2-Sol Sürme Sistem"): return SurmeSistem.Isicam._90Derece3Sag2SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Isı Camlı Sistem" + "90 Derece 3-Sağ 3-Sol Sürme Sistem"): return SurmeSistem.Isicam._90Derece3Sag3SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Isı Camlı Sistem" + "90 Derece 4-Sağ 3-Sol Sürme Sistem"): return SurmeSistem.Isicam._90Derece4Sag3SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Isı Camlı Sistem" + "90 Derece 4-Sağ 4-Sol Sürme Sistem"): return SurmeSistem.Isicam._90Derece4Sag4SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);

                case ("Tek Camlı Sistem" + "2'li Sürme Sistem"): return SurmeSistem.Tekcam._2liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "3'lü Sürme Sistem"): return SurmeSistem.Tekcam._3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "4'lü Sürme Sistem"): return SurmeSistem.Tekcam._4luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "5'li Sürme Sistem"): return SurmeSistem.Tekcam._5liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "3 + 2'li Sürme Sistem"): return SurmeSistem.Tekcam._3arti2liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "3 + 3'lü Sürme Sistem"): return SurmeSistem.Tekcam._3arti3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "4 + 3'lü Sürme Sistem"): return SurmeSistem.Tekcam._4arti3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "4 + 4'lü Sürme Sistem"): return SurmeSistem.Tekcam._4arti4luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "5 + 5'li Sürme Sistem"): return SurmeSistem.Tekcam._5arti5liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "90 Derece 3-Sağ 2-Sol Sürme Sistem"): return SurmeSistem.Tekcam._90Derece3Sag2SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Tek Camlı Sistem" + "90 Derece 3-Sağ 3-Sol Sürme Sistem"): return SurmeSistem.Tekcam._90Derece3Sag3SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Tek Camlı Sistem" + "90 Derece 4-Sağ 3-Sol Sürme Sistem"): return SurmeSistem.Tekcam._90Derece4Sag3SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Tek Camlı Sistem" + "90 Derece 4-Sağ 4-Sol Sürme Sistem"): return SurmeSistem.Tekcam._90Derece4Sag4SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                default:
                    return null;
            }

        }

        static List<CamBilgileri> surmeSistemEsiksizCam(string turAdi, string altSistemAdi, int boy, int en, int solEn, int adet)
        {
            switch (turAdi + altSistemAdi)
            {
                case ("Isı Camlı Sistem" + "2'li Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._2liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "4'lü Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._4luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "2 + 2'li Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._2arti2liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "3 + 2'li Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._3arti2liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "3 + 3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._3arti3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "4 + 3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._4arti3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "4 + 4'lü Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._4arti4luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Isı Camlı Sistem" + "90 Derece 3-Sağ 2-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._90Derece3Sag2SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Isı Camlı Sistem" + "90 Derece 3-Sağ 3-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._90Derece3Sag3SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Isı Camlı Sistem" + "90 Derece 4-Sağ 3-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._90Derece4Sag3SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Isı Camlı Sistem" + "90 Derece 4-Sağ 4-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._90Derece4Sag4SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);

                case ("Tek Camlı Sistem" + "2'li Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._2liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "4'lü Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._4luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "3 + 2'li Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._3arti2liSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "3 + 3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._3arti3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "4 + 3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._4arti3luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "4 + 4'lü Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._4arti4luSurmeSistem.CamYukseklikHesapla(boy, en, adet);
                case ("Tek Camlı Sistem" + "90 Derece 3-Sağ 2-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._90Derece3Sag2SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Tek Camlı Sistem" + "90 Derece 3-Sağ 3-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._90Derece3Sag3SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Tek Camlı Sistem" + "90 Derece 4-Sağ 3-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._90Derece4Sag3SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                case ("Tek Camlı Sistem" + "90 Derece 4-Sağ 4-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._90Derece4Sag4SolSurmeSistem.CamYukseklikHesapla(boy, solEn, en, adet);
                default:
                    return null;
            }

        }

        static List<Profil> surmeSistemProfil(string turAdi, string altSistemAdi, int en, int solEn, int boy, int adet, List<Profil> profils)
        {
            switch (turAdi + altSistemAdi)
            {
                case ("Isı Camlı Sistem" + "2'li Sürme Sistem"): return SurmeSistem.Isicam._2liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "3'lü Sürme Sistem"): return SurmeSistem.Isicam._3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "4'lü Sürme Sistem"): return SurmeSistem.Isicam._4luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "5'li Sürme Sistem"): return SurmeSistem.Isicam._5liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "2 + 2'li Sürme Sistem"): return SurmeSistem.Isicam._2arti2liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "3 + 2'li Sürme Sistem"): return SurmeSistem.Isicam._3arti2liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "3 + 3'lü Sürme Sistem"): return SurmeSistem.Isicam._3arti3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "4 + 3'lü Sürme Sistem"): return SurmeSistem.Isicam._4arti3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "4 + 4'lü Sürme Sistem"): return SurmeSistem.Isicam._4arti4luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "5 + 5'li Sürme Sistem"): return SurmeSistem.Isicam._5arti5liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "90 Derece 3-Sağ 2-Sol Sürme Sistem"): return SurmeSistem.Isicam._90Derece3Sag2SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "90 Derece 3-Sağ 3-Sol Sürme Sistem"): return SurmeSistem.Isicam._90Derece3Sag3SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "90 Derece 4-Sağ 3-Sol Sürme Sistem"): return SurmeSistem.Isicam._90Derece4Sag3SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "90 Derece 4-Sağ 4-Sol Sürme Sistem"): return SurmeSistem.Isicam._90Derece4Sag4SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);

                case ("Tek Camlı Sistem" + "2'li Sürme Sistem"): return SurmeSistem.Tekcam._2liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "3'lü Sürme Sistem"): return SurmeSistem.Tekcam._3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "4'lü Sürme Sistem"): return SurmeSistem.Tekcam._4luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "5'li Sürme Sistem"): return SurmeSistem.Tekcam._5liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "3 + 2'li Sürme Sistem"): return SurmeSistem.Tekcam._3arti2liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "3 + 3'lü Sürme Sistem"): return SurmeSistem.Tekcam._3arti3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "4 + 3'lü Sürme Sistem"): return SurmeSistem.Tekcam._4arti3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "4 + 4'lü Sürme Sistem"): return SurmeSistem.Tekcam._4arti4luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "5 + 5'li Sürme Sistem"): return SurmeSistem.Tekcam._5arti5liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "90 Derece 3-Sağ 2-Sol Sürme Sistem"): return SurmeSistem.Tekcam._90Derece3Sag2SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "90 Derece 3-Sağ 3-Sol Sürme Sistem"): return SurmeSistem.Tekcam._90Derece3Sag3SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "90 Derece 4-Sağ 3-Sol Sürme Sistem"): return SurmeSistem.Tekcam._90Derece4Sag3SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "90 Derece 4-Sağ 4-Sol Sürme Sistem"): return SurmeSistem.Tekcam._90Derece4Sag4SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                default:
                    return null;
            }

        }

        static List<Profil> surmeSistemEsiksizProfil(string turAdi, string altSistemAdi, int en, int solEn, int boy, int adet, List<Profil> profils)
        {
            switch (turAdi + altSistemAdi)
            {
                case ("Isı Camlı Sistem" + "2'li Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._2liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "4'lü Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._4luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "2 + 2'li Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._2arti2liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "3 + 2'li Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._3arti2liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "3 + 3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._3arti3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "4 + 3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._4arti3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "4 + 4'lü Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._4arti4luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "90 Derece 3-Sağ 2-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._90Derece3Sag2SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "90 Derece 3-Sağ 3-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._90Derece3Sag3SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "90 Derece 4-Sağ 3-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._90Derece4Sag3SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Isı Camlı Sistem" + "90 Derece 4-Sağ 4-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Isicam._90Derece4Sag4SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                
                case ("Tek Camlı Sistem" + "2'li Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._2liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "4'lü Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._4luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "3 + 2'li Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._3arti2liSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "3 + 3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._3arti3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "4 + 3'lü Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._4arti3luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "4 + 4'lü Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._4arti4luSurmeSistem.profilKesimOlcusuHesaplama(en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "90 Derece 3-Sağ 2-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._90Derece3Sag2SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "90 Derece 3-Sağ 3-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._90Derece3Sag3SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "90 Derece 4-Sağ 3-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._90Derece4Sag3SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                case ("Tek Camlı Sistem" + "90 Derece 4-Sağ 4-Sol Sürme Sistem"): return SurmeSistemEsiksiz.Tekcam._90Derece4Sag4SolSurmeSistem.profilKesimOlcusuHesaplama(solEn, en, boy, adet, profils);
                default:
                    return null;
            }
        }
    }
}
