using Camunda.Worker;
using MDSPermissions.Data;
using MDSPermissions.Services;
using MDSServiceApp.Services;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDSServiceApp
{
    [HandlerTopics("Topic_CreatePerson", LockDuration = 10_000)]
    public class CreatePersonTaskHandler : ExternalTaskHandler
    {
        private readonly IMediator bus;
        private readonly IMDSService _service;
        private readonly IRepository _repository;

        public CreatePersonTaskHandler(IMediator bus, IMDSService service, IRepository repository)
        {
            this.bus = bus;
            _service = service;
            _repository = repository;
        }

        public override async Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            var variabler = externalTask.Variables;

            var newperson = new Person() { 
                Gender = variabler["gender"].Value.ToString(),
                Födelsedatum = variabler["dob"].Value.ToString(),
                Efternamn = variabler["last_name"].Value.ToString(),
                Personnummer = variabler["personal_number"].Value.ToString(),
                Mellannamn = variabler["middle_name"].Value.ToString(),
                Postadress = null,
                Förnamn = variabler["first_name"].Value.ToString(),
                Privatmail = variabler["email"].Value.ToString()
            };

            await Task.Run(() => _repository.SetPerson(newperson));     // Only to satisfy the async Process

            return new CompleteResult
            {
                Variables = new Dictionary<string, Variable>
                {
                    ["personId"] = new Variable(newperson.UniktId.ToString(), VariableType.String)
                }
            };
        }
    }
}