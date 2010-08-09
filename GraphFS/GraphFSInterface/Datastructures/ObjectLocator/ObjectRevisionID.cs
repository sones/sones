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
 * RevisionID
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Runtime.Serialization;

using sones.GraphFS.Exceptions;

using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.UUID;
using sones.Lib.DataStructures.Timestamp;

#endregion

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    /// A ObjectRevisionID is an identificator for an object revision.
    /// </summary>    
    
    [AllowNonEmptyConstructor]

    public class ObjectRevisionID : IFastSerialize, IComparable, IComparable<ObjectRevisionID>, IEquatable<ObjectRevisionID>
    {
        
        #region Properties

        #region Timestamp

        /// <summary>
        /// The timestamp of this revision.
        /// </summary>
        public UInt64   Timestamp   { get; private set; }

        #endregion

        #region UUID

        /// <summary>
        /// A unique identification of the generation process of this revision.
        /// </summary>
        public UUID     UUID        { get; private set; }

        #endregion

        #endregion

        #region Constructors

        #region RevisionID(myUUID)

        /// <summary>
        /// A constructor used for generating an RevisionID based on the actual
        /// DateTime and the given (system) UUID.
        /// </summary>
        /// <param name="myUUID">An unique identification for this generation process</param>
        public ObjectRevisionID(UUID myUUID)
        {
            Timestamp   = (UInt64) TimestampNonce.Ticks;
            UUID        = myUUID;
        }

        #endregion

        #region RevisionID(myTimestamp, myUUID)

        /// <summary>
        /// A constructor used for generating an RevisionID based on the given
        /// timestamp and the given (system) UUID.
        /// </summary>
        /// <param name="myTimestamp">Any timestamp</param>
        /// <param name="myUUID">An unique identification for this generation process</param>
        public ObjectRevisionID(UInt64 myTimestamp, UUID myUUID)
        {
            Timestamp   = myTimestamp;
            UUID        = myUUID;
        }

        #endregion

        #region RevisionID(myDateTime, myUUID)

        /// <summary>
        /// A constructor used for generating an RevisionID based on the given
        /// DateTime and the given (system) UUID.
        /// </summary>
        /// <param name="myDateTime">Any DateTime</param>
        /// <param name="myUUID">An unique identification for this generation process</param>
        public ObjectRevisionID(DateTime myDateTime, UUID myUUID)
        {
            Timestamp   = (UInt64) myDateTime.Ticks;
            UUID        = myUUID;
        }

        #endregion

        #region RevisionID(myDateTime_String, myUUID)

        /// <summary>
        /// A constructor used for generating an RevisionID based on the given
        /// "yyyyddMM.HHmmss.fffffff"-formated DateTime and the given (system) UUID.
        /// </summary>
        /// <param name="myDateTime">A "yyyyddMM.HHmmss.fffffff"-formated DateTime string</param>
        /// <param name="myUUID">An unique identification for this generation process</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.FormatException"></exception>
        public ObjectRevisionID(String myDateTime_String, UUID myUUID)
        {
            Timestamp   = (UInt64) (DateTime.ParseExact(myDateTime_String, "yyyyddMM.HHmmss.fffffff", null)).Ticks;
            UUID        = myUUID;
        }

        #endregion

        #region RevisionID(myRevisionString)

        /// <summary>
        /// A constructor used for generating an RevisionID based on the given
        /// "yyyyddMM.HHmmss.fffffff(UUID)"-formated DateTime with attached UUID.
        /// </summary>
        /// <param name="myObjectRevisionIDString">A "yyyyddMM.HHmmss.fffffff(UUID)"-formated DateTime string with attached UUID</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.FormatException"></exception>
        public ObjectRevisionID(String myObjectRevisionIDString)
        {
            var RevisionID_Timestamp    = myObjectRevisionIDString.Remove(myObjectRevisionIDString.IndexOf("("));
            var RevisionID_UUID         = myObjectRevisionIDString.Substring(RevisionID_Timestamp.Length + 1, myObjectRevisionIDString.Length - RevisionID_Timestamp.Length - 2);
            Timestamp                   = (UInt64) (DateTime.ParseExact(RevisionID_Timestamp, "yyyyddMM.HHmmss.fffffff", null)).Ticks;
            UUID                        = new UUID(RevisionID_UUID);
        }

        #endregion

        #endregion


        #region Object-specific methods

        #region IncrementRevisionTimestamp()

        /// <summary>
        /// Increments the value of the RevisionTimestamp by one.
        /// This can be used to make the RevisionID unique in case of
        /// an imprecise resolution of your GetDateTime method.
        /// </summary>
        public void IncrementRevisionTimestamp()
        {
            Timestamp++;
        }

        #endregion

        #region Clone()

        public ObjectRevisionID Clone()
        {
            return new ObjectRevisionID(Timestamp, UUID.Clone());
        }

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator == (ObjectRevisionID myObjectRevisionID1, ObjectRevisionID myObjectRevisionID2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myObjectRevisionID1, myObjectRevisionID2))
                return true;

            // If one is null, but not both, return false.
            if (( (Object) myObjectRevisionID1 == null) || ( (Object) myObjectRevisionID2 == null))
                return false;

            return myObjectRevisionID1.Equals(myObjectRevisionID2);

        }

        #endregion

        #region Operator != (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator != (ObjectRevisionID myObjectRevisionID1, ObjectRevisionID myObjectRevisionID2)
        {
            return !(myObjectRevisionID1 == myObjectRevisionID2);
        }

        #endregion

        #region Operator < (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator < (ObjectRevisionID myObjectRevisionID1, ObjectRevisionID myObjectRevisionID2)
        {

            if (myObjectRevisionID1.Timestamp < myObjectRevisionID2.Timestamp)
                return true;

            if (myObjectRevisionID1.Timestamp > myObjectRevisionID2.Timestamp)
                return false;

            // myObjectRevisionID1.Timestamp == myObjectRevisionID2.Timestamp
            if (myObjectRevisionID1.UUID < myObjectRevisionID2.UUID)
                return true;

            return false;

        }

        #endregion

        #region Operator > (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator > (ObjectRevisionID myObjectRevisionID1, ObjectRevisionID myObjectRevisionID2)
        {

            if (myObjectRevisionID1.Timestamp > myObjectRevisionID2.Timestamp)
                return true;

            if (myObjectRevisionID1.Timestamp < myObjectRevisionID2.Timestamp)
                return false;

            // myObjectRevisionID1.Timestamp == myObjectRevisionID2.Timestamp
            if (myObjectRevisionID1.UUID > myObjectRevisionID2.UUID)
                return true;

            return false;

        }

        #endregion

        #region Operator <= (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator <= (ObjectRevisionID myObjectRevisionID1, ObjectRevisionID myObjectRevisionID2)
        {
            return !(myObjectRevisionID1 > myObjectRevisionID2);
        }

        #endregion

        #region Operator >= (myObjectRevisionID1, myObjectRevisionID2)

        public static Boolean operator >= (ObjectRevisionID myObjectRevisionID1, ObjectRevisionID myObjectRevisionID2)
        {
            return !(myObjectRevisionID1 < myObjectRevisionID2);
        }

        #endregion

        #endregion


        #region IComparable Member

        public Int32 CompareTo(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException();

            // If parameter cannot be cast to Point return false.
            var _RevisionID = myObject as ObjectRevisionID;
            if ( (Object) _RevisionID == null)
                throw new ArgumentException("myObject is not of type RevisionID!");

            if (this < _RevisionID) return -1;
            if (this > _RevisionID) return +1;
            
            return 0;

        }

        #endregion

        #region IComparable<ObjectRevisionID> Member

        public Int32 CompareTo(ObjectRevisionID myObjectRevisionID)
        {

            // Check if myObjectRevisionID is null
            if (myObjectRevisionID == null)
                throw new ArgumentNullException();

            if (this < myObjectRevisionID) return -1;
            if (this > myObjectRevisionID) return +1;

            return  0;

        }

        #endregion

        #region IEquatable Members

        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                return false;

            // If parameter cannot be cast to RevisionID return false.
            var _RevisionID = myObject as ObjectRevisionID;
            if ( (Object) _RevisionID == null)
                return false;

            return Equals(_RevisionID);

        }

        #endregion

        #region IEquatable<ObjectRevisionID> Members

        public Boolean Equals(ObjectRevisionID myObjectRevisionID)
        {

            // If parameter is null return false:
            if ( (Object) myObjectRevisionID == null)
                return false;

            // Check if the inner fields have the same values
            if (Timestamp   != myObjectRevisionID.Timestamp)
                return false;

            if (UUID        != myObjectRevisionID.UUID)
                return false;

            return true;

        }

        #endregion


        #region IFastSerialize Members

        #region isDirty

        private Boolean _isDirty = false;

        public Boolean isDirty
        {

            get
            {
                return _isDirty;
            }

            set
            {
                _isDirty = value;
            }

        }

        #endregion

        #region ModificationTime

        public DateTime ModificationTime
        {

            get
            {
                throw new NotImplementedException();
            }

        }

        #endregion

        #region Serialize(ref mySerializationWriter)

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {

            try
            {

                mySerializationWriter.WriteUInt64(Timestamp);
                UUID.Serialize(ref mySerializationWriter);

            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }

        }

        #endregion

        #region Deserialize(ref mySerializationReader)

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            try
            {
                Timestamp   = mySerializationReader.ReadUInt64();
                UUID        = new UUID(mySerializationReader.ReadByteArray());
            }

            catch (Exception e)
            {
                throw new PandoraFSException_RevisionIDCouldNotBeDeserialized("RevisionID could not be deserialized!\n\n" + e);
            }
        }

        #endregion

        #endregion


        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return Timestamp.GetHashCode() ^ UUID.GetHashCode();
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Returns a formated string representation of this revision
        /// </summary>
        /// <returns>A formated string representation of this revision</returns>
        public override String ToString()
        {
            return String.Format("{0:yyyyddMM.HHmmss.fffffff}({1})", new DateTime((Int64)Timestamp), UUID.ToString());
        }

        #endregion
    
    }

}
