using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class RolSayfaYetki
    {
        public Rol RolModel
        {
            get
            {
                RolRepo rolRepo = new RolRepo();

                return rolRepo.FindBy(e => e.Id == RolId).FirstOrDefault();
            }
        }
        public Sayfa SayfaModel
        {
            get
            {
                SayfaRepo sayfaRepo = new SayfaRepo();

                return sayfaRepo.FindBy(e => e.Id == SayfaId).FirstOrDefault();
            }
        }
        public Yetki Yetki
        {
            get
            {
                YetkiRepo yetkiRepo = new YetkiRepo();

                return yetkiRepo.FindBy(e => e.Id == YetkiId).FirstOrDefault();
            }
        }
    }
}
