using CamSistemDataLayer.Models;
using CamSistemDataLayer.Models.GenericRepo;
using System.ComponentModel;

namespace CamSistemDataLayer.Repos
{
    [DataObject(true)]
    public class TedarikRepo : GenericRepository<Tedarikci>
    {
    }
}
