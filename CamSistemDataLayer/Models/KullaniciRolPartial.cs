using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class KullaniciRol
    {
        public Kullanici KullaniciModel
        {
            get
            {
                KullaniciRepo kullaniciRepo = new KullaniciRepo();

                return kullaniciRepo.FindBy(e => e.Id == KullaniciId).FirstOrDefault();
            }
        }
        public Rol RolModel
        {
            get
            {
                RolRepo rolRepo = new RolRepo();

                return rolRepo.FindBy(e => e.Id == RolId).FirstOrDefault();
            }
        }
    }
}
