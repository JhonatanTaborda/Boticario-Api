using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boticario.Api.Models;
using Boticario.Api.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boticario.Api.Controllers
{       
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISalesRepository _salesRepository;        

        public SalesController(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;            
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SalesModel>> Create([FromBody] SalesModel model)
        {
            return await _salesRepository.Create(model);
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            await _salesRepository.Delete(id);

            return Ok();
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<SalesModel>> Update([FromBody] SalesModel model)
        {
            return await _salesRepository.Update(model);
        }

        [Authorize]
        [HttpGet]        
        public async Task<ActionResult<IList<SalesModel>>> GetAll()
        {
            var resutl = await _salesRepository.GetAll();

            return Ok(resutl);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesModel>> Get([FromRoute] string id)
        {
            var resutl = await _salesRepository.Get(id);

            return Ok(resutl);
        }
    }
}