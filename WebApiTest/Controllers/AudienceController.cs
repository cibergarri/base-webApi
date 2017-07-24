using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTest.Entities;
using WebApiTest.Models;
using WebApiTest.Auth;

namespace WebApiTest.Controllers
{
    /// <summary>
    /// Endpoint to create new resource servers (audiences), just for demo purposes
    /// In a production environment it should be a secure admin portal to create audiences
    /// and regenerate the key as well
    /// </summary>
    [RoutePrefix("api/audience")]
    public class AudienceController : ApiController
    {
        [Route("")]
        //api/audience (POST)
        public IHttpActionResult Post(AudienceModel audienceModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Audience newAudience = AudiencesStore.AddAudience(audienceModel.Name);

            return Ok<Audience>(newAudience);

        }
    }
}
