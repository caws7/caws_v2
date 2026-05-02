namespace CamSistemDataLayer.Models
{
    public class Maliyet
    {
        public string Malzeme { get; set; }
        public string Birim { get; set; }
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal ToplamTutar { get; set; }
    }
}
