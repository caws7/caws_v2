using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    partial class DisTedarik
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
