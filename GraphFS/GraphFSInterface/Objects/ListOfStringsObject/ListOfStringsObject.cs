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
