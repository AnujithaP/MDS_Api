using Camunda.Worker;
using MDSServiceApp.Models;
using MDSServiceApp.Services;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CreatePersonTaskHandler> _logger;

        public CreatePersonTaskHandler(IMediator bus, IMDSService service, IRepository repository, ILogger<CreatePersonTaskHandler> logger)
        {
            this.bus = bus;
            _service = service;
            _repository = repository;
            _logger = logger;
        }

        public override async Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            _logger.LogInformation(externalTask.TopicName + " called.");

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