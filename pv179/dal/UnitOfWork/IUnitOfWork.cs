﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dal.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task Commit();
        void RegisterAction(Action action);
    }
}
