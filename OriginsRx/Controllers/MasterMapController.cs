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

using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;
using System.Security.Claims;


namespace OriginsRx.Controllers
{
    [ApiController]
    public class MasterMapController : ControllerBasePlus
    {
        IMapService _service;

        public MasterMapController(IMapService service)
        {
            _service = service;
        }

        /// <summary>
        /// Returns a collection of MasterMaps
        /// </summary>
        /// <param name="offset">Which row to start on. Default=0.</param>
        /// <param name="limit">How many rows to take. Default=20",</param>
        /// <param name="includeDeleted">Include deleted records. Default=false.</param>
        /// <param name="sort">default=UpdateDate. To sort desc add :desc to fieldname, ex name:desc</param>
        [HttpPost]
        [Route("masterMap/all")]
        public async Task<ActionResult<MasterMaps>> GetAll(StdCollectionInputs sci)
        {
            if (!InGood<MasterMap>(sci)) return BadRequest(ErrorResponse);

            try
            {
                return await _service.GetAll(sci);
            }
            catch (Exception ex)
            {
 //               new ServerError(Request, sci.Route, ex);
            }

            return null;
        }

        [HttpGet]
        [Route("masterMap")]
        public async Task<ActionResult<ReturnResponse<MasterMap>>> GetMasterMap(long id)
        {
            if (!ValidId(id)) return BadRequest(ErrorResponse);

        //    string owner = (User.FindFirst(ClaimTypes.NameIdentifier))?.Value;

            return await _service.Get(id);
        }


        [HttpPost]
        [Route("masterMap/add")]
        public async Task<ActionResult<ReturnResponse<MasterMap>>> AddMasterMap(MasterMap map)
        {
            return await _service.Add(map, PersonId);
        }

        [HttpPatch]
        [Route("masterMap/mod")]
        public async Task<ActionResult<ReturnResponse<MasterMap>>> UpdateMasterMap([FromBody] MasterMap map)
        {
            return await _service.Update(map, PersonId);
        }

        [HttpGet]
        [Route("masterMap/make")]
        public async Task<ActionResult<int>> CreateDatatable(int id)
        {
            return await _service.CreateDataTable(id);
        }
    }
}
