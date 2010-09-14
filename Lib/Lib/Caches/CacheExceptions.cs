/* GraphFS - CacheExceptions...
 * (c) Stefan Licht, 2009
 * 
 * All possible Cache specific exceptions
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.Caches
{
    public class CacheExceptions : Exception
    {
        public CacheExceptions(string message)
            : base(message)
        {
            // do nothing extra
        }
    }

    public class CacheExceptions_CannotOperateOnClosedCache : CacheExceptions
    {
        public CacheExceptions_CannotOperateOnClosedCache(string message) : base(message) { }
    }

    public class CacheExceptions_CacheItemAlreadyExistWithDifferentContent : CacheExceptions
    {
        public CacheExceptions_CacheItemAlreadyExistWithDifferentContent(string message) : base(message) { }
    }

    public class CacheExceptions_CacheItemAlreadyExist : CacheExceptions
    {
        public CacheExceptions_CacheItemAlreadyExist(string message) : base(message) { }
    }

    public class CacheExceptions_CacheItemDoesNotExist : CacheExceptions
    {
        public CacheExceptions_CacheItemDoesNotExist(string message) : base(message) { }
    }

    public class CacheExceptions_DependingCacheItemDoesNotExist : CacheExceptions
    {
        public CacheExceptions_DependingCacheItemDoesNotExist(string message) : base(message) { }
    }
}
