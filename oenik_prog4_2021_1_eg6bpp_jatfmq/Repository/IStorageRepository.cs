using System;
using System.Linq;

namespace Repository
{
    public interface IStorageRepository<T> where T:class
    {
        void Insert(T entity);

        T GetOne(string name);

        IQueryable<T> GetAll();

        void Delete(T entity);

        void SaveChanges();
    }
}
