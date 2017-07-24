using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using WebApiTest.Intefaces;

namespace WebApiTest.Services
{
    public class NLogService : ILogService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public NLogService()
        {   
        }

        public void log(Exception ex)
        {
            logger.Error(ex, "Oops, an exception occured");
        }        
    }
}