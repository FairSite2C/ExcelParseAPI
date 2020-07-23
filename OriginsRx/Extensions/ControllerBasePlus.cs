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
    public class ControllerBasePlus : ControllerBase
    {
        public ControllerBasePlus()
        {
        
        }
        
        internal ReturnResponse<string> ErrorResponse = new ReturnResponse<string>();
                
        internal bool InGood<T>(StdInputs si)
        {
            InGoodBase<T>(si);

            return ErrorResponse.Success;
        }
          
        internal bool InGood<T>(StdCollectionInputsId sci)
        {
            ValidId(sci.Id);

            InGood<T>( (StdCollectionInputs) sci);

            return ErrorResponse.Success;
        }

        internal bool InGood<T>(StdCollectionInputs sci)
        {
            InGoodBase<T>(sci);

            sci.Sort = "";

            if (sci.SortParameters != null && sci.SortParameters.Count > 0)
            {
                string[] inEls = sci.Sort.Trim().TrimStart('[').TrimEnd(']').Split(',');


                foreach (var  sortEl in sci.SortParameters)
                {

                    string realField = new Models.Helpers.FindRealName().FromJson<T>(sortEl.Column);

                    if (realField == "")
                    {
                        ErrorResponse.Errors.Add
                        (
                             $"{sci.Route} : Invalid sort parameter - {sortEl.Column}"
                        );
                    }

                    sci.Sort += !sortEl.Desc ? $"{realField}," : $"{realField}:desc,";
                }

                sci.Sort = sci.Sort.TrimEnd(","[0]);

            }

            if (sci.Sort == "") sci.Sort = "createdt:desc";

            return ErrorResponse.Success;
        }

        private bool InGoodBase<T>(StdInputs si)
        {
            si.Route = Request.Path.Value;
            si.PersonId = PersonId;

            if (si.SearchParameters != null && si.SearchParameters.Count > 0)
            {
                foreach (var row in si.SearchParameters) {
 
                    string realField = new Models.Helpers.FindRealName().FromJson<T>(row.Column);

                    if (realField == "")
                    {
                        ErrorResponse.Errors.Add
                        (
                            $"{row.Column} : Invalid search parameter"
                        );
                    }

                    row.Column = realField;
                }
            }

            return ErrorResponse.Success;
        }

        internal long PersonId
        {
            get { return 2; }
        }

        internal bool ValidId(long? id)
        {
            if (id == null || id < 1)
            {
                ErrorResponse.Errors.Add
                (
                    $"input id is 0 or null"
                );
            }

            return ErrorResponse.Success;
        }

    }
}
