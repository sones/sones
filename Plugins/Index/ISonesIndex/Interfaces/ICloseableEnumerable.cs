using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index
{
    public interface ICloseableEnumerable<T> : IEnumerable<T>, IDisposable
    {
        void Close();
    }
}
