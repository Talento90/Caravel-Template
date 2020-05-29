using System.Net;
using Caravel.AspNetCore.Http;
using Caravel.Errors;
using Microsoft.AspNetCore.Mvc;

namespace CaravelTemplate.WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        protected ActionResult NotFound(Error error)
            => NotFound(new HttpError(HttpContext, HttpStatusCode.NotFound, error));
        
        protected ActionResult BadRequest(Error error)
            => BadRequest(new HttpError(HttpContext, HttpStatusCode.BadRequest, error));
    }
}