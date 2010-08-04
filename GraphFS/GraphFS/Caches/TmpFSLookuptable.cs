/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

/*
 * TmpFSLookuptable
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.Lib.DataStructures.Big;

#endregion

namespace sones.TmpFS.Caches
{

    /// <summary>
    /// Stores ObjectLocators and Objects within a TmpFS
    /// </summary>
    public class TmpFSLookuptable
    {

        #region Data

        private BigDictionary<ObjectLocation, ObjectLocator> _ObjectLocatorLookuptable;
        private BigDictionary<CacheUUID, AFSObject> _APandoraObjectLookuptable;

        #endregion

        #region Constructor

        public TmpFSLookuptable()
        {
            _ObjectLocatorLookuptable  = new BigDictionary<ObjectLocation, ObjectLocator>();
            _APandoraObjectLookuptable = new BigDictionary<CacheUUID, AFSObject>();
        }

        #endregion


        #region Set(myObjectLocation, myObjectLocator)

        public void Set(ObjectLocation myObjectLocation, ObjectLocator myObjectLocator)
        {

            if (_ObjectLocatorLookuptable.ContainsKey(myObjectLocation))
                _ObjectLocatorLookuptable[myObjectLocation] = myObjectLocator;

            else
                _ObjectLocatorLookuptable.Add(myObjectLocation, myObjectLocator);

        }

        #endregion

        #region Set(myObjectLocation, myAPandoraObject)

        public void Set(CacheUUID myCacheUUID, AFSObject myAPandoraObject)
        {

            if (_APandoraObjectLookuptable.ContainsKey(myCacheUUID))
                _APandoraObjectLookuptable[myCacheUUID] = myAPandoraObject;

            else
                _APandoraObjectLookuptable.Add(myCacheUUID, myAPandoraObject);

        }

        #endregion

       
        #region HasObjectLocator(myObjectLocation)

        public Boolean HasObjectLocator(ObjectLocation myObjectLocation)
        {
            return _ObjectLocatorLookuptable.ContainsKey(myObjectLocation);
        }

        #endregion
       
        #region GetObjectLocator(myObjectLocation)

        public ObjectLocator GetObjectLocator(ObjectLocation myObjectLocation)
        {

            ObjectLocator _ObjectLocator = null;

            if (_ObjectLocatorLookuptable.TryGetValue(myObjectLocation, out _ObjectLocator))
                return _ObjectLocator;

            return null;

        }

        #endregion

        #region GetAPandoraObject(myCacheUUID)

        public AFSObject GetAFSObject(CacheUUID myCacheUUID)
        {

            AFSObject _APandoraObject = null;

            if (_APandoraObjectLookuptable.TryGetValue(myCacheUUID, out _APandoraObject))
                return _APandoraObject;

            return null;

        }

        #endregion


        #region Move(myOldObjectLocation, myNewObjectLocation, myRecursive)

        public void Move(ObjectLocation myOldObjectLocation, ObjectLocation myNewObjectLocation, Boolean myRecursive)
        {

            lock (this)
            {

                if ( _ObjectLocatorLookuptable.ContainsKey(myOldObjectLocation) &&
                    !_ObjectLocatorLookuptable.ContainsKey(myNewObjectLocation))
                {

                    #region Recursive move objects under this location!

                    if (myRecursive)
                    {

                        var _ListOfItemsToMove = (from _Item in _ObjectLocatorLookuptable where _Item.Key.StartsWith(myOldObjectLocation) select _Item).ToList();

                        foreach (var _ItemToMove in _ListOfItemsToMove)
                        {

                            var _OldLocation = _ItemToMove.Key.ToString();

                            // Move the ObjectLocator to the new ObjectLocation
                            var _ObjectLocator = _ObjectLocatorLookuptable[_ItemToMove.Key];
                            _ObjectLocator.ObjectLocation = new ObjectLocation(myNewObjectLocation, _OldLocation.Remove(0, myOldObjectLocation.Length));
                            _ObjectLocatorLookuptable.Add(new ObjectLocation(myNewObjectLocation, _OldLocation.Remove(0, myOldObjectLocation.Length)), _ObjectLocator);
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

                                        if (_APandoraObjectLookuptable.TryGetValue(_RevisionIDRevision.Value.CacheUUID, out _Object))
                                        {
                                            var _OldObjectLocation = _Object.ObjectLocation.ToString();
                                            if (_OldObjectLocation.StartsWith(myOldObjectLocation.ToString() + FSPathConstants.PathDelimiter))
                                            {
                                                _Object.ObjectLocation = new ObjectLocation(myNewObjectLocation, _OldObjectLocation.Remove(0, myOldObjectLocation.Length));
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
                        var _ObjectLocator = _ObjectLocatorLookuptable[myOldObjectLocation];
                        _ObjectLocatorLookuptable.Add(myNewObjectLocation, _ObjectLocator);
                        _ObjectLocatorLookuptable.Remove(myOldObjectLocation);

                        // Move all _Object.ObjectLocations to the new ObjectLocation
                        foreach (var _StringStream in _ObjectLocator)
                            foreach (var _StringEdition in _StringStream.Value)
                                foreach (var _RevisionIDRevision in _StringEdition.Value)
                                {

                                    AFSObject _Object = null;

                                    if (_APandoraObjectLookuptable.TryGetValue(_RevisionIDRevision.Value.CacheUUID, out _Object))
                                    {
                                        var _OldLocation = _Object.ObjectLocation.ToString();
                                        if (_OldLocation.StartsWith(myOldObjectLocation))
                                        {
                                            _Object.ObjectLocation =  new ObjectLocation(myNewObjectLocation, _OldLocation.Remove(0, myOldObjectLocation.Length));
                                        }
                                    }

                                }

                    }

                    #endregion

                }

            }

        }

        #endregion

        #region Remove(myObjectLocation, myRecursive)

        public void Remove(ObjectLocation myObjectLocation, Boolean myRecursive)
        {

            lock (this)
            {

                if (_ObjectLocatorLookuptable.ContainsKey(myObjectLocation))
                {

                    #region Recursive remove objects under this location!
                    
                    if (myRecursive)
                    {

                        // Remove all objects at this location
                        foreach (var _StringStream in _ObjectLocatorLookuptable[myObjectLocation])
                        {

                            if (_StringStream.Key == FSConstants.DIRECTORYSTREAM)
                                Remove(new ObjectLocation(myObjectLocation, _StringStream.Key), true);

                            foreach (var _StringEdition in _StringStream.Value)
                                foreach (var _RevisionIDRevision in _StringEdition.Value)
                                    Remove(_RevisionIDRevision.Value.CacheUUID);

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
                                    Remove(_RevisionIDRevision.Value.CacheUUID);

                        // Remove ObjectLocator
                        _ObjectLocatorLookuptable.Remove(myObjectLocation);

                    }

                    #endregion

                }

            }

        }

        #endregion

        #region Remove(myCacheUUID)

        public void Remove(CacheUUID myCacheUUID)
        {
            if (_APandoraObjectLookuptable.ContainsKey(myCacheUUID))
                _APandoraObjectLookuptable.Remove(myCacheUUID);
        }

        #endregion

    }

}
