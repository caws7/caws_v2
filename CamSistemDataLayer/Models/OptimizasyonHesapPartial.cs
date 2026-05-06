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
                try
                {
                    if (ProfilId == null) return null;
                    ProfilRepo profilRepo = new ProfilRepo();
                    return profilRepo.FindBy(e => e.Id == ProfilId).FirstOrDefault();
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
