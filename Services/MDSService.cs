using MDS;
using MDSServiceApp.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSServiceApp.Services
{
    public class MDSService : IMDSService
    {
        private readonly ServiceClient _service;
        private readonly IConfiguration _configuration;
        private readonly string _accountName;
        private readonly string _accountPassword;
        private readonly string _modelName1;
        private readonly string _modelName2;
        private readonly string _versionName;

        private int LastError { get; set; }

        private enum PersonAttributes : int
        {
            Gender = 0,
            SocialSecurityNumber,
            Coordinationnumber,
            Dateofbirth,
            FirstName,
            GivenNameMarker,
            GivenName,
            MiddleName,
            LastName,
            DateofDeath,
            WorkEmail,
            PrivateEmail,
            PrivateCellPhoneNumber,
            PrivatePhoneNumber,
            EmigratedPerson,
            ProtectedPerson,
            PostAddress,
            SpecialAddress,
            AbroadAddress,
            County,
            Municipality,
            ProtectedPersonType
        }

        private string[] PersonAttributeNames = new string[] {
            "Gender",
            "Social Security Number",
            "Co-ordination number",
            "Date of birth",
            "First Name",
            "Given Name Marker",
            "Given Name",
            "Middle Name",
            "Last Name",
            "Date of Death",
            "Work Email",
            "Private Email",
            "Private Cell Phone Number",
            "Private Phone Number",
            "Emigrated Person",
            "Protected Person",
            "PostAddress",
            "SpecialAddress",
            "AbroadAddress",
            "County",
            "Municipality",
            "ProtectedPersonType"
            };

        private enum CoWorkerAttributes : int
        {
            Title = 0,
            MobileNumberWork,
            DirectTelephone,
            ExternalEmail,
            WorkEmail,
            PassportNumber,
            PassportExpiryDate,
            UserAccount,
            StartDate,
            EndDate,
            HSAID,
            PrescriptionCode,
            MedicalAdvisor,
            HOSP_ID,
            CoWorkerType,
            HomeOrganisation,
            OrganisationEnhet,
            FormOfEmployment,
            Label,
            WorkCategory,
            ManagerCode,
            ResponsibleManager,
            ExtendedWorkCode,
            Person,
            Legitimation
        }

        private enum LegitimationAttributes : int
        {
            RegisteredProfession = 0,
            ValidRegistration,
            Education,
            RegistrationDate,
            PrescriptionRights,
            PrescriptionRightsDate,
            DecisionDatePhysio
        }

        public MDSService(IConfiguration configuration)
        {
            _configuration = configuration;
            _accountName = _configuration["MDSAccount:AccountName"];
            _accountPassword = _configuration["MDSAccount:Password"];
            _modelName1 = _configuration["MDSModels:MDSModel1"];
            _modelName2 = _configuration["MDSModels:MDSModel2"];
            _versionName = _configuration["MDSVersion"];

            _service = new ServiceClient();
            _service.ClientCredentials.Windows.ClientCredential.UserName = _accountName;
            _service.ClientCredentials.Windows.ClientCredential.Password = _accountPassword;

            _service.OpenAsync().Wait();

            LastError = 0;
        }


        public int GetLastError()
        {
            return LastError;
        }

        public async Task<Person> GetPerson(Guid personId)
        {
            var person = new Person();
            var member = await GetEntityMemberByCode("Person", MemberType.Leaf, personId.ToString());
            if (member != null)
            {
                var fdatum = (string)member.Attributes[(int)PersonAttributes.Dateofbirth].Value;
                person.UniktId = new Guid(member.MemberId.Code);
                person.Efternamn = (string)member.Attributes[(int)PersonAttributes.LastName].Value;
                person.Förnamn = (string)member.Attributes[(int)PersonAttributes.FirstName].Value;
                person.Mellannamn = (string)member.Attributes[(int)PersonAttributes.MiddleName].Value;
                person.Personnummer = (string)member.Attributes[(int)PersonAttributes.SocialSecurityNumber].Value;
                person.Födelsedatum = string.Format("{0}-{1}-{2}", fdatum.Substring(0, 4), fdatum.Substring(4, 2), fdatum.Substring(6, 2));
                person.Jobbmail = (string)member.Attributes[(int)PersonAttributes.WorkEmail].Value;
                person.Privatmail = (string)member.Attributes[(int)PersonAttributes.PrivateEmail].Value;
                person.Mobilnummer = (double)member.Attributes[(int)PersonAttributes.PrivateCellPhoneNumber].Value;
            }

            return person;
        }

        private async Task<Legitimation> GetLegitimation(string LegitimationId)
        {
            var leg = new Legitimation();
            var member = await GetEntityMemberByCode("Legitimation", MemberType.Leaf, LegitimationId);
            if (member != null)
            {
                leg.Namn = member.MemberId.Name;
                leg.Id = Guid.Parse(member.MemberId.Code);
                leg.LegitimeratYrke = (string)member.Attributes[(int)LegitimationAttributes.RegisteredProfession].Value;
                leg.GiltigLegitimation = (string)member.Attributes[(int)LegitimationAttributes.ValidRegistration].Value;
                leg.Utbildning = (string)member.Attributes[(int)LegitimationAttributes.Education].Value;
                leg.LegitimationsDatum = (DateTime?)member.Attributes[(int)LegitimationAttributes.RegistrationDate].Value;
                leg.Förskrivningsrätt = (string)member.Attributes[(int)LegitimationAttributes.PrescriptionRights].Value;
                leg.FörskrivningsrättDatum = (string)member.Attributes[(int)LegitimationAttributes.PrescriptionRightsDate].Value;
            }

            return leg;
        }

        public async Task<Person> AddPerson(Person person)
        {
            var createRequest = new EntityMembersCreateRequest() { };
            createRequest.Members = new EntityMembers() { };
            createRequest.ReturnCreatedIdentifiers = true;

            // Set the modelId, versionId, and entityId.
            createRequest.Members.ModelId = new Identifier { Name = _modelName1 };
            createRequest.Members.VersionId = new Identifier { Name = _versionName };
            createRequest.Members.EntityId = new Identifier { Name = "Person" };
            createRequest.Members.MemberType = MemberType.Leaf;
            createRequest.Members.Members = new List<Member>() { };

            var aNewMember = new Member() { };
            aNewMember.MemberId = new MemberIdentifier() { Name = person.Efternamn + ", " + person.Förnamn, Code = Guid.NewGuid().ToString(), MemberType = MemberType.Leaf };
            aNewMember.Attributes = new List<MDS.Attribute>() { };

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.Gender], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Gender ?? string.Empty
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.SocialSecurityNumber], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Personnummer ?? string.Empty
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.Coordinationnumber], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Samordningsnummer ?? string.Empty
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.Dateofbirth], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Födelsedatum ?? string.Empty
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.FirstName], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Förnamn ?? string.Empty
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.GivenNameMarker], Id = Guid.NewGuid() },
                    Type = AttributeValueType.Number,
                    Value = person.Givetnamnmarkör ?? 0
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.GivenName], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Givetnamn ?? string.Empty
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.MiddleName], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Mellannamn ?? string.Empty
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.LastName], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Efternamn ?? string.Empty
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.DateofDeath], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Dödsdatum
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.WorkEmail], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Jobbmail ?? string.Empty
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.PrivateEmail], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Privatmail ?? string.Empty
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.PrivateCellPhoneNumber], Id = Guid.NewGuid() },
                    Type = AttributeValueType.Number,
                    Value = person.Mobilnummer
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.PrivatePhoneNumber], Id = Guid.NewGuid() },
                    Type = AttributeValueType.Number,
                    Value = person.Telefonnummer
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.EmigratedPerson], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Emigrerad
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.ProtectedPerson], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Skyddadperson
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.PostAddress], Id = Guid.NewGuid() },
                    Type = AttributeValueType.Domain,
                    Value = person.Postadress
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.SpecialAddress], Id = Guid.NewGuid() },
                    Type = AttributeValueType.Domain,
                    Value = person.Postadress
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.AbroadAddress], Id = Guid.NewGuid() },
                    Type = AttributeValueType.Domain,
                    Value = person.Postadress
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.County], Id = Guid.NewGuid() },
                    Type = AttributeValueType.Domain,
                    Value = new MemberIdentifier() { Name = person.Län?.Lännamn, Code = person.Län?.Länkod }
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.Municipality], Id = Guid.NewGuid() },
                    Type = AttributeValueType.Domain,
                    Value = new MemberIdentifier() { Name = person.Kommun?.Kommunnamn, Code = person.Kommun?.Kommunkod }
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = PersonAttributeNames[(int)PersonAttributes.ProtectedPersonType], Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = person.Skyddadpersontyp
                });

            createRequest.Members.Members.Add(aNewMember);

            // Create a new entity member
            EntityMembersCreateResponse createResponse = await _service.EntityMembersCreateAsync(createRequest);
            var newEntityMember = createResponse.CreatedMembers.FirstOrDefault();
            LastError = createResponse.OperationResult.Errors.Count;

            return person;
        }

        public async Task UpdatePerson(Person person)
        {
            // await EntityMetadataUpdatePerson(person);

            // Create the request object to get the entity information.
            var getRequest = new EntityMembersGetRequest() { };
            getRequest.MembersGetCriteria = new EntityMembersGetCriteria() { };

            // Set the modelId, versionId, entityId, and the member code.
            getRequest.MembersGetCriteria.ModelId = new Identifier { Name = _modelName1 };
            getRequest.MembersGetCriteria.VersionId = new Identifier { Name = _versionName };
            getRequest.MembersGetCriteria.EntityId = new Identifier { Name = "Person" };
            getRequest.MembersGetCriteria.MemberType = MemberType.Leaf;
            getRequest.MembersGetCriteria.MemberReturnOption = MemberReturnOption.Data;
            getRequest.MembersGetCriteria.SearchTerm = "Code = '" + person.UniktId.ToString() + "'";

            // Get the entity member information
            EntityMembersGetResponse getResponse = await _service.EntityMembersGetAsync(getRequest);

            // Get the entity member identifier.
            var member = getResponse.EntityMembers.Members.FirstOrDefault();
            if (member != null)
            {
                UpdateEntityMember(_modelName1, _versionName, "Person", person, member);
            }
        }

        private async Task<Member> GetEntityMemberByCode(string entityName, MemberType memberType, string code)
        {

            // Create the request object to get the entity information.
            var getRequest = new EntityMembersGetRequest() { };
            getRequest.MembersGetCriteria = new EntityMembersGetCriteria() { };

            // Set the modelId, versionId, entityId, and the member code.
            getRequest.MembersGetCriteria.ModelId = new Identifier { Name = _modelName1 };
            getRequest.MembersGetCriteria.VersionId = new Identifier { Name = _versionName };
            getRequest.MembersGetCriteria.EntityId = new Identifier { Name = entityName };
            getRequest.MembersGetCriteria.MemberType = memberType;
            getRequest.MembersGetCriteria.MemberReturnOption = MemberReturnOption.Data;
            getRequest.MembersGetCriteria.SearchTerm = "Code = '" + code + "'";

            // Get the entity member information
            EntityMembersGetResponse getResponse = await _service.EntityMembersGetAsync(getRequest);

            // Get the entity member identifier.
            var member = getResponse.EntityMembers.Members.FirstOrDefault();

            return member;
        }


        private void UpdateEntityMember(string modelName, string versionName, string entityName, Person person, Member aMember)
        {
            // Create the request object for entity update.
            var updateRequest = new EntityMembersUpdateRequest() { };
            updateRequest.Members = new EntityMembers() { };

            // Set the modelName, the versionName, and the entityName.
            updateRequest.Members.ModelId = new Identifier { Name = modelName };
            updateRequest.Members.VersionId = new Identifier { Name = versionName };
            updateRequest.Members.EntityId = new Identifier { Name = entityName };
            updateRequest.Members.MemberType = MemberType.Leaf;
            updateRequest.Members.Members = new List<Member>() { aMember };

            // Update the attributes. 
            aMember.Attributes[(int)PersonAttributes.Gender].Value = person.Gender;
            aMember.Attributes[(int)PersonAttributes.GivenName].Value = person.Givetnamn;
            aMember.Attributes[(int)PersonAttributes.GivenNameMarker].Value = person.Givetnamnmarkör;
            aMember.Attributes[(int)PersonAttributes.SocialSecurityNumber].Value = person.Personnummer;
            aMember.Attributes[(int)PersonAttributes.Dateofbirth].Value = person.Födelsedatum;
            aMember.Attributes[(int)PersonAttributes.FirstName].Value = person.Förnamn;
            aMember.Attributes[(int)PersonAttributes.MiddleName].Value = person.Mellannamn;
            aMember.Attributes[(int)PersonAttributes.LastName].Value = person.Efternamn;
            aMember.Attributes[(int)PersonAttributes.WorkEmail].Value = person.Jobbmail;
            aMember.Attributes[(int)PersonAttributes.PrivateEmail].Value = person.Privatmail;
            aMember.Attributes[(int)PersonAttributes.PrivateCellPhoneNumber].Value = person.Mobilnummer;
            ((MemberIdentifier)aMember.Attributes[(int)PersonAttributes.County].Value).Code = person.Län.Länkod;
            ((MemberIdentifier)aMember.Attributes[(int)PersonAttributes.County].Value).Name = person.Län.Lännamn;

            EntityMembersUpdateResponse updateResponse = _service.EntityMembersUpdate(updateRequest);
            LastError = updateResponse.OperationResult.Errors.Count;
        }

        private async Task<IEnumerable<EntityMembers>> EntityMetadataGet(string modelname, string entityName)
        {
            var members = new List<EntityMembers>() { };

            var metaFilter = new MDS.MetadataResultOptions()
            {
                Models = MDS.ResultType.Identifiers
            };

            var metadatareq = new MDS.MetadataGetRequest(new MDS.International(), Guid.NewGuid(), metaFilter, new MDS.MetadataSearchCriteria());
            var metadata = await _service.MetadataGetAsync(metadatareq);
            
            LastError = metadata.OperationResult.Errors.Count;

            if (LastError == 0)     // Has user permission?
            {
                var modelsFilter = new List<MDS.Identifier>()
                {
                    new MDS.Identifier()
                    {
                        Id = metadata.Metadata.Models.Where(m => m.Identifier.Name == modelname).FirstOrDefault().Identifier.Id
                    }
                };

                var modelEntitiesFilter = new List<MDS.Identifier>()
                {
                    new MDS.Identifier()
                    {
                        Name = entityName
                    }
                };

                var modelmembersreq = new MDS.ModelMembersGetRequest()
                {
                    International = new MDS.International(),
                    ModelsGetCriteria = new MDS.ModelMembersGetCriteria()
                    {
                        Models = modelsFilter,
                        Entities = modelEntitiesFilter,
                    },
                    ModelsResultCriteria = new MDS.ModelMembersResultCriteria()
                    {
                        IncludeCollectionMembers = true,
                        IncludeConsolidatedMembers = true,
                        IncludeLeafMembers = true
                    },
                    RequestId = Guid.NewGuid()
                };

                var response = await _service.ModelMembersGetAsync(modelmembersreq);
                members = response.ModelMembers;

                LastError = response.OperationResult.Errors.Count;
            }

            return members;
        }

        public async Task<IEnumerable<Organisation>> GetOrganisations()
        {
            var models = await EntityMetadataGet("Organisation", "Organisation");

            var orglist = new List<Organisation>() { };

            var members = models.FirstOrDefault().Members;

            foreach (var member in members)
            {
                orglist.Add(new Organisation() { OrganisationsNamn = member.MemberId.Name, OrganisationsNummer = member.MemberId.Code });
            }

            return orglist;
        }

        public async Task<IEnumerable<Person>> GetPersoner()
        {
            var models = await EntityMetadataGet("Person och Medarbetare", "Person");

            var people = new List<Person>() { };

            var members = models.FirstOrDefault().Members;

            foreach (var member in members)
            {
                var person = new Person()
                {
                    UniktId = new Guid(member.MemberId.Code),
                    Namn = member.MemberId.Name,
                    Gender = (string)member.Attributes[(int)PersonAttributes.Gender].Value,
                    Givetnamn = (string)member.Attributes[(int)PersonAttributes.GivenName].Value,
                    Givetnamnmarkör = (double?)member.Attributes[(int)PersonAttributes.GivenNameMarker].Value,

                    Kommun = new Municipality() { 
                        Kommunnamn = ((MemberIdentifier)member.Attributes[(int)PersonAttributes.Municipality].Value).Name,
                        Kommunkod = ((MemberIdentifier)member.Attributes[(int)PersonAttributes.Municipality].Value).Code,
                        Länkod = ((MemberIdentifier)member.Attributes[(int)PersonAttributes.County].Value).Code ?? string.Empty
                    },

                    Län = new County() { 
                        Lännamn = ((MemberIdentifier)member.Attributes[(int)PersonAttributes.County].Value).Name??string.Empty,
                        Länkod = ((MemberIdentifier)member.Attributes[(int)PersonAttributes.County].Value).Code??string.Empty
                    },

                    Efternamn = (string)member.Attributes[(int)PersonAttributes.LastName].Value,
                    Förnamn = (string)member.Attributes[(int)PersonAttributes.FirstName].Value,
                    Mellannamn = (string)member.Attributes[(int)PersonAttributes.MiddleName].Value,
                    Personnummer = (string)member.Attributes[(int)PersonAttributes.SocialSecurityNumber].Value,
                    Födelsedatum = (string)member.Attributes[(int)PersonAttributes.Dateofbirth].Value,
                    Jobbmail = (string)member.Attributes[(int)PersonAttributes.WorkEmail].Value,
                    Privatmail = (string)member.Attributes[(int)PersonAttributes.PrivateEmail].Value,
                    Mobilnummer = (double?)member.Attributes[(int)PersonAttributes.PrivateCellPhoneNumber].Value
                };

                people.Add(person);
            }

            return people;
        }

        public async Task<Medarbetare> GetCoWorker(string coworkerId)
        {
            var medarbetare = new Medarbetare();
            var member = await GetEntityMemberByCode("Co-worker", MemberType.Leaf, coworkerId.ToString());
            if (member != null)
            {
                medarbetare.Namn = member.MemberId.Name;
                medarbetare.UniktId = member.MemberId.Code;
                medarbetare.Titel = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.Title]);
                medarbetare.AnställningsForm = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.FormOfEmployment]);
                MemberIdentifier etikett = (MemberIdentifier)member.Attributes[(int)CoWorkerAttributes.Label].Value;
                medarbetare.Etikett = new Etikett() { Id = etikett.Id, EtikettNamn = etikett.Name };
                MemberIdentifier yrkeskategori = (MemberIdentifier)member.Attributes[(int)CoWorkerAttributes.WorkCategory].Value;
                medarbetare.Yrkeskategori = new YrkesKategori() { Id = yrkeskategori.Id, YrkeskategoriNamn = yrkeskategori.Name };
                MemberIdentifier chefskod = (MemberIdentifier)member.Attributes[(int)CoWorkerAttributes.ManagerCode].Value;
                medarbetare.Chefskod = new Chefskod() { Id = chefskod.Id, Befogenheter = chefskod.Name };
                medarbetare.MobilnummerArbete = (double)GetValue(member.Attributes[(int)CoWorkerAttributes.MobileNumberWork]);
                medarbetare.DirektTelefon = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.DirectTelephone]);
                medarbetare.EpostAdressExtern = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.ExternalEmail]);
                medarbetare.EpostAdressArbete = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.WorkEmail]);
                medarbetare.Passnummer = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.PassportNumber]);
                medarbetare.PassGiltigtill = (DateTime?)GetValue(member.Attributes[(int)CoWorkerAttributes.PassportExpiryDate]);
                medarbetare.AnvändarKonto = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.UserAccount]);
                medarbetare.Startdatum = (DateTime?)GetValue(member.Attributes[(int)CoWorkerAttributes.StartDate]);
                medarbetare.Slutdatum = (DateTime?)GetValue(member.Attributes[(int)CoWorkerAttributes.EndDate]);
                medarbetare.HSAId = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.HSAID]);
                medarbetare.Förskrivarkod = (double)member.Attributes[(int)CoWorkerAttributes.PrescriptionCode].Value;
                medarbetare.MedicinskRådgivare = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.MedicalAdvisor]);
                medarbetare.OrganisationsTillhörighet = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.HomeOrganisation]);
                medarbetare.HOSP_ID = (double)GetValue(member.Attributes[(int)CoWorkerAttributes.HOSP_ID]);
                MemberIdentifier utökadyrkeskod = (MemberIdentifier)member.Attributes[(int)CoWorkerAttributes.ExtendedWorkCode].Value;
                medarbetare.UtökadYrkeskod = new UtökadYrkeskod() { Id = utökadyrkeskod.Id, Benämning = utökadyrkeskod.Name };
                medarbetare.Person = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.Person]);
                medarbetare.Legitimation = new Legitimation();  // TODO: fixa detta objekt
            }

            return medarbetare;
        }

        public async Task<Medarbetare> AddCoworker(Medarbetare medarbetare)
        {
            // Create the request object for entity creation.
            var createRequest = new EntityMembersCreateRequest() { };
            createRequest.Members = new EntityMembers() { };
            createRequest.ReturnCreatedIdentifiers = true;

            // Set the modelId, versionId, and entityId.
            createRequest.Members.ModelId = new Identifier { Name = _modelName1 };
            createRequest.Members.VersionId = new Identifier { Name = _versionName };
            createRequest.Members.EntityId = new Identifier { Name = "Co-worker" };
            createRequest.Members.MemberType = MemberType.Leaf;
            createRequest.Members.Members = new List<Member>() { };

            var aNewMember = new Member() { };
            aNewMember.MemberId = new MemberIdentifier() { Name = medarbetare.Namn, Code = Guid.NewGuid().ToString(), MemberType = MemberType.Leaf };
            aNewMember.Attributes = new List<MDS.Attribute>() { };

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.Title), Id = Guid.NewGuid() },
                    Type = AttributeValueType.Number,
                    Value = medarbetare.Titel
                });

            aNewMember.Attributes.Add(
            new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.MobileNumberWork), Id = Guid.NewGuid() },
                    Type = AttributeValueType.Number,
                    Value = medarbetare.MobilnummerArbete
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.DirectTelephone), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.DirektTelefon
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.ExternalEmail), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.EpostAdressExtern
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.WorkEmail), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.EpostAdressArbete
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.PassportNumber), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.Passnummer
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.PassportExpiryDate), Id = Guid.NewGuid() },
                    Type = AttributeValueType.DateTime,
                    Value = medarbetare.PassGiltigtill
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.UserAccount), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.AnvändarKonto
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.StartDate), Id = Guid.NewGuid() },
                    Type = AttributeValueType.DateTime,
                    Value = medarbetare.Startdatum
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.EndDate), Id = Guid.NewGuid() },
                    Type = AttributeValueType.DateTime,
                    Value = medarbetare.Slutdatum
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = "HSA-ID", Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.HSAId
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.PrescriptionCode), Id = Guid.NewGuid() },
                    Type = AttributeValueType.Number,
                    Value = medarbetare.Förskrivarkod
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.MedicalAdvisor), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.MedicinskRådgivare
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.HOSP_ID), Id = Guid.NewGuid() },
                    Type = AttributeValueType.Number,
                    Value = medarbetare.HOSP_ID
                });

            //aNewMember.Attributes.Add(
            //    new MDS.Attribute()
            //    {
            //        Identifier = new Identifier() { Name = "Co-WorkerType", Id = Guid.NewGuid() },
            //        Type = AttributeValueType.Domain,
            //        Value = new MemberIdentifier() { Id = Guid.NewGuid(), Code = medarbetare.MedarbetarTyp.Id.ToString(), Name = medarbetare.MedarbetarTyp.Typ }
            //    });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.HomeOrganisation), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.OrganisationsTillhörighet
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.OrganisationEnhet), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.OrganisatoriskEnhetstillhörighet
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.FormOfEmployment), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.AnställningsForm
                });

            //aNewMember.Attributes.Add(
            //    new MDS.Attribute()
            //    {
            //        Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.Label), Id = Guid.NewGuid() },
            //        Type = AttributeValueType.Domain,
            //        Value = new MemberIdentifier() { Id = Guid.NewGuid(), Code = medarbetare.Etikett.Id.ToString(), Name = medarbetare.Etikett.EtikettNamn }
            //    });

            //aNewMember.Attributes.Add(
            //    new MDS.Attribute()
            //    {
            //        Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.WorkCategory), Id = Guid.NewGuid() },
            //        Type = AttributeValueType.Domain,
            //        Value = new MemberIdentifier() { Id = Guid.NewGuid(), Code = medarbetare.Yrkeskategori.Id.ToString(), Name = medarbetare.Yrkeskategori.YrkeskategoriNamn }
            //    });

            //aNewMember.Attributes.Add(
            //    new MDS.Attribute()
            //    {
            //        Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.ManagerCode), Id = Guid.NewGuid() },
            //        Type = AttributeValueType.Domain,
            //        Value = new MemberIdentifier() { Id = Guid.NewGuid(), Name = medarbetare.Chefskod.Befogenheter, Code = medarbetare.Chefskod.Id.ToString() }
            //    });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.ResponsibleManager), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.AnsvarigChef
                });

            //aNewMember.Attributes.Add(
            //    new MDS.Attribute()
            //    {
            //        Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.ExtendedWorkCode), Id = Guid.NewGuid() },
            //        Type = AttributeValueType.Domain,
            //        Value = new MemberIdentifier() { Name = medarbetare.UtökadYrkeskod.Benämning, Code = medarbetare.UtökadYrkeskod.Id.ToString(), Id = Guid.NewGuid() }
            //    });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.Person), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.Person
                });

            aNewMember.Attributes.Add(
                new MDS.Attribute()
                {
                    Identifier = new Identifier() { Name = Enum.GetName(CoWorkerAttributes.Legitimation), Id = Guid.NewGuid() },
                    Type = AttributeValueType.String,
                    Value = medarbetare.Legitimation != null ? medarbetare.Legitimation.Id.ToString() : null
                });


            createRequest.Members.Members.Add(aNewMember);

            // Create a new entity member
            EntityMembersCreateResponse createResponse = await _service.EntityMembersCreateAsync(createRequest);
            var newEntityMember = createResponse.CreatedMembers.FirstOrDefault();
            LastError = createResponse.OperationResult.Errors.Count;

            return medarbetare;
        }

        public async Task<IEnumerable<Medarbetare>> GetCoWorkers()
        {
            var models = await EntityMetadataGet("Person och Medarbetare", "Co-worker");

            var medarbetarlist = new List<Medarbetare>() { };

            var members = models.FirstOrDefault().Members;

            foreach (var member in members)
            {
                var medarbetare = new Medarbetare() { };

                medarbetare.Namn = member.MemberId.Name;
                medarbetare.UniktId = member.MemberId.Code;
                medarbetare.Titel = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.Title]);
                medarbetare.AnställningsForm = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.FormOfEmployment]);
                MemberIdentifier etikett = (MemberIdentifier)member.Attributes[(int)CoWorkerAttributes.Label].Value;
                medarbetare.Etikett = new Etikett() { Id = etikett.Id, EtikettNamn = etikett.Name };
                MemberIdentifier yrkeskategori = (MemberIdentifier)member.Attributes[(int)CoWorkerAttributes.WorkCategory].Value;
                medarbetare.Yrkeskategori = new YrkesKategori() { Id = yrkeskategori.Id, YrkeskategoriNamn = yrkeskategori.Name };
                MemberIdentifier chefskod = (MemberIdentifier)member.Attributes[(int)CoWorkerAttributes.ManagerCode].Value;
                medarbetare.Chefskod = new Chefskod() { Id = chefskod.Id, Befogenheter = chefskod.Name };
                medarbetare.MobilnummerArbete = (double)GetValue(member.Attributes[(int)CoWorkerAttributes.MobileNumberWork]);
                medarbetare.DirektTelefon = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.DirectTelephone]);
                medarbetare.EpostAdressExtern = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.ExternalEmail]);
                medarbetare.EpostAdressArbete = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.WorkEmail]);
                medarbetare.Passnummer = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.PassportNumber]);
                medarbetare.PassGiltigtill = (DateTime?)GetValue(member.Attributes[(int)CoWorkerAttributes.PassportExpiryDate]);
                medarbetare.AnvändarKonto = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.UserAccount]);
                medarbetare.Startdatum = (DateTime?)GetValue(member.Attributes[(int)CoWorkerAttributes.StartDate]);
                medarbetare.Slutdatum = (DateTime?)GetValue(member.Attributes[(int)CoWorkerAttributes.EndDate]);
                medarbetare.HSAId = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.HSAID]);
                medarbetare.Förskrivarkod = (double)member.Attributes[(int)CoWorkerAttributes.PrescriptionCode].Value;
                medarbetare.MedicinskRådgivare = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.MedicalAdvisor]);
                medarbetare.OrganisationsTillhörighet = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.HomeOrganisation]);
                medarbetare.HOSP_ID = (double)GetValue(member.Attributes[(int)CoWorkerAttributes.HOSP_ID]);
                MemberIdentifier utökadyrkeskod = (MemberIdentifier)member.Attributes[(int)CoWorkerAttributes.ExtendedWorkCode].Value;
                medarbetare.UtökadYrkeskod = new UtökadYrkeskod() { Id = utökadyrkeskod.Id, Benämning = utökadyrkeskod.Name };
                medarbetare.Person = (string)GetValue(member.Attributes[(int)CoWorkerAttributes.Person]);
                var leg = (MemberIdentifier)member.Attributes[(int)CoWorkerAttributes.Legitimation].Value;

                medarbetare.Legitimation = await GetLegitimation(leg.Code);

                medarbetarlist.Add(medarbetare);
            }

            return medarbetarlist;
        }

        private static object GetValue(MDS.Attribute attribute)
        {
            object result = null;
            if (attribute.Value != null)
            {
                // MDSSrv.MemberIdentifier
                if (attribute.Value is MemberIdentifier)
                {
                    var mi = (MemberIdentifier)attribute.Value;
                    if (mi.Name != null)
                    {
                        result = mi.Name;
                    }
                }
                else
                { 
                    result = attribute.Value;
                }
            }

            return result;
        }

        // Får inte denna att fungera, fel object
        public async Task<IEnumerable<User>> GetPermissions(string entityName, PermissionType permissionType, AccessPermissionType accessPermissionType)
        {
            _service.ClientCredentials.Windows.ClientCredential.UserName = _accountName;
            _service.ClientCredentials.Windows.ClientCredential.Password = _accountPassword;
            //_service.ClientCredentials.Windows.ClientCredential.UserName = "DESKTOP-HD2BG8E\\Anders Consid";
            //_service.ClientCredentials.Windows.ClientCredential.Password = "Heaven.7";

            await _service.OpenAsync();

            // Create the request object to get the entity information.
            var getRequest = new UserEffectiveObjectPermissionsGetRequest() { 
                International = new International(), 
                AccessPermission = accessPermissionType, 
                Permission = permissionType,
                RequestId = Guid.NewGuid(),
                ObjectType = ModelObjectType.Entity,
                ObjectId = new Identifier { Name = entityName }
            };

            UserEffectiveObjectPermissionsGetResponse response = await _service.UserEffectiveObjectPermissionsGetAsync(getRequest);

            var result = response.OperationResult;
            List<User> users = response.Users;

            _service.Close();

            return users;
        }

        public async Task<IEnumerable<User>> GetPrincipals()
        {
            List<User> users = null;

            try
            {
                // Gets security information.
                SecurityPrincipalsGetRequest principalGetRequest = new SecurityPrincipalsGetRequest();
                principalGetRequest.Criteria = new SecurityPrincipalsCriteria();
                principalGetRequest.Criteria.All = true;
                principalGetRequest.Criteria.Type = PrincipalType.UserAccount;
                principalGetRequest.Criteria.ResultType = ResultType.Details;
                principalGetRequest.Criteria.SecurityResolutionType = SecurityResolutionType.Users;
                principalGetRequest.Criteria.ModelPrivilege = ResultType.Details;
                principalGetRequest.Criteria.FunctionPrivilege = ResultType.Details;
                principalGetRequest.Criteria.HierarchyMemberPrivilege = ResultType.Details;

                // Gets the security principals for all the users.
                SecurityPrincipalsGetResponse principalGetResponse = await _service.SecurityPrincipalsGetAsync(principalGetRequest);

                users = principalGetResponse.Principals.Users;
            }
            catch (Exception ex)
            {

                
            }

            return users;
        }
    }
}
