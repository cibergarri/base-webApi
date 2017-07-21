using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver.Core;
using MongoDB.Driver;
using System.Configuration;
using MongoDB.Bson;
using WebApiTest.Models;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Conventions;
using System.Linq.Expressions;
using WebApiTest.Intefaces;

namespace WebApiTest.Services
{
    public class MongoDbProvider
    {
        private static readonly Lazy<IMongoDatabase> _db = new Lazy<IMongoDatabase>(() =>
            {
                var connectionString = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;
                var client = new MongoClient(connectionString);
                return client.GetDatabase("WebApiTest");
            });

        public static IMongoDatabase db { get { return _db.Value; } }

        public MongoDbProvider()
        {
            //To avoid having to decorate every class with the attribute that allows not to have the ID property
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
        }

        public static bool InsertElement<T>(T value) where T:class
        {
            try
            {
                var valuesCollection = db.GetCollection<T>(typeof(T).Name.ToLower());
                valuesCollection.InsertOne(value);
            }
            catch
            {
#warning "LOG"
                return false;
            }
            return true;
        }

        public static bool InsertIdElement<T>(T value) where T : IModel
        {
            try
            {
                var prueba = false;
                if (prueba)
                    MongoDbAutoIncrementalHelper.CreateSequence<T>();

                var valuesCollection = db.GetCollection<T>(typeof(T).Name.ToLower());
                value.Id = MongoDbAutoIncrementalHelper.GetNextSequence<T>();
                valuesCollection.InsertOne(value);
                
                MongoDbAutoIncrementalHelper.GetCurrentSequence<T>();
               //     MongoDbAutoIncrementalHelper.GetNextSequence<T>()
            }
            catch (Exception ex)
            {
#warning "LOG"
                return false;
            }
            return true;
        }

        public static List<T> GetElements<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            try
            {
                var valuesCollection = db.GetCollection<T>(typeof(T).Name.ToLower());
                return valuesCollection
                    .Find(expression==null? x => true : expression)
                     .ToList();//.ToListAsync().Result;
            }
            catch (Exception ex)
            {
#warning "LOG - controlled exception"
                throw;
            }            
        }
        
        public static bool DeleteElement<T>(int id) where T:class
        {
            try
            {
                db.DropCollection(typeof(T).Name.ToLower());
                //var valuesCollection = db.GetCollection<T>(typeof(T).Name.ToLower());
                //valuesCollection.DeleteOne<T>(x=>x.Id==id);
            }
            catch
            {
#warning "LOG"
                return false;
            }
            return true;
        }
    }

    
    public class MongoDbAutoIncrementalHelper
    {
        public static void CreateSequence<T>() where T : IModel
        {
            var counter = new Counter()
            {
                _id = typeof(T).Name.ToLower(),
                seq = 0
            };
            var collection = MongoDbProvider.db.GetCollection<Counter>("counter");
            collection.InsertOne(counter);
        }

        public static int GetNextSequence<T>() where T : IModel
        {
            
            var collection = MongoDbProvider.db.GetCollection<Counter>("counter");
            var update = Builders<Counter>.Update.Inc("seq",1); //seq:1
            var options = new FindOneAndUpdateOptions<Counter>()
            {
                IsUpsert = true,
                ReturnDocument=ReturnDocument.After
            };
            var ret = collection.FindOneAndUpdate<Counter>(x => x._id == typeof(T).Name.ToLower(), update, options);
            return ret.seq;
        }

        public static int GetCurrentSequence<T>() where T : IModel
        {
            var collection = MongoDbProvider.db.GetCollection<Counter>("counter");
            var ret = collection.Find<Counter>(x => x._id == typeof(T).Name.ToLower()).First();
            return ret.seq;
        }

        public static void DeleteCurrentSequence<T>() where T : IModel
        {
            var collection = MongoDbProvider.db.GetCollection<Counter>("counter");
            var result = collection.DeleteOne<Counter>(x => x._id == typeof(T).Name.ToLower());
        }

    }

    public class Counter
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public string _id { get; set; }

        public int seq { get; set; }
    }
}