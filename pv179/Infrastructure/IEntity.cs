﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Infrastructure
{
    public interface IEntity
    {
        int Id { get; set; }

        string TableName { get; }
    }
}
