using CamSistemWebArayuz.Models.Sistemler;
using System;
using System.Collections.Generic;

namespace CamSistemWebArayuz.Models
{
    public class Teklif4Pdf
    {
        public long SiparisNo { get; set; }
        public DateTime Tarih { get; set; }
        public DateTime TeslimTarihi { get; set; }
        public string Firma { get; set; }
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public decimal Toplam { get; set; }
        public decimal KDV { get; set; }
        public decimal GenelToplam { get; set; }
        public string TeklifTur { get; set; }
        public List<Demonte> DemonteList { get; set; }
        public List<CamCati> CamCatiList { get; set; }
        public List<Pergola> PergolaList { get; set; }
        public List<RuzgarKirici> RuzgarKiriciList { get; set; }
        public List<Surme> SurmeList { get; set; }
        public List<ZipPerde> ZipPerdeList { get; set; }
        public string PartialAdi { get; set; }
        public string ExcelAdi { get; set; }
    }
}