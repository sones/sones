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

/* <id name="sones GraphDB – PandoraType" />
 * <copyright file="PandoraType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <refactored>Achim 'ahzf' Friedland</refactored>
 * <refactored>Stefan Licht</refactored>
 * <refactored>Henning Rauch</refactored>
 * <developed>Henning Rauch</developed>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.Indices;
using sones.Lib.DataStructures.UUID;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;
using sones.Lib.Serializer;
using System.Diagnostics;

#endregion

namespace sones.GraphDB.TypeManagement
{

    public class GraphDBType : AFSObject, IComparable, IGetName
    {

        #region Data

        private Boolean isNew = true;
        private GraphDBType _ParentType = null;

        #endregion

        #region Properties

        #region ID

        private TypeUUID _UUID;

        public TypeUUID UUID
        {
            get
            {
                return _UUID;
            }
        }

        #endregion

        #region Name

        /// <summary>
        /// The name of the class.
        /// </summary>
        public String Name
        {
            get
            {
                return ObjectName;
            }
        }

        #endregion

        #region ParentType

        /// <summary>
        /// The parent type of this PandoraType
        /// </summary>
        public TypeUUID ParentTypeUUID { get; private set; }

        #endregion

        #region Attributes

        private Dictionary<String, ADBSettingsBase>         _TypeSettings;
        private List<AttributeUUID>                         _UniqueAttributes;
        private HashSet<AttributeUUID>                      _MandatoryAttributes;
        private HashSet<AttributeUUID>                      _MandatoryParentAttributes;

        /// <summary>
        /// All attributes which are explicitly defined by the user but just show the implicit backwardedge
        /// </summary>
        private Dictionary<EdgeKey, AttributeUUID>          _BackwardEdgeAttributes;
        
        #endregion

        #region Attributes

        [NotFastSerializable]
        private Dictionary<AttributeUUID, TypeAttribute> _Attributes;

        /// <summary>
        /// The myAttributes of this. &lt;ID, Attribute&gt;
        /// DO NOT call add, remove on this! Use AddAttribute, RemoveAttribute instead!
        /// </summary>
        [NotFastSerializable]
        public Dictionary<AttributeUUID, TypeAttribute> Attributes
        {
            get
            {
                return _Attributes;
            }
        }

        #endregion

        #region attribute lookup table

        private Dictionary<AttributeUUID, TypeAttribute> _TypeAttributeLookupTable;

        /// <summary>
        /// Lookup table of all attributes of the current type including all parent attributes.
        /// </summary>
        [NotFastSerializable]
        public Dictionary<AttributeUUID, TypeAttribute> AttributeLookupTable
        {
            get
            {
                return _TypeAttributeLookupTable;
            }
        }

        #endregion

        #region AttributeIndices

        /// <summary>
        /// This is a lookupt for attributename and the corresponding KeyDefinition
        /// </summary>
        private Dictionary<String, IndexKeyDefinition> _AttributeIndicesNameLookup;

        /// <summary>
        /// Contains a list of all indexes for this type. Index is a list of attribute names over 
        /// which the index is built. 
        /// &lt;ID, &lt;Edition, Index&gt; &gt;
        /// </summary>
        private Dictionary<IndexKeyDefinition, Dictionary<String, AAttributeIndex>> _AttributeIndices;

        /// <summary>
        /// Contains a list of all indexes for this type. Index is a list of attribute names over 
        /// which the index is built. 
        /// &lt;ID, &lt;Edition, Index&gt; &gt;
        /// </summary>
        public Dictionary<IndexKeyDefinition, Dictionary<String, AAttributeIndex>> AttributeIndices
        {
            get
            {
                return _AttributeIndices;
            }
            set
            {
                // HACK: remove this as soon as the _AttributeIndices are flushed with the Type
                _AttributeIndices = value;
            }
        }

        #endregion

        #region IsUserDefined

        private Boolean _IsUserDefined;

        /// <summary>
        /// Is true, if this type was defined by the user. Else, false.
        /// </summary>
        public Boolean IsUserDefined
        {            
            get
            {
                return _IsUserDefined;
            }
        }

        #endregion

        #region IsAbstract

        private Boolean _IsAbstract;

        /// <summary>
        /// Is true, if this type is defined as abstract.
        /// </summary>
        public Boolean IsAbstract
        {
            get
            {
                return _IsAbstract;
            }
        }

        #endregion

        #region IsBackwardEdge

        public Boolean IsBackwardEdge
        {
            get 
            {
                return DBBackwardEdgeType.UUID == _UUID;
            }
        }

        #endregion

        #region Comment

        private String _Comment = String.Empty;

        /// <summary>
        /// A comment for the type
        /// </summary>
        public String Comment
        {
            get
            {
                return _Comment;
            }
        }

        #endregion

        #endregion

        #region Constructor

        #region GraphDBType()

        /// <summary>
        /// This will create an empty PandoraType
        /// </summary>
        public GraphDBType()
        {

            // Members of APandoraStructure
            _StructureVersion       = 1;

            // Members of APandoraObject
            _ObjectStream           = DBConstants.DBTYPESTREAM;

            // Object specific data...
            //_IndexHashTable         = new Dictionary<String, DirectoryEntry>();

            // Set ObjectUUID
            if (ObjectUUID.Length == 0)
                ObjectUUID = ObjectUUID.NewUUID;

        }

        #endregion

        #region GraphDBType(myObjectLocation)

        /// <summary>
        /// This will create an em
        /// </summary>
        /// <param name="myObjectLocation">the location (object myPath and Name) of the requested PandoraType within the file system</param>
        public GraphDBType(ObjectLocation myObjectLocation)
            : this()
        {

            if (myObjectLocation == null || myObjectLocation.Length < FSPathConstants.PathDelimiter.Length)
                throw new ArgumentNullException("Invalid ObjectLocation!");

            // Set the property in order to automagically set the
            // ObjectPath and ObjectName
            ObjectLocation = myObjectLocation;
        }

        #endregion

        #region GraphDBType(myIGraphFS2Session, myObjectLocation, myIsUserDefined)

        /// <summary>
        /// This will create an em
        /// </summary>
        /// <param name="myIPandoraFS"></param>
        /// <param name="myObjectLocation">the location (object myPath and Name) of the requested PandoraType within the file system</param>
        public GraphDBType(IGraphFSSession myIGraphFSSession, ObjectLocation myObjectLocation, Boolean myIsUserDefined, Boolean myIsAbstract)
            : this()
        {

            if (myObjectLocation == null || myObjectLocation.Length < FSPathConstants.PathDelimiter.Length)
                throw new ArgumentNullException("Invalid ObjectLocation!");


            ParentTypeUUID              = new TypeUUID(0);
            _Attributes                 = new Dictionary<AttributeUUID,TypeAttribute>();
            _AttributeIndices           = new Dictionary<IndexKeyDefinition, Dictionary<string,AAttributeIndex>>();
            _AttributeIndicesNameLookup = new Dictionary<String, IndexKeyDefinition>();
            _TypeSettings               = new Dictionary<string, ADBSettingsBase>();
            _TypeAttributeLookupTable   = new Dictionary<AttributeUUID,TypeAttribute>();
            _BackwardEdgeAttributes     = new Dictionary<EdgeKey,AttributeUUID>();
            _MandatoryAttributes        = new HashSet<AttributeUUID>();
            _UniqueAttributes           = new List<AttributeUUID>();

            LoadPandoraType(myIGraphFSSession, myObjectLocation);

            _IsUserDefined = myIsUserDefined;
            _IsAbstract = myIsAbstract;

        }

        #endregion

        #region GraphDBType(myObjectLocation, myTypeName, myParentType, myAttributes, myIsListType, myIsUserDefined, myIsAbstract)

        public GraphDBType(TypeUUID myUUID, ObjectLocation myDBRootPath, String myTypeName, TypeUUID myParentType, Dictionary<AttributeUUID, TypeAttribute> myAttributes, Boolean myIsUserDefined, Boolean myIsAbstract, String myComment)
            : this(myDBRootPath + myTypeName)
        {

            // TypeManager typeMan = TypeManager.GetInstance();
            

            // PandoraType is the most abstract type, not inheriting from any type.
            // Any type defineable is a Pandora type.
            // Set the parent type
            ParentTypeUUID                 = myParentType;

            // PandoraType is the most abstract type.
            // It doesnt contain any special myAttributes.
            // Set the myAttributes
            _Attributes                 = myAttributes;
            _AttributeIndices           = new Dictionary<IndexKeyDefinition, Dictionary<String, AAttributeIndex>>();
            _AttributeIndicesNameLookup = new Dictionary<String, IndexKeyDefinition>();
            _MandatoryAttributes        = new HashSet<AttributeUUID>();
            _UniqueAttributes           = new List<AttributeUUID>();
            _IsUserDefined              = myIsUserDefined;
            _IsAbstract                 = myIsAbstract;
            _Comment                    = myComment;

            _BackwardEdgeAttributes     = new Dictionary<EdgeKey,AttributeUUID>();
            _TypeAttributeLookupTable   = new Dictionary<AttributeUUID, TypeAttribute>();

            if (myAttributes != null)
            {
                foreach (var attr in _Attributes.Values)
                    _TypeAttributeLookupTable.Add(attr.UUID, attr);
            }

            _TypeSettings               = new Dictionary<String, ADBSettingsBase>();

            _UUID = myUUID;

        }

        #endregion

        #endregion

        
        #region private helper methods

        #region GetParentMandatoryAttr(myDBTypeManager)

        private HashSet<AttributeUUID> GetParentMandatoryAttr(DBTypeManager myDBTypeManager)
        {

            Debug.Assert(myDBTypeManager != null);

            var _ParentGraphDBTypes = myDBTypeManager.GetAllParentTypes(this, false, false);

            if (_MandatoryParentAttributes == null)
                _MandatoryParentAttributes = new HashSet<AttributeUUID>();

            _MandatoryParentAttributes.Clear();

            foreach (var Type in _ParentGraphDBTypes)
            {
                foreach (var attribID in Type.GetMandatoryAttributesUUIDs(myDBTypeManager))
                {
                    _MandatoryParentAttributes.Add(attribID);
                }
            }

            return _MandatoryParentAttributes;

        }

        #endregion

        #region AddAttributeIndex(myAttributeIndex)

        /// <summary>
        /// Creates an index for the given myAttributes by filling the given the index with the objects
        /// of this type that are already stored.</summary>
        /// <param name="myAttributeNames">The names of the myAttributes, over which the index was created.</param>
        private Exceptional<Boolean> AddAttributeIndex(AAttributeIndex myAttributeIndex)
        {

            // Add this index to the list of indices of this PandoraType
            lock (_AttributeIndices)
            {

                if (!_AttributeIndices.ContainsKey(myAttributeIndex.IndexKeyDefinition))
                {

                    #region New IndexKeyDefinition

                    #region Check if the IndexName already exist - same name and different keyDefinitions are not allowed!

                    if (_AttributeIndicesNameLookup.ContainsKey(myAttributeIndex.IndexName))
                    {
                        return new Exceptional<Boolean>(new Error_IndexAlreadyExist(myAttributeIndex.IndexName));
                    }

                    #endregion

                    #region Add Index (IndexKeyDefinition, IndexEdition)

                    _AttributeIndices.Add(myAttributeIndex.IndexKeyDefinition, new Dictionary<string, AAttributeIndex>());
                    _AttributeIndices[myAttributeIndex.IndexKeyDefinition].Add(myAttributeIndex.IndexEdition, myAttributeIndex);

                    _AttributeIndicesNameLookup.Add(myAttributeIndex.IndexName, myAttributeIndex.IndexKeyDefinition);

                    #endregion

                    #endregion

                }

                else if (_AttributeIndices[myAttributeIndex.IndexKeyDefinition].ContainsKey(myAttributeIndex.IndexEdition)) 
                {

                    #region IndexKeyDefinition and Edition already exist

                    return new Exceptional<Boolean>(new Error_IndexAlreadyExist(_AttributeIndices[myAttributeIndex.IndexKeyDefinition][myAttributeIndex.IndexEdition].IndexName));

                    #endregion

                }

                else
                {

                    #region Existing IndexKeyDefinition but different edition

                    #region If the IndexName does not exist, add it - different names for same keyDefinition and DIFFERENT Edition are allowed

                    if (!_AttributeIndicesNameLookup.ContainsKey(myAttributeIndex.IndexName))
                    {
                        _AttributeIndicesNameLookup.Add(myAttributeIndex.IndexName, myAttributeIndex.IndexKeyDefinition);
                    }

                    #endregion

                    #region Add Index (IndexEdition)

                    _AttributeIndices[myAttributeIndex.IndexKeyDefinition].Add(myAttributeIndex.IndexEdition, myAttributeIndex);

                    #endregion

                    #endregion

                }

            }

            return new Exceptional<Boolean>(true);

        }

        #endregion

        #region FindAttributeInLookup(myTypeName, ref myAttributeUUID)

        private TypeAttribute FindAttributeInLookup(String myTypeName, ref AttributeUUID myAttributeUUID)
        {

            foreach (var attr in _TypeAttributeLookupTable)
            {
                if (attr.Value.Name == myTypeName)
                {
                    myAttributeUUID = attr.Key;
                    return attr.Value;
                }
            }

            return null;

        }

        #endregion

        #region LoadPandoraType(myIGraphFSSession, myObjectLocation)

        private void LoadPandoraType(IGraphFSSession myIGraphFSSession, ObjectLocation myObjectLocation)
        {


            if (myIGraphFSSession.ObjectExists(myObjectLocation).Value != Trinary.TRUE)
                throw new GraphDBException(new Error_DatabaseNotFound(myObjectLocation));

            if (myIGraphFSSession.UserMetadataExists(new ObjectLocation(myObjectLocation, DBConstants.DBTypeDefinition), "Name").Value != Trinary.TRUE)
                throw new GraphDBException(new Error_DatabaseNotFound(myObjectLocation));

            if (myIGraphFSSession.UserMetadataExists(new ObjectLocation(myObjectLocation, DBConstants.DBTypeDefinition), "Superclass").Value != Trinary.TRUE)
                throw new GraphDBException(new Error_DatabaseNotFound(myObjectLocation));

            ParentTypeUUID = (TypeUUID)myIGraphFSSession.GetUserMetadatum(new ObjectLocation(myObjectLocation, DBConstants.DBTypeDefinition), "Superclass").Value.First<Object>();
            _UUID = (TypeUUID)myIGraphFSSession.GetUserMetadatum(new ObjectLocation(myObjectLocation, DBConstants.DBTypeDefinition), "UUID").Value.First<Object>();


            #region Load attributes and settings

            // Attributes
            var _AttributeList = myIGraphFSSession.GetUserMetadata(new ObjectLocation(myObjectLocation, DBConstants.DBAttributesLocation));

            if (_AttributeList != null && _AttributeList.Success && _AttributeList.Value != null)
            {
                foreach (var _KeyValuePair in _AttributeList.Value)
                {

                    var _TypeAttribute = (TypeAttribute)_KeyValuePair.Value;

                    _Attributes.Add(_TypeAttribute.UUID, _TypeAttribute);
                    _TypeAttributeLookupTable.Add(_TypeAttribute.UUID, _TypeAttribute);

                }
            }

            // Settings
            var _SettingList = myIGraphFSSession.GetUserMetadata(new ObjectLocation(myObjectLocation, DBConstants.DBSettingsLocation));

            if (_SettingList != null && _SettingList.Success && _SettingList.Value != null)
            {
                foreach (var _KeyValuePair in _SettingList.Value)
                {
                    var _ADBSettingsBase = (ADBSettingsBase)_KeyValuePair.Value;
                    _TypeSettings.Add(_ADBSettingsBase.Name, _ADBSettingsBase);
                }
            }

            #endregion

            #region Load the list of Indices

            // ahzf: This looks strange!
            if (myIGraphFSSession.ObjectStreamExists(new ObjectLocation(myObjectLocation, DBConstants.DBIndicesLocation), FSConstants.DIRECTORYSTREAM).Value == Trinary.TRUE)
                _AttributeIndices = new Dictionary<IndexKeyDefinition, Dictionary<String, AAttributeIndex>>();

            else
                Console.WriteLine("No Indices for database type '" + Name + "' found!");

            #endregion


            // Set the property in order to automagically set the
            // ObjectPath and ObjectName
            ObjectLocation = myObjectLocation;

        }

        #endregion

        #region GetAttributes(myDBContext)

        private IEnumerable<TypeAttribute> GetAttributes(DBContext myDBContext)
        {

            foreach (var attrib in Attributes)
            {
                if (attrib.Value is ASpecialTypeAttribute)
                {
                    if ((bool)myDBContext.DBSettingsManager.GetSettingValue((attrib.Value as ASpecialTypeAttribute).ShowSettingName, myDBContext, TypesSettingScope.ATTRIBUTE, this, (attrib.Value as TypeAttribute)).Value.Value)
                    {
                        continue;
                    }
                }

                yield return attrib.Value;
            }

            yield break;

        }

        #endregion

        #endregion

        #region public methods

        #region Index

        public Exceptional CreateUUIDIndex(DBContext _DBContext, AttributeUUID myUUID)
        {
            var _NewUUIDIndex = new UUIDIndex(SpecialTypeAttribute_UUID.AttributeName, new IndexKeyDefinition(myUUID), this);

            return new Exceptional(AddAttributeIndex(_NewUUIDIndex));
        }

        #region GetAttributeIndex methods

        /// <summary>
        /// Returns the default edition index of the given attribute. If there is more than one (or empty) Edition then return the first.
        /// At some time, we could change this to take statistical information to get the best index
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute we want an index for.</param>
        /// <returns>The index for the given myAttributes if one exist. Else, null.</returns>
        public AAttributeIndex GetDefaultAttributeIndex(AttributeUUID myAttributeName)
        {

            return GetAttributeIndex(myAttributeName, DBConstants.DEFAULTINDEX).Value;

        }

        /// <summary>
        /// Returns the index of the given attribute
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute we want an index for.</param>
        /// <param name="myIndexEdition">The name of the index edition. May be null</param>
        /// <returns>The index for the given myAttributes if one exist. Else, null.</returns>
        public AAttributeIndex GetAttributeIndex(List<AttributeUUID> myAttributeNames, String myIndexEdition)
        {
            IndexKeyDefinition idxKey = new IndexKeyDefinition(myAttributeNames);
            if (_AttributeIndices.ContainsKey(idxKey) && _AttributeIndices[idxKey].ContainsKey(myIndexEdition))
                return _AttributeIndices[idxKey][myIndexEdition];

            throw new GraphDBException(new Error_IndexDoesNotExist(myAttributeNames.ToAggregatedString(), myIndexEdition));

        }

        /// <summary>
        /// Returns the index of the given attribute
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute we want an index for.</param>
        /// <param name="myIndexEdition">THe name of the index edition. May be null</param>
        /// <returns>The index for the given myAttributes if one exist. Else, null.</returns>
        public AAttributeIndex GetAttributeIndex(String myIndexName, String myIndexEdition)
        {

            if (_AttributeIndicesNameLookup.ContainsKey(myIndexName) && _AttributeIndices[_AttributeIndicesNameLookup[myIndexName]].ContainsKey(myIndexEdition))
                return _AttributeIndices[_AttributeIndicesNameLookup[myIndexName]][myIndexEdition];

            throw new GraphDBException(new Error_IndexDoesNotExist(myIndexName, myIndexEdition));

        }

        #region GetAttributeIndex(IndexKeyDefinition idxKey, string edition)

        /// <summary>
        /// Returns the attribute index with the specified key and edition.
        /// </summary>
        /// <param name="idxKey">The index key.</param>
        /// <param name="edition">The index edition.</param>
        /// <returns>The index or null if no index was found.</returns>
        private AAttributeIndex GetAttributeIndex(IndexKeyDefinition idxKey, string edition)
        {
            if (_AttributeIndices.ContainsKey(idxKey) && _AttributeIndices[idxKey].ContainsKey(DBConstants.UNIQUEATTRIBUTESINDEX))
            {
                return _AttributeIndices[idxKey][DBConstants.UNIQUEATTRIBUTESINDEX];
            }
            return null;
        }

        #endregion

        /// <summary>
        /// Returns the index of the given attribute
        /// </summary>
        /// <param name="myAttributeUUID">The name of the attribute we want an index for.</param>
        /// <param name="myIndexEdition">THe name of the index edition. May be null</param>
        /// <returns>The index for the given myAttributes if one exist. Else, null.</returns>
        public Exceptional<AAttributeIndex> GetAttributeIndex(AttributeUUID myAttributeUUID, String myIndexEdition)
        {
            IndexKeyDefinition idxKey = new IndexKeyDefinition(new List<AttributeUUID>() { myAttributeUUID });

            lock (_AttributeIndices)
            {

                if (_AttributeIndices.ContainsKey(idxKey))
                {
                    if (myIndexEdition == null)
                    {
                        if (_AttributeIndices[idxKey].ContainsKey(DBConstants.DEFAULTINDEX))
                            return new Exceptional<AAttributeIndex>(_AttributeIndices[idxKey][DBConstants.DEFAULTINDEX]);
                        else
                            return new Exceptional<AAttributeIndex>(_AttributeIndices[idxKey].First().Value);
                    }
                    else if (_AttributeIndices[idxKey].ContainsKey(myIndexEdition))
                    {
                        return new Exceptional<AAttributeIndex>(_AttributeIndices[idxKey][myIndexEdition]);
                    }
                }
            }

            return new Exceptional<AAttributeIndex>(new Error_IndexAttributeDoesNotExist(GetTypeAttributeByUUID(myAttributeUUID).Name));
        }

        #endregion

        /// <summary>
        /// Removes the given index from this type.
        /// </summary>
        /// <param name="myIndexName">The name of index.</param>
        public Exceptional<Boolean> RemoveIndex(String myIndexName, String myIndexEdition, DBTypeManager myTypeManager)
        {

            myIndexEdition = myIndexEdition ?? DBConstants.DEFAULTINDEX;

            foreach (var aidx in GetAllAttributeIndices())
            {
                if (aidx.IndexName.ToLower() == myIndexName.ToLower() && _AttributeIndices[aidx.IndexKeyDefinition].ContainsKey(myIndexEdition))
                {
                    _AttributeIndices[aidx.IndexKeyDefinition].Remove(aidx.IndexEdition);
                    _AttributeIndicesNameLookup.Remove(aidx.IndexName);

                    if (_AttributeIndices[aidx.IndexKeyDefinition].Count == 0)
                        _AttributeIndices.Remove(aidx.IndexKeyDefinition);

                    var FlushExcept = myTypeManager.FlushType(this);

                    if (FlushExcept.Failed)
                        return new Exceptional<Boolean>(FlushExcept);

                    return new Exceptional<Boolean>(true);
                }
            }

            return new Exceptional<Boolean>(new Error_IndexDoesNotExist(myIndexName, myIndexEdition));
        }

        #region CreateUniqueAttributeIndex(myIndexName, myAttributeName, myIndexEdition, myIndexObjectTypes, myFileSystemLocation)

        /// <summary>
        /// Creates an index for the given myAttribute by filling the given the index with the objects
        /// of this type that are already stored.</summary>
        /// <param name="myAttributeNames">The names of the myAttributes, over which the index was created.</param>
        public Exceptional<AttributeIndex> CreateUniqueAttributeIndex(DBContext myDBContext, String myIndexName, AttributeUUID myAttributeName, String myIndexEdition)
        {
            return CreateUniqueAttributeIndex(myDBContext, myIndexName, new List<AttributeUUID> { myAttributeName }, myIndexEdition);
        }

        #endregion

        #region CreateUniqueAttributeIndex(myIndexName, myAttributeUUIDs, myIndexEdition, myIndexObjectTypes, myFileSystemLocation)

        /// <summary>
        /// Creates an index for the given myAttribute by filling the given the index with the objects
        /// of this type that are already stored.</summary>
        /// <param name="myAttributeNames">The names of the myAttributes, over which the index was created.</param>
        public Exceptional<AttributeIndex> CreateUniqueAttributeIndex(DBContext myDBContext, String myIndexName, List<AttributeUUID> myAttributeUUIDs, String myIndexEdition)
        {
            // change this to a index type with a single value
            return CreateAttributeIndex(myDBContext, myIndexName, myAttributeUUIDs, myIndexEdition, VersionedHashIndexObject.Name);
        }

        #endregion

        #region CreateAttributeIndex(myIndexName, myAttributeName, myIndexEdition, myIndexObjectTypes, myFileSystemLocation)

        /// <summary>
        /// Creates an index for the given myAttribute by filling the given the index with the objects
        /// of this type that are already stored.</summary>
        /// <param name="myAttributeNames">The names of the myAttributes, over which the index was created.</param>
        public Exceptional<AttributeIndex> CreateAttributeIndex(DBContext myDBContext, String myIndexName, AttributeUUID myAttributeName, String myIndexEdition, String myIndexType = null)
        {
            return CreateAttributeIndex(myDBContext, myIndexName, new List<AttributeUUID> { myAttributeName }, myIndexEdition, myIndexType);
        }

        #endregion

        #region CreateAttributeIndex(myIndexName, myAttributeNames, myIndexEdition, myIndexObjectTypes, myFileSystemLocation)

        /// <summary>
        /// Create a new Index
        /// </summary>
        /// <param name="myIndexName"></param>
        /// <param name="myAttributeUUIDs"></param>
        /// <param name="myIndexEdition"></param>
        /// <param name="myIndexType">The index type name</param>
        /// <param name="myFileSystemLocation"></param>
        /// <returns></returns>
        public Exceptional<AttributeIndex> CreateAttributeIndex(DBContext myDBContext, String myIndexName, List<AttributeUUID> myAttributeUUIDs, String myIndexEdition, String myIndexType = null)
        {

            if (!String.IsNullOrEmpty(myIndexType))
            {

                #region Verify IndexType

                if (!myDBContext.DBPluginManager.HasIndex(myIndexType))
                {
                    return new Exceptional<AttributeIndex>(new Error_IndexTypeDoesNotExist(myIndexType));
                }

                #endregion

            }

            var _NewAttributeIndex = new AttributeIndex(myIndexName, myIndexEdition, myAttributeUUIDs, this, myIndexType);

            var CreateExcept = AddAttributeIndex(_NewAttributeIndex);

            if (CreateExcept.Failed)
            {
                return new Exceptional<AttributeIndex>(CreateExcept);
            }

            return new Exceptional<AttributeIndex>(_NewAttributeIndex);
        }

        #endregion

        /// <summary>
        /// Returns the DEFAULT Guid index
        /// </summary>
        /// <returns>The default guid index.</returns>
        public UUIDIndex GetUUIDIndex(DBTypeManager myTypeManager)
        {
            return (UUIDIndex)GetDefaultAttributeIndex(myTypeManager.GetUUIDTypeAttribute().UUID);
        }

        #endregion

        #region Attributes

        public void RemoveAttributeFromLookupTable(Dictionary<AttributeUUID, TypeAttribute> moreAttributes)
        {
            foreach (var aMoreAttribute in moreAttributes)
            {
                RemoveAttributeFromLookupTable(aMoreAttribute.Key);
            }
        }

        public void RemoveAttributeFromLookupTable(AttributeUUID attributeUUID)
        {
            _TypeAttributeLookupTable.Remove(attributeUUID);
        }

        public void AddAttributeToLookupTable(AttributeUUID myUUID, TypeAttribute myTypeAttribute)
        {
            _TypeAttributeLookupTable.Add(myUUID, myTypeAttribute);
        }

        public void AddAttributeToLookupTable(Dictionary<AttributeUUID, TypeAttribute> moreAttributes)
        {
            foreach (var aMoreAttribute in moreAttributes)
            {
                AddAttributeToLookupTable(aMoreAttribute.Key, aMoreAttribute.Value);
            }
        }

        /// <summary>
        /// This method gets a TypeAttribute by its UUID
        /// </summary>
        /// <param name="myAttributeUUID">The refered AttributeUUID of the attribute.</param>
        /// <returns>The TypeAttribute, else null.</returns>
        public TypeAttribute GetTypeAttributeByUUID(AttributeUUID myAttributeUUID)
        {

            TypeAttribute result = null;

            _TypeAttributeLookupTable.TryGetValue(myAttributeUUID, out result);

            return result;

        }

        /// <summary>
        /// This method tries to get a TypeAttribute by its Name
        /// </summary>
        /// <param name="myAttributeName">The Name of the Attribute.</param>
        /// <returns>The TypeAttribute, else null.</returns>
        public TypeAttribute GetTypeAttributeByName(String myAttributeName)
        {
            var retval = (from aAttrDef in _TypeAttributeLookupTable where aAttrDef.Value.Name == myAttributeName select aAttrDef.Value).FirstOrDefault();

            return retval;
        }

        public IEnumerable<TypeAttribute> GetSpecificAttributes(Predicate<TypeAttribute> predicate)
        {
            foreach (var attr in _Attributes)
            {
                if (predicate(attr.Value))
                {
                    yield return attr.Value;
                }
            }
        }

        /// <summary>
        /// return the attribute which relates to type and not his superclass
        /// </summary>
        /// <param name="myAttributeName">name of attribute</param>
        /// <returns></returns>
        public TypeAttribute GetTypeSpecificAttributeByName(string myAttributeName)
        {
            return _Attributes.FirstOrDefault(item => item.Value.Name == myAttributeName).Value;
        }

        /// <summary>
        /// return the attribute which relates to type and not his superclass
        /// </summary>
        /// <param name="myAttributeUUID">uuid of the attribute</param>
        /// <returns></returns>
        public TypeAttribute GetTypeSpecificAttributeByUUID(AttributeUUID myAttributeUUID)
        {
            return _Attributes.FirstOrDefault(item => item.Value.UUID == myAttributeUUID).Value;
        }

        public IEnumerable<AAttributeIndex> GetAllAttributeIndices(Boolean includeUUIDIndices = true)
        {

            foreach (var attrIndices in _AttributeIndices)
            {
                foreach (var attrIndex in attrIndices.Value)
                {
                    if (!(attrIndex.Value is UUIDIndex) || includeUUIDIndices)
                    {
                        yield return attrIndex.Value;
                    }
                }
            }

            yield break;
        }

        public Boolean HasAttributeIndices(List<AttributeUUID> myAttributeUUIDs)
        {
            return _AttributeIndices.ContainsKey(new IndexKeyDefinition(myAttributeUUIDs));
        }

        public Boolean HasAttributeIndices(AttributeUUID myAttributeUUID)
        {
            return _AttributeIndices.ContainsKey(new IndexKeyDefinition(new List<AttributeUUID>() { myAttributeUUID }));
        }

        public Boolean HasAttributeIndices(IndexKeyDefinition myIndexDefinition)
        {
            return _AttributeIndices.ContainsKey(myIndexDefinition);
        }

        public List<AAttributeIndex> GetAttributeIndices(IndexKeyDefinition IndexName)
        {
            if (_AttributeIndices.ContainsKey(IndexName))
                return new List<AAttributeIndex>(_AttributeIndices[IndexName].Values);

            return null;
        }

        public Exceptional<Boolean> RenameAttribute(AttributeUUID attributeUUID, string newName)
        {
            if (GetTypeSpecificAttributeByName(newName) != null)
                return new Exceptional<Boolean>(new Error_AttributeAlreadyExists(newName));

            _Attributes[attributeUUID].Name = newName;

            return new Exceptional<Boolean>(true);
        }

        public Exceptional<Boolean> RenameBackwardedge(TypeAttribute myBackwardEdge, string newName, DBTypeManager myTypeManager)
        {

            if (myBackwardEdge == null)
                return new Exceptional<Boolean>(new Error_ArgumentNullOrEmpty("myBackwardEdge"));

            if (String.IsNullOrEmpty(newName))
                return new Exceptional<Boolean>(new Error_ArgumentNullOrEmpty("newName"));

            if (GetTypeAttributeByName(newName) != null)
                return new Exceptional<Boolean>(new Error_AttributeAlreadyExists(newName));

            if (!myBackwardEdge.IsBackwardEdge)
                return new Exceptional<Boolean>(new Error_InvalidEdgeType(new Type[] { typeof(DBBackwardEdgeType) }));

            return myTypeManager.RenameAttributeOfType(this, myBackwardEdge.Name, newName);
        }

        public void AddBackwardEdgeAttribute(TypeAttribute ta)
        {
            if (ta.IsBackwardEdge)
                _BackwardEdgeAttributes.Add(ta.BackwardEdgeDefinition, ta.UUID);
        }

        public void RemoveBackwardEdgeAttribute(TypeAttribute ta)
        {
            if (ta.IsBackwardEdge && _BackwardEdgeAttributes.ContainsKey(ta.BackwardEdgeDefinition))
                _BackwardEdgeAttributes.Remove(ta.BackwardEdgeDefinition);
        }

        public Boolean IsBackwardEdgeAttribute(EdgeKey myEdgeKey)
        {
            return _BackwardEdgeAttributes.ContainsKey(myEdgeKey);
        }

        public AttributeUUID GetBackwardEdgeAttribute(EdgeKey myEdgeKey)
        {
            return _BackwardEdgeAttributes[myEdgeKey];
        }

        /// <summary>
        /// Returns all attribute of this type and all derived types
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TypeAttribute> GetAllAttributes(DBContext context)
        {
            foreach (var type in context.DBTypeManager.GetAllParentTypes(this, true, true))
            {
                foreach (var attrib in type.Attributes)
                {
                    if (attrib.Value is ASpecialTypeAttribute)
                    {
                        //check if there is an setting for this attribute
                        if (context.DBSettingsManager.HasSetting((attrib.Value as ASpecialTypeAttribute).ShowSettingName))
                        {
                            var settingVal = (IDBShowSetting)context.DBSettingsManager.GetSetting((attrib.Value as ASpecialTypeAttribute).ShowSettingName, context, TypesSettingScope.TYPE, this).Value;
                            if (settingVal == null || !settingVal.IsShown())
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                        
                    }
                    yield return attrib.Value;
                }
            }

            yield break;
        }

        public void AddAttribute(TypeAttribute myTypeAttribute)
        {
            _Attributes.Add(myTypeAttribute.UUID, myTypeAttribute);
            _TypeAttributeLookupTable.Add(myTypeAttribute.UUID, myTypeAttribute);
        }

        public void AddAttribute(AttributeUUID myUUID, TypeAttribute myTypeAttribute)
        {
            _Attributes.Add(myUUID, myTypeAttribute);
            _TypeAttributeLookupTable.Add(myUUID, myTypeAttribute);
        }

        public void RemoveAttribute(AttributeUUID myUUID)
        {
            _TypeAttributeLookupTable.Remove(myUUID);
            _Attributes.Remove(myUUID);
        }

        /// <summary>
        /// Returns all attribute of this type and all derived types
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TypeAttribute> GetAllAttributes(Predicate<TypeAttribute> predicate, DBContext context, Boolean includeParentTypes = true)
        {

            foreach (var attrib in GetAllAttributes(context))
            {
                if (predicate(attrib))
                {
                    yield return attrib;
                }
            }

            yield break;
        }

        #endregion

        #region setting

        /// <summary>
        /// Adds a setting to this type
        /// </summary>
        /// <param name="myName">The name of the setting</param>
        /// <param name="mySetting">The setting itself</param>
        /// <param name="myTypeManager">The DB type manager</param>
        /// <returns>A Result type</returns>
        public Exceptional SetPersistentSetting(string myName, ADBSettingsBase mySetting, DBTypeManager myTypeManager)
        {
            if (!_TypeSettings.ContainsKey(myName))
            {
                _TypeSettings.Add(myName, (ADBSettingsBase)mySetting.Clone());
            }
            else
            {
                _TypeSettings[myName] = (ADBSettingsBase)mySetting.Clone();
            }

            var FlushExcept = myTypeManager.FlushType(this);

            if (FlushExcept.Failed)
                return new Exceptional(FlushExcept);

            return Exceptional.OK;
        }

        /// <summary>
        /// Remove a setting from this type
        /// </summary>
        /// <param name="mySettingName">The name of the setting</param>
        /// <param name="myTypeManager">The DB type manager</param>
        /// <returns>A ResultType</returns>
        public Exceptional<bool> RemovePersistentSetting(string mySettingName, DBTypeManager myTypeManager)
        {
            _TypeSettings.Remove(mySettingName);

            var FlushExcept = myTypeManager.FlushType(this);

            if (FlushExcept.Failed)
            {
                return new Exceptional<bool>(FlushExcept);
            }

            return new Exceptional<bool>(true);
        }

        /// <summary>
        /// Returns a persistent setting of this type
        /// </summary>
        /// <param name="mySettingName">The name of the setting</param>
        /// <returns>A setting</returns>
        public ADBSettingsBase GetPersisitentSetting(String mySettingName)
        {
            if (_TypeSettings.ContainsKey(mySettingName))
            {
                return (ADBSettingsBase)_TypeSettings[mySettingName].Clone();
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, ADBSettingsBase> GetTypeSettings
        {
            get { return _TypeSettings; }
        }

        #endregion

        #region mandatory

        /// <summary>
        /// add an mandatory attribute to type
        /// </summary>
        /// <param name="myAttrib"></param>        
        public void AddMandatoryAttribute(AttributeUUID myAttribID, DBTypeManager myTypeManager)
        {
            List<GraphDBType> SubTypes = myTypeManager.GetAllSubtypes(this, false);

            foreach (var Types in SubTypes)
            {
                Types.AddMandatoryAttribute(myAttribID, myTypeManager);
            }

            _MandatoryAttributes.Add(myAttribID);
        }

        /// <summary>
        /// remove a mandatory attribute
        /// </summary>
        /// <param name="myAttribID"></param>
        public void RemoveMandatoryAttribute(AttributeUUID myAttribID, DBTypeManager myTypeManager)
        {
            List<GraphDBType> SubTypes = myTypeManager.GetAllSubtypes(this, false);

            foreach (var Types in SubTypes)
            {
                Types.RemoveMandatoryAttribute(myAttribID, myTypeManager);
            }

            _MandatoryAttributes.Remove(myAttribID);
        }

        public HashSet<AttributeUUID> GetMandatoryAttributesUUIDs(DBTypeManager myTypeManager)
        {
            foreach (var attribID in GetParentMandatoryAttr(myTypeManager))
                _MandatoryAttributes.Add(attribID);

            return _MandatoryAttributes;

            // TODO: Use this instead!!!
            /*
            foreach (var attribID in GetParentMandatoryAttr())
            {
                //_MandatoryAttributes.Add(attribID);
                yield return attribID;
            }
            foreach (var attribID in _MandatoryAttributes)
            {
                yield return attribID;
            }
            */
        }

        /// <summary>
        /// This is a really bad hack to get the mandatory attributes of THIS type!
        /// As soon as the mandatory behavior is fixed, this method SHOULD be changed
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AttributeUUID> GetMandatoryAttributes()
        {
            foreach (var attr in Attributes)
            {
                if (_MandatoryAttributes.Contains(attr.Key))
                {
                    yield return attr.Key;
                }
            }
        }

        public Exceptional<Boolean> DropMandatoryAttributes(DBTypeManager myTypeManager)
        {
            List<GraphDBType> SubTypes = myTypeManager.GetAllSubtypes(this, false);

            if (GetMandatoryAttributesUUIDs(myTypeManager) != null)
            {
                foreach (var type in SubTypes)
                {
                    foreach (var attrib in _MandatoryAttributes)
                    {
                        if (type._MandatoryAttributes.Contains(attrib))
                            type._MandatoryAttributes.Remove(attrib);
                    }
                }

                _MandatoryAttributes.Clear();

                var FlushExcept = myTypeManager.FlushType(this);

                if (FlushExcept.Failed)
                    return new Exceptional<Boolean>(FlushExcept);
            }

            return new Exceptional<Boolean>(true);
        }

        public Exceptional<Boolean> ChangeMandatoryAttributes(List<string> myAttribs, DBTypeManager myTypeManager)
        {
            TypeAttribute TypeAttr = null;

            DropMandatoryAttributes(myTypeManager);
            foreach (var attribs in myAttribs)
            {
                TypeAttr = GetTypeAttributeByName(attribs);
                if (TypeAttr == null)
                {
                    return new Exceptional<bool>(new Error_AttributeIsNotDefined(this.Name, attribs));
                }
                else
                {
                    AddMandatoryAttribute(TypeAttr.UUID, myTypeManager);
                    foreach (var attribID in GetParentMandatoryAttr(myTypeManager))
                    {
                        AddMandatoryAttribute(TypeAttr.UUID, myTypeManager);
                    }
                }
            }

            var FlushExcept = myTypeManager.FlushType(this);

            if (FlushExcept.Failed)
                return new Exceptional<Boolean>(FlushExcept);

            return new Exceptional<Boolean>(true);
        }

        #endregion

        #region unique

        /// <summary>
        /// add an unique attribute to type
        /// </summary>
        /// <param name="myAttribID"></param>
        public Exceptional<Boolean> AddUniqueAttributes(List<AttributeUUID> myAttribIDs, DBContext myDBContext)
        {
            if (!myAttribIDs.IsNullOrEmpty())
            {
                #region data

                AAttributeIndex AttribIndex = null;
                List<GraphDBType> SubTypes = myDBContext.DBTypeManager.GetAllSubtypes(this, false);
                IndexKeyDefinition idxKey = new IndexKeyDefinition(myAttribIDs);

                #endregion

                AttribIndex = GetAttributeIndex(idxKey, DBConstants.UNIQUEATTRIBUTESINDEX);

                if (!SubTypes.IsNullOrEmpty())
                {
                    string idxName = myDBContext.DBIndexManager.GetUniqueIndexName(myAttribIDs, this);

                    foreach (var aType in SubTypes)
                    {
                        foreach (var AttrID in myAttribIDs)
                        {
                            aType._UniqueAttributes.Add(AttrID);
                        }

                        if (AttribIndex != null)
                        {
                            var createIdxExcept = aType.CreateUniqueAttributeIndex(myDBContext, idxName, myAttribIDs, DBConstants.UNIQUEATTRIBUTESINDEX);

                            if (createIdxExcept.Failed)
                                return new Exceptional<Boolean>(createIdxExcept);
                        }
                    }
                }

                _UniqueAttributes.AddRange(myAttribIDs);
            }

            return new Exceptional<Boolean>(true);
        }

        /// <summary>
        /// remove an unique attribute
        /// </summary>
        /// <param name="myAttribID"></param>
        public Exceptional<Boolean> RemoveUniqueAttribute(AttributeUUID myAttribID, DBTypeManager myTypeManager)
        {
            List<GraphDBType> SubTypes = myTypeManager.GetAllSubtypes(this, false);
            List<AttributeUUID> AttrList = new List<AttributeUUID>();
            AttrList.Add(myAttribID);

            foreach (var Types in SubTypes)
            {
                Types._UniqueAttributes.Remove(myAttribID);
                var removeIdxExcept = Types.RemoveIndex(Types.GetAttributeIndex(AttrList, DBConstants.UNIQUEATTRIBUTESINDEX).IndexName, DBConstants.UNIQUEATTRIBUTESINDEX, myTypeManager);

                if (removeIdxExcept.Failed)
                    return new Exceptional<Boolean>(removeIdxExcept);
            }

            _UniqueAttributes.Remove(myAttribID);

            return new Exceptional<Boolean>(true);
        }

        public List<AttributeUUID> GetAllUniqueAttributes(Boolean includeCurrentType, DBTypeManager myTypeManager)
        {
            List<AttributeUUID> result = new List<AttributeUUID>();

            foreach (var aParentType in myTypeManager.GetAllParentTypes(this, includeCurrentType, false))
            {
                result.AddRange(aParentType._UniqueAttributes);
            }

            return result;
        }

        public IEnumerable<AttributeUUID> GetUniqueAttributes()
        {
            return _UniqueAttributes;
        }

        public Exceptional<ResultType> DropUniqueAttributes(DBTypeManager myTypeManager)
        {
            #region Remove old unique index and attributes

            var mayBeUniqueIdx = FindUniqueIndex();

            if (mayBeUniqueIdx != null)
            {
                var RemoveIdxExcept = RemoveIndex(mayBeUniqueIdx.IndexName, mayBeUniqueIdx.IndexEdition, myTypeManager);

                if (RemoveIdxExcept.Failed)
                {
                    return new Exceptional<ResultType>(RemoveIdxExcept);
                }

                foreach (var attrUUID in mayBeUniqueIdx.IndexKeyDefinition.IndexKeyAttributeUUIDs)
                {
                    foreach (var type in myTypeManager.GetAllSubtypes(this, false))
                    {
                        var RemoveUniqueExcept = type.RemoveUniqueAttribute(attrUUID, myTypeManager);

                        if (RemoveUniqueExcept.Failed)
                            return new Exceptional<ResultType>(RemoveUniqueExcept);
                    }
                }
                _UniqueAttributes.Clear();
            }

            #endregion

            var FlushExcept = myTypeManager.FlushType(this);

            if (FlushExcept.Failed)
                return new Exceptional<ResultType>(FlushExcept);

            return new Exceptional<ResultType>(ResultType.Successful);
        }

        public Exceptional<ResultType> ChangeUniqueAttributes(List<String> myAttributes, DBContext myDBContext)
        {

            List<AttributeUUID> attrUUIDs = new List<AttributeUUID>();

            #region Validate attributes

            foreach (var attr in myAttributes)
            {
                var typeAttr = GetTypeAttributeByName(attr);
                if (typeAttr == null)
                {
                    return new Exceptional<ResultType>(new Error_AttributeIsNotDefined(this.Name, attr));
                }
                attrUUIDs.Add(typeAttr.UUID);
            }

            #endregion

            #region Remove old unique index and attributes

            var mayBeUniqueIdx = FindUniqueIndex();

            if (mayBeUniqueIdx != null)
            {
                RemoveIndex(mayBeUniqueIdx.IndexName, mayBeUniqueIdx.IndexEdition, myDBContext.DBTypeManager);

                foreach (var attrUUID in mayBeUniqueIdx.IndexKeyDefinition.IndexKeyAttributeUUIDs)
                {
                    _UniqueAttributes.Add(attrUUID);
                }
            }

            #endregion

            String idxName = myDBContext.DBIndexManager.GetUniqueIndexName(attrUUIDs, this);
            var CreateIdxExcept = CreateUniqueAttributeIndex(myDBContext, idxName, attrUUIDs, DBConstants.UNIQUEATTRIBUTESINDEX);

            if (CreateIdxExcept.Failed)
                return new Exceptional<ResultType>(CreateIdxExcept);

            var rebuildResult = myDBContext.DBIndexManager.RebuildIndex(idxName, DBConstants.UNIQUEATTRIBUTESINDEX, this, IndexSetStrategy.UNIQUE);

            if (rebuildResult.Failed)
            {
                //add already removed idx
                if (mayBeUniqueIdx != null)
                {
                    AddAttributeIndex(mayBeUniqueIdx);
                }

                var RemoveIdxExcept = RemoveIndex(idxName, DBConstants.UNIQUEATTRIBUTESINDEX, myDBContext.DBTypeManager);

                if (RemoveIdxExcept.Failed)
                    return new Exceptional<ResultType>(RemoveIdxExcept);

                return rebuildResult;
            }

            #region Set the unique flag for the attributes

            List<AttributeUUID> ParentUniqueIDs = new List<AttributeUUID>();

            var AddUniqueAttrExcept = AddUniqueAttributes(ParentUniqueIDs, myDBContext);

            if (AddUniqueAttrExcept.Failed)
                return new Exceptional<ResultType>(AddUniqueAttrExcept);

            AddUniqueAttrExcept = AddUniqueAttributes(attrUUIDs, myDBContext);

            if (AddUniqueAttrExcept.Failed)
                return new Exceptional<ResultType>(AddUniqueAttrExcept);

            #endregion

            var FlushExcept = myDBContext.DBTypeManager.FlushType(this);

            if (FlushExcept.Failed)
                return new Exceptional<ResultType>(FlushExcept);

            return new Exceptional<ResultType>(ResultType.Successful);
        }

        public AAttributeIndex FindUniqueIndex()
        {
            foreach (var aIdx in this.AttributeIndices)
            {
                foreach (var aInnerIdx in aIdx.Value)
                {
                    if (aInnerIdx.Key == DBConstants.UNIQUEATTRIBUTESINDEX)
                    {
                        return aInnerIdx.Value;
                    }
                }
            }
            return null;
        }

        #endregion

        #region comment

        public void SetComment(String comment)
        {
            _Comment = comment;
        }

        #endregion

        #region misc

        public GraphDBType GetParentType(DBTypeManager myTypeManager)
        {
            if (_ParentType == null)
            {
                if (ParentTypeUUID == null)
                {
                    return null;
                }
                else
                {
                    _ParentType = myTypeManager.GetTypeByUUID(ParentTypeUUID);

                    return _ParentType;
                }
            }
            else
            {
                return _ParentType;
            }

        }

        public ObjectLocation GetObjectLocation(ObjectUUID objectUUID)
        {
            return (_ObjectLocation + DBConstants.DBObjectsLocation) + objectUUID.ToString();
        }

        public void SetParentTypeUUID(TypeUUID typeUUID)
        {
            ParentTypeUUID = typeUUID;
        }

        #endregion

        #endregion

        #region (internal) Initialize(myDBTypeManager)

        /// <summary>
        /// Initialize the type: verify it and set all lookuptables threadsafe
        /// </summary>
        /// <param name="myDBTypeManager"></param>
        /// <returns></returns>
        internal Exceptional Initialize(DBTypeManager myDBTypeManager)
        {

            lock (this)
            {

                if (!isNew)
                {
                    return Exceptional.OK;
                }

                #region check if the parent type exists

                GraphDBType parentType = myDBTypeManager.GetTypeByUUID(ParentTypeUUID);

                if (parentType == null)
                {
                    return new Exceptional(new Error_TypeDoesNotExist(ParentTypeUUID.ToString()));
                }
                else
                {

                    #region update lookup tables ob sub-classes

                    // in case, the type was still in the cache (usually happens if you invoke the RemoveAllUserDefinedTypes method) 
                    // just refill the lookup table

                    _TypeAttributeLookupTable.Clear();

                    foreach (var aParentType in myDBTypeManager.GetAllParentTypes(this, false, true).Where(type => type != this))
                    {
                        AddAttributeToLookupTable(aParentType.Attributes);
                    }

                    AddAttributeToLookupTable(Attributes);

                    #endregion

                }

                var parentTypeExcept = myDBTypeManager.HasParentType(ParentTypeUUID, DBBaseObject.UUID);

                if (parentTypeExcept.Failed)
                    return new Exceptional(parentTypeExcept);

                if (!parentTypeExcept.Value)
                {
                    return new Exceptional(new Error_Logic("The type " + Name + " can not be added, because all user defined types must be subtypes of PandoraObject."));
                }


                #endregion

                #region check type of attribute

                foreach (TypeAttribute attribute in _Attributes.Values)
                {
                    if (attribute.GetDBType(myDBTypeManager) == null)
                    {
                        //The typemanager is able to add myAttributes that are of its type
                        if (!myDBTypeManager.GetTypeByUUID(attribute.DBTypeUUID).Name.Equals(Name))
                        {
                            return new Exceptional(new Error_TypeDoesNotExist(attribute.DBTypeUUID.ToHexString()));
                        }
                    }
                }

                #endregion

                isNew = false;
            }

            return Exceptional.OK;

        }

        #endregion


        #region Serialization

        #region Serialize(ref mySerializationWriter)

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {

            if (mySerializationWriter != null)
            {

                try
                {

                    _UUID.Serialize(ref mySerializationWriter);
                    ParentTypeUUID.Serialize(ref mySerializationWriter);
                    mySerializationWriter.WriteBoolean(_IsUserDefined);
                    mySerializationWriter.WriteBoolean(_IsAbstract);
                    mySerializationWriter.WriteString(_Comment);

                    mySerializationWriter.WriteUInt32((UInt32)_Attributes.Count);

                    foreach (var pValPair in _Attributes)
                    {
                        pValPair.Key.Serialize(ref mySerializationWriter);
                        pValPair.Value.Serialize(ref mySerializationWriter);
                    }

                    mySerializationWriter.WriteUInt32((UInt32)_TypeSettings.Count);

                    foreach (var pValPair in _TypeSettings)
                        mySerializationWriter.WriteObject(pValPair.Value);                    

                    mySerializationWriter.WriteUInt32((UInt32)_UniqueAttributes.Count);

                    foreach (var pValPair in _UniqueAttributes)
                        mySerializationWriter.WriteObject(pValPair.GetByteArray());

                    mySerializationWriter.WriteUInt32((UInt32)_MandatoryAttributes.Count);

                    foreach (var pValPair in _MandatoryAttributes)
                        mySerializationWriter.WriteObject(pValPair.GetByteArray());


                    #region Indices

                    mySerializationWriter.WriteUInt32((UInt32)_AttributeIndices.Count);

                    foreach (var idx in _AttributeIndices)
                    {
                        idx.Key.Serialize(ref mySerializationWriter);

                        mySerializationWriter.WriteUInt32((UInt32)idx.Value.Count);
                        foreach (var idxType in idx.Value)
                        {
                            mySerializationWriter.WriteString(idxType.Key);
                            mySerializationWriter.WriteString(idxType.Value.FileSystemLocation.ToString());
                            mySerializationWriter.WriteString(idxType.Value.IndexEdition);
                            mySerializationWriter.WriteString(idxType.Value.IndexName);
                            mySerializationWriter.WriteString(idxType.Value.IndexType);
                            mySerializationWriter.WriteBoolean(idxType.Value is UUIDIndex);
                        }
                    }

                    #endregion

                }

                catch (Exception e)
                {
                    throw new SerializationException("The GraphDBType could not be serialized!\n\n" + e);
                }

            }

        }

        #endregion

        #region Deserialize(ref mySerializationReader)

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {

            UInt32 _Capacity;

            if (mySerializationReader != null)
            {
                try 
                {

                    _UUID = new TypeUUID();
                    UUID.Deserialize(ref mySerializationReader);
                    ParentTypeUUID = new TypeUUID();
                    ParentTypeUUID.Deserialize(ref mySerializationReader);
                    _IsUserDefined = mySerializationReader.ReadBoolean();
                    _IsAbstract = mySerializationReader.ReadBoolean();
                    _Comment = mySerializationReader.ReadString();

                    _Capacity = mySerializationReader.ReadUInt32();
                
                    _Attributes = new Dictionary<AttributeUUID, TypeAttribute>();
                    _BackwardEdgeAttributes = new Dictionary<EdgeKey, AttributeUUID>();

                    _TypeAttributeLookupTable = new Dictionary<AttributeUUID, TypeAttribute>();

                    for (UInt32 i = 0; i < _Capacity; i++)
                    {
                        var _AttrAtrib = new AttributeUUID();
                        _AttrAtrib.Deserialize(ref mySerializationReader);
                        var _TypeObj = new TypeAttribute();                        
                        _TypeObj.Deserialize(ref mySerializationReader);
                        _Attributes.Add(_AttrAtrib, _TypeObj);
                        _TypeAttributeLookupTable.Add(_AttrAtrib, _TypeObj);

                        if (_TypeObj.IsBackwardEdge)
                        {
                            AddBackwardEdgeAttribute(_TypeObj);
                        }
                    }

                    _Capacity = mySerializationReader.ReadUInt32();
                    _TypeSettings = new Dictionary<String, ADBSettingsBase>();
                
                    for (var i = 0; i < _Capacity; i++)
                    {
                        ADBSettingsBase _ADBSettingsBase = (ADBSettingsBase) mySerializationReader.ReadObject();
                        if(_ADBSettingsBase != null)
                            _TypeSettings.Add(_ADBSettingsBase.Name, _ADBSettingsBase);
                    }

                    _Capacity = mySerializationReader.ReadUInt32();
                    _UniqueAttributes = new List<AttributeUUID>();
                    AttributeUUID AttribID = null;

                    for (UInt32 i = 0; i < _Capacity; i++)
                    {
                        AttribID = new AttributeUUID(ref mySerializationReader);
                        _UniqueAttributes.Add(AttribID);
                    }

                    _Capacity = mySerializationReader.ReadUInt32();
                    _MandatoryAttributes = new HashSet<AttributeUUID>();

                    for (UInt32 i = 0; i < _Capacity; i++)
                    {
                        AttribID = new AttributeUUID(ref mySerializationReader);
                        _MandatoryAttributes.Add(AttribID);
                    }

                    #region Indices

                    _AttributeIndices = new Dictionary<IndexKeyDefinition, Dictionary<String, AAttributeIndex>>();
                    _AttributeIndicesNameLookup = new Dictionary<String, IndexKeyDefinition>();

                    var idxCount = mySerializationReader.ReadUInt32();
                    for (var i = 0; i < idxCount; i++)
                    {

                        var idxKey = new IndexKeyDefinition();
                        idxKey.Deserialize(ref mySerializationReader);

                        //_AttributeIndices.Add(idxKey, new Dictionary<String, AttributeIndex>());

                        var idxVersionCount = mySerializationReader.ReadUInt32();

                        for (var j = 0; j < idxVersionCount; j++)
                        {

                            var key                 = mySerializationReader.ReadString();
                            var fileSystemLocation  = new ObjectLocation(mySerializationReader.ReadString());
                            var indexEdition        = mySerializationReader.ReadString();
                            var indexName           = mySerializationReader.ReadString();
                            var indexType           = mySerializationReader.ReadString();
                            var isUUIDIdx           = mySerializationReader.ReadBoolean();

                            //var CreateIdxExcept = CreateAttributeIndex(indexName, idxKey.IndexKeyAttributeUUIDs, indexEdition, indexObjectType, fileSystemLocation);

                            if (isUUIDIdx)
                            {
                                AddAttributeIndex(new UUIDIndex(indexName, idxKey, this, indexType, indexEdition));
                            }
                            else
                            {
                                AddAttributeIndex(new AttributeIndex(indexName, idxKey, this, indexType, indexEdition));
                            }

                            //if (CreateIdxExcept.Failed)
                            //    throw new GraphDBException(CreateIdxExcept.Errors);
                        }

                    }

                    #endregion

                }

                catch (Exception e)
                {
                    throw new SerializationException("The GraphDBType could not be deserialized!\n\n" + e);
                }

            }

        }

        #endregion

        #endregion

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new GraphDBType();
            newT._AttributeIndices = _AttributeIndices;
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion


        #region Operator overloading

        #region myGraphDBType1 == myGraphDBType2

        public static Boolean operator == (GraphDBType myGraphDBType1, GraphDBType myGraphDBType2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(myGraphDBType1, myGraphDBType2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((Object)myGraphDBType1 == null) || ((Object)myGraphDBType2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return myGraphDBType1.Equals(myGraphDBType2);

        }

        #endregion

        #region myGraphDBType1 != myGraphDBType2

        public static Boolean operator != (GraphDBType myGraphDBType1, GraphDBType myGraphDBType2)
        {
            return !(myGraphDBType1 == myGraphDBType2);
        }

        #endregion

        #endregion

        #region IComparable Members

        public Int32 CompareTo(Object myObject)
        {
            
            var objType = (GraphDBType) myObject;

            return Name.CompareTo(objType.Name);

        }

        #endregion

        #region Equals Overrides

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            // If parameter is null return false.
            if (myObject == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            var _GraphDBType = myObject as GraphDBType;
            if ((Object) _GraphDBType == null)
            {
                return false;
            }

            return Equals(_GraphDBType);

        }

        #endregion

        #region Equals(myGraphDBType)

        public Boolean Equals(GraphDBType myGraphDBType)
        {

            // If parameter is null return false:
            if ((Object) myGraphDBType == null)
            {
                return false;
            }

            return (UUID == myGraphDBType.UUID);

        }

        #endregion

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return UUID.GetHashCode();
        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            var _StringBuilder = new StringBuilder(String.Empty);

            _StringBuilder.Append(Name);

            _StringBuilder.Append(" : " + ParentTypeUUID + " (");

            foreach (KeyValuePair<AttributeUUID, TypeAttribute> attr in _Attributes)
            {
                _StringBuilder.Append("<");
                var keyString = attr.Key.ToString();

                if (keyString.Length >= 3)
                {
                    _StringBuilder.Append(keyString.Substring(0, 3));
                }
                else
                {
                    _StringBuilder.Append(keyString);
                }

                _StringBuilder.Append("> ");
                _StringBuilder.Append(attr.Value.ToString());
                _StringBuilder.Append(",");
            }

            _StringBuilder.Remove(_StringBuilder.Length - 1, 1);

            _StringBuilder.Append(")");

            if (!_Comment.IsNullOrEmpty())
            {
                _StringBuilder.Append(String.Format(" Comment: {0}", _Comment));
            }

            return _StringBuilder.ToString();

        }

        #endregion
    }

}
