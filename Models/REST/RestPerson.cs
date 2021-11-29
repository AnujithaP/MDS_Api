using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MDSServiceApp.Models.REST
{
    public class RestPerson
    {
        [JsonPropertyName("förnamn")]
        public string Förnamn { get; set; }
        [JsonPropertyName("efternamn")]
        public string Efternamn { get; set; }
        [JsonPropertyName("personnummer")]
        public string Personnummer { get; set; }
    }
}
