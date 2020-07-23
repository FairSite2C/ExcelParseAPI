using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using OriginsRx.Models.DTOs;
using OriginsRx.Models.Interfaces.Services;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

using System.IO;

namespace OriginsRx.Controllers
{
    [ApiController]
    public class SaleController : ControllerBasePlus
    {
        ISaleService _service;

        public SaleController(ISaleService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("sale/all")]
        public async Task<ActionResult<Sales>> GetAll(StdCollectionInputs sci)
        {
            if (!InGood<Sale>(sci)) return BadRequest(ErrorResponse);

            try
            {
                return await _service.GetAll(sci);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return StatusCode(500,ex);
            }

        }

        [HttpPost]
        [Route("sale/person/all")]
        public async Task<ActionResult<Sales>> GetAllPerson(StdCollectionInputsId sci)
        {
            if (!InGood<Sale>(sci)) return BadRequest(ErrorResponse);

            try
            {
                return await _service.GetAllPerson(sci);
            }
            catch (Exception ex)
            {
                //          new ServerError(Request, sci.Route, ex);
            }

            return null;
        }

        [HttpPost]
        [Route("sale/company/all")]
        public async Task<ActionResult<Sales>> GetAllCompany(StdCollectionInputsId sci)
        {
            if (!InGood<Sale>(sci)) return BadRequest(ErrorResponse);

            try
            {
                return await _service.GetAllCompany(sci);
            }
            catch (Exception ex)
            {
                //          new ServerError(Request, sci.Route, ex);
            }

            return null;
        }

        [HttpGet]
        [Route("sale")]
        public async Task<ActionResult<ReturnResponse<Sale>>> GetSale(long id)
        {
            if (!ValidId(id)) return BadRequest(ErrorResponse);

            return await _service.Get(id);
        }
    }
}
