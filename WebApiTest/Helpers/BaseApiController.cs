using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApiTest.Intefaces;
using WebApiTest.Services;

namespace WebApiTest.Helpers
{
    public class BaseApiController<T> : ApiController where T : class,IBaseApiModel
    {
        private readonly IDbProvider _provider;

        public BaseApiController(IDbProvider provider)
        {
            this._provider = provider;
        }      

        public IEnumerable<T> Get()
        {   
            return this._provider.GetElements<T>();
        }

        // GET api/{model}/5
        public T Get(int id)
        {
            var element = this._provider.GetElementById<T>(id);
            if(element ==null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            return element;
        }

        // POST api/{model}
        public void Post([FromBody]T value)
        {
            if(!this._provider.InsertIdElement(value))
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
        }

        // PUT api/{model}/5
        public void Put(int id, [FromBody]T value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            if(!this._provider.DeleteElement<T>(id))
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            //Just for debugging purposes, delete:
            if (false)
                this._provider.DeleteTable<T>();
        }
    }
}