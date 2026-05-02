using CamSistemDataLayer.Models;
using System.Collections.Generic;

namespace CamSistemWebArayuz.Models
{
    public class TanimalamalarModel
    {
        public IEnumerable<Sistem> SistemModel { get; set; }
        public IEnumerable<AltSistem> AltSistemModel { get; set; }
        public IEnumerable<SistemTur> SistemTurModel { get; set; }
        public IEnumerable<SistemAltSistemJoin> SistemJoinModel { get; set; }
    }

}