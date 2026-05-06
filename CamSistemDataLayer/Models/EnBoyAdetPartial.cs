using CamSistemDataLayer.Repos;
using System.Collections.Generic;

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
                SistemRepo sRepo = new SistemRepo();
                SistemTurRepo stRepo = new SistemTurRepo();
                AltSistemRepo asRepo = new AltSistemRepo();

                int rowSistemId = (SistemId.HasValue && SistemId.Value > 0) ? SistemId.Value
                    : (siparisModel?.SistemId ?? 0);
                int rowSistemTurId = (SistemTurId.HasValue && SistemTurId.Value > 0) ? SistemTurId.Value
                    : (siparisModel?.SistemTurId ?? 0);
                int rowAltSistemId = (AltSistemId.HasValue && AltSistemId.Value > 0) ? AltSistemId.Value
                    : (siparisModel?.AltSistemId ?? 0);

                string retVal = "";
                if (rowSistemId > 0)
                {
                    var sistem = sRepo.FindBy(e => e.Id == rowSistemId).FirstOrDefault();
                    if (sistem != null) retVal = sistem.SistemAdi;
                }
                if (rowSistemTurId > 0 && rowSistemTurId != -1)
                {
                    var sistemTur = stRepo.FindBy(e => e.Id == rowSistemTurId).FirstOrDefault();
                    if (sistemTur != null) retVal = retVal + " / " + sistemTur.TurAdi;
                }
                if (rowAltSistemId > 0 && rowAltSistemId != -1)
                {
                    var altSistem = asRepo.FindBy(e => e.Id == rowAltSistemId).FirstOrDefault();
                    if (altSistem != null) retVal = retVal + " / " + altSistem.AltSistemAdi;
                }
                return retVal;
            }
        }
    }
}