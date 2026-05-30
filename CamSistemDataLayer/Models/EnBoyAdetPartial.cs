using CamSistemDataLayer.Helpers;
using CamSistemDataLayer.Repos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class SiparisEnBoyAdet
    {
        public List<Profil> profilList { get; set; }
        public List<CamBilgileri> camList { get; set; }
        public ProfilDetayBilgileri camModel { get; set; }
        public Siparis siparisModel { get; set; }
        public SiparisCam siparisCam { get; set; }
        public MaliyetToplam maliyet { get; set; }
        public List<OptimizasyonHesap> optimizasyonList { get; set; }
        public CamTedarik camTedarik { get; set; }
        public DisTedarik disTedarik { get; set; }
        public List<SiparisTeklif> teklifList { get; set; }
        public SiparisTeklifToplamBilgisi teklifToplamDetay { get; set; }
        public BoyaTedarik boyaTedarik { get; set; }
        public Sevkiyat sevkiyat { get; set; }
        public List<SiparisSevkiyatProfil> sevkiyatProfil { get; set; }
        public List<SiparisSevkiyatAksesuar> sevkiyatAksesuar { get; set; }

        /// <summary>
        /// Returns the system name for this specific measurement row only.
        /// Uses per-row SistemId/SistemTurId/AltSistemId, falling back to the order-level values.
        /// </summary>
        public string SistemTamamiRow
        {
            get
            {
                try
                {
                SistemRepo sRepo = new SistemRepo();
                SistemTurRepo stRepo = new SistemTurRepo();
                AltSistemRepo asRepo = new AltSistemRepo();

                bool satirBazliSistemVar = SistemId.HasValue && SistemId.Value > 0;
                int rowSistemId = satirBazliSistemVar ? SistemId.Value : (siparisModel?.SistemId ?? 0);
                int? rowSistemTurId = satirBazliSistemVar ? SistemTurId : siparisModel?.SistemTurId;
                int? rowAltSistemId = satirBazliSistemVar ? AltSistemId : siparisModel?.AltSistemId;
                if (satirBazliSistemVar && siparisModel != null && siparisModel.SistemId.HasValue && siparisModel.SistemId.Value > 0 && rowSistemId == siparisModel.SistemId.Value)
                {
                    if ((!rowSistemTurId.HasValue || rowSistemTurId.Value <= 0) && siparisModel.SistemTurId.HasValue && siparisModel.SistemTurId.Value > 0)
                        rowSistemTurId = siparisModel.SistemTurId;
                    if ((!rowAltSistemId.HasValue || rowAltSistemId.Value <= 0) && siparisModel.AltSistemId.HasValue && siparisModel.AltSistemId.Value > 0)
                        rowAltSistemId = siparisModel.AltSistemId;
                }

                string retVal = "";
                if (rowSistemId > 0)
                {
                    var sistem = sRepo.FindBy(e => e.Id == rowSistemId).FirstOrDefault();
                    if (sistem != null) retVal = sistem.SistemAdi;
                }
                if (rowSistemTurId.HasValue && rowSistemTurId.Value > 0 && rowSistemTurId.Value != -1)
                {
                    var sistemTur = stRepo.FindBy(e => e.Id == rowSistemTurId.Value).FirstOrDefault();
                    if (sistemTur != null) retVal = retVal + " / " + sistemTur.TurAdi;
                }
                if (rowAltSistemId.HasValue && rowAltSistemId.Value > 0 && rowAltSistemId.Value != -1)
                {
                    var altSistem = asRepo.FindBy(e => e.Id == rowAltSistemId.Value).FirstOrDefault();
                    if (altSistem != null) retVal = retVal + " / " + altSistem.AltSistemAdi;
                }
                if (!string.IsNullOrWhiteSpace(KasaTipi))
                {
                    retVal = retVal + " / " + KasaTipi;
                }
                return TurkishTextNormalizer.NormalizeDisplayText(retVal);
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("[SistemTamamiRow] Hata: " + ex.Message); return ""; }
            }
        }
    }
}
