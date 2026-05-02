using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CamSistemWebArayuz.Models
{
    public class SiparisSayfaModel
    {
        public long MusteriId { get; set; }
        public int RenkId { get; set; }
        public int AltSistemId { get; set; }
        public int SistemId { get; set; }
        public int SistemTurId { get; set; }
        public DateTime TahminiTeslim { get; set; }
        public List<int> SeciliAksesuarlar { get; set; }
 
    }
}