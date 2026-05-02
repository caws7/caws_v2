using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CamSistemWebArayuz.Models
{
    public class SiparisAksesuarFiyatModelList
    {
        public List<SiparisAksesuarFiyatModel> siparisAksesuarFiyatModels { get; set; }
    }
    public class SiparisAksesuarFiyatModel
    {
        public long siparisId { get; set; }
        public int siparisAksesuarId { get; set; }
        public string malzeme { get; set; }
        public string birim { get; set; }
        public decimal birimFiyat { get; set; }
    }
}