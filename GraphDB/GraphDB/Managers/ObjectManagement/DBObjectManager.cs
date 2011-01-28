/* <id name="GraphDB – ObjectManager" />
 * <copyright file="ObjectManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class manages the Object handling between DB and FS.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.TypeManagement;
using sones.GraphFS;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Indices;

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
            return CreateNewDBObjectStream(myGraphType, _attrs, myUndefAttributes, null, myToken, myCheckUniqueness);
        }

        #endregion

        #region CreateNewDBObject(myGraphType, myDBObjectAttributes, myExtractSettings)

        /// <summary>
        /// Creates a new DBObject of a given GraphType inserts its UUID into all indices of the given GraphType.
        /// </summary>
        /// <param name="_graphDBType">The GraphType of the DBObject to create</param>
        /// <param name="myDBObjectAttributes">A dictionary of attributes for the new DBObject</param>
        /// <param name="myExtractSettings">Special values which should be set to a object</param>
        /// <returns>The UUID of the new DBObject</returns>
        public Exceptional<DBObjectStream> CreateNewDBObject(GraphDBType myGraphDBType, Dictionary<String, IObject> myDBObjectAttributes, Dictionary<String, IObject> myUndefAttributes, Dictionary<ASpecialTypeAttribute, Object> mySpecialTypeAttributes, SessionSettings mySessionSettings, Boolean myCheckUniqueness)
        {
            Dictionary<AttributeUUID, IObject> _attrs = myDBObjectAttributes.ToDictionary(key => myGraphDBType.GetTypeAttributeByName(key.Key).UUID, value => value.Value);
            return CreateNewDBObjectStream(myGraphDBType, _attrs, myUndefAttributes, mySpecialTypeAttributes, mySessionSettings, myCheckUniqueness);
        }

        #endregion

        #region CreateNewDBObjectStream(myGraphType, myDBObjectAttributes)

        /// <summary>
        /// Creates a new DBObject of a given GraphType inserts its UUID into all indices of the given GraphType.
        /// </summary>
        /// <param name="_graphDBType">The GraphType of the DBObject to create</param>
        /// <param name="myDBObjectAttributes">A dictionary of attributes for the new DBObject</param>
        /// <param name="mySpecialTypeAttributes">Special values which should be set to a object</param>
        /// <param name="myCheckUniqueness">check for unique constraints</param> 
        /// <returns>The UUID of the new DBObject</returns>
        public Exceptional<DBObjectStream> CreateNewDBObjectStream(GraphDBType myGraphDBType, Dictionary<AttributeUUID, IObject> myDBObjectAttributes, Dictionary<String, IObject> myUndefAttributes, Dictionary<ASpecialTypeAttribute, Object> mySpecialTypeAttributes, SessionSettings mySessionSettings, Boolean myCheckUniqueness)
        {

            #region Input validation

            if (myGraphDBType == null)
                return new Exceptional<DBObjectStream>(new Error_ArgumentNullOrEmpty("The parameter myGraphType must not be null!"));

            if (myDBObjectAttributes == null)
                return new Exceptional<DBObjectStream>(new Error_ArgumentNullOrEmpty("The parameter myDBObjectAttributes must not be null!"));

            #endregion

            #region Check uniqueness

            if (myCheckUniqueness)
            {

                var parentTypes = _DBContext.DBTypeManager.GetAllParentTypes(myGraphDBType, true, false);
                var CheckVal = _DBContext.DBIndexManager.CheckUniqueConstraint(myGraphDBType, parentTypes, myDBObjectAttributes);

                if (CheckVal.Failed())
                    return new Exceptional<DBObjectStream>(CheckVal);

            }

            #endregion

            #region Create a new DBObjectStream

            DBObjectStream        _NewDBObjectStream               = null;
            ObjectUUID            _NewObjectUUID                   = null;
            ASpecialTypeAttribute _SpecialTypeAttribute_UUID_Key   = new SpecialTypeAttribute_UUID();
            Object                _SpecialTypeAttribute_UUID_Value = null;

            #region Search for an user-defined ObjectUUID

            if (mySpecialTypeAttributes != null)
            {
                if (mySpecialTypeAttributes.TryGetValue(_SpecialTypeAttribute_UUID_Key, out _SpecialTypeAttribute_UUID_Value))
                {

                    // User-defined ObjectUUID of _graphDBType UInt64
                    var _ValueAsUInt64 = _SpecialTypeAttribute_UUID_Value as UInt64?;
                    if (_ValueAsUInt64 != null)
                        _NewObjectUUID = new ObjectUUID(_ValueAsUInt64.Value);

                    // User-defined ObjectUUID of _graphDBType String or anything else...
                    else
                    {

                        var _String = _SpecialTypeAttribute_UUID_Value.ToString();
                        if (_String == null || _String == "")
                            return new Exceptional<DBObjectStream>(new Error_InvalidAttributeValue(SpecialTypeAttribute_UUID.AttributeName, _String));

                        _NewObjectUUID = new ObjectUUID(_SpecialTypeAttribute_UUID_Value.ToString());

                    }

                    mySpecialTypeAttributes[_SpecialTypeAttribute_UUID_Key] = _NewObjectUUID;

                }
            }

            #endregion

            // If _NewObjectUUID == null a new one will be generated!

            if (_NewObjectUUID == null)
            {
                _NewObjectUUID = ObjectUUID.NewUUID;
            }

            _NewDBObjectStream = new DBObjectStream(_NewObjectUUID, 
                                                    myGraphDBType, 
                                                    myDBObjectAttributes,
                                                    new ObjectLocation(myGraphDBType.ObjectLocation, DBConstants.DBObjectsLocation, _NewObjectUUID.ToString()));

            #endregion

            #region Check for duplicate ObjectUUIDs... maybe resolve duplicates!

            var _DBObjectStreamAlreadyExistsResult = ObjectExistsOnFS(_NewDBObjectStream);

            if (_DBObjectStreamAlreadyExistsResult.Failed())
                return new Exceptional<DBObjectStream>(_DBObjectStreamAlreadyExistsResult);
            


            while (_DBObjectStreamAlreadyExistsResult.Value == Trinary.TRUE)
            {
                if (_SpecialTypeAttribute_UUID_Value != null)
                {
                    //so there was an explicit UUID
                    return new Exceptional<DBObjectStream>(new Error_DBObjectCollision(_NewDBObjectStream));

                }

                var newUUID = ObjectUUID.NewUUID;

                _NewDBObjectStream = new DBObjectStream(newUUID, 
                                                        myGraphDBType, 
                                                        myDBObjectAttributes,
                                                        new ObjectLocation(myGraphDBType.ObjectLocation, DBConstants.DBObjectsLocation, newUUID.ToString()));

                _DBObjectStreamAlreadyExistsResult = ObjectExistsOnFS(_NewDBObjectStream);

                if (_DBObjectStreamAlreadyExistsResult.Failed())
                    return new Exceptional<DBObjectStream>(_DBObjectStreamAlreadyExistsResult);

            }

            #endregion

            #region Check for ExtractSetting attributes unlike UUID

            if (mySpecialTypeAttributes != null && mySpecialTypeAttributes.Any())
            {
                foreach (var _SpecialAttribute in mySpecialTypeAttributes)
                {
                    // Skip SpecialTypeAttribute_UUID!
                    if (!(_SpecialAttribute.Key is SpecialTypeAttribute_UUID))
                    {
                        var result = _SpecialAttribute.Key.ApplyTo(_NewDBObjectStream, _SpecialAttribute.Value);
                        if (result.Failed())
                        {
                            return new Exceptional<DBObjectStream>(result);
                        }
                    }
                }
            }

            #endregion
            
            // Flush new DBObject
            var flushResult = FlushDBObject(_NewDBObjectStream);

            if (flushResult.Failed())
                return new Exceptional<DBObjectStream>(flushResult);
            
            #region Add UUID of the new DBObject to all indices of myGraphType

            var uuidIndex = myGraphDBType.GetUUIDIndex(_DBContext);
            uuidIndex.Insert(_NewDBObjectStream, myGraphDBType, _DBContext);

            foreach (var _GraphDBType in _DBContext.DBTypeManager.GetAllParentTypes(myGraphDBType, true, false))
            {
                foreach (var _AAttributeIndex in _GraphDBType.GetAllAttributeIndices(_DBContext, false))
                {

                    //Find out if the dbobject carries all necessary attributes
                    if (_NewDBObjectStream.HasAtLeastOneAttribute(_AAttributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs, _GraphDBType, mySessionSettings))
                    {
                        //valid dbo for idx
                        _AAttributeIndex.Insert(_NewDBObjectStream, _GraphDBType, _DBContext);
                    }
                }
            }

            #endregion

            #region add undefined attributes to the object

            if (!myUndefAttributes.IsNullOrEmpty())
            {
                foreach (var item in myUndefAttributes)
                {
                    var addExcept = _NewDBObjectStream.AddUndefinedAttribute(item.Key, item.Value, this);

                    if (addExcept.Failed())
                    {
                        return new Exceptional<DBObjectStream>(addExcept);
                    }
                }
            }

            #endregion

            return new Exceptional<DBObjectStream>(_NewDBObjectStream);

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
        /// <param name="myObjectUUID">The unique identifier of the DBObject</param>
        /// <returns>The requested DBObject or null.</returns>
        public Exceptional<DBObjectStream> LoadDBObject(GraphDBType myGraphDBType, ObjectUUID myObjectUUID)
        {

            if (myObjectUUID == null)
                return new Exceptional<DBObjectStream>(new Error_ArgumentNullOrEmpty("ObjectUUID should not be null."));

            if(myGraphDBType == null)
                return new Exceptional<DBObjectStream>(new Error_ArgumentNullOrEmpty("DBType should not be null."));

            return LoadDBObject(myGraphDBType, myObjectUUID, myGraphDBType.Name);

        }

        public Exceptional<DBObjectStream> LoadDBObject(GraphDBType myGraphDBType, ObjectUUID myObjectUUID, String myNameOfType)
        {
            return LoadDBObject(new ObjectLocation(myGraphDBType.ObjectLocation, DBConstants.DBObjectsLocation, myObjectUUID.ToString()));
        }

        public Exceptional<DBObjectStream> LoadDBObject(ObjectLocation myObjectLocation)
        {

            var _DBExceptional        = new Exceptional<DBObjectStream>();
            var _GetObjectExceptional = _IGraphFSSession.GetFSObject<DBObjectStream>(myObjectLocation, DBConstants.DBOBJECTSTREAM, FSConstants.DefaultEdition, null, 0, false);

            if (_GetObjectExceptional == null || _GetObjectExceptional.Failed() || _GetObjectExceptional.Value == null)
            {
                return _GetObjectExceptional.Convert<DBObjectStream>().PushIErrorT(new Error_LoadObject(myObjectLocation));
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

            foreach (var anIndex in myTypeOfDBObject.GetAllAttributeIndices(_DBContext, true))
            {
                anIndex.Remove(myDBObject, myTypeOfDBObject, _DBContext);
            }

            #endregion

            #region remove from fs

            #region Remove DBOBJECTSTREAM

            var _RemoveObjectExceptional = _IGraphFSSession.RemoveObjectIfExists(myDBObjectLocation, DBConstants.DBOBJECTSTREAM, FSConstants.DefaultEdition);
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

            _RemoveObjectExceptional = _IGraphFSSession.RemoveObjectIfExists(myDBObjectLocation, DBConstants.UNDEFATTRIBUTESSTREAM, FSConstants.DefaultEdition);
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

            //var existExcept = _IGraphFSSession.ObjectStreamExists(myEdgeLocation, DBConstants.DBBACKWARDEDGESTREAM);

            //if (existExcept.Failed())
            //    return new Exceptional<BackwardEdgeStream>(existExcept);

            //if (existExcept.Value == Trinary.TRUE)
            //{
            //    var loadException = _IGraphFSSession.GetFSObject<BackwardEdgeStream>(myEdgeLocation, DBConstants.DBBACKWARDEDGESTREAM, null, null, 0, false);

            //    if (loadException.Failed())
            //        return new Exceptional<BackwardEdgeStream>(loadException);

            //    retDBBackwardEdge = loadException.Value;
            //}
            //else
            //{
            //    retDBBackwardEdge = new BackwardEdgeStream(myEdgeLocation);
            //}

            return _IGraphFSSession.GetOrCreateFSObject<BackwardEdgeStream>(myEdgeLocation, DBConstants.DBBACKWARDEDGESTREAM, () => new BackwardEdgeStream(myEdgeLocation), null, null, 0, false);

            return new Exceptional<BackwardEdgeStream>(retDBBackwardEdge);
        }

        public Exceptional<Boolean> AddBackwardEdge(DBObjectStream aDBObject, TypeUUID uUIDofType, AttributeUUID uUIDofAttribute, ObjectUUID reference)
        {
            return aDBObject.AddBackwardEdge(uUIDofType, uUIDofAttribute, reference, _DBContext.DBObjectManager);
        }

        internal Exceptional AddBackwardEdge(ObjectUUID uuid, TypeUUID typeOfBESource, EdgeKey beEdgeKey, ObjectUUID reference)
        {
            //load the backward edge
            var loadExcept = _DBContext.DBObjectCache.LoadDBBackwardEdgeStream(typeOfBESource, uuid); // LoadBackwardEdge(beLocation);

            if (loadExcept.Failed())
            {
                return new Exceptional(loadExcept);
            }

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

        public IEnumerable<string> GetAllStreamsRecursive(ObjectLocation objectLocation, string ObjectStream)
        {
            #region currentDir

            foreach (var aElement in _IGraphFSSession.GetFilteredDirectoryListing(objectLocation, null, null, null, new List<String>() { ObjectStream }, null, null, null, null, null, null).Value)
            {
                yield return aElement;
            }

            #endregion

            #region  subdirs

            var ignoredDirectories = new String[] { ".", ".." };

            foreach (var aSubdir in _IGraphFSSession.GetFilteredDirectoryListing(objectLocation, null, null, null, new List<String>() { FSConstants.DIRECTORYSTREAM }, null, null, null, null, null, null).Value)
            {
                if (!ignoredDirectories.Contains(aSubdir))
                {
                    foreach (var aElement in GetAllStreamsRecursive(new ObjectLocation(objectLocation, aSubdir), ObjectStream))
                    {
                        yield return aElement;
                    }
                }
            }

            #endregion

            yield break;
        }

    }
}
