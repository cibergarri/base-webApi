using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebApiTest.Intefaces
{
    public interface IDbProvider
    {
        bool InsertElement<T>(T value) where T : class;
        List<T> GetElements<T>(Expression<Func<T, bool>> expression = null) where T : class;

        bool InsertIdElement<T>(T value) where T : IBaseApiModel;
        T GetElementById<T>(int id) where T : IBaseApiModel;
        
        bool DeleteElement<T>(int id) where T : IBaseApiModel;
        bool DeleteTable<T>() where T : IBaseApiModel;
    }
}
