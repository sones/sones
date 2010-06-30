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


/* PandoraFS - ListObject
 * Achim Friedland, 2009
 * 
 * This implements a generic list data structure.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

using sones.Lib;
using sones.Lib.Serializer;
using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures;
using sones.GraphFS.Exceptions;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// This implements a generic list data structure.
    /// </summary>
    public class ListObject<T> : AFSObject, IList, IList<T>, ICollection, ICollection<T>, IEnumerable, IEnumerable<T>
    {


        #region Data

        protected List<T>  _List;

        #endregion

 
        #region Constructor

        #region ListObject()

        /// <summary>
        /// This will create an empty ListObject
        /// </summary>
        public ListObject()
        {

            // Members of APandoraStructure
            _StructureVersion        = 1;

            // Members of APandoraObject
            _ObjectStream           = "LIST_OF_...";

            // Object specific data...
            _List                   = new List<T>();

        }

        #endregion

        #region ListObject(myList)

        /// <summary>
        /// This will create a ListObject using the given list of T
        /// </summary>
        public ListObject(List<T> myList)
        {
            _List = myList;
        }

        #endregion

        #region ListObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">The ObjectLocation</param>
        /// <param name="mySerializedData">An array of bytes containing the serialized ListObject</param>
        public ListObject(ObjectLocation myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);
            _isNew = false;

        }

        #endregion

        #endregion


        #region Members of APandoraObject

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new ListObject<T>();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion


        #region Object-specific methods

        // Add all these List methods here!

        #region IList Member

        #region Add(myObject)

        public Int32 Add(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an Right object
            if (!(myObject is T))
                throw new ArgumentException("myObject is not of type " + typeof(T).ToString() + "!");
            
            Int32 _ReturnValue = ((IList) _List).Add(myObject);
            isDirty = true;

            return _ReturnValue;

        }

        #endregion

        #region Contains(myObject)

        public Boolean Contains(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an Right object
            if (!(myObject is T))
                throw new ArgumentException("myObject is not of type " + typeof(T).ToString() + "!");

            return ((IList) _List).Contains(myObject);
        
        }

        #endregion

        #region IndexOf(myObject)

        public Int32 IndexOf(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an Right object
            if (!(myObject is T))
                throw new ArgumentException("myObject is not of type " + typeof(T).ToString() + "!");

            return ((IList) _List).IndexOf(myObject);

        }

        #endregion

        #region Insert(myIndex, myObject)

        public void Insert(Int32 myIndex, Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an Right object
            if (!(myObject is T))
                throw new ArgumentException("myObject is not of type " + typeof(T).ToString() + "!");

            ((IList) _List).Insert(myIndex, myObject);
            isDirty = true;

        }

        #endregion

        #region IsFixedSize

        public Boolean IsFixedSize
        {
            get
            {
                return ((IList)_List).IsFixedSize;
            }
        }

        #endregion

        #region Remove(myObject)

        public void Remove(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an Right object
            if (!(myObject is T))
                throw new ArgumentException("myObject is not of type " + typeof(T).ToString() + "!");

            ((IList)_List).Remove(myObject);
            isDirty = true;

        }

        #endregion

        #region RemoveAt(myIndex)

        public void RemoveAt(Int32 myIndex)
        {
            ((IList)_List).RemoveAt(myIndex);
            isDirty = true;
        }

        #endregion

        #region this[myIndex]

        Object IList.this[Int32 myIndex]
        {
            get
            {
                return ((IList)_List)[myIndex];
            }
            set
            {
                ((IList)_List)[myIndex] = value;
                isDirty = true;
            }
        }

        #endregion

        #endregion

        #region IList<T> Members

        #region IndexOf(myItem)

        public Int32 IndexOf(T myItem)
        {
            return _List.IndexOf(myItem);
        }

        #endregion

        #region Insert(myIndex, myItem)

        public void Insert(Int32 myIndex, T myItem)
        {
            _List.Insert(myIndex, myItem);
            isDirty = true;
        }

        #endregion

        #region RemoveAt(myIndex)

        public void RemoveAt(UInt64 myIndex)
        {
            _List.RemoveAt((Int32)myIndex);
            isDirty = true;
        }

        #endregion

        #region this[myIndex]

        public T this[Int32 myIndex]
        {

            get
            {
                return _List[myIndex];
            }

            set
            {
                _List[myIndex] = value;
                isDirty = true;
            }

        }

        #endregion

        #endregion

        #region ICollection Member

        #region CopyTo(myArray, myIndex)

        public void CopyTo(Array myArray, Int32 myIndex)
        {
            ((ICollection)_List).CopyTo(myArray, myIndex);
        }

        #endregion

        #region Count

        int ICollection.Count
        {
            get
            {
                return ((ICollection)_List).Count;
            }
        }

        #endregion

        #region IsSynchronized

        public Boolean IsSynchronized
        {
            get
            {
                return ((ICollection)_List).IsSynchronized;
            }
        }

        #endregion

        #region SyncRoot

        public Object SyncRoot
        {
            get
            {
                return ((ICollection)_List).SyncRoot;
            }
        }

        #endregion

        #endregion

        #region ICollection<T> Members

        #region Add(myItem)

        public void Add(T myItem)
        {
            _List.Add(myItem);
            isDirty = true;
        }

        #endregion

        #region Clear()

        public void Clear()
        {
            _List.Clear();
            isDirty = true;
        }

        #endregion

        #region Contains(myItem)

        public Boolean Contains(T myItem)
        {
            return _List.Contains(myItem);
        }

        #endregion

        #region CopyTo(myArray, myArrayIndex)

        public void CopyTo(T[] myArray, Int32 myArrayIndex)
        {
            _List.CopyTo(myArray, myArrayIndex);
        }

        #endregion

        #region Count

        public Int32 Count
        {
            get
            {
                return _List.Count;
            }
        }

        #endregion

        #region IsReadOnly

        public Boolean IsReadOnly
        {

            get
            {
                return ( (ICollection<T>) _List).IsReadOnly;
            }

        }

        #endregion

        #region Remove(myItem)

        public Boolean Remove(T myItem)
        {
            return _List.Remove(myItem);
        }

        #endregion

        #endregion

        #region IEnumerable Members

        #region GetEnumerator()

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        #endregion

        #endregion

        #region IEnumerable<T> Members

        #region GetEnumerator<T>()

        public IEnumerator<T> GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        #endregion

        #endregion

        #endregion


      



        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {

            try
            {

                #region ListType T

                mySerializationWriter.WriteObject(typeof(T));

                #endregion

                #region T is IFastSerializeable

                if ((typeof(IFastSerialize)).IsAssignableFrom(typeof(T)))
                {

                    mySerializationWriter.WriteObject(true);
                    mySerializationWriter.WriteObject( (UInt64) _List.Count);

                    foreach (T _Item in _List)
                        ((IFastSerialize)_Item).Serialize(ref mySerializationWriter);

                }

                #endregion

                #region T is _not_ IFastSerializeable

                else
                {

                    mySerializationWriter.WriteObject(false);
                    mySerializationWriter.WriteObject( (UInt64) _List.Count);

                    foreach (T _Item in _List)
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

                Type    ListType                        = (Type) mySerializationReader.ReadObject();

                if (ListType != typeof(T))
                    throw new PandoraFSException_TypeParametersDiffer("Type parameter PT of List<PT> is different from the serialized ListType<" + ListType.ToString() + ">!");

                #endregion

                #region Read List items

                Object  ListItem;
                Byte[]  ListItemBytes;
                Boolean ListTypeIsIFastSerializeable    = (Boolean) mySerializationReader.ReadObject();
                UInt64  NumberOfListEntries             = (UInt64)  mySerializationReader.ReadObject();

                for (UInt64 i=0; i < NumberOfListEntries; i++)
                {

                    if (ListTypeIsIFastSerializeable)
                    {
                        ListItem       = Activator.CreateInstance(ListType);
                      //  ListItemBytes  = (Byte[]) mySerializationReader.ReadObject();
                        ((IFastSerialize)ListItem).Deserialize(ref mySerializationReader);
                    }

                    else
                        ListItem = mySerializationReader.ReadObject();

                    _List.Add( (T) ListItem);

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
