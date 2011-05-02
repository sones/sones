using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// Public this interface is for database use only.
    /// </summary>
    public interface IUnknownProvider
    {
        void ClearUnknown();
    }
}
