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

#endregion

namespace sones.GraphDB.Structures.EdgeTypes
{

    /// <summary>
    /// The actual connection to another DBObject
    /// </summary>
    public class Reference : IComparable, IComparable<Reference>, IFastSerialize, IFastSerializationTypeSurrogate
    {

        #region Properties

        public  ObjectUUID      ObjectUUID { get; private set; }
        private TypeUUID        _typeOfDBObjects = null;
        private WeakReference   _dbObject = null;

        private int _hashCode = 0;

        #endregion

        #region Constructor

        public Reference()
        {
            //_dbObject = new WeakReference(null);
            //ObjectUUID = null;
        }

        public Reference(ObjectUUID myObjectUUID, TypeUUID typeOfDBObjects, Exceptional<DBObjectStream> myDBObject = null)
        {
            //Debug.Assert(typeOfDBObjects != null);
            ObjectUUID          = myObjectUUID;
            _typeOfDBObjects    = typeOfDBObjects;
            _dbObject           = new WeakReference(myDBObject);

            _hashCode           = myObjectUUID.GetHashCode();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Gets the actual DBObjectstream that this reference is pointing to
        /// </summary>
        /// <param name="myDBObjectCache">The DBObjectcache to load the DBObjectStream</param>
        /// <returns>The actual DBObjectStream corresponding to this reference.</returns>
        public Exceptional<DBObjectStream> GetDBObjectStream(DBObjectCache myDBObjectCache)
        {
            var aDBO = _dbObject.Target as Exceptional<DBObjectStream>;
            if (aDBO == null)
            {

                Debug.Assert(_typeOfDBObjects != null);
                // Object was reclaimed, so get it again
                aDBO = myDBObjectCache.LoadDBObjectStream(_typeOfDBObjects, ObjectUUID);

                _dbObject.Target = aDBO;
            }

            return aDBO;
        }

        #endregion

        #region override

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

        #endregion

        #region ToString

        public override string ToString()
        {
            return String.Format("ObjectUUID: {0}, TypeUUID: {1}", ObjectUUID.ToString(), _typeOfDBObjects.ToString());
        }

        #endregion

        #endregion

        #region interfaces

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
            _typeOfDBObjects.Serialize(ref mySerializationWriter);

            ObjectUUID.Serialize(ref mySerializationWriter);
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            _typeOfDBObjects = new TypeUUID(ref mySerializationReader);

            ObjectUUID = new ObjectUUID();
            ObjectUUID.Deserialize(ref mySerializationReader);

            _hashCode = ObjectUUID.GetHashCode();

            _dbObject = new WeakReference(null);
        }

        #endregion

        #region IFastSerializationTypeSurrogate

        public bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public void Serialize(SerializationWriter writer, object value)
        {
            writer.WriteObject(((Reference)value)._typeOfDBObjects);
            writer.WriteObject(((Reference)value).ObjectUUID);
        }

        public object Deserialize(SerializationReader reader, Type myReference)
        {
            Reference thisObject = (Reference)Activator.CreateInstance(myReference);
            return Deserialize(ref reader, thisObject);
        }

        private object Deserialize(ref SerializationReader reader, Reference myReference)
        {
            myReference._typeOfDBObjects = (TypeUUID)reader.ReadObject();
            myReference.ObjectUUID = (ObjectUUID)reader.ReadObject();

            myReference._hashCode = myReference.ObjectUUID.GetHashCode();

            myReference._dbObject = new WeakReference(null);

            return myReference;
        }

        public uint TypeCode
        {
            get { return 701; }
        }

        #endregion

        #endregion
    
    }

}
