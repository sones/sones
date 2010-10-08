/* <id Name=”GraphFS – RightsObject” />
 * <copyright file=”RightsObject.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>The RightsObject carries information about a bunch of Rights<summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using sones.Lib;
using sones.Lib.BTree;
using sones.Lib.Serializer;

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.GraphFS.Objects;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.InternalObjects
{

    /// <summary>
    /// The RightsObject carries information about a bunch of Rights.
    /// </summary>

    
    public class RightsObject : ADictionaryObject<RightUUID, Right>
    {


        #region Constructor

        #region RightsObject()

        /// <summary>
        /// This will create an empty RightsObject
        /// </summary>
        public RightsObject()
        {

            // Members of AGraphStructure
            _StructureVersion   = 1;

            // Members of AGraphObject
            _ObjectStream   = FSConstants.RIGHTSSTREAM;

            // Object specific data...

        }

        #endregion

        #region RightsObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">the location of the RightsObject (constisting of the ObjectPath and ObjectName) within the file system</param>
        /// <param name="mySerializedData">An array of bytes[] containing the serialized RightsObject</param>
        public RightsObject(ObjectLocation myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
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

            var newT = new RightsObject();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion


        #region Object-specific methods

        #region AddRight(myRight)

        public void AddRight(Right myRight)
        {
            Add(myRight.UUID, myRight);
        }

        #endregion

        #region ContainsRight(myRightUUID)

        public Trinary ContainsRight(RightUUID myRightUUID)
        {
            return ContainsKey(myRightUUID);
        }

        #endregion

        #region ContainsName(myLogin)

        public Boolean ContainsName(String myName)
        {
            return ListOfNames.Contains(myName);
        }

        #endregion


        //ToDo: Remove this!
        #region this[String myRightUUID]

        public new Right this[RightUUID myRightUUID]
        {

            get 
            {
                return base[myRightUUID];
            }

            set 
            {
                base[myRightUUID] = value;
                isDirty                = true;
            }

        }

        #endregion

        #region GetRightByName(myRightName)

        public Right GetRightByName(String myRightName)
        {

            foreach (Right _Right in this.Values())
                if (_Right.RightsName.Equals(myRightName))
                    return _Right;

            return null;

        }

        #endregion

        #region ListOfNames

        public List<String> ListOfNames
        {

            get
            {

                var _ListOfNames = new List<String>();

                foreach (var _Right in this.Values())
                    _ListOfNames.Add(_Right.RightsName);

                return _ListOfNames;

            }

        }

        #endregion

        #region ListOfUUIDs

        public List<UUID> ListOfUUIDs
        {

            get
            {

                var ListOfUUIDs = new List<UUID>();

                foreach (var _Right in this.Values())
                    ListOfUUIDs.Add(_Right.UUID);

                return ListOfUUIDs;

            }

        }

        #endregion

        #region ListOfRights

        public List<Right> ListOfRights
        {

            get
            {
                var _ListOfRights = new List<Right>();

                foreach (var _Right in this.Values())
                    _ListOfRights.Add(_Right);

                return _ListOfRights;

            }

        }

        #endregion


        #region GetEnumerator()

        public IEnumerator<KeyValuePair<RightUUID, Right>> GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IsUserDefined(myLogin)

        public Boolean IsUserDefined(RightUUID myRightUUID)
        {
            return this[myRightUUID].IsUserDefined;
        }

        #endregion


        #region RemoveRight(myRight)

        public Boolean RemoveRight(RightUUID myRightUUID)
        {
            return Remove(myRightUUID);
        }

        #endregion

        #endregion

        #region IEstimable Members

        public override ulong GetEstimatedSize()
        {
            return EstimatedSizeConstants.UndefinedObjectSize;
        }

        public override ulong GetEstimatedSizeOfKey(RightUUID myTKey)
        {
            return myTKey.GetEstimatedSize();
        }

        public override ulong GetEstimatedSizeOfValue(Right myTValue)
        {
            return myTValue.GetEstimatedSize();
        }

        #endregion
    }

}
