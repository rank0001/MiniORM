using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniORM.DataAccessLayer
{
    public interface ISqlDataAccess<T> where T : class
    {
        void Delete(int id);
        void Delete(T item);
        void Insert(T item);
        void Update(T item);
        string GetById(int id);
        string GetAll();


    }
}
