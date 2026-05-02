using CamSistemDataLayer.Models;
using System.Collections.Generic;

namespace CamSistemWebArayuz.Models
{
    public class KullaniciRolModel
    {
        public IEnumerable<KullaniciRol> KullaniciRols { get; set; }
        public IEnumerable<Rol> Rols { get; set; }
    }
}