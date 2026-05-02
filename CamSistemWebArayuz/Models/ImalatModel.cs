using CamSistemDataLayer.Models;
using System.Collections.Generic;

namespace CamSistemWebArayuz.Models
{
    public class ImalatModel
    {
        public List<Stok> yetersizStokList { get; set; }
        public List<OptimizasyonSonuc> optiSonuc { get; set; }
        public double toplamAtikUzunluk { get; set; }
        public double toplamAtikAgirlik { get; set; }
        public double fireStogaEklenenToplam { get; set; }
        public double fireStogaEklenenToplamAgirlik { get; set; }
        public double kullanilanToplamUzunlukAsil { get; set; }
        public double kullanilanToplamAgirlikAsil { get; set; }
        public double kullanilanToplamUzunlukFire { get; set; }
        public double kullanilanToplamAgirlikFire { get; set; }
    }
}