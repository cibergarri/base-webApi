using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApiTest.Intefaces;
using WebApiTest.Services;

namespace WebApiTest.Helpers
{
    public class MongoDbAutoIncrementalHelper
    {

        public static int GetNextSequence<T>() where T : IBaseApiModel
        {

            var collection = MongoDbProvider.db.GetCollection<Counter>("counter");
            var update = Builders<Counter>.Update.Inc("seq", 1); //seq:1
            var options = new FindOneAndUpdateOptions<Counter>()
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            var ret = collection.FindOneAndUpdate<Counter>(x => x._id == typeof(T).Name.ToLower(), update, options);
            return ret.seq;
        }

        public static int GetCurrentSequence<T>() where T : IBaseApiModel
        {
            var collection = MongoDbProvider.db.GetCollection<Counter>("counter");
            var ret = collection.Find<Counter>(x => x._id == typeof(T).Name.ToLower()).First();
            return ret.seq;
        }

        public static void DeleteCurrentSequence<T>() where T : IBaseApiModel
        {
            var collection = MongoDbProvider.db.GetCollection<Counter>("counter");
            var result = collection.DeleteOne<Counter>(x => x._id == typeof(T).Name.ToLower());
        }

    }

    internal class Counter
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public string _id { get; set; }

        public int seq { get; set; }
    }
}