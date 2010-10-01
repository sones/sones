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

/*
 * IObjectCache
 * (c) Achim Friedland, 2010
 */

#region Using

using System;
using System.Collections.Generic;

using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Caches;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphFS
{

    public interface IObjectCache : IEnumerable<KeyValuePair<ObjectLocation, ObjectLocator>>
    {

        Boolean                     IsEmpty              { get; }
        UInt64                      NumberOfCachedItems  { get; }
        ObjectCacheSettings         ObjectCacheSettings  { get; set; }

        Exceptional<INode>          StoreINode           (INode          myINode,   ObjectLocation myObjectLocation, Boolean myIsPinned = false);
        Exceptional<ObjectLocator>  StoreObjectLocator   (ObjectLocator  myObjectLocator,  Boolean myIsPinned = false);
        Exceptional<AFSObject>      StoreAFSObject       (AFSObject      myAFSObject,      Boolean myIsPinned = false);

        Exceptional<INode>          GetINode             (ObjectLocation myObjectLocation);
        Exceptional<ObjectLocator>  GetObjectLocator     (ObjectLocation myObjectLocation);
        Exceptional<PT>             GetAFSObject<PT>     (CacheUUID      myCacheUUID) where PT : AFSObject;

        Exceptional                 CopyToLocation       (ObjectLocation mySourceLocation, ObjectLocation myTargetLocation);
        Exceptional                 MoveToLocation       (ObjectLocation mySourceLocation, ObjectLocation myTargetLocation);

        Exceptional                 RemoveObjectLocator  (ObjectLocator  myObjectLocator,  Boolean myRecursion = false);
        Exceptional                 RemoveObjectLocation (ObjectLocation myObjectLocation, Boolean myRecursion = false);
        Exceptional                 RemoveAFSObject      (CacheUUID      myCacheUUID);

        Exceptional                 Clear();
    
    }

}
