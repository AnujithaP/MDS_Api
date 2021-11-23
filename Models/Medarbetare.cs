using System;
using System.Text.Json.Serialization;

namespace MDSServiceApp.Models
{
    public class Medarbetare
    {
        // Namn
        [JsonPropertyName("namn")]
        public string Namn { get; set; }

        // Unique Identity
        [JsonPropertyName("uniktId")]
        public string UniktId { get; set; }

        // Title
        [JsonPropertyName("titel")]
        public string Titel { get; set; }

        // MobileNumberWork
        [JsonPropertyName("mobilnummerArbete")]
        public double MobilnummerArbete { get; set; }

        // DirectTelephone
        [JsonPropertyName("direktTelefon")]
        public string DirektTelefon { get; set; }

        // ExternalEmail
        [JsonPropertyName("epostAdressExtern")]
        public string EpostAdressExtern { get; set; }

        // WorkEmail
        [JsonPropertyName("epostadressArbete")]
        public string EpostAdressArbete { get; set; }

        // PassportNumber
        [JsonPropertyName("passnummer")]
        public string Passnummer { get; set; }

        // PassportExpiryDate
        [JsonPropertyName("passGiltigtill")]
        public DateTime? PassGiltigtill { get; set; }

        // UserAccount
        [JsonPropertyName("användarKonto")]
        public string AnvändarKonto { get; set; }

        // StartDate
        [JsonPropertyName("startdatum")]
        public DateTime? Startdatum { get; set; }

        // EndDate
        [JsonPropertyName("slutdatum")]
        public DateTime? Slutdatum { get; set; }

        // HSA-ID
        [JsonPropertyName("hsaId")]
        public string HSAId { get; set; }

        // PrescriptionCode
        [JsonPropertyName("förskrivarkod")]
        public double Förskrivarkod { get; set; }

        // MedicalAdvisor
        [JsonPropertyName("medicinskRådgivare")]
        public string MedicinskRådgivare { get; set; }

        // HOSP_ID
        [JsonPropertyName("hosp_Id")]
        public double HOSP_ID { get; set; }

        // Co-WorkerType
        [JsonPropertyName("medarbetarTyp")]
        public MedarbetarTyp MedarbetarTyp { get; set; }

        // HomeOrganisation
        [JsonPropertyName("organisationsTillhörighet")]
        public string OrganisationsTillhörighet { get; set; }

        // OrganisationEnhet
        [JsonPropertyName("organisatoriskEnhetstillhörighet")]
        public string OrganisatoriskEnhetstillhörighet { get; set; }

        // FormOfEmployment
        [JsonPropertyName("anställningsForm")]
        public string  AnställningsForm { get; set; }

        // Label
        [JsonPropertyName("etikett")]
        public Etikett Etikett { get; set; }

        // WorkCategory
        [JsonPropertyName("yrkeskategori")]
        public YrkesKategori Yrkeskategori { get; set; }

        // ManagerCode
        [JsonPropertyName("chefskod")]
        public Chefskod Chefskod { get; set; }

        [JsonPropertyName("ansvarigChef")]
        public string AnsvarigChef { get; set; }

        // ExtendedWorkCode
        [JsonPropertyName("utökadYrkeskod")]
        public UtökadYrkeskod UtökadYrkeskod { get; set; }

        [JsonPropertyName("gruppförskrivarkod")]
        public string Gruppförskrivarkod { get; set; }

        // Person (borde vara Guid?)
        [JsonPropertyName("person")]
        public string Person { get; set; }

        // Legitimation
        [JsonPropertyName("legitimation")]
        public Legitimation Legitimation { get; set; }
    }
}
