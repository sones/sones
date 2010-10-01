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

/* GraphFS - Right
 * (c) Henning Rauch, 2009
 *     Achim Friedland, 2009
 *  
 * The Right class represents an access right within the GraphFS.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Serializer;
using System.Runtime.Serialization;

using sones.Lib.NewFastSerializer;
using sones.GraphFS.Exceptions;
using sones.Lib;

#endregion

namespace sones.GraphFS.InternalObjects
{

    /// <summary>
    /// The Right class represents an access right within the GraphFS.
    /// </summary>
    
    [AllowNonEmptyConstructor]
    public class Right : IFastSerialize, IComparable, IComparable<Right>, IFastSerializationTypeSurrogate, IEstimable
    {


        #region Properties

        #region TypeCode
        public UInt32 TypeCode { get { return 202; } }
        #endregion

        #region RightsName

        private String _RightsName;

        public String RightsName
        {
            
            get
            {
                return _RightsName;
            }

        }

        #endregion 

        #region UUID

        private RightUUID _UUID = new RightUUID(0);

        public RightUUID UUID
        {
            
            get
            {
                return _UUID;
            }

        }

        #endregion

        #region IsUserDefined

        private Boolean _IsUserDefined = false;

        public Boolean IsUserDefined
        {
            
            get
            {
                return _IsUserDefined;
            }

        }

        #endregion

        #region ValidationScript

        private String _ValidationScript = "";

        public String ValidationScript
        {
            
            get
            {
                return _ValidationScript;
            }
        
        }

        #endregion

        #endregion


        #region Constructor

        public Right()
        {
        }

        #region Right(myRightsName, myRightUUID, myIsUserdefinedRight, myValidationScript)

        /// <summary>
        /// This is the constructor of the Right class.
        /// </summary>
        /// <param name="myLogin">The myLogin of the right.</param>
        /// <param name="myRightUUID">The Guid of the right.</param>
        /// <param name="myIsUserdefinedRight">Is this a user defined right.</param>
        /// <param name="myValidationScript">A user defined </param>
        public Right(String myRightsName, RightUUID myRightUUID, Boolean myIsUserdefinedRight, String myValidationScript)
        {

            if (myRightsName == null || myRightsName.Length == 0)
                throw new ArgumentNullException("myRightsName must not be null or its length be zero!");

            if (myRightUUID == null || myRightUUID.Length < 16)
                throw new ArgumentNullException("myRightUUID must not be null or less than 16 bytes!");

            if (myValidationScript == null)
                throw new ArgumentNullException("myValidationScript must not be null!");

            _RightsName         = myRightsName;
            _UUID               = myRightUUID;
            _IsUserDefined      = myIsUserdefinedRight;
            _ValidationScript   = myValidationScript;

        }

        #endregion

        //#region Right(mySerializedData)

        //public Right(Byte[] mySerializedData)
        //{

        //    if (mySerializedData == null || mySerializedData.Length == 0)
        //        throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

        //    Deserialize(mySerializedData);

        //}

        //#endregion

        #endregion


        #region Object-specific methods

        #region Operator overloading

        #region Equals(myObject)

        /// <summary>
        /// Compares this entity with the given object.
        /// </summary>
        /// <param name="myObject">An object of type Right</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an Right object
            Right myRight = myObject as Right;
            if ((Object) myRight == null)
                throw new ArgumentException("myObject is not of type Right!");

            return Equals(myRight);

        }

        #endregion

        #region Equals(myEntity)

        /// <summary>
        /// Compares this right with the given right.
        /// </summary>
        /// <param name="myRight">an Right object</param>
        /// <returns>true|false</returns>
        public Boolean Equals(Right myRight)
        {

            // Check if myRight is null
            if ((Object) myRight == null)
                throw new ArgumentNullException("myRight must not be null!");

            // Check if the inner fields have the same values

            if (_RightsName         != myRight.RightsName)
                return false;

            if (_UUID               != myRight.UUID)
                return false;

            if (_IsUserDefined      != myRight.IsUserDefined)
                return false;

            if (_ValidationScript   != myRight.ValidationScript)
                return false;

            return true;

        }

        #endregion

        #region Operator == (myRight1, myRight2)

        public static Boolean operator == (Right myRight1, Right myRight2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myRight1, myRight2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myRight1 == null) || ((Object) myRight2 == null))
                return false;

            return myRight1.Equals(myRight2);

        }

        #endregion

        #region Operator != (myRight1, myRight2)

        public static Boolean operator != (Right myRight1, Right myRight2)
        {
            return !(myRight1 == myRight2);
        }

        #endregion

        #region Operator  < (myRight1, myRight2)

        public static Boolean operator < (Right myRight1, Right myRight2)
        {

            // Check if myRight1 is null
            if (myRight1 == null)
                throw new ArgumentNullException("myRight1 must not be null!");

            // Check if myRight2 is null
            if (myRight2 == null)
                throw new ArgumentNullException("myRight2 must not be null!");


            Int32 _RightsNameComparison = myRight1.RightsName.CompareTo(myRight2.RightsName);

            if (_RightsNameComparison < 0)
                return true;

            if (_RightsNameComparison > 0)
                return false;


            if (myRight1.UUID < myRight2.UUID)
                return true;

            if (myRight1.UUID > myRight2.UUID)
                return false;


            return false;

        }

        #endregion

        #region Operator  > (myRight1, myRight2)

        public static Boolean operator > (Right myRight1, Right myRight2)
        {

            // Check if myRight1 is null
            if (myRight1 == null)
                throw new ArgumentNullException("myRight1 must not be null!");

            // Check if myRight2 is null
            if (myRight2 == null)
                throw new ArgumentNullException("myRight2 must not be null!");


            Int32 _RightsNameComparison = myRight1.RightsName.CompareTo(myRight2.RightsName);

            if (_RightsNameComparison > 0)
                return true;

            if (_RightsNameComparison < 0)
                return false;


            if (myRight1.UUID > myRight2.UUID)
                return true;

            if (myRight1.UUID < myRight2.UUID)
                return false;


            return false;

        }

        #endregion

        #region Operator <= (myRight1, myRight2)

        public static Boolean operator <= (Right myRight1, Right myRight2)
        {
            return !(myRight1 > myRight2);
        }

        #endregion

        #region Operator >= (myRight1, myRight2)

        public static Boolean operator >= (Right myRight1, Right myRight2)
        {
            return !(myRight1 < myRight2);
        }

        #endregion

        #region GetHashCode()

        public override int GetHashCode()
        {
            return _RightsName.GetHashCode() ^ _UUID.GetHashCode() ^ _IsUserDefined.GetHashCode() ^ _ValidationScript.GetHashCode();
        }

        #endregion

        #endregion

        #region IComparable Member

        public Int32 CompareTo(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an Right object
            Right myRight = myObject as Right;
            if ( (Object) myRight == null)
                throw new ArgumentException("myObject is not of type Right!");

            return CompareTo(myRight);

        }

        #endregion

        #region IComparable<Right> Member

        public Int32 CompareTo(Right myRight)
        {

            // Check if myRight is null
            if (myRight == null)
                throw new ArgumentNullException("myRight must not be null!");

            if (this < myRight) return -1;
            if (this > myRight) return +1;

            return 0;

        }

        #endregion

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

        //#region Serialize()

        //public Byte[] Serialize()
        //{

        //    #region Data

        //    SerializationWriter writer;

        //    #endregion

        //    try
        //    {

        //        #region Init SerializationWriter

        //        writer = new SerializationWriter();

        //        #endregion

        //        writer.WriteObject(_RightsName);
        //        writer.WriteObject(_UUID.ToByteArray());
        //        writer.WriteObject(_IsUserDefined);
        //        writer.WriteObject(_ValidationScript);

        //        isDirty = false;

        //        return writer.ToArray();

        //    }

        //    catch (SerializationException e)
        //    {
        //        throw new SerializationException(e.Message);
        //    }

        //}

        //#endregion

        //#region Deserialize(mySerializedData)

        //public void Deserialize(Byte[] mySerializedData)
        //{

        //    #region Data

        //    SerializationReader reader;

        //    #endregion

        //    #region Init reader

        //    reader = new SerializationReader(mySerializedData);

        //    #endregion

        //    try
        //    {

        //        _RightsName         = (String) reader.ReadObject();
        //        _UUID               = new RightUUID((Byte[]) reader.ReadObject());
        //        _IsUserDefined      = (Boolean) reader.ReadObject();
        //        _ValidationScript   = (String) reader.ReadObject();

        //    }

        //    catch (Exception e)
        //    {
        //        throw new GraphFSException_EntityCouldNotBeDeserialized("Right could not be deserialized!\n\n" + e);
        //    }

        //}

        //#endregion

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            try
            {

                mySerializationWriter.WriteString(_RightsName);
                UUID.Serialize(ref mySerializationWriter);
                mySerializationWriter.WriteBoolean(_IsUserDefined);
                mySerializationWriter.WriteString(_ValidationScript);

                isDirty = false;

            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            try
            {

                _RightsName         = mySerializationReader.ReadString();
                _UUID               = new RightUUID(mySerializationReader.ReadByteArray());
                _IsUserDefined      = mySerializationReader.ReadBoolean();
                _ValidationScript   = mySerializationReader.ReadString();

            }

            catch (Exception e)
            {
                throw new GraphFSException_EntityCouldNotBeDeserialized("Right could not be deserialized!\n\n" + e);
            }
        }

        #endregion


        #region IFastSerializationTypeSurrogate Members

        public bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public void Serialize(SerializationWriter mySerializationWriter, object value)
        {
            Right thisObject = (Right)value;

            try
            {
                mySerializationWriter.WriteString(thisObject._RightsName);
                thisObject._UUID.Serialize(ref mySerializationWriter);
                mySerializationWriter.WriteBoolean(thisObject._IsUserDefined);
                mySerializationWriter.WriteString(thisObject._ValidationScript);

                thisObject.isDirty = false;

            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }
        }

        public object Deserialize(SerializationReader mySerializationReader, Type type)
        {
            Right thisObject = (Right)Activator.CreateInstance(type);

            try
            {
                thisObject._RightsName          = mySerializationReader.ReadString();
                thisObject._UUID                = new RightUUID();
                thisObject._UUID.Deserialize(ref mySerializationReader);
                thisObject._IsUserDefined       = mySerializationReader.ReadBoolean();
                thisObject._ValidationScript    = mySerializationReader.ReadString();
            }

            catch (Exception e)
            {
                throw new GraphFSException_EntityCouldNotBeDeserialized("Right could not be deserialized!\n\n" + e);
            }

            return thisObject;
        }

        #endregion

        #region Iestimable

        public ulong GetEstimatedSize()
        {
            return EstimatedSizeConstants.UndefinedObjectSize;
        }

        #endregion
    }

}
