using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class OptimizasyonHesap
    {
        public Profil profil
        {
            get
            {
                ProfilRepo profilRepo = new ProfilRepo();
                return profilRepo.FindBy(e => e.Id == ProfilId).FirstOrDefault(); 
            }
        }
    }
}
