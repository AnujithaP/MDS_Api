using MDSServiceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSServiceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Authenticate : Controller
    {
        private readonly IMDSService _service;

        public IEnumerable<MDS.User> MDSUsers { get; set; }

        public Authenticate(IMDSService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IEnumerable<MDS.User>> Get()
        {
            StringValues accountName;

            MDSUsers = await _service.GetPrincipals();

            if (HttpContext.Request.Headers.ContainsKey("accountName"))
            {
                HttpContext.Request.Headers.TryGetValue("accountName", out accountName);

                var account = MDSUsers.Where(u => u.Identifier.Name == accountName).FirstOrDefault();
            }

            return MDSUsers;
        }
    }
}
