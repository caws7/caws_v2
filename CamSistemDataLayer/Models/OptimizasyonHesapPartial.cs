using System;
using CamSistemDataLayer.Repos;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class OptimizasyonHesap
    {
        public Profil profil
        {
            get
            {
                try
                {
                    if (ProfilId == null) return null;
                    ProfilRepo profilRepo = new ProfilRepo();
                    return profilRepo.FindBy(e => e.Id == ProfilId).FirstOrDefault();
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("[OptimizasyonHesap.profil] Hata ProfilId=" + ProfilId + ": " + ex.Message); return null; }
            }
        }
    }
}
