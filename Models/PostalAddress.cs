using System;

namespace MDSServiceApp.Models
{
    public class PostalAddress
    {
        public string AdressTyp { get; set; }
        public string UtdelningsAdress1 { get; set; }
        public string UtdelningsAdress2 { get; set; }
        public string CareOf { get; set; }
        public string PostNummer { get; set; }
        public string PostOrt { get; set; }
        public string UtdelningsAdress3 { get; set; }
        public string LandKod { get; set; }

        public DateTime? DatumFörUtlandsresa { get; set; }
        public DateTime? DatumFörRösträtt { get; set; }
    }
}
