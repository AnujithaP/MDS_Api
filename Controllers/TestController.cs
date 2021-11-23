using MDS;
using MDSServiceApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDSServiceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : Controller
    {
        private readonly IMDSService _service;

        public TestController(IMDSService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> Test()
        {

            return await _service.GetPrincipals();
        }
    }
}
