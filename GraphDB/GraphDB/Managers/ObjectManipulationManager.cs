/* <id name="GraphDB – ObjectManipulationManager" />
 * <copyright file="ObjectManipulationManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class will handle all manipulations on DBObjects coming from Irony statement nodes like InsertNode, UpdateNode, DeleteNode, etc.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.ExpressionGraph;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.Warnings;
using sones.GraphDB.Result;
using sones.GraphDB.TypeManagement;

using sones.GraphFS.DataStructures;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.NewAPI;
using sones.GraphDB.Settings.DatabaseSettings;

#endregion

namespace sones.GraphDB.Managers
{

    /// <summary>
    /// This class will handle all manipulations on DBObjects coming from Irony statement nodes like InsertNode, UpdateNode, DeleteNode, etc
    /// </summary>
    public class ObjectManipulationManager
    {

        #region Data & Properties
        
        #endregion

        public ObjectManipulationManager(DBContext dbContext = null)
        {
        }

        #region update methods

        /// <summary>
        /// This will evaluate the <paramref name="myWhereExpression"/> and add some warnings coming from 
        /// </summary>
        /// <param name="myWhereExpression"></param>
        /// <param name="myListOfUpdates"></param>
        /// <param name="undefAttributeNodes"></param>
        /// <param name="dbObjectCache"></param>
        /// <returns></returns>
        public QueryResult Update(IEnumerable<AAttributeAssignOrUpdateOrRemove> myListOfUpdates, DBContext myDBContext, GraphDBType myGraphDBType, BinaryExpressionDefinition myWhereExpression = null)
        {
            IEnumerable<Exceptional<DBObjectStream>> _dbobjects;
            IEnumerable<IWarning> warnings = null;
       
            #region get GUIDs

            if (myWhereExpression != null)
            {
                #region get guids via where

                myWhereExpression.Validate(myDBContext);
                if (myWhereExpression.ValidateResult.Failed())
                {
                    return new QueryResult(myWhereExpression.ValidateResult);
                }

                var _tempGraphResult = myWhereExpression.Calculon(myDBContext, new CommonUsageGraph(myDBContext), false);

                if (_tempGraphResult.Failed())
                {
                    return new QueryResult(_tempGraphResult);
                }
                else
                {
                    _dbobjects = _tempGraphResult.Value.Select(new LevelKey(myGraphDBType, myDBContext.DBTypeManager), null, true);
                    if (!_tempGraphResult.Success())
                    {
                        warnings = _tempGraphResult.IWarnings;
                    }
                }
                
                #endregion
            }
            else
            {
                #region get guids via guid-idx

                _dbobjects = myDBContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(myGraphDBType, myDBContext.DBTypeManager), myDBContext);

                #endregion
            }

            #endregion

            var updateResult = Update(_dbobjects, myListOfUpdates, myDBContext, myGraphDBType);

            #region expressionGraph error handling

            if (!warnings.IsNullOrEmpty())
            {
                updateResult.PushIWarnings(warnings);
            }

            #endregion

            return updateResult;

        }

        /// <summary>
        /// This is the update method which will change some <paramref name="myDBObjects"/> an behalf of the <paramref name="myListOfUpdates"/>
        /// </summary>
        /// <param name="myDBObjects">Some dbobjects</param>
        /// <param name="myListOfUpdates">The list of update tasks (assign, delete, etc)</param>
        /// <param name="dbObjectCache"></param>
        /// <returns></returns>
        public QueryResult Update(IEnumerable<Exceptional<DBObjectStream>> myDBObjects, IEnumerable<AAttributeAssignOrUpdateOrRemove> myListOfUpdates, DBContext myDBContext, GraphDBType myGraphDBType)
        {

            #region Data

            var queryResultContent = new List<Vertex>();
            var queryResult = new QueryResult();
            Warning_UndefinedAttribute undefAttrWarning = null;

            #endregion

            #region check for undefined attributes setting

            var undefAttrSetting = myDBContext.DBSettingsManager.GetSetting(SettingUndefAttrBehaviour.UUID, myDBContext, TypesSettingScope.DB);

            if (!undefAttrSetting.Success())
            {
                return new QueryResult(undefAttrSetting);
            }

            var undefSettingVal = ((SettingUndefAttrBehaviour)undefAttrSetting.Value).Behaviour;

            #endregion

            #region Validate attributes

            foreach (var updateOrAssign in myListOfUpdates)
            {
                System.Diagnostics.Debug.Assert(updateOrAssign != null);
                if (updateOrAssign is AAttributeAssignOrUpdateOrRemove && !(updateOrAssign is AttributeRemove))
                {
                    var result = (updateOrAssign as AAttributeAssignOrUpdateOrRemove).AttributeIDChain.Validate(myDBContext, true);

                    if (updateOrAssign.IsUndefinedAttributeAssign)
                    {
                        switch (undefSettingVal)
                        {
                            case UndefAttributeBehaviour.disallow:
                                return new QueryResult(new Error_UndefinedAttributes());

                            case UndefAttributeBehaviour.warn:
                                if (undefAttrWarning == null)
                                {
                                    undefAttrWarning = new Warning_UndefinedAttribute();
                                }

                                queryResult.PushIWarning(undefAttrWarning);
                                break;
                        }
                    }

                    if (result.Failed())
                    {
                        return new QueryResult(result);
                    }
                }
            }

            //var undefinedAttributeAssignments = myListOfUpdates.Where(a => a.IsUndefinedAttributeAssign);
            var definedAttributeAssignments = myListOfUpdates.Where(a => !a.IsUndefinedAttributeAssign);
            
            #endregion

            #region check unique constraint - refactor!!

            if (definedAttributeAssignments.CountIsGreater(0))
            {

                Exceptional<Boolean> CheckConstraint = null;

                IEnumerable<GraphDBType> parentTypes = myDBContext.DBTypeManager.GetAllParentTypes(myGraphDBType, true, false);
                Dictionary<AttributeUUID, IObject> ChkForUnique = new Dictionary<AttributeUUID, IObject>();

                foreach (var entry in myDBObjects)
                {
                    //all attributes, that are going to be changed
                    var attrsToCheck = entry.Value.GetAttributes().Where(item => definedAttributeAssignments.Select(updAttrs => GetAttributesToCheckForUnique(updAttrs)).Contains(item.Key));

                    foreach (var attrValue in attrsToCheck)
                    {
                        if (!ChkForUnique.ContainsKey(attrValue.Key))
                            ChkForUnique.Add(attrValue.Key, attrValue.Value);
                    }

                    CheckConstraint = myDBContext.DBIndexManager.CheckUniqueConstraint(myGraphDBType, parentTypes, ChkForUnique);
                    ChkForUnique.Clear();

                    if (CheckConstraint.Failed())
                        return new QueryResult(CheckConstraint.IErrors);

                }
            }

            #endregion

            #region regular update

            foreach (var aDBO in myDBObjects)
            {
                //key: attribute name
                //value: TypeAttribute, NewValue
                Dictionary<String, Tuple<TypeAttribute, IObject>> attrsForResult = new Dictionary<String, Tuple<TypeAttribute, IObject>>();

                if (aDBO.Failed())
                {
                    return new QueryResult(aDBO);
                }

                #region data

                Boolean sthChanged = false;

                #endregion
                
                #region iterate tasks

                //Exceptional<Boolean> partialResult;

                foreach (var attributeUpdateOrAssign in myListOfUpdates)
                {

                    var updateResult = attributeUpdateOrAssign.Update(myDBContext, aDBO.Value, myGraphDBType);
                    if (updateResult.Failed())
                    {
                        return new QueryResult(updateResult);
                    }

                    if (updateResult.Value.Count > 0)
                    {
                        sthChanged = true;
                        attrsForResult.AddRange(updateResult.Value);
                    }

                }

                #endregion
                
                if (sthChanged)
                {
                    var definedAttributes = ExtractDefinedAttributes(attrsForResult, myDBContext.DBTypeManager);

                    #region update Idx

                    foreach (var _AttributeIndex in myGraphDBType.GetAllAttributeIndices())
                    {
                        if(definedAttributes.Exists(item => _AttributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs.Contains(item.Key.Definition.UUID)))
                        {
                            //execute update
                            _AttributeIndex.Update(aDBO.Value, myGraphDBType, myDBContext);
                        }
                    }


                    #endregion

                    #region update dbobjects on fs

                    var flushResult = myDBContext.DBObjectManager.FlushDBObject(aDBO.Value);
                    if (!flushResult.Success())
                    {
                        return new QueryResult(flushResult);
                    }

                    #endregion

                    var resultSet = GetManipulationResultSet(myDBContext, aDBO, myGraphDBType, undefinedAttributes: ExtractUndefindedAttributes(attrsForResult), attributes: definedAttributes);
                    if (!resultSet.Success())
                    {
                        /*
                          what should happen now 
                          this should not break the update
                        */
                    }

                    queryResultContent.Add(resultSet.Value);
                }
            }

            #endregion

            #region Return all affected dbObjectUUIDs

            queryResult.Vertices = queryResultContent;

            return queryResult;

            #endregion

        }

        private Dictionary<TypeAndAttributeDefinition, IObject> ExtractDefinedAttributes(Dictionary<string, Tuple<TypeAttribute, IObject>> attrsForResult, DBTypeManager myTypeManager)
        {
            return attrsForResult.Where(item => !(item.Value.Item1 is UndefinedTypeAttribute)).ToDictionary(key => new TypeAndAttributeDefinition(key.Value.Item1, key.Value.Item1.GetDBType(myTypeManager)), value => value.Value.Item2);
        }

        private Dictionary<string, IObject> ExtractUndefindedAttributes(Dictionary<string, Tuple<TypeAttribute, IObject>> attrsForResult)
        {
            return attrsForResult.Where(item => (item.Value.Item1 is UndefinedTypeAttribute)).ToDictionary(key => key.Key, value => value.Value.Item2);
        }

        #region AttributesToCheckForUnique

        public AttributeUUID GetAttributesToCheckForUnique(AAttributeAssignOrUpdateOrRemove myAAttributeAssignOrUpdateOrRemove)
        {

            if (myAAttributeAssignOrUpdateOrRemove is AAttributeRemove)
                return null;

            if (myAAttributeAssignOrUpdateOrRemove.AttributeIDChain.IsUndefinedAttribute)
            {
                return null;
            }

            if (myAAttributeAssignOrUpdateOrRemove is AAttributeAssignOrUpdate)
            {
                return ((AAttributeAssignOrUpdate)myAAttributeAssignOrUpdateOrRemove).AttributeIDChain.LastAttribute.UUID;
            }

            if (myAAttributeAssignOrUpdateOrRemove is AttributeAssignOrUpdateList)
                return ((AttributeAssignOrUpdateList)myAAttributeAssignOrUpdateOrRemove).AttributeIDChain.LastAttribute.UUID;

            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        #endregion


        public Exceptional<Tuple<String, TypeAttribute, IListOrSetEdgeType>> ApplyUpdateListAttribute(AAttributeAssignOrUpdateOrRemove myAttributeUpdateOrAssign, DBContext dbContext, DBObjectStream aDBObject, GraphDBType _Type)
        {

            if (myAttributeUpdateOrAssign is AttributeAssignOrUpdateList)
            {

            }
            else if (myAttributeUpdateOrAssign is AttributeRemoveList)
            {

            }
            else
            {
                return new Exceptional<Tuple<String, TypeAttribute, IListOrSetEdgeType>>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            return new Exceptional<Tuple<String, TypeAttribute, IListOrSetEdgeType>>(null as Tuple<String, TypeAttribute, IListOrSetEdgeType>);
        }

        
        #endregion

        #region insert methods

        public QueryResult Insert(DBContext dbContext, GraphDBType type, ManipulationAttributes myManipulationAttributes, Boolean checkUniqueness = true)
        {

            var attributes              = myManipulationAttributes.Attributes;
            var undefinedAttributes     = myManipulationAttributes.UndefinedAttributes;
            var specialTypeAttributes   = myManipulationAttributes.SpecialTypeAttributes;

            if (type.IsAbstract)
            {
                return new QueryResult(new Error_InvalidType(type, "It is not possible to insert DBObjects of an abstract type"));
            }

            #region Data

            Exceptional<DBObjectStream> dbObjectStream;

            #endregion

            #region create new DBObject

            dbObjectStream = dbContext.DBObjectManager.CreateNewDBObject(type, attributes.ToDictionary(key => key.Key.Definition.Name, value => value.Value), undefinedAttributes, specialTypeAttributes, dbContext.SessionSettings, checkUniqueness);

            if (dbObjectStream.Failed())
            {
                return new QueryResult(dbObjectStream.IErrors);
            }

            #endregion

            #region set backward edges for reference attributes

            var userdefinedAttributes = from attr in attributes where attr.Key.TypeOfAttribute.IsUserDefined select attr;

            if (userdefinedAttributes.CountIsGreater(0))
            {
                var setBackEdges = SetBackwardEdges(type, userdefinedAttributes.ToDictionary(key => key.Key.Definition.UUID, value => value.Value), dbObjectStream.Value.ObjectUUID, dbContext);

                if (setBackEdges.Failed())
                    return new QueryResult(setBackEdges.IErrors);
            }

            #endregion

            #region Create QueryResult

            var readOut = GetManipulationResultSet(dbContext, dbObjectStream, type, attributes, undefinedAttributes, specialTypeAttributes);
            var selResultSet = new List<Vertex> { readOut.Value };
            var queryResult = new QueryResult(selResultSet);

            #endregion

            #region If readout creation failed, this is a readon to fail the query result at all

            if (readOut.Failed())
            {
                queryResult.PushIErrors(readOut.IErrors);
            }

            #endregion

            return queryResult;

        }

        #endregion

        #region InsertOrReplace

        public QueryResult InsertOrReplace(DBContext myDBContext, GraphDBType myGraphDBType, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myBinaryExpressionDefinition = null)
        {

            List<IWarning> warnings = new List<IWarning>();
            IEnumerable<Exceptional<DBObjectStream>> myDBObjects;

            #region myBinaryExpressionDefinition

            if (myBinaryExpressionDefinition != null)
            {
                myBinaryExpressionDefinition.Validate(myDBContext);
                if (myBinaryExpressionDefinition.ValidateResult.Failed())
                {
                    return new QueryResult(myBinaryExpressionDefinition.ValidateResult);
                }

                var _whereGraphResult = myBinaryExpressionDefinition.Calculon(myDBContext, new CommonUsageGraph(myDBContext), false);

                if (_whereGraphResult.Success())
                {
                    myDBObjects = _whereGraphResult.Value.Select(new LevelKey(myGraphDBType, myDBContext.DBTypeManager), null, true);
                }
                else
                {
                    return new QueryResult(_whereGraphResult.IErrors);
                }

                #region expressionGraph error handling

                warnings.AddRange(_whereGraphResult.Value.GetWarnings());

                #endregion

            }
            else
            {
                myDBObjects = new List<Exceptional<DBObjectStream>>();
            }

            #endregion

            IEnumerable<GraphDBType> parentTypes = myDBContext.DBTypeManager.GetAllParentTypes(myGraphDBType, true, false);

            #region Get attribute assignies prior to deletion to act on errors

            var attrsResult = EvaluateAttributes(myDBContext, myGraphDBType, myAttributeAssignList);
            if (!attrsResult.Success())
            {
                return new QueryResult(attrsResult);
            }

            var assingedAttrs = attrsResult.Value.Attributes.ToDictionary(key => key.Key.Definition.UUID, value => value.Value);

            var checkUniqueVal = myDBContext.DBIndexManager.CheckUniqueConstraint(myGraphDBType, parentTypes, assingedAttrs);
            if (checkUniqueVal.Failed())
                return new QueryResult(checkUniqueVal.IErrors);

            #endregion

            #region Delete the object

            if (myDBObjects.CountIsGreater(1))
            {
                return new QueryResult(new Error_MultipleResults());
            }
            else
            {
                if (myDBObjects.CountIs(1))
                {
                    #region delete


                    var DeleteResult = DeleteDBObjects(myGraphDBType, null, myDBObjects, myDBContext);

                    if (DeleteResult.Failed())
                    {
                        return new QueryResult(DeleteResult.IErrors);
                    }

                    #endregion
                }

            }

            #endregion

            #region Insert with new values

            //Insert
            var result = Insert(myDBContext, myGraphDBType, attrsResult.Value, false);
            result.PushIWarnings(warnings);
            //if there were any warnings during this process, the need to be added

            #endregion

            return result;

        }

        #endregion

        #region InsertOrUpdate

        public QueryResult InsertOrUpdate(DBContext myDBContext, GraphDBType myGraphDBType, List<AAttributeAssignOrUpdate>  _AttributeAssignList, BinaryExpressionDefinition myBinaryExpressionDefinition = null)
        {

            List<IWarning> warnings = new List<IWarning>();
            IEnumerable<Exceptional<DBObjectStream>> extractedDBOs = null;

            var manipulationAttributes = EvaluateAttributes(myDBContext, myGraphDBType, _AttributeAssignList);
            if (!manipulationAttributes.Success())
            {
                return new QueryResult(manipulationAttributes);
            }
            var attributeAssignOrUpdateOrRemoveList = manipulationAttributes.Value.AttributeToUpdateOrAssign;

            if (myBinaryExpressionDefinition != null)
            {
                myBinaryExpressionDefinition.Validate(myDBContext);
                if (myBinaryExpressionDefinition.ValidateResult.Failed())
                {
                    return new QueryResult(myBinaryExpressionDefinition.ValidateResult);
                }
                var _whereGraphResult = myBinaryExpressionDefinition.Calculon(myDBContext, new CommonUsageGraph(myDBContext), false);

                if (_whereGraphResult.Success())
                {
                    extractedDBOs = _whereGraphResult.Value.Select(new LevelKey(myGraphDBType, myDBContext.DBTypeManager), null, true);
                }
                else
                {
                    return new QueryResult(_whereGraphResult.IErrors);
                }

                #region expressionGraph error handling

                warnings.AddRange(_whereGraphResult.Value.GetWarnings());

                #endregion
            }
            else
            {
                extractedDBOs = new List<Exceptional<DBObjectStream>>();
            }

            QueryResult result;

            if (extractedDBOs.Count() == 0)
            {

                result = Insert(myDBContext, myGraphDBType, manipulationAttributes.Value, true);
            }
            else
            {
                if (extractedDBOs.Count() > 1)
                    return new QueryResult(new Error_MultipleResults());

                result = Update(extractedDBOs, attributeAssignOrUpdateOrRemoveList, myDBContext, myGraphDBType);
            }

            result.PushIWarnings(warnings);

            return result;

        }

        
        #endregion
        
        #region Replace

        public QueryResult Replace(DBContext myDBContext, GraphDBType myGraphDBType, List<AAttributeAssignOrUpdate> myAttributeAssignList, BinaryExpressionDefinition myBinaryExpressionDefinition)
        {

            List<IWarning> warnings = new List<IWarning>();
            IEnumerable<Exceptional<DBObjectStream>> _dbObjects = null;

            myBinaryExpressionDefinition.Validate(myDBContext);
            if (myBinaryExpressionDefinition.ValidateResult.Failed())
            {
                return new QueryResult(myBinaryExpressionDefinition.ValidateResult);
            }

            var _whereGraphResult = myBinaryExpressionDefinition.Calculon(myDBContext, new CommonUsageGraph(myDBContext), false);

            if (_whereGraphResult.Success())
            {
                _dbObjects = _whereGraphResult.Value.Select(new LevelKey(myGraphDBType, myDBContext.DBTypeManager), null, true);
            }
            else
            {
                return new QueryResult(_whereGraphResult.IErrors);
            }

            #region expressionGraph error handling

            warnings.AddRange(_whereGraphResult.Value.GetWarnings());

            #endregion

            if (_dbObjects.CountIsGreater(1))
            {
                return new QueryResult(new Error_MultipleResults());
            }

            if (_dbObjects.CountIs(0))
            {
                warnings.Add(new Warnings.Warning_NoObjectsToReplace());
                return new QueryResult(new List<IError>(), warnings);
            }

            IEnumerable<GraphDBType> parentTypes = myDBContext.DBTypeManager.GetAllParentTypes(myGraphDBType, true, false);
            Exceptional<Boolean> checkUniqueVal = null;

            var attrsResult = EvaluateAttributes(myDBContext, myGraphDBType, myAttributeAssignList);
            if (!attrsResult.Success())
            {
                return new QueryResult(attrsResult);
            }

            var assingedAttrs = attrsResult.Value.Attributes.ToDictionary(key => key.Key.Definition.UUID, value => value.Value);

            checkUniqueVal = myDBContext.DBIndexManager.CheckUniqueConstraint(myGraphDBType, parentTypes, assingedAttrs);
            if (!checkUniqueVal.Success())
                return new QueryResult(checkUniqueVal);

            var DeleteResult = DeleteDBObjects(myGraphDBType, null, _dbObjects, myDBContext);

            if (!DeleteResult.Success())
            {
                return new QueryResult(DeleteResult);
            }

            var result = Insert(myDBContext, myGraphDBType, attrsResult.Value, false);

            result.PushIWarnings(warnings);

            return result;

        }

        #endregion

        #region SetBackwardEdges

        internal Exceptional SetBackwardEdges(GraphDBType aType, Dictionary<AttributeUUID, IObject> userdefinedAttributes, ObjectUUID reference, DBContext dbContext)
        {

            var returnVal = new Exceptional();

            #region process attributes

            foreach (var aUserDefinedAttribute in userdefinedAttributes)
            {
                #region Data

                GraphDBType     typeOFAttribute     = null;
                TypeAttribute   attributesOfType    = null;

                #endregion

                #region get GraphType of Attribute

                //attributesOfType = aType.Attributes[aUserDefinedAttribute.Key];
                attributesOfType = aType.GetTypeAttributeByUUID(aUserDefinedAttribute.Key);

                typeOFAttribute = dbContext.DBTypeManager.GetTypeByUUID(attributesOfType.DBTypeUUID);

                #endregion

                /* The DBO independent version */
                var beEdge = new EdgeKey(aType.UUID, attributesOfType.UUID);
                
                var runMT = DBConstants.RunMT;
                runMT = false;
                if (runMT)
                {

                    #region The parallel version

                    /**/
                    /* The parallel version */
                    Parallel.ForEach(((IReferenceEdge)aUserDefinedAttribute.Value).GetAllReferenceIDs(), (uuid) =>
                    {
                        var addExcept = dbContext.DBObjectManager.AddBackwardEdge(uuid, attributesOfType.DBTypeUUID, beEdge, reference);

                        if (!addExcept.Success())
                        {
                            returnVal.PushIExceptional(addExcept);
                        }
                    });

                    if (!returnVal.Success())
                    {
                        return returnVal;
                    }
                    /**/
                    
                    #endregion
                    
                }
                else
                {

                    #region Single thread

                    foreach (var uuid in ((IReferenceEdge)aUserDefinedAttribute.Value).GetAllReferenceIDs())
                    {
                        var addExcept = dbContext.DBObjectManager.AddBackwardEdge(uuid, attributesOfType.DBTypeUUID, beEdge, reference);

                        if (addExcept.Failed())
                        {
                            return new Exceptional(addExcept);
                        }
                    }

                    #endregion

                }
            }

            #endregion

            return Exceptional.OK;
        }

        #endregion

        #region delete methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myGraphDBType"></param>
        /// <param name="myAttributes">Pass null if you want to delete the whole object with all attributes</param>
        /// <param name="myListOfAffectedDBObjectUUIDs"></param>
        /// <param name="myDBContext"></param>
        /// <param name="myToken"></param>
        /// <param name="myDBObjectCache"></param>
        /// <returns></returns>
        public Exceptional<IEnumerable<Vertex>> DeleteDBObjects(GraphDBType myGraphDBType, List<TypeAttribute> myAttributes, IEnumerable<Exceptional<DBObjectStream>> myListOfAffectedDBObjects, DBContext myDBContext)
        {
            //some stuff for queryResult
            UInt64 deletionCounter = 0;

            foreach (var aDBO in myListOfAffectedDBObjects)
            {
                if (aDBO.Failed())
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_LoadObject(aDBO.Value.ObjectLocation));
                }

                deletionCounter++;

                if (!myAttributes.IsNullOrEmpty())
                {

                    #region Delete some defined attributes

                    foreach (var Attr in myAttributes)
                    {
                        if (Attr.GetRelatedType(myDBContext.DBTypeManager).GetMandatoryAttributesUUIDs(myDBContext.DBTypeManager).Contains(Attr.UUID))
                        {

                            #region Attribute is mandatory - set default value

                            if ((Boolean)myDBContext.DBSettingsManager.GetSetting(SettingDefaultsOnMandatory.UUID, myDBContext, TypesSettingScope.TYPE, Attr.GetRelatedType(myDBContext.DBTypeManager)).Value.Value.Value)
                            {
                                //the attribute will be removed from the dbobject --> update index
                                var removeAttributefromIdxResult = RemoveDBObjectFromIndex(aDBO.Value, Attr.UUID, myDBContext);

                                if (removeAttributefromIdxResult.Failed())
                                {
                                    return new Exceptional<IEnumerable<Vertex>>(removeAttributefromIdxResult);
                                }

                                var defaultExcept = SetDefaultValue(Attr, aDBO, myDBContext);

                                if (defaultExcept.Failed())
                                    return new Exceptional<IEnumerable<Vertex>>(defaultExcept);

                                if (!defaultExcept.Value)
                                    return new Exceptional<IEnumerable<Vertex>>(new Error_UpdateAttributeValue(Attr));

                                #region update idx

                                //--> update index
                                var updateAttributefromIdxResult = InsertDBObjectIntoIndex(aDBO.Value, Attr.UUID, myDBContext);

                                if (updateAttributefromIdxResult.Failed())
                                {
                                    return new Exceptional<IEnumerable<Vertex>>(updateAttributefromIdxResult);
                                }

                                #endregion
                            }
                            else
                                return new Exceptional<IEnumerable<Vertex>>(new Error_MandatoryConstraintViolation(Attr.Name));

                            #endregion

                        }
                        else
                        {
                            //the attribute will be removed from the dbobject --> update index
                            var removeAttributefromIdxResult = RemoveDBObjectFromIndex(aDBO.Value, Attr.UUID, myDBContext);

                            if (removeAttributefromIdxResult.Failed())
                            {
                                return new Exceptional<IEnumerable<Vertex>>(removeAttributefromIdxResult);
                            }

                            if (!aDBO.Value.RemoveAttribute(Attr.UUID))
                            {
                                return new Exceptional<IEnumerable<Vertex>>(new Error_RemoveTypeAttribute(myGraphDBType, Attr));
                            }
                            
                        }
                    }

                    var saveResult = aDBO.Value.Save();

                    if (!saveResult.Success())
                    {
                        return new Exceptional<IEnumerable<Vertex>>(saveResult);
                    }

                    #endregion

                }
                else
                {

                    #region Remove complete DBO

                    var objID = aDBO.Value.ObjectUUID;

                    var backwardEdgeLoadExcept = myDBContext.DBObjectManager.LoadBackwardEdge(aDBO.Value.ObjectLocation);

                    if (!backwardEdgeLoadExcept.Success())
                        return new Exceptional<IEnumerable<Vertex>>(backwardEdgeLoadExcept.IErrors.First());

                    var backwarEdges = backwardEdgeLoadExcept.Value;

                    var removeResult = myDBContext.DBObjectManager.RemoveDBObject(myGraphDBType, aDBO.Value, myDBContext.DBObjectCache, myDBContext.SessionSettings);

                    if (!removeResult.Success())
                    {
                        return new Exceptional<IEnumerable<Vertex>>(removeResult);
                    }

                    if (backwarEdges != null)
                    {
                        var delEdgeRefs = DeleteObjectReferences(objID, backwarEdges, myDBContext);

                        if (!delEdgeRefs.Success())
                            return new Exceptional<IEnumerable<Vertex>>(delEdgeRefs.IErrors.First());
                    }

                    #endregion

                }            
            }

            if (deletionCounter == 0)
            {
                var result = new Exceptional<IEnumerable<Vertex>>(new List<Vertex>());
                return result.PushIWarningT(new Warning_NoObjectsToDelete());
            }

            return new Exceptional<IEnumerable<Vertex>>(new List<Vertex>());

        }

        /// <summary>
        /// Inserts into all indices that are affected if a DBObject is updated on a certain attibute
        /// </summary>
        /// <param name="dBObjectStream"></param>
        /// <param name="attributeUUID"></param>
        /// <param name="myDBContext"></param>
        /// <returns></returns>
        private Exceptional InsertDBObjectIntoIndex(DBObjectStream dBObjectStream, AttributeUUID attributeUUID, DBContext myDBContext)
        {
            foreach (var aType in myDBContext.DBTypeManager.GetAllParentTypes(myDBContext.DBTypeManager.GetTypeByUUID(dBObjectStream.TypeUUID), true, false))
            {
                foreach (var aAttributeIdx in aType.GetAttributeIndices(attributeUUID))
                {
                    var result = aAttributeIdx.Insert(dBObjectStream, aType, myDBContext);

                    if (result.Failed())
                    {
                        return new Exceptional(result);
                    }
                }
            }

            return new Exceptional();
        }

        /// <summary>
        /// Removes index entries corresponding to a DBObject
        /// </summary>
        /// <param name="dBObjectStream"></param>
        /// <param name="attributeUUID"></param>
        /// <param name="myDBContext"></param>
        /// <returns></returns>
        private Exceptional RemoveDBObjectFromIndex(DBObjectStream dBObjectStream, AttributeUUID attributeUUID, DBContext myDBContext)
        {
            foreach (var aType in myDBContext.DBTypeManager.GetAllParentTypes(myDBContext.DBTypeManager.GetTypeByUUID(dBObjectStream.TypeUUID), true, false))
            {
                foreach (var aAttributeIdx in aType.GetAttributeIndices(attributeUUID))
                {
                    var result = aAttributeIdx.Remove(dBObjectStream, aType, myDBContext);

                    if (result.Failed())
                    {
                        return new Exceptional(result);
                    }
                }
            }

            return new Exceptional();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myWhereExpression"></param>
        /// <param name="myDBContext"></param>
        /// <param name="myTypeWithUndefAttrs">Type, List of undef attrs</param>
        /// <param name="myDBTypeAttributeToDelete">Type, List of attributes</param>
        /// <param name="myReferenceTypeLookup">reference, type</param>
        /// <returns></returns>
        public QueryResult Delete(BinaryExpressionDefinition myWhereExpression, DBContext myDBContext, Dictionary<GraphDBType, List<string>> myTypeWithUndefAttrs, Dictionary<GraphDBType, List<TypeAttribute>> myDBTypeAttributeToDelete, Dictionary<String, GraphDBType> myReferenceTypeLookup)
        {

            QueryResult result = new QueryResult();

            try
            {

                #region get UUIDs

                if (myWhereExpression != null)
                {

                    #region _WhereExpression

                    myWhereExpression.Validate(myDBContext);
                    if (myWhereExpression.ValidateResult.Failed())
                    {
                        return new QueryResult(myWhereExpression.ValidateResult);
                    }

                    var resultGraph = myWhereExpression.Calculon(myDBContext, new CommonUsageGraph(myDBContext), false);
                    if (resultGraph.Failed())
                    {
                        return new QueryResult(resultGraph.IErrors);
                    }

                    IEnumerable<Exceptional<DBObjectStream>> _dbobjects;

                    #region undefined attributes

                    foreach (var type in myTypeWithUndefAttrs)
                    {
                        _dbobjects = resultGraph.Value.Select(new LevelKey(type.Key, myDBContext.DBTypeManager), null, false);

                        foreach (var dbObj in _dbobjects)
                        {
                            foreach (var undefAttr in type.Value)
                            {
                                var removeExcept = myDBContext.DBObjectManager.RemoveUndefinedAttribute(undefAttr, dbObj.Value);

                                if (removeExcept.Failed())
                                    return new QueryResult(removeExcept.IErrors);
                            }
                        }
                    }

                    #endregion

                    #region TypeAttributes to delete

                    Boolean generateLevel = true;

                    foreach (var aToBeDeletedAttribute in myDBTypeAttributeToDelete)
                    {
                        if (!resultGraph.Value.ContainsRelevantLevelForType(aToBeDeletedAttribute.Key))
                        {
                            generateLevel = false;
                        }
                        else
                        {
                            generateLevel = true;
                        }

                        var deleteResult = DeleteDBObjects(aToBeDeletedAttribute.Key, aToBeDeletedAttribute.Value, resultGraph.Value.Select(new LevelKey(aToBeDeletedAttribute.Key, myDBContext.DBTypeManager), null, generateLevel), myDBContext);

                        if (deleteResult.Failed())
                        {
                            return new QueryResult(deleteResult.IErrors);
                        }
                        else
                        {

                            if (!deleteResult.Success())
                            {
                                result.PushIWarnings(deleteResult.IWarnings);
                            }

                            result.Vertices = deleteResult.Value;

                        }

                    }

                    #endregion

                    #region expressionGraph error handling

                    result.PushIWarnings(resultGraph.Value.GetWarnings());

                    #endregion

                    #endregion
                }
                else
                {

                    if (myDBTypeAttributeToDelete.Count == 0)
                    {

                        #region delete only undefined attributes of types

                        foreach (var type in myTypeWithUndefAttrs)
                        {
                            var listOfAffectedDBObjects = myDBContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(type.Key, myDBContext.DBTypeManager), myDBContext);
                            RemoveUndefAttrs(listOfAffectedDBObjects, type.Value, myDBContext);
                        }

                        #endregion

                    }
                    else
                    {
                        #region get guids via guid-idx

                        foreach (var attributeToDelete in myDBTypeAttributeToDelete)
                        {
                            var listOfAffectedDBObjects = myDBContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(attributeToDelete.Key, myDBContext.DBTypeManager), myDBContext).ToList();

                            if (myTypeWithUndefAttrs.ContainsKey(attributeToDelete.Key))
                                RemoveUndefAttrs(listOfAffectedDBObjects, myTypeWithUndefAttrs[attributeToDelete.Key], myDBContext);

                            List<TypeAttribute> attributes = new List<TypeAttribute>();
                            foreach (var attr in attributeToDelete.Value)
                            {
                                attributes.Add(attr);
                            }

                            var Result = DeleteDBObjects(attributeToDelete.Key, attributes, listOfAffectedDBObjects, myDBContext);

                            if (Result.Failed())
                            {
                                return new QueryResult(Result.IErrors);
                            }

                        }

                        #endregion
                    }
                }

                #endregion
            }
            catch (GraphDBException gdbEx)
            {
                return new QueryResult(gdbEx.GraphDBErrors);
            }
            catch (Exception e)
            {
                return new QueryResult(new Error_UnknownDBError(e));
            }

            return result;
        }

        internal Exceptional<Boolean> DeleteObjectReferences(ObjectUUID myObjectUUID, BackwardEdgeStream myObjectBackwardEdges, DBContext dbContext)
        {            
            foreach (var item in myObjectBackwardEdges)
            {
                var type = dbContext.DBTypeManager.GetTypeByUUID(item.Key.TypeUUID);

                if (type == null)
                {
                    return new Exceptional<Boolean>(new Error_TypeDoesNotExist(""));
                }

                foreach (var objID in item.Value.GetAllEdgeDestinations(dbContext.DBObjectCache))
                {
                    if (objID.Failed())
                    {
                        return new Exceptional<Boolean>(objID);
                    }

                    var attr = objID.Value.GetAttribute(item.Key.AttrUUID);

                    if (attr is IReferenceEdge)
                    {
                        var removeResult = ((IReferenceEdge)attr).RemoveUUID(myObjectUUID);
                        if (removeResult)
                        {
                            #region Sucessfully removed the single edge ref - so remove the attribute

                            if (attr is ASingleReferenceEdgeType)
                            {
                                objID.Value.RemoveAttribute(item.Key.AttrUUID);
                            }

                            #endregion
                        }
                        else
                        {
                            return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        }
                    }

                    var flushExcept = dbContext.DBObjectManager.FlushDBObject(objID.Value);

                    if (!flushExcept.Success())
                        return new Exceptional<bool>(flushExcept.IErrors.First());
                }
            }

            return new Exceptional<bool>(true);
        }

        /// <summary>
        /// setting the default value for the attribute
        /// </summary>
        /// <param name="myAttr">the attribute</param>
        /// <param name="myDBObjectCache">the object cache</param>
        /// <returns>true if the value was changed</returns>
        private Exceptional<Boolean> SetDefaultValue(TypeAttribute myAttr, Exceptional<DBObjectStream> myDBO, DBContext dbContext)
        {
            if (myDBO.Success())
            {
                IObject defaultValue = myAttr.DefaultValue;

                if (defaultValue == null)
                    defaultValue = myAttr.GetDefaultValue(dbContext);
                
                var alterExcept = myDBO.Value.AlterAttribute(myAttr.UUID, defaultValue);

                if (alterExcept.Failed())
                    return new Exceptional<Boolean>(alterExcept);
                
                if (!alterExcept.Value)
                {
                    return new Exceptional<bool>(false);
                }
            }
            else
            {
                return new Exceptional<bool>(false);
            }

            return new Exceptional<bool>(true);
        }

        private Exceptional RemoveUndefAttrs(IEnumerable<Exceptional<DBObjectStream>> myDBObjStream, List<String> myUndefAttrs, DBContext dbContext)
        {
            #region delete undefined attributes

            foreach (var objects in myDBObjStream)
            {
                var undefAttrsStream = myUndefAttrs.Where(item => objects.Value.GetUndefinedAttributePayload(dbContext.DBObjectManager).Value.ContainsKey(item));

                foreach (var undefAttr in undefAttrsStream)
                {
                    var removeExcept = dbContext.DBObjectManager.RemoveUndefinedAttribute(undefAttr, objects.Value);

                    if (removeExcept.Failed())
                        return new Exceptional(removeExcept);
                }
            }

            #endregion

            return Exceptional.OK;
        }

        #endregion

        #region GetManipulationResultSet

        /// <summary>
        /// Create a readout based on the passed <paramref name="attributes"/>, <paramref name="undefinedAttributes"/>, <paramref name="specialTypeAttributes"/> which are all optional
        /// </summary>
        /// <param name="myDBObjectStreamExceptional"></param>
        /// <param name="attributes"></param>
        /// <param name="undefinedAttributes"></param>
        /// <param name="specialTypeAttributes"></param>
        /// <returns></returns>
        private Exceptional<Vertex> GetManipulationResultSet(DBContext myDBContext, Exceptional<DBObjectStream> myDBObjectStreamExceptional, GraphDBType myGraphDBType, Dictionary<TypeAndAttributeDefinition, IObject> attributes = null, Dictionary<String, IObject> undefinedAttributes = null, Dictionary<ASpecialTypeAttribute, Object> specialTypeAttributes = null)
        {

            Vertex _Vertex = null;

            #region Return inserted attributes

            #region attributes

            if (!attributes.IsNullOrEmpty())
            {
                _Vertex = new Vertex(attributes.ToDictionary(key => key.Key.Definition.Name, value => value.Value.GetReadoutValue()));
            }
            else
            {
                _Vertex = new Vertex();
            }

            #endregion

            #region UndefinedAttributes

            if (!undefinedAttributes.IsNullOrEmpty())
            {

                foreach (var undefAttr in undefinedAttributes)
                {
                    _Vertex.AddAttribute(undefAttr.Key, undefAttr.Value.GetReadoutValue());
                }

            }

            #endregion

            #region SpecialTypeAttributes

            if (!specialTypeAttributes.IsNullOrEmpty())
            {

                foreach (var specAttr in specialTypeAttributes)
                {
                    _Vertex.AddAttribute(specAttr.Key.Name, specAttr.Value);
                }

            }

            #endregion

            #region UUID

            if (!_Vertex.HasAttribute(SpecialTypeAttribute_UUID.AttributeName))
            {

                var extractedValue = new SpecialTypeAttribute_UUID().ExtractValue(myDBObjectStreamExceptional.Value, myGraphDBType, myDBContext);
                
                if (extractedValue.Failed())
                {
                    return new Exceptional<Vertex>(extractedValue);
                }

                _Vertex.AddAttribute(SpecialTypeAttribute_UUID.AttributeName, extractedValue.Value.GetReadoutValue());

            }

            #endregion

            #region REVISION

            if (!_Vertex.HasAttribute(SpecialTypeAttribute_REVISION.AttributeName)) // If it was updated by SpecialTypeAttributes we do not need to add them again
            {

                var extractedValue = new SpecialTypeAttribute_REVISION().ExtractValue(myDBObjectStreamExceptional.Value, myGraphDBType, myDBContext);
                if (extractedValue.Failed())
                {
                    return new Exceptional<Vertex>(extractedValue);
                }
                _Vertex.AddAttribute(SpecialTypeAttribute_REVISION.AttributeName, extractedValue.Value.GetReadoutValue());

            }

            #endregion

            #endregion

            return new Exceptional<Vertex>(_Vertex);

        }

        #endregion


        #region EvaluateAttributes

        public Exceptional<ManipulationAttributes> EvaluateAttributes(DBContext myDBContext, GraphDBType myGraphDBType, List<AAttributeAssignOrUpdate> myAttributeAssigns)
        {

            var result = GetRecursiveAttributes(myAttributeAssigns, myDBContext, myGraphDBType);
            
            if (result.Failed())
            {
                return result;
            }

            result.PushIExceptional(AddMandatoryAttributes(myDBContext, myGraphDBType, result.Value));

            return result;

        }

        private Exceptional AddMandatoryAttributes(DBContext myDBContext, GraphDBType myGraphDBType, ManipulationAttributes myManipulationAttributes)
        {

            Boolean mandatoryDefaults = (Boolean)myDBContext.DBSettingsManager.GetSettingValue((new SettingDefaultsOnMandatory()).ID, myDBContext, TypesSettingScope.TYPE, myGraphDBType).Value.Value;
            TypeAttribute typeAttr = null;
            GraphDBType typeOfAttr = null;
            var typeMandatoryAttribs = myGraphDBType.GetMandatoryAttributesUUIDs(myDBContext.DBTypeManager);

            if ((myManipulationAttributes.MandatoryAttributes.Count < typeMandatoryAttribs.Count()) && !mandatoryDefaults)
                return new Exceptional(new Error_MandatoryConstraintViolation(myGraphDBType.Name));

            foreach (var attrib in typeMandatoryAttribs)
            {
                if (!myManipulationAttributes.MandatoryAttributes.Contains(attrib))
                {
                    //if we have mandatory attributes in type but not in the current statement and USE_DEFAULTS_ON_MANDATORY is true then do this
                    if (mandatoryDefaults)
                    {
                        typeAttr = myGraphDBType.GetTypeAttributeByUUID(attrib);

                        if (typeAttr == null)
                            return new Exceptional(new Error_AttributeIsNotDefined(myGraphDBType.Name, typeAttr.Name));

                        typeOfAttr = myDBContext.DBTypeManager.GetTypeByUUID(typeAttr.DBTypeUUID);

                        IObject defaultValue = typeAttr.DefaultValue;

                        if (defaultValue == null)
                            defaultValue = typeAttr.GetDefaultValue(myDBContext);

                        myManipulationAttributes.Attributes.Add(new TypeAndAttributeDefinition(typeAttr, typeOfAttr), defaultValue);
                    }
                    else
                    {
                        return new Exceptional(new Error_MandatoryConstraintViolation("Attribute \"" + myGraphDBType.GetTypeAttributeByUUID(attrib).Name + "\" of Type \"" + myGraphDBType.Name + "\" is mandatory."));
                    }
                }
            }

            return Exceptional.OK;
        }

        /// <summary>
        /// Get attribute assignments for new DBObjects.
        /// </summary>
        /// <param name="myAttributeAssigns">The interesting ParseTreeNode.</param>
        /// <param name="typeManager">The TypeManager of the GraphDB.</param>
        /// <returns>A Dictionary of AttributeAssignments</returns>
        private Exceptional<ManipulationAttributes> GetRecursiveAttributes(List<AAttributeAssignOrUpdate> myAttributeAssigns, DBContext myDBContext, GraphDBType myGraphDBType)
        {

            #region Data

            var manipulationAttributes = new ManipulationAttributes();
            var resultExcept = new Exceptional<ManipulationAttributes>();

            if (myAttributeAssigns == null)
            {                
                return new Exceptional<ManipulationAttributes>(manipulationAttributes);
            }

            TypeAttribute attr;
            ADBBaseObject typedAttributeValue;
            BasicType correspondingCSharpType;
            Warning_UndefinedAttribute undefAttrWarning = null;

            var typeMandatoryAttribs = myGraphDBType.GetMandatoryAttributesUUIDs(myDBContext.DBTypeManager);

            var setExcept = myDBContext.DBSettingsManager.GetSetting(SettingUndefAttrBehaviour.UUID, myDBContext, TypesSettingScope.DB);

            if (!setExcept.Success())
            {
                return new Exceptional<ManipulationAttributes>(setExcept);
            }

            var undefAttrBehave = (SettingUndefAttrBehaviour)setExcept.Value;

            #endregion

            #region get Data
            
            #region proceed list

            foreach (var aAttributeAssign in myAttributeAssigns)
            {

                var validateResult = aAttributeAssign.AttributeIDChain.Validate(myDBContext, true);

                if (validateResult.Failed())
                {
                    return new Exceptional<ManipulationAttributes>(validateResult);
                }
                
                #region Undefined attributes - Refactor and add undefined logic into defined attribute AssignsOrUpdate

                System.Diagnostics.Debug.Assert(aAttributeAssign.AttributeIDChain != null);

                //in this case we have an undefined attribute
                if (aAttributeAssign.AttributeIDChain.IsUndefinedAttribute)
                {                    
                    
                    var UndefAttrName = aAttributeAssign.AttributeIDChain.UndefinedAttribute;

                    switch (undefAttrBehave.Behaviour)
                    {
                        case UndefAttributeBehaviour.disallow:
                            return new Exceptional<ManipulationAttributes>(new Error_UndefinedAttributes());

                        case UndefAttributeBehaviour.warn:
                            if (undefAttrWarning == null)
                            {
                                undefAttrWarning = new Warning_UndefinedAttribute();
                            }

                            resultExcept.PushIWarning(undefAttrWarning);
                            break;
                    }

                    if (aAttributeAssign is AttributeAssignOrUpdateList)
                    {

                        #region AttributeAssignCollection

                        var colDefinition = (aAttributeAssign as AttributeAssignOrUpdateList).CollectionDefinition;
                        EdgeTypeListOfBaseObjects valueList = new EdgeTypeListOfBaseObjects();

                        foreach (var tuple in colDefinition.TupleDefinition)
                        {
                            if (tuple.TypeOfValue == BasicType.Unknown)
                                valueList.Add((tuple.Value as ValueDefinition).Value);
                            else if (tuple.Value is ValueDefinition)
                                valueList.Add((tuple.Value as ValueDefinition).Value);
                            else
                                return new Exceptional<ManipulationAttributes>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        }

                        if (colDefinition.CollectionType == CollectionType.Set)
                            valueList.UnionWith(valueList);

                        manipulationAttributes.UndefinedAttributes.Add(UndefAttrName, valueList);                        
                        var undefAttr = new UndefinedAttributeDefinition() { AttributeName = UndefAttrName, AttributeValue = valueList };
                        var undefAttrAssign = new AttributeAssignOrUpdateUndefined((aAttributeAssign as AttributeAssignOrUpdateList).AttributeIDChain, undefAttr);
                        manipulationAttributes.AttributeToUpdateOrAssign.Add(undefAttrAssign);


                        #endregion
                    }
                    else if (aAttributeAssign is AttributeAssignOrUpdateValue)
                    {

                        #region AttributeAssignValue

                        var value = GraphDBTypeMapper.GetBaseObjectFromCSharpType((aAttributeAssign as AttributeAssignOrUpdateValue).Value);
                        manipulationAttributes.UndefinedAttributes.Add(UndefAttrName, value);
                        var undefAttr = new UndefinedAttributeDefinition() { AttributeName = UndefAttrName, AttributeValue = value };
                        var undefAttrAssign = new AttributeAssignOrUpdateUndefined((aAttributeAssign as AttributeAssignOrUpdateValue).AttributeIDChain, undefAttr);
                        manipulationAttributes.AttributeToUpdateOrAssign.Add(undefAttrAssign);
                        
                        #endregion

                    }
                    else if (aAttributeAssign is AttributeAssignOrUpdateSetRef)
                    {
                        return new Exceptional<ManipulationAttributes>(new Error_InvalidReferenceAssignmentOfUndefAttr());
                    }
                    else
                    {
                        return new Exceptional<ManipulationAttributes>(new Error_InvalidUndefinedAttributeName());
                    }

                    continue;
                }

                #endregion
                
                attr = aAttributeAssign.AttributeIDChain.LastAttribute;

                manipulationAttributes.AttributeToUpdateOrAssign.Add(aAttributeAssign);

                #region checks

                if (aAttributeAssign.AttributeIDChain.LastType != myGraphDBType)
                {
                    return new Exceptional<ManipulationAttributes>(new Error_InvalidAttribute(aAttributeAssign.AttributeIDChain.ToString()));
                }

                if (attr.GetDBType(myDBContext.DBTypeManager).IsBackwardEdge)
                {
                    return new Exceptional<ManipulationAttributes>(new Error_Logic("Adding values to BackwardEdges Attributes are not allowed! (" + attr.Name + ") is an BackwardEdge Attribute of GraphType \"" + myGraphDBType.Name + "\"."));
                }

                #endregion

                #region SpecialTypeAttribute

                if (attr is ASpecialTypeAttribute)
                {

                    if (!(aAttributeAssign is AttributeAssignOrUpdateValue))
                    {
                        return new Exceptional<ManipulationAttributes>(new Error_InvalidAttributeValue((attr as ASpecialTypeAttribute).Name, aAttributeAssign.ToString()));
                    }

                    manipulationAttributes.SpecialTypeAttributes.Add((attr as ASpecialTypeAttribute), (aAttributeAssign as AttributeAssignOrUpdateValue).Value);

                    continue;

                }

                #endregion

                #region check & add

                if (aAttributeAssign.AttributeIDChain.Edges.Count > 1)
                {
                    //in case of those statements: INSERT INTO Flower VALUES (Colors.Name = 'red')
                    //Colors.Name is an IDNode with 2 Edges. This is not possible.

                    return new Exceptional<ManipulationAttributes>(new Error_Logic("Invalid attribute assignment : " + aAttributeAssign.AttributeIDChain.ToString() + " = " + aAttributeAssign));
                }

                if (aAttributeAssign is AttributeAssignOrUpdateList)
                {

                    #region SetOfDBObjects

                    #region Check, whether this is valid for SetOfDBObjects

                    if (attr.KindOfType != KindsOfType.SetOfReferences)
                    {
                        if (attr.KindOfType != KindsOfType.ListOfNoneReferences)
                        {
                            if (attr.KindOfType != KindsOfType.SetOfNoneReferences)
                            {
                                return new Exceptional<ManipulationAttributes>(new Error_ReferenceAssignmentExpected(attr));//"Please use SETREF keyword instead of REF/REFERENCE or LISTOF."));
                            }
                        }
                    }

                    #endregion

                    #region list stuff

                    #region process tuple

                    #region process as list

                    var collectionDefinition = ((AttributeAssignOrUpdateList)aAttributeAssign).CollectionDefinition;
                    var dbType = attr.GetDBType(myDBContext.DBTypeManager);

                    if (dbType.IsUserDefined)
                    {

                        #region List of references

                        var uuids = collectionDefinition.GetEdge(attr, dbType, myDBContext);
                        if (uuids.Failed())
                        {
                            return new Exceptional<ManipulationAttributes>(uuids);
                        }

                        manipulationAttributes.Attributes.Add(new TypeAndAttributeDefinition(attr, dbType), uuids.Value);

                        if (typeMandatoryAttribs.Contains(attr.UUID))
                        {
                            manipulationAttributes.MandatoryAttributes.Add(attr.UUID);
                        }

                        #endregion

                    }
                    else
                    {

                        #region List of ADBBaseObjects (Integer, String, etc)

                        var edge = (aAttributeAssign as AttributeAssignOrUpdateList).GetBasicList(myDBContext);
                        if (edge.Failed())
                        {
                            return new Exceptional<ManipulationAttributes>(edge);
                        }

                        // If the collection was declared as a SETOF insert
                        if (collectionDefinition.CollectionType == CollectionType.Set)
                            edge.Value.Distinction();

                        manipulationAttributes.Attributes.Add(new TypeAndAttributeDefinition(attr, attr.GetDBType(myDBContext.DBTypeManager)), edge.Value);

                        if (typeMandatoryAttribs.Contains(attr.UUID))
                        {
                            manipulationAttributes.MandatoryAttributes.Add(attr.UUID);
                        }

                        #endregion

                    }

                    #endregion

                    #endregion

                    #endregion

                    #endregion

                }
                else if (aAttributeAssign is AttributeAssignOrUpdateSetRef)
                {

                    #region reference

                    var aSetRefNode = ((AttributeAssignOrUpdateSetRef)aAttributeAssign).SetRefDefinition;

                    var singleedge = aSetRefNode.GetEdge(attr, myDBContext, attr.GetRelatedType(myDBContext.DBTypeManager));
                    if (singleedge.Failed())
                    {
                        return new Exceptional<ManipulationAttributes>(singleedge);
                    }

                    if (attr.GetRelatedType(myDBContext.DBTypeManager).IsUserDefined)
                    {
                        //a list which carries elements of userdefined types consists of GUIDS
                        manipulationAttributes.Attributes.Add(new TypeAndAttributeDefinition(attr, attr.GetDBType(myDBContext.DBTypeManager)), singleedge.Value);

                        if (typeMandatoryAttribs.Contains(attr.UUID))
                        {
                            manipulationAttributes.MandatoryAttributes.Add(attr.UUID);
                        }

                    }
                    else
                    {
                        return new Exceptional<ManipulationAttributes>(new Error_UnknownDBError("Reference types cannot be basic types."));
                    }

                    #endregion

                }
                else if (aAttributeAssign is AttributeAssignOrUpdateExpression)
                {

                    #region Expression

                    (aAttributeAssign as AttributeAssignOrUpdateExpression).BinaryExpressionDefinition.Validate(myDBContext);
                    var value = (aAttributeAssign as AttributeAssignOrUpdateExpression).BinaryExpressionDefinition.ResultValue;

                    if (value.Failed())
                    {
                        return new Exceptional<ManipulationAttributes>(value.IErrors.First());
                    }


                    if (value.Value is ValueDefinition)
                    {

                        #region AtomValue

                        if (attr.KindOfType == KindsOfType.SetOfReferences || attr.KindOfType == KindsOfType.ListOfNoneReferences || attr.KindOfType == KindsOfType.SetOfNoneReferences)
                            return new Exceptional<ManipulationAttributes>(new Error_InvalidTuple(aAttributeAssign.ToString()));

                        if (!(value.Value as ValueDefinition).IsDefined)
                        {
                            (value.Value as ValueDefinition).ChangeType(attr.GetDBType(myDBContext.DBTypeManager).UUID);
                        }
                        var val = (value.Value as ValueDefinition).Value.Value;

                        correspondingCSharpType = GraphDBTypeMapper.ConvertGraph2CSharp(attr, attr.GetDBType(myDBContext.DBTypeManager));
                        if (GraphDBTypeMapper.GetEmptyGraphObjectFromType(correspondingCSharpType).IsValidValue(val))
                        {
                            typedAttributeValue = GraphDBTypeMapper.GetGraphObjectFromType(correspondingCSharpType, val);
                            manipulationAttributes.Attributes.Add(new TypeAndAttributeDefinition(attr, attr.GetDBType(myDBContext.DBTypeManager)), typedAttributeValue);

                            if (typeMandatoryAttribs.Contains(attr.UUID))
                                manipulationAttributes.MandatoryAttributes.Add(attr.UUID);
                        }
                        else
                        {
                            return new Exceptional<ManipulationAttributes>(new Error_InvalidAttributeValue(attr.Name, (aAttributeAssign as AttributeAssignOrUpdateValue).Value));
                        }

                        #endregion

                    }
                    else // TupleValue!
                    {

                        #region TupleValue

                        if (attr.KindOfType != KindsOfType.SetOfReferences)
                            return new Exceptional<ManipulationAttributes>(new Error_InvalidTuple(aAttributeAssign.ToString()));

                        if (!(attr.EdgeType is IBaseEdge))
                            return new Exceptional<ManipulationAttributes>(new Error_InvalidEdgeType(attr.EdgeType.GetType(), typeof(IBaseEdge)));

                        correspondingCSharpType = GraphDBTypeMapper.ConvertGraph2CSharp(attr, attr.GetDBType(myDBContext.DBTypeManager));

                        if ((value.Value as TupleDefinition).TypeOfOperatorResult != correspondingCSharpType)
                            return new Exceptional<ManipulationAttributes>(new Error_DataTypeDoesNotMatch(correspondingCSharpType.ToString(), (value.Value as TupleDefinition).TypeOfOperatorResult.ToString()));

                        var edge = attr.EdgeType.GetNewInstance() as IBaseEdge;
                        edge.AddRange((value.Value as TupleDefinition).Select(te => (te.Value as ValueDefinition).Value));
                        manipulationAttributes.Attributes.Add(new TypeAndAttributeDefinition(attr, attr.GetDBType(myDBContext.DBTypeManager)), edge as IBaseEdge);

                        #endregion

                    }

                    #endregion

                }
                else if (aAttributeAssign is AttributeAssignOrUpdateValue)
                {

                    #region Simple value

                    var attrVal = aAttributeAssign as AttributeAssignOrUpdateValue;

                    if (attr.KindOfType == KindsOfType.ListOfNoneReferences)
                        return new Exceptional<ManipulationAttributes>(new Error_InvalidTuple(attrVal.ToString()));

                    correspondingCSharpType = GraphDBTypeMapper.ConvertGraph2CSharp(attr, attr.GetDBType(myDBContext.DBTypeManager));
                    if (GraphDBTypeMapper.GetEmptyGraphObjectFromType(correspondingCSharpType).IsValidValue(attrVal.Value))
                    {
                        typedAttributeValue = GraphDBTypeMapper.GetGraphObjectFromType(correspondingCSharpType, attrVal.Value);
                        manipulationAttributes.Attributes.Add(new TypeAndAttributeDefinition(attr, attr.GetDBType(myDBContext.DBTypeManager)), typedAttributeValue);

                        if (typeMandatoryAttribs.Contains(attr.UUID))
                            manipulationAttributes.MandatoryAttributes.Add(attr.UUID);
                    }
                    else
                    {
                        return new Exceptional<ManipulationAttributes>(new Error_InvalidAttributeValue(attr.Name, attrVal.Value));
                    }


                    #endregion

                }
                else
                {
                    return new Exceptional<ManipulationAttributes>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                #endregion

            }

            #endregion

            #endregion

            resultExcept.Value = manipulationAttributes;

            return resultExcept;
        }

        #endregion

        #region Truncate

        public Exceptional Truncate(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            #region Remove

            #region remove dbobjects

            var listOfAffectedDBObjects = myDBContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(myGraphDBType, myDBContext.DBTypeManager), myDBContext).ToList();

            foreach (var aDBO in listOfAffectedDBObjects)
            {

                if (!aDBO.Success())
                {
                    return new Exceptional(aDBO);
                }

                var dbObjID = aDBO.Value.ObjectUUID;

                var dbBackwardEdgeLoadExcept = myDBContext.DBObjectManager.LoadBackwardEdge(aDBO.Value.ObjectLocation);

                if (!dbBackwardEdgeLoadExcept.Success())
                    return new Exceptional(dbBackwardEdgeLoadExcept);

                var dbBackwardEdges = dbBackwardEdgeLoadExcept.Value;

                var result = myDBContext.DBObjectManager.RemoveDBObject(myGraphDBType, aDBO.Value, myDBContext.DBObjectCache, myDBContext.SessionSettings);

                if (!result.Success())
                {
                    return new Exceptional(result);
                }

                if (dbBackwardEdges != null)
                {
                    var deleteReferences = DeleteObjectReferences(dbObjID, dbBackwardEdges, myDBContext);

                    if (!deleteReferences.Success())
                        return new Exceptional(deleteReferences);
                }

            }

            #endregion

            #region remove indices

            Parallel.ForEach(myGraphDBType.GetAllAttributeIndices(false), aIdx =>
                {
                    //it is not necessary to clear the UUID IDX because this is done by deleting the objects in the fs
                    aIdx.ClearAndRemoveFromDisc(myDBContext.DBIndexManager);
                });

            #endregion

            #endregion

            return Exceptional.OK;

        }

        #endregion
        
    }
}
