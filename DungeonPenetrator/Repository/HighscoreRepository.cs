using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class HighscoreRepository<T> : StorageRepository<T>, IHighscoreRepository<T> where T : class
    {
        public HighscoreRepository() : base()
        {

        }
    }
}
