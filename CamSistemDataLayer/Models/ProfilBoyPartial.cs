using CamSistemDataLayer.Repos;
using System.Collections.Generic;
using System.Linq;

namespace CamSistemDataLayer.Models
{
    public partial class ProfilBoy
    {
        StokRepo sRepo;
        public int ToplamStokAdetProfilBoy
        {
            get
            {
                sRepo = new StokRepo();
                return (int)sRepo.FindBy(e => e.ProfilBoyId == Id).ToList().Sum(e => e.StokAdet);
            }
        }

        public int ToplamStokAdet
        {
            get
            {
                sRepo = new StokRepo();
                return (int)sRepo.GetAll().ToList().Sum(e => e.StokAdet);
            }
        }

        public string profilBoyBazindaProfilStok
        {
            get
            {
                sRepo = new StokRepo();
                SabitRepo sabitRepo = new SabitRepo();
                ProfilRepo profilRepo = new ProfilRepo();
                List<string> list = new List<string>();

                int sabit = (int)sabitRepo.FindBy(e => e.Id == 7).FirstOrDefault().SabitDeger;
                List<int> ids = sRepo.FindBy(e => e.ProfilBoyId == Id && e.StokAdet <= sabit).Select(e => (int)e.ProfilId).ToList();
                ids.ForEach(e => list.AddRange(profilRepo.FindBy(a => a.Id == e).Select(i => i.ProfilKodu).ToList()));

                return string.Join(", ", list);
            }
        }

        public string stoktaOlmayanProfiller
        {
            get
            {
                sRepo = new StokRepo();
                ProfilRepo profilRepo = new ProfilRepo();
                List<string> kodlar = new List<string>();
                List<int> profilIdsList = profilRepo.GetAll().Select(e => e.Id).ToList();
                List<int> boyaGoreProfilIds = sRepo.FindBy(e => e.ProfilBoyId == Id).Select(e => (int)e.ProfilId).ToList();
                List<int> olmayanlarIds = profilIdsList.Except(boyaGoreProfilIds).ToList();
                olmayanlarIds.ForEach(e => kodlar.AddRange(profilRepo.FindBy(a => a.Id == e).Select(i => i.ProfilKodu)));

                return string.Join(", ", kodlar);
            }
        }
    }
}
