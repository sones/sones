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

using sones.GraphDBInterface.Result;
using sones.GraphDBInterface.Transactions;

using sones.GraphDS.API.CSharp;

using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDSClient
{

    public class GraphDSClient1 : AGraphDSSharp
    {

        #region Data

        private String _RestURL = null;
        private String username = null;
        private String password = null;

        #endregion

        #region Properties

        #endregion


        #region Constructor(s)

        #region GraphDSClient1(myRestURI, myDatabase, myUsername, myPassword)

        public GraphDSClient1(Uri myRestURI, String myDatabase, String myUsername, String myPassword)
        {
            
            _RestURL = String.Format("{0}gql?", myRestURI.ToString());
            username = myUsername;
            password = myPassword;

        }

        #endregion

        #endregion

        #region Query(myQuery, myAction = null, mySuccessAction = null, myPartialSuccessAction = null, myFailureAction = null)

        public override QueryResult Query(String myQuery, Action<QueryResult> myAction = null, Action<QueryResult> mySuccessAction = null, Action<QueryResult> myPartialSuccessAction = null, Action<QueryResult> myFailureAction = null)
        {

            #region Init

            String      queryResultAsXMLString  = null;
            XDocument   queryResultAsXML        = null;

            var warnings                     = new List<IWarning>();
            var errors                       = new List<IError>();
            String query                     = String.Empty;
            ResultType queryResultType       = ResultType.Failed;
            UInt64 duration                  = 0;
            SelectionResultSet srs           = null;

            QueryResult result               = null;

            #endregion

            #region catch communication errors and build xml

            try
            {
                queryResultAsXMLString = QueryXml_private(myQuery);

                //get a valid xml document
                queryResultAsXML = XDocument.Parse(queryResultAsXMLString, LoadOptions.None);
            }
            catch (Exception e)
            {
                return new QueryResult(new Error_Unspecified(e.GetType().Name, e.Message));
            }

            #endregion

            #region Get QueryResult infos

            //get the queryResult
            var queryResultElement = queryResultAsXML.Element("sones").Element("GraphDB").Element("queryresult");

            //get the query ("From User Select *")
            query = queryResultElement.Element("query").Value;

            //get the ResultType of the query
            queryResultType = (ResultType)Enum.Parse(typeof(ResultType), queryResultElement.Element("result").Value);

            //get the duration
            duration = UInt64.Parse(queryResultElement.Element("duration").Value);

            // Warnings
            var warningsFromXML = queryResultElement.Element("warnings").Elements("warning");
            if (warningsFromXML != null)
            {
                foreach (var aWarningXML in warningsFromXML)
                {
                    warnings.Add(GenerateWarningFromXML(aWarningXML));
                }
            }

            // Errors
            var errorsFromXML = queryResultElement.Element("errors").Elements("error");
            if (errorsFromXML != null)
            {
                foreach (var aErrorXML in errorsFromXML)
                {
                    errors.Add(GenerateErrorFromXML(aErrorXML));
                }
            }

            #endregion

            #region build queryResult

            switch (queryResultType)
            {
                case ResultType.Failed:

                    result = new QueryResult(errors, warnings) { Duration = duration, Query = query };

                    break;

                case ResultType.PartialSuccessful:
                case ResultType.Successful:

                    var resultsFromXML = queryResultElement.Element("results");
                    if (resultsFromXML != null)
                    {
                        srs = GetSelectionResultSetsFromXML(resultsFromXML);
                    }

                    result = new QueryResult(srs, warnings) { Duration = duration, Query = query };

                    break;
                default:
                    break;
            }

            #endregion

            QueryResultAction(result, myAction, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return result;

        }

        public QueryResult QueryXML(String myQuery, Action<QueryResult> myAction = null, Action<QueryResult> mySuccessAction = null, Action<QueryResult> myPartialSuccessAction = null, Action<QueryResult> myFailureAction = null)
        {

            var readout = new DBObjectReadout();

            readout.Attributes.Add("query", QueryXml_private(myQuery));

            var _QueryResult = new QueryResult(new SelectionResultSet(new List<DBObjectReadout>() { readout }));

            QueryResultAction(_QueryResult, myAction, mySuccessAction, myPartialSuccessAction, myFailureAction);

            return _QueryResult;

        }

        private String QueryXml_private(String myQuery)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_RestURL + HttpUtility.UrlEncode(myQuery));
            
            #region Add credentials

            String usernamePassword = username + ":" + password;
            if (!String.IsNullOrEmpty(usernamePassword))
            {
                CredentialCache ccache = new CredentialCache();
                ccache.Add(request.RequestUri, "Basic", new NetworkCredential(username, password));
                request.Credentials = ccache;
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(usernamePassword)));
            }
            
            #endregion

            request.Accept = "application/xml";

            var stream = new StreamReader(request.GetResponse().GetResponseStream());
            var result = stream.ReadToEnd();

            return result; // .FromBase64();

        }

        #endregion


        public override SelectToObjectGraph QuerySelect(String myQuery)
        {
            return new SelectToObjectGraph(QueryXml_private(myQuery));
        }


        public override FSTransaction BeginFSTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? myCreated = null)
        {
            throw new NotImplementedException();
        }

        public override DBTransaction BeginTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? myCreated = null)
        {
            throw new NotImplementedException();
        }



        #region IGraphFSSession

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


        #region private methods

        /// <summary>
        /// Extracts Selections from XML
        /// </summary>
        /// <param name="resultsFromXML">XElements which contain a SelectionResultSet representation.</param>
        /// <returns>A list of SelectionResultSets</returns>
        private SelectionResultSet GetSelectionResultSetsFromXML(XElement resultFromXML)
        {
            return GenerateSelectionResultSet(resultFromXML);
        }

        /// <summary>
        /// Generates a single SelectionResultSet
        /// </summary>
        /// <param name="aResultElement">XElement which represents a SelectionResultSet</param>
        /// <returns>A SelectionResultSet</returns>
        private SelectionResultSet GenerateSelectionResultSet(XElement aResultElement)
        {
            return new SelectionResultSet(GenerateReadouts(aResultElement.Elements("DBObject")));
        }

        /// <summary>
        /// Generates an IEnumerable of DBObjectReadouts corresponding to their XML representation
        /// </summary>
        /// <param name="myDBObjectReadoutsXML">An IEnumerable of XElements which represent DBObjectReadouts</param>
        /// <returns>An IEnumerable of DBObjectReadout</returns>
        private IEnumerable<DBObjectReadout> GenerateReadouts(IEnumerable<XElement> myDBObjectReadoutsXML)
        {
            if (myDBObjectReadoutsXML == null)
            {
                yield break;
            }

            foreach (var aDBObjectReadoutXML in myDBObjectReadoutsXML)
            {
                yield return GenerateDBObjectReadoutFromXML(aDBObjectReadoutXML);
            }

            yield break;
        }

        /// <summary>
        /// Generates a single DBObjectReadout based on its XML representation
        /// </summary>
        /// <param name="aDBObjectReadoutXML">The XML representation of a DBObjectReadout</param>
        /// <returns>A DBObjectReadout</returns>
        private DBObjectReadout GenerateDBObjectReadoutFromXML(XElement aDBObjectReadoutXML)
        {
            Dictionary<String, Object> payload = new Dictionary<String, Object>();

            //Get all non reference attributes
            foreach (var aAttribute in aDBObjectReadoutXML.Elements("attribute"))
            {
                String nameOfAttribute = aAttribute.Attribute("name").Value;
                String typeOfAttribute = aAttribute.Attribute("type").Value;
                String valueOfAttribute = aAttribute.Value;

                payload.Add(nameOfAttribute, GetAttribute(typeOfAttribute, valueOfAttribute));
            }

            //Get all edges
            foreach (var aAttribute in aDBObjectReadoutXML.Elements("edge"))
            {
                payload.Add(aAttribute.Attribute("name").Value, new Edge(GenerateEdgeContent(aAttribute.Element("hyperedgelabel"), aAttribute.Elements("DBObject")), aAttribute.Attribute("type").Value));
            }

            //generate DBObjectStream

            var edgeLabel = aDBObjectReadoutXML.Element("edgelabel");

            if (edgeLabel != null)
            {
                //there's an edgelabel... use it.

                var edgeIdentifierElement = edgeLabel.Element("attribute").Attribute("name");

                if (edgeIdentifierElement != null)
                {

                    switch (edgeIdentifierElement.Value)
                    {
                        case "weight":

                            return new DBWeightedObjectReadout(payload, edgeLabel.Value, edgeLabel.Element("attribute").Attribute("type").Value);

                        case "group":

                            return new DBObjectReadoutGroup(payload, GenerateReadouts(edgeLabel.Element("attribute").Elements("DBObject")));

                        default:

                            //unknown label... use default
                            return new DBObjectReadout(payload);

                    }
                }
                else
                {
                    //TODO: mhhh, maybe it would be better to throw an exception 

                    return new DBObjectReadout(payload);
                }
            }
            else
            {
                //no edgelabel... standart DBObjectReadout

                return new DBObjectReadout(payload);
            }
        }

        private object GetAttribute(String typeOfAttribute, String valueOfAttribute)
        {
            if (typeOfAttribute == "Double")
            {
                return Convert.ToDouble(valueOfAttribute);
            }
            else if (typeOfAttribute == "Int64")
            {
                return Convert.ToInt64(valueOfAttribute);
            }
            else if (typeOfAttribute == "Int32")
            {
                return Convert.ToInt32(valueOfAttribute);
            }
            else if (typeOfAttribute == "UInt64")
            {
                return Convert.ToUInt64(valueOfAttribute);

            }
            else if (typeOfAttribute == "DateTime")
            {
                return Convert.ToDateTime(valueOfAttribute);

            }
            else if (typeOfAttribute == "Boolean")
            {
                return Convert.ToBoolean(valueOfAttribute);

            }
            else if (typeOfAttribute == "String")
            {
                return valueOfAttribute;
            }
            else if (typeOfAttribute == "ObjectUUID")
            {
                return new ObjectUUID(valueOfAttribute);
            }
            else if (typeOfAttribute == "ObjectRevisionID")
            {
                return new ObjectRevisionID(valueOfAttribute);
            }
            else
            {
                return valueOfAttribute;
            }
        }

        /// <summary>
        /// Genereate DBObjectReadouts corresponding to a hyperEdgeLabel and their XML representation
        /// </summary>
        /// <param name="hyperEdgeLabel">future feature</param>
        /// <param name="myDBObjectReadoutsXML">XML representation of DBObjectReadouts</param>
        /// <returns>An IEnumerable of DBObjectReadout</returns>
        private IEnumerable<DBObjectReadout> GenerateEdgeContent(XElement hyperEdgeLabel, IEnumerable<XElement> myDBObjectReadoutsXML)
        {
            //TODO: process hyperEdgeLabel

            foreach (var aDBReadoutXML in myDBObjectReadoutsXML)
            {
                yield return GenerateDBObjectReadoutFromXML(aDBReadoutXML);
            }

            yield break;
        }

        /// <summary>
        /// Generates a GeneralError that can be integrated into QueryResult
        /// </summary>
        /// <param name="aErrorXML">The XML representation of a generalized DBError</param>
        /// <returns>A GeneralError</returns>
        private Error_Unspecified GenerateErrorFromXML(XElement aErrorXML)
        {
            return new Error_Unspecified(aErrorXML.Attribute("code").Value, aErrorXML.Value);
        }

        /// <summary>
        /// Generate a GraphDBWarning
        /// </summary>
        /// <param name="aWarningXML">The XML representation of a GraphDBWarning</param>
        /// <returns>A GraphDBWarning</returns>
        private Warning_Unspecified GenerateWarningFromXML(XElement aWarningXML)
        {
            return new Warning_Unspecified(aWarningXML.Attribute("code").Value, aWarningXML.Value);
        }

        #endregion
    
    }

}
