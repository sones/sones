/* <id name="GraphdbDB – IndexKey" />
 * <copyright file="IndexKey.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;

using sones.Lib.NewFastSerializer;
using sones.Lib.Serializer;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib;

#endregion

namespace sones.GraphDB.Indices
{
    /// <summary>
    /// IndexKey for any AttributeIndex
    /// </summary>
    public class IndexKey : IFastSerialize, IComparable, IFastSerializationTypeSurrogate, IEstimable
    {

        #region Data

        private int     _hashCode = 0;
        private UInt64  _estimatedSize = 0;

        private List<ADBBaseObject> _indexKeyValues = new List<ADBBaseObject>();
        public List<ADBBaseObject> IndexKeyValues { get { return _indexKeyValues; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Needed for IFastSerialize
        /// </summary>
        public IndexKey()
        {

        }

        /// <summary>
        /// Adds a single ADBBaseObject to the IndexKex
        /// </summary>
        /// <param name="myAttributeUUID">AttributeUUID corresponding to the ADBBaseObject</param>
        /// <param name="myIndexKeyPayload">The ADBBaseObject that is going to be added</param>
        /// <param name="myIndexDefinition">The corresponding IndexKeyDefinition</param>
        public IndexKey(AttributeUUID myAttributeUUID, ADBBaseObject myIndexKeyPayload, IndexKeyDefinition myIndexDefinition)
        {
            if (!myIndexDefinition.IndexKeyAttributeUUIDs.Contains(myAttributeUUID))
            {
                throw new GraphDBException(new Error_IndexKeyCreationError(myAttributeUUID, myIndexKeyPayload, myIndexDefinition));
            }
            else
            {
                _estimatedSize = GetBaseSize();

                AddAAKey(myAttributeUUID, myIndexKeyPayload);
            }
        }

        /// <summary>
        /// Recommended way of creating a new IndexKey object.
        /// </summary>
        /// <param name="myIndexKeyValues">All ADBBaseObjects in relation with their AttributeUUID</param>
        /// <param name="myIndexDefinition">The corresponding IndexKeyDefinition</param>
        public IndexKey(Dictionary<AttributeUUID, ADBBaseObject> myIndexKeyValues, IndexKeyDefinition myIndexDefinition)
        {
            _estimatedSize = GetBaseSize();

            foreach (var aDefElement in myIndexDefinition.IndexKeyAttributeUUIDs)
            {
                if (!myIndexKeyValues.ContainsKey(aDefElement))
                {
                    throw new GraphDBException(new Error_IndexKeyCreationError(aDefElement, null, myIndexDefinition));
                }
                else
                {
                    AddAAKey(aDefElement, myIndexKeyValues[aDefElement]);
                }
            }
        }

        /// <summary>
        /// Instantiates a new IndexKey on the base of another one. Additionally a AADBBaseObject is added.
        /// </summary>
        /// <param name="myStartingIndexKey">The base of the new IndexKey</param>
        /// <param name="myAttributeUUID">AttributeUUID corresponding to the ADBBaseObject</param>
        /// <param name="myIndexKeyPayload">The ADBBaseObject that is going to be added</param>
        /// <param name="myIndexDefinition">The corresponding IndexKeyDefinition</param>
        public IndexKey(IndexKey myStartingIndexKey, AttributeUUID myAttributeUUID, ADBBaseObject myIndexKeyPayload, IndexKeyDefinition myIndexDefinition)
        {
            _hashCode = myStartingIndexKey.GetHashCode();

            #region IEstimable

            _estimatedSize = GetBaseSize();

            foreach (var aADBBaseObject in myStartingIndexKey.IndexKeyValues)
            {
                _indexKeyValues.Add(aADBBaseObject);
                _estimatedSize += aADBBaseObject.GetEstimatedSize();
            }

            #endregion

            if (!myIndexDefinition.IndexKeyAttributeUUIDs.Contains(myAttributeUUID))
            {
                throw new GraphDBException(new Error_IndexKeyCreationError(myAttributeUUID, myIndexKeyPayload, myIndexDefinition));
            }
            else
            {
                AddAAKey(myAttributeUUID, myIndexKeyPayload);
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Adds a ADBBaseObject to the IndexKey
        /// </summary>
        /// <param name="myAttributeUUID">AttributeUUID corresponding to the ADBBaseObject</param>
        /// <param name="myADBBaseObject">The ADBBaseObject that is going to be added</param>
        public void AddAADBBAseObject(AttributeUUID myAttributeUUID, ADBBaseObject myADBBaseObject)
        {
            AddAAKey(myAttributeUUID, myADBBaseObject);
        }

        #endregion

        #region private helper

        /// <summary>
        /// Adds a ADBBaseObject to the IndexKey
        /// </summary>
        /// <param name="myAttributeUUID">AttributeUUID corresponding to the ADBBaseObject</param>
        /// <param name="myADBBaseObject">The ADBBaseObject that is going to be added</param>
        private void AddAAKey(AttributeUUID myAttributeUUID, ADBBaseObject myADBBaseObject)
        {
            _indexKeyValues.Add(myADBBaseObject);

            _estimatedSize += myADBBaseObject.GetEstimatedSize();

            CalcNewHashCode(myADBBaseObject, ref _hashCode);
        }

        private void Serialize(ref SerializationWriter mySerializationWriter, IndexKey myValue)
        {
            mySerializationWriter.WriteUInt32((UInt32)myValue.IndexKeyValues.Count);
            foreach (var attr in myValue.IndexKeyValues)
            {
                //attr.Serialize(ref mySerializationWriter);
                mySerializationWriter.WriteObject(attr);                
            }
        }

        private object Deserialize(ref SerializationReader mySerializationReader, IndexKey myValue)
        {
            #region IEstimable

            myValue._estimatedSize = GetBaseSize();

            #endregion


            UInt32 count = mySerializationReader.ReadUInt32();
            for (UInt32 i = 0; i < count; i++)
            {
                var a = (ADBBaseObject)mySerializationReader.ReadObject();

                myValue._indexKeyValues.Add(a);

                _estimatedSize += a.GetEstimatedSize();

                CalcNewHashCode(a, ref myValue._hashCode);
            }

            return myValue;
        }

        private void CalcNewHashCode(ADBBaseObject myADBBaseObject, ref int currentHashCode)
        {
            currentHashCode = currentHashCode ^ (int)(myADBBaseObject.GetHashCode());
        }

        #endregion

        #region Overrides

        #region Equals Overrides

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override Boolean Equals(Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is IndexKey)
            {
                IndexKey p = (IndexKey)obj;
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(IndexKey p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            if (this._indexKeyValues.Count != p.IndexKeyValues.Count)
            {
                return false;
            }

            for (int i = 0; i < _indexKeyValues.Count; i++)
            {
                if (!p.IndexKeyValues[i].Equals(this._indexKeyValues[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static Boolean operator ==(IndexKey a, IndexKey b)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(a, b))
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

        public static Boolean operator !=(IndexKey a, IndexKey b)
        {
            return !(a == b);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int counter = 1;
            sb.AppendFormat("#{0}: ", _indexKeyValues.Count);

            foreach (var aADBBaseObject in _indexKeyValues)
            {
                sb.AppendFormat("{0}, ", aADBBaseObject.Value.ToString());
                counter++;
            }
            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        #endregion

        #endregion

        #region IFastSerialize Members

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
            mySerializationWriter.WriteUInt32((UInt32)_indexKeyValues.Count);

            foreach (var attr in _indexKeyValues)
            {
                attr.Serialize(ref mySerializationWriter);
            }
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            UInt32 count = mySerializationReader.ReadUInt32();

            _indexKeyValues = new List<ADBBaseObject>();
            for (UInt32 i = 0; i < count; i++)
            {
                var a = (ADBBaseObject)mySerializationReader.ReadObject();

                _indexKeyValues.Add(a);
                CalcNewHashCode(a, ref _hashCode);
            }
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            var otherIndexKeyReloaded = obj as IndexKey;
            if (otherIndexKeyReloaded != null)
            {
                int currentCompareValue = 0;

                for (int i = 0; i < _indexKeyValues.Count; i++)
                {
                    currentCompareValue = _indexKeyValues[i].CompareTo(otherIndexKeyReloaded.IndexKeyValues[i]);

                    if (currentCompareValue == 0)
                    {
                        return 0;
                    }
                }

                return currentCompareValue;
            }
            else if (obj is ADBBaseObject && _indexKeyValues.Count == 1)
            {
                var basObj = obj as ADBBaseObject;
                return _indexKeyValues[0].CompareTo(basObj);
            }
            else
            {
                throw new GraphDBException(new Error_ArgumentException("Object is not an IndexKey"));
            }

        }

        #endregion

        #region IFastSerializationTypeSurrogate Members

        public bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (IndexKey)value);
        }

        public object Deserialize(SerializationReader reader, Type type)
        {
            IndexKey thisObject = (IndexKey)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        public uint TypeCode
        {
            get { return 601; }
        }

        #endregion

        #region IEstimable Members

        public ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        private ulong GetBaseSize()
        {
            //ClassDefaultSize + IndexKeyValues + EstimatedSize + HashCode
            return EstimatedSizeConstants.ClassDefaultSize + EstimatedSizeConstants.List + EstimatedSizeConstants.UInt64 + EstimatedSizeConstants.Int32;
        }

        #endregion
    }

}
