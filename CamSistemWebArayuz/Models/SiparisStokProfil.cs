namespace CamSistemWebArayuz.Models
{
    public class SiparisStokProfil
    {
        public string Kodu { get; set; }
        public string Adi { get; set; }
        public string Kesit { get; set; }
        public double BirimAgirlik { get; set; }
        public string Birim { get; set; }
        public string Renk { get; set; }
        public double Olcu { get; set; }
        public int Miktar { get; set; }
        public double ToplamMetre { get; set; }
        public double ToplamKg { get; set; }
        public decimal BirimFiyatKgM { get; set; }
        public decimal ToplamTutar { get; set; }
    }
}