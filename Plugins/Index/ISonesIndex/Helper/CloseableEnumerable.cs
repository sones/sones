using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.Helper
{
    public class CloseableEnumerable<T> : ICloseableEnumerable<T>
    {
        IEnumerable<T> _Enumerable;
        Action _Close = null;

        public CloseableEnumerable(IEnumerable<T> myEnumerable, Action myClose = null)
        {
            _Enumerable = myEnumerable;
            _Close = myClose;
        }

        public virtual void Close()
        {
            if (_Close != null)
            {
                _Close();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Enumerable.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Enumerable.GetEnumerator();
        }

        public virtual void Dispose()
        {
            Close();
        }
    }
}
