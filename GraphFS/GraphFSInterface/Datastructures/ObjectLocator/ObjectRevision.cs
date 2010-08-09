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


/* 
 * GraphFSInterface - ObjectRevision
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using sones.Lib.Serializer;
using sones.Lib.DataStructures;
using sones.StorageEngines;
using sones.GraphFS.Objects;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.InternalObjects;
using System.Diagnostics;

#endregion

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    /// An object copy is part of the object locator describing
    /// multiple copies of an stream of data.
    /// </summary>

    public class ObjectRevision : IEnumerable<ObjectDatastream>, IDirectoryListing
    {


        #region Data

        private List<ObjectDatastream> _ObjectDatastreams;

        #endregion

        #region Properties

        #region RevisionID

        private ObjectRevisionID _RevisionID;

        public ObjectRevisionID RevisionID
        {
            get
            {
                return _RevisionID;
            }
        }

        #endregion

        #region ParentRevisionIDs

        private HashSet<ObjectRevisionID> _ParentRevisionIDs;

        /// <summary>
        /// Parent Revisions
        /// </summary>
        public HashSet<ObjectRevisionID> ParentRevisionIDs
        {

            get
            {
                return _ParentRevisionIDs;
            }

            set
            {

                if (value == null)
                    _ParentRevisionIDs = new HashSet<ObjectRevisionID>();

                _ParentRevisionIDs = value;

                isDirty = true;

            }

        }

        #endregion

        #region MinNumberOfCopies

        private UInt64 _MinNumberOfCopies;

        /// <summary>
        /// Minimal number of copies of an object stream. The file system will
        /// guarantee that this number of copies will be on disc at any time.
        /// </summary>
        public UInt64 MinNumberOfCopies
        {

            get
            {
                return _MinNumberOfCopies;
            }

            set
            {
                _MinNumberOfCopies = value;
                isDirty = true;
            }

        }

        #endregion

        #region MaxNumberOfCopies

        private UInt64 _MaxNumberOfCopies;

        /// <summary>
        /// Minimal number of copies of an object stream. The file system will
        /// automatically delete supernumerous copies if this value decreases
        /// or the file system is getting low on free space.
        /// </summary>
        public UInt64 MaxNumberOfCopies
        {

            get
            {
                return _MaxNumberOfCopies;
            }

            set
            {
                _MaxNumberOfCopies = value;
                isDirty = true;
            }

        }

        #endregion

        #region ObjectStream

        private String _ObjectStream;

        public String ObjectStream
        {
            get
            {
                return _ObjectStream;
            }
        }

        #endregion

        #region SerializedObject

        private Byte[] _SerializedObject;

        public Byte[] SerializedObject
        {

            get
            {
                return _SerializedObject;
            }

            set
            {
                _SerializedObject = value;
            }

        }

        #endregion

        #region CacheUUID

        private CacheUUID _CacheUUID;

        public CacheUUID CacheUUID
        {

            get
            {
                return _CacheUUID;
            }

            set
            {
                _CacheUUID = value;
            }

        }

        #endregion

        #region Count

        public Int32 Count
        {
            get
            {
                return _ObjectDatastreams.Count;
            }
        }

        #endregion

        #endregion

        #region Constructors

        #region ObjectRevision(myObjectStream)

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="myObjectStream">the object stream type of the file system object</param>
        public ObjectRevision(String myObjectStream)
        {

            _ObjectPath             = "";
            _ObjectName             = "";
            _ObjectLocation         = null;

            _RevisionID             = new ObjectRevisionID(UUID.NewUUID);
            _ParentRevisionIDs      = new HashSet<ObjectRevisionID>();
            _MinNumberOfCopies      = FSConstants.MIN_NUMBER_OF_COPIES;
            _MaxNumberOfCopies      = FSConstants.MAX_NUMBER_OF_COPIES;
            _ObjectStream           = myObjectStream;
            _SerializedObject       = null;

            // TODO: Use a cheaper way to generate the UUID - e.g. number etc.
            _CacheUUID              = CacheUUID.NewUUID;

            _ObjectDatastreams      = new List<ObjectDatastream>();

        }

        #endregion

        #region ObjectRevision(myRevisionID)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myObjectStream"></param>
        /// <param name="myRevisionID"></param>
        public ObjectRevision(String myObjectStream, ObjectRevisionID myRevisionID)
            : this(myObjectStream)
        {
            _RevisionID = myRevisionID;
        }

        #endregion

        #region ObjectRevision(myObjectRevision, myCopyObjectDatastreams)

        /// <summary>
        /// Creates a new ObjectRevision based on the content of the given myObjectRevision
        /// and will set the given myObjectRevision as parent of the new ObjectRevision.
        /// </summary>
        /// <param name="myObjectRevision">The ObjectCopies to be cloned</param>
        /// <param name="myCopyObjectDatastreams">Should the ObjectDatastreams should be cloned or not!</param>
        public ObjectRevision(ObjectRevision myObjectRevision, Boolean myCopyObjectDatastreams)
        {

            //if (myObjectRevision.RevisionID == null)
            //    Debug.WriteLine("myObjectRevision.RevisionID must not be null!");// throw new ArgumentNullException("myObjectRevision.RevisionID must not be null!");

            _CacheUUID              = myObjectRevision.CacheUUID;
            _isDirty                = myObjectRevision._isDirty;
            _isNew                  = myObjectRevision.isNew;
            _SerializedObject       = myObjectRevision.SerializedObject;

            ObjectLocation          = myObjectRevision.ObjectLocation;

            _ParentRevisionIDs      = (myObjectRevision.RevisionID == null) ? new HashSet<ObjectRevisionID>() : new HashSet<ObjectRevisionID>() { myObjectRevision.RevisionID };
            _MinNumberOfCopies      = myObjectRevision.MinNumberOfCopies;
            _MaxNumberOfCopies      = myObjectRevision.MaxNumberOfCopies;
            _ObjectStream           = myObjectRevision.ObjectStream;

            _ObjectDatastreams      = new List<ObjectDatastream>();

            if (myCopyObjectDatastreams)
                foreach (var _ObjectDatastream in myObjectRevision)
                    _ObjectDatastreams.Add(new ObjectDatastream(_ObjectDatastream));

        }

        #endregion
     
        #endregion


        #region ObjectRevision-specific methods

        #region Add(myObjectStream)

        public void Add(ObjectDatastream myObjectStream)
        {
            _ObjectDatastreams.Add(myObjectStream);
        }

        #endregion

        #region Add(myObjectStreams)

        public void Add(IEnumerable<ObjectDatastream> myObjectStreams)
        {
            _ObjectDatastreams.AddRange(myObjectStreams);
        }

        #endregion

        #region Set(myObjectStreams)

        public void Set(IEnumerable<ObjectDatastream> myObjectStreams)
        {
            _ObjectDatastreams.Clear();
            _ObjectDatastreams.AddRange(myObjectStreams);
        }

        #endregion

        #region this[myCopy]

        public ObjectDatastream this[Int32 myCopy]
        {

            get
            {
                return _ObjectDatastreams[myCopy];
            }

            set
            {
                _ObjectDatastreams[myCopy] = value;
                isDirty = true;
            }

        }

        #endregion

        #endregion


        #region IEnumerable<ObjectDatastream> Members

        public IEnumerator<ObjectDatastream> GetEnumerator()
        {
            return _ObjectDatastreams.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _ObjectDatastreams.GetEnumerator();
        }

        #endregion

        #region IObjectLocation Members

        #region ObjectPath

        [NonSerialized]
        private String _ObjectPath;

        [NotIFastSerialized]
        public String ObjectPath
        {

            get
            {
                return _ObjectPath;
            }

            set
            {
                _ObjectPath      = value;
                _ObjectLocation = new ObjectLocation(DirectoryHelper.Combine(_ObjectPath, _ObjectName));
            }

        }

        #endregion

        #region ObjectName

        [NonSerialized]
        private String _ObjectName;

        [NotIFastSerialized]
        public String ObjectName
        {

            get
            {
                return _ObjectName;
            }

            set
            {
                _ObjectName      = value;
                _ObjectLocation = new ObjectLocation(DirectoryHelper.Combine(_ObjectPath, _ObjectName));
            }

        }

        #endregion

        #region ObjectLocation

        [NonSerialized]
        private ObjectLocation _ObjectLocation;

        [NotIFastSerialized]
        public ObjectLocation ObjectLocation
        {

            get
            {
                return _ObjectLocation;
            }

            set
            {
                
                _ObjectLocation  = value;

                if (_ObjectLocation != null)
                {
                    _ObjectPath = _ObjectLocation.Path;
                    _ObjectName = _ObjectLocation.Name;
                }

                else
                {
                    _ObjectPath = null;
                    _ObjectName = null;
                }

            }

        }

        #endregion

        #endregion

        #region IDirectoryListing Members

        #region IPandoraFSReference

        private IGraphFS _IPandoraFSReference;

        public IGraphFS IGraphFSReference
        {

            get
            {
                return _IPandoraFSReference;
            }

            set
            {
                _IPandoraFSReference = value;
            }

        }

        #endregion

        #region ObjectExists(myObjectName)

        public Trinary ObjectExists(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.DotLink))
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.DotDotLink))
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.ObjectCopies_MinNumberOfCopiesName))
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.ObjectCopies_MaxNumberOfCopiesName))
                return Trinary.TRUE;

            Int32 myObjectNameInt32 = Int32.Parse(myObjectName);

            if (myObjectNameInt32 > 0 && myObjectNameInt32 < _ObjectDatastreams.Count)
                return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region ObjectStreamExists(myObjectName, myObjectStream)

        public Trinary ObjectStreamExists(String myObjectName, String myObjectStream)
        {

            if (myObjectName.Equals(FSConstants.DotLink) && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.DotDotLink) && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.ObjectCopies_MinNumberOfCopiesName) && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.ObjectCopies_MaxNumberOfCopiesName) && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            Int32 myObjectNameInt32 = Int32.Parse(myObjectName);

            if (myObjectNameInt32 >= 0 && myObjectNameInt32 < _ObjectDatastreams.Count && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region GetObjectStreamsList(myObjectName)

        public IEnumerable<String> GetObjectStreamsList(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.DotLink))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            if (myObjectName.Equals(FSConstants.DotDotLink))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            if (myObjectName.Equals(FSConstants.ObjectCopies_MinNumberOfCopiesName))
                return new List<String> { FSConstants.INLINEDATA };

            if (myObjectName.Equals(FSConstants.ObjectCopies_MaxNumberOfCopiesName))
                return new List<String> { FSConstants.INLINEDATA };

            Int32 myObjectNameInt32 = Int32.Parse(myObjectName);

            if (myObjectNameInt32 >= 0 && myObjectNameInt32 < _ObjectDatastreams.Count)
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            return new List<String>();

        }

        #endregion

        #region GetObjectINodePositions(myObjectName)

        public IEnumerable<ExtendedPosition> GetObjectINodePositions(String myObjectName)
        {
            return new List<ExtendedPosition>();
        }

        #endregion

        #region GetInlineData(myObjectName)

        public Byte[] GetInlineData(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.ObjectCopies_MinNumberOfCopiesName))
                return Encoding.UTF8.GetBytes(_MinNumberOfCopies.ToString());

            if (myObjectName.Equals(FSConstants.ObjectCopies_MaxNumberOfCopiesName))
                return Encoding.UTF8.GetBytes(_MaxNumberOfCopies.ToString());

            if (myObjectName.Equals(_ObjectLocation))
                return Encoding.UTF8.GetBytes(ObjectLocation);

            return new Byte[0];

        }

        #endregion

        #region hasInlineData(myObjectName)

        public Trinary hasInlineData(String myObjectName)
        {
            return ObjectStreamExists(myObjectName, FSConstants.INLINEDATA);
        }

        #endregion

        #region GetSymlink(myObjectName)

        public ObjectLocation GetSymlink(String myObjectName)
        {
            return null;
        }

        #endregion

        #region isSymlink(myObjectName)

        public Trinary isSymlink(String myObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetDirectoryListing()

        public IEnumerable<String> GetDirectoryListing()
        {

            var _DirectoryListing = new List<String>();

            _DirectoryListing.Add(".");
            _DirectoryListing.Add("..");

            _DirectoryListing.Add(FSConstants.ObjectCopies_MinNumberOfCopiesName);
            _DirectoryListing.Add(FSConstants.ObjectCopies_MaxNumberOfCopiesName);

            for (Int32 i=0; i < _ObjectDatastreams.Count; i++)
                _DirectoryListing.Add(String.Format("{0}", i));

            return _DirectoryListing;

        }

        #endregion

        #region GetDirectoryListing(myFunc)

        public IEnumerable<String> GetDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetDirectoryListing(myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

        public IEnumerable<String> GetDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
        {
            return GetDirectoryListing();
        }

        #endregion

        #region GetExtendedDirectoryListing()

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing()
        {

            var _ExtendedDirectoryListing = new List<DirectoryEntryInformation>();
            var _OutputParameter = new DirectoryEntryInformation();

            _OutputParameter.Name         = ".";
            _OutputParameter.Streams  = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name         = "..";
            _OutputParameter.Streams  = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name         = FSConstants.ObjectCopies_MinNumberOfCopiesName;
            _OutputParameter.Streams  = new HashSet<String> { FSConstants.INLINEDATA };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            _OutputParameter.Name         = FSConstants.ObjectCopies_MaxNumberOfCopiesName;
            _OutputParameter.Streams  = new HashSet<String> { FSConstants.INLINEDATA };
            _ExtendedDirectoryListing.Add(_OutputParameter);

            for (Int32 i=0; i < _ObjectDatastreams.Count; i++)
            {
                _OutputParameter.Name        = String.Format("{0}", i);
                _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
                _ExtendedDirectoryListing.Add(_OutputParameter);
            }

            return _ExtendedDirectoryListing;

        }

        #endregion

        #region GetExtendedDirectoryListing(myFunc)

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetExtendedDirectoryListing(myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing(string[] myName, string[] myIgnoreName, string[] myRegExpr, List<String> myObjectStream, List<String> myIgnoreObjectStreams)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DirCount()

        public UInt64 DirCount
        {
            get
            {
                return (UInt64)GetDirectoryListing().LongCount();
            }
        }

        #endregion


        public NHIDirectoryObject NotificationHandling
        {
            get { throw new NotImplementedException(); }
        }

        public void SubscribeNotification(NHIDirectoryObject myNotificationHandling)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeNotification(NHIDirectoryObject myNotificationHandling)
        {
            throw new NotImplementedException();
        }


        #region GetDirectoryEntry(myObjectName)

        public DirectoryEntry GetDirectoryEntry(String myObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion


        #region isNew

        [NonSerialized]
        private Boolean _isNew;

        [NotIFastSerialized]
        public Boolean isNew
        {

            get
            {
                return _isNew;
            }

            set
            {
                _isNew = value;
            }

        }

        #endregion

        #region isDirty

        [NonSerialized]
        private Boolean _isDirty;

        [NotIFastSerialized]
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

        #region ToString()

        public override String ToString()
        {

            var _CopyCount    = (_ObjectDatastreams == null) ? 0 : _ObjectDatastreams.Count;
            var _ParentsCount = (_ParentRevisionIDs == null) ? 0 : _ParentRevisionIDs.Count;

            return String.Format("RevisionID: {0}, CopyCount: {1}, Min: {2}, Max: {3}, ParentsCount: {4}", RevisionID.ToString(), _CopyCount, _MinNumberOfCopies, _MaxNumberOfCopies, _ParentsCount);
        }

        #endregion
       
    }

}
