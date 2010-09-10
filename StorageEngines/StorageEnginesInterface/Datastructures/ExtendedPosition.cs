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

/* GraphFS - ExtendedPosition
 * (c) Achim Friedland, 2008 - 2010
 *  
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;

#endregion

namespace sones.StorageEngines
{

    /// <summary>
    /// An ExtendedPosition is a file system position consisting of a
    /// StorageID and a position within this storage.
    /// </summary>

    
    public struct ExtendedPosition : IComparable, IComparable<ExtendedPosition>, IEquatable<ExtendedPosition>
    {


        #region Properties

        #region StorageUUID

        private StorageUUID _StorageUUID;
        
        /// <summary>
        /// 0 == uset, 1 == thisFilesystem, >2 == StorageUUID
        /// </summary>
        public StorageUUID StorageUUID
        {

            get
            {
                return _StorageUUID;
            }

            set
            {
                _StorageUUID = value;
            }

        }

        #endregion

        #region Position

        private UInt64 _Position;

        public UInt64 Position
        {

            get
            {
                return _Position;
            }

            set
            {
                _Position = value;
            }

        }

        #endregion

        #endregion


        #region Constructor

        #region ExtendedPosition(myPosition)

        /// <summary>
        /// Creates a new ExtendedPosition based on the given position. Will
        /// use the default StorageUUID!
        /// </summary>
        public ExtendedPosition(UInt64 myPosition)
        {
            _StorageUUID    = new StorageUUID(0UL);
            _Position       = myPosition;
        }

        #endregion

        #region ExtendedPosition(myStorageUUID, myPosition)

        /// <summary>
        /// Creates a new ExtendedPosition based on the given parameters
        /// </summary>
        public ExtendedPosition(StorageUUID myStorageUUID, UInt64 myPosition)
        {
            _StorageUUID    = myStorageUUID;
            _Position       = myPosition;
        }

        #endregion

        #region ExtendedPosition(myExtendedPosition)

        /// <summary>
        /// Creates a new ExtendedPosition based on the content of myExtendedPosition
        /// </summary>
        /// <param name="myExtendedPosition">The ExtendedPosition to be cloned</param>
        public ExtendedPosition(ExtendedPosition myExtendedPosition)
        {
            #if(__MonoCS__)
            _StorageUUID    = (StorageUUID)myExtendedPosition.StorageUUID.Clone();
            #else
            _StorageUUID    = new StorageUUID(myExtendedPosition.StorageUUID);
            #endif
            _Position       = myExtendedPosition.Position;
        }

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (myExtendedPosition1, myExtendedPosition2)

        public static Boolean operator == (ExtendedPosition myExtendedPosition1, ExtendedPosition myExtendedPosition2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myExtendedPosition1, myExtendedPosition2))
                return true;

            // If one is null, but not both, return false.
            if (( (Object) myExtendedPosition1 == null) || ( (Object) myExtendedPosition2 == null))
                return false;

            return myExtendedPosition1.Equals(myExtendedPosition2);

        }

        #endregion

        #region Operator != (myExtendedPosition1, myExtendedPosition2)

        public static Boolean operator != (ExtendedPosition myExtendedPosition1, ExtendedPosition myExtendedPosition2)
        {
            return !(myExtendedPosition1 == myExtendedPosition2);
        }

        #endregion

        #region Operator < (myExtendedPosition1, myExtendedPosition2)

        public static Boolean operator <  (ExtendedPosition myExtendedPosition1, ExtendedPosition myExtendedPosition2)
        {

            if (myExtendedPosition1.StorageUUID != myExtendedPosition2.StorageUUID)
                return false;

            if (myExtendedPosition1.Position  <  myExtendedPosition2.Position)
                return true;

            return false;

        }

        #endregion

        #region Operator > (myExtendedPosition1, myExtendedPosition2)

        public static Boolean operator >  (ExtendedPosition myExtendedPosition1, ExtendedPosition myExtendedPosition2)
        {

            if (myExtendedPosition1.StorageUUID != myExtendedPosition2.StorageUUID)
                return false;

            if (myExtendedPosition1.Position > myExtendedPosition2.Position)
                return true;

            return false;

        }

        #endregion

        #region Operator <= (myExtendedPosition1, myExtendedPosition2)

        public static Boolean operator <= (ExtendedPosition myExtendedPosition1, ExtendedPosition myExtendedPosition2)
        {
            return !(myExtendedPosition1 > myExtendedPosition2);
        }

        #endregion

        #region Operator >= (myExtendedPosition1, myExtendedPosition2)

        public static Boolean operator >= (ExtendedPosition myExtendedPosition1, ExtendedPosition myExtendedPosition2)
        {
            return !(myExtendedPosition1 < myExtendedPosition2);
        }

        #endregion

        #endregion


        #region IComparable Members

        public Int32 CompareTo(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException();

            // If parameter cannot be cast to ExtendedPosition return false.
            ExtendedPosition _ExtendedPosition;

            try
            {
                _ExtendedPosition = (ExtendedPosition) myObject;
            }
            catch
            {
                throw new ArgumentException("myObject is not of type ExtendedPosition!");
            }

            if (this < _ExtendedPosition) return -1;
            if (this > _ExtendedPosition) return +1;

            return 0;

        }

        #endregion

        #region IComparable<ExtendedPosition> Members

        public Int32 CompareTo(ExtendedPosition myExtendedPosition)
        {

            // Check if myExtendedPosition is null
            if (myExtendedPosition == null)
                throw new ArgumentNullException();

            if (this < myExtendedPosition) return -1;
            if (this > myExtendedPosition) return +1;

            return 0;

        }

        #endregion

        #region IEquatable Members

        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                return false;

            // If parameter cannot be cast to ExtendedPosition return false.
            ExtendedPosition _ExtendedPosition;

            try
            {
                _ExtendedPosition = (ExtendedPosition)myObject;
            }
            catch
            {
                return false;
            }

            return Equals(_ExtendedPosition);

        }

        #endregion

        #region IEquatable<ObjectExtent> Members

        public Boolean Equals(ExtendedPosition myExtendedPosition)
        {

            // If parameter is null return false:
            if ((Object) myExtendedPosition == null)
                return false;

            // Check if the inner fields have the same values
            if (_Position != myExtendedPosition.Position)
                return false;

            if (_StorageUUID != myExtendedPosition.StorageUUID)
                return false;

            return true;

        }

        #endregion


        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return _StorageUUID.GetHashCode() ^ _Position.GetHashCode();
        }

        #endregion

        #region ToString()

        public override String ToString()
        {
            return _StorageUUID.ToString() + ":" + _Position;
        }

        #endregion


    }

}