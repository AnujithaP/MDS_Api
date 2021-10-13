﻿using MDSPermissions.Data;
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
    public class MedarbetarEntityController : Controller
    {
        private readonly IMDSService _service;

        public MedarbetarEntityController(IMDSService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<Medarbetare>> MedarbetareLista()
        {
            return await _service.GetCoWorkers();
        }

        [HttpGet("{id}")]
        public async Task<Medarbetare> Medarbetare(string id)
        {
            return await _service.GetCoWorker(id);
        }
    }
}