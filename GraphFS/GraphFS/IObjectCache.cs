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
