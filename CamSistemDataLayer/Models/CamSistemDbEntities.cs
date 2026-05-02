using System;

namespace CamSistemDataLayer.Models
{
    // Projede eski isim CamSistemDbEntities bekleniyorsa,
    // yeni oluşturulan context (CamSistemModel) üzerine alias sağlarız.
    public partial class CamSistemDbEntities : CamSistemModel
    {
        public CamSistemDbEntities() : base()
        {
        }
    }
}