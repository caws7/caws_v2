using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class BoyaTedarik
    {
        public string TedarikciAdi
        {
            get
            {
                TedarikRepo tedarikRepo = new TedarikRepo();
                return tedarikRepo.FindBy(e => e.Id == TedarikciId).FirstOrDefault().TedarikciAdi;
            }
        }
    }
}
