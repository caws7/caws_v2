using CamSistemDataLayer.Models;
using System.Collections.Generic;

namespace CamSistemWebArayuz.Models
{
    public class SiparisveEnBoyAdet
    {
        public Siparis SiparisModel { get; set; }
        public List<SiparisEnBoyAdet> SiparisEnBoyAdetList { get; set; }
        public SiparisCam SiparisCamModel { get; set; }
    }
}