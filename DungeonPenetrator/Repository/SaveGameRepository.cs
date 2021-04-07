using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SaveGameRepository<T> : StorageRepository<T>, ISaveGameRepository<T> where T : class
    {
        public SaveGameRepository() : base()
        {

        }
    }
}
