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
        
        UInt64                      Capacity             { get; set; }
        UInt64                      MinCapacity          { get; }
        UInt64                      DefaultCapacity      { get; }
        UInt64                      CurrentLoad          { get; }

        ObjectCacheSettings         ObjectCacheSettings  { get; set; }
        

        Exceptional<INode>          StoreINode           (INode          myINode,   ObjectLocation myObjectLocation, CachePriority myPriority = CachePriority.LOW);
        Exceptional<ObjectLocator>  StoreObjectLocator   (ObjectLocator  myObjectLocator,                            CachePriority myPriority = CachePriority.LOW);
        Exceptional<AFSObject>      StoreAFSObject       (AFSObject      myAFSObject,                                CachePriority myPriority = CachePriority.LOW);

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
