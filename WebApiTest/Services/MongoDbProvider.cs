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
using WebApiTest.Helpers;

namespace WebApiTest.Services
{
    public class MongoDbProvider : IDbProvider
    {
        private readonly ILogService _service;

        private static readonly Lazy<IMongoDatabase> _db = new Lazy<IMongoDatabase>(() =>
            {
                var connectionString = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;
                var client = new MongoClient(connectionString);
                return client.GetDatabase("WebApiTest");
            });

        public static IMongoDatabase db { get { return _db.Value; } }

        public MongoDbProvider(ILogService logService)
        {
            //To avoid having to decorate every class with the attribute 
            //that allows not to have the _id property
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
            _service = logService;
        }

        public bool InsertElement<T>(T value) where T: class
        {
            try
            {
                var valuesCollection = db.GetCollection<T>(typeof(T).Name.ToLower());
                valuesCollection.InsertOne(value);
            }
            catch(Exception ex)
            {
                _service.log(ex);
                return false;
            }
            return true;
        }

        public bool InsertIdElement<T>(T value) where T : IBaseApiModel
        {
            try
            {
                var valuesCollection = db.GetCollection<T>(typeof(T).Name.ToLower());
                value.Id = MongoDbAutoIncrementalHelper.GetNextSequence<T>();
                valuesCollection.InsertOne(value);                
            }
            catch (Exception ex)
            {
                _service.log(ex);
                return false;
            }
            return true;
        }

        public T GetElementById<T>(int id) where T : IBaseApiModel
        {
            try
            {
                var valuesCollection = db.GetCollection<T>(typeof(T).Name.ToLower());
                var element= valuesCollection
                    .Find(x => x.Id == id)
                    .FirstOrDefault();                
                return element;
            }
            catch (Exception ex)
            {
                _service.log(ex);
                throw;
            }
        }

        public List<T> GetElements<T>(Expression<Func<T, bool>> expression = null) 
            where T : class
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
                _service.log(ex);
                throw;
            }            
        }
        
        public bool DeleteElement<T>(int id) where T:IBaseApiModel
        {
            try
            {
                var valuesCollection = db.GetCollection<T>(typeof(T).Name.ToLower());
                var deleteResult = valuesCollection.DeleteOne<T>(x=>x.Id==id);
                return (deleteResult.DeletedCount == 1);                    
            }
            catch (Exception ex)
            {
                _service.log(ex);
                throw new System.Web.Http.HttpResponseException(
                            System.Net.HttpStatusCode.InternalServerError);                
            }
        }

        public bool DeleteTable<T>() where T:IBaseApiModel
        {
            try
            {
                db.DropCollection(typeof(T).Name.ToLower());
            }
            catch (Exception ex)
            {
                _service.log(ex);
                return false;
            }
            return true;
            
        }
    }
}