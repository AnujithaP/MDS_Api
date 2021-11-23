using Camunda.Worker;
using MDSServiceApp.Models;
using MDSServiceApp.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDSServiceApp.Bpmn
{
    [HandlerTopics("Topic_GenerateEmail", LockDuration = 10_000)]
    public class CreateEpostTaskHandler : ExternalTaskHandler
    {
        private readonly IMediator _bus;
        private readonly IMDSService _service;
        private readonly IRepository _repository;
        private readonly ILogger<CreateEpostTaskHandler> _logger;

        public CreateEpostTaskHandler(IMediator bus, IMDSService service, IRepository repository, ILogger<CreateEpostTaskHandler> logger)
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

            var startdatum = DateTime.MinValue;
            var enddatum = DateTime.MinValue;
            DateTime.TryParse(variabler["start_date"].Value.ToString(), out startdatum);
            DateTime.TryParse(variabler["end_date"].Value.ToString(), out enddatum);

            var newmedarbetare = new Medarbetare()
            {
                UniktId = variabler["unique_identity"].Value.ToString(),
                Titel = variabler["title"].Value.ToString(),
                AnställningsForm = variabler["employment_form"].Value.ToString(),
                MobilnummerArbete = double.Parse(variabler["mobile_number"].Value.ToString()),
                EpostAdressArbete = variabler["work_email"].Value.ToString(),
                EpostAdressExtern = variabler["private_email"].Value.ToString(),
                Passnummer = variabler["passport_number"].Value.ToString(),
                Startdatum = startdatum,
                Slutdatum = enddatum
            };

            _repository.GetPerson().Jobbmail = newmedarbetare.EpostAdressArbete;
            await Task.Run(() => _repository.SetMedarbetare(newmedarbetare));     // Only to satisfy the async Process

            return new CompleteResult
            {
                Variables = new Dictionary<string, Variable>
                {
                    ["medarbetarId"] = new Variable(newmedarbetare.UniktId.ToString(), VariableType.String)
                }
            };
        }
    }
}
