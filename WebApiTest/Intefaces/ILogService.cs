using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebApiTest.Intefaces
{
    public interface ILogService
    {
        void log(Exception ex);
    }
}
