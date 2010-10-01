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
 * GraphFSInterface - ObjectEdition
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections; 
using System.Collections.Generic;

using sones.Lib;
using sones.Lib.Serializer;

using sones.Lib.DataStructures;
using sones.StorageEngines;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Exceptions;
using sones.GraphFS.InternalObjects;

#endregion

namespace sones.GraphFS.DataStructures
{
  
    /// <summary>
    /// An ObjectEdition is part of the ObjectLocator describing
    /// different revisions of an ObjectStream. It keeps track of the
    /// oldest and latest ObjectRevision.
    /// </summary>

    public class ObjectEdition : IGraphFSDictionary<ObjectRevisionID, ObjectRevision>, IDirectoryListing
    {


        #region Data

        private SortedDictionary<ObjectRevisionID, ObjectRevision>    _ObjectRevisions;
        //private StefanTuple<RevisionID, ObjectRevision, UUID>   _TransactionRevision;
        //private Object                                          _TransactionLockObject      = new Object();

        private String                                          _MinNumberOfRevisionsName   = "MinNumberOfRevisions";
        private String                                          _MaxNumberOfRevisionsName   = "MaxNumberOfRevisions";
        private String                                          _MinRevisionDeltaName       = "MinRevisionDelta";
        private String                                          _MaxRevisionAgeName         = "MaxRevisionAge";

        #endregion

        #region Properties

        #region Name

        private String _ObjectEditionName;

        public String Name
        {
            get
            {
                return _ObjectEditionName;
            }
        }

        #endregion

        #region IsDeleted

        private Boolean _IsDeleted;

        public Boolean IsDeleted
        {

            get
            {
                return _IsDeleted;
            }

            set
            {
                _IsDeleted = value;
            }

        }

        #endregion

        #region MinNumberOfRevisions

        private UInt64 _MinNumberOfRevisions;

        /// <summary>
        /// Minimal number of revisions of an object stream. The file system will
        /// guarantee that this number of revisions will be on disc at any time.
        /// </summary>
        public UInt64 MinNumberOfRevisions
        {

            get
            {
                return _MinNumberOfRevisions;
            }

            set
            {
                _MinNumberOfRevisions = value;
            }

        }

        #endregion

        #region MaxNumberOfRevisions

        private UInt64 _MaxNumberOfRevisions;

        /// <summary>
        /// Minimal number of revisions of an object stream. The file system will
        /// automatically delete supernumerous revisions if this value decreases
        /// or the file system is getting low on free space.
        /// </summary>
        public UInt64 MaxNumberOfRevisions
        {

            get
            {
                return _MaxNumberOfRevisions;
            }

            set
            {
                _MaxNumberOfRevisions = value;
            }

        }

        #endregion

        #region MinRevisionDelta

        private UInt64 _MinRevisionDelta;

        /// <summary>
        /// Minimal timespan between to revisions.
        /// If the timespan between two revisions is smaller both revisions will
        /// be combined to the later revision.
        /// </summary>
        public UInt64 MinRevisionDelta
        {

            get
            {
                return _MinRevisionDelta;
            }

            set
            {
                _MinRevisionDelta = value;

            }

        }

        #endregion

        #region MaxRevisionAge

        private UInt64 _MaxRevisionAge;

        /// <summary>
        /// Maximal age of an object revision. Older revisions will be
        /// deleted automatically if they also satify the MaxNumberOfRevisions
        /// criterium.
        /// </summary>
        public UInt64 MaxRevisionAge
        {

            get
            {
                return _MaxRevisionAge;
            }

            set
            {
                _MaxRevisionAge = value;
            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        #region ObjectEdition()

        /// <summary>
        /// Main constructor
        /// </summary>
        public ObjectEdition()
        {

            _ObjectPath             = null;
            _ObjectName             = "";
            _ObjectLocation         = null;

            _ObjectRevisions        = new SortedDictionary<ObjectRevisionID, ObjectRevision>();

            _ObjectEditionName      = "";
            _IsDeleted              = false;
            _MinNumberOfRevisions   = FSConstants.MIN_NUMBER_OF_REVISIONS;
            _MaxNumberOfRevisions   = FSConstants.MAX_NUMBER_OF_REVISIONS;
            _MinRevisionDelta       = FSConstants.MIN_REVISION_DELTA;
            _MaxRevisionAge         = FSConstants.MAX_REVISION_AGE;

        }

        #endregion

        #region ObjectEdition(myObjectEditionName)

        /// <summary>
        /// Sets the ObjectEditionName
        /// </summary>
        /// <param name="myObjectEditionName"></param>
        public ObjectEdition(String myObjectEditionName)
            : this()
        {
            _ObjectEditionName = myObjectEditionName;
        }

        #endregion

        #region ObjectEdition(myObjectEditionName, myObjectRevisionID, myObjectRevision)

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="myObjectEditionName"></param>
        /// <param name="myRevisionID"></param>
        /// <param name="myObjectCopies"></param>
        public ObjectEdition(String myObjectEditionName, ObjectRevisionID myObjectRevisionID, ObjectRevision myObjectRevision)
            : this(myObjectEditionName)
        {
            SetRevision(myObjectRevisionID, myObjectRevision);
        }

        #endregion

        #endregion


        #region Object-specific methods

        #region SetRevision(myRevisionID, myObjectRevision)

        public void SetRevision(ObjectRevisionID myRevisionID, ObjectRevision myObjectRevision)
        {

            ObjectRevision _ObjectRevision = null;

            if (_ObjectRevisions.TryGetValue(myRevisionID, out _ObjectRevision))
            {
                _ObjectRevision = myObjectRevision;
                return;
            }

            Add(myRevisionID, myObjectRevision);

        }

        #endregion


        #region GetListOfRevisionIDs()

        /// <summary>
        /// Returns a list of all stored RevisionIDs
        /// </summary>
        /// <returns>a list of all stored RevisionIDs</returns>
        public IEnumerable<ObjectRevisionID> GetListOfRevisionIDs()
        {
            return from _RevisionID in _ObjectRevisions.Keys select _RevisionID;
        }

        #endregion


        #region LatestRevision

        /// <summary>
        /// Returns the latest ObjectRevision object.
        /// </summary>
        public ObjectRevision LatestRevision
        {
            get
            {
                return this[LatestRevisionID];
            }
        }

        #endregion

        #region LatestRevisionID

        /// <summary>
        /// Returns the latest ObjectRevisionID.
        /// </summary>
        public ObjectRevisionID LatestRevisionID
        {
            get
            {

                if (_ObjectRevisions != null)
                    return _ObjectRevisions.Keys.Max();

                return null;

            }
        }

        #endregion


        #region LatestFullRevision

        /// <summary>
        /// Returns the latest ObjectRevision object which is not a delta revision.
        /// </summary>
        public ObjectRevision LatestFullRevision
        {
            get
            {
                return this[LatestFullRevisionID];
            }
        }

        #endregion

        #region LatestFullRevisionID

        /// <summary>
        /// Returns the latest ObjectRevisionID which is not a delta revision.
        /// </summary>
        public ObjectRevisionID LatestFullRevisionID
        {
            get
            {

                if (_ObjectRevisions != null)
                    return _ObjectRevisions.Where(_Revision => _Revision.Value.IsDeltaRevision == false).Max().Key;

                return null;

            }
        }

        #endregion


        #region GetObjectRevisionByNearestRevisionID(myRevisionID)

        /// <summary>
        /// Returns the latest ObjectRevision for the given RevisionID
        /// </summary>
        /// <returns>the latest ObjectRevision for the given RevisionID</returns>
        public ObjectRevision GetObjectRevisionByNearestRevisionID(ObjectRevisionID myRevisionID)
        {
            return this[GetNearestRevisionID(myRevisionID)];
        }

        #endregion

        #region GetNearestRevisionID(myRevisionID)

        /// <summary>
        /// Returns the nearest RevisionID for the given RevisionID
        /// </summary>
        /// <returns>the nearest object RevisionID for the given RevisionID</returns>
        public ObjectRevisionID GetNearestRevisionID(ObjectRevisionID myRevisionID)
        {

            if (myRevisionID == null)
                throw new ArgumentNullException("myRevisionID must not be null!");

            if (_ObjectRevisions != null)
                return (from _RevisionID in _ObjectRevisions.Keys select _RevisionID).Where(key => key < myRevisionID).OrderByDescending(key => key).FirstOrDefault();

            return null;

        }

        #endregion


        #region SecondOldestRevision

        /// <summary>
        /// Returns the oldest object copy object
        /// </summary>
        /// <returns>the oldest object copy object</returns>
        public ObjectRevision SecondOldestRevision
        {
            get
            {

                //if (_ObjectRevisions != null)
                //    if (_RevisionIDs != null)
                //        if (_RevisionIDs.First != null)
                //            if (_RevisionIDs.First.Next != null)
                //                return _ObjectRevisions[_RevisionIDs.First.Next.Value];

                return null;

            }
        }

        #endregion

        #region SecondOldestRevisionID

        /// <summary>
        /// Returns the oldest RevisionID.
        /// </summary>
        /// <returns>the oldest RevisionID</returns>
        public ObjectRevisionID SecondOldestRevisionID
        {
            get
            {

                //if (_RevisionIDs != null)
                //    if (_RevisionIDs.First != null)
                //        if (_RevisionIDs.First.Next != null)
                //            return _RevisionIDs.First.Next.Value;

                return null;

            }
        }

        #endregion


        #region OldestRevision

        /// <summary>
        /// Returns the oldest object copy object
        /// </summary>
        /// <returns>the oldest object copy object</returns>
        public ObjectRevision OldestRevision
        {
            get
            {
                return this[OldestRevisionID];
            }
        }

        #endregion

        #region OldestRevisionID

        /// <summary>
        /// Returns the oldest RevisionID.
        /// </summary>
        /// <returns>the oldest RevisionID</returns>
        public ObjectRevisionID OldestRevisionID
        {
            get
            {

                if (_ObjectRevisions != null)
                    return _ObjectRevisions.Keys.Min();

                return null;

            }
        }

        #endregion


        #region GetChildRevisions(myRevisionID)

        /// <summary>
        /// Returns a list of child revisions
        /// </summary>
        /// <returns>a list of child revisions</returns>
        public IEnumerable<ObjectRevisionID> GetChildRevisions(ObjectRevisionID myRevisionID)
        {

            var _ListOfChildRevisionIDs = new HashSet<ObjectRevisionID>();

            foreach (var _KeyValuePair in _ObjectRevisions)
            {
                if (_KeyValuePair.Value.ParentRevisionIDs.Contains(myRevisionID))
                    _ListOfChildRevisionIDs.Add(_KeyValuePair.Key);
            }

            return _ListOfChildRevisionIDs;

        }

        #endregion

        #region GetMaxPathLength(myRevisionID)

        /// <summary>
        /// Return the max path length. This is equal or smaller then the number of revisions.
        /// </summary>
        /// <param name="myRevisionID">Starting with this RevisionID</param>
        /// <returns>The number of parents of the longest path</returns>
        public UInt64 GetMaxPathLength(ObjectRevisionID myRevisionID)
        {
            return GetMaxPathLength(myRevisionID, 1);
        }

        #endregion

        #region GetMaxPathLength(myRevisionID, myDepth)

        private UInt64 GetMaxPathLength(ObjectRevisionID myRevisionID, UInt64 myDepth)
        {

            if (_ObjectRevisions == null)
                return 0;

            ObjectRevision _ObjectRevision = null;
            IEnumerable<ObjectRevisionID> _ListOfParentRevisionIDs = null;

            if (_ObjectRevisions.TryGetValue(myRevisionID, out _ObjectRevision))
                _ListOfParentRevisionIDs = _ObjectRevision.ParentRevisionIDs;
            else
                return myDepth;

            if (_ListOfParentRevisionIDs == null)
                return myDepth;

            foreach (var _RevisionID in _ListOfParentRevisionIDs)
            {

                var temp = GetMaxPathLength(_RevisionID, myDepth + 1);

                if (temp > myDepth)
                    myDepth = temp;

            }

            return myDepth;

        }

        #endregion

        #endregion


        #region IGraphFSDictionary<RevisionID, ObjectRevision> Members

        #region Add(myRevisionID, myObjectRevision)

        public Boolean Add(ObjectRevisionID myRevisionID, ObjectRevision myObjectRevision)
        {

            lock (this)
            {

                if (myRevisionID == null)
                    throw new ArgumentNullException();

                if (_ObjectRevisions == null)
                    return false;

                var _LatestRevisionID = LatestRevisionID;

                if (_LatestRevisionID != null && myObjectRevision != null)
                {
                    if (myObjectRevision.ParentRevisionIDs == null)
                        myObjectRevision.ParentRevisionIDs = new HashSet<ObjectRevisionID>() { _LatestRevisionID };
                    else
                        myObjectRevision.ParentRevisionIDs.Add(_LatestRevisionID);
                }

                _ObjectRevisions.Add(myRevisionID, myObjectRevision);
                
                return true;

            }
        
        }

        #endregion

        #region Add(myKeyValuePair)

        public Boolean Add(KeyValuePair<ObjectRevisionID, ObjectRevision> myKeyValuePair)
        {
            return Add(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion


        #region this[myRevisionID]

        public ObjectRevision this[ObjectRevisionID myRevisionID]
        {

            get
            {

                ObjectRevision _ObjectRevision = null;

                if (_ObjectRevisions != null)
                    if (_ObjectRevisions.TryGetValue(myRevisionID, out _ObjectRevision))
                        return _ObjectRevision;

                return null;

            }

            set
            {

                if (_ObjectRevisions.ContainsKey(myRevisionID))
                    _ObjectRevisions[myRevisionID] = value;

                else Add(myRevisionID, value);

                isDirty = true;

            }

        }

        #endregion


        #region ContainsKey(myRevisionID)

        public Boolean ContainsKey(ObjectRevisionID myRevisionID)
        {
            return _ObjectRevisions.ContainsKey(myRevisionID);
        }

        #endregion

        #region Contains(myKeyValuePair)

        public Boolean Contains(KeyValuePair<ObjectRevisionID, ObjectRevision> myKeyValuePair)
        {

            ObjectRevision _ObjectRevision;

            if (_ObjectRevisions.TryGetValue(myKeyValuePair.Key, out _ObjectRevision))
                if (myKeyValuePair.Value.Equals(_ObjectRevision))
                    return true;

            return false;

        }

        #endregion

        #region Keys

        public IEnumerable<ObjectRevisionID> Keys
        {
            get
            {
                return from _Items in _ObjectRevisions select _Items.Key;
            }
        }

        #endregion

        #region Values

        public IEnumerable<ObjectRevision> Values
        {
            get
            {
                return from _Items in _ObjectRevisions select _Items.Value;
            }
        }

        #endregion

        #region Count

        public UInt64 Count
        {
            get
            {
                return _ObjectRevisions.ULongCount();
            }
        }

        #endregion


        #region Remove(myRevisionID)

        public Boolean Remove(ObjectRevisionID myRevisionID)
        {
            return _ObjectRevisions.Remove(myRevisionID);
        }

        #endregion

        #region Remove(myKeyValuePair)

        public Boolean Remove(KeyValuePair<ObjectRevisionID, ObjectRevision> myKeyValuePair)
        {

            ObjectRevision _ObjectRevision;

            if (_ObjectRevisions.TryGetValue(myKeyValuePair.Key, out _ObjectRevision))
                if (myKeyValuePair.Value.Equals(_ObjectRevision))
                    return _ObjectRevisions.Remove(myKeyValuePair.Key);

            return false;

        }

        #endregion

        #region Clear()

        public void Clear()
        {
            _ObjectRevisions.Clear();
        }

        #endregion

        #endregion


        #region IEnumerable<KeyValuePair<RevisionID, ObjectCopies>> Members

        public IEnumerator<KeyValuePair<ObjectRevisionID, ObjectRevision>> GetEnumerator()
        {
            return _ObjectRevisions.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _ObjectRevisions.GetEnumerator();
        }

        #endregion

        #region IObjectLocation Members

        #region ObjectPath

        [NonSerialized]
        private ObjectLocation _ObjectPath;

        [NotIFastSerialized]
        public ObjectLocation ObjectPath
        {

            get
            {
                return _ObjectPath;
            }

            set
            {
                _ObjectPath      = value;
                _ObjectLocation = new ObjectLocation(_ObjectPath, _ObjectName);
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
                _ObjectLocation = new ObjectLocation(_ObjectPath, _ObjectName);
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
                _ObjectPath      = _ObjectLocation.Path;
                _ObjectName      = _ObjectLocation.Name;

            }

        }

        #endregion

        #endregion

        #region IDirectoryListing Members

        #region IGraphFSReference

        private IGraphFS _IGraphFSReference;

        public IGraphFS IGraphFSReference
        {

            get
            {
                return _IGraphFSReference;
            }

            set
            {
                _IGraphFSReference = value;
            }

        }

        #endregion

        #region ObjectExists(myObjectName)

        Trinary IDirectoryListing.ObjectExists(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.DotLink))
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.DotDotLink))
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.DotLatestRevisionSymlink))
                return Trinary.TRUE;

            if (myObjectName.Equals(_MinNumberOfRevisionsName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_MaxNumberOfRevisionsName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_MinRevisionDeltaName))
                return Trinary.TRUE;

            if (myObjectName.Equals(_MaxRevisionAgeName))
                return Trinary.TRUE;

            foreach (var _RevisionID in _ObjectRevisions.Keys)
                if (myObjectName.Equals(String.Format("{0:yyyyddMM.HHmmss.fffffff}({1})", new DateTime((Int64)_RevisionID.Timestamp), _RevisionID.UUID.ToString())))
                    return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region ObjectStreamExists(myObjectName, myObjectStream)

        Trinary IDirectoryListing.ObjectStreamExists(String myObjectName, String myObjectStream)
        {

            if (myObjectName.Equals(FSConstants.DotLink) && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.DotDotLink) && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.DotLatestRevisionSymlink) && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            if (myObjectName.Equals(_MinNumberOfRevisionsName) && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            if (myObjectName.Equals(_MaxNumberOfRevisionsName) && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            if (myObjectName.Equals(_MinRevisionDeltaName) && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            if (myObjectName.Equals(_MaxRevisionAgeName) && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            foreach (var _RevisionID in _ObjectRevisions.Keys)
                if (myObjectName.Equals(String.Format("{0:yyyyddMM.HHmmss.fffffff}({1})", new DateTime((Int64)_RevisionID.Timestamp), _RevisionID.UUID.ToString())) &&
                    myObjectStream == FSConstants.VIRTUALDIRECTORY)
                    return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region GetObjectStreamsList(myObjectName)

        IEnumerable<String> IDirectoryListing.GetObjectStreamsList(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.DotLink))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            if (myObjectName.Equals(FSConstants.DotDotLink))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            foreach (var _RevisionID in _ObjectRevisions.Keys)
                if (myObjectName.Equals(String.Format("{0:yyyyddMM.HHmmss.fffffff}({1})", new DateTime((Int64)_RevisionID.Timestamp), _RevisionID.UUID.ToString())))
                    return new List<String> { FSConstants.VIRTUALDIRECTORY };

            return new List<String>();

        }

        #endregion

        #region GetObjectINodePositions(myObjectName)

        IEnumerable<ExtendedPosition> IDirectoryListing.GetObjectINodePositions(String myObjectName)
        {
            return new List<ExtendedPosition>();
        }

        #endregion

        #region GetInlineData(myObjectName)

        Byte[] IDirectoryListing.GetInlineData(String myObjectName)
        {

            if (myObjectName.Equals(_MinNumberOfRevisionsName))
                return Encoding.UTF8.GetBytes(_MinNumberOfRevisions.ToString());

            if (myObjectName.Equals(_MaxNumberOfRevisionsName))
                return Encoding.UTF8.GetBytes(_MaxNumberOfRevisions.ToString());

            if (myObjectName.Equals(_MinRevisionDeltaName))
                return Encoding.UTF8.GetBytes(_MinRevisionDelta.ToString());

            if (myObjectName.Equals(_MaxRevisionAgeName))
                return Encoding.UTF8.GetBytes(_MaxRevisionAge.ToString());

            return new Byte[0];

        }

        #endregion

        #region hasInlineData(myObjectName)

        Trinary IDirectoryListing.hasInlineData(String myObjectName)
        {
            return ((IDirectoryListing)this).ObjectStreamExists(myObjectName, FSConstants.INLINEDATA);
        }

        #endregion

        #region GetSymlink(myObjectName)

        ObjectLocation IDirectoryListing.GetSymlink(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.DotLatestRevisionSymlink))
                return new ObjectLocation(".", String.Format("{0:yyyyddMM.HHmmss.fffffff}({1})", new DateTime((Int64)_ObjectRevisions.Keys.Max().Timestamp), _ObjectRevisions.Keys.Max().UUID.ToString()));

            return null;

        }

        #endregion

        #region isSymlink(myObjectName)

        Trinary IDirectoryListing.isSymlink(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.DotLatestRevisionSymlink))
                return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region GetDirectoryListing()

        IEnumerable<String> IDirectoryListing.GetDirectoryListing()
        {

            var _DirectoryListing = new List<String>();

            _DirectoryListing.Add(".");
            _DirectoryListing.Add("..");

            foreach (var __DirectoryEntry in _ObjectRevisions.Keys)
                _DirectoryListing.Add(String.Format("{0:yyyyddMM.HHmmss.fffffff}({1})", new DateTime((Int64)__DirectoryEntry.Timestamp), __DirectoryEntry.UUID.ToString()));

            _DirectoryListing.Add(FSConstants.DotLatestRevisionSymlink);
            _DirectoryListing.Add("MinNumberOfRevisions");
            _DirectoryListing.Add("MaxNumberOfRevisions");
            _DirectoryListing.Add("MinRevisionDelta");
            _DirectoryListing.Add("MaxRevisionAge");

            return _DirectoryListing;

        }

        #endregion

        #region GetDirectoryListing(myFunc)

        IEnumerable<String> IDirectoryListing.GetDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetDirectoryListing(myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

        IEnumerable<String> IDirectoryListing.GetDirectoryListing(String[] myName, String[] myIgnoreName, String[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
        {
            return ((IDirectoryListing)this).GetDirectoryListing();
        }

        #endregion

        #region GetExtendedDirectoryListing()

        IEnumerable<DirectoryEntryInformation> IDirectoryListing.GetExtendedDirectoryListing()
        {

            var _Output = new List<DirectoryEntryInformation>();
            var _OutputParameter = new DirectoryEntryInformation();

            _OutputParameter.Name         = ".";
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _Output.Add(_OutputParameter);

            _OutputParameter.Name         = "..";
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _Output.Add(_OutputParameter);

            foreach (var __DirectoryEntry in _ObjectRevisions.Keys)
            {
                _OutputParameter.Name         = String.Format("{0:yyyyddMM.HHmmss.fffffff}({1})", new DateTime( (Int64) __DirectoryEntry.Timestamp), __DirectoryEntry.UUID.ToString());
                _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
                _Output.Add(_OutputParameter);
            }

            _OutputParameter.Name         = FSConstants.DotLatestRevisionSymlink;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.SYMLINK };
            _Output.Add(_OutputParameter);

            _OutputParameter.Name         = "MinNumberOfRevisions";
            _OutputParameter.Streams = new HashSet<String> { FSConstants.INLINEDATA };
            _Output.Add(_OutputParameter);

            _OutputParameter.Name         = "MaxNumberOfRevisions";
            _OutputParameter.Streams = new HashSet<String> { FSConstants.INLINEDATA };
            _Output.Add(_OutputParameter);

            _OutputParameter.Name         = "MinRevisionDelta";
            _OutputParameter.Streams = new HashSet<String> { FSConstants.INLINEDATA };
            _Output.Add(_OutputParameter);

            _OutputParameter.Name         = "MaxRevisionAge";
            _OutputParameter.Streams = new HashSet<String> { FSConstants.INLINEDATA };
            _Output.Add(_OutputParameter);

            return _Output;

        }

        #endregion

        #region GetExtendedDirectoryListing(myFunc)

        IEnumerable<DirectoryEntryInformation> IDirectoryListing.GetExtendedDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetExtendedDirectoryListing(myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

        IEnumerable<DirectoryEntryInformation> IDirectoryListing.GetExtendedDirectoryListing(string[] myName, string[] myIgnoreName, string[] myRegExpr, List<String> myObjectStream, List<String> myIgnoreObjectStreams)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DirCount()

        UInt64 IDirectoryListing.DirCount
        {
            get
            {
                return ((IDirectoryListing)this).GetDirectoryListing().ULongCount();
            }
        }

        #endregion


        NHIDirectoryObject IDirectoryListing.NotificationHandling
        {
            get { throw new NotImplementedException(); }
        }

        void IDirectoryListing.SubscribeNotification(NHIDirectoryObject myNotificationHandling)
        {
            throw new NotImplementedException();
        }

        void IDirectoryListing.UnsubscribeNotification(NHIDirectoryObject myNotificationHandling)
        {
            throw new NotImplementedException();
        }


        #region GetDirectoryEntry(myObjectName)

        DirectoryEntry IDirectoryListing.GetDirectoryEntry(String myObjectName)
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

            var _ObjectRevisionsCount = (_ObjectRevisions == null) ? 0 : _ObjectRevisions.Count;

            return String.Format("Name: {0}, Deleted: {1}, RevisionCount: {2}, Min: {3}, Max: {4}, MinDelta: {5}, MaxAge: {6}", _ObjectEditionName, _IsDeleted, _ObjectRevisionsCount, _MinNumberOfRevisions, _MaxNumberOfRevisions, _MinRevisionDelta, _MaxRevisionAge);

        }

        #endregion

    }

}
