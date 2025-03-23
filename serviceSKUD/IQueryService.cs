using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serviceSKUD
{
    public interface IQueryService<in TIn, out TOut>
                     where TIn: IQuery
    {
        TOut Execute(TIn obj);
    }
}
