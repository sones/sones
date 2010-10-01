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

/*
 * GraphDSClient
 * (c) Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;

using sones.GraphFS;
using sones.GraphFS.Caches;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Events;
using sones.GraphFS.InternalObjects;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.GraphFS.Transactions;

using sones.GraphDB.Result;
using sones.GraphDB.Transactions;

using sones.GraphIO;

using sones.GraphDS.API.CSharp;

using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDSClient
{

    public class GraphDSClient1 : AGraphDSSharp
    {

        #region Data

        String _UsernameAndPassword;
        String _AuthorizationHeader;
        readonly CredentialCache _CredentialCache;

        #endregion

        #region Properties

        #region URI

        private Uri     _URI;
        private String  _URIString;

        public Uri URI
        {
            
            get
            {
                return _URI;
            }

            set
            {
                if (value != null)
                {
                    _URI        = value;
                    _URIString  = value.ToString() + "gql?";
                }
            }

        }

        #endregion

        // REST or Thrift or...

        #region TransmissionProtocol

        private IObjectsIO _TransmissionProtocol;

        public IObjectsIO TransmissionProtocol
        {

            get
            {
                return _TransmissionProtocol;
            }

            set
            {
                if (value != null)
                    _TransmissionProtocol = value;
            }

        }

        #endregion

        #region Database

        private String _Database;

        public String Database
        {

            get
            {
                return _Database;
            }

            set
            {
                if (value != null)
                    _Database = value;
            }

        }

        #endregion

        #region Username

        String _Username;

        public String Username
        {

            get
            {
                return _Username;
            }

            set
            {
                if (value != null)
                {
                    _Username = value;
                    SetUsernameAndPassword(_Username, _Password);
                }
            }
        
        }

        #endregion

        #region Password

        String _Password;

        public String Password
        {

            get
            {
                return _Password;
            }

            set
            {
                if (value != null)
                {
                    _Password = value;
                    SetUsernameAndPassword(_Username, _Password);
                }
            }
        
        }

        #endregion

        #endregion


        #region Constructor(s)

        #region GraphDSClient1(myURI, myTransmissionProtocol, myDatabase, myUsername, myPassword)

        /// <summary>
        /// Creates an sones GraphDSClient in order to connect to remote database instances.
        /// </summary>
        /// <param name="myURI">The URI of the remote database server</param>
        /// <param name="myTransmissionProtocol">The transmission protocol (XML, JSON, ...)</param>
        /// <param name="myDatabase">The database to connect</param>
        /// <param name="myUsername">The username for BASIC authentication.</param>
        /// <param name="myPassword">The passwort for BASIC authentication.</param>
        public GraphDSClient1(Uri myURI, IObjectsIO myTransmissionProtocol, String myDatabase, String myUsername, String myPassword)
        {

            #region Initial checks

            if (myURI == null)
                throw new ArgumentNullException("myURI must not be null!");

            if (myTransmissionProtocol == null)
                throw new ArgumentNullException("myTransmissionProtocol must not be null!");

            if (myDatabase == null)
                throw new ArgumentNullException("myDatabase must not be null!");

            if (myUsername == null)
                throw new ArgumentNullException("myUsername must not be null!");

            if (myPassword == null)
                throw new ArgumentNullException("myPassword must not be null!");

            #endregion

            URI                     = myURI;
            TransmissionProtocol    = myTransmissionProtocol;
            _Database               = myDatabase;
            _CredentialCache        = new CredentialCache();

            SetUsernameAndPassword(myUsername, myPassword);

        }

        #endregion

        #endregion


        #region QueryAsString(myQueryString)

        public override Exceptional<String> QueryAsString(String myQueryString)
        {

            #region Data

            HttpWebRequest  _Request                = null;
            String          _QueryResultAsString    = null;

            #endregion

            #region Create WebRequest

            try
            {
                _Request = (HttpWebRequest) WebRequest.Create(_URIString + HttpUtility.UrlEncode(myQueryString));
            }
            catch (Exception e)
            {
                return new Exceptional<String>(new UnspecifiedError(e.GetType().Name, e.Message));
            }

            #endregion

            #region Add Credentials and ContentType to WebRequest

            _Request.Credentials = _CredentialCache;
            _Request.Accept      = TransmissionProtocol.ImportContentType.ToString();
            _Request.Headers.Add("Authorization", _AuthorizationHeader);

            #endregion

            #region Read WebResponse

            try
            {
                _QueryResultAsString = new StreamReader(_Request.GetResponse().GetResponseStream()).ReadToEnd();
            }
            catch (Exception e)
            {
                return new Exceptional<String>(new UnspecifiedError(e.GetType().Name, e.Message));
            }

            #endregion

            return new Exceptional<String>(_QueryResultAsString);

        }

        #endregion

        #region Query(myQueryString, myAction = null, mySuccessAction = null, myPartialSuccessAction = null, myFailureAction = null)

        public override QueryResult Query(String myQueryString, Action<QueryResult> myAction = null, Action<QueryResult> mySuccessAction = null, Action<QueryResult> myPartialSuccessAction = null, Action<QueryResult> myFailureAction = null)
        {

            #region Init

            var _IWarnings                   = new List<IWarning>();
            var _IErrors                     = new List<IError>();
            ResultType queryResultType       = ResultType.Failed;
            UInt64 duration                  = 0;
            IEnumerable<Vertex> srs = null;
            QueryResult _QueryResult         = null;

            #endregion

            #region Catch communication errors and build xml

            var _QueryResultAsStringExceptional = QueryAsString(myQueryString);

            //get a valid xml document
            var queryResultAsXML = XDocument.Parse(_QueryResultAsStringExceptional.Value, LoadOptions.None);

            #endregion

            #region Get QueryResult infos

            //get the queryResult
            var _QueryResultElement = queryResultAsXML.Element("sones").Element("graphdb").Element("queryresult");

            //get the query ("From User Select *")
            var _QueryString = _QueryResultElement.Element("query").Value;

            //get the ResultType of the query
            queryResultType = (ResultType) Enum.Parse(typeof(ResultType), _QueryResultElement.Element("result").Value);

            //get the duration
            duration = UInt64.Parse(_QueryResultElement.Element("duration").Value);

            #endregion

            #region Warnings

            var warningsFromXML = _QueryResultElement.Element("warnings").Elements("warning");
            if (warningsFromXML != null)
            {
                foreach (var aWarningXML in warningsFromXML)
                {
                    _IWarnings.Add(TransmissionProtocol.GenerateUnspecifiedWarning(aWarningXML));
                }
            }

            #endregion

            #region Errors

            var errorsFromXML = _QueryResultElement.Element("errors").Elements("error");
            if (errorsFromXML != null)
            {
                foreach (var aErrorXML in errorsFromXML)
                {
                    _IErrors.Add(TransmissionProtocol.GenerateUnspecifiedError(aErrorXML));
                }
            }

            #endregion

            #region build queryResult

            switch (queryResultType)
            {

                case ResultType.Failed:
                    _QueryResult = new QueryResult(_IErrors, _IWarnings) { Duration = duration, Query = _QueryString };
                    break;

                case ResultType.PartialSuccessful:
                case ResultType.Successful:

                    var resultsFromXML = _QueryResultElement.Element("results");
                    if (resultsFromXML != null)
                    {
                        srs = GenerateVertices(resultsFromXML);
                    }

                    _QueryResult = new QueryResult(srs, _IWarnings) { Duration = duration, Query = _QueryString };

                    break;
                
                default:
                    break;

            }

            #endregion

            QueryResultAction(_QueryResult, myAction, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        #endregion

        #region QuerySelect(myQuery)

        public override SelectToObjectGraph QuerySelect(String myQuery)
        {
            return new SelectToObjectGraph(QueryAsString(myQuery).Value);
        }

        #endregion



        #region IGraphDBSession

        public override DBTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? myCreated = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphFSSession

        public override FSTransaction BeginFSTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? myCreated = null)
        {
            throw new NotImplementedException();
        }

        public override IGraphFS IGraphFS
        {
            get { throw new NotImplementedException(); }
            protected set { throw new NotImplementedException(); }
        }

        public override IGraphFSSession CreateNewSession(String myUsername)
        {
            throw new NotImplementedException();
        }

        public override Boolean IsMounted
        {
            get { throw new NotImplementedException(); }
        }

        public override Boolean IsPersistent
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<Object> TraverseChildFSs(Func<IGraphFS, UInt64, IEnumerable<Object>> myFunc, UInt64 myDepth)
        {
            throw new NotImplementedException();
        }

        public override FileSystemUUID GetFileSystemUUID()
        {
            throw new NotImplementedException();
        }

        public override FileSystemUUID GetFileSystemUUID(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FileSystemUUID> GetFileSystemUUIDs(UInt64 myDepth)
        {
            throw new NotImplementedException();
        }

        public override String GetFileSystemDescription()
        {
            throw new NotImplementedException();
        }

        public override String GetFileSystemDescription(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<String> GetFileSystemDescriptions(UInt64 myDepth)
        {
            throw new NotImplementedException();
        }

        public override Exceptional WipeFileSystem()
        {
            throw new NotImplementedException();
        }

        public override void SetFileSystemDescription(String myFileSystemDescription)
        {
            throw new NotImplementedException();
        }

        public override void SetFileSystemDescription(ObjectLocation myObjectLocation, String myFileSystemDescription)
        {
            throw new NotImplementedException();
        }

        public override UInt64 GetNumberOfBytes()
        {
            throw new NotImplementedException();
        }

        public override UInt64 GetNumberOfBytes(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<UInt64> GetNumberOfBytes(Boolean myRecursiveOperation)
        {
            throw new NotImplementedException();
        }

        public override UInt64 GetNumberOfFreeBytes()
        {
            throw new NotImplementedException();
        }

        public override UInt64 GetNumberOfFreeBytes(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<UInt64> GetNumberOfFreeBytes(Boolean myRecursiveOperation)
        {
            throw new NotImplementedException();
        }

        public override AccessModeTypes GetAccessMode()
        {
            throw new NotImplementedException();
        }

        public override AccessModeTypes GetAccessMode(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<AccessModeTypes> GetAccessModes(Boolean myRecursiveOperation)
        {
            throw new NotImplementedException();
        }

        public override IGraphFS ParentFileSystem
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override IEnumerable<ObjectLocation> GetChildFileSystemMountpoints(Boolean myRecursiveOperation)
        {
            throw new NotImplementedException();
        }

        public override IGraphFS GetChildFileSystem(ObjectLocation myObjectLocation, Boolean myRecursive)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<ObjectCacheSettings> GetObjectCacheSettings()
        {
            throw new NotImplementedException();
        }

        public override Exceptional<ObjectCacheSettings> GetObjectCacheSettings(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional SetObjectCacheSettings(ObjectCacheSettings myObjectCacheSettings)
        {
            throw new NotImplementedException();
        }

        public override Exceptional SetObjectCacheSettings(ObjectLocation myObjectLocation, ObjectCacheSettings myObjectCacheSettings)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<FileSystemUUID> MakeFileSystem(String myDescription, UInt64 myNumberOfBytes, Boolean myOverwriteExistingFileSystem, Action<double> myAction)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<UInt64> GrowFileSystem(UInt64 myNumberOfBytesToAdd)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<UInt64> ShrinkFileSystem(UInt64 myNumberOfBytesToRemove)
        {
            throw new NotImplementedException();
        }

        public override Exceptional MountFileSystem(AccessModeTypes myFSAccessMode)
        {
            throw new NotImplementedException();
        }

        public override Exceptional MountFileSystem(ObjectLocation myMountPoint, IGraphFSSession myIGraphFSSession, AccessModeTypes myFSAccessMode)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemountFileSystem(AccessModeTypes myFSAccessMode)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemountFileSystem(ObjectLocation myMountPoint, AccessModeTypes myFSAccessMode)
        {
            throw new NotImplementedException();
        }

        public override Exceptional UnmountFileSystem()
        {
            throw new NotImplementedException();
        }

        public override Exceptional UnmountFileSystem(ObjectLocation myMountPoint)
        {
            throw new NotImplementedException();
        }

        public override Exceptional UnmountAllFileSystems()
        {
            throw new NotImplementedException();
        }

        public override Exceptional ChangeRootDirectory(String myChangeRootPrefix)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<INode> GetINode(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<ObjectLocator> GetObjectLocator(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional LockFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevisionID, ObjectLocks myObjectLock, ObjectLockTypes myObjectLockType, UInt64 myLockingTime)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<PT> GetOrCreateFSObject<PT>(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<PT> GetOrCreateFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures = false)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<PT> GetOrCreateFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, Func<PT> myFunc, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures = false)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<PT> GetFSObject<PT>(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<PT> GetFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures = false)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<PT> GetFSObject<PT>(ObjectLocation myObjectLocation, String myObjectStream, Func<PT> myFunc, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null, UInt64 myObjectCopy = 0, Boolean myIgnoreIntegrityCheckFailures = false)
        {
            throw new NotImplementedException();
        }

        public override Exceptional StoreFSObject(AFSObject myAGraphObject, Boolean myAllowOverwritting)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<Trinary> ObjectExists(ObjectLocation myObjectLocatio)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<Trinary> ObjectStreamExists(ObjectLocation myObjectLocation, String myObjectStream)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<Trinary> ObjectEditionExists(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<Trinary> ObjectRevisionExists(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<String>> GetObjectStreams(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<String>> GetObjectEditions(ObjectLocation myObjectLocation, String myObjectStream)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<ObjectRevisionID>> GetObjectRevisionIDs(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RenameFSObject(ObjectLocation myObjectLocation, String myNewObjectName)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemoveFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public override Exceptional EraseFSObject(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition = FSConstants.DefaultEdition, ObjectRevisionID myObjectRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public override Exceptional AddSymlink(ObjectLocation myObjectLocation, ObjectLocation myTargetLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional AddSymlink(ObjectLocation myObjectLocation, AFSObject myTargetAFSObject)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<Trinary> isSymlink(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<ObjectLocation> GetSymlink(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemoveSymlink(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IDirectoryObject> CreateDirectoryObject(ObjectLocation myObjectLocation, UInt64 myBlocksize = 0, Boolean myRecursive = false)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<Trinary> isIDirectoryObject(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<String>> GetDirectoryListing(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<String>> GetDirectoryListing(ObjectLocation myObjectLocation, Func<KeyValuePair<String, GraphFS.InternalObjects.DirectoryEntry>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<String>> GetFilteredDirectoryListing(ObjectLocation myObjectLocation, String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams, String[] mySize, String[] myCreationTime, String[] myLastModificationTime, String[] myLastAccessTime, String[] myDeletionTime)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<DirectoryEntryInformation>> GetExtendedDirectoryListing(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<DirectoryEntryInformation>> GetFilteredExtendedDirectoryListing(ObjectLocation myObjectLocation, String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams, String[] mySize, String[] myCreationTime, String[] myLastModificationTime, String[] myLastAccessTime, String[] myDeletionTime)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemoveDirectoryObject(ObjectLocation myObjectLocation, Boolean removeRecursive)
        {
            throw new NotImplementedException();
        }

        public override Exceptional EraseDirectoryObject(ObjectLocation myObjectLocation, Boolean eradeRecursive)
        {
            throw new NotImplementedException();
        }

        public override Exceptional SetMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue, Lib.DataStructures.Indices.IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public override Exceptional SetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, IEnumerable<KeyValuePair<String, TValue>> myMetadata, Lib.DataStructures.Indices.IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<Trinary> MetadatumExists<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<Trinary> MetadataExists<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<TValue>> GetMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myMinKey, String myMaxKey)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<KeyValuePair<String, TValue>>> GetMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, Func<KeyValuePair<String, TValue>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemoveMetadatum<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey, TValue myValue)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, String myKey)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, IEnumerable<KeyValuePair<String, TValue>> myMetadata)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemoveMetadata<TValue>(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, Func<KeyValuePair<String, TValue>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        public override Exceptional SetUserMetadatum(ObjectLocation myObjectLocation, String myKey, Object myObject, Lib.DataStructures.Indices.IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public override Exceptional SetUserMetadata(ObjectLocation myObjectLocation, IEnumerable<KeyValuePair<String, Object>> myUserMetadata, Lib.DataStructures.Indices.IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<Trinary> UserMetadatumExists(ObjectLocation myObjectLocation, String myKey, Object myMetadatum)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<Trinary> UserMetadataExists(ObjectLocation myObjectLocation, String myKey)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<Object>> GetUserMetadatum(ObjectLocation myObjectLocation, String myKey)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<KeyValuePair<String, Object>>> GetUserMetadata(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<KeyValuePair<String, Object>>> GetUserMetadata(ObjectLocation myObjectLocation, String myMinKey, String myMaxKey)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IEnumerable<KeyValuePair<String, Object>>> GetUserMetadata(ObjectLocation myObjectLocation, Func<KeyValuePair<String, Object>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemoveUserMetadatum(ObjectLocation myObjectLocation, String myKey, Object myObject)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemoveUserMetadata(ObjectLocation myObjectLocation, String myKey)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemoveUserMetadata(ObjectLocation myObjectLocation, IEnumerable<KeyValuePair<String, Object>> myMetadata)
        {
            throw new NotImplementedException();
        }

        public override Exceptional RemoveUserMetadata(ObjectLocation myObjectLocation, Func<KeyValuePair<String, Object>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<FileObject> GetFileObject(ObjectLocation myObjectLocation)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<FileObject> GetFileObject(ObjectLocation myObjectLocation, ObjectRevisionID myRevisionID)
        {
            throw new NotImplementedException();
        }

        public override Exceptional StoreFileObject(ObjectLocation myObjectLocation, Byte[] myData, Boolean myAllowToOverwrite = false)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<GraphFS.IGraphFSStream> OpenStream(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevision, UInt64 myObjectCopy)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<GraphFS.IGraphFSStream> OpenStream(ObjectLocation myObjectLocation, String myObjectStream, String myObjectEdition, ObjectRevisionID myObjectRevision, UInt64 myObjectCopy, FileMode myFileMode, FileAccess myFileAccess, FileShare myFileShare, FileOptions myFileOptions, UInt64 myBufferSize)
        {
            throw new NotImplementedException();
        }

        public override String Implementation
        {
            get { throw new NotImplementedException(); }
        }

        public override Exceptional StoreFileObject(ObjectLocation myObjectLocation, String myData, Boolean myAllowToOverwrite = false)
        {
            throw new NotImplementedException();
        }


        #region File system event

        public override event GraphFSEventHandlers.OnLoadEventHandler OnLoad;

        public override event GraphFSEventHandlers.OnLoadedEventHandler OnLoaded;

        public override event GraphFSEventHandlers.OnLoadedAsyncEventHandler OnLoadedAsync;


        public override event GraphFSEventHandlers.OnSaveEventHandler OnSave;

        public override event GraphFSEventHandlers.OnSavedEventHandler OnSaved;

        public override event GraphFSEventHandlers.OnSavedAsyncEventHandler OnSavedAsync;


        public override event GraphFSEventHandlers.OnRemoveEventHandler OnRemove;

        public override event GraphFSEventHandlers.OnRemovedEventHandler OnRemoved;

        public override event GraphFSEventHandlers.OnRemovedAsyncEventHandler OnRemovedAsync;


        public override event GraphFSEventHandlers.OnTransactionStartEventHandler OnTransactionStart;

        public override event GraphFSEventHandlers.OnTransactionStartedEventHandler OnTransactionStarted;

        public override event GraphFSEventHandlers.OnTransactionStartedAsyncEventHandler OnTransactionStartedAsync;


        public override event GraphFSEventHandlers.OnTransactionCommitEventHandler OnTransactionCommit;

        public override event GraphFSEventHandlers.OnTransactionCommittedEventHandler OnTransactionCommitted;

        public override event GraphFSEventHandlers.OnTransactionCommittedAsyncEventHandler OnTransactionCommittedAsync;


        public override event GraphFSEventHandlers.OnTransactionRollbackEventHandler OnTransactionRollback;

        public override event GraphFSEventHandlers.OnTransactionRollbackedEventHandler OnTransactionRollbacked;

        public override event GraphFSEventHandlers.OnTransactionRollbackedAsyncEventHandler OnTransactionRollbackedAsync;

        #endregion

        #endregion



        #region (private) GenerateVertices(myVerticesXML)

        /// <summary>
        /// Generates an IEnumerable of Vertices corresponding to their XML representation
        /// </summary>
        /// <param name="myVerticesXML">An IEnumerable of XElements which represent Vertices</param>
        /// <returns>An IEnumerable of Vertex</returns>
        private IEnumerable<Vertex> GenerateVertices(XElement myVerticesXML)
        {

            var _VerticesXML = myVerticesXML.Elements("vertex");

            if (_VerticesXML == null)
            {
                yield break;
            }

            foreach (var _VertexXML in _VerticesXML)
            {
                yield return GenerateVertexFromXML(_VertexXML);
            }

            yield break;

        }

        #endregion

        #region (private) GenerateVertexFromXML(myVertexXML)

        /// <summary>
        /// Generates a single Vertex based on its XML representation
        /// </summary>
        /// <param name="myVertexXML">The XML representation of a Vertex</param>
        /// <returns>A Vertex</returns>
        private Vertex GenerateVertexFromXML(XElement myVertexXML)
        {

            String AttributeName, AttributeType, AttributeValue;

            var payload = new Dictionary<String, Object>();


            //Get all non reference attributes
            foreach (var _Attribute in myVertexXML.Elements("attribute"))
            {
                
                AttributeName  = _Attribute.Attribute("name").Value;
                AttributeType  = _Attribute.Attribute("type").Value;
                AttributeValue = _Attribute.Value;

                payload.Add(AttributeName, ParseAttribute(AttributeType, AttributeValue));

            }

            //Get all edges
            foreach (var _Edge in myVertexXML.Elements("edge"))
            {
                payload.Add(_Edge.Attribute("name").Value, new Edge(null,
                    GenerateEdgeContent(_Edge.Element("hyperedgelabel"), _Edge.Elements("vertex")))
                    { EdgeTypeName = _Edge.Attribute("type").Value });
            }


            //generate DBObjectStream
            var edgeLabel = myVertexXML.Element("edgelabel");

            if (edgeLabel != null)
            {
                //there's an edgelabel... use it.

                var edgeIdentifierElement = edgeLabel.Element("attribute").Attribute("name");

                if (edgeIdentifierElement != null)
                {

                    switch (edgeIdentifierElement.Value)
                    {

                        case "weight":

                            return new Vertex_WeightedEdges(payload, edgeLabel.Value, edgeLabel.Element("attribute").Attribute("type").Value);

                        case "group":

                            return new VertexGroup(payload, GenerateVertices(edgeLabel.Element("attribute")));

                        default:
                            //unknown label... use default
                            return new Vertex(payload);

                    }

                }

                else
                {
                    //TODO: mhhh, maybe it would be better to throw an exception 
                    return new Vertex(payload);
                }

            }

            else
            {
                //no edgelabel... standart DBObjectReadout
                return new Vertex(payload);
            }

        }

        #endregion

        #region (private) GenerateEdgeContent(hyperEdgeLabel, myDBObjectReadoutsXML)

        /// <summary>
        /// Genereate DBObjectReadouts corresponding to a hyperEdgeLabel and their XML representation
        /// </summary>
        /// <param name="hyperEdgeLabel">future feature</param>
        /// <param name="myDBObjectReadoutsXML">XML representation of DBObjectReadouts</param>
        /// <returns>An IEnumerable of DBObjectReadout</returns>
        private IEnumerable<Vertex> GenerateEdgeContent(XElement hyperEdgeLabel, IEnumerable<XElement> myDBObjectReadoutsXML)
        {

            //TODO: process hyperEdgeLabel

            foreach (var aDBReadoutXML in myDBObjectReadoutsXML)
            {
                yield return GenerateVertexFromXML(aDBReadoutXML);
            }

            yield break;

        }

        #endregion


        #region Private Helpers

        #region (private) ParseAttribute(myAttributeType, myAttributeValue)

        private Object ParseAttribute(String myAttributeType, String myAttributeValue)
        {

            switch (myAttributeType)
            {

                case "Double"           : return Convert.ToDouble(myAttributeValue);
                case "Int64"            : return Convert.ToInt64(myAttributeValue);
                case "Int32"            : return Convert.ToInt32(myAttributeValue);
                case "UInt64"           : return Convert.ToUInt64(myAttributeValue);
                case "DateTime"         : return Convert.ToDateTime(myAttributeValue);
                case "Boolean"          : return Convert.ToBoolean(myAttributeValue);

                case "ObjectUUID"       : return new ObjectUUID(myAttributeValue);
                case "ObjectRevisionID" : return new ObjectRevisionID(myAttributeValue);
                
                // String and all other...
                default : return myAttributeValue;

            }

        }

        #endregion

        #region (private) SetUsernameAndPassword(myUsername, myPassword)

        public void SetUsernameAndPassword(String myUsername, String myPassword)
        {

            _Username            = myUsername;
            _Password            = myPassword;            
            _UsernameAndPassword = _Username + ":" + _Password;

            _CredentialCache.Remove(_URI, "Basic");
            _CredentialCache.Add(_URI, "Basic", new NetworkCredential(_Username, _Password));

            _AuthorizationHeader = "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(_UsernameAndPassword));

        }

        #endregion

        #endregion


    }

}
