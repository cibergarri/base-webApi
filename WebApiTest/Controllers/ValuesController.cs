using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTest.Helpers;
using WebApiTest.Intefaces;
using WebApiTest.Models;
using WebApiTest.Services;

namespace WebApiTest.Controllers
{
    /// <summary>
    /// /api/values/
    /// </summary>
    public class ValuesController : BaseApiController<Value>
    {
        public ValuesController(IDbProvider provider) :base(provider)
        {

        }        
    }
}
