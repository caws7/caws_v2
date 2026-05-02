using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class AtikStok
    {
        public Profil ProfilModel
        {
            get
            {
                ProfilRepo pRepo = new ProfilRepo();
                return pRepo.FindBy(e => e.Id == ProfilId).FirstOrDefault();
            }
            set { }
        }
    }
}
