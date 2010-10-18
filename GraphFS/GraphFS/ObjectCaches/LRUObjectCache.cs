/*
 * LRUObjectCache
 * (c) Achim Friedland, 2010
 */

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using sones.GraphFS.DataStructures;
using sones.GraphFS.Events;
using sones.GraphFS.Objects;

using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.Timestamp;
using sones.GraphFS.Caches;
using sones.Lib.Caches;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.Big;
using sones.GraphFS.Errors;
using System.Diagnostics;
using System.Collections;


#endregion

namespace sones.GraphFS
{

    /// <summary>
    /// An Last-Recently-Use ObjectCache implemantation of the IObjectCache interface
    /// for storing INodes, ObjectLocators and AFSObjects. This cache will remove the
    /// entries as soon as memory gets low or the stored items are getting very old.
    /// </summary>
    public class LRUObjectCache : ALRUObjectCache, IObjectCache
    {


        #region Constructor(s)

        #region ObjectCache()

        public LRUObjectCache()
            : this(_DefaultCapacity)
        {
        }

        #endregion

        #region ObjectCache(myCapacity)

        public LRUObjectCache(UInt64 myCapacity)
            : base(myCapacity)
        {
        }

        #endregion

        #endregion


        #region (protected) StoreObjectLocator_protected(myObjectLocator, myCachePriority = CachePriority.LOW)

        protected override Exceptional<ObjectLocator> StoreObjectLocator_protected(ObjectLocator myObjectLocator, CachePriority myCachePriority = CachePriority.LOW)
        {

            if (_ObjectLocatorLRUList.ULongCount() >= _Capacity)
            {

                // Remove oldest LinkedListNode from LRUList and add new ObjectLocator to the ObjectCache
                OnItemDiscarded(new DiscardEventArgs(myObjectLocator.ObjectLocation));

                //_ObjectLocatorCache.Remove(_ObjectLocatorLRUList.First.Value.ObjectLocation);                    
                    
                var __ObjectLocation = _ObjectLocatorLRUList.First.Value.ObjectLocation;

                if (__ObjectLocation == ObjectLocation.Root)
                {
                    var _ObjectLocatorNode = _ObjectLocatorLRUList.First();
                    _ObjectLocatorLRUList.RemoveFirst();
                    _ObjectLocatorLRUList.AddLast(_ObjectLocatorNode);
                }

                __ObjectLocation = _ObjectLocatorLRUList.First.Value.ObjectLocation;
                RemoveObjectLocation(__ObjectLocation);

            }

            return new Exceptional<ObjectLocator>(myObjectLocator);

        }

        #endregion

        #region (protected) StoreAFSObject_protected(myAFSObject, myCacheUUID, myCachePriority = CachePriority.LOW)

        protected override Exceptional<AFSObject> StoreAFSObject_protected(AFSObject myAFSObject, CacheUUID myCacheUUID, CachePriority myCachePriority = CachePriority.LOW)
        {

            Debug.Assert(myCacheUUID                        != null);

            return new Exceptional<AFSObject>();

        }

        #endregion


    }

}
