﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IHighscoreRepository<T> : IStorageRepository<T> where T : class
    {

    }
}
