/* GraphFS - Entity
 * (c) Henning Rauch, 2009
 *     Achim Friedland, 2009
 *  
 * The Entity class represents the generalization of the user/group model.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Serializer;

using sones.Lib.DataStructures.PasswordHash;
using sones.Lib.DataStructures.PublicKey;
using System.Runtime.Serialization;
using sones.Lib.NewFastSerializer;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Exceptions;
using sones.Lib;

#endregion

namespace sones.GraphFS.InternalObjects
{

    /// <summary>
    /// The Entity class represents the generalization of the user/group model.
    /// </summary>
    
    [AllowNonEmptyConstructor]
    public class Entity : IFastSerialize, IComparable, IComparable<Entity>, IFastSerializationTypeSurrogate, IEstimable
    {


        #region TypeCode
        public UInt32 TypeCode { get { return 203; } }
        #endregion

        #region Data

        private PasswordHash                            _PasswordHash   = null;

        #endregion


        #region Properties

        #region UUID

        private EntityUUID _UUID = null;

        /// <summary>
        /// The UUID of this entity object.
        /// Its value will be generated within the constructor of the entity object
        /// and can not be changed afterwards.
        /// </summary>
        public EntityUUID UUID
        {
            
            get
            {
                return _UUID;
            }
        
        }

        #endregion

        #region Login

        private String _Login = null;

        /// <summary>
        /// The login of this entity object.
        /// Its value will be set within the constructor of the entity object
        /// and can not be changed afterwards.
        /// </summary>
        public String Login
        {
            
            get
            {
                return _Login;
            }
        
        }

        #endregion

        #region Realname

        private String _Realname = null;

        /// <summary>
        /// The realname of this entity object.
        /// Its value will be set within the constructor of the entity object
        /// and can not be changed afterwards.
        /// </summary>
        public String Realname
        {
            
            get
            {
                return _Realname;
            }
        
        }

        #endregion

        #region Description

        private String _Description = null;

        /// <summary>
        /// The intention of the entity description is to clarify the
        /// denotation of the entity object.
        /// </summary>
        public String Description
        {

            get
            {
                return _Description;
            }

            set
            {
                _Description = value;
                isDirty = true;
            }

        }

        #endregion

        #region ContactList

        private Dictionary<ContactTypes, List<String>>  _Contacts       = null;

        /// <summary>
        /// A list of contatc information for this entity
        /// </summary>
        public Dictionary<ContactTypes, List<String>> ContactList
        {

            get
            {
                return _Contacts;
            }

            set
            {
                _Contacts = value;
                isDirty = true;
            }

        }

        #endregion

        #region Status

        private EntityStatus _Status = 0;

        /// <summary>
        /// The entity status describes...
        /// </summary>
        public EntityStatus Status
        {

            get
            {
                return _Status;
            }

            set
            {
                _Status = value;
                isDirty = true;
            }

        }

        #endregion

        #region PublicKeyList

        private List<PublicKey> _PublicKeyList = null;

        public List<PublicKey> PublicKeyList
        {

            get
            {
                return _PublicKeyList;
            }

        }

        #endregion

        #region IsMemberOf

        private HashSet<EntityUUID> _Membership = null;

        public HashSet<EntityUUID> Memberships
        {
            
            get
            {
                return _Membership;
            }
        
        }

        #endregion

        #endregion


        #region Constructor

        #region Entity()

        /// <summary>
        /// A constructor used for creating a new entity
        /// </summary>
        public Entity()
        {
            _UUID          = new EntityUUID();
            _Login         = "";
            _Contacts      = new Dictionary<ContactTypes, List<String>>();
            _Description   = "";
            _Status        = 0;
            _PasswordHash  = new PasswordHash("");
            _PublicKeyList = new List<PublicKey>();
            _Membership    = new HashSet<EntityUUID>();
        }

        #endregion

        #region Entity(myLogin, myRealname, myDescription, myContactList, myPassword, myPublicKeyList, myMembership)

        /// <summary>
        /// A constructor used for creating a new entity
        /// </summary>
        /// <param name="myLogin">The login of this entity</param>
        /// <param name="myRealname">The realname of this entity</param>
        /// <param name="myDescription">The description of this entity</param>
        /// <param name="myContactList">The contact information of this entity</param>
        /// <param name="myPassword">The password of this entity</param>
        /// <param name="myPublicKeyList">The list of public keys of this entity</param>
        /// <param name="myMembership">A HashSet of entities, in which the current entity is member of.</param>
        public Entity(String myLogin, String myRealname, String myDescription, Dictionary<ContactTypes, List<String>> myContacts, String myPassword, List<PublicKey> myPublicKeyList, HashSet<EntityUUID> myMembership)
            : this()
        {

            if (myLogin == null || myLogin.Equals(""))
                throw new ArgumentNullException("The name of the entity must not be null or its length be zero!");

            if (myRealname == null)
                throw new ArgumentNullException("The realname must not be null!");

            if (myDescription == null)
                throw new ArgumentNullException("The description must not be null!");

            if (myContacts == null)
                throw new ArgumentNullException("The contacts must not be null!");

            if (myPassword == null)
                throw new ArgumentNullException("The password of the entity must not be null!");

            if (myPublicKeyList == null)
                throw new ArgumentNullException("The public key list of the entity must not be null!");

            if (myMembership == null)
                throw new ArgumentNullException("The mebership of the entity must not be null!");

            _Login              = myLogin;
            _Realname           = myRealname;
            _Description        = myDescription;
            _Contacts           = myContacts;
            _PasswordHash       = new PasswordHash("");
            _PasswordHash.ChangePassword("", myPassword);
            _PublicKeyList      = myPublicKeyList;
            _Membership         = myMembership;

        }

        #endregion

        //#region Entity(mySerializedData)

        ///// <summary>
        ///// A constructor used for fast deserializing
        ///// </summary>
        ///// <param name="mySerializedData">An array of bytes[] containing a serialized entity</param>
        //public Entity(Byte[] mySerializedData)
        //    : this()
        //{

        //    if (mySerializedData == null || mySerializedData.Length == 0)
        //        throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

        //    Deserialize(mySerializedData);

        //}

        //#endregion

        #endregion


        #region Object-specific methods

        #region ChangeRealname(myPassword, myNewRealname)

        /// <summary>
        /// Changes the realname of this entity.
        /// </summary>
        /// <param name="myPassword">the password for this entity</param>
        /// <param name="myNewRealname">the new realname</param>
        public void ChangeRealname(String myPassword, String myNewRealname)
        {

            #region Input validation

            if (myPassword == null)
                throw new ArgumentNullException("The password must not be null!");

            if (myNewRealname == null)
                throw new ArgumentNullException("The new realname must not be null!");

            #endregion

            if (_PasswordHash.CheckPassword(myPassword))
                throw new GraphFSException_EntityPasswordInvalid("The realname could not be changed as the password is invalid!");

            _Realname = myNewRealname;

        }

        #endregion

        #region ChangeDescription(myPassword, myNewDescription)

        /// <summary>
        /// Changes the realname of this entity.
        /// </summary>
        /// <param name="myPassword">the password for this entity</param>
        /// <param name="myNewDescription">the new description</param>
        public void ChangeDescription(String myPassword, String myNewDescription)
        {

            #region Input validation

            if (myPassword == null)
                throw new ArgumentNullException("The password must not be null!");

            if (myNewDescription == null)
                throw new ArgumentNullException("The new description must not be null!");

            #endregion

            if (_PasswordHash.CheckPassword(myPassword))
                throw new GraphFSException_EntityPasswordInvalid("The description could not be changed as the password is invalid!");

            _Realname = myNewDescription;

        }

        #endregion
        
        #region ChangeContactList(myPassword, myNewContacts)

        /// <summary>
        /// Changes the contact list of this entity.
        /// </summary>
        /// <param name="myPassword">the password for this entity</param>
        /// <param name="myNewContactList">the new contact list</param>
        public void ChangeContactList(String myPassword, Dictionary<ContactTypes, List<String>> myNewContactList)
        {

            #region Input validation

            if (myPassword == null)
                throw new ArgumentNullException("The password must not be null!");

            if (myNewContactList == null)
                throw new ArgumentNullException("The new contact list must not be null!");

            #endregion

            if (_PasswordHash.CheckPassword(myPassword))
                throw new GraphFSException_EntityPasswordInvalid("The contact list could not be changed as the password is invalid!");

            _Contacts = myNewContactList;

        }

        #endregion

        #region ChangePassword(myOldPassword, myNewPassword)

        /// <summary>
        /// Changes the password of this entity.
        /// </summary>
        /// <param name="myOldPassword">the old password</param>
        /// <param name="myNewPassword">the new password</param>
        public void ChangePassword(String myOldPassword, String myNewPassword)
        {

            _PasswordHash.ChangePassword(myOldPassword, myNewPassword);

        }

        #endregion

        #region ChangePublicKeyList(myPassword, myNewPublicKey)

        /// <summary>
        /// Changes the public key of this entity.
        /// </summary>
        /// <param name="myPassword">the password for this entity</param>
        /// <param name="myNewPublicKeyList">the new public key list</param>
        public void ChangePublicKeyList(String myPassword, List<PublicKey> myNewPublicKeyList)
        {

            #region Input validation

            if (myPassword == null)
                throw new ArgumentNullException("The password must not be null!");

            if (myNewPublicKeyList == null)
                throw new ArgumentNullException("The new list of public keys must not be null!");

            #endregion

            if (_PasswordHash.CheckPassword(myPassword))
                throw new GraphFSException_EntityPasswordInvalid("The public key could not be changed as the password is invalid!");

            _PublicKeyList = myNewPublicKeyList;

        }

        #endregion


        #region Add/Check/Remove Status

        #region AddStatus(myStatus)

        /// <summary>
        /// Adds the given status to the status of this entity.
        /// </summary>
        /// <param name="myStatus">the additional status of this entity</param>
        public void AddStatus(EntityStatus myStatus)
        {
            _Status |= myStatus;
            isDirty = true;
        }
        
        #endregion

        #region CheckStatus(myStatus)

        /// <summary>
        /// Checks the status of this entity.
        /// </summary>
        /// <param name="myStatus">the new status of this entity</param>
        public Boolean CheckStatus(EntityStatus myStatus)
        {

            if ((_Status & myStatus) == myStatus)
                return true;

            return false;

        }

        #endregion

        #region RemoveStatus(myStatus)

        /// <summary>
        /// Removes the given status from the status of this entity.
        /// </summary>
        /// <param name="myStatus">the entity status to remove</param>
        public void RemoveStatus(EntityStatus myStatus)
        {
            _Status &= ~myStatus;
            isDirty = true;
        }

        #endregion

        #endregion

        #region Add/Is/Remove Membership(s)

        #region AddMembership(myMembershipUUID)

        public void AddMembership(EntityUUID myMembershipUUID)
        {

            #region Input validation

            if (myMembershipUUID == null)
                throw new ArgumentNullException();

            #endregion

            //ToDo: check if the current user can add (its) memberships

            _Membership.Add(myMembershipUUID);

            isDirty = true;

        }

        #endregion

        #region AddMemberships(myMembershipUUIDs)

        public void AddMemberships(HashSet<EntityUUID> myMembershipUUIDs)
        {

            #region Input validation

            if (myMembershipUUIDs == null)
                throw new ArgumentNullException();

            #endregion

            //ToDo: check if the current user can add (its) memberships

            foreach (EntityUUID _EntityUUID in myMembershipUUIDs)
                _Membership.Add(_EntityUUID);

            isDirty = true;

        }

        #endregion


        #region IsMemberOf(myMembershipUUID)

        public Boolean IsMemberOf(EntityUUID myMembershipUUID)
        {
            //ToDo: Recursive resolving!
            return _Membership.Contains(myMembershipUUID);
        }

        #endregion

        #region IsMemberOf(myMembershipUUIDs)

        public Boolean IsMemberOf(HashSet<EntityUUID> myMembershipUUIDs)
        {

            //ToDo: Recursive resolving!
            foreach (EntityUUID _EntityUUID in myMembershipUUIDs)
                if (!_Membership.Contains(_EntityUUID))
                    return false;

            return true;

        }

        #endregion


        #region RemoveMembership(myMembershipUUID)

        public void RemoveMembership(EntityUUID myMembershipUUID)
        {

            #region Input Exceptions

            if (myMembershipUUID == null)
                throw new ArgumentNullException();

            #endregion

            //ToDo: check if the current user can remove (its) memberships

            _Membership.Remove(myMembershipUUID);

        }

        #endregion

        #region RemoveMemberships(myMembershipUUIDs)

        public void RemoveMemberships(HashSet<EntityUUID> myMembershipUUIDs)
        {

            #region Input Exceptions

            if (myMembershipUUIDs == null)
                throw new ArgumentNullException();

            #endregion

            //ToDo: check if the current user can remove (its) memberships

            foreach (EntityUUID _EntityUUID in myMembershipUUIDs)
                _Membership.Remove(_EntityUUID);

        }

        #endregion

        #endregion


        #region Operator overloading

        #region Equals(myObject)

        /// <summary>
        /// Compares this entity with the given object.
        /// </summary>
        /// <param name="myObject">An object of type Entity</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                return false;

            // If parameter cannot be cast to Entity return false.
            Entity _Entity = myObject as Entity;
            if ((Object) _Entity == null)
                return false;

            return Equals(_Entity);

        }

        #endregion

        #region Equals(myEntity)

        /// <summary>
        /// Compares this entity with the given entity.
        /// </summary>
        /// <param name="myEntity">an entity object</param>
        /// <returns>true|false</returns>
        public Boolean Equals(Entity myEntity)
        {

            // If parameter is null return false:
            if ((Object) myEntity == null)
                return false;

            // Check if the inner fields have the same values
            if (_UUID           != myEntity.UUID)
                return false;

            if (_Login          != myEntity.Login)
                return false;

            if (_Realname       != myEntity.Realname)
                return false;

            if (_Description    != myEntity.Description)
                return false;

            // ContactList
            if (!_Contacts.Count.Equals(myEntity.ContactList.Count))
                return false;

            foreach (KeyValuePair<ContactTypes, List<String>> _KeyValuePair in _Contacts)
            {
                if (!myEntity.ContactList.ContainsKey(_KeyValuePair.Key))
                    return false;

                else
                {
                    if (myEntity.ContactList[_KeyValuePair.Key].Count != _Contacts[_KeyValuePair.Key].Count)
                        return false;

                    else
                    {
                        foreach (String _String in _Contacts[_KeyValuePair.Key])
                            if (!myEntity.ContactList[_KeyValuePair.Key].Contains(_String))
                                return false;
                    }

                }

            }

            // Status
            if (_Status         != myEntity.Status)
                return false;

            //PublicKeyList
            if (!_PublicKeyList.Count.Equals(myEntity.PublicKeyList.Count))
                return false;

            foreach (PublicKey _PublicKey in _PublicKeyList)
                if (!myEntity.PublicKeyList.Contains(_PublicKey))
                    return false;

            // Memberships
            if (!_Membership.Count.Equals(myEntity.Memberships.Count))
                return false;

            foreach (EntityUUID _EntityUUID in Memberships)
                if (!myEntity.Memberships.Contains(_EntityUUID))
                    return false;

            return true;

        }

        #endregion

        #region Operator == (myEntity1, myEntity2)

        public static Boolean operator == (Entity myEntity1, Entity myEntity2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(myEntity1, myEntity2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myEntity1 == null) || ((Object) myEntity2 == null))
                return false;

            return myEntity1.Equals(myEntity2);

        }

        #endregion

        #region Operator != (myEntity1, myEntity2)

        public static Boolean operator != (Entity myEntity1, Entity myEntity2)
        {
            return !(myEntity1 == myEntity2);
        }

        #endregion

        #region Operator < (myEntity1, myEntity2)

        public static Boolean operator < (Entity myEntity1, Entity myEntity2)
        {

            // Check if myEntity1 is null
            if (myEntity1 == null)
                throw new ArgumentNullException("myEntity1 must not be null!");

            // Check if myEntity2 is null
            if (myEntity2 == null)
                throw new ArgumentNullException("myEntity2 must not be null!");


            Int32 _LoginComparison = myEntity1.Login.CompareTo(myEntity2.Login);

            if (_LoginComparison < 0)
                return true;

            if (_LoginComparison > 0)
                return false;


            Int32 _RealnameComparison = myEntity1.Realname.CompareTo(myEntity2.Realname);

            if (_RealnameComparison < 0)
                return true;

            if (_RealnameComparison > 0)
                return false;


            Int32 _DescriptionComparison = myEntity1.Description.CompareTo(myEntity2.Description);

            if (_DescriptionComparison < 0)
                return true;

            if (_DescriptionComparison > 0)
                return false;


            if (myEntity1.UUID < myEntity2.UUID)
                return true;

            if (myEntity1.UUID > myEntity2.UUID)
                return false;

            return false;

        }

        #endregion

        #region Operator > (myEntity1, myEntity2)

        public static Boolean operator > (Entity myEntity1, Entity myEntity2)
        {

            // Check if myEntity1 is null
            if (myEntity1 == null)
                throw new ArgumentNullException("myEntity1 must not be null!");

            // Check if myEntity2 is null
            if (myEntity2 == null)
                throw new ArgumentNullException("myEntity2 must not be null!");


            Int32 _LoginComparison = myEntity1.Login.CompareTo(myEntity2.Login);

            if (_LoginComparison > 0)
                return true;

            if (_LoginComparison < 0)
                return false;


            Int32 _RealnameComparison = myEntity1.Realname.CompareTo(myEntity2.Realname);

            if (_RealnameComparison > 0)
                return true;

            if (_RealnameComparison < 0)
                return false;


            Int32 _DescriptionComparison = myEntity1.Description.CompareTo(myEntity2.Description);

            if (_DescriptionComparison > 0)
                return true;

            if (_DescriptionComparison < 0)
                return false;


            if (myEntity1.UUID > myEntity2.UUID)
                return true;

            if (myEntity1.UUID < myEntity2.UUID)
                return false;

            return false;

        }

        #endregion

        #region Operator <= (myEntity1, myEntity2)

        public static Boolean operator <= (Entity myEntity1, Entity myEntity2)
        {
            return !(myEntity1 > myEntity2);
        }

        #endregion

        #region Operator >= (myEntity1, myEntity2)

        public static Boolean operator >= (Entity myEntity1, Entity myEntity2)
        {
            return !(myEntity1 < myEntity2);
        }

        #endregion

        #region GetHashCode()

        public override int GetHashCode()
        {
            return _UUID.GetHashCode() ^ Login.GetHashCode() ^ _Description.GetHashCode() ^ _Status.GetHashCode() ^ _PasswordHash.GetHashCode() ^ _PublicKeyList.GetHashCode();
        }

        #endregion

        #endregion
        
        #region IComparable Member

        public Int32 CompareTo(Object myObject)
        {

            // Check if myObject is null
            if (myObject == null)
                throw new ArgumentNullException("myObject must not be null!");

            // Check if myObject can be casted to an Entity object
            Entity myEntity = myObject as Entity;
            if ( (Object) myEntity == null)
                throw new ArgumentException("myObject is not of type Entity!");

            return CompareTo(myEntity);

        }

        #endregion

        #region IComparable<Entity> Member

        public Int32 CompareTo(Entity myEntity)
        {

            // Check if myEntity is null
            if (myEntity == null)
                throw new ArgumentNullException("myEntity must not be null!");

            if (this < myEntity) return -1;
            if (this > myEntity) return +1;

            return 0;

        }

        #endregion

        #endregion    


        #region IFastSerialize Members

        #region isDirty

        private Boolean _isDirty = false;

        public Boolean isDirty
        {

            get
            {
                return _isDirty;
            }

            set
            {
                _isDirty = value;
            }

        }

        #endregion

        #region ModificationTime

        public DateTime ModificationTime
        {

            get
            {
                throw new NotImplementedException();
            }

        }

        #endregion

        //#region Serialize()

        //public Byte[] Serialize()
        //{

        //    #region Data

        //    SerializationWriter writer;

        //    #endregion

        //    try
        //    {

        //        #region Init SerializationWriter

        //        writer = new SerializationWriter();

        //        #endregion


        //        #region Write Entity Basics

        //        writer.WriteObject(_UUID.ToByteArray());
        //        writer.WriteObject(_Login);
        //        writer.WriteObject(_Realname);
        //        writer.WriteObject(_Description);
        //        writer.WriteObject((UInt16) _Status);

        //        #endregion

        //        #region ContactList

        //        writer.WriteObject((UInt32) _Contacts.Count);
        //        foreach (KeyValuePair<ContactTypes, List<String>> _KeyValuePair in _Contacts)
        //        {
                    
        //            writer.WriteObject( (Byte) _KeyValuePair.Key);
                    
        //            writer.WriteObject((UInt32) _KeyValuePair.Value.Count);
        //            foreach (String _String in _KeyValuePair.Value)
        //                writer.WriteObject(_String);

        //        }

        //        #endregion

        //        #region PasswordHash

        //        writer.WriteObject(_PasswordHash.ToByteArray());

        //        #endregion

        //        #region PublicKeyList

        //        writer.WriteObject((UInt32) _PublicKeyList.Count);
        //        foreach (PublicKey _PublicKey in _PublicKeyList)
        //            writer.WriteObject(_PublicKey.ToByteArray());

        //        #endregion

        //        #region Memberships

        //        writer.WriteObject((UInt64) _Membership.Count);
        //        foreach (EntityUUID _MembershipUUID in _Membership)
        //            writer.WriteObject(_MembershipUUID.ToByteArray());

        //        #endregion

        //        isDirty = false;

        //        return writer.ToArray();

        //    }

        //    catch (SerializationException e)
        //    {
        //        throw new SerializationException(e.Message);
        //    }


        //}

        //#endregion

        //#region Deserialize(mySerializedData)

        //public void Deserialize(Byte[] mySerializedData)
        //{

        //    #region Data

        //    SerializationReader reader;

        //    #endregion

        //    #region Init reader

        //    reader = new SerializationReader(mySerializedData);

        //    #endregion

        //    try
        //    {

        //        #region Read Entity Basics

        //        _UUID           = new EntityUUID( (Byte[]) reader.ReadObject());
        //        _Login          = (String) reader.ReadObject();
        //        _Realname       = (String) reader.ReadObject();
        //        _Description    = (String) reader.ReadObject();
        //        _Status         = (EntityStatus) reader.ReadObject();

        //        #endregion

        //        #region Read ContactList

        //        UInt32 _NumberOfContacts = (UInt32) reader.ReadObject();

        //        for (UInt32 i=0; i<_NumberOfContacts; i++)
        //        {
                    
        //            ContactTypes _ContactTypes = (ContactTypes) reader.ReadObject();

        //            List<String> _StringList = new List<String>();

        //            UInt32 _ListEntries = (UInt32) reader.ReadObject();

        //            for (UInt32 j=0; j<_NumberOfContacts; j++)
        //                _StringList.Add( (String) reader.ReadObject());

        //            _Contacts.Add(_ContactTypes, _StringList);
                        
        //        }

        //        #endregion

        //        #region PasswordHash

        //        Byte[] PasswordHashBytes = (Byte[])reader.ReadObject();
        //        if (PasswordHashBytes.Length > 0)
        //            _PasswordHash = new PasswordHash(PasswordHashBytes);
        //        else
        //            _PasswordHash = new PasswordHash("");

        //        #endregion

        //        #region PublicKeyList

        //        UInt32 _NumberOfPublicKeys = (UInt32) reader.ReadObject();

        //        for (UInt32 i=0; i<_NumberOfPublicKeys; i++)
        //            _PublicKeyList.Add(new PublicKey( (Byte[]) reader.ReadObject()));

        //        #endregion

        //        #region Memberships

        //        UInt64 _NumberOfMemberships = (UInt64) reader.ReadObject();

        //        for (UInt64 i=0; i<_NumberOfMemberships; i++)
        //            _Membership.Add(new EntityUUID( (Byte[]) reader.ReadObject()));

        //        #endregion

        //    }

        //    catch (Exception e)
        //    {
        //        throw new GraphFSException_EntityCouldNotBeDeserialized("Entity could not be deserialized!\n\n" + e);
        //    }

        //}

        //#endregion

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            try
            {

                #region Write Entity Basics

                _UUID.Serialize(ref mySerializationWriter);
                mySerializationWriter.WriteString(_Login);
                mySerializationWriter.WriteString(_Realname);
                mySerializationWriter.WriteString(_Description);
                mySerializationWriter.WriteByte((Byte)_Status);

                #endregion

                #region ContactList

                mySerializationWriter.WriteUInt32((UInt32)_Contacts.Count);
                foreach (KeyValuePair<ContactTypes, List<String>> _KeyValuePair in _Contacts)
                {

                    mySerializationWriter.WriteByte((Byte)_KeyValuePair.Key);

                    mySerializationWriter.WriteUInt32((UInt32)_KeyValuePair.Value.Count);
                    foreach (String _String in _KeyValuePair.Value)
                        mySerializationWriter.WriteString(_String);

                }

                #endregion

                #region PasswordHash

                _PasswordHash.Serialize(ref mySerializationWriter);

                #endregion

                #region PublicKeyList

                mySerializationWriter.WriteUInt32((UInt32)_PublicKeyList.Count);
                foreach (PublicKey _PublicKey in _PublicKeyList)
                    _PublicKey.Serialize(ref mySerializationWriter);

                #endregion

                #region Memberships

                mySerializationWriter.WriteUInt32((UInt32)_Membership.Count);
                foreach (EntityUUID _MembershipUUID in _Membership)
                    _MembershipUUID.Serialize(ref mySerializationWriter);

                #endregion

                isDirty = false;

            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }

        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            try
            {

                #region Read Entity Basics
                                
                _UUID.Deserialize(ref mySerializationReader);
                _Login          = mySerializationReader.ReadString();
                _Realname       = mySerializationReader.ReadString();
                _Description    = mySerializationReader.ReadString();
                _Status         = (EntityStatus)mySerializationReader.ReadOptimizedByte();

                #endregion

                #region Read ContactList

                UInt32 _NumberOfContacts = mySerializationReader.ReadUInt32();

                for (UInt32 i=0; i<_NumberOfContacts; i++)
                {

                    ContactTypes _ContactTypes = (ContactTypes)mySerializationReader.ReadOptimizedByte();

                    List<String> _StringList = new List<String>();

                    UInt32 _ListEntries = mySerializationReader.ReadUInt32();

                    for (UInt32 j=0; j<_NumberOfContacts; j++)
                        _StringList.Add( mySerializationReader.ReadString());

                    _Contacts.Add(_ContactTypes, _StringList);
                        
                }

                #endregion

                #region PasswordHash
                
                _PasswordHash = new PasswordHash();
                _PasswordHash.Deserialize(ref mySerializationReader);

                #endregion

                #region PublicKeyList

                UInt32 _NumberOfPublicKeys = mySerializationReader.ReadUInt32();

                for (UInt32 i=0; i<_NumberOfPublicKeys; i++)
                    _PublicKeyList.Add(new PublicKey(mySerializationReader.ReadByteArray()));

                #endregion

                #region Memberships

                UInt64 _NumberOfMemberships = mySerializationReader.ReadUInt64();

                for (UInt64 i=0; i<_NumberOfMemberships; i++)
                    _Membership.Add(new EntityUUID(mySerializationReader.ReadByteArray()));

                #endregion

            }

            catch (Exception e)
            {
                throw new GraphFSException_EntityCouldNotBeDeserialized("Entity could not be deserialized!\n\n" + e);
            }
        }

        #endregion

        #region IFastSerializationTypeSurrogate Members

        public bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public void Serialize(SerializationWriter mySerializationWriter, object value)
        {
            Entity thisObject = (Entity)value;
            
            try
            {

                #region Write Entity Basics

                thisObject._UUID.Serialize(ref mySerializationWriter);
                mySerializationWriter.WriteString(thisObject._Login);
                mySerializationWriter.WriteString(thisObject._Realname);
                mySerializationWriter.WriteString(thisObject._Description);
                mySerializationWriter.WriteByte((Byte)thisObject._Status);

                #endregion

                #region ContactList

                mySerializationWriter.WriteUInt32((UInt32)thisObject._Contacts.Count);
                foreach (KeyValuePair<ContactTypes, List<String>> _KeyValuePair in thisObject._Contacts)
                {

                    mySerializationWriter.WriteByte((Byte)_KeyValuePair.Key);

                    mySerializationWriter.WriteUInt32((UInt32)_KeyValuePair.Value.Count);
                    foreach (String _String in _KeyValuePair.Value)
                        mySerializationWriter.WriteObject(_String);

                }

                #endregion

                #region PasswordHash

                thisObject._PasswordHash.Serialize(ref mySerializationWriter);

                #endregion

                #region PublicKeyList

                mySerializationWriter.WriteUInt32((UInt32)thisObject._PublicKeyList.Count);

                foreach (PublicKey _PublicKey in thisObject._PublicKeyList)
                    _PublicKey.Serialize(ref mySerializationWriter);

                #endregion

                #region Memberships

                mySerializationWriter.WriteUInt64((UInt64)thisObject._Membership.Count);

                foreach (EntityUUID _MembershipUUID in thisObject._Membership)
                    _MembershipUUID.Serialize(ref mySerializationWriter);

                #endregion

                thisObject.isDirty = false;

            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }
    
        }

        public object Deserialize(SerializationReader mySerializationReader, Type type)
        {
            Entity thisObject = (Entity)Activator.CreateInstance(type);
            
            try
            {

                #region Read Entity Basics

                thisObject._UUID.Deserialize(ref mySerializationReader);
                thisObject._Login = mySerializationReader.ReadString();
                thisObject._Realname = mySerializationReader.ReadString();
                thisObject._Description = mySerializationReader.ReadString();
                thisObject._Status = (EntityStatus)mySerializationReader.ReadOptimizedByte();

                #endregion

                #region Read ContactList

                UInt32 _NumberOfContacts = mySerializationReader.ReadUInt32();

                for (UInt32 i = 0; i < _NumberOfContacts; i++)
                {

                    ContactTypes _ContactTypes = (ContactTypes)mySerializationReader.ReadOptimizedByte();

                    List<String> _StringList = new List<String>();

                    UInt32 _ListEntries = (UInt32)mySerializationReader.ReadObject();

                    for (UInt32 j = 0; j < _NumberOfContacts; j++)
                        _StringList.Add(mySerializationReader.ReadString());

                    thisObject._Contacts.Add(_ContactTypes, _StringList);

                }

                #endregion

                #region PasswordHash

                thisObject._PasswordHash = new PasswordHash();
                thisObject._PasswordHash.Deserialize(ref mySerializationReader);

                #endregion

                #region PublicKeyList

                UInt32 _NumberOfPublicKeys = (UInt32)mySerializationReader.ReadUInt32();

                for (UInt32 i = 0; i < _NumberOfPublicKeys; i++)
                    thisObject._PublicKeyList.Add(new PublicKey(mySerializationReader.ReadByteArray()));

                #endregion

                #region Memberships

                UInt64 _NumberOfMemberships = mySerializationReader.ReadUInt64();

                for (UInt64 i = 0; i < _NumberOfMemberships; i++)
                    thisObject._Membership.Add(new EntityUUID(mySerializationReader.ReadByteArray()));

                #endregion

            }

            catch (Exception e)
            {
                throw new GraphFSException_EntityCouldNotBeDeserialized("Entity could not be deserialized!\n\n" + e);
            }

            return thisObject;
        }

        #endregion

        #region IEstimable

        public ulong GetEstimatedSize()
        {
            return EstimatedSizeConstants.UndefinedObjectSize;
        }

        #endregion
    }

}
