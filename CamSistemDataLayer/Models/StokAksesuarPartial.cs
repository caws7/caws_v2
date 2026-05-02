using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class StokAksesuar
    {
        public Aksesuar AksesuarModel
        {
            get
            {
                AksesuarRepo aksesuarRepo = new AksesuarRepo();
                return aksesuarRepo.FindBy(e => e.Id == AksesuarId).FirstOrDefault();
            }
            set { }
        }
    }
}
