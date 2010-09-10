/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
