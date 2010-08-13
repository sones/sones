/*
 * ObjectStore
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Linq;

using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;

using sones.Lib.DataStructures;
using sones.Lib.DataStructures.Big;
using sones.GraphFS.Caches;
using sones.Lib.ErrorHandling;
using System.Collections.Generic;
using sones.GraphFS.Errors;

#endregion

namespace sones.GraphFS
{

    /// <summary>
    /// Stores INodes, ObjectLocators and AFSObjects within
    /// the memory-only GraphFS1
    /// </summary>
    public class ObjectStore : IObjectCache
    {

        #region Data

        private BigDictionary<ObjectLocation, ObjectLocator> _ObjectLocatorLookuptable;
        private BigDictionary<CacheUUID, AFSObject>          _AFSObjectLookuptable;

        #endregion

        #region Properties

        #region IsEmpty

        public Boolean IsEmpty
        {
            get
            {
                return !_ObjectLocatorLookuptable.Any();
            }
        }

        #endregion

        #region NumberOfCachedItems

        public UInt64 NumberOfCachedItems
        {
            get
            {
                return (UInt64) _ObjectLocatorLookuptable.Count();
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        public ObjectStore()
        {
            _ObjectLocatorLookuptable = new BigDictionary<ObjectLocation, ObjectLocator>();
            _AFSObjectLookuptable     = new BigDictionary<CacheUUID, AFSObject>();
        }

        #endregion


        #region ObjectCacheSettings

        public ObjectCacheSettings ObjectCacheSettings { get; set; }

        #endregion


        #region StoreINode(myINode, myObjectLocation, myIsPinned = false)

        public Exceptional<INode> StoreINode(INode myINode, ObjectLocation myObjectLocation, Boolean myIsPinned = false)
        {

            var _Exceptional = GetObjectLocator(myObjectLocation);

            if (_Exceptional.Failed())
                return _Exceptional.Convert<INode>();

            _Exceptional.Value.INodeReference = myINode;

            return new Exceptional<INode>();

        }

        #endregion

        #region StoreObjectLocator(myObjectLocator, myIsPinned = false)

        public Exceptional<ObjectLocator> StoreObjectLocator(ObjectLocator myObjectLocator, Boolean myIsPinned = false)
        {

            if (_ObjectLocatorLookuptable.ContainsKey(myObjectLocator.ObjectLocation))
                _ObjectLocatorLookuptable[myObjectLocator.ObjectLocation] = myObjectLocator;

            else
                _ObjectLocatorLookuptable.Add(myObjectLocator.ObjectLocation, myObjectLocator);

            return new Exceptional<ObjectLocator>(myObjectLocator);

        }

        #endregion

        #region StoreAFSObject(myCacheUUID, myAFSObject, myIsPinned = false)

        public Exceptional<AFSObject> StoreAFSObject(CacheUUID myCacheUUID, AFSObject myAFSObject, Boolean myIsPinned = false)
        {

            if (_AFSObjectLookuptable.ContainsKey(myCacheUUID))
                _AFSObjectLookuptable[myCacheUUID] = myAFSObject;

            else
                _AFSObjectLookuptable.Add(myCacheUUID, myAFSObject);

            return new Exceptional<AFSObject>(myAFSObject);

        }

        #endregion


        #region GetINode(myObjectLocation)

        public Exceptional<INode> GetINode(ObjectLocation myObjectLocation)
        {

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

        public Exceptional<ObjectLocator> GetObjectLocator(ObjectLocation myObjectLocation)
        {

            ObjectLocator _ObjectLocator = null;

            if (_ObjectLocatorLookuptable.TryGetValue(myObjectLocation, out _ObjectLocator))
                if (_ObjectLocator != null)
                    return new Exceptional<ObjectLocator>(_ObjectLocator);

            //ToDo: This might be a bit too expensive!
            return new Exceptional<ObjectLocator>(new GraphFSError("Not within the ObjectCache!"));

        }

        #endregion

        #region GetAFSObject<PT>(myCacheUUID)

        public Exceptional<PT> GetAFSObject<PT>(CacheUUID myCacheUUID)
            where PT : AFSObject
        {

            AFSObject _AFSObject = null;

            if (_AFSObjectLookuptable.TryGetValue(myCacheUUID, out _AFSObject))
                if (_AFSObject != null)
                    return new Exceptional<PT>(_AFSObject as PT);

            //ToDo: This might be a bit too expensive!
            return new Exceptional<PT>(new GraphFSError("Not within the ObjectCache!"));

        }

        #endregion

        //#region HasObjectLocator(myObjectLocation)

        //public Exceptional<Boolean> HasObjectLocator(ObjectLocation myObjectLocation)
        //{
        //    return new Exceptional<Boolean>(_ObjectLocatorLookuptable.ContainsKey(myObjectLocation));
        //}

        //#endregion


        #region Copy(mySourceLocation, myTargetLocation, myRecursion = false)

        public Exceptional Copy(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation, Boolean myRecursion = false)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Move(mySourceLocation, myTargetLocation, myRecursion = false)

        public Exceptional Move(ObjectLocation mySourceLocation, ObjectLocation myTargetLocation, Boolean myRecursion = false)
        {

            lock (this)
            {

                if (_ObjectLocatorLookuptable.ContainsKey(mySourceLocation) &&
                    !_ObjectLocatorLookuptable.ContainsKey(myTargetLocation))
                {

                    #region Recursive move objects under this location!

                    if (myRecursion)
                    {

                        var _ListOfItemsToMove = (from _Item in _ObjectLocatorLookuptable where _Item.Key.StartsWith(mySourceLocation) select _Item).ToList();

                        foreach (var _ItemToMove in _ListOfItemsToMove)
                        {

                            var _OldLocation = _ItemToMove.Key.ToString();

                            // Move the ObjectLocator to the new ObjectLocation
                            var _ObjectLocator = _ObjectLocatorLookuptable[_ItemToMove.Key];
                            _ObjectLocator.ObjectLocation = new ObjectLocation(myTargetLocation, _OldLocation.Remove(0, mySourceLocation.Length));
                            _ObjectLocatorLookuptable.Add(new ObjectLocation(myTargetLocation, _OldLocation.Remove(0, mySourceLocation.Length)), _ObjectLocator);
                            _ObjectLocatorLookuptable.Remove(_ItemToMove.Key);


                            // Remove all objects at this location
                            foreach (var _StringStream in _ObjectLocator)
                            {

                                //if (_StringStream.Key == FSConstants.DIRECTORYSTREAM)
                                //    Remove(new ObjectLocation(myOldObjectLocation, _StringStream.Key), true);

                                foreach (var _StringEdition in _StringStream.Value)
                                    foreach (var _RevisionIDRevision in _StringEdition.Value)
                                    {

                                        AFSObject _Object = null;

                                        if (_AFSObjectLookuptable.TryGetValue(_RevisionIDRevision.Value.CacheUUID, out _Object))
                                        {
                                            var _OldObjectLocation = _Object.ObjectLocation.ToString();
                                            if (_OldObjectLocation.StartsWith(mySourceLocation.ToString() + FSPathConstants.PathDelimiter))
                                            {
                                                _Object.ObjectLocation = new ObjectLocation(myTargetLocation, _OldObjectLocation.Remove(0, mySourceLocation.Length));
                                            }
                                        }

                                    }

                            }

                        }

                    }

                    #endregion

                    #region Move objects at this location!

                    else
                    {

                        // Move the ObjectLocator to the new ObjectLocation
                        var _ObjectLocator = _ObjectLocatorLookuptable[mySourceLocation];
                        _ObjectLocatorLookuptable.Add(myTargetLocation, _ObjectLocator);
                        _ObjectLocatorLookuptable.Remove(mySourceLocation);

                        // Move all _Object.ObjectLocations to the new ObjectLocation
                        foreach (var _StringStream in _ObjectLocator)
                            foreach (var _StringEdition in _StringStream.Value)
                                foreach (var _RevisionIDRevision in _StringEdition.Value)
                                {

                                    AFSObject _Object = null;

                                    if (_AFSObjectLookuptable.TryGetValue(_RevisionIDRevision.Value.CacheUUID, out _Object))
                                    {
                                        var _OldLocation = _Object.ObjectLocation.ToString();
                                        if (_OldLocation.StartsWith(mySourceLocation))
                                        {
                                            _Object.ObjectLocation = new ObjectLocation(myTargetLocation, _OldLocation.Remove(0, mySourceLocation.Length));
                                        }
                                    }

                                }

                    }

                    #endregion

                }

            }

            return Exceptional.OK;

        }

        #endregion


        #region RemoveObjectLocator(myObjectLocator, myRecursion = false)

        public Exceptional RemoveObjectLocator(ObjectLocator myObjectLocator, Boolean myRecursion = false)
        {
            return RemoveObjectLocation(myObjectLocator.ObjectLocation, myRecursion);
        }

        #endregion

        #region RemoveObjectLocation(myObjectLocation, myRecursion = false)

        public Exceptional RemoveObjectLocation(ObjectLocation myObjectLocation, Boolean myRecursion = false)
        {

            lock (this)
            {

                if (_ObjectLocatorLookuptable.ContainsKey(myObjectLocation))
                {

                    #region Recursive remove objects under this location!

                    if (myRecursion)
                    {

                        // Remove all objects at this location
                        foreach (var _StringStream in _ObjectLocatorLookuptable[myObjectLocation])
                        {

                            if (_StringStream.Key == FSConstants.DIRECTORYSTREAM)
                                RemoveObjectLocation(new ObjectLocation(myObjectLocation, _StringStream.Key), true);

                            foreach (var _StringEdition in _StringStream.Value)
                                foreach (var _RevisionIDRevision in _StringEdition.Value)
                                    RemoveAFSObject(_RevisionIDRevision.Value.CacheUUID);

                        }

                        // Remove ObjectLocator
                        _ObjectLocatorLookuptable.Remove(myObjectLocation);

                    }

                    #endregion

                    #region Remove objects at this location!

                    else
                    {

                        // Remove all objects at this location
                        foreach (var _StringStream in _ObjectLocatorLookuptable[myObjectLocation])
                            foreach (var _StringEdition in _StringStream.Value)
                                foreach (var _RevisionIDRevision in _StringEdition.Value)
                                    RemoveAFSObject(_RevisionIDRevision.Value.CacheUUID);

                        // Remove ObjectLocator
                        _ObjectLocatorLookuptable.Remove(myObjectLocation);

                    }

                    #endregion

                }

            }

            return Exceptional.OK;

        }

        #endregion

        #region RemoveAFSObject(myCacheUUID)

        public Exceptional RemoveAFSObject(CacheUUID myCacheUUID)
        {

            if (_AFSObjectLookuptable.ContainsKey(myCacheUUID))
                _AFSObjectLookuptable.Remove(myCacheUUID);

            return Exceptional.OK;

        }

        #endregion


        #region Clear()

        public Exceptional Clear()
        {

            _ObjectLocatorLookuptable.Clear();

            return Exceptional.OK;

        }

        #endregion


    }

}
