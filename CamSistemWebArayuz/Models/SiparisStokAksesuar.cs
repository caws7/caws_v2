namespace CamSistemWebArayuz.Models
{
    public class SiparisStokAksesuar
    {
        public string Kodu { get; set; }
        public string Adi { get; set; }
        public string Birim { get; set; }
        public string Gorsel { get; set; }
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal ToplamTutar { get; set; }
    }
}