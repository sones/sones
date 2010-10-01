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

/* <id name="Reference" />
 * <copyright file="Reference.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>The actual connection to another DBObject</summary>
 */

#region Usings

using System;
using System.Diagnostics;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;
using sones.Lib.Serializer;
using sones.GraphDB.TypeManagement;
using sones.Lib;

#endregion

namespace sones.GraphDB.Structures.EdgeTypes
{

    /// <summary>
    /// The actual connection to another DBObjectStream
    /// </summary>
    public class Reference : IComparable, IComparable<Reference>, IFastSerialize, IFastSerializationTypeSurrogate, IEstimable
    {

        #region Data

        private TypeUUID       _TypeUUID        = null;
        private WeakReference  _WeakReference   = null;
        private Int32          _hashCode        = 0;

        private UInt64          _estimatedSize  = 0;


        #endregion

        #region Properties

        public  ObjectUUID      ObjectUUID { get; private set; }

        #endregion

        #region Constructor(s)

        #region Reference()

        public Reference()
        {
        }

        #endregion

        #region Reference(myObjectUUID, myTypeUUID, myDBObjectStream = null)

        public Reference(ObjectUUID myObjectUUID, TypeUUID myTypeUUID, Exceptional<DBObjectStream> myDBObjectStream = null)
        {
            ObjectUUID          = myObjectUUID;
            _TypeUUID           = myTypeUUID;
            _WeakReference      = new WeakReference(myDBObjectStream);
            _hashCode           = myObjectUUID.GetHashCode();
        }

        #endregion

        #endregion


        #region GetDBObjectStream(myDBObjectCache)

        /// <summary>
        /// Gets the actual DBObjectstream that this reference is pointing to
        /// </summary>
        /// <param name="myDBObjectCache">The DBObjectcache to load the DBObjectStream</param>
        /// <returns>The actual DBObjectStream corresponding to this reference.</returns>
        public Exceptional<DBObjectStream> GetDBObjectStream(DBObjectCache myDBObjectCache)
        {

            var _DBObjectStream = _WeakReference.Target as Exceptional<DBObjectStream>;
            if (_DBObjectStream == null)
            {

                Debug.Assert(_TypeUUID != null);
                // Object was reclaimed, so get it again
                _DBObjectStream = myDBObjectCache.LoadDBObjectStream(_TypeUUID, ObjectUUID);

                _WeakReference.Target = _DBObjectStream;

            }

            return _DBObjectStream;

        }

        #endregion


        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is Reference)
            {
                Reference p = (Reference)obj;
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(Reference p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return this.ObjectUUID == p.ObjectUUID;
        }

        public static Boolean operator ==(Reference a, Reference b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(Reference a, Reference b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        #region ToString

        public override string ToString()
        {
            return String.Format("ObjectUUID: {0}, TypeUUID: {1}", ObjectUUID.ToString(), _TypeUUID.ToString());
        }

        #endregion

        #endregion

        #region ICompareable

        public int CompareTo(object obj)
        {
            // Check if myObject is null
            if (obj == null)
                throw new ArgumentNullException("obj must not be null!");

            // Check if myObject can be casted to a Reference object
            var myReference = obj as Reference;
            if (myReference == null)
                throw new ArgumentException("myObject is not of type UUID!");

            return CompareTo(myReference);
        }

        public int CompareTo(Reference other)
        {
            // Check if other is null
            if (other == null)
                throw new ArgumentNullException("other must not be null!");

            return this.ObjectUUID.CompareTo(other.ObjectUUID);
        }

        #endregion

        #region IFastSerialize

        public bool isDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            _TypeUUID.Serialize(ref mySerializationWriter);

            ObjectUUID.Serialize(ref mySerializationWriter);
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            _TypeUUID = new TypeUUID(ref mySerializationReader);

            ObjectUUID = new ObjectUUID();
            ObjectUUID.Deserialize(ref mySerializationReader);

            _hashCode = ObjectUUID.GetHashCode();

            _WeakReference = new WeakReference(null);
        }

        #endregion

        #region IFastSerializationTypeSurrogate

        public bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public void Serialize(SerializationWriter writer, object value)
        {
            writer.WriteObject(((Reference)value)._TypeUUID);
            writer.WriteObject(((Reference)value).ObjectUUID);
        }

        public object Deserialize(SerializationReader reader, Type myReference)
        {
            Reference thisObject = (Reference)Activator.CreateInstance(myReference);
            return Deserialize(ref reader, thisObject);
        }

        private object Deserialize(ref SerializationReader reader, Reference myReference)
        {
            myReference._TypeUUID = (TypeUUID)reader.ReadObject();
            myReference.ObjectUUID = (ObjectUUID)reader.ReadObject();

            myReference._hashCode = myReference.ObjectUUID.GetHashCode();

            myReference._WeakReference = new WeakReference(null);

            return myReference;
        }

        public uint TypeCode
        {
            get { return 701; }
        }

        #endregion

        #region estimated size

        public ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        private void CalcEstimatedSize(Reference myTypeAttribute)
        {
            //ObjectUUID + BaseSize
            _estimatedSize = EstimatedSizeConstants.CalcUUIDSize(_TypeUUID) + EstimatedSizeConstants.CalcUUIDSize(ObjectUUID) + EstimatedSizeConstants.UInt64 + EstimatedSizeConstants.Int32 + EstimatedSizeConstants.ClassDefaultSize + EstimatedSizeConstants.WeakReference;
        }

        #endregion


    }

}
