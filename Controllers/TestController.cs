using MDS;
using MDSPermissions.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
