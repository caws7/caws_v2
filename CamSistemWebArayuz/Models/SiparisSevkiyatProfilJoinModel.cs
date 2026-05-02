using CamSistemDataLayer.Models;
using System.Collections.Generic;

namespace CamSistemWebArayuz.Models
{
    public class SiparisSevkiyatProfilJoinModel
    {
        public Sevkiyat Sevkiyat { get; set; }
        public Siparis Siparis { get; set; }
        public List<SiparisSevkiyatProfil> SevkiyatProfil { get; set; }
        public List<SiparisSevkiyatAksesuar> SevkiyatAksesuar { get; set; }
        public List<SevkiyatPaket> SevkiyatPaket { get; set; }
    }
}