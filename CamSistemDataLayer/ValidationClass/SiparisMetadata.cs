using System.ComponentModel.DataAnnotations;

namespace CamSistemDataLayer.ValidationClass
{
    public class SiparisMetadata
    {
        [Required(ErrorMessage = "Bu alan zorunludur !")]
        public string GirilenEn { get; set; }
        
        [Required(ErrorMessage = "Bu alan zorunludur !")]
        public string GirilenBoy { get; set; }
        
    }

    [MetadataType(typeof(SiparisMetadata))]
    public partial class Siparis
    {
    }
}
