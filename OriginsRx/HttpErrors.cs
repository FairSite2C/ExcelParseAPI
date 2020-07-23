using System;
using System.Collections.Generic;

using OriginsRx.Models.DTOs;

using Microsoft.AspNetCore.Http;

namespace OriginsRx
{
    /*
     * Not defined in the yaml but was using it in development
     * decided better just to return null or empty collections than throw error
    */
    public class CheckInputs
    {

        public class NoContent : Error
        {
            public NoContent(HttpRequest Request, string route)
            {
                Status = 204;
                Title = "No Content";
                Type = "http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html";
                Detail = "No Data Found";
                Instance = route;
                //            throw new HttpResponseException(Request.CreateResponse((HttpStatusCode.NoContent));
            }
        }

        public class Unauthorized : Error
        {
            public Unauthorized(HttpRequest Request)
            {

                Status = 401;
                Title = "Unauthorized";
                Type = "http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html";
                Detail = $"Access to requested resource unauthorized.";
                //        Instance = $"{Request.Uri.AbsolutePath}";
            }
        }

        public class Forbidden : Error
        {
            public Forbidden(HttpRequest Request, string route)
            {
                Status = 403;
                Title = "Forbidden";
                Type = "http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html";
                Detail = "The client is not authorized to access this resource.";
                Instance = route;
                //          throw new HttpException(Request.CreateResponse(HttpStatusCode.Forbidden));
            }
        }

        public class NotFound : Error
        {
            public NotFound(HttpRequest Request)
            {
                Status = 404;
                Title = "Not Found";
                Type = "http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html";
                Detail = $"The requested resource was not found.";
                //         Instance = $"{Request.RequestUri.AbsolutePath}";
            }
        }

        public class BadRequest : Error
        {

            public BadRequest(HttpRequest Request, string route, string errMessage)
            {
                Status = 400;
                Title = "Failed Validation";
                Type = "http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html";
                Detail = errMessage;
                Instance = route;
                //          throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
        }

        public class ServerError : Error
        {
            public ServerError(HttpRequest Request, string route, Exception ex)
            {
                // don't think will error get this error with entity framework but maybe
                if (ex.Message == "Conversion failed when converting date and/or time from character string.")
                {

                    Status = 400;
                    Title = "Bad Request";
                    Type = "http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html";
                    Detail = "Invalid Date format for UpdatedSince";
                    Instance = route;
                    //                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
                }
                else
                {


                    Status = 500;
                    Title = "Internal Server Error";
                    Type = "http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html";
                    Detail = ex.Message;
                    Instance = route;
                    //              throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError));
                }
            }
        }
    }
}
