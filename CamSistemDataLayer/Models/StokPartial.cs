using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class Stok
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

        public int ProfilBoy
        {
            get
            {
                ProfilBoyRepo pbRepo = new ProfilBoyRepo();
                if (ProfilBoyId != null && ProfilBoyId > 0)
                {

                    return (int)pbRepo.FindBy(e => e.Id == ProfilBoyId).FirstOrDefault().ProfilBoyu;
                }
                else
                {
                    return 0;
                }
            }
        }

        public ProfilBoy ProfilBoyModel
        {
            get
            {
                ProfilBoyRepo pbRepo = new ProfilBoyRepo();
                return pbRepo.FindBy(e => e.Id == ProfilBoyId).FirstOrDefault();
            }
        }
    }
}
