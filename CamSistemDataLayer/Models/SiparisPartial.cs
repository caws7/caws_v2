using CamSistemDataLayer.Repos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class Siparis
    {
        public string MusteriTamAdi
        {
            get
            {
                try
                {
                    MusteriRepo mRepo = new MusteriRepo();
                    Musteri musteri = mRepo.FindBy(e => e.Id == MusteriId).FirstOrDefault();
                    if (musteri != null)
                        return musteri.AdSoyadSirketAdi;
                    else
                        return "";
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("[MusteriTamAdi] Hata MusteriId=" + MusteriId + ": " + ex.Message); return ""; }
            }
        }

        public string MusteriAdres
        {
            get
            {
                try
                {
                    MusteriRepo mRepo = new MusteriRepo();
                    Musteri musteri = mRepo.FindBy(e => e.Id == MusteriId).FirstOrDefault();
                    if (musteri == null)
                        return "";
                    var adres = musteri.Adres;
                    if (adres == null)
                        return "";
                    return adres.AcikAdres + " " + adres.PostaKodu + " " + adres.Ilce + " - " + adres.Il + " / " + adres.Ulke;
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("[MusteriAdres] Hata MusteriId=" + MusteriId + ": " + ex.Message); return ""; }
            }
        }

        public string RenkAdi { get; set; }

        public Renk Renk
        {
            get
            {
                try
                {
                    RenkRepo renkRepo = new RenkRepo();
                    return renkRepo.FindBy(e => e.Id == RenkId).FirstOrDefault();
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("[Renk] Hata RenkId=" + RenkId + ": " + ex.Message); return null; }
            }
        }

        public string Motor { get; set; }
        public string Aksesuar { get; set; }
        public IList<string> SeciliAksesuarlar { get; set; }

        public string KullaniciTamAdi
        {
            get
            {
                try
                {
                    KullaniciRepo kRepo = new KullaniciRepo();
                    Kullanici kullanici = kRepo.FindBy(e => e.Id == OnayIptalKullaniciId).FirstOrDefault();
                    if (kullanici == null)
                    {
                        return "";
                    }
                    else
                    {
                        return kullanici.KullaniciAdi + " " + kullanici.KullaniciSoyadi;
                    }
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("[KullaniciTamAdi] Hata KullaniciId=" + OnayIptalKullaniciId + ": " + ex.Message); return ""; }
            }
        }

        public string SistemTamami
        {
            get
            {
                try
                {
                    SistemRepo sRepo = new SistemRepo();
                    SistemTurRepo stRepo = new SistemTurRepo();
                    AltSistemRepo asRepo = new AltSistemRepo();
                    SiparisEnBoyAdetRepo sebaRepo = new SiparisEnBoyAdetRepo();

                    // Collect unique system descriptions from per-row data when available
                    var rows = sebaRepo.FindBy(e => e.SiparisId == Id).ToList();
                    var hasPerRowSystem = rows.Any(r => r.SistemId.HasValue && r.SistemId.Value > 0);

                    if (hasPerRowSystem)
                    {
                        var sistemler = new System.Collections.Generic.List<string>();
                        foreach (var row in rows)
                        {
                            int rowSistemId = (row.SistemId.HasValue && row.SistemId.Value > 0) ? row.SistemId.Value : (SistemId ?? 0);
                            int rowSistemTurId = (row.SistemTurId.HasValue && row.SistemTurId.Value > 0) ? row.SistemTurId.Value : (SistemTurId ?? 0);
                            int rowAltSistemId = (row.AltSistemId.HasValue && row.AltSistemId.Value > 0) ? row.AltSistemId.Value : (AltSistemId ?? 0);

                            string rowRetVal = "";
                            if (rowSistemId > 0)
                            {
                                var sistem = sRepo.FindBy(e => e.Id == rowSistemId).FirstOrDefault();
                                if (sistem != null) rowRetVal = sistem.SistemAdi;
                            }
                            if (rowSistemTurId > 0 && rowSistemTurId != -1)
                            {
                                var sistemTur = stRepo.FindBy(e => e.Id == rowSistemTurId).FirstOrDefault();
                                if (sistemTur != null) rowRetVal = rowRetVal + " / " + sistemTur.TurAdi;
                            }
                            if (rowAltSistemId > 0 && rowAltSistemId != -1)
                            {
                                var altSistem = asRepo.FindBy(e => e.Id == rowAltSistemId).FirstOrDefault();
                                if (altSistem != null) rowRetVal = rowRetVal + " / " + altSistem.AltSistemAdi;
                            }
                            if (!string.IsNullOrWhiteSpace(rowRetVal) && !sistemler.Contains(rowRetVal))
                                sistemler.Add(rowRetVal);
                        }
                        if (sistemler.Count > 0)
                            return string.Join(", ", sistemler);
                    }

                    // Fallback to order-level system
                    string retVal = "";
                    if (SistemId != null && SistemId != -1)
                    {
                        var sistem = sRepo.FindBy(e => e.Id == SistemId).FirstOrDefault();
                        if (sistem != null)
                            retVal = sistem.SistemAdi;
                    }
                    if (SistemTurId != -1 && SistemTurId != null)
                    {
                        var sistemTur = stRepo.FindBy(e => e.Id == SistemTurId).FirstOrDefault();
                        if (sistemTur != null)
                            retVal = retVal + " / " + sistemTur.TurAdi;
                    }
                    if (AltSistemId != -1 && AltSistemId != null)
                    {
                        var altSistem = asRepo.FindBy(e => e.Id == AltSistemId).FirstOrDefault();
                        if (altSistem != null)
                            retVal = retVal + " / " + altSistem.AltSistemAdi;
                    }
                    return retVal;
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("[SistemTamami] Hata SiparisId=" + Id + ": " + ex.Message); return ""; }
            }
        }

        // ------- EKLEDİK: Detaylar Listesi --------
        public List<SiparisEnBoyAdet> enBoyAdetList
        {
            get
            {
                SiparisEnBoyAdetRepo sebaRepo = new SiparisEnBoyAdetRepo();
                return sebaRepo.FindBy(e => e.SiparisId == Id).ToList();
            }
        }
        //-------------------------------------------

        public bool SevkiyatVarMi
        {
            get
            {
                SevkiyatRepo sevkiyatRepo = new SevkiyatRepo();
                Sevkiyat sevkiyat = sevkiyatRepo.FindBy(e => e.SiparisId == Id).FirstOrDefault();
                if (sevkiyat == null)
                    return false;
                else
                    return true;
            }
        }
    }
}
