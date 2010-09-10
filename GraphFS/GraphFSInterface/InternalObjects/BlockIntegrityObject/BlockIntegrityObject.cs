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

/* PandoraFS - BlockIntegrityObject
 * (c) Achim Friedland, 2008 - 2009
 * 
 * This represents a structure for saving the block integrity check
 * values of an object stream within an array of fixed sized byte arrays.
 * This structure can handle multiple block integrity check arrays using
 * different block sizes.
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

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.InternalObjects
{

    /// <summary>
    /// This represents a structure for saving the block integrity check
    /// values of an object stream within an array of fixed sized byte arrays.
    /// This structure can handle multiple block integrity check arrays using
    /// different block sizes.
    /// </summary>

    

    public class BlockIntegrityObject : ADictionaryObject<UInt64, Byte[]>
    {

        #region Data

        byte[] _SerializedObject;
        UInt64 _SerializedSize;

        #endregion


        #region Constructor

        #region BlockIntegrityObject()

        /// <summary>
        /// This will create an empty BlockIntegrityObject
        /// </summary>
        public BlockIntegrityObject()
        {

            // Members of APandoraStructure
            _StructureVersion   = 1;

            // Members of APandoraObject
            _ObjectStream   = FSConstants.BLOCKINTEGRITYSTREAM;

            // Object specific data...

        }

        #endregion

        #region BlockIntegrityObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">the location of the BlockIntegrityObject (constisting of the ObjectPath and ObjectName) within the file system</param>
        /// <param name="mySerializedData">An array of bytes[] containing the serialized BlockIntegrityObject</param>
        public BlockIntegrityObject(ObjectLocation myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);
            _isNew = false;

        }

        #endregion

        #endregion


        #region Members of APandoraObject

        #endregion



        #region Objectspecific methods

        #region IntegrityCheckAlgorithm

        private IntegrityCheckTypes _IntegrityCheckAlgorithm;

        /// <summary>
        /// The algorithm used for block integrity checking
        /// </summary>
        public IntegrityCheckTypes IntegrityCheckAlgorithm
        {
            get
            {
                return _IntegrityCheckAlgorithm;
            }

            set
            {
                _IntegrityCheckAlgorithm  = value;
                isDirty                   = true;
//                _BlockIntegrityArray          = new Dictionary<UInt64, Byte[][]>();
            }

        }

        #endregion

        //#region BlockIntegrityArray

        ///// <summary>
        ///// A structure for saving the block integrity check values of an object
        ///// stream within an array of fixed sized byte arrays. This structure can
        ///// handle multiple block integrity check arrays using different block sizes
        ///// by using a dictionary <BlockSize, Byte[NumberOfBlocks][IntegrityCheckBlockSize]>
        ///// </summary>
        //public Dictionary<UInt64, Byte[][]> BlockIntegrityArray
        //{
        //    get
        //    {
        //        return _BlockIntegrityArray;
        //    }

        //    set
        //    {
        //        _BlockIntegrityArray = value;
        //        isDirty = true;
        //        _IndexHashTable = new Dictionary<UInt64, Byte[][]>();
        //    }

        //}

        //#endregion
        
        #endregion


        #region Members of APandoraObject

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new BlockIntegrityObject();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion

    }

}

