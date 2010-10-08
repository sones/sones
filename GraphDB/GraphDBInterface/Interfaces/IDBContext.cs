using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Context
{
    public interface IDBContext
    {
        IDBContext CopyMe();

        void CopyTo(IDBContext myDBContext);

    }
}
