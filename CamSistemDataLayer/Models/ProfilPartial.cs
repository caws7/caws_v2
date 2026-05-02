using System.Collections.Generic;

namespace CamSistemDataLayer.Models
{
    public partial class Profil
    {
        public int KesimOlcusu { get; set; }
        public int KesimAdet { get; set; }
        public double ToplamAgirlik { get; set; }
        public List<int> KesilenUzunluklar { get; set; }
        public ProfilBoy profilBoyModel { get; set; }
    }
}
