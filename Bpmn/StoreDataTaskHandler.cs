using Camunda.Worker;
using MDSPermissions.Services;
using MDSServiceApp.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSServiceApp.Bpmn
{
    [HandlerTopics("Topic_StoreData", LockDuration = 10_000)]

    public class StoreDataTaskHandler : ExternalTaskHandler
    {
        private readonly IMediator _bus;
        private readonly IMDSService _service;
        private readonly IRepository _repository;

        public StoreDataTaskHandler(IMediator bus, IMDSService service, IRepository repository)
        {
            _bus = bus;
            _service = service;
            _repository = repository;
        }

        public override async Task<IExecutionResult> Process(ExternalTask externalTask)
        {
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
