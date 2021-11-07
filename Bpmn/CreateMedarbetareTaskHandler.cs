using Camunda.Worker;
using MDSPermissions.Data;
using MDSPermissions.Services;
using MDSServiceApp.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace MDSServiceApp.Bpmn
{
    [HandlerTopics("Topic_GenerateEmail", LockDuration = 10_000)]
    public class CreateMedarbetareTaskHandler : ExternalTaskHandler
    {
        private readonly IMediator _bus;
        private readonly IMDSService _service;
        private readonly IRepository _repository;

        public CreateMedarbetareTaskHandler(IMediator bus, IMDSService service, IRepository repository)
        {
            _bus = bus;
            _service = service;
            _repository = repository;
        }

        public override async Task<IExecutionResult> Process(ExternalTask externalTask)
        {
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
