using CamSistemDataLayer.Models;
using System;
using System.Collections.Generic;

namespace CamSistemWebArayuz.Models
{
    public class SiparisStokSablon
    {
        public List<SiparisStokProfil> profilList { get; set; } = new List<SiparisStokProfil>();
        public List<SiparisStokAksesuar> aksesuarList { get; set; } = new List<SiparisStokAksesuar>();
        public double ProfilToplamKg { get; set; }
        public decimal ProfilToplamTutar { get; set; }
        public decimal AksesuarToplamTutar { get; set; }
        public decimal GenelToplamTutar { get; set; }
        public decimal Kdv18 { get; set; }
        public decimal KdvliToplam { get; set; }
        public long SiparisId { get; set; }
        public string SirketAd { get; set; }
        public string SirketAdres { get; set; }
        public DateTime SiparisTarih { get; set; }
        public DateTime TeslimTarihi { get; set; }
        public Siparis Siparis { get; set; }
        public BoyaTedarik BoyaTedarik { get; set; }
        public CamTedarik CamTedarik { get; set; }
        public DisTedarik DisTedarik { get; set; }
        public Sevkiyat sevkiyat { get; set; }
        public List<SiparisSevkiyatProfil> sevkiyatProfil { get; set; } = new List<SiparisSevkiyatProfil>();
        public List<SiparisSevkiyatAksesuar> sevkiyatAksesuar { get; set; } = new List<SiparisSevkiyatAksesuar>();
    }
}
