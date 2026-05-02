using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using CamSistemWebArayuz.Models.Sistemler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamSistemWebArayuz.Models
{
    public static class SistemCiktisi
    {
        static AdresRepo adresRepo;
        static SistemAltSistemJoinRepo sistemAltSistemJoinRepo;
        static SiparisRepo siparisRepo;
        static AksesuarRepo aksesuarRepo;
        static MusteriRepo musteriRepo;
        static SiparisEnBoyAdetRepo sebaRepo;
        static SiparisAksesuarRepo siparisAksesuarRepo;
        static SiparisCamRepo scRepo;

        public static SiparisStokSablon profilGonderimi(long SiparisId)
        {
            return null;
        }

        public static Teklif4Pdf demonteGonderimi(long SiparisId)
        {
            Teklif4Pdf sablon = new Teklif4Pdf();
            siparisRepo = new SiparisRepo();

            Siparis siparis = siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
            SistemRepo sistemRepo = new SistemRepo();
            Sistem sistem = sistemRepo.FindBy(e => e.Id == siparis.SistemId).FirstOrDefault();

            switch (sistem.SistemAdi)
            {
                case string a when a.ToLower().Contains("giyotin"): sablon = giyotin(siparis); break;
                case string a when a.ToLower().Contains("cam çatı"): sablon = camCati(siparis); break;
                case string a when a.ToLower().Contains("perde"): sablon = zipPerde(siparis); break;
                case string a when a.ToLower().Contains("rüzgar"): sablon = ruzgarKirici(siparis); break;
                case string a when a.ToLower().Contains("sürme"): sablon = surme(siparis); break;
                case string a when a.ToLower().Contains("pergola"): sablon = pergola(siparis); break;
                case string a when a.ToLower().Contains("aura plus"): sablon = giyotin(siparis); break;
                case string a when a.ToLower().Contains("aura silinebilir sistem"): sablon = giyotin(siparis); break;
            }

            return sablon;
        }

        /* -- bitti -- */
        public static Teklif4Pdf giyotin(Siparis siparis)
        {
            decimal birimFiyat = 0;
            adresRepo = new AdresRepo();
            sistemAltSistemJoinRepo = new SistemAltSistemJoinRepo();

            aksesuarRepo = new AksesuarRepo();
            musteriRepo = new MusteriRepo();
            sebaRepo = new SiparisEnBoyAdetRepo();
            siparisAksesuarRepo = new SiparisAksesuarRepo();
            scRepo = new SiparisCamRepo();

            List<SiparisAksesuar> siparisAksesuar = siparisAksesuarRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            SiparisCam siparisCam = scRepo.FindBy(e => e.SiparisId == siparis.Id).FirstOrDefault();

            Musteri musteri = musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault();
            int adresId = (int)musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault().AdresId;
            Adres adres = adresRepo.FindBy(e => e.Id == adresId).FirstOrDefault();

            if (siparis.SistemBirimFiyat == null)
            {
                SistemAltSistemJoin sistemAltSistemJoin = sistemAltSistemJoinRepo.FindBy(e => e.SistemId == siparis.SistemId &&
                                                            e.AltSistemId == siparis.AltSistemId && e.SistemTurId == siparis.SistemTurId).FirstOrDefault();
                birimFiyat = (decimal)sistemAltSistemJoin.BirimFiyat;
            }
            else
            {
                birimFiyat = (decimal)siparis.SistemBirimFiyat;
            }

            List<SiparisEnBoyAdet> enBoyList = sebaRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            Teklif4Pdf teklif4Pdf = new Teklif4Pdf();
            teklif4Pdf.SiparisNo = siparis.Id;
            teklif4Pdf.Tarih = Convert.ToDateTime(siparis.KayitTarihi);
            teklif4Pdf.TeslimTarihi = Convert.ToDateTime(siparis.TahminiTeslim);
            teklif4Pdf.Firma = musteri.AdSoyadSirketAdi;
            teklif4Pdf.Adres = adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;
            teklif4Pdf.Telefon = musteri.MusteriTelefon;
            List<Demonte> demonteTeklif4Pdfs = new List<Demonte>();
            foreach (SiparisEnBoyAdet item in enBoyList)
            {
                OrtakAlanlar ortak = new OrtakAlanlar();
                Demonte demonteTeklif4Pdf = new Demonte();
                ortak.UrunAciklama = siparis.SistemTamami;
                demonteTeklif4Pdf.CamKombinasyon = siparisCam == null ? string.Empty : siparisCam.CamKombinasyon;
                demonteTeklif4Pdf.En = Convert.ToInt32(item.GirilenEn);
                demonteTeklif4Pdf.Boy = Convert.ToInt32(item.GirilenBoy);
                ortak.Adet = Convert.ToInt32(item.GirilenAdet);
                ortak.Alan = ((double)demonteTeklif4Pdf.En / 1000) * ((double)demonteTeklif4Pdf.Boy / 1000);

                ortak.BirimFiyat = Convert.ToDecimal(birimFiyat);
                if (ortak.Alan > 6.5)
                    ortak.ToplamTutar = Convert.ToDecimal(ortak.Adet * ortak.BirimFiyat * (decimal)ortak.Alan);
                else
                    ortak.ToplamTutar = Convert.ToDecimal(ortak.Adet * ortak.BirimFiyat * (decimal)6.5);


                foreach (var itemAksesuar in siparisAksesuar)
                {
                    Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == itemAksesuar.AksesuarId).FirstOrDefault();
                    if (aksesuar.Motor != null && (bool)aksesuar.Motor)
                    {
                        demonteTeklif4Pdf.Motor = aksesuar.AksesuarAdi;
                    }
                    else if (aksesuar.Kumanda != null && (bool)aksesuar.Kumanda)
                    {
                        demonteTeklif4Pdf.Kumanda = aksesuar.AksesuarAdi;
                    }
                    else if (aksesuar.AksesuarAdi.ToLower().Contains("kayış") || aksesuar.AksesuarAdi.ToLower().Contains("zincir"))
                    {
                        demonteTeklif4Pdf.AksesuarSet = aksesuar.AksesuarAdi;
                    }
                    else
                    {
                        demonteTeklif4Pdf.BaglantiSistem = aksesuar.AksesuarAdi;
                    }
                }

                demonteTeklif4Pdf.ortak = ortak;
                demonteTeklif4Pdfs.Add(demonteTeklif4Pdf);
            }

            teklif4Pdf.DemonteList = demonteTeklif4Pdfs;
            teklif4Pdf.Toplam = demonteTeklif4Pdfs.ToList().Select(e => e.ortak.ToplamTutar).Sum();
            teklif4Pdf.KDV = teklif4Pdf.Toplam * 20 / 100;
            teklif4Pdf.GenelToplam = teklif4Pdf.Toplam + teklif4Pdf.KDV;
            teklif4Pdf.PartialAdi = "_siparisDemonteTeklif4Pdf";
            teklif4Pdf.ExcelAdi = "sablonDemonte.xlsx";

            return teklif4Pdf;
        }

        /* -- bitti -- */ /* -- bilinmeyen var -- */
        private static Teklif4Pdf camCati(Siparis siparis)
        {
            decimal birimFiyat = 0;
            adresRepo = new AdresRepo();
            sistemAltSistemJoinRepo = new SistemAltSistemJoinRepo();

            aksesuarRepo = new AksesuarRepo();
            musteriRepo = new MusteriRepo();
            sebaRepo = new SiparisEnBoyAdetRepo();
            siparisAksesuarRepo = new SiparisAksesuarRepo();
            scRepo = new SiparisCamRepo();

            List<SiparisAksesuar> siparisAksesuar = siparisAksesuarRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            SiparisCam siparisCam = scRepo.FindBy(e => e.SiparisId == siparis.Id).FirstOrDefault();

            Musteri musteri = musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault();
            int adresId = (int)musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault().AdresId;
            Adres adres = adresRepo.FindBy(e => e.Id == adresId).FirstOrDefault();

            if (siparis.SistemBirimFiyat == null)
            {
                SistemAltSistemJoin sistemAltSistemJoin = sistemAltSistemJoinRepo.FindBy(e => e.SistemId == siparis.SistemId).FirstOrDefault();
                birimFiyat = (decimal)sistemAltSistemJoin.BirimFiyat;
            }
            else
            {
                birimFiyat = (decimal)siparis.SistemBirimFiyat;
            }

            List<SiparisEnBoyAdet> enBoyList = sebaRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            Teklif4Pdf teklif4Pdf = new Teklif4Pdf();
            teklif4Pdf.SiparisNo = siparis.Id;
            teklif4Pdf.Tarih = Convert.ToDateTime(siparis.KayitTarihi);
            teklif4Pdf.Firma = musteri.AdSoyadSirketAdi;
            teklif4Pdf.Adres = adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;
            teklif4Pdf.Telefon = musteri.MusteriTelefon;
            List<CamCati> camCatiTeklif4Pdfs = new List<CamCati>();
            foreach (SiparisEnBoyAdet item in enBoyList)
            {
                CamCati camCatiTeklif4Pdf = new CamCati();
                camCatiTeklif4Pdf.CamKombinasyon = siparisCam.CamKombinasyon;
                camCatiTeklif4Pdf.En = Convert.ToInt32(item.GirilenEn);
                camCatiTeklif4Pdf.Boy = Convert.ToInt32(item.GirilenBoy);

                //bilinmeyenler
                camCatiTeklif4Pdf.OnYukseklik = "0";
                camCatiTeklif4Pdf.ArkaYukseklik = "0";

                OrtakAlanlar ortak = new OrtakAlanlar();
                ortak.UrunAciklama = siparis.SistemTamami;
                ortak.Adet = Convert.ToInt32(item.GirilenAdet);
                ortak.Alan = ((double)camCatiTeklif4Pdf.En / 1000) * ((double)camCatiTeklif4Pdf.Boy / 1000);
                ortak.BirimFiyat = Convert.ToDecimal(birimFiyat);
                ortak.ToplamTutar = Convert.ToDecimal(ortak.Adet * ortak.BirimFiyat * (decimal)ortak.Alan);

                foreach (var itemAksesuar in siparisAksesuar)
                {
                    Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == itemAksesuar.AksesuarId).FirstOrDefault();
                    if (aksesuar.AksesuarAdi.ToLower().Contains("aksesuar seti") && !aksesuar.AksesuarAdi.ToLower().Contains("aparat"))
                    {
                        camCatiTeklif4Pdf.AksesuarSet = aksesuar.AksesuarAdi;
                    }
                }

                camCatiTeklif4Pdf.ortak = ortak;
                camCatiTeklif4Pdfs.Add(camCatiTeklif4Pdf);
            }

            teklif4Pdf.CamCatiList = camCatiTeklif4Pdfs;
            teklif4Pdf.Toplam = camCatiTeklif4Pdfs.ToList().Sum(e => e.ortak.ToplamTutar);
            teklif4Pdf.KDV = teklif4Pdf.Toplam * 20 / 100;
            teklif4Pdf.GenelToplam = teklif4Pdf.Toplam + teklif4Pdf.KDV;
            teklif4Pdf.PartialAdi = "_siparisCamCatiTeklif4Pdf";
            teklif4Pdf.ExcelAdi = "sablonCamCati.xlsx";

            return teklif4Pdf;
        }

        /* -- bitti -- */
        private static Teklif4Pdf surme(Siparis siparis)
        {
            decimal birimFiyat = 0;
            adresRepo = new AdresRepo();
            sistemAltSistemJoinRepo = new SistemAltSistemJoinRepo();

            aksesuarRepo = new AksesuarRepo();
            musteriRepo = new MusteriRepo();
            sebaRepo = new SiparisEnBoyAdetRepo();
            siparisAksesuarRepo = new SiparisAksesuarRepo();
            scRepo = new SiparisCamRepo();

            List<SiparisAksesuar> siparisAksesuar = siparisAksesuarRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            SiparisCam siparisCam = scRepo.FindBy(e => e.SiparisId == siparis.Id).FirstOrDefault();

            Musteri musteri = musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault();
            int adresId = (int)musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault().AdresId;
            Adres adres = adresRepo.FindBy(e => e.Id == adresId).FirstOrDefault();

            if (siparis.SistemBirimFiyat == null)
            {
                SistemAltSistemJoin sistemAltSistemJoin = sistemAltSistemJoinRepo.FindBy(e => e.SistemId == siparis.SistemId &&
                                                            e.AltSistemId == siparis.AltSistemId && e.SistemTurId == siparis.SistemTurId).FirstOrDefault();
                birimFiyat = (decimal)sistemAltSistemJoin.BirimFiyat;
            }
            else
            {
                birimFiyat = (decimal)siparis.SistemBirimFiyat;
            }

            List<SiparisEnBoyAdet> enBoyList = sebaRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            Teklif4Pdf teklif4Pdf = new Teklif4Pdf();
            teklif4Pdf.SiparisNo = siparis.Id;
            teklif4Pdf.Tarih = Convert.ToDateTime(siparis.KayitTarihi);
            teklif4Pdf.Firma = musteri.AdSoyadSirketAdi;
            teklif4Pdf.Adres = adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;
            teklif4Pdf.Telefon = musteri.MusteriTelefon;
            List<Surme> surmeTeklif4Pdfs = new List<Surme>();
            foreach (SiparisEnBoyAdet item in enBoyList)
            {
                Surme surmeTeklif4Pdf = new Surme();
                surmeTeklif4Pdf.CamKombinasyon = siparisCam.CamKombinasyon;
                surmeTeklif4Pdf.En = Convert.ToInt32(item.GirilenEn);
                surmeTeklif4Pdf.Boy = Convert.ToInt32(item.GirilenBoy);

                OrtakAlanlar ortak = new OrtakAlanlar();
                ortak.UrunAciklama = siparis.SistemTamami;
                ortak.Adet = Convert.ToInt32(item.GirilenAdet);
                ortak.Alan = ((double)surmeTeklif4Pdf.En / 1000) * ((double)surmeTeklif4Pdf.Boy / 1000);
                ortak.BirimFiyat = Convert.ToDecimal(birimFiyat);
                ortak.ToplamTutar = Convert.ToDecimal(ortak.Adet * ortak.BirimFiyat * (decimal)ortak.Alan);

                foreach (var itemAksesuar in siparisAksesuar)
                {
                    Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == itemAksesuar.AksesuarId).FirstOrDefault();
                    if (aksesuar.AksesuarAdi.ToLower().Contains("aksesuar seti") && !aksesuar.AksesuarAdi.ToLower().Contains("aparat"))
                    {
                        surmeTeklif4Pdf.AksesuarSet = aksesuar.AksesuarAdi;
                    }
                }

                surmeTeklif4Pdf.ortak = ortak;
                surmeTeklif4Pdfs.Add(surmeTeklif4Pdf);
            }

            teklif4Pdf.SurmeList = surmeTeklif4Pdfs;
            teklif4Pdf.Toplam = surmeTeklif4Pdfs.ToList().Sum(e => e.ortak.ToplamTutar);
            teklif4Pdf.KDV = teklif4Pdf.Toplam * 20 / 100;
            teklif4Pdf.GenelToplam = teklif4Pdf.Toplam + teklif4Pdf.KDV;
            teklif4Pdf.PartialAdi = "_siparisSurmeTeklif4Pdf";
            teklif4Pdf.ExcelAdi = "sablonSurme.xlsx";

            return teklif4Pdf;
        }

        /* -- bitti -- */
        private static Teklif4Pdf ruzgarKirici(Siparis siparis)
        {
            decimal birimFiyat = 0;
            adresRepo = new AdresRepo();
            sistemAltSistemJoinRepo = new SistemAltSistemJoinRepo();

            aksesuarRepo = new AksesuarRepo();
            musteriRepo = new MusteriRepo();
            sebaRepo = new SiparisEnBoyAdetRepo();
            siparisAksesuarRepo = new SiparisAksesuarRepo();
            scRepo = new SiparisCamRepo();

            List<SiparisAksesuar> siparisAksesuar = siparisAksesuarRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            SiparisCam siparisCam = scRepo.FindBy(e => e.SiparisId == siparis.Id).FirstOrDefault();
            Musteri musteri = musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault();
            int adresId = (int)musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault().AdresId;
            Adres adres = adresRepo.FindBy(e => e.Id == adresId).FirstOrDefault();

            if (siparis.SistemBirimFiyat == null)
            {
                SistemAltSistemJoin sistemAltSistemJoin = sistemAltSistemJoinRepo.FindBy(e => e.SistemId == siparis.SistemId).FirstOrDefault();
                birimFiyat = (decimal)sistemAltSistemJoin.BirimFiyat;
            }
            else
            {
                birimFiyat = (decimal)siparis.SistemBirimFiyat;
            }

            List<SiparisEnBoyAdet> enBoyList = sebaRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            Teklif4Pdf teklif4Pdf = new Teklif4Pdf();
            teklif4Pdf.SiparisNo = siparis.Id;
            teklif4Pdf.Tarih = Convert.ToDateTime(siparis.KayitTarihi);
            teklif4Pdf.Firma = musteri.AdSoyadSirketAdi;
            teklif4Pdf.Adres = adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;
            teklif4Pdf.Telefon = musteri.MusteriTelefon;
            List<RuzgarKirici> ruzgarKiriciTeklif4Pdfs = new List<RuzgarKirici>();
            foreach (SiparisEnBoyAdet item in enBoyList)
            {
                RuzgarKirici ruzgarKiriciTeklif4Pdf = new RuzgarKirici();
                ruzgarKiriciTeklif4Pdf.CamKombinasyon = siparisCam.CamKombinasyon;
                ruzgarKiriciTeklif4Pdf.En = Convert.ToInt32(item.GirilenEn);
                ruzgarKiriciTeklif4Pdf.Boy = Convert.ToInt32(item.GirilenBoy);

                OrtakAlanlar ortak = new OrtakAlanlar();
                ortak.UrunAciklama = siparis.SistemTamami;
                ortak.Adet = Convert.ToInt32(item.GirilenAdet);
                ortak.Alan = ((double)ruzgarKiriciTeklif4Pdf.En / 1000) * ((double)ruzgarKiriciTeklif4Pdf.Boy / 1000);
                ortak.BirimFiyat = Convert.ToDecimal(birimFiyat);
                ortak.ToplamTutar = Convert.ToDecimal(ortak.Adet * ortak.BirimFiyat * (decimal)ortak.Alan);

                foreach (var itemAksesuar in siparisAksesuar)
                {
                    Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == itemAksesuar.AksesuarId).FirstOrDefault();
                    if (aksesuar.AksesuarAdi.ToLower().Contains("aksesuar seti") && !aksesuar.AksesuarAdi.ToLower().Contains("aparat"))
                    {
                        ruzgarKiriciTeklif4Pdf.AksesuarSet = aksesuar.AksesuarAdi;
                    }
                    else
                    {
                        ruzgarKiriciTeklif4Pdf.BaglantiSistem = aksesuar.AksesuarAdi;
                    }
                }

                ruzgarKiriciTeklif4Pdf.ortak = ortak;
                ruzgarKiriciTeklif4Pdfs.Add(ruzgarKiriciTeklif4Pdf);
            }

            teklif4Pdf.RuzgarKiriciList = ruzgarKiriciTeklif4Pdfs;
            teklif4Pdf.Toplam = ruzgarKiriciTeklif4Pdfs.ToList().Sum(e => e.ortak.ToplamTutar);
            teklif4Pdf.KDV = teklif4Pdf.Toplam * 20 / 100;
            teklif4Pdf.GenelToplam = teklif4Pdf.Toplam + teklif4Pdf.KDV;
            teklif4Pdf.PartialAdi = "_siparisRuzgarKiriciTeklif4Pdf";
            teklif4Pdf.ExcelAdi = "sablonRuzgarKirici.xlsx";

            return teklif4Pdf;
        }

        /* -- bitti -- */ /* -- bilinmeyen var -- */
        private static Teklif4Pdf zipPerde(Siparis siparis)
        {
            decimal birimFiyat = 0;
            adresRepo = new AdresRepo();
            sistemAltSistemJoinRepo = new SistemAltSistemJoinRepo();

            aksesuarRepo = new AksesuarRepo();
            musteriRepo = new MusteriRepo();
            sebaRepo = new SiparisEnBoyAdetRepo();
            siparisAksesuarRepo = new SiparisAksesuarRepo();
            scRepo = new SiparisCamRepo();

            List<SiparisAksesuar> siparisAksesuar = siparisAksesuarRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            SiparisCam siparisCam = scRepo.FindBy(e => e.SiparisId == siparis.Id).FirstOrDefault();
            Musteri musteri = musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault();
            int adresId = (int)musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault().AdresId;
            Adres adres = adresRepo.FindBy(e => e.Id == adresId).FirstOrDefault();

            if (siparis.SistemBirimFiyat == null)
            {
                SistemAltSistemJoin sistemAltSistemJoin = sistemAltSistemJoinRepo.FindBy(e => e.SistemId == siparis.SistemId).FirstOrDefault();
                birimFiyat = (decimal)sistemAltSistemJoin.BirimFiyat;
            }
            else
            {
                birimFiyat = (decimal)siparis.SistemBirimFiyat;
            }

            List<SiparisEnBoyAdet> enBoyList = sebaRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            Teklif4Pdf teklif4Pdf = new Teklif4Pdf();
            teklif4Pdf.SiparisNo = siparis.Id;
            teklif4Pdf.Tarih = Convert.ToDateTime(siparis.KayitTarihi);
            teklif4Pdf.Firma = musteri.AdSoyadSirketAdi;
            teklif4Pdf.Adres = adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;
            teklif4Pdf.Telefon = musteri.MusteriTelefon;
            List<ZipPerde> zipPerdeTeklif4Pdfs = new List<ZipPerde>();
            foreach (SiparisEnBoyAdet item in enBoyList)
            {
                ZipPerde zipPerdeTeklif4Pdf = new ZipPerde();
                zipPerdeTeklif4Pdf.En = Convert.ToInt32(item.GirilenEn);
                zipPerdeTeklif4Pdf.Boy = Convert.ToInt32(item.GirilenBoy);

                //bilinmeyenler
                zipPerdeTeklif4Pdf.Kumas = "";

                OrtakAlanlar ortak = new OrtakAlanlar();
                ortak.UrunAciklama = siparis.SistemTamami;
                ortak.Adet = Convert.ToInt32(item.GirilenAdet);
                ortak.Alan = ((double)zipPerdeTeklif4Pdf.En / 1000) * ((double)zipPerdeTeklif4Pdf.Boy / 1000);
                ortak.BirimFiyat = Convert.ToDecimal(birimFiyat);
                ortak.ToplamTutar = Convert.ToDecimal(ortak.Adet * ortak.BirimFiyat * (decimal)ortak.Alan);

                foreach (var itemAksesuar in siparisAksesuar)
                {
                    Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == itemAksesuar.AksesuarId).FirstOrDefault();
                    if (aksesuar.Motor != null && (bool)aksesuar.Motor)
                    {
                        zipPerdeTeklif4Pdf.Motor = aksesuar.AksesuarAdi;
                    }
                    else if (aksesuar.Kumanda != null && (bool)aksesuar.Kumanda)
                    {
                        zipPerdeTeklif4Pdf.Kumanda = aksesuar.AksesuarAdi;
                    }
                    else if (aksesuar.AksesuarAdi.ToLower().Contains("aksesuar seti") && !aksesuar.AksesuarAdi.ToLower().Contains("aparat"))
                    {
                        zipPerdeTeklif4Pdf.AksesuarSet = aksesuar.AksesuarAdi;
                    }
                }

                zipPerdeTeklif4Pdf.ortak = ortak;
                zipPerdeTeklif4Pdfs.Add(zipPerdeTeklif4Pdf);
            }

            teklif4Pdf.ZipPerdeList = zipPerdeTeklif4Pdfs;
            teklif4Pdf.Toplam = zipPerdeTeklif4Pdfs.ToList().Sum(e => e.ortak.ToplamTutar);
            teklif4Pdf.KDV = teklif4Pdf.Toplam * 20 / 100;
            teklif4Pdf.GenelToplam = teklif4Pdf.Toplam + teklif4Pdf.KDV;
            teklif4Pdf.PartialAdi = "_siparisZipPerdeTeklif4Pdf";
            teklif4Pdf.ExcelAdi = "sablonZipPerde.xlsx";

            return teklif4Pdf;
        }

        /* --  -- */ /* -- bilinmeyen var -- */
        private static Teklif4Pdf pergola(Siparis siparis)
        {
            decimal birimFiyat = 0;
            adresRepo = new AdresRepo();
            sistemAltSistemJoinRepo = new SistemAltSistemJoinRepo();

            aksesuarRepo = new AksesuarRepo();
            musteriRepo = new MusteriRepo();
            sebaRepo = new SiparisEnBoyAdetRepo();
            siparisAksesuarRepo = new SiparisAksesuarRepo();
            scRepo = new SiparisCamRepo();

            List<SiparisAksesuar> siparisAksesuar = siparisAksesuarRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            SiparisCam siparisCam = scRepo.FindBy(e => e.SiparisId == siparis.Id).FirstOrDefault();

            Musteri musteri = musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault();
            int adresId = (int)musteriRepo.FindBy(e => e.Id == siparis.MusteriId).FirstOrDefault().AdresId;
            Adres adres = adresRepo.FindBy(e => e.Id == adresId).FirstOrDefault();

            if (siparis.SistemBirimFiyat == null)
            {
                SistemAltSistemJoin sistemAltSistemJoin = sistemAltSistemJoinRepo.FindBy(e => e.SistemId == siparis.SistemId &&
                                                                e.AltSistemId == siparis.AltSistemId && e.SistemTurId == siparis.SistemTurId).FirstOrDefault();
                birimFiyat = (decimal)sistemAltSistemJoin.BirimFiyat;
            }
            else
            {
                birimFiyat = (decimal)siparis.SistemBirimFiyat;
            }

            List<SiparisEnBoyAdet> enBoyList = sebaRepo.FindBy(e => e.SiparisId == siparis.Id).ToList();
            Teklif4Pdf teklif4Pdf = new Teklif4Pdf();
            teklif4Pdf.SiparisNo = siparis.Id;
            teklif4Pdf.Tarih = Convert.ToDateTime(siparis.KayitTarihi);
            teklif4Pdf.Firma = musteri.AdSoyadSirketAdi;
            teklif4Pdf.Adres = adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;
            teklif4Pdf.Telefon = musteri.MusteriTelefon;
            List<Pergola> pergolaTeklif4Pdfs = new List<Pergola>();
            foreach (SiparisEnBoyAdet item in enBoyList)
            {
                OrtakAlanlar ortak = new OrtakAlanlar();
                Pergola pergolaTeklif4Pdf = new Pergola();
                ortak.UrunAciklama = siparis.SistemTamami;
                ortak.Adet = Convert.ToInt32(item.GirilenAdet);
                //ortak.Alan = ((double)pergolaTeklif4Pdf.En / 1000) * ((double)pergolaTeklif4Pdf.Boy / 1000);
                ortak.BirimFiyat = Convert.ToDecimal(birimFiyat);
                ortak.ToplamTutar = Convert.ToDecimal(ortak.Adet * ortak.BirimFiyat * (decimal)ortak.Alan);

                foreach (var itemAksesuar in siparisAksesuar)
                {
                    Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == itemAksesuar.AksesuarId).FirstOrDefault();
                    if (aksesuar.Motor != null && (bool)aksesuar.Motor)
                    {
                        pergolaTeklif4Pdf.Motor = aksesuar.AksesuarAdi;
                    }
                    else if (aksesuar.Kumanda != null && (bool)aksesuar.Kumanda)
                    {
                        pergolaTeklif4Pdf.Kumanda = aksesuar.AksesuarAdi;
                    }
                }

                pergolaTeklif4Pdf.ortak = ortak;
                pergolaTeklif4Pdfs.Add(pergolaTeklif4Pdf);
            }

            teklif4Pdf.PergolaList = pergolaTeklif4Pdfs;
            teklif4Pdf.Toplam = pergolaTeklif4Pdfs.ToList().Sum(e => e.ortak.ToplamTutar);
            teklif4Pdf.KDV = teklif4Pdf.Toplam * 20 / 100;
            teklif4Pdf.GenelToplam = teklif4Pdf.Toplam + teklif4Pdf.KDV;
            teklif4Pdf.PartialAdi = "_siparisPergolaTeklif4Pdf";
            teklif4Pdf.ExcelAdi = "sablonPergola.xlsx";

            return teklif4Pdf;
        }
    }
}