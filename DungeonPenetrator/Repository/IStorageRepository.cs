using System;
using System.Linq;

namespace Repository
{
    public interface IStorageRepository<T> where T:class
    {
        void Insert(T entity);
    }
}
