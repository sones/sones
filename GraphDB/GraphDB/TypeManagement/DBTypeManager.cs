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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.Objects;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.GraphDB.Indices;
using sones.GraphDB.Settings;
using sones.GraphDB.Plugin;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Exceptions;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.Lib.DataStructures;
using sones.GraphFS;
using sones.GraphDB.Structures;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Warnings;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.Lib;
using sones.GraphDB.QueryLanguage.Enums;
using System.Diagnostics;

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
        private String _DatabaseRootPath;

        /// <summary>
        /// The myIPandoraFS where the information is stored. Remove when InstanceSettings contain the myIPandoraFS.
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
        /// <param name="myIPandoraDBSession">The filesystem where the information is stored.</param>
        /// <param name="DatabaseRootPath">The database root path.</param>
        public DBTypeManager(IGraphFSSession myIPandoraFS, ObjectLocation myDatabaseRootPath, EntityUUID myUserID, Dictionary<String, ADBSettingsBase> myDBSettings, DBContext dbContext)
        {

            _DBContext = dbContext;
            _UserID = myUserID;
            _DatabaseRootPath                               = myDatabaseRootPath;
            _IGraphFSSession                                = myIPandoraFS;
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
            var listOfLocations = _IGraphFSSession.GetOrCreateObject<ListOfStringsObject>(new ObjectLocation(myDatabaseRootPath, DBConstants.DBTypeLocations), FSConstants.LISTOF_STRINGS, null, null, 0, false);

            if (listOfLocations.Failed)
            {
                throw new GraphDBException(listOfLocations.Errors);
            }

            return listOfLocations.Value;
        }

        #endregion

        #region Init(myIPandoraFS, myDatabaseLocation)

        /// <summary>
        /// Initializes the type manager. This method may only be called once, else it throws an TypeInitializationException.
        /// </summary>
        /// <param name="myIPandoraFS">The myIPandoraFS, on which the database is stored.</param>
        /// <param name="myDatabaseLocation">The databases root path in the myIPandoraFS.</param>
        public Exceptional Init(IGraphFSSession myIPandoraFS, ObjectLocation myDatabaseLocation, Boolean myRebuildIndices)
        {

            #region Input validation

            if (myIPandoraFS == null)
                return new Exceptional<bool>(new Error_ArgumentNullOrEmpty("The parameter myIPandoraFS must not be null!"));
            
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

            var typeDBReference = new GraphDBType(DBReference.UUID, myDatabaseLocation, DBReference.Name, DBBaseObject.UUID, new Dictionary<AttributeUUID, TypeAttribute>(), false, false, "The base of all user defined database types");

            #region UUID special Attribute

            var specialTypeAttribute_UUID = new SpecialTypeAttribute_UUID() { DBTypeUUID = DBReference.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_UUID.UUID, specialTypeAttribute_UUID);
            _GUIDTypeAttribute = specialTypeAttribute_UUID;

            #endregion

            #region CreationTime special attribute

            var specialTypeAttribute_CrTime = new SpecialTypeAttribute_CREATIONTIME() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_CrTime.UUID, specialTypeAttribute_CrTime);

            #endregion
            
            #region DeletionTime special attribute

            var specialTypeAttribute_DelTime = new SpecialTypeAttribute_DELETIONTIME() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_DelTime.UUID, specialTypeAttribute_DelTime);

            #endregion

            #region Edition special attribute

            var specialTypeAttribute_Edition = new SpecialTypeAttribute_EDITION() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_Edition.UUID, specialTypeAttribute_Edition);

            #endregion

            #region Editions special attribute

            var specialTypeAttribute_Editions = new SpecialTypeAttribute_EDITIONS() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_Editions.UUID, specialTypeAttribute_Editions);

            #endregion

            #region LastAccessTime special attribute

            var specialTypeAttribute_AcTime = new SpecialTypeAttribute_LASTACCESSTIME() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_AcTime.UUID, specialTypeAttribute_AcTime);

            #endregion

            #region LastModificationTime special attribute

            var specialTypeAttribute_LastModTime = new SpecialTypeAttribute_LASTMODIFICATIONTIME() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_LastModTime.UUID, specialTypeAttribute_LastModTime);

            #endregion            

            #region TypeName special Attribute

            var specialTypeAttribute_TYPE = new SpecialTypeAttribute_TYPE() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_TYPE.UUID, specialTypeAttribute_TYPE);

            #endregion

            #region REVISION special Attribute

            var specialTypeAttribute_REVISION = new SpecialTypeAttribute_REVISION() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_REVISION.UUID, specialTypeAttribute_REVISION);

            #endregion

            #region REVISIONS special Attribute

            var specialTypeAttribute_REVISIONS = new SpecialTypeAttribute_REVISIONS() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_REVISIONS.UUID, specialTypeAttribute_REVISIONS);

            #endregion

            #region STREAMS special Attribute

            var specialTypeAttribute_STREAMS = new SpecialTypeAttribute_STREAMS() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_STREAMS.UUID, specialTypeAttribute_STREAMS);

            #endregion

            #region NUMBER OF REVISIONS Attribute

            var specialTypeAttribute_NUMBEROFREVISIONS = new SpecialTypeAttribute_NUMBEROFREVISIONS() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_NUMBEROFREVISIONS.UUID, specialTypeAttribute_NUMBEROFREVISIONS);

            #endregion

            #region NUMBER OF COPIES

            var specialTypeAttribute_NUMBEROFCOPIES = new SpecialTypeAttribute_NUMBEROFCOPIES() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_NUMBEROFCOPIES.UUID, specialTypeAttribute_NUMBEROFCOPIES);

            #endregion

            #region PARENT REVISION IDs

            var specialTypeAttribute_PARENTREVISIONIDs = new SpecialTypeAttribute_PARENTREVISIONS() { DBTypeUUID = DBString.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_PARENTREVISIONIDs.UUID, specialTypeAttribute_PARENTREVISIONIDs);

            #endregion

            #region MAX REVISION AGE

            var specialTypeAttribute_MAXREVISIONAGE = new SpecialTypeAttribute_MAXREVISIONAGE() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_MAXREVISIONAGE.UUID, specialTypeAttribute_MAXREVISIONAGE);

            #endregion

            #region MIN NUMBER OF REVISIONS

            var specialTypeAttribute_MINNUMBEROFREVISIONS = new SpecialTypeAttribute_MINNUMBEROFREVISIONS() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_MINNUMBEROFREVISIONS.UUID, specialTypeAttribute_MINNUMBEROFREVISIONS);

            #endregion
            
            #region MAX NUMBER OF REVISIONS

            var specialTypeAttribute_MAXNUMBEROFREVISIONS = new SpecialTypeAttribute_MAXNUMBEROFREVISIONS() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_MAXNUMBEROFREVISIONS.UUID, specialTypeAttribute_MAXNUMBEROFREVISIONS);

            #endregion

            #region MAX NUMBER OF COPIES

            var specialTypeAttribute_MAXNUMBEROFCOPIES = new SpecialTypeAttribute_MAXNUMBEROFCOPIES() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_MAXNUMBEROFCOPIES.UUID, specialTypeAttribute_MAXNUMBEROFCOPIES);

            #endregion

            #region MIN NUMBER OF COPIES

            var specialTypeAttribute_MINNUMBEROFCOPIES = new SpecialTypeAttribute_MINNUMBEROFCOPIES() { DBTypeUUID = DBUInt64.UUID, RelatedGraphDBTypeUUID = typeDBReference.UUID, KindOfType = KindsOfType.SpecialAttribute };
            typeDBReference.AddAttribute(specialTypeAttribute_MINNUMBEROFCOPIES.UUID, specialTypeAttribute_MINNUMBEROFCOPIES);

            #endregion

            _SystemTypes.Add(typeDBReference.UUID, typeDBReference);

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

            foreach (GraphDBType ptype in _SystemTypes.Values)
                _TypesNameLookUpTable.Add(ptype.Name, ptype);

            foreach (GraphDBType ptype in _BasicTypes.Values)
                _TypesNameLookUpTable.Add(ptype.Name, ptype);            

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
                {
                    _ObjectLocations.Add(new ObjectLocation(_String.Split(new String[] { FSPathConstants.PathDelimiter }, StringSplitOptions.RemoveEmptyEntries)));
                }
            }

            return LoadUserDefinedDatabaseTypes(_ObjectLocations, myRebuildIndices);

        }

        #endregion

        #region LoadUserDefinedDatabaseTypes(myObjectLocations)

        /// <summary>
        /// Tries to load all database types from the given ObjectLocations
        /// </summary>
        private Exceptional LoadUserDefinedDatabaseTypes(List<ObjectLocation> myObjectLocations, Boolean myRebuildIndices)
        {

            var ListOfDatabaseTypes = new List<GraphDBType>();

            // Search the StorageLocation for ObjectSchemes and put them to the bulk load list
            foreach (var _ActualObjectLocation in myObjectLocations)
            {

                if (GetTypeByName(DirectoryHelper.GetObjectName(_ActualObjectLocation)) != null)
                    return new Exceptional<Boolean>(new Error_TypeAlreadyExist(DirectoryHelper.GetObjectName(_ActualObjectLocation)));

                //ListOfDatabaseTypes.Add(new PandoraType(_IGraphFS2Session, _ActualObjectLocation, true, this));
                var pt = _IGraphFSSession.GetObject<GraphDBType>(_ActualObjectLocation);

                if (pt.Failed)
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
                if (!result.Success)
                {
                    return new Exceptional(result);
                }
            }

            #endregion

            // Uncomment as soon as index is serializeable
            if (myRebuildIndices)
            {
                return _DBContext.DBIndexManager.RebuildIndices(_UserDefinedTypes);
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

                    if (removeExcept.Failed)
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

        private Exceptional<ResultType> AddAttributeToDBObject(GraphDBType myTypeOfDBObject, ObjectUUID myUUID, AttributeUUID myAttributeUUID, AObject myAttributeValue)
        {
            //myPandoraType is needed due to correctness concerning the attribute name

            #region Input exceptions

            if ((myTypeOfDBObject == null) || (myUUID == null) || (myAttributeUUID == null) || (myAttributeValue == null))
            {
                throw new ArgumentNullException();
            }

            #endregion

            #region Check PandoraType for new Attribute

            TypeAttribute typeAttribute = myTypeOfDBObject.GetTypeAttributeByUUID(myAttributeUUID);

            if (typeAttribute == null)
            {
                //Todo: add notification here (the user has to be informed about the detailed circumstances)

                GraphDBError aError = new Error_AttributeDoesNotExists(myTypeOfDBObject.Name, myAttributeUUID.ToHexString());

                return new Exceptional<ResultType>(aError);
            }

            #endregion

            #region Data

            String objectLocation = myTypeOfDBObject.ObjectLocation + FSPathConstants.PathDelimiter + DBConstants.DBObjectsLocation + FSPathConstants.PathDelimiter + myUUID;
            Exceptional<DBObjectStream> aNewDBObject;
            Exceptional<ResultType> result = new Exceptional<ResultType>();

            #endregion

            #region add attribute

            aNewDBObject = _DBContext.DBObjectManager.LoadDBObject(myTypeOfDBObject, myUUID);

            if (aNewDBObject.Failed)
            {
                result.Push(new Error_LoadObject(myUUID.ToString()));
                return result;
            }

            result = aNewDBObject.Value.AddAttribute(typeAttribute.UUID, myAttributeValue);

            if (result.Failed)
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
        public Exceptional<ResultType> AddAttributeToType(String targetClass, String attributeName, TypeAttribute myTypeAttribute)
        {

            #region INPUT EXCEPTIONS

            if (String.IsNullOrEmpty(targetClass))
            {
                return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty("targetClass"));
            }
            if (String.IsNullOrEmpty(attributeName))
            {
                return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty("attributeName"));
            }
            if (myTypeAttribute == null)
            {
                return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty("myTypeAttribute"));
            }

            #endregion

            #region Data

            GraphDBType tempTargetClassType = GetTypeByName(targetClass);

            #endregion

            #region check if already initialized

            if (GetTypeByUUID(myTypeAttribute.DBTypeUUID) == null)
            {
                return new Exceptional<ResultType>(new Error_TypeDoesNotExist(myTypeAttribute.DBTypeUUID.ToHexString()));
            }

            if (tempTargetClassType == null)
            {
                return new Exceptional<ResultType>(new Error_TypeDoesNotExist(targetClass));
            }

            #endregion

            #region Check if any ParentType already have an attribute with this name

            foreach (var aType in GetAllParentTypes(GetTypeByName(targetClass), true, true))
            {
                if (aType.GetTypeAttributeByName(attributeName) != null)
                {
                    return new Exceptional<ResultType>(new Error_AttributeExistsInSupertype(attributeName, aType.Name));
                }
            }

            #endregion

            if (myTypeAttribute.IsBackwardEdge)
            {
                GetTypeByName(targetClass).AddBackwardEdgeAttribute(myTypeAttribute);
            }

            #region adapt type

            //if we reach this code, no other superclass contains an attribute with this name, so add it!
            myTypeAttribute.RelatedGraphDBTypeUUID = tempTargetClassType.UUID;
            
            if (myTypeAttribute.DefaultValue != null)
            {
                tempTargetClassType.AddMandatoryAttribute(myTypeAttribute.UUID, this);    
            }
            
            tempTargetClassType.AddAttribute(myTypeAttribute.UUID, myTypeAttribute);

            var FlushExcept = FlushType(tempTargetClassType);

            if (FlushExcept.Failed)
                return new Exceptional<ResultType>(FlushExcept);

            #endregion

            #region update lookup tables ob sub-classes

            foreach (var aSubType in GetAllSubtypes(tempTargetClassType).Where(aType => aType != tempTargetClassType))
            {
                aSubType.AddAttributeToLookupTable(myTypeAttribute.UUID, myTypeAttribute);
            }

            #endregion


            return new Exceptional<ResultType>(ResultType.Successful);

        }

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

                if (Result.Failed)
                    return new Exceptional<Boolean>(Result);


                var FlushExcept = FlushType(myType);

                if (FlushExcept.Failed)
                {

                    Result = myType.RenameAttribute(typeAttribute.UUID, oldName);

                    if (Result.Failed)
                        return new Exceptional<Boolean>(Result);

                    return new Exceptional<Boolean>(FlushExcept);
                }

            }
            else
            {
                return new Exceptional<Boolean>(new Error_AttributeDoesNotExists(myType.Name, oldName));
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
                if (typeAttribute.IsBackwardEdge)
                    aUserType.RemoveBackwardEdgeAttribute(typeAttribute);

                var FlushExcept = FlushType(aUserType);

                if (FlushExcept.Failed)
                {
                    aUserType.AddAttribute(typeAttribute);

                    if (typeAttribute.IsBackwardEdge)
                        aUserType.AddBackwardEdgeAttribute(typeAttribute);

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
                return new Exceptional<ResultType>(new Error_AttributeDoesNotExists(aUserType.Name, typeAttribute.Name));
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
                retVal = new Exceptional<ResultType>(new Error_AttributeDoesNotExists(attributeName));
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

            readOut.Attributes.Add(DBConstants.DbGraphType, myGraphDBType);
            
            readOut.Attributes.Add(SpecialTypeAttribute_UUID.AttributeName, myGraphDBType.ObjectUUID);
            readOut.Attributes.Add(SpecialTypeAttribute_REVISION.AttributeName, myGraphDBType.ObjectRevision);
            readOut.Attributes.Add(SpecialTypeAttribute_EDITION.AttributeName, myGraphDBType.ObjectEdition);

            return new Exceptional<DBObjectReadout>(readOut);

        }

        #region AddBulkTypes(TypeList, flushToFs)

        /// <summary>
        /// This method adds a bunch of new PandoraTypeDefinitions (comes from a CREATE TYPE(S) statement) to the TypeManager.
        /// If a certain PType can't be added (because of some inheritance or 
        /// attribute errors), this method tries to add it in a second 
        /// step.
        /// </summary>
        /// <param name="TypeList">List of PandoraType definitions that should 
        /// be added to the TypeManager</param>
        /// <returns>List of PandoraError</returns>
        public Exceptional<QueryResult> AddBulkTypes(List<GraphDBTypeDefinition> TypeList)
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
                    TypeUUID _NewPandoraTypeUUID = new TypeUUID();

                    #region Add type

                    TypeUUID parentUUID = (parentType == null) ? null : parentType.UUID;

                    #region hack

                    GraphDBType _NewPandoraType = new GraphDBType(_NewPandoraTypeUUID, new ObjectLocation(_DatabaseRootPath), aTypeDef.Name, parentUUID, attributes, true, aTypeDef.IsAbstract, aTypeDef.Comment);

                    #endregion

                    addedTypes.Add(_NewPandoraType);

                    _UserDefinedTypes.Add(_NewPandoraType.UUID, _NewPandoraType);
                    _TypesNameLookUpTable.Add(_NewPandoraType.Name, _NewPandoraType);

                    #endregion

                }

                #endregion

                var backwardEdgesToBeAddedAfterwards = new Dictionary<GraphDBType, List<BackwardEdgeNode>>();

                #region Validate the previously added types and add the attributes and backwardEdges

                foreach (var aTypeDef in TypeList)
                {
                    GraphDBType aType = addedTypes.Where(item => item.Name == aTypeDef.Name).FirstOrDefault();

                    #region Check and set parent

                    GraphDBType parentType;

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

                    var parentTypeExcept = HasParentType(parentType.UUID, DBBaseObject.UUID);
                    
                    if(!parentTypeExcept.Success)
                        return new Exceptional<QueryResult>(parentTypeExcept);
                    
                    if (!parentTypeExcept.Value)
                    {
                        RemoveRecentlyAddedTypes(addedTypes);
                        return new Exceptional<QueryResult>(new Error_InvalidDerive("The type " + aType.Name + " can not be added, because all user defined types must be subtypes of PandoraObject."));
                    }

                    #endregion

                    #region check and set type of attributes

                    foreach (KeyValuePair<TypeAttribute, String> attribute in aTypeDef.Attributes)
                    {
                        if (attribute.Key.Name == aType.Name)
                        {
                            RemoveRecentlyAddedTypes(addedTypes);
                            return new Exceptional<QueryResult>(new Error_InvalidAttributeName("The attribute " + attribute.Key.Name + " can not be added, because it has the same name as its related type."));
                        }

                        GraphDBType attrType = GetTypeByName(attribute.Value);
                        if (attrType == null)
                        {
                            attrType = addedTypes.Where(item => item.Name == attribute.Value).FirstOrDefault();
                            if (attrType == null)
                            {
                                RemoveRecentlyAddedTypes(addedTypes);
                                return new Exceptional<QueryResult>(new Error_TypeDoesNotExist(attribute.Value));
                            }
                        }

                        TypeAttribute newAttr = attribute.Key;

                        newAttr.DBTypeUUID = attrType.UUID;
                        newAttr.RelatedGraphDBTypeUUID = aType.UUID;

                        #region we had not defined a special EdgeType - for single reference attributes we need to set the EdgeTypeSingle NOW!

                        if (newAttr.KindOfType == KindsOfType.SingleReference && attrType.IsUserDefined && newAttr.EdgeType == null)
                            newAttr.EdgeType = new EdgeTypeSingleReference();

                        #endregion

                        #region Validate EdgeType in terms of List & Single

                        if (newAttr.KindOfType == KindsOfType.SingleReference && attrType.IsUserDefined && !(newAttr.EdgeType is ASingleReferenceEdgeType))
                        {
                            RemoveRecentlyAddedTypes(addedTypes);
                            return new Exceptional<QueryResult>(new Error_InvalidEdgeType(newAttr.EdgeType.GetType(), typeof(ASingleReferenceEdgeType)));
                        }
                        else if (newAttr.KindOfType == KindsOfType.SetOfReferences || newAttr.KindOfType == KindsOfType.SetOfNoneReferences)
                        {
                            if (attrType.IsUserDefined && !(newAttr.EdgeType is ASetReferenceEdgeType))
                            {
                                RemoveRecentlyAddedTypes(addedTypes);
                                return new Exceptional<QueryResult>(new Error_InvalidEdgeType(newAttr.EdgeType.GetType(), typeof(ASetReferenceEdgeType)));
                            }
                            else if (!attrType.IsUserDefined && !(newAttr.EdgeType is AListBaseEdgeType))
                            {
                                RemoveRecentlyAddedTypes(addedTypes);
                                return new Exceptional<QueryResult>(new Error_InvalidEdgeType(newAttr.EdgeType.GetType(), typeof(AListBaseEdgeType)));
                            }
                        }

                        #endregion

                        aType.AddAttribute(newAttr.UUID, newAttr);
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
                    foreach (var be in beDefinition.Value)
                    {

                        var bedgeAttribute = CreateBackwardEdgeAttribute(be, aType);

                        if (!bedgeAttribute.Success)
                        {
                            RemoveRecentlyAddedTypes(addedTypes);
                            return new Exceptional<QueryResult>(bedgeAttribute);
                        }

                        aType.AddAttribute(bedgeAttribute.Value);
                        aType.AddBackwardEdgeAttribute(bedgeAttribute.Value);
                    }
                }

                #endregion

                #region Validate Attribute dependencies
                                
                List<DBObjectReadout> readOutList   = new List<DBObjectReadout>();
                SelectionResultSet    selResult     = null;
                
                foreach (GraphDBType aType in addedTypes)
                {
                    foreach (GraphDBType _PandoraType in GetAllParentTypes(aType, false, true))
                    {
                        var _MandatoryAttr = _PandoraType.GetMandatoryAttributesUUIDs(this);
                        List<AttributeUUID> _UniqueAttr = _PandoraType.GetAllUniqueAttributes(false, this);

                        foreach (TypeAttribute ta in aType.Attributes.Values)
                        {
                            if (_PandoraType.GetTypeAttributeByName(ta.Name) != null)
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
                    var AddUniqueAttrExcept = aType.AddUniqueAttributes(uniqueAttrIDs, this);

                    if(AddUniqueAttrExcept.Failed)
                        return new Exceptional<QueryResult>(AddUniqueAttrExcept);

                    uniqueAttrIDs.Clear();
                }
                
                #endregion

                #region add attribute lookup table of parent type to the actual one

                addedTypes.ForEach(item => item.AddAttributeToLookupTable(GetTypeByUUID(item.ParentTypeUUID).AttributeLookupTable));

                #endregion

                #region Create indices

                foreach (GraphDBType aType in addedTypes)
                {
                    #region Create userdefined Indices

                    var aTypeDef = TypeList.Where(item => item.Name == aType.Name).FirstOrDefault();

                    if (!aTypeDef.Indices.IsNullOrEmpty())
                    {
                        foreach (var indexExceptional in aTypeDef.Indices)
                        {

                            if (indexExceptional.Failed)
                            {
                                return new Exceptional<QueryResult>(indexExceptional);
                            }
                            if (!indexExceptional.Success)
                            {
                                result.AddWarnings(indexExceptional.Warnings);
                            }
                            var index = indexExceptional.Value;

                            if (!index.IndexAttributeNames.All(node => aType.GetTypeAttributeByName(node.IndexAttribute) != null))
                            {
                                RemoveRecentlyAddedTypes(addedTypes);
                                return new Exceptional<QueryResult>(new Error_AttributeDoesNotExists(aType.Name, aType.Name));
                            }

                            List<AttributeUUID> indexAttrs = new List<AttributeUUID>(index.IndexAttributeNames.Select(node => aType.GetTypeAttributeByName(node.IndexAttribute).UUID));

                            foreach (var item in GetAllSubtypes(aType))
                            {
                                var CreateIdxExcept = item.CreateAttributeIndex(index.IndexName, indexAttrs, index.Edition, index.IndexType);

                                if (!CreateIdxExcept.Success)
                                    return new Exceptional<QueryResult>(CreateIdxExcept);
                            }
                        }
                    }

                    #endregion

                    // GUID index                    
                    var createIndexExcept = aType.CreateUniqueAttributeIndex(SpecialTypeAttribute_UUID.AttributeName, GetGUIDTypeAttribute().UUID, DBConstants.DEFAULTINDEX);

                    if (!createIndexExcept.Success)
                        return new Exceptional<QueryResult>(createIndexExcept);

                    List<AttributeUUID> UniqueIDs = aType.GetAllUniqueAttributes(true, this);
                    
                    if (UniqueIDs.Count() > 0)
                    {
                        var idxName = _DBContext.DBIndexManager.GetUniqueIndexName(UniqueIDs, aType); // UniqueIDs.Aggregate<AttributeUUID, String>("Idx", (result, item) => result = result + "_" + aType.GetTypeAttributeByUUID(item).Name);
                        var createIdxExcept = aType.CreateUniqueAttributeIndex(idxName, UniqueIDs, DBConstants.UNIQUEATTRIBUTESINDEX);

                        if (!createIdxExcept.Success)
                            return new Exceptional<QueryResult>(createIdxExcept);
                    }
                }

                #endregion


                #region flush to fs

                
                foreach (var item in addedTypes)
                {
                    var createException = CreateTypeOnFS(item);

                    if (!createException.Success)
                        return new Exceptional<QueryResult>(createException);

                    #region get system attributes from type

                    var readOut = GetTypeReadouts(item);

                    if (!readOut.Success)
                        return new Exceptional<QueryResult>(readOut.Errors.First());

                    readOutList.Add(readOut.Value);

                    #endregion
                }

                selResult = new SelectionResultSet(readOutList);
                result.AddResult(selResult);
                

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

        public Exceptional<TypeAttribute> CreateBackwardEdgeAttribute(BackwardEdgeNode myBackwardEdgeNode, GraphDBType myDBTypeStream)
        {

            var edgeType      = GetTypeByName(myBackwardEdgeNode.TypeName);
            
            if (edgeType == null)
                return new Exceptional<TypeAttribute>(new Error_TypeDoesNotExist(myBackwardEdgeNode.TypeName));

            var edgeAttribute = edgeType.GetTypeAttributeByName(myBackwardEdgeNode.TypeAttributeName);

            if (edgeAttribute == null)
                return new Exceptional<TypeAttribute>(new Error_AttributeDoesNotExists(edgeType.Name, myBackwardEdgeNode.TypeAttributeName));

            if (!edgeAttribute.GetDBType(this).IsUserDefined)
                return new Exceptional<TypeAttribute>(new Error_BackwardEdgesForNotReferenceAttributeTypesAreNotAllowed(myBackwardEdgeNode.TypeAttributeName));

            if (edgeAttribute.GetDBType(this) != myDBTypeStream)
                return new Exceptional<TypeAttribute>(new Error_BackwardEdgeDestinationIsInvalid(myDBTypeStream.Name, myBackwardEdgeNode.TypeAttributeName));

            var edgeKey = new EdgeKey(edgeType.UUID, edgeAttribute.UUID);

            if (myDBTypeStream.IsBackwardEdgeAttribute(edgeKey))
                return new Exceptional<TypeAttribute>(new Error_BackwardEdgeAlreadyExist(myDBTypeStream, edgeType.Name, edgeAttribute.Name));

            var ta = new TypeAttribute();
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

        private Exceptional<GraphDBType> AddingType(String myTypeName, String myParentType, Dictionary<TypeAttribute, String> myAttributes, Boolean myIsAbstract, String myComment)
        {
            var ptd = new GraphDBTypeDefinition(myTypeName, myParentType, false, myAttributes, null, null, myComment);

            var errors = AddBulkTypes(new List<GraphDBTypeDefinition>(new[] { ptd }));

            if (errors.Failed)
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
        public Exceptional<GraphDBType> AddType(String myTypeName, String myParentType, Dictionary<TypeAttribute, String> myAttributes, Boolean myIsAbstract, String myComment)
        {
            return AddingType(myTypeName, myParentType, myAttributes, myIsAbstract, myComment);
        }
        
        /// <summary>
        /// Adds the given type to the list of types.
        /// </summary>
        /// <param name="Type">The type to be added.</param>
        public Exceptional<GraphDBType> AddType(String myTypeName, String myParentType, Dictionary<TypeAttribute, String> myAttributes, String myComment)
        {
            return AddingType(myTypeName, myParentType, myAttributes, false, myComment);
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


        #region CreateTypeOnFS(myPandoraType)

        private Exceptional<Boolean> CreateTypeOnFS(GraphDBType myPandoraType)
        {
            using (var _Transaction = _IGraphFSSession.BeginTransaction())
            {
                var CreateException = CreateTypeOnFS_internal(myPandoraType);

                if (CreateException.Failed)
                    return new Exceptional<bool>(CreateException);
                
                _Transaction.Commit();
            }

            return new Exceptional<bool>(true);
        }

        #endregion

        #region CreateTypeOnFS_internal(myPandoraType)

        private Exceptional<Boolean> CreateTypeOnFS_internal(GraphDBType myPandoraType)
        {

            #region Data

            var typeName = myPandoraType.Name;
            var typeDir  = myPandoraType.ObjectLocation;

            #endregion
            
            #region Change the database instance settings containing the StorageLocation paths (Object Schemes)

            lock (_ObjectLocationsOfAllUserDefinedDatabaseTypes)
            {
                _ObjectLocationsOfAllUserDefinedDatabaseTypes.Add(typeDir);
                _ObjectLocationsOfAllUserDefinedDatabaseTypes.Save();
            }

            #endregion

            #region Store the new type on the file system

            var isDirExcept = _IGraphFSSession.isIDirectoryObject(typeDir);

            if(isDirExcept.Failed)
                return new Exceptional<bool>(isDirExcept);
            
            if (isDirExcept.Value == Trinary.TRUE)
                return new Exceptional<Boolean>(new Error_UnknownDBError("Default directory for the new type " + typeName + " could not be created, cause it already exists."));

            #region Get blocksize of the TypeDirectory from "TYPEDIRBLOCKSIZE" setting

            UInt64 TypeDirBlockSize = 0;
            var blocksizeSetting = new SettingTypeDirBlocksize();
            var _TypeDireBlockSizeExceptional = blocksizeSetting.Get(_DBContext, TypesSettingScope.DB);

            if (!_TypeDireBlockSizeExceptional.Success)
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

            if (CreateDirExcept.Failed)
                return new Exceptional<bool>(CreateDirExcept);

            // Create a subdirectory for the objects of this new type
            CreateDirExcept = _IGraphFSSession.CreateDirectoryObject(new ObjectLocation(typeDir, DBConstants.DBObjectsLocation));
            
            if (CreateDirExcept.Failed)
                return new Exceptional<bool>(CreateDirExcept);

            // Create a subdirectory for the indices of this new type
            CreateDirExcept = _IGraphFSSession.CreateDirectoryObject(new ObjectLocation(typeDir, DBConstants.DBIndicesLocation));

            if (CreateDirExcept.Failed)
                return new Exceptional<bool>(CreateDirExcept);

            #endregion

            var FlushExcept = FlushType(myPandoraType);

            if (FlushExcept.Failed)
                return new Exceptional<bool>(FlushExcept);

            return new Exceptional<bool>(true);
        }

        #endregion    


        #region FlushType(PandoraType myPandoraType)

        public Exceptional FlushType(GraphDBType myPandoraType)
        {
            
            var _Exceptional = _IGraphFSSession.StoreFSObject(myPandoraType, true);

            if (_Exceptional == null || _Exceptional.Failed)
                return new Exceptional(_Exceptional);

            return new Exceptional();

        }

        #endregion


        #region RemoveType(myType)

        /// <summary>
        /// Removes the given type (and its list types) and all subtypes. The generic list types cant 
        /// get removed, cause they get created/deleted autopmatically whenevery a content type 
        /// was added/removed by the user. Furthermore all myAttributes of the deleted types of other 
        /// PandoraTypes will be also deleted.
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

                if (removeTypeExcept.Failed)
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
                if (!_ObjectLocationsOfAllUserDefinedDatabaseTypes.Contains(myType.ObjectLocation))
                {
                    var existExcept = _IGraphFSSession.ObjectExists(myType.ObjectLocation);

                    if (existExcept.Failed)
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

                if (toBeDeletedTypes.ParentTypeUUID != null)
                {
                    _DBContext.DBIndexManager.RemoveGuidIndexEntriesOfParentTypes(toBeDeletedTypes, _DBContext.DBIndexManager);
                }

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

                    var DbObjectSchemeSettingsDest = _DatabaseRootPath + FSPathConstants.PathDelimiter + DBConstants.DBTypeLocations;
                    _ObjectLocationsOfAllUserDefinedDatabaseTypes.Remove(toBeDeletedTypes.ObjectLocation);
                    _ObjectLocationsOfAllUserDefinedDatabaseTypes.Save();

                }
                catch (Exception e)
                {
                    return new Exceptional<bool>(new Error_UnknownDBError(e));
                }

                #endregion

                #region remove type from fs

                var removeTypeExcept = RemoveTypeFromFs(toBeDeletedTypes.ObjectLocation);

                if (removeTypeExcept.Failed)
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
        /// Removes PandoraTypes from filesystem
        /// </summary>
        /// <param name="typeDir">The directory of the PandoraType that is going to be deleted.</param>
        /// <returns>True for success or otherwise false.</returns>
        private Exceptional<Boolean> RemoveTypeFromFs(ObjectLocation typeDir)
        {

            using (var _Transaction = _IGraphFSSession.BeginTransaction())
            {

                var removeObjectExcept = _IGraphFSSession.RemoveObject(typeDir, DBConstants.DBTYPESTREAM, null, null);

                if (removeObjectExcept.Failed)
                {
                    return new Exceptional<bool>(removeObjectExcept); // return and rollback transaction
                }

                var removeDirExcept = _IGraphFSSession.RemoveDirectoryObject(typeDir, true);

                if (removeDirExcept.Failed)
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

            if (retVal.Failed)
                return new Exceptional<Boolean>(retVal);

            _TypesNameLookUpTable.Remove(oldName);
            _TypesNameLookUpTable.Add(newName, myType);
            _ObjectLocationsOfAllUserDefinedDatabaseTypes.Remove(oldLocation);
            _ObjectLocationsOfAllUserDefinedDatabaseTypes.Add(myType.ObjectLocation.ToString());
            
            var saveResult = _ObjectLocationsOfAllUserDefinedDatabaseTypes.Save();
            if (!saveResult.Success)
            {
                return new Exceptional<bool>(saveResult);
            }

            return new Exceptional<Boolean>(true);
        }
        #endregion

        public Exceptional ChangeCommentOnType(GraphDBType atype, string comment)
        {
            if (atype == null)
                return new Exceptional<Boolean>(new Error_ArgumentNullOrEmpty("atype"));

            if (String.IsNullOrEmpty(comment))
                return new Exceptional<Boolean>(new Error_ArgumentNullOrEmpty("newName"));

            atype.SetComment(comment);

            var flushExcept = FlushType(atype);

            if (!flushExcept.Success)
            {
                return new Exceptional<Boolean>(true);
            }
            else
            {
                return new Exceptional<Boolean>(flushExcept);
            }
        }

        #endregion


        #region Some Gettings

        #region Get type methods

        #region GetTypeByName(TypeName)

        /// <summary>
        /// Returns the PandoraType which has the name Name.
        /// </summary>
        /// <param name="Name">Name of the Type.</param>
        /// <returns>The PandoraType, if it exists. Else, null.</returns>
        public GraphDBType GetTypeByName(String myTypeName)
        {

            #region Input Validation

            if (myTypeName == null)
            {
                throw new ArgumentNullException("The parameter myTypeName must not be null!");
            }

            #endregion

            GraphDBType _PandoraType = null;

            _TypesNameLookUpTable.TryGetValue(myTypeName, out _PandoraType);

            return _PandoraType;

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
        /// This method returns all PandoraTypes that are currently loaded by the TypeManager.
        /// BasicPandoraTypes are also included.
        /// </summary>
        /// <returns>A list of Strings that with the names of the PandoraTypes that are currently loaded by the TypeManager</returns>
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
        /// Resturns all PandoraTypes, that are PandoraObjects and that are subtypes of the Type, which name is given.
        /// These are all child types - all types which derives from the <paramref name="myGraphDBType"/>
        /// </summary>
        /// <param name="Name">The name of the supertype.</param>
        /// <returns>The list of all subtypes.</returns>
        public List<GraphDBType> GetAllSubtypes(GraphDBType myGraphDBType, Boolean myThisTypeIncluding = true)
        {

            #region INPUT EXCEPTIONS

            if (myGraphDBType == null) return null;
            if (!_UserDefinedTypes.ContainsKey(myGraphDBType.UUID)) return null;

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

            #region special handling for PandoraObject, cause it is not in the userdefinedtypes dict

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

        public TypeAttribute GetGUIDTypeAttribute()
        {
            return _GUIDTypeAttribute;
        }

        #endregion

        #endregion

    }
}
