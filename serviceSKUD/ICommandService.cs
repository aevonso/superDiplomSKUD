﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serviceSKUD
{
    public interface ICommandService <in T>
    {
        void Execute(T obj);
    }
}
