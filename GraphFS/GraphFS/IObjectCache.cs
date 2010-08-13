
#region Using

using System;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Caches;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphFS
{

    public interface IObjectCache
    {

        Boolean             IsEmpty             { get; }
        UInt64              NumberOfCachedItems { get; }
        ObjectCacheSettings ObjectCacheSettings { get; set; }

        Exceptional<INode>         StoreINode(INode myINode, ObjectLocation myObjectLocation, Boolean myIsPinned = false);
        Exceptional<ObjectLocator> StoreObjectLocator (ObjectLocator  myObjectLocator,  Boolean        myIsPinned = false);
        Exceptional<AFSObject>     StoreAFSObject     (CacheUUID      myCacheUUID,      AFSObject      myAFSObject,      Boolean myIsPinned = false);

        Exceptional<INode>         GetINode           (ObjectLocation myObjectLocation);
        Exceptional<ObjectLocator> GetObjectLocator   (ObjectLocation myObjectLocation);
        Exceptional<PT>            GetAFSObject<PT>   (CacheUUID      myCacheUUID) where PT : AFSObject;
//        Exceptional<Boolean>       HasObjectLocator   (ObjectLocation myObjectLocation);

        Exceptional   Copy(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation, Boolean myRecursion = false);
        Exceptional   Move(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation, Boolean myRecursion = false);

        Exceptional   RemoveObjectLocator(ObjectLocator myObjectLocator, Boolean myRecursion = false);
        Exceptional   RemoveObjectLocation(ObjectLocation myObjectLocation, Boolean myRecursion = false);
        Exceptional   RemoveAFSObject(CacheUUID myCacheUUID);

        Exceptional   Clear();
    
    }

}
