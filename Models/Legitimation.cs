using System;

namespace MDSServiceApp.Models
{
    public class Legitimation
    {
        public Guid Id { get; set; }
        public string Namn { get; set; }
        public string LegitimeratYrke { get; set; }
        public string GiltigLegitimation { get; set; }
        public string Utbildning { get; set; }
        public DateTime? LegitimationsDatum { get; set; }
        public string Förskrivningsrätt { get; set; }
        public string FörskrivningsrättDatum { get; set; }
        public string FysioterapeutBeslutdatum { get; set; }
    }
}
