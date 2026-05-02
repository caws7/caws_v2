using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CamSistemWebArayuz.ViewModels
{
    public class SiparisGorunumViewModel
    {
        public long Id { get; set; }
        public string SistemTamami { get; set; }
        public int? DurumId { get; set; }
        public int? SistemId { get; set; }         // ekledik
        public string SiparisTur { get; set; }     // ekledik
        public string OlusturanKullaniciAdi { get; set; }
        public DateTime? KayitTarihi { get; set; }
    }
}