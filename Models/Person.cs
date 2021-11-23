using System;
using System.Text.Json.Serialization;

namespace MDSServiceApp.Models
{
    public class Person
    {
        [JsonPropertyName("uniktId")]
        public Guid? UniktId { get; set; }
        [JsonPropertyName("namn")]
        public string Namn { get; set; }
        [JsonPropertyName("gender")]
        public string Gender { get; set; }
        [JsonPropertyName("personnummer")]
        public string Personnummer { get; set; }
        [JsonPropertyName("samordningsnummer")]
        public string Samordningsnummer { get; set; }
        [JsonPropertyName("födelsedatum")]
        public string Födelsedatum { get; set; }
        [JsonPropertyName("förnamn")]
        public string Förnamn { get; set; }
        [JsonPropertyName("givetnamnmarkör")]
        public double? Givetnamnmarkör { get; set; }
        [JsonPropertyName("givetnamn")]
        public string Givetnamn { get; set; }
        [JsonPropertyName("mellannamn")]
        public string Mellannamn { get; set; }
        [JsonPropertyName("efternamn")]
        public string Efternamn { get; set; }
        [JsonPropertyName("dödsdatum")]
        public DateTime? Dödsdatum { get; set; }
        [JsonPropertyName("jobbmail")]
        public string Jobbmail { get; set; }
        [JsonPropertyName("privatmail")]
        public string Privatmail { get; set; }
        [JsonPropertyName("mobilnummer")]
        public double? Mobilnummer { get; set; }
        [JsonPropertyName("telefonnummer")]
        public double? Telefonnummer { get; set; }
        [JsonPropertyName("emigrerad")]
        public string Emigrerad { get; set; }
        [JsonPropertyName("skyddadperson")]
        public string Skyddadperson { get; set; }
        [JsonPropertyName("postadress")]
        public Guid? Postadress { get; set; }
        [JsonPropertyName("specialadress")]
        public Guid? Specialadress { get; set; }
        [JsonPropertyName("utlandsadress")]
        public Guid? Utlandsadress { get; set; }
        [JsonPropertyName("län")]
        public County Län { get; set; }
        [JsonPropertyName("kommun")]
        public Municipality Kommun { get; set; }
        [JsonPropertyName("skyddadpersontyp")]
        public string Skyddadpersontyp { get; set; }
    }
}
