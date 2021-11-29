using MDSServiceApp.Models;
using MDSServiceApp.Models.REST;
using MDSServiceApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDSServiceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PersonEntityController : Controller
    {
        private readonly IMDSService _service;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger _logger;
        private string _accountName;

        public PersonEntityController(IMDSService service, IHttpContextAccessor contextAccessor, ILogger<PersonEntityController> logger)
        {
            _service = service;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        [HttpPost]
        public void Authenticate([FromBody] string userAccount)
        {
            _accountName = userAccount;
        }

        [HttpGet]
        public async Task<IEnumerable<Person>> PersonLista()
        {
            if (HttpContext.Request.Headers.ContainsKey("accountName"))
            {
                StringValues value;
                var result = HttpContext.Request.Headers.TryGetValue("accountName", out value);

                //var users = await _service.GetPermissions("Person", MDS.PermissionType.Read, MDS.AccessPermissionType.Read);
                //var users = await _service.GetPrincipals();
            }

            return await _service.GetPersoner();
        }

        [HttpGet("{id:Guid}")]
        public async Task<Person> Person(Guid id)
        {
            return await _service.GetPerson(id);
        }

        [HttpPost]
        public async Task<Person> AddPerson([FromBody]Person person)
        {
            return await _service.AddPerson(person);
        }

        [HttpPost]
        public async Task<Person> AddPersonParams(string förnamn, string efternamn, string personnummer)
        {
            var p = new Person() { Förnamn = förnamn, Efternamn = efternamn, Personnummer = personnummer };

            return await _service.AddPerson(p);
        }

        [HttpPost]
        public async Task<Person> AddPerson([FromBody]RestPerson person)
        {
            var p = new Person() { Förnamn = person.Förnamn, Efternamn = person.Efternamn, Personnummer = person.Personnummer };

            return await _service.AddPerson(p);
        }

        [HttpPost]
        public void UpdatePerson([FromBody] Person person)
        {
            _service.UpdatePerson(person);
        }
    }
}
