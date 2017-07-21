using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiTest.Models
{

    public class Value : BaseModel
    {
        public string value { get; set; }
    }
}