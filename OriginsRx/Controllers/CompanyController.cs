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
    public class CompanyController : ControllerBasePlus
    {
        ICompanyService _service;

        public CompanyController(ICompanyService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("company/all")]
        public async Task<ActionResult<Companies>> GetAll(StdCollectionInputs sci)
        {
            if (!InGood<Company>(sci)) return BadRequest(ErrorResponse);
             
            try
            {
                return await _service.GetAll(sci);
            }
            catch (Exception ex)
            {
    //            new ServerError(Request, sci.Route, ex);
            }

            return null;
        }

        [HttpGet]
        [Route("company")]
        public async Task<ActionResult<ReturnResponse<Company>>> GetCompany(long id)
        {
            if (!ValidId(id)) return BadRequest(ErrorResponse);

            return await _service.Get(id);
        }

        [HttpPost]
        [Route("company/add")]
        public async Task<ActionResult<ReturnResponse<Company>>> AddCompany(CompanyAdd inVal)
        {
            try
            {
                return await _service.Add(inVal);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch]
        [Route("company/mod")]
        public async Task<ActionResult<ReturnResponse<Company>>> UpdateCompany(CompanyMod inVal)
        {
            return await _service.Update(inVal);
        }

        [HttpPost]
        [Route("company/map/all")]
        public async Task<ActionResult<PersonMaps>> GetAllMaps(StdCollectionInputsId sci)
        {
            if (!InGood<PersonMap>(sci)) return BadRequest(ErrorResponse);

 
            try
            {
                return await _service.GetAllPersonMaps(sci);
            }
            catch (Exception ex)
            {
//                new ServerError(Request, sci.Route, ex);
            }

            return null;
        }
    }
}
