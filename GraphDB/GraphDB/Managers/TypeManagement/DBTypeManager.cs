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


#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.Managers.AlterType;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Plugin;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Structures.Enums;

using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.Warnings;
using sones.GraphFS;
using sones.GraphFS.DataStructures;
using sones.GraphFS.InternalObjects;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDBInterface.Result;
using sones.GraphDBInterface.TypeManagement;
using sones.GraphDB.Managers.TypeManagement.BasicTypes;

#endregion

namespace sones.GraphDB.TypeManagement
{

    public class DBTypeManager
    {

        #region Data

        Dictionary<TypeUUID, GraphDBType> _BasicTypes = new Dictionary<TypeUUID, GraphDBType>();

        Dictionary<TypeUUID, GraphDBType> _SystemTypes = new Dictionary<TypeUUID, GraphDBType>();
        
        Dictionary<TypeUUID, GraphDBType> _UserDefinedTypes = new Dictionary<TypeUUID, GraphDBType>();

        private Dictionary<String, GraphDBType> _TypesNameLookUpTable = new Dictionary<String, GraphDBType>();

        private ListOfStringsObject _ObjectLocationsOfAllUserDefinedDatabaseTypes = null;

        private EntityUUID _UserID = null;

        // This is just a temp for GetGUIDIndexAttribute
        private TypeAttribute _GUIDTypeAttribute = null;

        /// <summary>
        /// The database root path.
        /// </summary>
        private ObjectLocation _DatabaseRootPath;

        /// <summary>
        /// The myIGraphFS where the information is stored. Remove when InstanceSettings contain the myIGraphFS.
        /// </summary>
        private IGraphFSSession _IGraphFSSession;

        DBContext _DBContext;

        #region Manager
        
        public DBIndexManager DBIndexManager
        {
            get { return _DBContext.DBIndexManager; }
        }

        public DBSettingsManager DBSettingsManager
        {
            get { return _DBContext.DBSettingsManager; }
        }

        public DBPluginManager DBPluginManager
        {
            get { return _DBContext.DBPluginManager; }
        }

        public DBObjectManager ObjectManager
        {
            get { return _DBContext.DBObjectManager; }
        }
        
        #endregion

        #endregion

        #region Constructor

        public DBTypeManager() { }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="myIGraphDBSession">The filesystem where the information is stored.</param>
        /// <param name="DatabaseRootPath">The database root path.</param>
        public DBTypeManager(IGraphFSSession myIGraphFS, ObjectLocation myDatabaseRootPath, EntityUUID myUserID, Dictionary<String, ADBSettingsBase> myDBSettings, DBContext dbContext)
        {

            _DBContext = dbContext;
            _UserID = myUserID;
            _DatabaseRootPath                               = myDatabaseRootPath;
            _IGraphFSSession                                = myIGraphFS;
            _ObjectLocationsOfAllUserDefinedDatabaseTypes   = LoadListOfTypeLocations(myDatabaseRootPath);

        }

        public DBTypeManager(DBTypeManager dBTypeManager)
        {

            _DBContext                                      = dBTypeManager._DBContext;
            _UserID                                         = dBTypeManager._UserID;
            _DatabaseRootPath                               = dBTypeManager._DatabaseRootPath;
            _IGraphFSSession                                = dBTypeManager._IGraphFSSession;
            _ObjectLocationsOfAllUserDefinedDatabaseTypes   = dBTypeManager._ObjectLocationsOfAllUserDefinedDatabaseTypes;

            _SystemTypes                                    = dBTypeManager._SystemTypes;
            _BasicTypes                                     = dBTypeManager._BasicTypes;
            _GUIDTypeAttribute                              = dBTypeManager._GUIDTypeAttribute;

            //TODO: As soon as we have serialized Indices we can recomment these sections
            #region As soon as we have serialized Indices we can recomment these sections

            //_UserDefinedTypes                               = dBTypeManager._UserDefinedTypes;
            //_TypesNameLookUpTable                           = dBTypeManager._TypesNameLookUpTable;
            
            foreach (GraphDBType ptype in _SystemTypes.Values)
                _TypesNameLookUpTable.Add(ptype.Name, ptype);

            foreach (GraphDBType ptype in _BasicTypes.Values)
                _TypesNameLookUpTable.Add(ptype.Name, ptype);

            LoadUserDefinedDatabaseTypes(false);
            

            #endregion

        }

        /// <summary>
        /// Load the locations of all types. Throws an exceptions if it fails to load from FS.
        /// </summary>
        /// <param name="myDatabaseRootPath"></param>
        /// <exception cref="GraphDBException"></exception>
        /// <returns></returns>
        private ListOfStringsObject LoadListOfTypeLocations(ObjectLocation myDatabaseRootPath)
        {

            var listOfLocations = _IGraphFSSession.GetOrCreateFSObject<ListOfStringsObject>(new ObjectLocation(myDatabaseRootPath, DBConstants.DBTypeLocations), FSConstants.LISTOF_STRINGS, null, null, 0, false);

            if (listOfLocations.Failed())
            {
                throw new GraphDBException(listOfLocations.Errors);
            }

            return listOfLocations.Value;

        }

        #endregion

        #region Init(myIGraphFS, myDatabaseLocation)

        /// <summary>
        /// Initializes the type manager. This method may only be called once, else it throws an TypeInitializationException.
        /// </summary>
        /// <param name="myIGraphFSSession">The myIGraphFS, on which the database is stored.</param>
        /// <param name="myDatabaseLocation">The databases root path in the myIGraphFS.</param>
        public Exceptional Init(IGraphFSSession myIGraphFSSession, ObjectLocation myDatabaseLocation, Boolean myRebuildIndices)
        {

            #region Input validation

            if (myIGraphFSSession == null)
                return new Exceptional<bool>(new Error_ArgumentNullOrEmpty("The parameter myIGraphFS must not be null!"));
            
            if (myDatabaseLocation == null)
                return new Exceptional<bool>(new Error_ArgumentNullOrEmpty("The parameter myDatabaseLocation must not be null!"));

            //ToDo: Find a better way to check this!
            if (_BasicTypes.ContainsKey(DBBaseObject.UUID))
                return new Exceptional<bool>(new Error_UnknownDBError("The TypeManager had already been initialized!"));
            
            #endregion


            #region DBObject - The base of all database types
            // DBObject is a child of DBBaseObject

            var typeDBBaseObject = new GraphDBType(DBBaseObject.UUID, myDatabaseLocation, DBBaseObject.Name, null, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, "The base of all database types");
            _SystemTypes.Add(typeDBBaseObject.UUID, typeDBBaseObject);

            #endregion

            #region DBReference - The base of all user defined database types
            // DBObject is a child of DBBaseObject

            // == DBObject!
            var typeDBReference = new GraphDBType(DBReference.UUID, myDatabaseLocation, DBReference.Name, DBBaseObject.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, "The base of all user defined database types");


            #region UUID special Attribute

            var specialTypeAttribute_UUID = new SpecialTypeAttribute_UUID() { DBTypeUUID = DBReference.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_UUID, this, false);
            _GUIDTypeAttribute = specialTypeAttribute_UUID;

            #endregion

            #region CreationTime special attribute

            var specialTypeAttribute_CrTime = new SpecialTypeAttribute_CREATIONTIME() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_CrTime, this, false);

            #endregion
            
            #region DeletionTime special attribute

            var specialTypeAttribute_DelTime = new SpecialTypeAttribute_DELETIONTIME() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_DelTime, this, false);

            #endregion

            #region Edition special attribute

            var specialTypeAttribute_Edition = new SpecialTypeAttribute_EDITION() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_Edition, this, false);

            #endregion

            #region Editions special attribute

            var specialTypeAttribute_Editions = new SpecialTypeAttribute_EDITIONS() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_Editions, this, false);

            #endregion

            #region LastAccessTime special attribute

            var specialTypeAttribute_AcTime = new SpecialTypeAttribute_LASTACCESSTIME() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_AcTime, this, false);

            #endregion

            #region LastModificationTime special attribute

            var specialTypeAttribute_LastModTime = new SpecialTypeAttribute_LASTMODIFICATIONTIME() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_LastModTime, this, false);

            #endregion            

            #region TypeName special Attribute

            var specialTypeAttribute_TYPE = new SpecialTypeAttribute_TYPE() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_TYPE, this, false);

            #endregion

            #region REVISION special Attribute

            var specialTypeAttribute_REVISION = new SpecialTypeAttribute_REVISION() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_REVISION, this, false);

            #endregion

            #region REVISIONS special Attribute

            var specialTypeAttribute_REVISIONS = new SpecialTypeAttribute_REVISIONS() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_REVISIONS, this, false);

            #endregion

            #region STREAMS special Attribute

            var specialTypeAttribute_STREAMS = new SpecialTypeAttribute_STREAMS() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_STREAMS, this, false);

            #endregion

            #region NUMBER OF REVISIONS Attribute

            var specialTypeAttribute_NUMBEROFREVISIONS = new SpecialTypeAttribute_NUMBEROFREVISIONS() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_NUMBEROFREVISIONS, this, false);

            #endregion

            #region NUMBER OF COPIES

            var specialTypeAttribute_NUMBEROFCOPIES = new SpecialTypeAttribute_NUMBEROFCOPIES() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_NUMBEROFCOPIES, this, false);

            #endregion

            #region PARENT REVISION IDs

            var specialTypeAttribute_PARENTREVISIONIDs = new SpecialTypeAttribute_PARENTREVISIONS() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_PARENTREVISIONIDs, this, false);

            #endregion

            #region MAX REVISION AGE

            var specialTypeAttribute_MAXREVISIONAGE = new SpecialTypeAttribute_MAXREVISIONAGE() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_MAXREVISIONAGE, this, false);

            #endregion

            #region MIN NUMBER OF REVISIONS

            var specialTypeAttribute_MINNUMBEROFREVISIONS = new SpecialTypeAttribute_MINNUMBEROFREVISIONS() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_MINNUMBEROFREVISIONS, this, false);

            #endregion
            
            #region MAX NUMBER OF REVISIONS

            var specialTypeAttribute_MAXNUMBEROFREVISIONS = new SpecialTypeAttribute_MAXNUMBEROFREVISIONS() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_MAXNUMBEROFREVISIONS, this, false);

            #endregion

            #region MAX NUMBER OF COPIES

            var specialTypeAttribute_MAXNUMBEROFCOPIES = new SpecialTypeAttribute_MAXNUMBEROFCOPIES() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_MAXNUMBEROFCOPIES, this, false);

            #endregion

            #region MIN NUMBER OF COPIES

            var specialTypeAttribute_MINNUMBEROFCOPIES = new SpecialTypeAttribute_MINNUMBEROFCOPIES() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_MINNUMBEROFCOPIES, this, false);

            #endregion

            _SystemTypes.Add(typeDBReference.UUID, typeDBReference);

            #endregion

            #region DBVertex

            var typeDBVertex = new GraphDBType(DBVertex.UUID, myDatabaseLocation, DBConstants.DBVertexName, DBReference.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, "The base of all user defined database vertices");

            _SystemTypes.Add(typeDBVertex.UUID, typeDBVertex);

            #endregion

            #region DBEdge

            var typeDBEdge = new GraphDBType(new TypeUUID(DBConstants.DBEdgeID), myDatabaseLocation, DBConstants.DBEdgeName, DBReference.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, "The base of all user defined database edges");

            _SystemTypes.Add(typeDBEdge.UUID, typeDBEdge);

            #endregion

            #region Build-in basic database types
            // These are children of DBBaseObject

            _BasicTypes.Add(DBBoolean.UUID, new GraphDBType(DBBoolean.UUID, new ObjectLocation(myDatabaseLocation), DBBoolean.Name, DBBaseObject.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, ""));
            _BasicTypes.Add(DBDateTime.UUID, new GraphDBType(DBDateTime.UUID, new ObjectLocation(myDatabaseLocation), DBDateTime.Name, DBBaseObject.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, ""));
            _BasicTypes.Add(DBDouble.UUID, new GraphDBType(DBDouble.UUID, new ObjectLocation(myDatabaseLocation), DBDouble.Name, DBBaseObject.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, ""));
            _BasicTypes.Add(DBInt64.UUID, new GraphDBType(DBInt64.UUID, new ObjectLocation(myDatabaseLocation), DBInt64.Name, DBBaseObject.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, ""));
            _BasicTypes.Add(DBInt32.UUID, new GraphDBType(DBInt32.UUID, new ObjectLocation(myDatabaseLocation), DBInt32.Name, DBBaseObject.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, ""));
            _BasicTypes.Add(DBUInt64.UUID, new GraphDBType(DBUInt64.UUID, new ObjectLocation(myDatabaseLocation), DBUInt64.Name, DBBaseObject.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, ""));
            _BasicTypes.Add(DBString.UUID, new GraphDBType(DBString.UUID, new ObjectLocation(myDatabaseLocation), DBString.Name, DBBaseObject.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, ""));

            _BasicTypes.Add(DBBackwardEdgeType.UUID, new GraphDBType(DBBackwardEdgeType.UUID, new ObjectLocation(myDatabaseLocation), DBBackwardEdgeType.Name, DBBaseObject.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, ""));

            #endregion

            foreach (var _GraphDBType in _SystemTypes.Values)
                _TypesNameLookUpTable.Add(_GraphDBType.Name, _GraphDBType);

            foreach (var _GraphDBType in _BasicTypes.Values)
                _TypesNameLookUpTable.Add(_GraphDBType.Name, _GraphDBType);            

            return LoadUserDefinedDatabaseTypes(myRebuildIndices);

        }

        #endregion

        #region LoadUserDefinedDatabaseTypes

        #region LoadUserDefinedDatabaseTypes()

        /// <summary>
        /// Tries to load all database types from their default ObjectLocations
        /// </summary>
        private Exceptional LoadUserDefinedDatabaseTypes(Boolean myRebuildIndices)
        {

            var _ObjectLocations = new List<ObjectLocation>();

            lock (_ObjectLocationsOfAllUserDefinedDatabaseTypes)
            {
                foreach (var _String in _ObjectLocationsOfAllUserDefinedDatabaseTypes.ListOfStrings)
                    _ObjectLocations.Add(ObjectLocation.ParseString(_String));
            }

            return LoadUserDefinedDatabaseTypes(_ObjectLocations, myRebuildIndices);

        }

        #endregion

        #region LoadUserDefinedDatabaseTypes(myObjectLocations)

        /// <summary>
        /// Tries to load all database types from the given ObjectLocations
        /// </summary>
        private Exceptional LoadUserDefinedDatabaseTypes(IEnumerable<ObjectLocation> myObjectLocations, Boolean myRebuildIndices)
        {

            var ListOfDatabaseTypes = new List<GraphDBType>();

            // Search the StorageLocation for ObjectSchemes and put them to the bulk load list
            foreach (var _ActualObjectLocation in myObjectLocations)
            {

                if (GetTypeByName(_ActualObjectLocation.Name) != null)
                    return new Exceptional<Boolean>(new Error_TypeAlreadyExist(_ActualObjectLocation.Name));

                //ListOfDatabaseTypes.Add(new GraphType(_IGraphFS2Session, _ActualObjectLocation, true, this));
                var pt = _IGraphFSSession.GetFSObject<GraphDBType>(_ActualObjectLocation);

                if (pt.Failed())
                    return new Exceptional(pt);

                ListOfDatabaseTypes.Add(pt.Value);

                _UserDefinedTypes.Add(pt.Value.UUID, pt.Value);
                _TypesNameLookUpTable.Add(pt.Value.Name, pt.Value);

            }

            // Add types but do not flush them to the file system
            //AddBulkTypes(ListOfDatabaseTypes);

            #region Check ParentType and AttributeTypes

            foreach (var aType in ListOfDatabaseTypes)
            {
                var result = aType.Initialize(this);
                if (!result.Success())
                {
                    return new Exceptional(result);
                }
            }

            #endregion


            // Uncomment as soon as index is serializeable
            if (myRebuildIndices)
            {
                return _DBContext.DBIndexManager.RebuildIndices(_UserDefinedTypes.Values);
            }

            return Exceptional.OK;
        }

        #endregion

        #endregion

        #region RemoveAllUserDefinedTypes()

        /// <summary>
        /// Clears the typemanager of the user defined types.
        /// </summary>
        private Exceptional<Boolean> RemoveAllUserDefinedTypes()
        {
            var uuidsToRemove = new List<TypeUUID>(_UserDefinedTypes.Keys);
            foreach (var uuid in uuidsToRemove)
            {
                var theType = GetTypeByUUID(uuid as TypeUUID);
                if (theType != null) // it might happen, that the type was already removed from a supertype
                {
                    var removeExcept = RemoveType(theType);

                    if (removeExcept.Failed())
                        return new Exceptional<bool>(removeExcept);

                    _TypesNameLookUpTable.Remove(theType.Name);
                }
            }
            _UserDefinedTypes = new Dictionary<TypeUUID, GraphDBType>();

            return new Exceptional<bool>(true);
        }

        #endregion

        #region Attribute handling

        #region Add Attribute to DBObject

        private Exceptional<ResultType> AddAttributeToDBObject(GraphDBType myTypeOfDBObject, ObjectUUID myUUID, AttributeUUID myAttributeUUID, IObject myAttributeValue)
        {
            //myGraphType is needed due to correctness concerning the attribute name

            #region Input exceptions

            if ((myTypeOfDBObject == null) || (myUUID == null) || (myAttributeUUID == null) || (myAttributeValue == null))
            {
                throw new ArgumentNullException();
            }

            #endregion

            #region Check GraphType for new Attribute

            TypeAttribute typeAttribute = myTypeOfDBObject.GetTypeAttributeByUUID(myAttributeUUID);

            if (typeAttribute == null)
            {
                //Todo: add notification here (the user has to be informed about the detailed circumstances)

                GraphDBError aError = new Error_AttributeIsNotDefined(myTypeOfDBObject.Name, myAttributeUUID.ToString());

                return new Exceptional<ResultType>(aError);
            }

            #endregion

            #region Data

            var objectLocation = new ObjectLocation(myTypeOfDBObject.ObjectLocation, DBConstants.DBObjectsLocation, myUUID.ToString());
            Exceptional<DBObjectStream> aNewDBObject;
            Exceptional<ResultType> result = new Exceptional<ResultType>();

            #endregion

            #region add attribute

            aNewDBObject = _DBContext.DBObjectManager.LoadDBObject(myTypeOfDBObject, myUUID);

            if (aNewDBObject.Failed())
            {
                result.Push(new Error_LoadObject(aNewDBObject.Value.ObjectLocation));
                return result;
            }

            result = aNewDBObject.Value.AddAttribute(typeAttribute.UUID, myAttributeValue);

            if (result.Failed())
                return result;

            try
            {
                _DBContext.DBObjectManager.FlushDBObject(aNewDBObject.Value);
            }
            catch (Exception ex)
            {
                result.Push(new Error_FlushObject(aNewDBObject.Value.ObjectLocation, ex));
                aNewDBObject.Value.RemoveAttribute(typeAttribute.UUID);
            }

            #endregion

            return result;

        }

        #endregion

        #region AddAttributeToType(targetClass, attributeName, attributeType)

        /// <summary>
        /// Adds an attribute with given name and type to the class with the given name
        /// </summary>
        /// <param name="targetClass">The class, we want to add the new attribute to.</param>
        /// <param name="myAttributeName">The name of the attribute.</param>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <returns>Ture, if the attribute could be added to the target class. Else, false. (attribute contained in superclass)</returns>
        public Exceptional<ResultType> AddAttributeToType(GraphDBType mytype, TypeAttribute myTypeAttribute)
        {
            #region check if already initialized

            if (GetTypeByUUID(myTypeAttribute.DBTypeUUID) == null)
            {
                return new Exceptional<ResultType>(new Error_TypeDoesNotExist(myTypeAttribute.DBTypeUUID.ToHexString()));
            }

            #endregion

            #region Check if any ParentType already have an attribute with this name

            foreach (var aType in GetAllParentTypes(mytype, true, true))
            {
                if (aType.GetTypeAttributeByName(myTypeAttribute.Name) != null)
                {
                    return new Exceptional<ResultType>(new Error_AttributeExistsInSupertype(myTypeAttribute.Name, aType.Name));
                }
            }

            #endregion

            #region adapt type

            //if we reach this code, no other superclass contains an attribute with this name, so add it!
            myTypeAttribute.RelatedGraphDBTypeUUID = mytype.UUID;

            if (myTypeAttribute.DefaultValue != null)
            {
                mytype.AddMandatoryAttribute(myTypeAttribute.UUID, this);
            }

            mytype.AddAttribute(myTypeAttribute, this, true);

            var FlushExcept = FlushType(mytype);

            if (FlushExcept.Failed())
                return new Exceptional<ResultType>(FlushExcept);

            #endregion

            #region update lookup tables ob sub-classes

            foreach (var aSubType in GetAllSubtypes(mytype, false))
            {
                aSubType.AttributeLookupTable.Add(myTypeAttribute.UUID, myTypeAttribute);
            }

            #endregion


            return new Exceptional<ResultType>(ResultType.Successful);

        }


        //#region AddAttributeToType(targetClass, attributeName, attributeType)

        ///// <summary>
        ///// Adds an attribute with given name and type to the class with the given name
        ///// </summary>
        ///// <param name="targetClass">The class, we want to add the new attribute to.</param>
        ///// <param name="myAttributeName">The name of the attribute.</param>
        ///// <param name="attributeType">The type of the attribute.</param>
        ///// <returns>Ture, if the attribute could be added to the target class. Else, false. (attribute contained in superclass)</returns>
        //public Exceptional<ResultType> AddAttributeToType(String targetClass, String attributeName, TypeAttribute myTypeAttribute)
        //{

        //    #region INPUT EXCEPTIONS

        //    if (String.IsNullOrEmpty(targetClass))
        //    {
        //        return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty("targetClass"));
        //    }
        //    if (String.IsNullOrEmpty(attributeName))
        //    {
        //        return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty("attributeName"));
        //    }
        //    if (myTypeAttribute == null)
        //    {
        //        return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty("myTypeAttribute"));
        //    }

        //    #endregion

        //    #region Data

        //    GraphDBType tempTargetClassType = GetTypeByName(targetClass);

        //    #endregion

        //    #region check if already initialized

        //    if (GetTypeByUUID(myTypeAttribute.DBTypeUUID) == null)
        //    {
        //        return new Exceptional<ResultType>(new Error_TypeDoesNotExist(myTypeAttribute.DBTypeUUID.ToHexString()));
        //    }

        //    if (tempTargetClassType == null)
        //    {
        //        return new Exceptional<ResultType>(new Error_TypeDoesNotExist(targetClass));
        //    }

        //    #endregion

        //    #region Check if any ParentType already have an attribute with this name

        //    foreach (var aType in GetAllParentTypes(GetTypeByName(targetClass), true, true))
        //    {
        //        if (aType.GetTypeAttributeByName(attributeName) != null)
        //        {
        //            return new Exceptional<ResultType>(new Error_AttributeExistsInSupertype(attributeName, aType.Name));
        //        }
        //    }

        //    #endregion

        //    #region adapt type

        //    //if we reach this code, no other superclass contains an attribute with this name, so add it!
        //    myTypeAttribute.RelatedGraphDBTypeUUID = tempTargetClassType.UUID;
            
        //    if (myTypeAttribute.DefaultValue != null)
        //    {
        //        tempTargetClassType.AddMandatoryAttribute(myTypeAttribute.UUID, this);    
        //    }

        //    tempTargetClassType.AddAttribute(myTypeAttribute, this, true);

        //    var FlushExcept = FlushType(tempTargetClassType);

        //    if (FlushExcept.Failed())
        //        return new Exceptional<ResultType>(FlushExcept);

        //    #endregion

        //    #region update lookup tables ob sub-classes

        //    foreach (var aSubType in GetAllSubtypes(tempTargetClassType, false))
        //    {
        //        aSubType.AttributeLookupTable.Add(myTypeAttribute.UUID, myTypeAttribute);
        //    }

        //    #endregion


        //    return new Exceptional<ResultType>(ResultType.Successful);

        //}

        #endregion

        #region RenameAttributeOfType(type)

        public Exceptional<Boolean> RenameAttributeOfType(GraphDBType myType, String oldName, String newName)
        {

            #region INPUT EXCEPTIONS

            if (myType == null)
            {
                return new Exceptional<Boolean>(new Error_ArgumentNullOrEmpty("myType"));
            }

            if (String.IsNullOrEmpty(oldName))
            {
                return new Exceptional<Boolean>(new Error_ArgumentNullOrEmpty("oldName)"));
            }
            if (String.IsNullOrEmpty(newName))
            {
                return new Exceptional<Boolean>(new Error_ArgumentNullOrEmpty("newName"));
            }

            #endregion

            #region Data

            TypeAttribute typeAttribute = null;

            #endregion

            #region rename attribute of type

            typeAttribute = myType.GetTypeAttributeByName(oldName);

            if (typeAttribute != null)
            {

                Exceptional<Boolean> Result = myType.RenameAttribute(typeAttribute.UUID, newName);

                if (Result.Failed())
                    return new Exceptional<Boolean>(Result);


                var FlushExcept = FlushType(myType);

                if (FlushExcept.Failed())
                {

                    Result = myType.RenameAttribute(typeAttribute.UUID, oldName);

                    if (Result.Failed())
                        return new Exceptional<Boolean>(Result);

                    return new Exceptional<Boolean>(FlushExcept);
                }

            }
            else
            {
                return new Exceptional<Boolean>(new Error_AttributeIsNotDefined(myType.Name, oldName));
            }

            #endregion

            return new Exceptional<Boolean>(true);
        }


        #endregion

        #region RemoveAttributeOfType

        /// <summary>
        /// Removes an attribute of the given type.
        /// </summary>
        /// <param name="aUserType">The target type.</param>
        /// <param name="attributeUUID">The attribute uuid, referencing the deprecated attribute.</param>
        /// <returns></returns>
        private Exceptional<ResultType> RemoveAttributeOfType(GraphDBType aUserType, AttributeUUID attributeUUID)
        {

            #region INPUT EXCEPTIONS

            if (aUserType == null)
            {
                return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty("aUserType"));
            }

            #endregion

            #region remove attribute from type

            TypeAttribute typeAttribute = aUserType.Attributes[attributeUUID];

            if (typeAttribute != null)
            {

                aUserType.RemoveAttribute(typeAttribute.UUID);

                var FlushExcept = FlushType(aUserType);

                if (FlushExcept.Failed())
                {
                    aUserType.AddAttribute(typeAttribute, this, false);

                    return new Exceptional<ResultType>(FlushExcept);
                }

                #region update lookup tables ob sub-classes

                foreach (var aSubType in GetAllSubtypes(aUserType).Where(aType => aType != aUserType))
                {
                    //delete from lookuptable
                    aSubType.RemoveAttributeFromLookupTable(typeAttribute.UUID);
                }

                #endregion


            }
            else
            {
                return new Exceptional<ResultType>(new Error_AttributeIsNotDefined(aUserType.Name, typeAttribute.Name));
            }

            #endregion

            return new Exceptional<ResultType>(ResultType.Successful);

        }

        #endregion

        #region RemoveAttributeFromType(targetClass, attributeName)

        /// <summary>
        /// Removes an attribute of the given type.
        /// </summary>
        /// <param name="targetClass">The target type.</param>
        /// <param name="myAttributeName">The attribute name, referencing the deprecated attribute.</param>
        /// <returns>True, if the attribute was successfully removed. Else, false.(it didnt exist)</returns>
        public Exceptional<ResultType> RemoveAttributeFromType(String targetClass, String attributeName, DBTypeManager myTypeManager)
        {

            #region INPUT EXCEPTIONS

            if (String.IsNullOrEmpty(targetClass))
                return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty(targetClass));

            if (String.IsNullOrEmpty(attributeName))
                return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty(attributeName));

            #endregion

            #region Data

            GraphDBType aType = GetTypeByName(targetClass);

            #endregion

            #region check if type exists

            if (aType == null)
                return new Exceptional<ResultType>(new Error_TypeDoesNotExist(targetClass));

            #endregion

            TypeAttribute Attribute = aType.GetTypeAttributeByName(attributeName);

            Exceptional<ResultType> retVal = null;

            if (Attribute != null)
            {
                //Check for other attributes that reference this one (i.e. as BackwardEdge)

                var referncedBy = GetBackwardReferencesForAttribute(Attribute);
                if (referncedBy.Count > 0)
                {
                    return new Exceptional<ResultType>(new Error_DropOfAttributeNotAllowed(Attribute.GetRelatedType(this).Name, attributeName, referncedBy.ToDictionary(key => key, value => value.GetRelatedType(this))));
                }

                if (Attribute.GetRelatedType(this) != aType)
                {
                    return new Exceptional<ResultType>(new Error_DropOfDerivedAttributeIsNotAllowed(Attribute.GetRelatedType(this).Name, attributeName));
                }
                retVal = RemoveAttributeOfType(aType, Attribute.UUID);
            }
            else
            {
                retVal = new Exceptional<ResultType>(new Error_AttributeIsNotDefined(attributeName));
            }

            return retVal;
        }

        private List<TypeAttribute> GetBackwardReferencesForAttribute(TypeAttribute myAttribute)
        {
            List<TypeAttribute> result = new List<TypeAttribute>();

            foreach (var aType in _UserDefinedTypes)
            {
                foreach (var aAttribute in aType.Value.GetAllAttributes(_DBContext))
                {
                    if (aAttribute.IsBackwardEdge)
                    {
                        if (aAttribute.BackwardEdgeDefinition.AttrUUID == myAttribute.UUID)
                        {
                            result.Add(aAttribute);
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region AreValidAttributes(myTypeOfDBObject, params myAttributes)

        /// <summary>
        /// Checks if the given attributes are valid attributes of the given type.
        /// </summary>
        /// <param name="myTypeOfDBObject">Name of the type.</param>
        /// <param name="myAttributes">Array of attributes.</param>
        public Exceptional<Boolean> AreValidAttributes(GraphDBType myTypeOfDBObject, params String[] myAttributes)
        {

            #region Input Exceptions

            if (myTypeOfDBObject == null)
                return new Exceptional<bool>(new Error_ArgumentNullOrEmpty("Parameter myTypeOfDBObject is invalid."));

            if (myAttributes == null || myAttributes.Length == 0)
                return new Exceptional<bool>(new Error_ArgumentNullOrEmpty("Parameters myAttributes are invalid."));

            #endregion

            foreach (String _Attribute in myAttributes)
            {
                Boolean result = false;
                foreach (GraphDBType aType in GetAllParentTypes(GetTypeByName(myTypeOfDBObject.Name), true, true))
                {
                    if (aType.GetTypeAttributeByName(_Attribute) != null)
                    {
                        result = true;
                        break;
                    }
                }
                if (!result)
                    return new Exceptional<Boolean>(false);
            }

            //if this code is reached, we didnt find any attribute with this name so we return null
            return new Exceptional<Boolean>(true);
        }

        #endregion

        #endregion

        #region GraphDBType handling

        #region GetTypeReadouts(myGraphDBType)

        /// <summary>
        /// return the uuid, revision id and the editition for a new created type
        /// </summary>
        /// <param name="myGraphDBType">the created type</param>
        /// <returns></returns>
        private Exceptional<DBObjectReadout> GetTypeReadouts(GraphDBType myGraphDBType)
        {

            var readOut = new DBObjectReadout();

            if (myGraphDBType == null)
                return new Exceptional<DBObjectReadout>(new Error_ArgumentNullOrEmpty("The type should not be null."));

            readOut.Attributes.Add(DBConstants.DbGraphType, myGraphDBType.Name);
            
            readOut.Attributes.Add(SpecialTypeAttribute_UUID.AttributeName, myGraphDBType.ObjectUUID);
            readOut.Attributes.Add(SpecialTypeAttribute_REVISION.AttributeName, myGraphDBType.ObjectRevisionID);
            readOut.Attributes.Add(SpecialTypeAttribute_EDITION.AttributeName, myGraphDBType.ObjectEdition);

            return new Exceptional<DBObjectReadout>(readOut);

        }

        #endregion

        #region AddBulkTypes(TypeList, flushToFs)

        /// <summary>
        /// This method adds a bunch of new GraphTypeDefinitions (comes from a CREATE TYPE(S) statement) to the TypeManager.
        /// If a certain PType can't be added (because of some inheritance or 
        /// attribute errors), this method tries to add it in a second 
        /// step.
        /// </summary>
        /// <param name="TypeList">List of GraphType definitions that should 
        /// be added to the TypeManager</param>
        /// <returns>List of GraphError</returns>
        public Exceptional<QueryResult> AddBulkTypes(List<GraphDBTypeDefinition> TypeList, DBContext currentContext)
        {

            #region Input Exceptions

            if (TypeList == null)
            {
                return new Exceptional<QueryResult>(new QueryResult());
            }

            if (TypeList.Count.Equals(0))
            {
                return new Exceptional<QueryResult>(new QueryResult());
            }

            #endregion

            #region Data

            var errors = new List<GraphDBError>();
            List<GraphDBType> addedTypes = new List<GraphDBType>();
            List<AttributeUUID> uniqueAttrIDs = new List<AttributeUUID>();
            QueryResult result = new QueryResult();

            #endregion

            try
            {

                #region Create the types without attributes

                foreach (GraphDBTypeDefinition aTypeDef in TypeList)
                {

                    #region Input validation

                    if (String.IsNullOrEmpty(aTypeDef.Name))
                        return new Exceptional<QueryResult>(new Error_ArgumentNullOrEmpty("myTypeName"));

                    if (String.IsNullOrEmpty(aTypeDef.ParentType))
                        return new Exceptional<QueryResult>(new Error_ArgumentNullOrEmpty("myParentType"));

                    if (aTypeDef.Attributes == null)
                        return new Exceptional<QueryResult>(new Error_ArgumentNullOrEmpty("myAttributes"));

                    #endregion

                    GraphDBType thisType = GetTypeByName(aTypeDef.Name);

                    #region Check if the name of the type is already used

                    if (thisType != null)
                    {
                        return new Exceptional<QueryResult>(new Error_TypeAlreadyExist(aTypeDef.Name));
                    }

                    #endregion

                    GraphDBType parentType = GetTypeByName(aTypeDef.ParentType);
                    Dictionary<AttributeUUID, TypeAttribute> attributes = new Dictionary<AttributeUUID, TypeAttribute>();                    

                    #region Add type

                    TypeUUID parentUUID = (parentType == null) ? null : parentType.UUID;

                    #region hack

                    GraphDBType _NewGraphType = new GraphDBType(null, new ObjectLocation(_DatabaseRootPath), aTypeDef.Name, parentUUID, attributes, true, aTypeDef.IsAbstract, aTypeDef.Comment);

                    #endregion

                    addedTypes.Add(_NewGraphType);

                    _UserDefinedTypes.Add(_NewGraphType.UUID, _NewGraphType);
                    _TypesNameLookUpTable.Add(_NewGraphType.Name, _NewGraphType);

                    #endregion

                }

                #endregion

                var backwardEdgesToBeAddedAfterwards = new Dictionary<GraphDBType, List<BackwardEdgeDefinition>>();

                #region Validate the previously added types and add the attributes and backwardEdges

                foreach (var aTypeDef in TypeList)
                {
                    GraphDBType aType = addedTypes.Where(item => item.Name == aTypeDef.Name).FirstOrDefault();

                    #region Check and set parent

                    GraphDBType parentType;

                    #region Verify base type existence

                    if (aType.ParentTypeUUID == null)
                    {
                        parentType = addedTypes.Where(item => item.Name == aTypeDef.ParentType).FirstOrDefault();
                        if (parentType == null)
                        {
                            RemoveRecentlyAddedTypes(addedTypes);
                            return new Exceptional<QueryResult>(new Error_ParentTypeDoesNotExist(aTypeDef.ParentType, aTypeDef.Name));
                        }
                        aType.SetParentTypeUUID(parentType.UUID);
                    }
                    else
                    {
                        parentType = aType.GetParentType(this);
                    }

                    #endregion

                    #region Verify that the type inherit DBReference

                    var parentTypeExcept = HasParentType(parentType.UUID, DBReference.UUID);

                    if (parentTypeExcept.Failed())
                    {
                        return new Exceptional<QueryResult>(parentTypeExcept);
                    }

                    if (!parentTypeExcept.Value)
                    {
                        RemoveRecentlyAddedTypes(addedTypes);
                        return new Exceptional<QueryResult>(new Error_InvalidBaseType(parentType.Name));
                    }

                    #endregion

                    #endregion

                    //add TypeAttributeLookuptable to current type
                    aType.AttributeLookupTable.AddRange(parentType.AttributeLookupTable);

                    #region check and set type of attributes
                    UInt16 attributeCounter = 0;
                    foreach (var attributeDef in aTypeDef.Attributes)
                    {
                        var attribute = attributeDef.Key.CreateTypeAttribute(currentContext, addedTypes, attributeCounter);
                        if (attribute.Failed())
                        {
                            return new Exceptional<QueryResult>(attribute);
                        }

                        if (attribute.Value.Name == aType.Name)
                        {
                            RemoveRecentlyAddedTypes(addedTypes);
                            return new Exceptional<QueryResult>(new Error_InvalidAttributeName("The attribute " + attribute.Value.Name + " can not be added, because it has the same name as its related type."));
                        }

                        GraphDBType attrType = GetTypeByName(attributeDef.Value);
                        if (attrType == null)
                        {
                            attrType = addedTypes.Where(item => item.Name == attributeDef.Value).FirstOrDefault();
                            if (attrType == null)
                            {
                                RemoveRecentlyAddedTypes(addedTypes);
                                return new Exceptional<QueryResult>(new Error_TypeDoesNotExist(attributeDef.Value));
                            }
                        }

                        attributeCounter++;

                        TypeAttribute newAttr = attribute.Value;

                        newAttr.DBTypeUUID = attrType.UUID;
                        newAttr.RelatedGraphDBTypeUUID = aType.UUID;

                        #region we had not defined a special EdgeType - for single reference attributes we need to set the EdgeTypeSingle NOW!

                        if (newAttr.KindOfType == KindsOfType.SingleReference && attrType.IsUserDefined && newAttr.EdgeType == null)
                            newAttr.EdgeType = new EdgeTypeSingleReference(null, newAttr.DBTypeUUID);

                        #endregion

                        #region Validate EdgeType in terms of List & Single

                        if (newAttr.KindOfType == KindsOfType.SingleReference && attrType.IsUserDefined && !(newAttr.EdgeType is ASingleReferenceEdgeType))
                        {
                            RemoveRecentlyAddedTypes(addedTypes);
                            return new Exceptional<QueryResult>(new Error_InvalidEdgeType(newAttr.EdgeType.GetType(), typeof(ASingleReferenceEdgeType)));
                        }
                        else if (newAttr.KindOfType == KindsOfType.SetOfReferences || newAttr.KindOfType == KindsOfType.SetOfNoneReferences)
                        {
                            if (attrType.IsUserDefined && !(newAttr.EdgeType is ASetOfReferencesEdgeType))
                            {
                                RemoveRecentlyAddedTypes(addedTypes);
                                return new Exceptional<QueryResult>(new Error_InvalidEdgeType(newAttr.EdgeType.GetType(), typeof(ASetOfReferencesEdgeType)));
                            }
                            else if (!attrType.IsUserDefined && !(newAttr.EdgeType is ASetOfBaseEdgeType))
                            {
                                RemoveRecentlyAddedTypes(addedTypes);
                                return new Exceptional<QueryResult>(new Error_InvalidEdgeType(newAttr.EdgeType.GetType(), typeof(AListOfBaseEdgeType)));
                            }
                        }

                        #endregion

                        aType.AddAttribute(newAttr, this, true);
                    }

                    #endregion

                    #region Set BackwardEdges

                    if (!aTypeDef.BackwardEdgeNodes.IsNullOrEmpty())
                    {
                        backwardEdgesToBeAddedAfterwards.Add(aType, aTypeDef.BackwardEdgeNodes);
                    }

                    #endregion
                }

                #endregion

                #region Add the BackwardEdges

                foreach (var beDefinition in backwardEdgesToBeAddedAfterwards)
                {
                    var aType = beDefinition.Key;
                    UInt16 beAttrCounter = 0;
                    foreach (var be in beDefinition.Value)
                    {

                        var bedgeAttribute = CreateBackwardEdgeAttribute(be, aType, beAttrCounter);

                        if (!bedgeAttribute.Success())
                        {
                            RemoveRecentlyAddedTypes(addedTypes);
                            return new Exceptional<QueryResult>(bedgeAttribute);
                        }

                        aType.AddAttribute(bedgeAttribute.Value, this, true);

                        beAttrCounter++;
                    }
                }

                #endregion

                #region Validate Attribute dependencies
                                
                List<DBObjectReadout> readOutList   = new List<DBObjectReadout>();
                SelectionResultSet    selResult     = null;
                
                foreach (GraphDBType aType in addedTypes)
                {
                    foreach (GraphDBType _GraphType in GetAllParentTypes(aType, false, true))
                    {
                        var _MandatoryAttr = _GraphType.GetMandatoryAttributesUUIDs(this);
                        List<AttributeUUID> _UniqueAttr = _GraphType.GetAllUniqueAttributes(false, this);

                        foreach (TypeAttribute ta in aType.Attributes.Values)
                        {
                            if (_GraphType.GetTypeAttributeByName(ta.Name) != null)
                            {
                                // Todo: Use notification here
                                RemoveRecentlyAddedTypes(addedTypes);
                                return new Exceptional<QueryResult>(new Error_AttributeExistsInSupertype(ta.Name, aType.Name));
                            }

                            #region unique and mandatory attributes

                            if (ta.TypeCharacteristics.IsUnique && !_UniqueAttr.Contains(ta.UUID))
                            {
                                //if the attrbute has been marked unique and it is not contained in uniques of super types
                                if (!uniqueAttrIDs.Contains(ta.UUID))
                                {
                                    uniqueAttrIDs.Add(ta.UUID);
                                }
                            }

                            if (ta.TypeCharacteristics.IsMandatory && !_MandatoryAttr.Contains(ta.UUID))
                            {
                                aType.AddMandatoryAttribute(ta.UUID, this);
                            }

                            #endregion
                        }
                    }

                    //Add the unique attribute ids for the current type
                    var AddUniqueAttrExcept = aType.AddUniqueAttributes(uniqueAttrIDs, currentContext);

                    if(AddUniqueAttrExcept.Failed())
                        return new Exceptional<QueryResult>(AddUniqueAttrExcept);

                    uniqueAttrIDs.Clear();
                }
                
                #endregion

                //#region add attribute lookup table of parent type to the actual one

                //foreach (var aAddedType in addedTypes)
                //{
                //    foreach (var aLookupAttributeInParentType in aAddedType.GetParentType(this).AttributeLookupTable)
                //    {
                //        if (!aAddedType.AttributeLookupTable.ContainsKey(aLookupAttributeInParentType.Key))
                //        {
                //            aAddedType.AttributeLookupTable.Add(aLookupAttributeInParentType.Key, aLookupAttributeInParentType.Value);
                //        }
                //    }
                //}

                //#endregion

                #region Create indices

                foreach (GraphDBType aType in addedTypes)
                {
                    #region Create userdefined Indices

                    var aTypeDef = TypeList.Where(item => item.Name == aType.Name).FirstOrDefault();

                    if (!aTypeDef.Indices.IsNullOrEmpty())
                    {
                        foreach (var index in aTypeDef.Indices)
                        {

                            if (!index.IndexAttributeDefinitions.All(node => !node.IndexAttribute.Validate(currentContext, false, aType).Failed()))
                            {
                                RemoveRecentlyAddedTypes(addedTypes);
                                return new Exceptional<QueryResult>(new Error_AttributeIsNotDefined(aType.Name, aType.Name));
                            }
                            
                            var idxName = index.IndexName;
                            if (String.IsNullOrEmpty(index.IndexName))
                            {
                                idxName = index.IndexAttributeDefinitions.Aggregate(new StringBuilder(DBConstants.IndexKeyPrefix), (stringB, elem) => { stringB.Append(String.Concat(DBConstants.IndexKeySeperator, elem.IndexAttribute.LastAttribute.Name)); return stringB; }).ToString();
                            }

                            List<AttributeUUID> indexAttrs = new List<AttributeUUID>(index.IndexAttributeDefinitions.Select(node => node.IndexAttribute.LastAttribute.UUID));

                            foreach (var item in GetAllSubtypes(aType))
                            {
                                var CreateIdxExcept = item.CreateAttributeIndex(currentContext, idxName, indexAttrs, index.Edition, index.IndexType);

                                if (!CreateIdxExcept.Success())
                                    return new Exceptional<QueryResult>(CreateIdxExcept);
                            }
                        }
                    }

                    #endregion

                    //UUID index                    
                    var createIndexExcept = aType.CreateUUIDIndex(_DBContext, GetUUIDTypeAttribute().UUID);

                    if (!createIndexExcept.Success())
                    {
                        return new Exceptional<QueryResult>(createIndexExcept);
                    }

                    List<AttributeUUID> UniqueIDs = aType.GetAllUniqueAttributes(true, this);
                    
                    if (UniqueIDs.Count() > 0)
                    {
                        var idxName = _DBContext.DBIndexManager.GetUniqueIndexName(UniqueIDs, aType); // UniqueIDs.Aggregate<AttributeUUID, String>("Idx", (result, item) => result = result + "_" + aType.GetTypeAttributeByUUID(item).Name);
                        var createIdxExcept = aType.CreateUniqueAttributeIndex(currentContext, idxName, UniqueIDs, DBConstants.UNIQUEATTRIBUTESINDEX);

                        if (!createIdxExcept.Success())
                            return new Exceptional<QueryResult>(createIdxExcept);
                    }
                }

                #endregion

                #region flush to fs

                
                foreach (var item in addedTypes)
                {
                    var createException = CreateTypeOnFS(item);

                    if (!createException.Success())
                        return new Exceptional<QueryResult>(createException);

                    #region get system attributes from type

                    var readOut = GetTypeReadouts(item);

                    if (!readOut.Success())
                        return new Exceptional<QueryResult>(readOut.Errors.First());

                    readOutList.Add(readOut.Value);

                    #endregion
                }

                selResult = new SelectionResultSet(readOutList);
                result.SetResult(selResult);
                

                #endregion

            }
            catch (GraphDBException ee)
            {
                RemoveRecentlyAddedTypes(addedTypes);

                var _Exceptional = new Exceptional<QueryResult>();
                foreach (var _ex in ee.GraphDBErrors)
                    _Exceptional.Push(_ex);

                return _Exceptional;

            }
            catch (GraphDBWarningException ee)
            {
                RemoveRecentlyAddedTypes(addedTypes);
                return new Exceptional<QueryResult>(ee.GraphDBWarning);
            }
            catch (Exception e)
            //finally
            {
                //if (!succeeded)
                //{
                addedTypes.ForEach(item =>
                {
                    _UserDefinedTypes.Remove(item.UUID);
                    _TypesNameLookUpTable.Remove(item.Name);
                });
                //}

                return new Exceptional<QueryResult>(new Error_UnknownDBError(e));

            }

            return new Exceptional<QueryResult>(result);

        }

        private void RemoveRecentlyAddedTypes(List<GraphDBType> addedTypes)
        {
            addedTypes.ForEach(item =>
            {
                _UserDefinedTypes.Remove(item.UUID);
                _TypesNameLookUpTable.Remove(item.Name);
            });
        }

        #endregion

        #region CreateBackwardEdgeAttribute(myBackwardEdgeNode, myDBTypeStream)

        public Exceptional<TypeAttribute> CreateBackwardEdgeAttribute(BackwardEdgeDefinition myBackwardEdgeNode, GraphDBType myDBTypeStream, UInt16 beAttrCounter = 0)
        {

            var edgeType      = GetTypeByName(myBackwardEdgeNode.TypeName);
            
            if (edgeType == null)
                return new Exceptional<TypeAttribute>(new Error_TypeDoesNotExist(myBackwardEdgeNode.TypeName));

            var edgeAttribute = edgeType.GetTypeAttributeByName(myBackwardEdgeNode.TypeAttributeName);

            //error if the attribute does not exist
            #region 
            if (edgeAttribute == null)
            {
                return new Exceptional<TypeAttribute>(new Error_AttributeIsNotDefined(edgeType.Name, myBackwardEdgeNode.TypeAttributeName));
            }
            #endregion

            //error if the attribute does not represent non userdefined content
            #region
            if (!edgeAttribute.GetDBType(this).IsUserDefined)
            {
                return new Exceptional<TypeAttribute>(new Error_BackwardEdgesForNotReferenceAttributeTypesAreNotAllowed(myBackwardEdgeNode.TypeAttributeName));
            }
            #endregion

            //invalid backwardEdge destination
            #region
            if (edgeAttribute.GetDBType(this) != myDBTypeStream)
                return new Exceptional<TypeAttribute>(new Error_BackwardEdgeDestinationIsInvalid(myDBTypeStream.Name, myBackwardEdgeNode.TypeAttributeName));
            #endregion

            //error if there is already an be attribute on the to be changed type that points to the same destination
            #region

            var edgeKey = new EdgeKey(edgeType.UUID, edgeAttribute.UUID);
            if (myDBTypeStream.Attributes.Exists(aKV => aKV.Value.IsBackwardEdge && (aKV.Value.BackwardEdgeDefinition == edgeKey)))
            {
                return new Exceptional<TypeAttribute>(new Error_BackwardEdgeAlreadyExist(myDBTypeStream, edgeType.Name, edgeAttribute.Name));
            }

            #endregion

            //error if the backwardEdge points to a backward edge
            #region
            var beDestinationAttribute = edgeKey.GetTypeAndAttributeInformation(this).Item2;

            if (beDestinationAttribute.IsBackwardEdge)
                return new Exceptional<TypeAttribute>(new Error_BackwardEdgeAlreadyExist(myDBTypeStream, edgeType.Name, edgeAttribute.Name));
            #endregion

            var ta = new TypeAttribute(Convert.ToUInt16(beAttrCounter + DBConstants.DefaultBackwardEdgeIDStart));
            ta.DBTypeUUID = DBBackwardEdgeType.UUID;
            ta.BackwardEdgeDefinition = edgeKey;
            ta.KindOfType = KindsOfType.SetOfReferences;
            ta.Name = myBackwardEdgeNode.AttributeName;
            ta.EdgeType = myBackwardEdgeNode.EdgeType.GetNewInstance();
            ta.TypeCharacteristics.IsBackwardEdge = true;
            ta.RelatedGraphDBTypeUUID = myDBTypeStream.UUID;

            return new Exceptional<TypeAttribute>(ta);

        }

        #endregion

        #region AddType(myTypeName, myParentType, myAttributes)

        private Exceptional<GraphDBType> AddingType(String myTypeName, String myParentType, Dictionary<AttributeDefinition, String> myAttributes, Boolean myIsAbstract, String myComment)
        {

            var ptd = new GraphDBTypeDefinition(myTypeName, myParentType, false, myAttributes, null, null, myComment);

            var errors = AddBulkTypes(new List<GraphDBTypeDefinition>(new[] { ptd }), _DBContext);

            if (errors.Failed())
                return new Exceptional<GraphDBType>(errors);

            else
                return new Exceptional<GraphDBType>(GetTypeByName(myTypeName));        

        }

        /// <summary>
        /// Adds the given type to the list of types.
        /// </summary>
        /// <param name="Type">The type to be added.</param>
        /// <param name="myParentType">parent types of the type which should be added</param>
        /// <param name="myIsAbstract">flag for an abstract type</param>
        public Exceptional<GraphDBType> AddType(String myTypeName, String myParentType, Dictionary<AttributeDefinition, String> myAttributes, Boolean myIsAbstract, String myComment)
        {
            return AddingType(myTypeName, myParentType, myAttributes, myIsAbstract, myComment);
        }
        
        /// <summary>
        /// Adds the given type to the list of types.
        /// </summary>
        /// <param name="Type">The type to be added.</param>
        public Exceptional<GraphDBType> AddType(String myTypeName, String myParentType, Dictionary<AttributeDefinition, String> myAttributes, String myComment)
        {
            return AddingType(myTypeName, myParentType, myAttributes, false, myComment);
        }

        [Obsolete("Change TypeAttribute to AttributeDefinition in all tests!")]
        public Exceptional<GraphDBType> AddType(String myTypeName, String myParentType, Dictionary<TypeAttribute, string> myAttributes, String myComment)
        {
            var attrs = new Dictionary<AttributeDefinition, String>();
            foreach (var val in myAttributes)
            {
                attrs.Add(new AttributeDefinition(new DBTypeOfAttributeDefinition() { Name = val.Value, Type = val.Key.KindOfType, TypeCharacteristics = val.Key.TypeCharacteristics }, val.Key.Name, null), val.Value);
            }
            return AddingType(myTypeName, myParentType, attrs, false, myComment);
        }

        #endregion

        #region HasParentType(derivedTypeName, superTypeName)

        /// <summary>
        /// Returns true, if the type with myClassName baseTypeName is "a" base type to the type with 
        /// myClassName superTypeName.
        /// </summary>
        /// <param name="derivedTypeName">The name of the derived class.</param>
        /// <param name="superTypeName">The name of the super class.</param>
        /// <returns>True, if the super class is a supertype to derived type.</returns>
        public Exceptional<Boolean> HasParentType(TypeUUID derivedTypeUUID, TypeUUID superTypeUUID)
        {

            #region INPUT EXCEPTIONS

            if (derivedTypeUUID == null)
                return new Exceptional<bool>(new Error_ArgumentNullOrEmpty("UUID of derived type should not be null."));

            if (superTypeUUID == null)
                return new Exceptional<bool>(new Error_ArgumentNullOrEmpty("UUID of super type should not be null."));

            if (GetTypeByUUID(derivedTypeUUID) == null)
                return new Exceptional<bool>(new Error_TypeDoesNotExist(""));

            if (GetTypeByUUID(superTypeUUID) == null)
                return new Exceptional<bool>(new Error_TypeDoesNotExist(""));

            #endregion

            bool result = false;
            GraphDBType derivedType = GetTypeByUUID(derivedTypeUUID);

            while (derivedType.ParentTypeUUID != null && !derivedType.UUID.Equals(superTypeUUID))
            {
                derivedType = GetTypeByUUID(derivedType.ParentTypeUUID);
            }

            if (derivedType.UUID.Equals(superTypeUUID))
                result = true;

            return new Exceptional<Boolean>(result);
        }

        #endregion


        #region CreateTypeOnFS(myGraphType)

        private Exceptional<Boolean> CreateTypeOnFS(GraphDBType myGraphType)
        {
            using (var _Transaction = _IGraphFSSession.BeginFSTransaction())
            {
                var CreateException = CreateTypeOnFS_internal(myGraphType);

                if (CreateException.Failed())
                    return new Exceptional<bool>(CreateException);
                
                _Transaction.Commit();
            }

            return new Exceptional<bool>(true);
        }

        #endregion

        #region CreateTypeOnFS_internal(myGraphType)

        private Exceptional<Boolean> CreateTypeOnFS_internal(GraphDBType myGraphType)
        {

            #region Data

            var typeName = myGraphType.Name;
            var typeDir  = myGraphType.ObjectLocation;

            #endregion
            
            #region Change the database instance settings containing the StorageLocation paths (Object Schemes)

            lock (_ObjectLocationsOfAllUserDefinedDatabaseTypes)
            {
                _ObjectLocationsOfAllUserDefinedDatabaseTypes.Add(typeDir.ToString());
                _ObjectLocationsOfAllUserDefinedDatabaseTypes.Save();
            }

            #endregion

            #region Store the new type on the file system

            var isDirExcept = _IGraphFSSession.isIDirectoryObject(typeDir);

            if(isDirExcept.Failed())
                return new Exceptional<bool>(isDirExcept);
            
            if (isDirExcept.Value == Trinary.TRUE)
                return new Exceptional<Boolean>(new Error_UnknownDBError("Default directory for the new type " + typeName + " could not be created, cause it already exists."));

            #region Get blocksize of the TypeDirectory from "TYPEDIRBLOCKSIZE" setting

            UInt64 TypeDirBlockSize = 0;
            var blocksizeSetting = new SettingTypeDirBlocksize();
            var _TypeDireBlockSizeExceptional = blocksizeSetting.Get(_DBContext, TypesSettingScope.DB);

            if (!_TypeDireBlockSizeExceptional.Success())
            {
                return new Exceptional<Boolean>(_TypeDireBlockSizeExceptional);
                //throw new GraphDBException(new Error_SettingDoesNotExist("TYPEDIRBLOCKSIZE"));
            }

            if (_TypeDireBlockSizeExceptional.Value == null)
            {
                TypeDirBlockSize = (UInt64)(Int64)blocksizeSetting.Default.Value;
            }
            else
            {
                TypeDirBlockSize = (UInt64)(Int64)_TypeDireBlockSizeExceptional.Value.Value.Value;
            }

            #endregion

            // Create the directory for the new type
            var CreateDirExcept = _IGraphFSSession.CreateDirectoryObject(typeDir, TypeDirBlockSize);

            if (CreateDirExcept.Failed())
                return new Exceptional<bool>(CreateDirExcept);

            // Create a subdirectory for the objects of this new type
            CreateDirExcept = _IGraphFSSession.CreateDirectoryObject(new ObjectLocation(typeDir, DBConstants.DBObjectsLocation));
            
            if (CreateDirExcept.Failed())
                return new Exceptional<bool>(CreateDirExcept);

            // Create a subdirectory for the indices of this new type
            CreateDirExcept = _IGraphFSSession.CreateDirectoryObject(new ObjectLocation(typeDir, DBConstants.DBIndicesLocation));

            if (CreateDirExcept.Failed())
                return new Exceptional<bool>(CreateDirExcept);

            #endregion

            var FlushExcept = FlushType(myGraphType);

            if (FlushExcept.Failed())
                return new Exceptional<bool>(FlushExcept);

            return new Exceptional<bool>(true);
        }

        #endregion    


        #region FlushType(GraphType myGraphType)

        public Exceptional FlushType(GraphDBType myGraphType)
        {
            
            var _Exceptional = _IGraphFSSession.StoreFSObject(myGraphType, true);

            if (_Exceptional == null || _Exceptional.Failed())
                return new Exceptional(_Exceptional);

            return new Exceptional();

        }

        #endregion


        #region RemoveType(myType)

        /// <summary>
        /// Removes the given type (and its list types) and all subtypes. The generic list types cant 
        /// get removed, cause they get created/deleted autopmatically whenevery a content type 
        /// was added/removed by the user. Furthermore all myAttributes of the deleted types of other 
        /// GraphTypes will be also deleted.
        /// </summary>
        /// <param name="Type">The type to be removed.</param>
        /// <returns>True, if the type was removed. And false, if the type with that name, wasnt contained.</returns>
        public Exceptional<Boolean> RemoveType(GraphDBType myType)
        {

            #region INPUT EXCEPTIONS

            if (myType == null)
            {
                return new Exceptional<bool>(new Error_ArgumentNullOrEmpty("The DBType should not be null."));
            }

            #endregion

            #region Remove type

            if (_UserDefinedTypes.ContainsKey(myType.UUID))
            {
                var removeTypeExcept = ProcessTypeRemoval(GetAllSubtypes(myType), myType);

                if (removeTypeExcept.Failed())
                    return new Exceptional<bool>(removeTypeExcept);
            }
            else
            {
                Debug.WriteLine("[ TypeManager -> RemoveType(String TypeName) ] " + "Type " + myType + " could not be deleted, because no such user defined type does exist.");
                return new Exceptional<bool>(new GraphDBError("[ TypeManager -> RemoveType(String TypeName) ] " + "Type " + myType + " could not be deleted, because no such user defined type does exist."));
            }

            #endregion

            if (!_UserDefinedTypes.ContainsKey(myType.UUID))
            {
                if (!_ObjectLocationsOfAllUserDefinedDatabaseTypes.Contains(myType.ObjectLocation.ToString()))
                {

                    var existExcept = _IGraphFSSession.ObjectExists(myType.ObjectLocation);

                    if (existExcept.Failed())
                        return new Exceptional<bool>(existExcept);

                    if (existExcept.Value != Trinary.TRUE)
                        return new Exceptional<bool>(true);

                }
            }

            return new Exceptional<bool>(false);

        }

        #endregion

        #region ProcessListTypeRemoval(toDelete)

        private Exceptional<Boolean> ProcessTypeRemoval(List<GraphDBType> toDelete, GraphDBType startingType)
        {

            foreach (var aType in _UserDefinedTypes)
            {
                //remove attributes
                foreach (var deprecatedAttribute in (from attribute in aType.Value.Attributes where toDelete.Contains(attribute.Value.GetDBType(this)) select attribute.Value).ToList())
                {
                    RemoveAttributeOfType(aType.Value, deprecatedAttribute.UUID);
                }
            }

            foreach (var toBeDeletedTypes in toDelete)
            {

                #region remove from userdefined types

                if (!_UserDefinedTypes.Remove(toBeDeletedTypes.UUID))
                {
                    return new Exceptional<bool>(new Error_TypeDoesNotExist(toBeDeletedTypes.Name));
                }
                _TypesNameLookUpTable.Remove(toBeDeletedTypes.Name);

                #endregion

                #region remove from ObjectScheme

                try
                {

                    var DbObjectSchemeSettingsDest = _DatabaseRootPath + DBConstants.DBTypeLocations;
                    _ObjectLocationsOfAllUserDefinedDatabaseTypes.Remove(toBeDeletedTypes.ObjectLocation.ToString());
                    _ObjectLocationsOfAllUserDefinedDatabaseTypes.Save();

                }
                catch (Exception e)
                {
                    return new Exceptional<bool>(new Error_UnknownDBError(e));
                }

                #endregion

                #region remove type from fs

                var removeTypeExcept = RemoveTypeFromFs(toBeDeletedTypes.ObjectLocation);

                if (removeTypeExcept.Failed())
                {
                    return new Exceptional<bool>(removeTypeExcept);
                }

                #endregion
            
            }

            return new Exceptional<bool>(true);

        }


        #endregion

        #region RemoveTypeFromFs(typeDir)

        /// <summary>
        /// Removes GraphTypes from filesystem
        /// </summary>
        /// <param name="typeDir">The directory of the GraphType that is going to be deleted.</param>
        /// <returns>True for success or otherwise false.</returns>
        private Exceptional<Boolean> RemoveTypeFromFs(ObjectLocation typeDir)
        {

            using (var _Transaction = _IGraphFSSession.BeginFSTransaction())
            {

                var removeObjectExcept = _IGraphFSSession.RemoveFSObject(typeDir, DBConstants.DBTYPESTREAM, null, null);

                if (removeObjectExcept.Failed())
                {
                    return new Exceptional<bool>(removeObjectExcept); // return and rollback transaction
                }

                var removeDirExcept = _IGraphFSSession.RemoveDirectoryObject(typeDir, true);

                if (removeDirExcept.Failed())
                {
                    return new Exceptional<bool>(removeDirExcept); // return and rollback transaction
                }

                _Transaction.Commit();

            }

            return new Exceptional<bool>(true);
        }

        #endregion

        #region RenameType
        public Exceptional<Boolean> RenameType(GraphDBType myType, String newName)
        {
            if (myType == null)
                return new Exceptional<Boolean>(new Error_ArgumentNullOrEmpty("myType"));

            if (String.IsNullOrEmpty(newName))
                return new Exceptional<Boolean>(new Error_ArgumentNullOrEmpty("newName"));

            if (GetTypeByName(newName) != null)
                return new Exceptional<Boolean>(new Error_TypeAlreadyExist(newName));

            String oldName = myType.Name;
            String oldLocation = myType.ObjectLocation.ToString();

            var retVal = myType.Rename(newName);

            if (retVal.Failed())
                return new Exceptional<Boolean>(retVal);

            _TypesNameLookUpTable.Remove(oldName);
            _TypesNameLookUpTable.Add(newName, myType);
            _ObjectLocationsOfAllUserDefinedDatabaseTypes.Remove(oldLocation);
            _ObjectLocationsOfAllUserDefinedDatabaseTypes.Add(myType.ObjectLocation.ToString());
            
            var saveResult = _ObjectLocationsOfAllUserDefinedDatabaseTypes.Save();
            if (!saveResult.Success())
            {
                return new Exceptional<bool>(saveResult);
            }

            return new Exceptional<Boolean>(true);
        }
        #endregion

        public Exceptional ChangeCommentOnType(GraphDBType myGraphDBType, String myComment)
        {

            if (myGraphDBType == null)
                return new Exceptional<Boolean>(new Error_ArgumentNullOrEmpty("myGraphDBType"));

            if (String.IsNullOrEmpty(myComment))
                return new Exceptional<Boolean>(new Error_ArgumentNullOrEmpty("myComment"));

            myGraphDBType.SetComment(myComment);

            var flushExcept = FlushType(myGraphDBType);

            if (!flushExcept.Success())
            {
                return new Exceptional<Boolean>(true);
            }
            
            else
            {
                return new Exceptional<Boolean>(flushExcept);
            }

        }

        #endregion

        #region Alter type methods

        public Exceptional<QueryResult> AlterType(DBContext dbInnerContext, GraphDBType atype, AAlterTypeCommand alterTypeCommand)
        {
            var result = alterTypeCommand.Execute(dbInnerContext, atype);

            if (!result.Success())
            {
                var retVal = new Exceptional<QueryResult>(result);
                return retVal;
            }
            else
            {   
                SelectionResultSet resultReadout = alterTypeCommand.CreateReadout(dbInnerContext, atype);

                if (resultReadout != null)
                {
                    Exceptional<QueryResult>  retVal = new Exceptional<QueryResult>(new QueryResult(resultReadout));
                    retVal.AddErrorsAndWarnings(result);

                    return retVal;
                }
                
                return new Exceptional<QueryResult>(result);
            }
        }

        #endregion

        #region Some Gettings

        #region Get type methods

        #region GetTypeByName(TypeName)

        /// <summary>
        /// Returns the GraphType which has the name Name.
        /// </summary>
        /// <param name="Name">Name of the Type.</param>
        /// <returns>The GraphType, if it exists. Else, null.</returns>
        public GraphDBType GetTypeByName(String myTypeName)
        {

            #region Input Validation

            if (myTypeName == null)
            {
                throw new ArgumentNullException("The parameter myTypeName must not be null!");
            }

            #endregion

            GraphDBType _GraphType = null;

            _TypesNameLookUpTable.TryGetValue(myTypeName, out _GraphType);

            return _GraphType;

        }

        #endregion

        #region GetTypeByUUID(myUUID)

        public GraphDBType GetTypeByUUID(TypeUUID myUUID)
        {

            #region Input Validation

            if (myUUID == null)
                throw new ArgumentNullException("The parameter myTypeName must not be null!");

            #endregion

            if (_SystemTypes.ContainsKey(myUUID))
            {
                return _SystemTypes[myUUID];
            }
            else
            {
                if (_BasicTypes.ContainsKey(myUUID))
                {
                    return _BasicTypes[myUUID];
                }
                else
                {
                    if (_UserDefinedTypes.ContainsKey(myUUID))
                    {
                        return _UserDefinedTypes[myUUID];
                    }
                }
            }

            return null;
        }

        #endregion

        #region GetAllTypes()

        /// <summary>
        /// This method returns all GraphTypes that are currently loaded by the TypeManager.
        /// BasicGraphTypes are also included.
        /// </summary>
        /// <returns>A list of Strings that with the names of the GraphTypes that are currently loaded by the TypeManager</returns>
        public IEnumerable<GraphDBType> GetAllTypes(Boolean includeBasicTypes = true)
        {
            if (includeBasicTypes)
            {
                foreach (var type in _BasicTypes)
                {
                    yield return type.Value;
                }
            }
            foreach (var type in _UserDefinedTypes)
            {
                yield return type.Value;
            }
        }

        #endregion

        #endregion

        #region GetTypeAttribute

        /// <summary>
        /// This returns the TypeAttribute defined by an edge
        /// </summary>
        /// <param name="myEdgeKey">An EdgeKey containing the TypeUUID and AttributeUUID</param>
        /// <returns>TypeAttribute defined by an edge</returns>
        public TypeAttribute GetTypeAttributeByEdge(EdgeKey myEdgeKey)
        {
            return GetTypeByUUID(myEdgeKey.TypeUUID).GetTypeAttributeByUUID(myEdgeKey.AttrUUID);
        }

        /// <summary>
        /// Do not use this method if you have to care about performance!!!
        /// It will return a TypeAttribute of the AttributeUUID
        /// </summary>
        /// <param name="myAttributeUUID"></param>
        /// <returns>TypeAttribute or null if it was not found</returns>
        public TypeAttribute GetTypeAttributeByAttributeUUID(AttributeUUID myAttributeUUID)
        {
            var found = from table in _TypesNameLookUpTable
                        where table.Value.Attributes.ContainsKey(myAttributeUUID)
                        select table.Value.Attributes[myAttributeUUID];

            if (found.Count() == 0)
                return null;

            return found.First();
        }

        #endregion

        #region GetAllSubtypes

        #region GetAllSubtypes(String myGraphDBType)

        public List<GraphDBType> GetAllSubtypes(String myGraphDBType)
        {
            return GetAllSubtypes(GetTypeByName(myGraphDBType));
        }

        #endregion

        #region GetAllSubtypes(myGraphDBType)

        /// <summary>
        /// Resturns all GraphTypes, that are GraphObjects and that are subtypes of the Type, which name is given.
        /// These are all child types - all types which derives from the <paramref name="myGraphDBType"/>
        /// </summary>
        /// <param name="Name">The name of the supertype.</param>
        /// <returns>The list of all subtypes.</returns>
        public List<GraphDBType> GetAllSubtypes(GraphDBType myGraphDBType, Boolean myThisTypeIncluding = true)
        {

            #region INPUT EXCEPTIONS

            if (myGraphDBType == null) return null;
            if (!_UserDefinedTypes.ContainsKey(myGraphDBType.UUID) && !myGraphDBType.UUID.Equals(DBVertex.UUID)) return null;

            #endregion

            List<GraphDBType> result = new List<GraphDBType>();
            result.Add(myGraphDBType);

            bool somethingToDo = true;
            while (somethingToDo)
            {
                somethingToDo = false;
                foreach (var type in _UserDefinedTypes)
                {
                    List<GraphDBType> tmpTypeList = new List<GraphDBType>();
                    GraphDBType aType = (GraphDBType)type.Value;

                    foreach (GraphDBType subtypeAllreadyFound in result)
                    {
                        if (aType.ParentTypeUUID.Equals(subtypeAllreadyFound.UUID) &&
                            !tmpTypeList.Contains(aType) && !result.Contains(aType))
                        {
                            somethingToDo = true;
                            tmpTypeList.Add(aType);
                        }
                    }

                    foreach (GraphDBType recentSelectedTypes in tmpTypeList)
                        result.Add(recentSelectedTypes);
                }
            }

            #region special handling for GraphObject, cause it is not in the userdefinedtypes dict

            if (myGraphDBType.UUID.Equals(DBBaseObject.UUID))
                result.Add(myGraphDBType);

            #endregion

            if (!myThisTypeIncluding)
                result.Remove(myGraphDBType);

            return result;
        }
        
        #endregion

        #endregion

        #region GetAllParentTypes

        /// <summary>
        /// Get all parent type (the type from which the <paramref name="myType"/> derives)
        /// </summary>
        /// <param name="myType"></param>
        /// <param name="myThisTypeIncluding">If true, the <paramref name="myType"/> is added to the result.</param>
        /// <returns></returns>
        public IEnumerable<GraphDBType> GetAllParentTypes(GraphDBType myType, Boolean myThisTypeIncluding, Boolean includeSystemTypes)
        {

            #region INPUT EXCEPTIONS

            if (myType == null)
            {
                throw new GraphDBException(new Error_ArgumentNullOrEmpty("DBType should not be null."));
            }

            #endregion

            List<GraphDBType> result = new List<GraphDBType>();

            if (myThisTypeIncluding)
                result.Add(myType);

            if (myType.ParentTypeUUID != null)
            {
                if (myType.GetParentType(this).IsUserDefined)
                {
                    result.AddRange(GetAllParentTypes(myType.GetParentType(this), true, includeSystemTypes));
                }
                else
                {
                    if (includeSystemTypes)
                    {
                        result.AddRange(GetAllParentTypes(myType.GetParentType(this), true, includeSystemTypes));
                    }
                }
            }

            return result;
        }

        #endregion

        #region GetGUIDTypeAttribute

        public TypeAttribute GetUUIDTypeAttribute()
        {
            return _GUIDTypeAttribute;
        }

        #endregion

        #endregion

        public Exceptional<DirectoryObject> GetObjectsDirectory(GraphDBType myTypeOfDBObject)
        {
            return _IGraphFSSession.GetFSObject<DirectoryObject>(new ObjectLocation(myTypeOfDBObject.ObjectLocation, DBConstants.DBObjectsLocation));
        }

    }

}
