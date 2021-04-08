using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public abstract class StorageRepository<T> : IStorageRepository<T> where T : class
    {
        public StorageRepository()
        {
        }

        public abstract void Insert(T entity);
    }
}
