using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class SiparisSevkiyatAksesuar
    {
        public Aksesuar Aksesuar
        {
            get
            {
                AksesuarRepo aksesuarRepo = new AksesuarRepo();
                Aksesuar aksesuar = aksesuarRepo.FindBy(e => e.Id == AksesuarId).FirstOrDefault();
                if (aksesuar != null)
                    return aksesuar;
                else
                    return null;
            }
        }
        public Kullanici Kullanici
        {
            get
            {
                KullaniciRepo kullaniciRepo = new KullaniciRepo();
                return kullaniciRepo.FindBy(e => e.Id == KullaniciId).FirstOrDefault();
            }
        }
    }
}
