using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

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
    public class ImportController : ControllerBasePlus
    {
        IImportServicee _service;

        public ImportController(IImportServicee service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("imports/masterMap")]
        public async Task<ActionResult<Imports>> GetAllByMasterMap(StdCollectionInputsId sci)
        {
            if (!InGood<Import>(sci)) return BadRequest(ErrorResponse);

            try
            {
                return await _service.GetAllByMasterMap(sci);
            }
            catch (Exception ex)
            {
        
 //               new ServerError(Request, sci.Route, ex);
            }

            return null;
        }

        [HttpPost]
        [Route("imports/personMap")]
        public async Task<ActionResult<Imports>> GetAllByPersonMap(StdCollectionInputsId sci)
        {
            if (!InGood<Import>(sci)) return BadRequest(ErrorResponse);

            try
            {
                return await _service.GetAllByPersonMap(sci);
            }
            catch (Exception ex)
            {
 //               new ServerError(Request, sci.Route, ex);
            }

            return null;
        }

        [HttpPost]
        [Route("imports/person")]
        public async Task<ActionResult<Imports>> GetAllByPerson(StdCollectionInputsId sci)
        {
            if (!InGood<Import>(sci)) return BadRequest(ErrorResponse);

            try
            {
                return await _service.GetAllByPerson(sci);
            }
            catch (Exception ex)
            {
   //             new ServerError(Request, sci.Route, ex);
            }

            return null;
        }

        [HttpPost]
        [Route("imports/company")]
        public async Task<ActionResult<Imports>> GetAllByCompany(StdCollectionInputsId sci)
        {
            if (!InGood<Import>(sci)) return BadRequest(ErrorResponse);

            try
            {
                return await _service.GetAllByCompany(sci);
            }
            catch (Exception ex)
            {
                ErrorResponse.Errors.Add(ex.Message);
            }

            return null;
        }

        [HttpGet]
        [Route("import")]
        public async Task<ActionResult<ReturnResponse<Import>>> GetImport(long id)
        {
            if (!ValidId(id)) return BadRequest(ErrorResponse);

            return await _service.Get(id);
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("import/getSheetHeaders")]
        public async Task<ActionResult<string[]>> GetSheetHeaders()
        {

            var stream = new MemoryStream();
            var form = Request.Form;

            await form.Files[0].CopyToAsync(stream);
            try
            {
                return await _service.GetSheetHeaders(stream);
            }catch(Exception ex)
            {
                string wtf = ex.Message;
            }

            return null;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("import/upload/xl")]
        public async Task<ActionResult<List<ImportPost>>> UploadXl()
        {

            var retVal = new List<ImportPost>();
            var form = Request.Form;

            long pId = Convert.ToInt64(form["personId"].ToString());
            long pmId = Convert.ToInt64(form["personMapId"].ToString());

            foreach (IFormFile file in form.Files)
            {
                var stream = new MemoryStream();

                await file.CopyToAsync(stream);

                var sheet = new ImportPost()
                {
                    PersonMapId = pmId,
                    PersonId = pId,
                    FileName = file.FileName,
                    FileLength = file.Length,
                    Stream = stream
                };
                try
                {
                    retVal.Add(await _service.ImportSheet(sheet));
                }
                catch (Exception ex)
                {
                    var nw = ex.Message;
                }
            }

            return retVal;
        }
    }
}
