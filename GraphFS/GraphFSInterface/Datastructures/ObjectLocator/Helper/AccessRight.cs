///* GraphFS - AccessRight
// * (c) Achim Friedland, 2008 - 2009
// * 
// * This represents the users rights and the associated encryption
// * parameters of an object stream.
// * 
// * Lead programmer:
// *      Achim Friedland
// * 
// * */

//#region Usings

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Text;
//using System.Diagnostics;
//using System.IO;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;


//using sones.Graph.Storage.GraphFS.Objects;

//#endregion

//namespace sones.Graph.Storage.GraphFS.Datastructures
//{

//    /// <summary>
//    /// This represents the users rights and the associated encryption
//    /// parameters of an object stream.
//    /// </summary>

//    

//    public struct AccessRight
//    {

///*
//        Use the PublicKeyList of the User to encrypt the stored
//        key within AccessControlType.        

//        Allow SecretSharing and -Splicing (?) -> 
//        e.g. 4 of 5 Users must decrypt (parts of) the key
//        in order to read the ObjectStreams.
//*/


//        // Properties

//        #region UserID

//        private UInt64 _UserID;

//        public UInt64 UserID
//        {

//            get
//            {
//                return _UserID;
//            }

//            set
//            {
//                _UserID  = value;
//                isDirty  = true;
//            }

//        }

//        #endregion

//        #region AccessFlags

//        private AccessFlagsType _AccessFlags;

//        public AccessFlagsType AccessFlags
//        {

//            get
//            {
//                return _AccessFlags;
//            }

//            set
//            {
//                _AccessFlags  = value;
//                isDirty       = true;
                
//            }

//        }

//        #endregion

//        #region EncryptionParameters

//        private Byte[] _EncryptionParameters;

//        public Byte[] EncryptionParameters
//        {

//            get
//            {
//                return _EncryptionParameters;
//            }

//            set
//            {
//                _EncryptionParameters  = value;
//                isDirty                = true;
                
//            }

//        }

//        #endregion

//        #region isDirty

//        private Boolean _isDirty;

//        public Boolean isDirty
//        {

//            get
//            {
//                return _isDirty;
//            }

//            set
//            {
//                _isDirty = value;
//            }

//        }

//        #endregion



//        // Methods

//        #region addFlag(myAccessFlag)

//        /// <summary>
//        /// Adds the given flag to the access flag
//        /// </summary>
//        /// <param name="myAccessFlag">an access flag</param>
//        public void addFlag(AccessFlagsType myAccessFlag)
//        {

//            _AccessFlags |= myAccessFlag;
//            isDirty       = true;

//        }

//        #endregion

//        #region addFlag(myAccessFlag)

//        /// <summary>
//        /// Removes the given flag from the access flag
//        /// </summary>
//        /// <param name="myAccessFlag">an access flag</param>
//        public void removeFlag(AccessFlagsType myAccessFlag)
//        {

//            _AccessFlags = _AccessFlags &= ~(myAccessFlag);
//            isDirty      = true;

//        }

//        #endregion



//    }

//}
