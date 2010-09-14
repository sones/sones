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
    public class LRUObjectCache : IObjectCache
    {

        #region Data

        //private ReaderWriterLockSlim _CacheItemReaderWriterLockSlim;

        public  static   UInt64                                                      MinCapacity     = 100;
        private const    UInt64                                                     _DefaultCapacity = 50000;
        private readonly Dictionary<ObjectLocation, LinkedListNode<ObjectLocator>>  _ObjectLocatorCache;
        private readonly LinkedList<ObjectLocator>                                  _ObjectLocatorLRUList;
        private readonly IDictionary<CacheUUID, AFSObject>                          _AFSObjectStore;

        public  event    EventHandler                                               DiscardingOldestItem;

        #endregion

        #region Properties

        #region IsEmpty

        public Boolean IsEmpty
        {
            get
            {
                lock (this)
                {
                    return !_ObjectLocatorCache.Any();
                }
            }
        }

        #endregion

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
                if (value >= MinCapacity)
                    _Capacity = value;
            }
        
        }

        #endregion

        #region NumberOfCachedItems

        public UInt64 NumberOfCachedItems
        {
            get
            {
                lock (this)
                {
                    return (UInt64) _ObjectLocatorCache.Count();
                }
            }
        }

        #endregion

        #region ObjectCacheSettings

        public ObjectCacheSettings ObjectCacheSettings { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        #region ObjectCache()

        public LRUObjectCache()
            : this(_DefaultCapacity)
        {
        }

        #endregion

        #region ObjectCache(myCapacity)

        public LRUObjectCache(UInt64 myCapacity)
            : base()
        {

            if (myCapacity < MinCapacity)
                throw new ArgumentException("myCapacity must be larger than zero!");

            _Capacity               = myCapacity;
            _ObjectLocatorCache     = new Dictionary<ObjectLocation, LinkedListNode<ObjectLocator>>();
            _ObjectLocatorLRUList   = new LinkedList<ObjectLocator>();
            _AFSObjectStore         = new Dictionary<CacheUUID, AFSObject>();

        }

        #endregion

        #endregion


        #region StoreINode(myINode, myObjectLocation, myIsPinned = false)

        public virtual Exceptional<INode> StoreINode(INode myINode, ObjectLocation myObjectLocation, Boolean myIsPinned = false)
        {

            Debug.Assert(myINode                != null);
            Debug.Assert(myObjectLocation       != null);
            Debug.Assert(_ObjectLocatorCache    != null);

            var _Exceptional = GetObjectLocator(myObjectLocation);

            if (_Exceptional.Failed())
                return _Exceptional.Convert<INode>();

            _Exceptional.Value.INodeReferenceSetter = myINode;

            return new Exceptional<INode>();

        }

        #endregion

        #region StoreObjectLocator(myObjectLocator, myIsPinned = false)

        public virtual Exceptional<ObjectLocator> StoreObjectLocator(ObjectLocator myObjectLocator, Boolean myIsPinned = false)
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
                    // Remove LinkedListNode from LRUList and update ObjectLocator within the ObjectCache
                    _ObjectLocatorLRUList.Remove(_ObjectLocatorCache[myObjectLocator.ObjectLocation]);
                    _ObjectLocatorCache[myObjectLocator.ObjectLocation] = _ObjectLocatorLRUList.AddLast(myObjectLocator);
                    return new Exceptional<ObjectLocator>();
                }

                if (_ObjectLocatorLRUList.ULongCount() >= Capacity)
                {
                    // Remove oldest LinkedListNode from LRUList and add new ObjectLocator to the ObjectCache
                    if (DiscardingOldestItem != null)
                        DiscardingOldestItem(this, new EventArgs());
                    _ObjectLocatorCache.Remove(_ObjectLocatorLRUList.First.Value.ObjectLocation);
                    _ObjectLocatorLRUList.RemoveFirst();
                }

                // Add new ObjectLocator to the ObjectCache and add it to the LRUList
                _ObjectLocatorCache.Add(myObjectLocator.ObjectLocation, _ObjectLocatorLRUList.AddLast(myObjectLocator));

                return new Exceptional<ObjectLocator>(myObjectLocator);

            }

        }

        #endregion

        #region StoreAFSObject(myAFSObject, myIsPinned = false)

        public virtual Exceptional<AFSObject> StoreAFSObject(AFSObject myAFSObject, Boolean myIsPinned = false)
        {

            Debug.Assert(myAFSObject                        != null);
            Debug.Assert(myAFSObject.ObjectLocation         != null);
            Debug.Assert(myAFSObject.ObjectLocatorReference != null);
            Debug.Assert(myAFSObject.ObjectStream           != null);
            Debug.Assert(myAFSObject.ObjectEdition          != null);
            Debug.Assert(myAFSObject.ObjectRevisionID       != null);
            Debug.Assert(_ObjectLocatorCache                != null);
            Debug.Assert(_AFSObjectStore                    != null);

            lock (this)
            {

                var _CacheUUID = myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition][myAFSObject.ObjectRevisionID].CacheUUID;
                Debug.Assert(_CacheUUID != null);

                if (_AFSObjectStore.ContainsKey(_CacheUUID))
                    _AFSObjectStore[_CacheUUID] = myAFSObject;

                else
                    _AFSObjectStore.Add(_CacheUUID, myAFSObject);

                return new Exceptional<AFSObject>(myAFSObject);
            
            }

        }

        #endregion


        #region GetINode(myObjectLocation)

        public virtual Exceptional<INode> GetINode(ObjectLocation myObjectLocation)
        {
            
            Debug.Assert(myObjectLocation   != null);

            var _Exceptional = GetObjectLocator(myObjectLocation);

            if (_Exceptional.Failed())
                return _Exceptional.Convert<INode>();

            if (_Exceptional.Value.INodeReference != null)
                return new Exceptional<INode>(_Exceptional.Value.INodeReference);

            //ToDo: This might be a bit too expensive!
            return new Exceptional<INode>(new GraphFSError("Not within the ObjectCache!"));

        }

        #endregion

        #region GetObjectLocator(myObjectLocation)

        public virtual Exceptional<ObjectLocator> GetObjectLocator(ObjectLocation myObjectLocation)
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

        public virtual Exceptional<PT> GetAFSObject<PT>(CacheUUID myCacheUUID)
            where PT : AFSObject
        {

            Debug.Assert(myCacheUUID        != null);
            Debug.Assert(_AFSObjectStore    != null);

            lock (this)
            {

                AFSObject _AFSObject = null;

                if (_AFSObjectStore.TryGetValue(myCacheUUID, out _AFSObject))
                    if (_AFSObject != null)
                        return new Exceptional<PT>(_AFSObject as PT);

                //ToDo: This might be a bit too expensive!
                return new Exceptional<PT>(new GraphFSError("Not within the ObjectCache!"));

            }

        }

        #endregion


        #region Copy(mySourceLocation, myTargetLocation)

        public virtual Exceptional CopyToLocation(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation)
        {

            Debug.Assert(_ObjectLocatorCache    != null);
            Debug.Assert(_AFSObjectStore        != null);
            Debug.Assert(mySourceLocation       != null);
            Debug.Assert(myTargetLocation       != null);

            lock (this)
            {

                if (_ObjectLocatorCache.ContainsKey(mySourceLocation) &&
                    !_ObjectLocatorCache.ContainsKey(myTargetLocation))
                {

                    foreach (var _ItemToMove in from _Item in _ObjectLocatorCache where _Item.Key.StartsWith(mySourceLocation.ToString()) select _Item)
                    {

                        // Copy the ObjectLocator to the new ObjectLocation
                        var _ObjectLocatorNode = _ObjectLocatorCache[_ItemToMove.Key];
                        var _NewLocation   = new ObjectLocation(myTargetLocation, _ItemToMove.Key.PathElements.Skip(mySourceLocation.PathElements.Count()));
                        _ObjectLocatorNode.Value.ObjectLocationSetter = _NewLocation;
                        _ObjectLocatorCache.Add(_NewLocation, _ObjectLocatorNode);

                    }

                }

            }

            return Exceptional.OK;

        }

        #endregion

        #region Move(mySourceLocation, myTargetLocation)

        public virtual Exceptional MoveToLocation(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation)
        {

            Debug.Assert(_ObjectLocatorCache    != null);
            Debug.Assert(_AFSObjectStore        != null);
            Debug.Assert(mySourceLocation       != null);
            Debug.Assert(myTargetLocation       != null);

            lock (this)
            {

                if (_ObjectLocatorCache.ContainsKey(mySourceLocation) &&
                    !_ObjectLocatorCache.ContainsKey(myTargetLocation))
                {

                    // Get a copy of all ObjectsLocations to move, as we will modify the list later...
                    var _ListOfItemsToMove = (from _Item in _ObjectLocatorCache where _Item.Key.StartsWith(mySourceLocation.ToString()) select _Item).ToList();

                    foreach (var _ItemToMove in _ListOfItemsToMove)
                    {

                        // Move the ObjectLocator to the new ObjectLocation
                        var _ObjectLocatorNode = _ObjectLocatorCache[_ItemToMove.Key];
                        var _NewLocation   = new ObjectLocation(myTargetLocation, _ItemToMove.Key.PathElements.Skip(mySourceLocation.PathElements.Count()));
                        _ObjectLocatorNode.Value.ObjectLocationSetter = _NewLocation;
                        _ObjectLocatorCache.Add(_NewLocation, _ObjectLocatorNode);
                        _ObjectLocatorCache.Remove(_ItemToMove.Key);

                        // No longer needed... as all AFSObjects ask the ObjectLocator for their ObjectLocation
                        //var _OldLocation = _ItemToMove.Key.ToString();
                        //// Remove all objects at this location
                        //foreach (var _StringStream in _ObjectLocator)
                        //{

                        //    //if (_StringStream.Key == FSConstants.DIRECTORYSTREAM)
                        //    //    Remove(new ObjectLocation(myOldObjectLocation, _StringStream.Key), true);

                        //    foreach (var _StringEdition in _StringStream.Value)
                        //        foreach (var _RevisionIDRevision in _StringEdition.Value)
                        //        {

                        //            AFSObject _Object = null;

                        //            if (_AFSObjectLookuptable.TryGetValue(_RevisionIDRevision.Value.CacheUUID, out _Object))
                        //            {
                        //                var _OldObjectLocation = _Object.ObjectLocation.ToString();
                        //                if (_OldObjectLocation.StartsWith(mySourceLocation.ToString() + FSPathConstants.PathDelimiter))
                        //                {
                        //                    _Object.ObjectLocation = new ObjectLocation(myTargetLocation, _OldObjectLocation.Remove(0, mySourceLocation.Length));
                        //                }
                        //            }

                        //        }

                        //}

                    }

                }

            }

            return Exceptional.OK;

        }

        #endregion


        #region RemoveObjectLocator(myObjectLocator, myRecursion = false)

        public virtual Exceptional RemoveObjectLocator(ObjectLocator myObjectLocator, Boolean myRecursion = false)
        {

            Debug.Assert(myObjectLocator                != null);
            Debug.Assert(myObjectLocator.ObjectLocation != null);
            Debug.Assert(_ObjectLocatorCache            != null);
            Debug.Assert(_AFSObjectStore                != null);

            return RemoveObjectLocation(myObjectLocator.ObjectLocation, myRecursion);

        }

        #endregion

        #region RemoveObjectLocation(myObjectLocation, myRecursion = false)

        public virtual Exceptional RemoveObjectLocation(ObjectLocation myObjectLocation, Boolean myRecursion = false)
        {

            Debug.Assert(myObjectLocation       != null);
            Debug.Assert(_ObjectLocatorCache    != null);
            Debug.Assert(_AFSObjectStore        != null);

            lock (this)
            {

                if (_ObjectLocatorCache.ContainsKey(myObjectLocation))
                {

                    #region Recursive remove objects under this location!

                    if (myRecursion)
                    {

                        // Remove all objects at this location
                        foreach (var _StringStream in _ObjectLocatorCache[myObjectLocation].Value)
                        {

                            if (_StringStream.Key == FSConstants.DIRECTORYSTREAM)
                                RemoveObjectLocation(new ObjectLocation(myObjectLocation, _StringStream.Key), true);

                            foreach (var _StringEdition in _StringStream.Value)
                                foreach (var _RevisionIDRevision in _StringEdition.Value)
                                    RemoveAFSObject(_RevisionIDRevision.Value.CacheUUID);

                        }

                        // Remove ObjectLocator
                        _ObjectLocatorCache.Remove(myObjectLocation);

                    }

                    #endregion

                    #region Remove objects at this location!

                    else
                    {

                        // Remove all objects at this location
                        foreach (var _StringStream in _ObjectLocatorCache[myObjectLocation].Value)
                            foreach (var _StringEdition in _StringStream.Value)
                                foreach (var _RevisionIDRevision in _StringEdition.Value)
                                    RemoveAFSObject(_RevisionIDRevision.Value.CacheUUID);

                        // Remove ObjectLocator
                        _ObjectLocatorCache.Remove(myObjectLocation);

                    }

                    #endregion

                }

            }

            return Exceptional.OK;

        }

        #endregion

        #region RemoveAFSObject(myCacheUUID)

        public virtual Exceptional RemoveAFSObject(CacheUUID myCacheUUID)
        {

            Debug.Assert(myCacheUUID        != null);
            Debug.Assert(_AFSObjectStore    != null);

            lock (this)
            {

                if (_AFSObjectStore.ContainsKey(myCacheUUID))
                    _AFSObjectStore.Remove(myCacheUUID);

                return Exceptional.OK;

            }

        }

        #endregion


        #region Clear()

        public Exceptional Clear()
        {

            Debug.Assert(_ObjectLocatorCache    != null);
            Debug.Assert(_AFSObjectStore        != null);

            lock (this)
            {

                _ObjectLocatorCache.Clear();
                _AFSObjectStore.Clear();

                return Exceptional.OK;

            }

        }

        #endregion


        #region IEnumerable Members

        /// <summary>
        /// Iterates through the items of the LRUObjectCache.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var _KeyValuePair in _ObjectLocatorCache)
            {
                yield return new KeyValuePair<ObjectLocation, ObjectLocator>(_KeyValuePair.Key, _KeyValuePair.Value.Value);
            }
        }

        #endregion

        #region IEnumerable<KeyValuePair<ObjectLocation, ObjectLocator>> Members

        /// <summary>
        /// Iterates through the items of the LRUObjectCache.
        /// </summary>
        IEnumerator<KeyValuePair<ObjectLocation, ObjectLocator>> IEnumerable<KeyValuePair<ObjectLocation, ObjectLocator>>.GetEnumerator()
        {
            foreach (var _KeyValuePair in _ObjectLocatorCache)
            {
                yield return new KeyValuePair<ObjectLocation, ObjectLocator>(_KeyValuePair.Key, _KeyValuePair.Value.Value);
            }
        }

        #endregion


    }

}
