using CamSistemDataLayer.Models;
using CamSistemDataLayer.Models.GenericRepo;
using System;
using System.ComponentModel;

namespace CamSistemDataLayer.Repos
{
    [DataObject(true)]
    public class SiparisAksesuarRepo : GenericRepository<SiparisAksesuar>
    {
        public static implicit operator SiparisAksesuarRepo(SiparisAksesuar v)
        {
            throw new NotImplementedException();
        }
    }
}
