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


/* <id name="GraphDB – ObjectManager" />
 * <copyright file="ObjectManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class manages the Object handling between DB and FS.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

using sones.GraphFS;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Warnings;

using sones.Lib.Session;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.Indices;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib;
using sones.GraphDB.Transactions;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.Enums;

#endregion

namespace sones.GraphDB.ObjectManagement
{

    public class DBObjectManager
    {

        #region Data

        private DBContext     _DBContext;
        private IGraphFSSession _IGraphFSSession;

        #endregion

        #region Constructors

        public DBObjectManager(DBContext dbContext, IGraphFSSession myIGraphFSSession)
        {
            _DBContext     = dbContext;
            _IGraphFSSession = myIGraphFSSession;
        }
        
        #endregion

        #region DBO 

        #region CreateNewDBObject(myGraphType, myDBObjectAttributes)

        /// <summary>
        /// Creates a new DBObject of a given GraphType inserts its UUID into all indices of the given GraphType.
        /// </summary>
        /// <param name="myGraphType">The GraphType of the DBObject to create</param>
        /// <param name="myDBObjectAttributes">A dictionary of attributes for the new DBObject</param>
        /// <param name="myCheckUniqueness">check for unique constraints</param>
        /// <returns>The UUID of the new DBObject</returns>
        public Exceptional<DBObjectStream> CreateNewDBObject(GraphDBType myGraphType, Dictionary<String, IObject> myDBObjectAttributes, Dictionary<String, IObject> myUndefAttributes, SessionSettings myToken, Boolean myCheckUniqueness)
        {
            Dictionary<AttributeUUID, IObject> _attrs = myDBObjectAttributes.ToDictionary(key => myGraphType.GetTypeAttributeByName(key.Key).UUID, value => value.Value);
            return CreateNewDBObject(myGraphType, _attrs, myUndefAttributes, null, myToken, myCheckUniqueness);
        }

        #endregion

        #region CreateNewDBObject(myGraphType, myDBObjectAttributes, myExtractSettings)

        /// <summary>
        /// Creates a new DBObject of a given GraphType inserts its UUID into all indices of the given GraphType.
        /// </summary>
        /// <param name="myGraphDBType">The GraphType of the DBObject to create</param>
        /// <param name="myDBObjectAttributes">A dictionary of attributes for the new DBObject</param>
        /// <param name="myExtractSettings">Special values which should be set to a object</param>
        /// <returns>The UUID of the new DBObject</returns>
        public Exceptional<DBObjectStream> CreateNewDBObject(GraphDBType myGraphDBType, Dictionary<String, IObject> myDBObjectAttributes, Dictionary<String, IObject> myUndefAttributes, Dictionary<ASpecialTypeAttribute, Object> mySpecialTypeAttributes, SessionSettings mySessionSettings, Boolean myCheckUniqueness)
        {
            Dictionary<AttributeUUID, IObject> _attrs = myDBObjectAttributes.ToDictionary(key => myGraphDBType.GetTypeAttributeByName(key.Key).UUID, value => value.Value);
            return CreateNewDBObject(myGraphDBType, _attrs, myUndefAttributes, mySpecialTypeAttributes, mySessionSettings, myCheckUniqueness);
        }

        #endregion

        #region CreateNewDBObject(myGraphType, myDBObjectAttributes)
        
        /// <summary>
        /// Creates a new DBObject of a given GraphType inserts its UUID into all indices of the given GraphType.
        /// </summary>
        /// <param name="myGraphDBType">The GraphType of the DBObject to create</param>
        /// <param name="myDBObjectAttributes">A dictionary of attributes for the new DBObject</param>
        /// <param name="mySpecialTypeAttributes">Special values which should be set to a object</param>
        /// <param name="myCheckUniqueness">check for unique constraints</param> 
        /// <returns>The UUID of the new DBObject</returns>
        public Exceptional<DBObjectStream> CreateNewDBObject(GraphDBType myGraphDBType, Dictionary<AttributeUUID, IObject> myDBObjectAttributes, Dictionary<String, IObject> myUndefAttributes, Dictionary<ASpecialTypeAttribute, Object> mySpecialTypeAttributes, SessionSettings mySessionSettings, Boolean myCheckUniqueness)
        {

            #region Input validation

            if (myGraphDBType == null)
                return new Exceptional<DBObjectStream>(new Error_ArgumentNullOrEmpty("The parameter myGraphType must not be null!"));

            if (myDBObjectAttributes == null)
                return new Exceptional<DBObjectStream>(new Error_ArgumentNullOrEmpty("The parameter myDBObjectAttributes must not be null!"));

            #endregion

            if (myCheckUniqueness)
            {

                var parentTypes = _DBContext.DBTypeManager.GetAllParentTypes(myGraphDBType, true, false);
                var CheckVal = _DBContext.DBIndexManager.CheckUniqueConstraint(myGraphDBType, parentTypes, myDBObjectAttributes);

                if (CheckVal.Failed())
                    return new Exceptional<DBObjectStream>(CheckVal);
            }

            #region Create a new DBObject

            DBObjectStream NewDBObject = null;
            //String settingEncoding = null;
            Boolean isUserdefinedUUID = false;
                      

            NewDBObject = new DBObjectStream(myGraphDBType, myDBObjectAttributes);

            #region Check for ExtractSetting attributes like UUID

            if (!mySpecialTypeAttributes.IsNullOrEmpty())
            {
                foreach (var specialAttr in mySpecialTypeAttributes)
                {
                    if (specialAttr.Key is SpecialTypeAttribute_UUID)
                    {
                        var result = specialAttr.Key.ApplyTo(NewDBObject, specialAttr.Value);
                        if (result.Failed())
                        {
                            return new Exceptional<DBObjectStream>(result);
                        }

                        isUserdefinedUUID = true;

                    }
                    else
                    {
                        var result = specialAttr.Key.ApplyTo(NewDBObject, specialAttr.Value);
                        if (result.Failed())
                        {
                            return new Exceptional<DBObjectStream>(result);
                        }
                    }
                }
             }
        
            #endregion

            #endregion

            #region checking for existence

            var objectExistsResult = ObjectExistsOnFS(NewDBObject);

            if (objectExistsResult.Failed())
                return new Exceptional<DBObjectStream>(objectExistsResult);
            
            while (objectExistsResult.Value == Trinary.TRUE)
            {
                
                if (isUserdefinedUUID)
                    return new Exceptional<DBObjectStream>(new Error_DBObjectCollision(NewDBObject));

                //NLOG: temporarily commented
                ////_Logger.Warn("ObjectUUID collision in CreateNewDBObject.");
                NewDBObject = new DBObjectStream(myGraphDBType, myDBObjectAttributes);

                objectExistsResult = ObjectExistsOnFS(NewDBObject);

                if (objectExistsResult.Failed())
                    return new Exceptional<DBObjectStream>(objectExistsResult);

            }

            //NLOG: temporarily commented
            ////_Logger.Trace("CreateNewDBObject created " + NewDBObject.ObjectUUID.ToString());

            #endregion

            
            // Flush new DBObject
            var flushResult = FlushDBObject(NewDBObject);
            //NLOG: temporarily commented
            ////_Logger.Trace("CreateNewDBObject flushed " + NewDBObject.ObjectUUID.ToString());

            if (flushResult.Failed())
                return new Exceptional<DBObjectStream>(flushResult);

            #region Check for existing object - might be removed at some time

            var exists = ObjectExistsOnFS(NewDBObject);

            if (exists.Failed())
                return new Exceptional<DBObjectStream>(exists);

            if (exists.Value != Trinary.TRUE)
            {
                return new Exceptional<DBObjectStream>(new Error_UnknownDBError("DBObject with path " + NewDBObject.ObjectLocation + " does not exist."));
            }

            #endregion
            
            #region Add UUID of the new DBObject to all indices of myGraphType

            foreach (var type in _DBContext.DBTypeManager.GetAllParentTypes(myGraphDBType, true, false))
            {
                foreach (var aIdx in type.GetAllAttributeIndices(false))
                {
                    //Find out if the dbobject carries all necessary attributes
                    if (NewDBObject.HasAtLeastOneAttribute(aIdx.IndexKeyDefinition.IndexKeyAttributeUUIDs, type, mySessionSettings))
                    {
                        //valid dbo for idx
                        aIdx.Insert(NewDBObject, type, _DBContext);
                    }
                }
            }

            #endregion

            #region add undefined attributes to the object

            if (!myUndefAttributes.IsNullOrEmpty())
            {
                foreach (var item in myUndefAttributes)
                {
                    var addExcept = NewDBObject.AddUndefinedAttribute(item.Key, item.Value, this);

                    if (addExcept.Failed())
                    {
                        return new Exceptional<DBObjectStream>(addExcept);
                    }
                }
            }

            #endregion

            return new Exceptional<DBObjectStream>(NewDBObject);

        }

        private Exceptional<Trinary> ObjectExistsOnFS(DBObjectStream NewDBObject)
        {
            var objectExistException = _IGraphFSSession.ObjectStreamExists(NewDBObject.ObjectLocation, DBConstants.DBOBJECTSTREAM);

            if (objectExistException.Failed())
                return new Exceptional<Trinary>(objectExistException);

            return new Exceptional<Trinary>(objectExistException.Value);
        }

        #endregion

        #region LoadDBObject(myTypeLocation, myUUID, myNameOfType)

        /// <summary>
        /// Loads a DBObject from filesystem.
        /// </summary>
        /// <param name="myGraphType">The GraphType of the DBObject (for path resolution).</param>
        /// <param name="myUUID">The unique identifier of the DBObject</param>
        /// <returns>The requested DBObject or null.</returns>
        public Exceptional<DBObjectStream> LoadDBObject(GraphDBType myGraphDBType, ObjectUUID myUUID)
        {
            if (myUUID == null)
                return new Exceptional<DBObjectStream>(new Error_ArgumentNullOrEmpty("ObjectUUID should not be null."));

            if(myGraphDBType == null)
                return new Exceptional<DBObjectStream>(new Error_ArgumentNullOrEmpty("DBType should not be null."));

            return LoadDBObject(myGraphDBType, myUUID, myGraphDBType.Name);

        }

        public Exceptional<DBObjectStream> LoadDBObject(GraphDBType myGraphDBType, ObjectUUID myUUID, String myNameOfType)
        {
            return LoadDBObject(new ObjectLocation(myGraphDBType.ObjectLocation, DBConstants.DBObjectsLocation, myUUID.ToString())); ;
        }

        public Exceptional<DBObjectStream> LoadDBObject(ObjectLocation myObjectLocation)
        {

            var _DBExceptional = new Exceptional<DBObjectStream>();

            // This will lock 
            
            var _GetObjectExceptional = _IGraphFSSession.GetFSObject<DBObjectStream>(myObjectLocation, DBConstants.DBOBJECTSTREAM, null, null, 0, false);

            if (_GetObjectExceptional == null || _GetObjectExceptional.Failed() || _GetObjectExceptional.Value == null)
            {
                return _GetObjectExceptional.Convert<DBObjectStream>().PushT(new Error_LoadObject(myObjectLocation));
            }

            _DBExceptional.Value = _GetObjectExceptional.Value;
            return _DBExceptional;

        }



        #endregion

        #region FlushDBObject(myDBObject)

        /// <summary>
        /// Flushes a DBObject to FS.
        /// </summary>
        /// <param name="myDBObject">The DBObject to be flushed.</param>
        /// <returns>True for success or otherwise false.</returns>
        public Exceptional FlushDBObject(DBObjectStream myDBObject)
        {

            #region Input validation

            if (myDBObject == null)
                return new Exceptional(new Error_ArgumentNullOrEmpty("myDBObject"));

            #endregion

            if (myDBObject.IGraphFSSessionReference == null)
                return _IGraphFSSession.StoreFSObject(myDBObject, true);

            return myDBObject.Save();

        }

        #endregion

        #region RemoveDBObject(myTypeOfDBObject, myUUID)

        /// <summary>
        /// Removes a DBObject.
        /// </summary>
        /// <param name="myGraphType">The Type of the DBObject that is to be removed.</param>
        /// <param name="myDBObject">The UUID of the DBObject.</param>
        public Exceptional RemoveDBObject(GraphDBType myTypeOfDBObject, DBObjectStream myDBObject, DBObjectCache myDBObjectCache, SessionSettings myToken)
        {

            #region Input exceptions

            if (myTypeOfDBObject == null)
            {
                return new Exceptional(new Error_ArgumentNullOrEmpty("myTypeOfDBObject"));
            }
            if (myDBObject == null)
            {
                return new Exceptional(new Error_ArgumentNullOrEmpty("myUUID"));
            }


            #endregion

            #region Data

            ObjectLocation myDBObjectLocation;

            #endregion

            // Get DBObject path
            myDBObjectLocation = myDBObject.ObjectLocation;

            #region remove from attributeIDX

            foreach (var anIndex in myTypeOfDBObject.GetAllAttributeIndices(false))
            {
                anIndex.Remove(myDBObject, myTypeOfDBObject, _DBContext);
            }

            #endregion

            #region remove from fs

            #region Remove DBOBJECTSTREAM

            var _RemoveObjectExceptional = _IGraphFSSession.RemoveObjectIfExists(myDBObjectLocation, DBConstants.DBOBJECTSTREAM);
            if (_RemoveObjectExceptional.Failed())
            {
                return _RemoveObjectExceptional;
            }

            #endregion

            #region Remove DBBACKWARDEDGESTREAM

            _RemoveObjectExceptional = _IGraphFSSession.RemoveObjectIfExists(myDBObjectLocation, DBConstants.DBBACKWARDEDGESTREAM);
            if (_RemoveObjectExceptional.Failed())
            {
                return _RemoveObjectExceptional;
            }

            #endregion

            #region Remove UNDEFATTRIBUTESSTREAM

            _RemoveObjectExceptional = _IGraphFSSession.RemoveObjectIfExists(myDBObjectLocation, DBConstants.UNDEFATTRIBUTESSTREAM);
            if (_RemoveObjectExceptional.Failed())
            {
                return _RemoveObjectExceptional;
            }

            #endregion

            #endregion

            return Exceptional.OK;
        }

        #endregion

        #endregion

        #region dbObjectCache

        /// <summary>
        /// Wrapper method for query cache generation
        /// </summary>
        /// <returns></returns>
        public DBObjectCache GetSimpleDBObjectCache(DBContext dbContext)
        {

            var _SettingMaxCacheItems = 100000L;

            using (var _Exceptional = (new SettingMaxCacheItems()).Get(dbContext, TypesSettingScope.DB))
            {
                if (_Exceptional.Success() && _Exceptional.Value != null)
                    _SettingMaxCacheItems = (long)_Exceptional.Value.Value.Value;
            }

            return new DBObjectCache(dbContext.DBTypeManager, dbContext.DBObjectManager, _SettingMaxCacheItems);

        }

        #endregion

        #region undefined attributes

        public Exceptional<UndefinedAttributesStream> LoadUndefinedAttributes(ObjectLocation myAttributeLocation)
        {
            #region data

            UndefinedAttributesStream retAttributes = null;

            #endregion

            var existExcept = _IGraphFSSession.ObjectStreamExists(myAttributeLocation, DBConstants.UNDEFATTRIBUTESSTREAM);

            if (existExcept.Failed())
                return new Exceptional<UndefinedAttributesStream>(existExcept);

            if (existExcept.Value == Trinary.TRUE)
            {
                var loadException = _IGraphFSSession.GetFSObject<UndefinedAttributesStream>(myAttributeLocation, DBConstants.UNDEFATTRIBUTESSTREAM, null, null, 0, false);

                if (loadException.Failed())
                    return new Exceptional<UndefinedAttributesStream>(loadException);

                retAttributes = loadException.Value;

            }
            else
                retAttributes = new UndefinedAttributesStream(myAttributeLocation);


            return new Exceptional<UndefinedAttributesStream>(retAttributes);
        }

        public Exceptional<Boolean> StoreUndefinedAttributes(UndefinedAttributesStream UndefAttributes)
        {
            if (UndefAttributes != null && UndefAttributes.isDirty)
            {
                var storeExcept = _IGraphFSSession.StoreFSObject(UndefAttributes, true);

                if (storeExcept == null || storeExcept.Failed())
                    return new Exceptional<Boolean>(storeExcept);
            }

            return new Exceptional<bool>(true);
        }    

        public Exceptional<Boolean> AddUndefinedAttribute(String myName, IObject myValue, DBObjectStream myObject)
        {
            return myObject.AddUndefinedAttribute(myName, myValue, this);
        }

        public Exceptional<Boolean> RemoveUndefinedAttribute(String myName, DBObjectStream myObject)
        {
            return myObject.RemoveUndefinedAttribute(myName, this);
        }

        #endregion

        #region BackwardEdge

        public Exceptional<BackwardEdgeStream> LoadBackwardEdge(ObjectLocation myEdgeLocation)
        {
            #region data

            BackwardEdgeStream retDBBackwardEdge = null;

            #endregion

            var existExcept = _IGraphFSSession.ObjectStreamExists(myEdgeLocation, DBConstants.DBBACKWARDEDGESTREAM);

            if (existExcept.Failed())
                return new Exceptional<BackwardEdgeStream>(existExcept);

            if (existExcept.Value == Trinary.TRUE)
            {
                var loadException = _IGraphFSSession.GetFSObject<BackwardEdgeStream>(myEdgeLocation, DBConstants.DBBACKWARDEDGESTREAM, null, null, 0, false);

                if (loadException.Failed())
                    return new Exceptional<BackwardEdgeStream>(loadException);

                retDBBackwardEdge = loadException.Value;
            }
            else
            {
                retDBBackwardEdge = new BackwardEdgeStream(myEdgeLocation);
            }

            return new Exceptional<BackwardEdgeStream>(retDBBackwardEdge);
        }

        public Exceptional<Boolean> AddBackwardEdge(DBObjectStream aDBObject, TypeUUID uUIDofType, AttributeUUID uUIDofAttribute, ObjectUUID reference)
        {
            return aDBObject.AddBackwardEdge(uUIDofType, uUIDofAttribute, reference, _DBContext.DBObjectManager);
        }

        internal Exceptional AddBackwardEdge(ObjectUUID uuid, TypeUUID typeOfBESource, EdgeKey beEdgeKey, ObjectUUID reference)
        {

            //var beLocation = DBObjectStream.GetObjectLocation(_DBContext.DBTypeManager.GetTypeByUUID(typeOfBESource), uuid);
            var loadExcept = _DBContext.DBObjectCache.LoadDBBackwardEdgeStream(typeOfBESource, uuid); // LoadBackwardEdge(beLocation);

            if (loadExcept.Failed())
            {
                return new Exceptional(loadExcept);
            }

            //EdgeKey tempKey = new EdgeKey(typeUUID, attributeUUID);
            loadExcept.Value.AddBackwardEdge(beEdgeKey, reference, this);
            if (loadExcept.Value.isNew)
            {
                return _IGraphFSSession.StoreFSObject(loadExcept.Value, true);
            }
            else
            {
                return loadExcept.Value.Save();
            }

        }

        public Exceptional<Boolean> RemoveBackwardEdge(DBObjectStream aDBObject, TypeUUID uUIDofType, AttributeUUID uUIDofAttribute, ObjectUUID reference)
        {
            return aDBObject.RemoveBackwardEdge(uUIDofType, uUIDofAttribute, reference, _DBContext.DBObjectManager);
        }

        public EdgeKey GetBackwardEdgeKey(List<EdgeKey> myEdges, int myDesiredBELevel)
        {
            #region data

            GraphDBType tempType;
            TypeAttribute tempAttr;

            #endregion

            tempType = _DBContext.DBTypeManager.GetTypeByUUID(myEdges[myDesiredBELevel].TypeUUID);
            tempAttr = tempType.GetTypeAttributeByUUID(myEdges[myDesiredBELevel].AttrUUID);

            if (!tempAttr.IsBackwardEdge)
            {
                return new EdgeKey(tempType.UUID, tempAttr.UUID);
            }
            else
            {
                return tempAttr.BackwardEdgeDefinition;
            }
        }

        #endregion


    }
}
