using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTest.Intefaces;

namespace WebApiTest.Controllers
{
    public class LogsController : ApiController
    {
        private readonly IDbProvider _provider;

        public LogsController(IDbProvider provider)
        {
            this._provider = provider;
        }

        public IEnumerable<BsonDocument> Get()
        {
            var logsCollection = Services.MongoDbProvider.db.GetCollection<BsonDocument>("Log");

            return logsCollection
                .Find(x => true)
                 .ToList();
        }
    }
}
