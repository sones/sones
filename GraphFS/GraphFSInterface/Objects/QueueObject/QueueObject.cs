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


/* PandoraFS - QueueObject
 * Achim Friedland, 2009
 * 
 * This implements a queue data structure.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using sones.Lib.Serializer;

using sones.Lib;

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// This implements a queue data structure.
    /// </summary>
    public class QueueObject<T> : AFSObject
    {


        #region Data

        private UInt64   _DefaultMaxSizeOfQueue = 100000;
        private UInt64   _MaxSizeOfQueue;
        private Queue<T> _Queue;

        #endregion

 
        #region Constructor

        #region QueueObject()

        /// <summary>
        /// This will create an empty QueueObject
        /// </summary>
        public QueueObject()
        {

            // Members of AGraphStructure
            _StructureVersion        = 1;

            // Members of AGraphObject
            _ObjectStream        = FSConstants.QUEUESTREAM;

            // Object specific data...
            _Queue                   = new Queue<T>();
            _MaxSizeOfQueue          = _DefaultMaxSizeOfQueue;

        }

        #endregion

        #region QueueObject(myMaxSizeOfQueue)

        /// <summary>
        /// This will create an empty QueueObject
        /// </summary>
        public QueueObject(UInt64 myMaxSizeOfQueue)
            : this()
        {
            _MaxSizeOfQueue         = myMaxSizeOfQueue;
        }

        #endregion

        #region QueueObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">The ObjectLocation</param>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized QueueObject</param>
        public QueueObject(ObjectLocation myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
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

        #region Enqueue(mySerializedData)

        public void Enqueue(T myData)
        {

            _Queue.Enqueue(myData);

            while (( (UInt64) _Queue.Count) > _MaxSizeOfQueue)
                _Queue.Dequeue();

        }

        #endregion

        #region Contains(myQueueItem)

        public Boolean Contains(T myQueueItem)
        {
            return _Queue.Contains(myQueueItem);
        }

        #endregion

        #region Count

        public int Count
        {
        
            get 
            {
                return _Queue.Count;
            }

        }

        #endregion


        #region Dequeue()

        public T Dequeue()
        {
            return _Queue.Dequeue();
        }

        #endregion

        #region Clear()

        public void Clear()
        {
            _Queue.Clear();
        }

        #endregion


        #region ToArray()

        public T[] ToArray()
        {
            return _Queue.ToArray();
        }

        #endregion

        #endregion


        #region Members of AGraphObject

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new QueueObject<T>();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion



        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {

            try
            {

                #region QueueObject Header

                // Write the type of the stored data
                mySerializationWriter.WriteType(typeof(T));

                // Write if T is IFastSerializeable 
                if ((typeof(IFastSerialize)).IsAssignableFrom(typeof(T)))
                {
                    mySerializationWriter.WriteBoolean(true);
                }
                else
                {
                    mySerializationWriter.WriteBoolean(false);
                }

                mySerializationWriter.WriteUInt64(_MaxSizeOfQueue);

                #endregion

                #region QueueObject Data

                mySerializationWriter.WriteUInt32((UInt32) _Queue.Count);

                if ((typeof(IFastSerialize)).IsAssignableFrom(typeof(T)))
                {
                    foreach (T _QueueItem in _Queue)
                        ((IFastSerialize)_QueueItem).Serialize(ref mySerializationWriter);
                }
                else
                {
                    foreach (T _QueueItem in _Queue)
                        mySerializationWriter.WriteObject(_QueueItem);
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

                #region QueueObject Header

                Type TType                      = mySerializationReader.ReadTypeOptimized();
                Boolean TIsIFastSerializeable   = mySerializationReader.ReadBoolean();
                _MaxSizeOfQueue                 = mySerializationReader.ReadUInt64();

                #endregion

                #region QueueObject Data

                Object  TObject;
                UInt32 NrOfQueueEntries         = mySerializationReader.ReadUInt32();

                for (UInt64 i=0; i < NrOfQueueEntries; i++)
                {

                    if (TIsIFastSerializeable)
                    {
                        TObject = Activator.CreateInstance(TType);
                        //Byte[] KeyBytes = (Byte[]) mySerializationReader.ReadObject();
                        //((IFastSerialize) TObject).Deserialize(KeyBytes);
                    }

                    else
                        TObject = mySerializationReader.ReadObject();

                    _Queue.Enqueue((T) TObject);

                }

                #endregion

            }

            catch (Exception e)
            {
                throw new Exception("QueueObject could not be deserialized!\n\n" + e);
            }

        }


    }

}
