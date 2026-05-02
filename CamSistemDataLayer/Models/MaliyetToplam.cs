using System.Collections.Generic;

namespace CamSistemDataLayer.Models
{
    public class MaliyetToplam
    {
        public List<Maliyet> MaliyetList { get; set; }
        public decimal ToplamMaliyet { get; set; }
        public decimal M2 { get; set; }
        public decimal Teklif { get; set; }
    }
}
