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
 * ObjectCache
 * (c) Achim Friedland, 2008 - 2010
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


#endregion

namespace sones.GraphFS
{

    #region CacheTimer<T>

    public struct CacheTimer<T>
    {

        #region Data

        private readonly T       _Object;
        private          UInt64  _Timestamp;

        #endregion

        #region Constructor(s)

        public CacheTimer(T myObject)
        {
            _Object     = myObject;
            _Timestamp  = TimestampNonce.Ticks;
        }

        #endregion

        #region Object

        public T Object
        {
            get
            {
                _Timestamp  = TimestampNonce.Ticks;
                return _Object;
            }
        }

        #endregion

        #region Timestamp

        public UInt64 Timestamp
        {
            get
            {
                return _Timestamp;
            }
        }

        #endregion

    }

    #endregion


    /// <summary>
    /// An implemantation of the IObjectCache interface for storing INodes,
    /// ObjectLocators and AFSObjects. This cache will remove the entries
    /// as soon as memory gets low or the stored items are getting very old.
    /// </summary>
    public class ObjectCache : ObjectStore
    {

        #region Data

        //private Timer  _FlushTimer;
        //private ReaderWriterLockSlim _CacheItemReaderWriterLockSlim;

        private const    UInt64                                                     _MinCapacity     = 100;
        private const    UInt64                                                     _DefaultCapacity = 5000;
        private readonly Dictionary<ObjectLocation, LinkedListNode<ObjectLocator>>  _ObjectLocatorCache;
        private readonly LinkedList<ObjectLocator>                                  _ObjectLocatorLRUList;

        #endregion

        #region Properties

        #region Capacity

        private UInt64 _Capacity;

        public UInt64 Capacity
        {

            get
            {
                return _Capacity;
            }

            set
            {
                if (value >= _MinCapacity)
                    _Capacity = value;
            }
        
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region ObjectCache()

        public ObjectCache()
            : this(_DefaultCapacity)
        {
        }

        #endregion

        #region ObjectCache(myCapacity)

        public ObjectCache(UInt64 myCapacity)
            : base()
        {

            if (myCapacity < _MinCapacity)
                throw new ArgumentException("myCapacity must be larger than zero!");

            _Capacity               = myCapacity;
            _ObjectLocatorCache     = new Dictionary<ObjectLocation, LinkedListNode<ObjectLocator>>();
            _ObjectLocatorLRUList   = new LinkedList<ObjectLocator>();

        }

        #endregion

        #endregion


        #region StoreINode(myINode, myObjectLocation, myIsPinned = false)

        public override Exceptional<INode> StoreINode(INode myINode, ObjectLocation myObjectLocation, Boolean myIsPinned = false)
        {

            Debug.Assert(_ObjectLocatorCache            != null);

            return base.StoreINode(myINode, myObjectLocation, myIsPinned);

        }

        #endregion

        #region StoreObjectLocator(myObjectLocator, myIsPinned = false)

        public override Exceptional<ObjectLocator> StoreObjectLocator(ObjectLocator myObjectLocator, Boolean myIsPinned = false)
        {

            Debug.Assert(_ObjectLocatorCache            != null);
            Debug.Assert(_ObjectLocatorLRUList          != null);
            Debug.Assert(myObjectLocator                != null);
            Debug.Assert(myObjectLocator.ObjectLocation != null);
            Debug.Assert(_AFSObjectStore                != null);

            lock (this)
            {

                if (_ObjectLocatorCache.ContainsKey(myObjectLocator.ObjectLocation))
                {
                    _ObjectLocatorLRUList.Remove(_ObjectLocatorCache[myObjectLocator.ObjectLocation]);
                    _ObjectLocatorCache[myObjectLocator.ObjectLocation] = _ObjectLocatorLRUList.AddLast(myObjectLocator);
                    return new Exceptional<ObjectLocator>();
                }

                if ((UInt64) _ObjectLocatorLRUList.Count >= Capacity)
                {
                    _ObjectLocatorCache.Remove(_ObjectLocatorLRUList.First.Value.ObjectLocation);
                    _ObjectLocatorLRUList.RemoveFirst();
                }

                _ObjectLocatorCache.Add(myObjectLocator.ObjectLocation, _ObjectLocatorLRUList.AddLast(myObjectLocator));

                return new Exceptional<ObjectLocator>(myObjectLocator);

            }

        }

        #endregion

        #region StoreAFSObject(myAFSObject, myIsPinned = false)

        public override Exceptional<AFSObject> StoreAFSObject(AFSObject myAFSObject, Boolean myIsPinned = false)
        {

            Debug.Assert(_ObjectLocatorCache != null);

            return base.StoreAFSObject(myAFSObject, myIsPinned);

        }

        #endregion


        #region GetINode(myObjectLocation)

        public override Exceptional<INode> GetINode(ObjectLocation myObjectLocation)
        {
            return base.GetINode(myObjectLocation);
        }

        #endregion

        #region GetObjectLocator(myObjectLocation)

        public override Exceptional<ObjectLocator> GetObjectLocator(ObjectLocation myObjectLocation)
        {

            Debug.Assert(myObjectLocation       != null);
            Debug.Assert(_ObjectLocatorCache    != null);
            Debug.Assert(_ObjectLocatorLRUList  != null);

            lock (this)
            {

                LinkedListNode<ObjectLocator> _ObjectLocatorNode = null;

                if (_ObjectLocatorCache.TryGetValue(myObjectLocation, out _ObjectLocatorNode))
                    if (_ObjectLocatorNode != null)
                        return new Exceptional<ObjectLocator>(_ObjectLocatorNode.Value);

                //ToDo: This might be a bit too expensive!
                return new Exceptional<ObjectLocator>(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));

            }

        }

        #endregion

        #region GetAFSObject<PT>(myCacheUUID)

        public override Exceptional<PT> GetAFSObject<PT>(CacheUUID myCacheUUID)
        {
            return base.GetAFSObject<PT>(myCacheUUID);
        }

        #endregion


        #region Copy(mySourceLocation, myTargetLocation)

        public override Exceptional CopyToLocation(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation)
        {

            Debug.Assert(_ObjectLocatorCache    != null);
            Debug.Assert(mySourceLocation       != null);
            Debug.Assert(myTargetLocation       != null);

            return base.CopyToLocation(mySourceLocation, myTargetLocation);

        }

        #endregion

        #region Move(mySourceLocation, myTargetLocation)

        public override Exceptional MoveToLocation(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation)
        {

            Debug.Assert(_ObjectLocatorCache    != null);
            Debug.Assert(mySourceLocation       != null);
            Debug.Assert(myTargetLocation       != null);

            return base.MoveToLocation(mySourceLocation, myTargetLocation);

        }

        #endregion


        #region RemoveObjectLocator(myObjectLocator, myRecursion = false)

        public override Exceptional RemoveObjectLocator(ObjectLocator myObjectLocator, Boolean myRecursion = false)
        {

            Debug.Assert(myObjectLocator                != null);
            Debug.Assert(myObjectLocator.ObjectLocation != null);
            Debug.Assert(_ObjectLocatorCache            != null);
            Debug.Assert(_ObjectLocatorLRUList          != null);
            Debug.Assert(_AFSObjectStore                != null);

            lock (this)
            {

                if (_ObjectLocatorCache.ContainsKey(myObjectLocator.ObjectLocation))
                {
                    _ObjectLocatorLRUList.Remove(_ObjectLocatorCache[myObjectLocator.ObjectLocation]);
                    _ObjectLocatorCache.Remove(myObjectLocator.ObjectLocation);
                }
                
                return Exceptional.OK;

            }

        }

        #endregion

        #region RemoveObjectLocation(myObjectLocation, myRecursion = false)

        public override Exceptional RemoveObjectLocation(ObjectLocation myObjectLocation, Boolean myRecursion = false)
        {

            Debug.Assert(_ObjectLocatorCache != null);
            
            return base.RemoveObjectLocation(myObjectLocation, myRecursion);

        }

        #endregion

        #region RemoveAFSObject(myCacheUUID)

        public override Exceptional RemoveAFSObject(CacheUUID myCacheUUID)
        {
            return base.RemoveAFSObject(myCacheUUID);
        }

        #endregion


        #region Clear()

        public Exceptional Clear()
        {

            base.Clear();

            _ObjectLocatorCache.Clear();

            return Exceptional.OK;

        }

        #endregion

    }

}
