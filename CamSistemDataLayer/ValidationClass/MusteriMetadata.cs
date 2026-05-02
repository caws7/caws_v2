using System.ComponentModel.DataAnnotations;

namespace CamSistemDataLayer.ValidationClass
{
    public class MusteriMetadata
    {
        [Required(ErrorMessage = "Bu alan zorunludur!")]
        public string MusteriAdi { get; set; }

        [Required(ErrorMessage = "Bu alan zorunludur!")]
        public string MusteriSoyadi { get; set; }

    }

    [MetadataType(typeof(MusteriMetadata))]
    public partial class Musteri
    {
    }
}
