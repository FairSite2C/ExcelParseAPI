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

namespace OriginsRx.Controllers
{
    [ApiController]
    public class PersonController : ControllerBasePlus
    {
        IPersonService _service;

        public PersonController(IPersonService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("person/all")]
        public async Task<ActionResult<Persons>> GetAll(StdCollectionInputs sci)
        {
            if (!InGood<Person>(sci)) return BadRequest(ErrorResponse);

            try
            {
                return await _service.GetAll(sci);
            }
            catch (Exception ex)
            {
   //             new ServerError(Request, sci.Route, ex);
            }

            return null;
        }

        [HttpGet]
        [Route("person")]
        public async Task<ActionResult<ReturnResponse<Person>>> GetPerson(long id)
        {
            if (!ValidId(id)) return BadRequest(ErrorResponse);
            return await _service.Get(id);
        }

        /*

                [HttpGet]
                [Route("person/me")]
                public async Task<ActionResult<long>> GetPersonMe()
                {
                    // get the identity stuff from azure
                    // match on email
                    string email = "felix@hoffman.com";
                    return  await _service.GetMe(email);
                }

                [HttpPost]
                [Route("person/add")]
                public async Task<Person> AddPerson(Person inVal)
                {
                    return await _service.Add(inVal);
                }
*/
        [HttpPatch]
        [Route("person/mod")]
        public async Task<ReturnResponse<Person>> UpdatePerson(Person inVal)
        {
            return await _service.Update(inVal);
        }

        [HttpPost]
        [Route("person/map/all")]
        public async Task<ActionResult<PersonMaps>> GetAllMaps(StdCollectionInputsId sci)
        {
            if (!InGood<PersonMap>(sci)) return BadRequest(ErrorResponse);
 
            try
            {
                return await _service.GetAllMaps(sci, PersonId);
            }
            catch (Exception ex)
            {
  //              new ServerError(Request, sci.Route, ex);
            }

            return null;
        }

        [HttpGet]
        [Route("person/map")]
        public async Task<ActionResult<ReturnResponse<PersonMap>>> GetMap(long id)
        {
            if (!ValidId(id)) return BadRequest(ErrorResponse);

            try
            {
                var retVal = (await _service.GetMap(id));
                return retVal;
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        [HttpPost]
        [Route("person/map/add")]
        public async Task<ActionResult<ReturnResponse<PersonMap>>> AddMap([FromBody] PersonMapAdd map)
        {
              return await _service.AddMap(map,PersonId);
        }

        [HttpPatch]
        [Route("person/map/mod")]
        public async Task<ActionResult<ReturnResponse<PersonMap>>> UpdateMap([FromBody] PersonMapMod map)
        {
            return await _service.UpdateMap(map,PersonId);
        }
    }
}
