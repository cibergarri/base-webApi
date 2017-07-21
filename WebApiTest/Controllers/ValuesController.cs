using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTest.Models;
using WebApiTest.Services;

namespace WebApiTest.Controllers
{
    public class ValuesController : ApiController
    {
        //private MongoHelper<string> _values;

        // GET api/values
        public IEnumerable<Value> Get()
        {
            return MongoDbProvider.GetElements<Value>();
        }

        // GET api/values/5
        public Value Get(int id)
        {
            return MongoDbProvider.GetElements<Value>( x =>x.Id == id).First();
        }

        // POST api/values
        public void Post([FromBody]Value value)
        {
            MongoDbProvider.InsertIdElement(value);
            
            
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]Value value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            MongoDbProvider.DeleteElement<Value>(id);
        }
    }
}
