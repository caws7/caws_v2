using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{

    public partial class SistemAltSistemJoin
    {
        public string SistemAdi
        {
            get
            {
                SistemRepo sRepo = new SistemRepo();
                return sRepo.FindBy(e => e.Id == SistemId).FirstOrDefault().SistemAdi;
            }
        }
        public string SistemTurAdi
        {
            get
            {
                SistemTurRepo stRepo = new SistemTurRepo();
                SistemTur sistemTur = stRepo.FindBy(e => e.Id == SistemTurId).FirstOrDefault();
                return sistemTur == null ? "" : sistemTur.TurAdi;
            }
        }
        public string AltSistemAdi
        {
            get
            {
                AltSistemRepo asRepo = new AltSistemRepo();
                AltSistem altSistem = asRepo.FindBy(e => e.Id == AltSistemId).FirstOrDefault();
                return altSistem == null ? "" : altSistem.AltSistemAdi;
            }
        }
    }
}
