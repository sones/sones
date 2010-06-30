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


/* ObjectExtent
 * Achim Friedland, 2008 - 2010
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
    /// An ObjectExtent is part of the ObjectLocator describing
    /// where to find the allocated parts of a ObjectDatastream.
    /// </summary>

    

    public class ObjectExtent : IComparable, IComparable<ObjectExtent>, IEquatable<ObjectExtent>
    {


        #region Properties

        #region LogicalPosition

        private UInt64 _LogicalPosition;

        /// <summary>
        /// The logical position of this extent within the datastream.
        /// </summary>
        public UInt64 LogicalPosition
        {

            get
            {
                return _LogicalPosition;
            }
            
            set
            {
                _LogicalPosition = value;
            }

        }

        #endregion

        #region Length

        private UInt64 _Length;

        /// <summary>
        /// The length of this extent in bytes.
        /// </summary>
        public UInt64 Length
        {

            get
            {
                return _Length;
            }

            set
            {
                _Length = value;
            }

        }

        #endregion

        #region PhysicalPosition

        private UInt64 _PhysicalPosition;

        /// <summary>
        /// The physical position of this extent on the storage.
        /// </summary>
        public UInt64 PhysicalPosition
        {

            get
            {
                return _PhysicalPosition;
            }

            set
            {
                _PhysicalPosition = value;
            }

        }

        #endregion

        #region StorageUUID

        private StorageUUID _StorageUUID;

        /// <summary>
        /// The UUID of the storage this extent is stored at.
        /// 0 == uset, 1 == thisFilesystem, >2 == StorageID
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

        #region NextExtent

        private ExtendedPosition _NextExtent;

        /// <summary>
        /// For data safety all extents should be linked together to support
        /// crash recovery if the ObjectLocator is invalid.
        /// </summary>
        public ExtendedPosition NextExtent
        {

            get
            {

                if (_NextExtent == null)
                    return new ExtendedPosition(0, 0);
                
                return _NextExtent;                

            }
            
            set
            {
                _NextExtent = value;
            }

        }

        #endregion

        #endregion


        #region Constructors

        #region ObjectExtent()

        public ObjectExtent()
        {
            _Length             = 0;
            _LogicalPosition    = 0;
            _StorageUUID        = new StorageUUID(0UL);
            _PhysicalPosition   = 0;
            _NextExtent         = new ExtendedPosition(0, 0);
        }

        #endregion

        #region ObjectExtent(myLogicalPosition, myLength, myExtendedPhysicalPosition)

        public ObjectExtent(UInt64 myLogicalPosition, UInt64 myLength, ExtendedPosition myExtendedPhysicalPosition)
        {
            _LogicalPosition    = myLogicalPosition;
            _Length             = myLength;
            _PhysicalPosition   = myExtendedPhysicalPosition.Position;
            _StorageUUID        = myExtendedPhysicalPosition.StorageUUID;
            _NextExtent         = new ExtendedPosition(0, 0);
        }

        #endregion

        #region ObjectExtent(myLogicalPosition, myLength, myPhysicalPosition, myStorageUUID)

        public ObjectExtent(UInt64 myLogicalPosition, UInt64 myLength, UInt64 myPhysicalPosition, StorageUUID myStorageUUID)
        {
            _LogicalPosition    = myLogicalPosition;
            _Length             = myLength;
            _PhysicalPosition   = myPhysicalPosition;
            _StorageUUID        = myStorageUUID;
            _NextExtent         = new ExtendedPosition(0, 0);
        }

        #endregion

        #region ObjectExtent(myLogicalPosition, myLength, myPhysicalPosition, myStorageUUID, myNextExtent)

        public ObjectExtent(UInt64 myLogicalPosition, UInt64 myLength, UInt64 myPhysicalPosition, StorageUUID myStorageUUID, ExtendedPosition myNextExtent)
        {
            _LogicalPosition    = myLogicalPosition;
            _Length             = myLength;
            _PhysicalPosition   = myPhysicalPosition;
            _StorageUUID        = myStorageUUID;
            _NextExtent         = myNextExtent;
        }

        #endregion

        #region ObjectExtent(myObjectExtent)

        /// <summary>
        /// Creates a new ObjectExtent based on the content of myObjectExtent
        /// </summary>
        /// <param name="myObjectExtent">The ObjectExtent to be cloned</param>
        public ObjectExtent(ObjectExtent myObjectExtent)
        {

            _Length             = myObjectExtent.Length;
            _LogicalPosition    = myObjectExtent.LogicalPosition;
            _StorageUUID        = myObjectExtent.StorageUUID;
            _PhysicalPosition   = myObjectExtent.PhysicalPosition;

            if (myObjectExtent.NextExtent != null)
                _NextExtent     = new ExtendedPosition(myObjectExtent.NextExtent.StorageUUID, myObjectExtent.NextExtent.Position);

            else 
                _NextExtent     = new ExtendedPosition(0, 0);

        }

        #endregion

        #endregion


        #region Object-specific methods

        #region AreOverlapping(myObjectExtent)

        public Boolean AreOverlapping(ObjectExtent myObjectExtent)
        {

            //
            // Echt kleiner: |--this--| :                  :
            //               :        : |--myObjectExtent--|
            if (_LogicalPosition < myObjectExtent.LogicalPosition &&
                _LogicalPosition + _Length < myObjectExtent.LogicalPosition)
                return false;

            //
            // Echt größer:  :                  : |--this--|
            //               |--myObjectExtent--| :        :
            if (_LogicalPosition > myObjectExtent.LogicalPosition &&
                _LogicalPosition > myObjectExtent.LogicalPosition + myObjectExtent.Length)
                return false;

            //
            // Gleich:       |-------this-------|
            //               |--myObjectExtent--|
            if (_LogicalPosition == myObjectExtent.LogicalPosition &&
                _Length          == myObjectExtent.Length)
                return false;

            return true;

        }

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (myObjectExtent1, myObjectExtent2)

        public static Boolean operator == (ObjectExtent myObjectExtent1, ObjectExtent myObjectExtent2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myObjectExtent1, myObjectExtent2))
                return true;

            // If one is null, but not both, return false.
            if (( (Object) myObjectExtent1 == null) || ( (Object) myObjectExtent2 == null))
                return false;

            return myObjectExtent1.Equals(myObjectExtent2);

        }

        #endregion

        #region Operator != (myObjectExtent1, myObjectExtent2)

        public static Boolean operator != (ObjectExtent myObjectExtent1, ObjectExtent myObjectExtent2)
        {
            return !(myObjectExtent1 == myObjectExtent2);
        }

        #endregion

        #region Operator < (myObjectExtent1, myObjectExtent2)

        public static Boolean operator <  (ObjectExtent myObjectExtent1, ObjectExtent myObjectExtent2)
        {

            if (myObjectExtent1.LogicalPosition < myObjectExtent2.LogicalPosition)
                return true;

            if (myObjectExtent1.LogicalPosition > myObjectExtent2.LogicalPosition)
                return false;

            //myObjectExtent1.LogicalPosition == myObjectExtent2.LogicalPosition
            if (myObjectExtent1.Length          < myObjectExtent2.Length)
                return true;

            return false;

        }

        #endregion

        #region Operator > (myObjectExtent1, myObjectExtent2)

        public static Boolean operator >  (ObjectExtent myObjectExtent1, ObjectExtent myObjectExtent2)
        {

            if (myObjectExtent1.LogicalPosition > myObjectExtent2.LogicalPosition)
                return true;

            if (myObjectExtent1.LogicalPosition < myObjectExtent2.LogicalPosition)
                return false;

            //myObjectExtent1.LogicalPosition == myObjectExtent2.LogicalPosition
            if (myObjectExtent1.Length          > myObjectExtent2.Length)
                return true;

            return false;

        }

        #endregion

        #region Operator <= (myObjectExtent1, myObjectExtent2)

        public static Boolean operator <= (ObjectExtent myObjectExtent1, ObjectExtent myObjectExtent2)
        {
            return !(myObjectExtent1 > myObjectExtent2);
        }

        #endregion

        #region Operator >= (myObjectExtent1, myObjectExtent2)

        public static Boolean operator >=(ObjectExtent myObjectExtent1, ObjectExtent myObjectExtent2)
        {
            return !(myObjectExtent1 < myObjectExtent2);
        }

        #endregion

        #endregion


        #region IComparable Members

        public Int32 CompareTo(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException();

            // If parameter cannot be cast to ObjectExtent return false.
            var _ObjectExtent = myObject as ObjectExtent;
            if ((Object) _ObjectExtent == null)
                throw new ArgumentException("myObject is not of type ObjectExtent!");

            if (this < _ObjectExtent) return -1;
            if (this > _ObjectExtent) return +1;

            return 0;

        }

        #endregion

        #region IComparable<ObjectExtent> Members

        public Int32 CompareTo(ObjectExtent myObjectExtent)
        {

            // Check if myObjectExtent is null
            if (myObjectExtent == null)
                throw new ArgumentNullException();

            if (this < myObjectExtent) return -1;
            if (this > myObjectExtent) return +1;

            return 0;

        }

        #endregion

        #region IEquatable Members

        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                return false;

            // If parameter cannot be cast to ObjectExtent return false.
            var _ObjectExtent = myObject as ObjectExtent;
            if ((Object) _ObjectExtent == null)
                return false;

            return Equals(_ObjectExtent);

        }

        #endregion

        #region IEquatable<ObjectExtent> Members

        public Boolean Equals(ObjectExtent myObjectExtent)
        {

            // If parameter is null return false:
            if ((Object) myObjectExtent == null)
                return false;

            // Check if the inner fields have the same values
            if (LogicalPosition != myObjectExtent.LogicalPosition)
                return false;

            if (Length != myObjectExtent.Length)
                return false;

            return true;

        }

        #endregion


        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return _Length.GetHashCode() ^ _LogicalPosition.GetHashCode();
        }

        #endregion

        #region ToString()

        public override String ToString()
        {
            return _LogicalPosition + ":" + _Length + " -> " + _PhysicalPosition + ":" + _Length + " on " + _StorageUUID.ToString() + " (NextExtent: " + _NextExtent.ToString() + ")";
        }

        #endregion


    }

}