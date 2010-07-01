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
 * AObjectOntology
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphFS.DataStructures;
using sones.Lib.Serializer;
using sones.Lib.DataStructures;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The abstract class for the GraphFS object ontology.
    /// </summary>

    public abstract class AObjectOntology : AObjectHeader, IObjectOntology
    {


        #region Constructors

        #region APandoraOntology()

        /// <summary>
        /// This will set all important variables within this PandoraObject.
        /// This will especially create a new ObjectUUID and mark the
        /// APandoraObject as "new" and "dirty".
        /// </summary>
        public AObjectOntology()
        {

            _ObjectName             = "";
            _ObjectPath             = "";
            _ObjectLocation         = null;
            _ObjectStream           = null;

            _ObjectSize             = 0;
            _ObjectSizeOnDisc       = 0;

        }

        #endregion

        #region APandoraOntology(myObjectUUID)

        /// <summary>
        /// This will set all important variables within this PandoraObject.
        /// Additionally it sets the ObjectUUID to the given value and marks
        /// the APandoraObject as "new" and "dirty".
        /// </summary>
        public AObjectOntology(ObjectUUID myObjectUUID)
            : this()
        {
            // Members of APandoraStructure
            ObjectUUID               = myObjectUUID;
        }

        #endregion

        #endregion


        #region Properties - in-memory only!

        // The scope of these properties is in-memory only!
        // This means that these properties must not be written on disc,
        // as it might create conflicts during MakeFileSystem and while
        // creating and using hardlinks.

        #region ObjectLocation

        [NonSerialized]
        protected ObjectLocation _ObjectLocation;

        /// <summary>
        /// Stores the complete ObjectLocation (ObjectPath and ObjectName) of
        /// this file system object. Changing this property will automagically
        /// change the ObjectPath and ObjectName property.
        /// </summary>
        [NotIFastSerialized]
        public ObjectLocation ObjectLocation
        {

            get
            {
                return _ObjectLocation;
            }

            set
            {

                if (value == null || value.Length < FSPathConstants.PathDelimiter.Length)
                    throw new ArgumentNullException("Invalid ObjectLocation!");

                _ObjectLocation  = value;
                _ObjectPath      = _ObjectLocation.Path;
                _ObjectName      = _ObjectLocation.Name;

                isDirty          = true;

            }

            //set
            //{

            //    if (value == null || value.Length < FSPathConstants.PathDelimiter.Length)
            //        throw new ArgumentNullException("Invalid ObjectLocation!");

            //    _ObjectLocation  = value;
            //    _ObjectPath      = _ObjectLocation.Path;
            //    _ObjectName      = _ObjectLocation.Name;

            //    isDirty          = true;

            //}

        }

        #endregion

        #region ObjectPath

        [NonSerialized]
        protected String _ObjectPath;

        /// <summary>
        /// Stores the ObjectPath of this APandoraObject. Changing this
        /// property will automagically change the myObjectLocation property.
        /// </summary>
        [NotIFastSerialized]
        public String ObjectPath
        {

            get
            {
                //if (_ObjectPath == null || _ObjectPath == String .Empty)
                //    _ObjectPath = DirectoryHelper.GetObjectPath(_ObjectLocation);
                return _ObjectPath;
            }

            set
            {

                if (value == null || value.Length < FSPathConstants.PathDelimiter.Length)
                    throw new ArgumentNullException("The ObjectPath must not be null or its length zero!");

                _ObjectPath     = value;
                _ObjectLocation = new ObjectLocation(DirectoryHelper.Combine(_ObjectPath, _ObjectName));
                isDirty         = true;

            }

        }

        #endregion

        #region ObjectName

        [NonSerialized]
        protected String _ObjectName;

        /// <summary>
        /// Stores the ObjectName of this APandoraObject. Changing this
        /// property will automagically change the myObjectLocation property.
        /// </summary>
        [NotIFastSerialized]
        public String ObjectName
        {

            get
            {
                //if (_ObjectName == null || _ObjectName == String.Empty)
                //    _ObjectName = DirectoryHelper.GetObjectName(_ObjectLocation);
                return _ObjectName;
            }

            set
            {

                if (value == null || value.Length == 0)
                    throw new ArgumentNullException("The ObjectName must not be null or its length zero!");

                _ObjectName     = value;
                _ObjectLocation = new ObjectLocation(DirectoryHelper.Combine(_ObjectPath, _ObjectName));
                isDirty         = true;

            }

        }

        #endregion


        #region ObjectStream

        [NonSerialized]
        protected String _ObjectStream = FSConstants.FILESTREAM;

        /// <summary>
        /// The actual name of this ObjectStream
        /// </summary>
        [NotIFastSerialized]
        public String ObjectStream
        {
            
            get
            {
                return _ObjectStream;
            }
            
            set
            {

                if (value == null || value.Length == 0)
                    throw new ArgumentNullException("The ObjectStream must not be null or its length zero!");

                _ObjectStream   = value;
                isDirty         = true;

            }

        }

        #endregion

        #region ObjectStreams

        /// <summary>
        /// Stores all ObjectStreams and their associated names, e.g. FILESTREAM
        /// </summary>
        [NotIFastSerialized]
        public IDictionary<String, ObjectStream> ObjectStreams
        {
            get
            {
                
                if (ObjectLocatorReference != null)
                    return ObjectLocatorReference.ToDictionary(key => key.Key, value => value.Value);

                return new Dictionary<String, ObjectStream>();

            }
        }

        #endregion


        #region ObjectEdition

        [NonSerialized]
        protected String _ObjectEdition = FSConstants.DefaultEdition;

        /// <summary>
        /// The actual name of this ObjectEdition
        /// </summary>
        [NotIFastSerialized]
        public String ObjectEdition
        {
            
            get
            {
                return _ObjectEdition;
            }
            
            set
            {

                if (value == null || value.Length == 0)
                    throw new ArgumentNullException("The ObjectEdition must not be null or its length zero!");
                
                _ObjectEdition  = value;
                isDirty         = true;

            }

        }

        #endregion

        #region ObjectEditions

        /// <summary>
        /// Stores all ObjectEditions and their associated names
        /// </summary>
        [NotIFastSerialized]
        public IDictionary<String, ObjectEdition> ObjectEditions
        {
            get
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        return ObjectLocatorReference[_ObjectStream].ToDictionary(key => key.Key, value => value.Value);

                return new Dictionary<String, ObjectEdition>();

            }
        }

        #endregion


        #region MinNumberOfRevisions

        /// <summary>
        /// The minimal number of revisions to store
        /// </summary>
        public UInt64 MinNumberOfRevisions
        {

            get
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            return ObjectLocatorReference[_ObjectStream][_ObjectEdition].MinNumberOfRevisions;

                throw new ArgumentException("Could not get MinNumberOfRevisions!");

            }

            set
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            // This will mark the ObjectLocator dirty!
                            ObjectLocatorReference[_ObjectStream][_ObjectEdition].MinNumberOfRevisions = value;

                throw new ArgumentException("Could not set MinNumberOfRevisions!");

            }

        }

        #endregion

        #region NumberOfRevisions

        /// <summary>
        /// The actual number of revisions stored
        /// </summary>
        public UInt64 NumberOfRevisions
        {

            get
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            return ObjectLocatorReference[_ObjectStream][_ObjectEdition].Count;

                throw new ArgumentException("Could not get NumberOfRevisions!");

            }

        }

        #endregion

        #region MaxNumberOfRevisions

        /// <summary>
        /// The maximal number of revisions to store
        /// </summary>
        public UInt64 MaxNumberOfRevisions
        {

            get
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            // This will mark the ObjectLocator dirty!
                            return ObjectLocatorReference[_ObjectStream][_ObjectEdition].MaxNumberOfRevisions;

                throw new ArgumentException("Could not get MaxNumberOfRevisions!");

            }

            set
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            ObjectLocatorReference[_ObjectStream][_ObjectEdition].MaxNumberOfRevisions = value;

                throw new ArgumentException("Could not set MaxNumberOfRevisions!");

            }

        }

        #endregion

        #region MinRevisionDelta

        /// <summary>
        /// Minimal timespan between to revisions.
        /// If the timespan between two revisions is smaller both revisions will
        /// be combined to the later revision.
        /// </summary>
        public UInt64 MinRevisionDelta
        {

            get
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            // This will mark the ObjectLocator dirty!
                            return ObjectLocatorReference[_ObjectStream][_ObjectEdition].MinRevisionDelta;

                throw new ArgumentException("Could not get MinRevisionDelta!");

            }

            set
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            ObjectLocatorReference[_ObjectStream][_ObjectEdition].MinRevisionDelta = value;

                throw new ArgumentException("Could not set MinRevisionDelta!");

            }

        }

        #endregion

        #region MaxRevisionAge

        /// <summary>
        /// Maximal age of an object revision. Older revisions will be
        /// deleted automatically if they also satify the MaxNumberOfRevisions
        /// criterium.
        /// </summary>
        public UInt64 MaxRevisionAge
        {

            get
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            // This will mark the ObjectLocator dirty!
                            return ObjectLocatorReference[_ObjectStream][_ObjectEdition].MaxRevisionAge;

                throw new ArgumentException("Could not get MaxRevisionAge!");

            }

            set
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            ObjectLocatorReference[_ObjectStream][_ObjectEdition].MaxRevisionAge = value;

                throw new ArgumentException("Could not set MaxRevisionAge!");

            }

        }

        #endregion

        #region ObjectRevision

        [NonSerialized]
        protected RevisionID _ObjectRevisionID = null;

        /// <summary>
        /// The RevisionID of this file system object
        /// </summary>
        [NotIFastSerialized]
        public RevisionID ObjectRevision
        {

            get
            {
                return _ObjectRevisionID;
            }

            set
            {

                if (value == null)
                    throw new ArgumentNullException("The ObjectRevisionID must not be null!");

                _ObjectRevisionID = value;
                isDirty = true;

            }

        }

        #endregion

        #region ObjectRevisions

        /// <summary>
        /// Stores the mapping between a RevisionID and the associated ObjectRevision
        /// </summary>
        [NotIFastSerialized]
        public IDictionary<RevisionID, ObjectRevision> ObjectRevisions
        {            
            get
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            return ObjectLocatorReference[_ObjectStream][_ObjectEdition].ToDictionary(key => key.Key, value => value.Value);

                return new Dictionary<RevisionID, ObjectRevision>();

            }
        }

        #endregion


        #region MinNumberOfCopies

        /// <summary>
        /// The minimal number of copies to store
        /// </summary>
        public UInt64 MinNumberOfCopies
        {

            get
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            if (ObjectLocatorReference[_ObjectStream][_ObjectEdition].ContainsKey(_ObjectRevisionID))
                                // This will mark the ObjectLocator dirty!
                                return ObjectLocatorReference[_ObjectStream][_ObjectEdition][_ObjectRevisionID].MinNumberOfCopies;

                throw new ArgumentException("Could not get MinNumberOfCopies!");

            }

            set
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            if (ObjectLocatorReference[_ObjectStream][_ObjectEdition].ContainsKey(_ObjectRevisionID))
                                ObjectLocatorReference[_ObjectStream][_ObjectEdition][_ObjectRevisionID].MinNumberOfCopies = value;

                throw new ArgumentException("Could not set MinNumberOfCopies!");

            }

        }

        #endregion

        #region NumberOfCopies

        /// <summary>
        /// The actual number of copies stored
        /// </summary>
        public UInt64 NumberOfCopies
        {

            get
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            if (ObjectLocatorReference[_ObjectStream][_ObjectEdition].ContainsKey(_ObjectRevisionID))
                                return (UInt64) ObjectLocatorReference[_ObjectStream][_ObjectEdition][_ObjectRevisionID].Count;

                throw new ArgumentException("Could not get NumberOfCopies!");

            }

        }

        #endregion

        #region MaxNumberOfCopies

        /// <summary>
        /// The maximal number of copies to store
        /// </summary>
        public UInt64 MaxNumberOfCopies
        {

            get
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            if (ObjectLocatorReference[_ObjectStream][_ObjectEdition].ContainsKey(_ObjectRevisionID))
                                // This will mark the ObjectLocator dirty!
                                return ObjectLocatorReference[_ObjectStream][_ObjectEdition][_ObjectRevisionID].MaxNumberOfCopies;

                throw new ArgumentException("Could not get MaxNumberOfCopies!");

            }

            set
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            if (ObjectLocatorReference[_ObjectStream][_ObjectEdition].ContainsKey(_ObjectRevisionID))
                                ObjectLocatorReference[_ObjectStream][_ObjectEdition][_ObjectRevisionID].MaxNumberOfCopies = value;

                throw new ArgumentException("Could not set MaxNumberOfCopies!");

            }

        }

        #endregion

        #region ObjectCopies

        /// <summary>
        /// Stores all ObjectDatastreams
        /// </summary>
        [NotIFastSerialized]
        public IEnumerable<ObjectDatastream> ObjectCopies
        {
            get
            {

                if (ObjectLocatorReference != null)
                    if (ObjectLocatorReference.ContainsKey(_ObjectStream))
                        if (ObjectLocatorReference[_ObjectStream].ContainsKey(_ObjectEdition))
                            if (ObjectLocatorReference[_ObjectStream][_ObjectEdition].ContainsKey(_ObjectRevisionID))
                                return ObjectLocatorReference[_ObjectStream][_ObjectEdition][_ObjectRevisionID].ToList();

                return new List<ObjectDatastream>();

            }
        }

        #endregion


        #region ObjectSize

        [NonSerialized]
        protected UInt64 _ObjectSize;

        /// <summary>
        /// The payload size of this APandoraObject.
        /// </summary>
        [NotIFastSerialized]
        public UInt64 ObjectSize
        {
            get
            {
                return _ObjectSize;
            }
        }

        #endregion

        #region ObjectSizeOnDisc

        [NonSerialized]
        protected UInt64 _ObjectSizeOnDisc;

        /// <summary>
        /// The payload size of this APandoraObject.
        /// </summary>
        [NotIFastSerialized]
        public UInt64 ObjectSizeOnDisc
        {
            get
            {
                return _ObjectSizeOnDisc;
            }
        }

        #endregion

        #endregion


        #region CloneObjectOntology(myAObjectOntology)

        public void CloneObjectOntology(AObjectOntology myAObjectOntology)
        {

            // Members of APandoraStructure
            _ObjectLocatorReference     = myAObjectOntology.ObjectLocatorReference;
            _INodeReference             = myAObjectOntology.INodeReference;

            ObjectUUID                  = myAObjectOntology.ObjectUUID;

            // Members of IObjectLocation
            _ObjectLocation             = myAObjectOntology.ObjectLocation;
            _ObjectPath                 = myAObjectOntology.ObjectPath;
            _ObjectName                 = myAObjectOntology.ObjectName;

            // Members of IObjectOntology
            _ObjectStream               = myAObjectOntology.ObjectStream;
            _ObjectEdition              = myAObjectOntology.ObjectEdition;
            _ObjectRevisionID           = myAObjectOntology.ObjectRevision;

            _ObjectSize                 = myAObjectOntology.ObjectSize;
            _ObjectSizeOnDisc           = myAObjectOntology.ObjectSizeOnDisc;

        }

        #endregion


    }

}
