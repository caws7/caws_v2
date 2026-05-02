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
    }
}