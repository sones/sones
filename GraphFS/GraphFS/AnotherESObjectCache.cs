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

    public class AnotherESObjectCache : IObjectCache
    {

        #region Data

        private const    UInt64                                                                 _DefaultCapacity = 50000;
        private          UInt64                                                                 _FillLevel = 0;
        private readonly Dictionary<ObjectLocation, ObjectLocator>                              _ObjectLocatorCache;
        private readonly Dictionary<CacheUUID, AFSObject>                                       _AFSObjectStore;
        private readonly Dictionary<CacheUUID, UInt64>                                          _EstimatedAFSObjectSize;
        private readonly Boolean                                                                _talk = false;

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
                if (value > _DefaultCapacity)
                {
                    _Capacity = value;
                }
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
                    return (UInt64)_ObjectLocatorCache.Count;
                }
            }
        }

        #endregion

        #region FillLevel

        public UInt64 FillLevel
        {
            get
            {
                lock (this)
                {
                    return _FillLevel;
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

        public AnotherESObjectCache()
            : this(_DefaultCapacity)
        {
        }

        #endregion

        #region ObjectCache(myCapacity)

        public AnotherESObjectCache(UInt64 myCapacity, Boolean talk = false)
            : base()
        {

            _talk = talk;

            if (myCapacity < _DefaultCapacity)
            {
                // :)
                myCapacity = _DefaultCapacity;
            }

            _Capacity = myCapacity;
            _ObjectLocatorCache = new Dictionary<ObjectLocation, ObjectLocator>();
            _AFSObjectStore = new Dictionary<CacheUUID, AFSObject>();
            _EstimatedAFSObjectSize = new Dictionary<CacheUUID, UInt64>();

        }

        #endregion

        #endregion


        #region StoreINode(myINode, myObjectLocation, myIsPinned = false)

        public virtual Exceptional<INode> StoreINode(INode myINode, ObjectLocation myObjectLocation, Boolean myIsPinned = false)
        {

            Debug.Assert(myINode != null);
            Debug.Assert(myObjectLocation != null);
            Debug.Assert(_ObjectLocatorCache != null);

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

            Debug.Assert(_ObjectLocatorCache != null);
            Debug.Assert(myObjectLocator != null);
            Debug.Assert(myObjectLocator.ObjectLocation != null);
            Debug.Assert(_AFSObjectStore != null);

            lock (this)
            {

                if (_ObjectLocatorCache.ContainsKey(myObjectLocator.ObjectLocation))
                {
                    return new Exceptional<ObjectLocator>();
                }

                //the object doesn't fit into the cache
                if (myObjectLocator.GetEstimatedSize() > _Capacity)
                {
                    PrintTooBig(myObjectLocator.ToString(), myObjectLocator.GetEstimatedSize());
                    return new Exceptional<ObjectLocator>(myObjectLocator);
                }

                Boolean aufgeraeumt = false;

                while ((myObjectLocator.GetEstimatedSize() + _FillLevel) > _Capacity)
                {
                    ObjectLocator oldestEntry = GetRandomObjectLocatorThatIsNoRootObject();

                    if (oldestEntry == null)
                    {
                        return new Exceptional<ObjectLocator>(myObjectLocator);
                    }

                    var currentFillLevel = _FillLevel;
                    RemoveObjectLocator(oldestEntry);

                    aufgeraeumt = true;

                    PrintOut(currentFillLevel, _FillLevel, CalcActuallFillsize(), oldestEntry.ToString());

                    if (currentFillLevel <= _FillLevel)
                    {
                        //removal of some elemtents did not effect the FillLevel --> so the current element does not fit into the cache
                        return new Exceptional<ObjectLocator>(myObjectLocator);
                    }
                }

                if (aufgeraeumt)
                {
                    PrintAufgeraeumt(_FillLevel);
                }

                // Add new ObjectLocator to the ObjectCache
                if (!_ObjectLocatorCache.ContainsKey(myObjectLocator.ObjectLocation))
                {
                    _ObjectLocatorCache.Add(myObjectLocator.ObjectLocation, myObjectLocator);
                    IncreaseFillLevel(myObjectLocator.GetEstimatedSize());
                }

                return new Exceptional<ObjectLocator>(myObjectLocator);

            }

        }

        private void PrintTooBig(string p, ulong p_2)
        {
            if (_talk)
            {
                Console.WriteLine(String.Format("Objekt zu gross... Name: {0}, Groesse: {1}, Kapazitaet: {2}", p, p_2, _Capacity));
            }
        }

        private ObjectLocator GetRandomObjectLocatorThatIsNoRootObject()
        {
            return (from kv in _ObjectLocatorCache where kv.Key != ObjectLocation.Root select kv.Value).FirstOrDefault();
        }

        #endregion

        #region StoreAFSObject(myAFSObject, myIsPinned = false)

        public virtual Exceptional<AFSObject> StoreAFSObject(AFSObject myAFSObject, Boolean myIsPinned = false)
        {

            Debug.Assert(myAFSObject != null);
            Debug.Assert(myAFSObject.ObjectLocation != null);
            Debug.Assert(myAFSObject.ObjectLocatorReference != null);
            Debug.Assert(myAFSObject.ObjectStream != null);
            Debug.Assert(myAFSObject.ObjectEdition != null);
            Debug.Assert(myAFSObject.ObjectRevisionID != null);
            Debug.Assert(_ObjectLocatorCache != null);
            Debug.Assert(_AFSObjectStore != null);

            lock (this)
            {
                var _CacheUUID = myAFSObject.ObjectLocatorReference[myAFSObject.ObjectStream][myAFSObject.ObjectEdition][myAFSObject.ObjectRevisionID].CacheUUID;

                Debug.Assert(_CacheUUID != null);

                //the object doesn't fit into the cache
                if (myAFSObject.GetEstimatedSize() > _Capacity)
                {
                    PrintTooBig(myAFSObject.ToString(), myAFSObject.GetEstimatedSize());
                    return new Exceptional<AFSObject>(myAFSObject);
                }

                bool aufgeraeumt = false;

                while (_FillLevel + myAFSObject.GetEstimatedSize() > _Capacity)
                {
                    var oldestLocator = GetRandomObjectLocatorThatIsNoRootObject();

                    if (oldestLocator == null)
                    {
                        return new Exceptional<AFSObject>(myAFSObject);
                    }

                    var currentFillLevel = _FillLevel;
                    RemoveObjectLocator(oldestLocator);

                    aufgeraeumt = true;

                    PrintOut(currentFillLevel, _FillLevel, CalcActuallFillsize(), oldestLocator.ToString());

                    if (currentFillLevel <= _FillLevel)
                    {
                        //removal of some elemtents did not effect the FillLevel --> so the current element does not fit into the cache

                        return new Exceptional<AFSObject>(myAFSObject);
                    }

                }

                if (aufgeraeumt)
                {
                    PrintAufgeraeumt(_FillLevel);
                }

                IncreaseFillLevel(myAFSObject.GetEstimatedSize());

                if (_AFSObjectStore.ContainsKey(_CacheUUID))
                {
                    _AFSObjectStore[_CacheUUID] = myAFSObject;
                    _EstimatedAFSObjectSize[_CacheUUID] = myAFSObject.GetEstimatedSize();
                }
                else
                {
                    _AFSObjectStore.Add(_CacheUUID, myAFSObject);
                    _EstimatedAFSObjectSize.Add(_CacheUUID, myAFSObject.GetEstimatedSize());
                }

                if (!_ObjectLocatorCache.ContainsKey(myAFSObject.ObjectLocation))
                {
                    _ObjectLocatorCache.Add(myAFSObject.ObjectLocation, myAFSObject.ObjectLocatorReference);
                    IncreaseFillLevel(myAFSObject.ObjectLocatorReference.GetEstimatedSize());
                }
            }

            return new Exceptional<AFSObject>(myAFSObject);
        }

        private void PrintAufgeraeumt(ulong _FillLevel)
        {
            if (_talk)
            {
                Console.WriteLine(String.Format("Fertig mit aufraeumen... FillLevel: {0}", _FillLevel.ToString()));
            }
        }

        private void PrintOut(ulong currentFillLevel, ulong _FillLevel, ulong calculatedCacheSize, string p_2)
        {
            if (_talk)
            {
                Console.WriteLine(String.Format("Aufraeumen... Vor Aufräumen: {0}, Nach Aufräumen: {1}, Berechnete Cache Groesse: {2}, Geloeschtes Object: {3}", currentFillLevel, _FillLevel, calculatedCacheSize, p_2));
            }
        }

        private ulong CalcActuallFillsize()
        {
            UInt64 counter = 0;

            foreach (var aAFSItem in _AFSObjectStore)
            {
                counter += aAFSItem.Value.GetEstimatedSize();
            }

            foreach (var aOL in _ObjectLocatorCache)
            {
                counter += aOL.Value.GetEstimatedSize();
            }

            return counter;
        }

        #endregion


        #region GetINode(myObjectLocation)

        public virtual Exceptional<INode> GetINode(ObjectLocation myObjectLocation)
        {

            Debug.Assert(myObjectLocation != null);

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

            Debug.Assert(myObjectLocation != null);
            Debug.Assert(_ObjectLocatorCache != null);

            lock (this)
            {
                ObjectLocator _ObjectLocatorNode;

                if (_ObjectLocatorCache.TryGetValue(myObjectLocation, out _ObjectLocatorNode))
                {
                    if (_ObjectLocatorNode != null)
                    {
                        return new Exceptional<ObjectLocator>(_ObjectLocatorNode);
                    }
                }
                //ToDo: This might be a bit too expensive!
                return new Exceptional<ObjectLocator>(new GraphFSError_ObjectLocatorNotFound(myObjectLocation));

            }

        }

        #endregion

        #region GetAFSObject<PT>(myCacheUUID)

        public virtual Exceptional<PT> GetAFSObject<PT>(CacheUUID myCacheUUID)
            where PT : AFSObject
        {

            Debug.Assert(myCacheUUID != null);
            Debug.Assert(_AFSObjectStore != null);

            lock (this)
            {

                AFSObject _AFSObject = null;

                _AFSObjectStore.TryGetValue(myCacheUUID, out _AFSObject);

                if (_AFSObject != null)
                {
                    //update size
                    DecreaseFillLevel(_EstimatedAFSObjectSize[myCacheUUID]);
                    var newSize = _AFSObject.GetEstimatedSize();
                    _EstimatedAFSObjectSize[myCacheUUID] = newSize;
                    IncreaseFillLevel(newSize);

                    return new Exceptional<PT>(_AFSObject as PT);
                }

                //ToDo: This might be a bit too expensive!
                return new Exceptional<PT>(new GraphFSError("Not within the ObjectCache!"));

            }

        }

        #endregion


        #region Copy(mySourceLocation, myTargetLocation)

        public virtual Exceptional CopyToLocation(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation)
        {

            Debug.Assert(_ObjectLocatorCache != null);
            Debug.Assert(_AFSObjectStore != null);
            Debug.Assert(mySourceLocation != null);
            Debug.Assert(myTargetLocation != null);

            lock (this)
            {

                if (_ObjectLocatorCache.ContainsKey(mySourceLocation) &&
                    !_ObjectLocatorCache.ContainsKey(myTargetLocation))
                {

                    foreach (var _ItemToMove in (from _Item in _ObjectLocatorCache where _Item.Key.StartsWith(mySourceLocation.ToString()) select _Item).ToList())
                    {
                        // Copy the ObjectLocator to the new ObjectLocation
                        var _ObjectLocatorNode = _ObjectLocatorCache[_ItemToMove.Key];
                        var _NewLocation   = new ObjectLocation(myTargetLocation, _ItemToMove.Key.PathElements.Skip(mySourceLocation.PathElements.Count()));
                        _ObjectLocatorNode.ObjectLocationSetter = _NewLocation;

                        StoreObjectLocator(_ObjectLocatorNode);
                    }
                }
            }

            return Exceptional.OK;

        }

        #endregion

        #region Move(mySourceLocation, myTargetLocation)

        public virtual Exceptional MoveToLocation(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation)
        {

            Debug.Assert(_ObjectLocatorCache != null);
            Debug.Assert(_AFSObjectStore != null);
            Debug.Assert(mySourceLocation != null);
            Debug.Assert(myTargetLocation != null);

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
                        _ObjectLocatorNode.ObjectLocationSetter = _NewLocation;

                        //StoreObjectLocator(_ObjectLocatorNode);
                        _ObjectLocatorCache.Add(_ObjectLocatorNode.ObjectLocation, _ObjectLocatorNode);
                        RemoveObjectLocation(_ItemToMove.Key);
                    }

                }

            }

            return Exceptional.OK;

        }

        #endregion


        #region RemoveObjectLocator(myObjectLocator, myRecursion = false)

        public virtual Exceptional RemoveObjectLocator(ObjectLocator myObjectLocator, Boolean myRecursion = false)
        {

            Debug.Assert(myObjectLocator != null);
            Debug.Assert(myObjectLocator.ObjectLocation != null);
            Debug.Assert(_ObjectLocatorCache != null);
            Debug.Assert(_AFSObjectStore != null);

            return RemoveObjectLocation(myObjectLocator.ObjectLocation, myRecursion);

        }

        #endregion

        #region RemoveObjectLocation(myObjectLocation, myRecursion = false, myRemovedSize = 0)

        public virtual Exceptional RemoveObjectLocation(ObjectLocation myObjectLocation, Boolean myRecursion = false)
        {

            Debug.Assert(myObjectLocation != null);
            Debug.Assert(_ObjectLocatorCache != null);
            Debug.Assert(_AFSObjectStore != null);

            lock (this)
            {
                if (_ObjectLocatorCache.ContainsKey(myObjectLocation))
                {

                    var _ObjectLocatorNode = _ObjectLocatorCache[myObjectLocation];

                    #region Recursive remove objects under this location!

                    if (myRecursion)
                    {
                        // Remove all objects at this location
                        foreach (var _String_ObjectStream_Pair in _ObjectLocatorNode)
                        {
                            // Remove subordinated ObjectLocations recursively!
                            if (_String_ObjectStream_Pair.Key == FSConstants.DIRECTORYSTREAM)
                            {
                                RemoveObjectLocation(new ObjectLocation(myObjectLocation, _String_ObjectStream_Pair.Key), true);
                            }

                            foreach (var _String_ObjectEdition_Pair in _String_ObjectStream_Pair.Value)
                            {
                                foreach (var _RevisionIDRevision in _String_ObjectEdition_Pair.Value)
                                {
                                    RemoveAFSObject(_RevisionIDRevision.Value.CacheUUID);
                                }
                            }
                        }

                        // Remove ObjectLocator
                        if (_ObjectLocatorCache.Remove(myObjectLocation))
                        {
                            DecreaseFillLevel(_ObjectLocatorNode.GetEstimatedSize());
                        }
                    }

                    #endregion

                    #region Remove objects at this location!

                    else
                    {

                        // Remove all objects at this location
                        foreach (var _StringStream in _ObjectLocatorNode)
                        {
                            foreach (var _StringEdition in _StringStream.Value)
                            {
                                foreach (var _RevisionIDRevision in _StringEdition.Value)
                                {
                                    RemoveAFSObject(_RevisionIDRevision.Value.CacheUUID);
                                }
                            }
                        }

                        if (_ObjectLocatorCache.Remove(myObjectLocation))
                        {
                            DecreaseFillLevel(_ObjectLocatorNode.GetEstimatedSize());
                        }
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

            Debug.Assert(myCacheUUID != null);
            Debug.Assert(_AFSObjectStore != null);

            AFSObject remObject = null;

            lock (this)
            {
                if (_AFSObjectStore.TryGetValue(myCacheUUID, out remObject))
                {

                    _AFSObjectStore.Remove(myCacheUUID);

                    DecreaseFillLevel(_EstimatedAFSObjectSize[myCacheUUID]);

                    _EstimatedAFSObjectSize.Remove(myCacheUUID);

                    if (_ObjectLocatorCache.Remove(remObject.ObjectLocation))
                    {
                        DecreaseFillLevel(remObject.ObjectLocatorReference.GetEstimatedSize());
                    }
                }
            }

            return Exceptional.OK;
        }

        #endregion


        #region Clear()

        public Exceptional Clear()
        {

            Debug.Assert(_ObjectLocatorCache != null);
            Debug.Assert(_AFSObjectStore != null);

            lock (this)
            {

                _ObjectLocatorCache.Clear();
                _AFSObjectStore.Clear();
                _EstimatedAFSObjectSize.Clear();
                _FillLevel = 0;

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
                yield return new KeyValuePair<ObjectLocation, ObjectLocator>(_KeyValuePair.Key, _KeyValuePair.Value);
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
                yield return new KeyValuePair<ObjectLocation, ObjectLocator>(_KeyValuePair.Key, _KeyValuePair.Value);
            }
        }

        #endregion

        private void IncreaseFillLevel(UInt64 size)
        {
            _FillLevel += size;
        }

        private void DecreaseFillLevel(UInt64 size)
        {
            _FillLevel -= size;
        }

    }

}