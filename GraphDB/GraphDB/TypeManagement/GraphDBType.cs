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
using sones.GraphDB.Settings;
using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement.PandoraTypes;
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
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.Enums;

#endregion

namespace sones.GraphDB.TypeManagement
{

    public class GraphDBType : AFSObject, IComparable//, ICloneable
    {

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

        private TypeUUID _ParentTypeUUID;

        /// <summary>
        /// The parent type of this PandoraType
        /// </summary>
        public TypeUUID ParentTypeUUID
        {
            get
            {
                return _ParentTypeUUID;
            }
        }

        private GraphDBType _ParentType = null;

        #endregion

        #region Attributes

        private Dictionary<String, ADBSettingsBase>         _TypeSettings;
        private List<AttributeUUID>                         _UniqueAttributes;
        private HashSet<AttributeUUID>                      _MandatoryAttributes;
        private HashSet<AttributeUUID>                      _ParentMandatoryAttribs;

        /// <summary>
        /// All attributes which are explicitly defined by the user but just show the implicit backwardedge
        /// </summary>
        private Dictionary<EdgeKey, AttributeUUID> _BackwardEdgeAttributes;

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
        private Dictionary<IndexKeyDefinition, Dictionary<String, AttributeIndex>> _AttributeIndices;

        /// <summary>
        /// Contains a list of all indexes for this type. Index is a list of attribute names over 
        /// which the index is built. 
        /// &lt;ID, &lt;Edition, Index&gt; &gt;
        /// </summary>
        public Dictionary<IndexKeyDefinition, Dictionary<String, AttributeIndex>> AttributeIndices
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

        private Boolean isNew = true;

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


            _ParentTypeUUID                 = new TypeUUID(0);
            _Attributes                 = new Dictionary<AttributeUUID,TypeAttribute>();
            _AttributeIndices           = new Dictionary<IndexKeyDefinition, Dictionary<string,AttributeIndex>>();
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
            _ParentTypeUUID                 = myParentType;

            // PandoraType is the most abstract type.
            // It doesnt contain any special myAttributes.
            // Set the myAttributes
            _Attributes                 = myAttributes;
            _AttributeIndices           = new Dictionary<IndexKeyDefinition, Dictionary<String, AttributeIndex>>();
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

        #region APandoraStructure Members

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new GraphDBType();
            newT._AttributeIndices = _AttributeIndices;
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            GraphDBType objType = (GraphDBType) obj;
            return this.Name.CompareTo(objType.Name);
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            GraphDBType p = obj as GraphDBType;
            if ((System.Object)p == null)
            {
                return false;
            }

            return Equals(p);

        }

        public Boolean Equals(GraphDBType p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return (this.UUID == p.UUID);
        }

        public override int GetHashCode()
        {
            return this.UUID.GetHashCode();
        }

        public static Boolean operator ==(GraphDBType a, GraphDBType b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(GraphDBType a, GraphDBType b)
        {
            return !(a == b);
        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            var _returnValue = new StringBuilder(String.Empty);

            _returnValue.Append(Name);

            _returnValue.Append(" : " + _ParentTypeUUID + " (");

            foreach (KeyValuePair<AttributeUUID, TypeAttribute> attr in _Attributes)
            {
                _returnValue.Append("<");
                var keyString = attr.Key.ToString();

                if (keyString.Length >= 3)
                {
                    _returnValue.Append(keyString.Substring(0, 3));
                }
                else
                {
                    _returnValue.Append(keyString);
                }

                _returnValue.Append("> ");
                _returnValue.Append(attr.Value.ToString());
                _returnValue.Append(",");
            }
            _returnValue.Remove(_returnValue.Length - 1, 1);

            _returnValue.Append(")");

            if (!_Comment.IsNullOrEmpty())
            {
                _returnValue.Append(String.Format(" Comment: {0}", _Comment));
            }

            return _returnValue.ToString();

        }

        #endregion

        #region Serialization

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            if (mySerializationWriter != null)
            {
                try
                {
                    _UUID.Serialize(ref mySerializationWriter);
                    _ParentTypeUUID.Serialize(ref mySerializationWriter);
                    mySerializationWriter.WriteObject(_IsUserDefined);
                    mySerializationWriter.WriteObject(_IsAbstract);
                    mySerializationWriter.WriteObject(_Comment);

                    mySerializationWriter.WriteObject((UInt32)_Attributes.Count);
                    foreach (var pValPair in _Attributes)
                    {
                        pValPair.Key.Serialize(ref mySerializationWriter);
                        pValPair.Value.Serialize(ref mySerializationWriter);
                    }

                    mySerializationWriter.WriteObject((UInt32)_TypeSettings.Count);
                    foreach (var pValPair in _TypeSettings)
                        mySerializationWriter.WriteObject(pValPair.Value);                    

                    mySerializationWriter.WriteObject((UInt32)_UniqueAttributes.Count);
                    foreach (var pValPair in _UniqueAttributes)
                        mySerializationWriter.WriteObject(pValPair.GetByteArray());

                    mySerializationWriter.WriteObject((UInt32)_MandatoryAttributes.Count);
                    foreach (var pValPair in _MandatoryAttributes)
                        mySerializationWriter.WriteObject(pValPair.GetByteArray());


                    #region Indices

                    mySerializationWriter.WriteObject(_AttributeIndices.Count);
                    foreach (var idx in _AttributeIndices)
                    {
                        idx.Key.Serialize(ref mySerializationWriter);

                        mySerializationWriter.WriteObject(idx.Value.Count);
                        foreach (var idxType in idx.Value)
                        {
                            mySerializationWriter.WriteObject(idxType.Key);
                            mySerializationWriter.WriteObject(idxType.Value.FileSystemLocation.ToString());
                            mySerializationWriter.WriteObject(idxType.Value.IndexEdition);
                            mySerializationWriter.WriteObject(idxType.Value.IndexName);
                            mySerializationWriter.WriteObject(idxType.Value.IndexType);
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

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            UInt32              _Capacity;

            if (mySerializationReader != null)
            {
                try 
                {
                    _UUID = new TypeUUID();
                    UUID.Deserialize(ref mySerializationReader);
                    _ParentTypeUUID = new TypeUUID();
                    _ParentTypeUUID.Deserialize(ref mySerializationReader);
                    _IsUserDefined = (Boolean)mySerializationReader.ReadObject();
                    _IsAbstract = (Boolean)mySerializationReader.ReadObject();
                    _Comment = (String)mySerializationReader.ReadObject();

                    _Capacity = (UInt32)mySerializationReader.ReadObject();
                
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
                               
                    _Capacity = (UInt32)mySerializationReader.ReadObject();
                    _TypeSettings = new Dictionary<String, ADBSettingsBase>();
                
                    for (UInt32 i = 0; i < _Capacity; i++)
                    {                        
                        ADBSettingsBase _ADBSettingsBase = (ADBSettingsBase)mySerializationReader.ReadObject();
                        if(_ADBSettingsBase != null)
                            _TypeSettings.Add(_ADBSettingsBase.Name, _ADBSettingsBase);
                    }

                    _Capacity = (UInt32)mySerializationReader.ReadObject();
                    _UniqueAttributes = new List<AttributeUUID>();
                    AttributeUUID AttribID = null;

                    for (UInt32 i = 0; i < _Capacity; i++)
                    {
                        AttribID = new AttributeUUID(ref mySerializationReader);
                        _UniqueAttributes.Add(AttribID);
                    }

                    _Capacity = (UInt32)mySerializationReader.ReadObject();
                    _MandatoryAttributes = new HashSet<AttributeUUID>();

                    for (UInt32 i = 0; i < _Capacity; i++)
                    {
                        AttribID = new AttributeUUID(ref mySerializationReader);
                        _MandatoryAttributes.Add(AttribID);
                    }

                    #region Indices

                    _AttributeIndices = new Dictionary<IndexKeyDefinition, Dictionary<String, AttributeIndex>>();
                    _AttributeIndicesNameLookup = new Dictionary<string, IndexKeyDefinition>();

                    Int32 idxCount = (Int32)mySerializationReader.ReadObject();
                    for (Int32 i = 0; i < idxCount; i++)
                    {

                        IndexKeyDefinition idxKey = new IndexKeyDefinition();
                        idxKey.Deserialize(ref mySerializationReader);

                        //_AttributeIndices.Add(idxKey, new Dictionary<String, AttributeIndex>());

                        Int32 idxVersionCount = (Int32)mySerializationReader.ReadObject();
                        for (UInt32 j = 0; j < idxVersionCount; j++)
                        {
                            String key = (String)mySerializationReader.ReadObject();
                            var fileSystemLocation = new ObjectLocation((String)mySerializationReader.ReadObject());
                            String indexEdition = (String)mySerializationReader.ReadObject();
                            String indexName = (String)mySerializationReader.ReadObject();
                            String indexType = (String)mySerializationReader.ReadObject();

                            //var CreateIdxExcept = CreateAttributeIndex(indexName, idxKey.IndexKeyAttributeUUIDs, indexEdition, indexObjectType, fileSystemLocation);
                            AddAttributeIndex(new AttributeIndex(indexName, idxKey, this, indexType, indexEdition));

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

        #region private helper methods

        private HashSet<AttributeUUID> GetParentMandatoryAttr(DBTypeManager myTypeManager)
        {
            IEnumerable<GraphDBType> ParentTypes = myTypeManager.GetAllParentTypes(this, false, false);

            if (_ParentMandatoryAttribs == null)
                _ParentMandatoryAttribs = new HashSet<AttributeUUID>();

            _ParentMandatoryAttribs.Clear();
            foreach (var Type in ParentTypes)
            {
                foreach (var attribID in Type.GetMandatoryAttributesUUIDs(myTypeManager))
                {
                    _ParentMandatoryAttribs.Add(attribID);
                }
            }

            return _ParentMandatoryAttribs;
        }

        /// <summary>
        /// Creates an index for the given myAttributes by filling the given the index with the objects
        /// of this type that are already stored.</summary>
        /// <param name="myAttributeNames">The names of the myAttributes, over which the index was created.</param>
        private Exceptional<Boolean> AddAttributeIndex(AttributeIndex myAttributeIndex)
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

                    _AttributeIndices.Add(myAttributeIndex.IndexKeyDefinition, new Dictionary<string, AttributeIndex>());
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

        private TypeAttribute FindAttributeInLookup(string myName, ref AttributeUUID myKey)
        {
            foreach (var attr in _TypeAttributeLookupTable)
            {
                if (attr.Value.Name == myName)
                {
                    myKey = attr.Key;
                    return attr.Value;
                }
            }

            return null;
        }

        private void LoadPandoraType(IGraphFSSession myIGraphFS2Session, ObjectLocation myObjectLocation)
        {


            if (myIGraphFS2Session.ObjectExists(myObjectLocation).Value != Trinary.TRUE)
                throw new GraphDBException(new Error_DatabaseNotFound(myObjectLocation));

            if (myIGraphFS2Session.UserMetadataExists(new ObjectLocation(myObjectLocation, DBConstants.DBTypeDefinition), "Name").Value != Trinary.TRUE)
                throw new GraphDBException(new Error_DatabaseNotFound(myObjectLocation));

            if (myIGraphFS2Session.UserMetadataExists(new ObjectLocation(myObjectLocation, DBConstants.DBTypeDefinition), "Superclass").Value != Trinary.TRUE)
                throw new GraphDBException(new Error_DatabaseNotFound(myObjectLocation));

            _ParentTypeUUID = (TypeUUID)myIGraphFS2Session.GetUserMetadatum(new ObjectLocation(myObjectLocation, DBConstants.DBTypeDefinition), "Superclass").Value.First<Object>();
            _UUID = (TypeUUID)myIGraphFS2Session.GetUserMetadatum(new ObjectLocation(myObjectLocation, DBConstants.DBTypeDefinition), "UUID").Value.First<Object>();


            #region Load attributes and settings

            // Attributes
            var _AttributeList = myIGraphFS2Session.GetUserMetadata(new ObjectLocation(myObjectLocation, DBConstants.DBAttributesLocation));

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
            var _SettingList = myIGraphFS2Session.GetUserMetadata(new ObjectLocation(myObjectLocation, DBConstants.DBSettingsLocation));

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
            if (myIGraphFS2Session.ObjectStreamExists(new ObjectLocation(myObjectLocation, DBConstants.DBIndicesLocation), FSConstants.DIRECTORYSTREAM).Value == Trinary.TRUE)
                _AttributeIndices = new Dictionary<IndexKeyDefinition, Dictionary<String, AttributeIndex>>();

            else
                Console.WriteLine("No Indices for database type '" + Name + "' found!");

            #endregion


            // Set the property in order to automagically set the
            // ObjectPath and ObjectName
            ObjectLocation = myObjectLocation;

        }

        private IEnumerable<TypeAttribute> GetAttributes(DBContext context)
        {
            foreach (var attrib in Attributes)
            {
                if (attrib.Value is ASpecialTypeAttribute)
                {
                    if ((bool)context.DBSettingsManager.GetSettingValue((attrib.Value as ASpecialTypeAttribute).ShowSettingName, context, TypesSettingScope.ATTRIBUTE, this, (attrib.Value as TypeAttribute)).Value.Value)
                    {
                        continue;
                    }
                }

                yield return attrib.Value;
            }

            yield break;
        }

        #endregion

        #region public methods

        #region Index

        #region GetAttributeIndex methods

        /// <summary>
        /// Returns the default edition index of the given attribute. If there is more than one (or empty) Edition then return the first.
        /// At some time, we could change this to take statistical information to get the best index
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute we want an index for.</param>
        /// <returns>The index for the given myAttributes if one exist. Else, null.</returns>
        public AttributeIndex GetDefaultAttributeIndex(AttributeUUID myAttributeName)
        {

            return GetAttributeIndex(myAttributeName, DBConstants.DEFAULTINDEX).Value;

        }

        /// <summary>
        /// Returns the index of the given attribute
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute we want an index for.</param>
        /// <param name="myIndexEdition">The name of the index edition. May be null</param>
        /// <returns>The index for the given myAttributes if one exist. Else, null.</returns>
        public AttributeIndex GetAttributeIndex(List<AttributeUUID> myAttributeNames, String myIndexEdition)
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
        public AttributeIndex GetAttributeIndex(String myIndexName, String myIndexEdition)
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
        private AttributeIndex GetAttributeIndex(IndexKeyDefinition idxKey, string edition)
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
        public Exceptional<AttributeIndex> GetAttributeIndex(AttributeUUID myAttributeUUID, String myIndexEdition)
        {
            IndexKeyDefinition idxKey = new IndexKeyDefinition(new List<AttributeUUID>() { myAttributeUUID });

            lock (_AttributeIndices)
            {

                if (_AttributeIndices.ContainsKey(idxKey))
                {
                    if (myIndexEdition == null)
                    {
                        if (_AttributeIndices[idxKey].ContainsKey(DBConstants.DEFAULTINDEX))
                            return new Exceptional<AttributeIndex>(_AttributeIndices[idxKey][DBConstants.DEFAULTINDEX]);
                        else
                            return new Exceptional<AttributeIndex>(_AttributeIndices[idxKey].First().Value);
                    }
                    else if (_AttributeIndices[idxKey].ContainsKey(myIndexEdition))
                    {
                        return new Exceptional<AttributeIndex>(_AttributeIndices[idxKey][myIndexEdition]);
                    }
                }
            }

            return new Exceptional<AttributeIndex>(new Error_IndexAttributeDoesNotExist(GetTypeAttributeByUUID(myAttributeUUID).Name));
        }

        #endregion

        #region GetAttributeIndexReference(myAttributeName, myIndexEdition)

        /// <summary>
        /// Returns the index of the given attribute
        /// </summary>
        /// <param name="myAttributeUUID">The name of the attribute we want an index for.</param>
        /// <param name="myIndexEdition">THe name of the index edition. May be null</param>
        /// <returns>The index for the given myAttributes if one exist. Else, null.</returns>
        public Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>> GetAttributeIndexReference(AttributeUUID myAttributeUUID, String myIndexEdition, DBIndexManager indexManager)
        {
            var attrIdx = GetAttributeIndex(myAttributeUUID, myIndexEdition);
            if (!attrIdx.Success)
            {
                return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(attrIdx);
            }

            return attrIdx.Value.GetIndexReference(indexManager);
        }

        #endregion

        /// <summary>
        /// Removes the given index from this type.
        /// </summary>
        /// <param name="myIndexName">The name of index.</param>
        public Exceptional<Boolean> RemoveIndex(String myIndexName, String myIndexEdition, DBTypeManager myTypeManager)
        {

            myIndexEdition = myIndexEdition ?? DBConstants.DEFAULTINDEX;

            foreach (AttributeIndex aidx in GetAllAttributeIndices())
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
        public Exceptional<AttributeIndex> CreateUniqueAttributeIndex(String myIndexName, AttributeUUID myAttributeName, String myIndexEdition)
        {
            return CreateUniqueAttributeIndex(myIndexName, new List<AttributeUUID> { myAttributeName }, myIndexEdition);
        }

        #endregion

        #region CreateUniqueAttributeIndex(myIndexName, myAttributeUUIDs, myIndexEdition, myIndexObjectTypes, myFileSystemLocation)

        /// <summary>
        /// Creates an index for the given myAttribute by filling the given the index with the objects
        /// of this type that are already stored.</summary>
        /// <param name="myAttributeNames">The names of the myAttributes, over which the index was created.</param>
        public Exceptional<AttributeIndex> CreateUniqueAttributeIndex(String myIndexName, List<AttributeUUID> myAttributeUUIDs, String myIndexEdition)
        {
            // change this to a index type with a single value
            return CreateAttributeIndex(myIndexName, myAttributeUUIDs, myIndexEdition, VersionedHashIndexObject.Name);
        }

        #endregion

        #region CreateAttributeIndex(myIndexName, myAttributeName, myIndexEdition, myIndexObjectTypes, myFileSystemLocation)

        /// <summary>
        /// Creates an index for the given myAttribute by filling the given the index with the objects
        /// of this type that are already stored.</summary>
        /// <param name="myAttributeNames">The names of the myAttributes, over which the index was created.</param>
        public Exceptional<AttributeIndex> CreateAttributeIndex(String myIndexName, AttributeUUID myAttributeName, String myIndexEdition, String myIndexType = null)
        {
            return CreateAttributeIndex(myIndexName, new List<AttributeUUID> { myAttributeName }, myIndexEdition, myIndexType);
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
        public Exceptional<AttributeIndex> CreateAttributeIndex(String myIndexName, List<AttributeUUID> myAttributeUUIDs, String myIndexEdition, String myIndexType = null)
        {

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
        public AttributeIndex GetUUIDIndex(DBTypeManager myTypeManager)
        {

            return GetDefaultAttributeIndex(myTypeManager.GetGUIDTypeAttribute().UUID);

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

        public IEnumerable<AttributeIndex> GetAllAttributeIndices(Boolean includeGUIDIndices = true)
        {

            foreach (var attrIndices in _AttributeIndices)
            {
                foreach (var attrIndex in attrIndices.Value)
                {
                    if (!attrIndex.Value.IsUuidIndex || includeGUIDIndices)
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

        public List<AttributeIndex> GetAttributeIndices(IndexKeyDefinition IndexName)
        {
            if (_AttributeIndices.ContainsKey(IndexName))
                return new List<AttributeIndex>(_AttributeIndices[IndexName].Values);

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
        public Exceptional<bool> SetPersistentSetting(string myName, ADBSettingsBase mySetting, DBTypeManager myTypeManager)
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
                return new Exceptional<bool>(FlushExcept);

            return new Exceptional<bool>(true);
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
                    return new Exceptional<bool>(new Error_AttributeDoesNotExists(this.Name, attribs));
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
        public Exceptional<Boolean> AddUniqueAttributes(List<AttributeUUID> myAttribIDs, DBTypeManager myTypeManager)
        {
            if (!myAttribIDs.IsNullOrEmpty())
            {
                #region data

                AttributeIndex AttribIndex = null;
                List<GraphDBType> SubTypes = myTypeManager.GetAllSubtypes(this, false);
                IndexKeyDefinition idxKey = new IndexKeyDefinition(myAttribIDs);

                #endregion

                AttribIndex = GetAttributeIndex(idxKey, DBConstants.UNIQUEATTRIBUTESINDEX);

                if (!SubTypes.IsNullOrEmpty())
                {
                    string idxName = myTypeManager.DBIndexManager.GetUniqueIndexName(myAttribIDs, this);

                    foreach (var aType in SubTypes)
                    {
                        foreach (var AttrID in myAttribIDs)
                        {
                            aType._UniqueAttributes.Add(AttrID);
                        }

                        if (AttribIndex != null)
                        {
                            var createIdxExcept = aType.CreateUniqueAttributeIndex(idxName, myAttribIDs, DBConstants.UNIQUEATTRIBUTESINDEX);

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

        public Exceptional<ResultType> ChangeUniqueAttributes(List<String> myAttributes, DBTypeManager myTypeManager)
        {

            List<AttributeUUID> attrUUIDs = new List<AttributeUUID>();

            #region Validate attributes

            foreach (var attr in myAttributes)
            {
                var typeAttr = GetTypeAttributeByName(attr);
                if (typeAttr == null)
                {
                    return new Exceptional<ResultType>(new Error_AttributeDoesNotExists(this.Name, attr));
                }
                attrUUIDs.Add(typeAttr.UUID);
            }

            #endregion

            #region Remove old unique index and attributes

            var mayBeUniqueIdx = FindUniqueIndex();

            if (mayBeUniqueIdx != null)
            {
                RemoveIndex(mayBeUniqueIdx.IndexName, mayBeUniqueIdx.IndexEdition, myTypeManager);

                foreach (var attrUUID in mayBeUniqueIdx.IndexKeyDefinition.IndexKeyAttributeUUIDs)
                {
                    _UniqueAttributes.Add(attrUUID);
                }
            }

            #endregion

            String idxName = myTypeManager.DBIndexManager.GetUniqueIndexName(attrUUIDs, this);
            var CreateIdxExcept = CreateUniqueAttributeIndex(idxName, attrUUIDs, DBConstants.UNIQUEATTRIBUTESINDEX);

            if (CreateIdxExcept.Failed)
                return new Exceptional<ResultType>(CreateIdxExcept);

            var rebuildResult = myTypeManager.DBIndexManager.RebuildIndex(idxName, DBConstants.UNIQUEATTRIBUTESINDEX, this, IndexSetStrategy.UNIQUE);

            if (rebuildResult.Failed)
            {
                //add already removed idx
                if (mayBeUniqueIdx != null)
                {
                    AddAttributeIndex(mayBeUniqueIdx);
                }

                var RemoveIdxExcept = RemoveIndex(idxName, DBConstants.UNIQUEATTRIBUTESINDEX, myTypeManager);

                if (RemoveIdxExcept.Failed)
                    return new Exceptional<ResultType>(RemoveIdxExcept);

                return rebuildResult;
            }

            #region Set the unique flag for the attributes

            List<AttributeUUID> ParentUniqueIDs = new List<AttributeUUID>();

            var AddUniqueAttrExcept = AddUniqueAttributes(ParentUniqueIDs, myTypeManager);

            if (AddUniqueAttrExcept.Failed)
                return new Exceptional<ResultType>(AddUniqueAttrExcept);

            AddUniqueAttrExcept = AddUniqueAttributes(attrUUIDs, myTypeManager);

            if (AddUniqueAttrExcept.Failed)
                return new Exceptional<ResultType>(AddUniqueAttrExcept);

            #endregion

            var FlushExcept = myTypeManager.FlushType(this);

            if (FlushExcept.Failed)
                return new Exceptional<ResultType>(FlushExcept);

            return new Exceptional<ResultType>(ResultType.Successful);
        }

        public AttributeIndex FindUniqueIndex()
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

        public void SetComment(string comment)
        {
            _Comment = comment;
        }

        #endregion

        #region misc

        public GraphDBType GetParentType(DBTypeManager myTypeManager)
        {
            if (_ParentType == null)
            {
                if (_ParentTypeUUID == null)
                {
                    return null;
                }
                else
                {
                    _ParentType = myTypeManager.GetTypeByUUID(_ParentTypeUUID);

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
            _ParentTypeUUID = typeUUID;
        }

        #endregion

        #endregion

        /// <summary>
        /// Initialize the type: verify it and set all lookuptables threadsafe
        /// </summary>
        /// <param name="dBTypeManager"></param>
        /// <returns></returns>
        internal Exceptional Initialize(DBTypeManager dBTypeManager)
        {
            lock (this)
            {

                if (!isNew)
                {
                    return Exceptional.OK;
                }

                #region check if the parent type exists

                GraphDBType parentType = dBTypeManager.GetTypeByUUID(ParentTypeUUID);

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

                    foreach (var aParentType in dBTypeManager.GetAllParentTypes(this, false, true).Where(type => type != this))
                    {
                        AddAttributeToLookupTable(aParentType.Attributes);
                    }

                    AddAttributeToLookupTable(Attributes);

                    #endregion

                }

                var parentTypeExcept = dBTypeManager.HasParentType(ParentTypeUUID, DBBaseObject.UUID);

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
                    if (attribute.GetDBType(dBTypeManager) == null)
                    {
                        //The typemanager is able to add myAttributes that are of its type
                        if (!dBTypeManager.GetTypeByUUID(attribute.DBTypeUUID).Name.Equals(Name))
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
    }
}
