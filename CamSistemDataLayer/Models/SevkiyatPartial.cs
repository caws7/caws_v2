using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class Sevkiyat
    {
        public Siparis Siparis
        {
            get
            {
                SiparisRepo siparisRepo = new SiparisRepo();
                return siparisRepo.FindBy(e => e.Id == SiparisId).FirstOrDefault();
            }
        }
    }
}
