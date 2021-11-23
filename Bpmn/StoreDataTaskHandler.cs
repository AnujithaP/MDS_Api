using Camunda.Worker;
using MDSServiceApp.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDSServiceApp.Bpmn
{
    [HandlerTopics("Topic_StoreData", LockDuration = 10_000)]
    public class StoreDataTaskHandler : ExternalTaskHandler
    {
        private readonly IMediator _bus;
        private readonly IMDSService _service;
        private readonly IRepository _repository;
        private readonly ILogger<StoreDataTaskHandler> _logger;

        public StoreDataTaskHandler(IMediator bus, IMDSService service, IRepository repository, ILogger<StoreDataTaskHandler> logger)
        {
            _bus = bus;
            _service = service;
            _repository = repository;
            _logger = logger;
        }

        public override async Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            _logger.LogInformation(externalTask.TopicName + " called.");

            var variabler = externalTask.Variables;

            var storedPerson = _repository.GetPerson();
            var storedMedarbetare = _repository.GetMedarbetare();

            if (storedPerson != null && storedMedarbetare != null)
            {
                if (variabler["terms"].Value.ToString() == "1")
                {
                    var person = await _service.AddPerson(storedPerson);
                    var medarbetare = await _service.AddCoworker(storedMedarbetare);
                }

                _repository.Clear();
            }

            return new CompleteResult
            {
                Variables = new Dictionary<string, Variable>
                {
                    ["personId"] = new Variable(storedPerson.UniktId.ToString() ?? "", VariableType.String)
                }
            };
        }
    }
}
