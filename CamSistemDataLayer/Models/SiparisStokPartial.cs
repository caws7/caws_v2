using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class SiparisStok
    {
        public Profil profil
        {
            get
            {
                ProfilRepo profilRepo = new ProfilRepo();

                return profilRepo.FindBy(e => e.Id == ProfilId).FirstOrDefault();
            }
        }

        public ProfilBoy profilBoy
        {
            get
            {
                ProfilBoyRepo profilBoyRepo = new ProfilBoyRepo();

                return profilBoyRepo.FindBy(e => e.Id == ProfilBoyId).FirstOrDefault();
            }
        }

        public Aksesuar aksesuar
        {
            get
            {
                AksesuarRepo aksesuarRepo = new AksesuarRepo();

                return aksesuarRepo.FindBy(e => e.Id == AksesuarId).FirstOrDefault();
            }
        }
    }
}
