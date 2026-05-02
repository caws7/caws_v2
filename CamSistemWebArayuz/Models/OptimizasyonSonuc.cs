using CamSistemDataLayer.Models;

namespace CamSistemWebArayuz.Models
{
    public class OptimizasyonSonuc
    {
        public Profil profil { get; set; }
        public int KullanilacakOlcu {get;set;}
        public string KesileceklerOlcusu { get; set; }
        public int Adet { get; set; }
        public int FireAtik { get; set; }
        public string KullanilanAlan { get; set; }
        public int eksikAdet { get; set; }
        public int mevcutStokMiktari { get; set; }
    }
}