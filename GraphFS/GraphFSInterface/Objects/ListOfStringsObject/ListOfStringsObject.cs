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

/* GraphFS - ListOfStringsObject
 * (c) Achim Friedland, 2008 - 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using System.Collections.Generic;
using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    ///  A ListOfStringsObject to store user defined information.
    /// </summary>

    public class ListOfStringsObject : ListObject<String>
    {


        #region Constructor

        #region ListOfStringsObject()

        /// <summary>
        /// This will create an empty ListOfStringsObject
        /// </summary>
        public ListOfStringsObject()
        {

            // Members of AGraphStructure
            _StructureVersion   = 1;

            // Members of AGraphObject
            _ObjectStream       = FSConstants.LISTOF_STRINGS;

            // Object specific data...
            _List               = new List<String>();

        }

        #endregion

        #region ListOfStringsObject(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">The ObjectLocation</param>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized ListOfStringsObject</param>
        public ListOfStringsObject(Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);
            _isNew = false;

        }

        #endregion

        #endregion


        #region Members of AGraphObject

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new ListOfStringsObject();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion


        #region ListOfStrings

        public List<String> ListOfStrings
        {

            get
            {
                return _List;
            }

            set
            {
                _List = value;
            }

        }

        #endregion



    }

}
