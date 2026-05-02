using CamSistemDataLayer.Models;
using System.Collections.Generic;

namespace CamSistemWebArayuz.Models
{
    public class SiparisStokModel
    {
        public Siparis SiparisModel { get; set; }
        public List<SiparisStok> SiparisStokList { get; set; }
        public SiparisStok SiparisStok { get; set; }
    }
}