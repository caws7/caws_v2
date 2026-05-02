using System;
using System.ComponentModel.DataAnnotations;

namespace CamSistemDataLayer.ValidationClass
{
    public class DisSiparisMetadata
    {
        [Display(Name ="Tutar Giriniz...")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode =true)]
        public decimal TedarikFiyat { get; set; }

        public int Id { get; set; }
        public long MusteriId { get; set; }
        public string FisNo { get; set; }
        public int DurumId { get; set; }
        public string SistemBilgisi { get; set; }
        public string SistemAciklamasi { get; set; }
        public string IslemDurum { get; set; }
        public string GenelAciklama { get; set; }
        public DateTime OnayIptalTarihi { get; set; }
        public int OnayIptalKullaniciId { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public int TedarikciId { get; set; }
    }

    [MetadataType(typeof(DisSiparisMetadata))]
    public partial class DisSiparis
    {
    }
}
