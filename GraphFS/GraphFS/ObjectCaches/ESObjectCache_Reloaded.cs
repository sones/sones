#region Using

using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using sones.GraphFS.Errors;
using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;

using sones.Lib;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphFS
{

    /// <summary>
    /// An Last-Recently-Use ObjectCache implemantation of the IObjectCache interface
    /// for storing INodes, ObjectLocators and AFSObjects. This cache will remove the
    /// entries as soon as memory gets low or the stored items are getting very old.
    /// </summary>    

    public class ESObjectCache_Reloaded : ALRUObjectCache, IObjectCache
    {

        #region Data

        private          UInt64                         _FillLevel = 0;
        private readonly Dictionary<CacheUUID, UInt64>  _EstimatedAFSObjectSize;
        private readonly Dictionary<String, UInt64>     _EstimatedObjectLocatorSize;

        #endregion

        #region Properties

        #region CurrentLoad

        public override UInt64 CurrentLoad
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

        #endregion

        #region Constructor(s)

        #region ESObjectCache_Reloaded()

        public ESObjectCache_Reloaded()
            : base()
        {
            _EstimatedAFSObjectSize = new Dictionary<CacheUUID, UInt64>();
            _FillLevel              = 0;
        }

        #endregion

        #region ESObjectCache_Reloaded(myCapacity)

        public ESObjectCache_Reloaded(UInt64 myCapacity)
            : base(myCapacity)
        {
            _EstimatedAFSObjectSize = new Dictionary<CacheUUID, UInt64>();
            _FillLevel              = 0;
        }

        #endregion

        #endregion


        private void IncLevel(UInt64 myIncValue)
        {
            _FillLevel += myIncValue;
        }

        private void DecLevel(UInt64 myDecValue)
        {
            _FillLevel -= myDecValue;
        }

        private Boolean ValidateFillLevel()
        {

            var targetSize = 0UL;

            if (_EstimatedAFSObjectSize.Values.IsNotNullOrEmpty())
                targetSize += _EstimatedAFSObjectSize.Values.Aggregate((a, b) => a + b);

            if (_ObjectLocatorLRUList.IsNotNullOrEmpty())
                targetSize += _ObjectLocatorLRUList.Aggregate(0UL, (_Counter, _ObjectLocator) => _Counter + _ObjectLocator.GetEstimatedSize());

            if (targetSize != _FillLevel)
                return false;

            return (targetSize == _FillLevel);

        }

        public ICollection<AFSObject> GetObjects
        {

            get
            {
                return _AFSObjectStore.Values;
            }

        }



        #region (protected) StoreObjectLocator_protected(myObjectLocator, myIsPinned = false)

        protected override Exceptional<ObjectLocator> StoreObjectLocator_protected(ObjectLocator myObjectLocator, CachePriority myCachePriority = CachePriority.LOW)
        {

            Debug.Assert(_ObjectLocatorCache            != null);
            Debug.Assert(_ObjectLocatorLRUList          != null);
            Debug.Assert(myObjectLocator                != null);
            Debug.Assert(myObjectLocator.ObjectLocation != null);
            Debug.Assert(_AFSObjectStore                != null);

            var _ObjectLocatorSize = myObjectLocator.GetEstimatedSize();

            if (_ObjectLocatorSize > Capacity)
                return new Exceptional<ObjectLocator>(new GraphFSError("The ObjectLocator is too large for the ObjectCache!"));

            //ToDo: Here a endless-loop might happen!
            while (_ObjectLocatorSize + _FillLevel >= Capacity)
            {
                
                var _OldestLocator = _ObjectLocatorLRUList.First;

                if (_OldestLocator.Value.ObjectLocation == ObjectLocation.Root)
                {
                    _ObjectLocatorLRUList.RemoveFirst();
                    _ObjectLocatorLRUList.AddLast(_OldestLocator);
                    _OldestLocator = _ObjectLocatorLRUList.First;
                }

                RemoveObjectLocation(_OldestLocator.Value.ObjectLocation);

            }

            IncLevel(myObjectLocator.GetEstimatedSize());

            return new Exceptional<ObjectLocator>(myObjectLocator);

        }

        #endregion

        #region (protected) StoreAFSObject_protected(myAFSObject, myCacheUUID, myCachePriority = CachePriority.LOW)

        protected override Exceptional<AFSObject> StoreAFSObject_protected(AFSObject myAFSObject, CacheUUID myCacheUUID, CachePriority myCachePriority = CachePriority.LOW)
        {

            Debug.Assert(myAFSObject                        != null);
            Debug.Assert(myAFSObject.ObjectLocation         != null);
            Debug.Assert(myAFSObject.ObjectLocatorReference != null);
            Debug.Assert(myAFSObject.ObjectStream           != null);
            Debug.Assert(myAFSObject.ObjectEdition          != null);
            Debug.Assert(myAFSObject.ObjectRevisionID       != null);
            Debug.Assert(myCacheUUID                        != null);
            Debug.Assert(_AFSObjectStore                    != null);

            lock (this)
            {
                
                var _AFSObjectSize = myAFSObject.GetEstimatedSize();

                if (_AFSObjectSize > Capacity)
                    return new Exceptional<AFSObject>(new GraphFSError("The AFSObject is too large for the ObjectCache!"));

                //ToDo: Here a endless-loop might happen!
                while (_FillLevel + _AFSObjectSize >= Capacity)
                {

                    var _OldestLocator = _ObjectLocatorLRUList.First;

                    if (_OldestLocator.Value.ObjectLocation == ObjectLocation.Root)
                    {
                        _ObjectLocatorLRUList.RemoveFirst();
                        _ObjectLocatorLRUList.AddLast(_OldestLocator);
                        _OldestLocator = _ObjectLocatorLRUList.First;
                    }

                    RemoveObjectLocation(_OldestLocator.Value.ObjectLocation);

                }


                if (_EstimatedAFSObjectSize.ContainsKey(myCacheUUID))
                {
                    DecLevel(_EstimatedAFSObjectSize[myCacheUUID]);
                    _EstimatedAFSObjectSize[myCacheUUID] = _AFSObjectSize;
                    IncLevel(_AFSObjectSize);
                }

                else
                {
                    _EstimatedAFSObjectSize.Add(myCacheUUID, _AFSObjectSize);
                    IncLevel(_AFSObjectSize);
                }


                //if (!_ObjectLocatorCache.ContainsKey(myAFSObject.ObjectLocation))
                //{
                //    _ObjectLocatorCache.Add(myAFSObject.ObjectLocation, _ObjectLocatorLRUList.AddLast(myAFSObject.ObjectLocatorReference));
                //    IncLevel(myAFSObject.ObjectLocatorReference.GetEstimatedSize());
                //}

            }

            return new Exceptional<AFSObject>(myAFSObject);

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
                    var _ObjectLocation = _AFSObject.ObjectLocation;

                    if (_ObjectLocation != null)
                    {
                        var _ObjectLocatorNode = _ObjectLocatorCache[_ObjectLocation];

                        // Remove the ObjectLocator from LRU-list and readd it!                        
                        RemoveObjectLocator(_ObjectLocatorNode.Value);
                        StoreAFSObject(_AFSObject);
                    }

                    return new Exceptional<PT>(_AFSObject as PT);
                }

                //ToDo: This might be a bit too expensive!
                return new Exceptional<PT>(new GraphFSError("Not within the ObjectCache!"));

            }

        }

        #endregion


        #region Copy(mySourceLocation, myTargetLocation)

        public override Exceptional CopyToLocation(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation)
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
                        var _NewLocation = new ObjectLocation(myTargetLocation, _ItemToMove.Key.PathElements.Skip(mySourceLocation.PathElements.Count()));

                        _ObjectLocatorNode.Value.ObjectLocationSetter = _NewLocation;

                        var objectLocatorSize = _ObjectLocatorNode.Value.GetEstimatedSize();
                        while (_FillLevel + objectLocatorSize > Capacity)
                        {

                            var _LastObjectLocatorNode = _ObjectLocatorLRUList.First;

                            //MoveRootToEnd(_LastObjectLocatorNode);

                            _LastObjectLocatorNode = _ObjectLocatorLRUList.First;

                            if (_LastObjectLocatorNode.Value.ObjectLocation != ObjectLocation.Root)
                            {
                                RemoveObjectLocator(_LastObjectLocatorNode.Value);
                            }
                            else
                            {
                                return Exceptional.OK;
                            }
                        }

                        StoreObjectLocator(_ObjectLocatorNode.Value);

                    }
                }
            }

            return Exceptional.OK;

        }

        #endregion


        #region RemoveObjectLocation(myObjectLocation, myRecursion = false, myRemovedSize = 0)

        public override Exceptional RemoveObjectLocation(ObjectLocation myObjectLocation, Boolean myRecursion = false)
        {

            lock (this)
            {

                UInt64                        _EstimatedSize = 0;
                LinkedListNode<ObjectLocator> _ObjectLocatorNode;

                if (_ObjectLocatorCache.TryGetValue(myObjectLocation, out _ObjectLocatorNode))
                    _EstimatedSize = _ObjectLocatorNode.Value.GetEstimatedSize();

                var _Exceptional = base.RemoveObjectLocation(myObjectLocation, myRecursion);
                if (_Exceptional.Failed())
                    return _Exceptional;

                DecLevel(_EstimatedSize);

            }

            return Exceptional.OK;

        }

        #endregion

        #region RemoveAFSObject(myCacheUUID)

        public override Exceptional RemoveAFSObject(CacheUUID myCacheUUID)
        {

            lock (this)
            {

                var _EstimatedSize = _EstimatedAFSObjectSize[myCacheUUID];

                var _Exceptional = base.RemoveAFSObject(myCacheUUID);
                if (_Exceptional.Failed())
                    return _Exceptional;

                DecLevel(_EstimatedSize);

            }

            return Exceptional.OK;

        }

        #endregion


        #region Clear()

        public override Exceptional Clear()
        {

            lock (this)
            {

                base.Clear();

                _EstimatedAFSObjectSize.Clear();
                _FillLevel = 0;

                return Exceptional.OK;

            }

        }

        #endregion
        

    }

}