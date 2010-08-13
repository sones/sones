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


///* PandoraFS - AccessRight
// * Achim Friedland, 2008 - 2009
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
