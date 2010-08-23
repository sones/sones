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


/* <id Name=”PandoraFS – Entity object” />
 * <copyright file=”EntitiesObject.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>The EntitiesObject carries information about a bunch of Entities<summary>
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

using sones.Lib.DataStructures.PasswordHash;
using sones.Lib.DataStructures.PublicKey;
using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Exceptions;

#endregion

namespace sones.GraphFS.InternalObjects
{

    /// <summary>
    /// The EntitiesObject carries information about a bunch of Entities.
    /// </summary>
    public class EntitiesObject : ADictionaryObject<EntityUUID, Entity>
    {


        #region Constructor

        #region EntitiesObject()

        /// <summary>
        /// This will create an empty EntitiesObject
        /// </summary>
        public EntitiesObject()
        {

            // Members of AGraphStructure
            _StructureVersion = 1;

            // Members of AGraphObject
            _ObjectStream = FSConstants.ENTITIESSTREAM;

            // Object specific data...

        }

        #endregion

        #region EntitiesObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">the location of the EntitiesObject (constisting of the ObjectPath and ObjectName) within the file system</param>
        /// <param name="mySerializedData">An array of bytes[] containing the serialized EntitiesObject</param>
        public EntitiesObject(ObjectLocation myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
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

            var newT = new EntitiesObject();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion


        #region Object-specific methods

        #region AddEntity(myLogin, myRealname, myDescription, myContactList, myPassword, myPublicKeyList, MyMembership)

        public EntityUUID AddEntity(String myLogin, String myRealname, String myDescription, Dictionary<ContactTypes, List<String>> myContactList, String myPassword, List<PublicKey> myPublicKeyList, HashSet<EntityUUID> MyMembership)
        {

            #region Validate IsMemberOf...

            if (MyMembership == null)
                throw new ArgumentNullException("The IsMemberOf must not be null!");

            foreach (EntityUUID _MembershipUUID in MyMembership)
                if (base.ContainsKey(_MembershipUUID) == Trinary.FALSE)
                    throw new GraphFSException_EntityNotFound("The entity could not be added, as the membership includes an unkown EntityUUID: " + _MembershipUUID + "!");

            #endregion

            #region Generate a new Entity and prove the uniqueness of its EntityUUID

            Entity _Entity;

            // Prove its uniquness... even this is veeeery unlikely to fail!
            do
            {
                _Entity = new Entity(myLogin, myRealname, myDescription, myContactList, myPassword, myPublicKeyList, MyMembership);
            }
            while (base.ContainsKey(_Entity.UUID) == Trinary.TRUE);

            #endregion

            base.Add(_Entity.UUID, _Entity);

            isDirty = true;

            return _Entity.UUID;

        }

        #endregion


        #region ChangeRealname(myEntityUUID, myPassword, myNewRealname)

        public void ChangeRealname(EntityUUID myEntityUUID, String myPassword, String myNewRealname)
        {

            #region Input Exceptions

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            #endregion

            base[myEntityUUID].ChangeRealname(myPassword, myNewRealname);

            isDirty = true;

        }

        #endregion

        #region ChangeDescription(myEntityUUID, myPassword, myNewDescription)

        public void ChangeDescription(EntityUUID myEntityUUID, String myPassword, String myNewDescription)
        {

            #region Input Exceptions

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            #endregion

            base[myEntityUUID].ChangeDescription(myPassword, myNewDescription);

            isDirty = true;

        }

        #endregion

        #region ChangeContactList(myEntityUUID, myPassword, myNewContactList)

        public void ChangeContactList(EntityUUID myEntityUUID, String myPassword, Dictionary<ContactTypes, List<String>> myNewContactList)
        {

            #region Input Exceptions

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            #endregion

            base[myEntityUUID].ChangeContactList(myPassword, myNewContactList);

            isDirty = true;

        }

        #endregion

        #region ChangePassword(myEntityUUID, myOldPassword, myNewPassword)

        public void ChangePassword(EntityUUID myEntityUUID, String myOldPassword, String myNewPassword)
        {

            #region Input Exceptions

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            #endregion

            base[myEntityUUID].ChangePassword(myOldPassword, myNewPassword);

            isDirty = true;

        }

        #endregion

        #region ChangePublicKeyList(myEntityUUID, myPassword, myNewPublicKeyList)

        public void ChangePublicKeyList(EntityUUID myEntityUUID, String myPassword, List<PublicKey> myNewPublicKeyList)
        {

            #region Input Exceptions

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            #endregion

            base[myEntityUUID].ChangePublicKeyList(myPassword, myNewPublicKeyList);

            isDirty = true;

        }

        #endregion


        #region this[myEntityUUID] <== REMOVE ME!

        //ToDo: this[myEntityUUID]

        /// <summary>
        /// Returns the entity with the given EntityUUID
        /// </summary>
        /// <param name="myEntityUUID">the EntityUUID of the requested entity</param>
        /// <returns>the entity with the given name</returns>
        /// <exception cref="GraphFSException_EntityNotFound">if no entity with the given EntityUUID could be found!</exception>
        public new Entity this[EntityUUID myEntityUUID]
        {

            get
            {

                if (base.ContainsKey(myEntityUUID) == Trinary.TRUE)
                    return base[myEntityUUID];

                throw new GraphFSException_EntityNotFound("An entity with UUID '" + myEntityUUID + "' could not be found!");

            }

            set
            {
                base[myEntityUUID] = value;
                isDirty = true;
            }

        }

        #endregion

        #region ContainsEntityUUID(myEntityUUID)

        public Trinary ContainsEntityUUID(EntityUUID myEntityUUID)
        {
            return base.ContainsKey(myEntityUUID);
        }

        #endregion


        #region GetEntityUUID(myLogin)

        /// <summary>
        /// Returns the entity with the given name
        /// </summary>
        /// <param name="myLogin">the login of the requested entity</param>
        /// <returns>the EntityUUID with the given login</returns>
        /// <exception cref="GraphFSException_EntityNotFound">if no entity with the given login could be found!</exception>
        public EntityUUID GetEntityUUID(String myLogin)
        {

            #region Input validation

            if (myLogin == null || myLogin.Length == 0)
                throw new ArgumentNullException("The login must not be null or its length be zero!");

            #endregion

            foreach (KeyValuePair<EntityUUID, Entity> _KeyValuePair in _IDictionary)
                if (_KeyValuePair.Value.Login.Equals(myLogin))
                    return _KeyValuePair.Key;

            throw new GraphFSException_EntityNotFound("An entity with login name '" + myLogin + "' could not be found!");

        }

        #endregion

        #region GetRealname(myEntityUUID)

        public String GetRealname(EntityUUID myEntityUUID)
        {

            #region Input validation

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            #endregion

            return base[myEntityUUID].Realname;

        }

        #endregion

        #region GetDescription(myEntityUUID)

        public String GetDescription(EntityUUID myEntityUUID)
        {

            #region Input validation

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            #endregion

            return base[myEntityUUID].Description;

        }

        #endregion

        #region GetContactList(myEntityUUID)

        public Dictionary<ContactTypes, List<String>> GetContactList(EntityUUID myEntityUUID)
        {

            #region Input validation

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            #endregion

            return base[myEntityUUID].ContactList;

        }

        #endregion

        #region GetPublicKeyList(myEntityUUID)

        public List<PublicKey> GetPublicKeyList(EntityUUID myEntityUUID)
        {

            #region Input validation

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            #endregion

            return base[myEntityUUID].PublicKeyList;

        }

        #endregion

        #region (private) GetRecursiveMemberships(myEntityUUID, ref myRecursiveMemberships)

        private void GetRecursiveMemberships(EntityUUID myEntityUUID, ref HashSet<EntityUUID> myRecursiveMemberships)
        {

            myRecursiveMemberships.Add(myEntityUUID);

            foreach (EntityUUID _MembershipUUID in base[myEntityUUID].Memberships)
                if (!myRecursiveMemberships.Contains(_MembershipUUID))
                    GetRecursiveMemberships(_MembershipUUID, ref myRecursiveMemberships);

        }

        #endregion

        #region GetMemberships(myEntityUUID)

        public HashSet<EntityUUID> GetMemberships(EntityUUID myEntityUUID, Boolean myRecursion)
        {

            #region Input validation

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            //if (myRecursion == null)
            //    myRecursion = false;

            #endregion

            #region No recursion...

            if (!myRecursion)
                return base[myEntityUUID].Memberships;

            #endregion

            #region ...or recursion!

            HashSet<EntityUUID> RecursiveMemberships = new HashSet<EntityUUID>();

            GetRecursiveMemberships(myEntityUUID, ref RecursiveMemberships);

            // Remove myself from the memberships list!
            //RecursiveMemberships.Remove(myEntityUUID);

            return RecursiveMemberships;

            #endregion

        }

        #endregion


        #region ListOfEntityUUIDs

        public List<EntityUUID> ListOfEntityUUIDs
        {

            get
            {

                var _ListOfEntityUUIDs = new List<EntityUUID>();

                foreach (var _EntityUUID in base.Keys())
                    _ListOfEntityUUIDs.Add(_EntityUUID);

                return _ListOfEntityUUIDs;

            }

        }

        #endregion


        #region AddMembership(myEntityUUID, myMembershipUUID)

        public void AddMembership(EntityUUID myEntityUUID, EntityUUID myMembershipUUID)
        {

            #region Input validation

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            if (myMembershipUUID == null)
                throw new ArgumentNullException("The MembershipUUID must not be null!");

            #endregion

            #region  Check if the MembershipUUID is valid!

            if (base.ContainsKey(myMembershipUUID) != Trinary.TRUE)
                throw new GraphFSException_EntityNotFound("An (membership) entity with UUID '" + myMembershipUUID + "' could not be found!");

            #endregion

            base[myEntityUUID].AddMembership(myMembershipUUID);

            isDirty = true;

        }

        #endregion

        #region AddMemberships(myEntityUUID, myMembershipUUIDs)

        public void AddMembership(EntityUUID myEntityUUID, HashSet<EntityUUID> myMembershipUUIDs)
        {

            #region Input validation

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            if (myMembershipUUIDs == null || myMembershipUUIDs.Count == 0)
                throw new ArgumentNullException("The MembershipUUID HashSet must not be null or its size be zero!");

            #endregion

            #region  Check if the MembershipUUIDs are valid!

            foreach (EntityUUID _MembershipUUID in myMembershipUUIDs)
                if (base.ContainsKey(_MembershipUUID) != Trinary.TRUE)
                    throw new GraphFSException_EntityNotFound("An (membership) entity with UUID '" + _MembershipUUID + "' could not be found!");

            #endregion

            base[myEntityUUID].AddMemberships(myMembershipUUIDs);

            isDirty = true;

        }

        #endregion

        #region RemoveMembership(myEntityUUID, myMembershipUUID)

        public void RemoveMembership(EntityUUID myEntityUUID, EntityUUID myMembershipUUID)
        {

            #region Input validation

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            if (myMembershipUUID == null)
                throw new ArgumentNullException("The MembershipUUID must not be null!");

            #endregion

            base[myEntityUUID].RemoveMembership(myMembershipUUID);

            isDirty = true;

        }

        #endregion

        #region RemoveMemberships(myEntityUUID, myMembershipUUIDs)

        public void RemoveMemberships(EntityUUID myEntityUUID, HashSet<EntityUUID> myMembershipUUIDs)
        {

            #region Input validation

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            if (myMembershipUUIDs == null || myMembershipUUIDs.Count == 0)
                throw new ArgumentNullException("The MembershipUUID HashSet must not be null or its size be zero!");

            #endregion

            base[myEntityUUID].RemoveMemberships(myMembershipUUIDs);

            isDirty = true;

        }

        #endregion


        #region RemoveEntity(myEntityUUID)

        public Boolean RemoveEntity(EntityUUID myEntityUUID)
        {

            #region Input validation

            if (myEntityUUID == null)
                throw new ArgumentNullException("The EntityUUID must not be null!");

            #endregion

            foreach (KeyValuePair<EntityUUID, Entity> aEntity in _IDictionary)
            {
                aEntity.Value.RemoveMembership(myEntityUUID);
            }

            isDirty = true;

            return base.Remove(myEntityUUID);

        }

        #endregion

        #endregion



    }

}
