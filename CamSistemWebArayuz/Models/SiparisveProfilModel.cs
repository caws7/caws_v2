using CamSistemDataLayer.Models;
using System.Collections.Generic;

namespace CamSistemWebArayuz.Models
{
    public class SiparisveProfilModel
    {
        public Siparis SiparisModel { get; set; }
        public CamDetayBilgileri CamDetay { get; set; }
        public IEnumerable<CamBilgileri> CamBilgi { get; set; }
        public IEnumerable<Profil> ProfilModel { get; set; }
        public IEnumerable<Aksesuar> AksesuarModel { get; set; }
        public IEnumerable<SiparisAksesuar> DetayModel { get; set; }
        public IEnumerable<Siparis> SiparisListModel { get; set; }
    }
}