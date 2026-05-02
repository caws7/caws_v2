namespace CamSistemWebArayuz.Models.Sistemler
{
    public class Demonte
    {
        public OrtakAlanlar ortak { get; set; }
        public string Motor { get; set; }
        public string Kumanda { get; set; }
        public string CamKombinasyon { get; set; }
        public string BaglantiSistem { get; set; }
        public string AksesuarSet { get; set; }
        public int En { get; set; }
        public int Boy { get; set; }
    }
}