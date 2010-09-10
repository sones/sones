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

/* GraphFS - HashSetObject<T>
 * (c) Achim Friedland, 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

using sones.Lib;
using sones.Lib.Serializer;
using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.NewFastSerializer;
using sones.GraphFS.Exceptions;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// This implements a generic HashSet data structure.
    /// </summary>

    

    public class HashSetObject<T> : AFSObject, ICollection<T>, IEnumerable<T>
    {


        #region Data

        private HashSet<T>  _HashSet;

        #endregion

 
        #region Constructor

        #region HashSetObject()

        /// <summary>
        /// This will create an empty HashSetObject
        /// </summary>
        public HashSetObject()
        {

            // Members of AGraphStructure
            _StructureVersion   = 1;

            // Members of AGraphObject
            _ObjectStream       = "HASHSET_OF_...";

            // Object specific data...
            _HashSet            = new HashSet<T>();

        }

        #endregion

        #region HashSetObject(myICollection)

        /// <summary>
        /// This will create a HashSetObject using the given myICollection
        /// </summary>
        public HashSetObject(ICollection<T> myICollection)
            : this()
        {
            _HashSet = new HashSet<T>(myICollection);
        }

        #endregion

        #region HashSetObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">The ObjectLocation</param>
        /// <param name="mySerializedData">An array of bytes containing the serialized HashSetObject</param>
        public HashSetObject(String myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
            : this()
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);
            _isNew = false;

        }

        #endregion

        #endregion


        #region Members of AGraphObject

        #endregion


        #region Object-specific methods

        #region ICollection<T> Member

        public void Add(T item)
        {
            _HashSet.Add(item);
        }

        public void Clear()
        {
            _HashSet.Clear();
        }

        public bool Contains(T item)
        {
            return _HashSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _HashSet.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _HashSet.Count; }
        }

        public bool IsReadOnly
        {
            get {  throw new NotImplementedException(); }
        }

        public bool Remove(T item)
        {
            return _HashSet.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Member

        public IEnumerator<T> GetEnumerator()
        {
            return _HashSet.GetEnumerator();
        }

        #endregion

        #region IEnumerable Member

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _HashSet.GetEnumerator();
        }

        #endregion


        #region Members of AGraphObject

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new HashSetObject<T>();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion

        #endregion


        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {

            try
            {

                #region ListType T
                
                mySerializationWriter.WriteType(typeof(T));

                #endregion

                #region T is IFastSerializeable

                if ((typeof(IFastSerialize)).IsAssignableFrom(typeof(T)))
                {

                    mySerializationWriter.WriteBoolean(true);
                    mySerializationWriter.WriteUInt32( (UInt32) _HashSet.Count);

                    foreach (T _Item in _HashSet)
                        ((IFastSerialize)_Item).Serialize(ref mySerializationWriter);

                }

                #endregion

                #region T is _not_ IFastSerializeable

                else
                {

                    mySerializationWriter.WriteBoolean(false);
                    mySerializationWriter.WriteUInt32((UInt32)_HashSet.Count);

                    foreach (T _Item in _HashSet)
                        mySerializationWriter.WriteObject(_Item);

                }

                #endregion

            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }

        }


        public override void Deserialize(ref SerializationReader mySerializationReader)
        {

            try
            {

                #region Read T Type

                Type ListType = mySerializationReader.ReadTypeOptimized();

                if (ListType != typeof(T))
                    throw new GraphFSException_TypeParametersDiffer("Type parameter PT of List<PT> is different from the serialized ListType<" + ListType.ToString() + ">!");

                #endregion

                #region Read List items

                Object  ListItem;
                Boolean ListTypeIsIFastSerializeable    = mySerializationReader.ReadBoolean();
                UInt32  NumberOfListEntries             = mySerializationReader.ReadUInt32();

                for (UInt32 i=0; i < NumberOfListEntries; i++)
                {

                    if (ListTypeIsIFastSerializeable)
                    {
                        ListItem = Activator.CreateInstance(ListType, mySerializationReader.ReadObject());
                        //ListItemBytes  = (Byte[]) mySerializationReader.ReadObject();
                        //((IFastSerialize) ListItem).Deserialize(ListItemBytes);
                    }

                    else
                        ListItem = mySerializationReader.ReadObject();

                    _HashSet.Add((T)ListItem);

                }

                #endregion

            }

            catch (Exception e)
            {
                throw new Exception("ListObject<" + typeof(T).ToString() + "> could not be deserialized!\n\n" + e);
            }

        }


    }

}
