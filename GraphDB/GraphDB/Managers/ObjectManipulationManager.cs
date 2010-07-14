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


/* <id name="GraphDB – ObjectManipulationManager" />
 * <copyright file="ObjectManipulationManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class will handle all manipulations on DBObjects coming from Irony statement nodes like InsertNode, UpdateNode, DeleteNode, etc.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements;
using sones.GraphDB.QueryLanguage.Operators;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.Warnings;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.Session;
using sones.Lib;
using sones.Lib.Frameworks.Irony.Parsing;
using System.Threading.Tasks;
using sones.GraphDB.Settings;

#endregion

namespace sones.GraphDB.Managers
{
    /// <summary>
    /// This class will handle all manipulations on DBObjects coming from Irony statement nodes like InsertNode, UpdateNode, DeleteNode, etc
    /// </summary>
    public class ObjectManipulationManager
    {

        GraphDBType _Type;
        public GraphDBType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        AStatement _CorrespondingStatement;
        //DBContext _DBContext;
        //SessionSettings _SessionInfos;
        
        /// <summary>
        /// dictionary of attribute assignments
        /// </summary>
        private Dictionary<AttributeDefinition, AObject> _Attributes = null;
        public Dictionary<AttributeDefinition, AObject> Attributes
        {
            get { return _Attributes; }
        }

        /// <summary>
        /// mandatory attributes in the current insert statement
        /// </summary>
        protected HashSet<AttributeUUID> _MandatoryStmtAttr = null;

        protected HashSet<AttributeAssignNode> _AssignNodeList = null;
        
        public HashSet<AttributeAssignNode> AssignNodeList
        { 
            get { return _AssignNodeList; } 
        }

        protected Dictionary<ASpecialTypeAttribute, Object> _SpecialTypeAttributes;

        protected Dictionary<String, AObject> _UndefinedAttributes = null;

        public Dictionary<String, AObject> UndefinedAttributes
        {
            get { return _UndefinedAttributes; }
        }

        /// <summary>
        /// mandatory attributes of the type
        /// </summary>
        private IEnumerable<AttributeUUID> _TypeMandatoryAttribs = null;
        public IEnumerable<AttributeUUID> TypeMandatoryAttribs
        {
            get { return _TypeMandatoryAttribs; }
        }

        public ObjectManipulationManager(SessionSettings sessionInfos, GraphDBType type, DBContext dbContext, AStatement correspondingStatement)
        {
            _Type = type;
            if (_Type != null)
            {
                _TypeMandatoryAttribs = _Type.GetMandatoryAttributesUUIDs(dbContext.DBTypeManager);
            }
            //_DBContext = dbContext;
            _CorrespondingStatement = correspondingStatement;
            //_SessionInfos = sessionInfos;
            _Attributes = new Dictionary<AttributeDefinition, AObject>();
        }

        #region update methods        

        /// <summary>
        /// This will evaluate the <paramref name="whereExpression"/> and add some warnings coming from 
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="listOfUpdates"></param>
        /// <param name="undefAttributeNodes"></param>
        /// <param name="dbObjectCache"></param>
        /// <returns></returns>
        public QueryResult Update(BinaryExpressionNode whereExpression, IEnumerable<AttributeUpdateOrAssign> listOfUpdates, DBContext dbContext)
        {
            IEnumerable<Exceptional<DBObjectStream>> _dbobjects;
            IEnumerable<IWarning> warnings = null;
       
            #region get GUIDs

            if (whereExpression != null)
            {
                #region get guids via where

                var _tempGraphResult = whereExpression.Calculon(dbContext, new CommonUsageGraph(dbContext), false);

                if (_tempGraphResult.Failed)
                {
                    throw new GraphDBException(_tempGraphResult.Errors.First());
                }
                else
                {
                    _dbobjects = _tempGraphResult.Value.Select(new LevelKey(_Type), null, true);
                    if (!_tempGraphResult.Success)
                    {
                        warnings = _tempGraphResult.Warnings;
                    }
                }
                
                #endregion
            }
            else
            {
                #region get guids via guid-idx

                _dbobjects = dbContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(_Type), dbContext);

                #endregion
            }

            #endregion

            var updateResult = Update(_dbobjects, listOfUpdates, dbContext);

            #region expressionGraph error handling

            if (!warnings.IsNullOrEmpty())
            {
                updateResult.AddWarnings(warnings);
            }

            #endregion

            return updateResult;

        }

        /// <summary>
        /// This is the update method which will change some <paramref name="dbObjects"/> an behalf of the <paramref name="listOfUpdates"/>
        /// </summary>
        /// <param name="dbObjects">Some dbobjects</param>
        /// <param name="listOfUpdates">The list of update tasks (assign, delete, etc)</param>
        /// <param name="dbObjectCache"></param>
        /// <returns></returns>
        public QueryResult Update(IEnumerable<Exceptional<DBObjectStream>> dbObjects, IEnumerable<AttributeUpdateOrAssign> listOfUpdates, DBContext dbContext)
        {

            #region Data

            var queryResultContent = new List<DBObjectReadout>();

            #endregion

            #region check unique constraint
            Exceptional<Boolean> CheckConstraint = null;

            IEnumerable<GraphDBType> parentTypes = dbContext.DBTypeManager.GetAllParentTypes(_Type, true, false);
            Dictionary<AttributeUUID, AObject> ChkForUnique = new Dictionary<AttributeUUID, AObject>();

            foreach (var entry in dbObjects)
            {
                //all attributes, that are going to be changed
                var attrsToCheck = entry.Value.GetAttributes().Where(item => listOfUpdates.Select(updAttrs => GetAttributesToCheckForUnique(updAttrs)).Contains(item.Key));

                foreach (var attrValue in attrsToCheck)
                {
                    if (!ChkForUnique.ContainsKey(attrValue.Key))
                        ChkForUnique.Add(attrValue.Key, attrValue.Value);
                }

                CheckConstraint = dbContext.DBIndexManager.CheckUniqueConstraint(_Type, parentTypes, ChkForUnique);
                ChkForUnique.Clear();

                if (CheckConstraint.Failed)
                    return new QueryResult(CheckConstraint.Errors);

            }

            #endregion

            #region data

            String UndefName = String.Empty;
            var specialAttributeUUID = new SpecialTypeAttribute_UUID();
            var specialAttributeRev = new SpecialTypeAttribute_REVISION();

            #endregion

            #region regular update

            foreach (var aDBO in dbObjects)
            {
                //key: attribute name
                //value: TypeAttribute, NewValue
                Dictionary<String, Tuple<TypeAttribute, AObject>> attrsForResult = new Dictionary<String, Tuple<TypeAttribute, AObject>>();

                if (aDBO.Failed)
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                #region data

                Boolean sthChanged = false;

                #endregion
                
                #region iterate tasks

                //Exceptional<Boolean> partialResult;

                foreach (AttributeUpdateOrAssign aPartialTask in listOfUpdates)
                {
                    switch (aPartialTask.TypeOfUpdate)
                    {
                        case TypesOfUpdate.AssignAttribute:

                            #region AssignAttribute

                            if (aPartialTask.IsUndefinedAttribute)
                            {
                                #region undefined attributes

                                //TODO: change this to a more handling thing than KeyValuePair
                                var keyVal = (KeyValuePair<String, AObject>)(aPartialTask.Value);
                                var addExcept = dbContext.DBObjectManager.AddUndefinedAttribute(keyVal.Key, keyVal.Value, aDBO.Value);

                                if (addExcept.Failed)
                                    return new QueryResult(addExcept.Errors);

                                sthChanged = true;

                                attrsForResult.Add(keyVal.Key, new Tuple<TypeAttribute, AObject>(null, keyVal.Value));

                                #endregion
                            }
                            else
                            {
                                #region Usual attribute

                                var applyResult = ApplyAssignAttribute(aPartialTask.Value as AttributeAssignNode, dbContext, aDBO.Value);

                                if (applyResult.Failed)
                                {
                                    return new QueryResult(applyResult.Errors);
                                }

                                if (applyResult.Value != null)
                                {
                                    sthChanged = true;

                                    #region Add to queryResult

                                    attrsForResult.Add(applyResult.Value.Item1, new Tuple<TypeAttribute, AObject>(applyResult.Value.Item2, applyResult.Value.Item3));

                                    #endregion
                                }

                                #endregion
                            }

                            #endregion

                            break;
                        case TypesOfUpdate.RemoveAttribute:

                            #region divide remove list in undefined and defined attributes

                            var attrsToRemove = ((AttrRemoveNode)aPartialTask.Value).ToBeRemovedAttributes;
                            
                            var undefAttrsExcept = aDBO.Value.GetUndefinedAttributes(dbContext.DBObjectManager);

                            if(undefAttrsExcept.Failed)
                                return new QueryResult(undefAttrsExcept.Errors);

                            var undefAttrsOfObject = undefAttrsExcept.Value;

                            var undefAttrsToRemove = attrsToRemove.Where(item => undefAttrsOfObject.ContainsKey(item)).ToList();

                            List<String> defAttrsToRemove = new List<String>();
                            List<String> unknowAttrs = new List<String>();

                            foreach (var item in attrsToRemove.Where(item => !undefAttrsOfObject.ContainsKey(item)).ToList())
                            {
                                if (_Type.GetTypeAttributeByName(item) != null)
                                    defAttrsToRemove.Add(item);
                                else
                                    unknowAttrs.Add(item);
                            }

                            #endregion

                            #region remove undefined attributes

                            if (!unknowAttrs.IsNullOrEmpty())
                            {   
                                return new QueryResult(new Error_InvalidUndefinedAttributes(unknowAttrs));
                            }

                            foreach (var aAttribute in undefAttrsToRemove)
                            {
                                var removeExcept = dbContext.DBObjectManager.RemoveUndefinedAttribute(aAttribute, aDBO.Value);

                                if (removeExcept.Failed)
                                    return new QueryResult(removeExcept.Errors);

                                attrsForResult.Add(aAttribute, new Tuple<TypeAttribute, AObject>(null, null));
                            }

                            if(!undefAttrsToRemove.IsNullOrEmpty())
                                sthChanged = true;

                            #endregion

                            #region RemoveAttribute

                            var applyRemoveResult = ApplyRemoveAttribute(defAttrsToRemove, dbContext, aDBO.Value);

                            if (applyRemoveResult.Failed)
                            {
                                return new QueryResult(applyRemoveResult.Errors);
                            }

                            if (applyRemoveResult.Value.Count > 0)
                            {
                                sthChanged = true;

                                #region Add to queryResult

                                foreach (var attr in applyRemoveResult.Value)
                                {
                                    attrsForResult.Add(attr.Name, new Tuple<TypeAttribute, AObject>(attr, null));
                                }

                                #endregion

                            }

                            #endregion

                            break;
                        case TypesOfUpdate.UpdateListAttribute:

                            #region UpdateListAttribute

                            var applyUpdateListResult = ApplyUpdateListAttribute(aPartialTask.Value, dbContext, aDBO.Value);

                            if (applyUpdateListResult.Failed)
                            {
                                return new QueryResult(applyUpdateListResult.Errors);
                            }

                            if (applyUpdateListResult.Value != null)
                            {
                                sthChanged = true;

                                #region Add to QueryResult

                                attrsForResult.Add(applyUpdateListResult.Value.Item1, new Tuple<TypeAttribute, AObject>(applyUpdateListResult.Value.Item2, applyUpdateListResult.Value.Item3));

                                #endregion

                            }

                            #endregion

                            break;

                        default:
                            return new QueryResult(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                    }
                }

                #endregion
                
                if (sthChanged)
                {
                    var definedAttributes = ExtractDefinedAttributes(attrsForResult, dbContext.DBTypeManager);

                    #region update Idx

                    foreach (var _AttributeIndex in _Type.GetAllAttributeIndices())
                    {
                        if(definedAttributes.Exists(item => _AttributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs.Contains(item.Key.Definition.UUID)))
                        {
                            //execute update
                            _AttributeIndex.Update(aDBO.Value, _Type, dbContext);
                        }
                    }


                    #endregion

                    #region update dbobjects on fs

                    var flushResult = dbContext.DBObjectManager.FlushDBObject(aDBO.Value);
                    if (!flushResult.Success)
                    {
                        return new QueryResult(flushResult);
                    }

                    #endregion

                    var resultSet = GetManipulationResultSet(dbContext, undefinedAttributes: ExtractUndefindedAttributes(attrsForResult), attributes: definedAttributes, dbObjectStream: aDBO);
                    if (!resultSet.Success)
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

            return new QueryResult(new List<SelectionResultSet> { new SelectionResultSet(_Type, queryResultContent, new Dictionary<String, String>()) });

            #endregion
        }

        private Dictionary<AttributeDefinition, AObject> ExtractDefinedAttributes(Dictionary<string, Tuple<TypeAttribute, AObject>> attrsForResult, DBTypeManager myTypeManager)
        {
            return attrsForResult.Where(item => item.Value.Item1 != null).ToDictionary(key => new AttributeDefinition(key.Value.Item1, key.Value.Item1.GetDBType(myTypeManager)), value => value.Value.Item2);
        }

        private Dictionary<string, AObject> ExtractUndefindedAttributes(Dictionary<string, Tuple<TypeAttribute, AObject>> attrsForResult)
        {
            return attrsForResult.Where(item => item.Value.Item1 == null).ToDictionary(key => key.Key, value => value.Value.Item2);
        }

        #region AttributesToCheckForUnique

        public AttributeUUID GetAttributesToCheckForUnique(AttributeUpdateOrAssign myAssignNode)
        {
            if (myAssignNode.IsUndefinedAttribute)
                return null;

            if (myAssignNode.Value is AttributeAssignNode)
                return ((AttributeAssignNode)myAssignNode.Value).AttributeIDNode.LastAttribute.UUID;

            if (myAssignNode.Value is AddToListAttrUpdateNode)
                return ((AddToListAttrUpdateNode)myAssignNode.Value).Attribute.UUID;

            if (myAssignNode.Value is RemoveFromListAttrUpdateNode || myAssignNode.Value is AttrRemoveNode)
                return null;

            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        #endregion

        #region load undefined attributes

        private Exceptional<IDictionary<String, AObject>> LoadUndefAttributes(String myName, DBContext dbContext, DBObjectStream myObjStream)
        {
            var loadExcept = myObjStream.GetUndefinedAttributes(dbContext.DBObjectManager);

            if (loadExcept.Failed)
                return new Exceptional<IDictionary<string, AObject>>(loadExcept);

            return new Exceptional<IDictionary<string, AObject>>(loadExcept.Value);
        }

        #endregion


        public Exceptional<Tuple<String, TypeAttribute, AListEdgeType>> ApplyUpdateListAttribute(object aTask, DBContext dbContext, DBObjectStream aDBObject)
        {
            if (aTask is AddToListAttrUpdateNode)
            {
                #region data

                AddToListAttrUpdateNode aAddToListAttrUpdateNode = (AddToListAttrUpdateNode)aTask;
                TypeAttribute attrDef = aAddToListAttrUpdateNode.Attribute;
                AListEdgeType elementsToBeAdded;
                EdgeTypeListOfBaseObjects undefAttrList;

                #endregion

                #region undefined attributes

                if (aAddToListAttrUpdateNode.Attribute == null)
                {
                    var loadExcept = LoadUndefAttributes(aAddToListAttrUpdateNode.AttributeName, dbContext, aDBObject);

                    if (loadExcept.Failed)
                        return new Exceptional<Tuple<string, TypeAttribute, AListEdgeType>>(loadExcept);

                    if (!loadExcept.Value.ContainsKey(aAddToListAttrUpdateNode.AttributeName))
                    {
                        var addExcept = dbContext.DBObjectManager.AddUndefinedAttribute(aAddToListAttrUpdateNode.AttributeName, new EdgeTypeListOfBaseObjects(), aDBObject);

                        if (addExcept.Failed)
                            return new Exceptional<Tuple<string, TypeAttribute, AListEdgeType>>(addExcept);
                    }
                    
                    if (!(loadExcept.Value[aAddToListAttrUpdateNode.AttributeName] is EdgeTypeListOfBaseObjects))
                        return new Exceptional<Tuple<string, TypeAttribute, AListEdgeType>>(new Error_InvalidAttributeKind());

                    undefAttrList = (EdgeTypeListOfBaseObjects)loadExcept.Value[aAddToListAttrUpdateNode.AttributeName];

                    elementsToBeAdded = (AListBaseEdgeType)undefAttrList.GetNewInstance();

                    foreach (var tuple in aAddToListAttrUpdateNode.ElementsToBeAdded.TupleNodeElement.Tuple)
                    {
                        if (tuple.TypeOfValue == TypesOfOperatorResult.Unknown)
                        {
                            ADBBaseObject elemToAdd = GraphDBTypeMapper.GetBaseObjectFromCSharpType(tuple.Value);
                            ((EdgeTypeListOfBaseObjects)elementsToBeAdded).Add(elemToAdd);
                            undefAttrList.Add(elemToAdd);
                        }
                        else
                        {
                            return new Exceptional<Tuple<string, TypeAttribute, AListEdgeType>>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        }
                    }

                    if (aAddToListAttrUpdateNode.ElementsToBeAdded.CollectionType == CollectionType.Set)
                        undefAttrList.UnionWith(undefAttrList);

                    var addUndefExcept = dbContext.DBObjectManager.AddUndefinedAttribute(aAddToListAttrUpdateNode.AttributeName, undefAttrList, aDBObject);

                    if (addUndefExcept.Failed)
                        return new Exceptional<Tuple<string, TypeAttribute, AListEdgeType>>(addUndefExcept);

                    return new Exceptional<Tuple<string, TypeAttribute, AListEdgeType>>(new Tuple<string, TypeAttribute, AListEdgeType>(aAddToListAttrUpdateNode.AttributeName, null, elementsToBeAdded));
                }
                #endregion

                #region Validation that this is really a list

                if (attrDef.KindOfType == KindsOfType.SetOfReferences && aAddToListAttrUpdateNode.ElementsToBeAdded.CollectionType == CollectionType.List)
                {
                    return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(new Error_InvalidAssignOfSet(attrDef.Name));
                }

                #endregion

                if (attrDef.KindOfType == KindsOfType.ListOfNoneReferences)
                {
                    #region ListOfNoneReferences

                    elementsToBeAdded = ((AListBaseEdgeType)aAddToListAttrUpdateNode.Attribute.EdgeType.GetNewInstance());
                    foreach (var tupleElem in aAddToListAttrUpdateNode.ElementsToBeAdded.TupleNodeElement.Tuple)
                    {
                        (elementsToBeAdded as AListBaseEdgeType).Add(GraphDBTypeMapper.GetPandoraObjectFromTypeName(attrDef.GetDBType(dbContext.DBTypeManager).Name, tupleElem.Value), tupleElem.Parameters.ToArray());
                    }

                    #endregion
                }
                else
                {
                    #region References

                    if (aAddToListAttrUpdateNode.ElementsToBeAdded.CollectionType == CollectionType.SetOfUUIDs)
                    {
                        var result = aAddToListAttrUpdateNode.ElementsToBeAdded.TupleNodeElement.GetAsUUIDEdge(dbContext, attrDef);
                        if (result.Failed)
                        {
                            return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(result);
                        }
                        elementsToBeAdded = (AListEdgeType)result.Value;
                    }
                    else
                    {

                        var result = aAddToListAttrUpdateNode.GetCorrespondigDBObjectGuidAsList(_Type, dbContext, aAddToListAttrUpdateNode.ElementsToBeAdded.TupleNodeElement, aAddToListAttrUpdateNode.Attribute.EdgeType.GetNewInstance(), aAddToListAttrUpdateNode.Attribute.GetDBType(dbContext.DBTypeManager));
                        if (result.Failed)
                        {
                            return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(result);
                        }
                        elementsToBeAdded = (AListEdgeType)result.Value;
                    }

                    #endregion

                }

                #region add elements

                if (elementsToBeAdded == null)
                {
                    return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(new Error_UpdateAttributeNoElements(aAddToListAttrUpdateNode.Attribute));
                }

                if (aDBObject.HasAttribute(aAddToListAttrUpdateNode.Attribute.UUID, attrDef.GetRelatedType(dbContext.DBTypeManager), dbContext.SessionSettings))
                {
                    ((AListEdgeType)aDBObject.GetAttribute(aAddToListAttrUpdateNode.Attribute.UUID)).UnionWith(elementsToBeAdded);
                }
                else
                {
                    aDBObject.AddAttribute(aAddToListAttrUpdateNode.Attribute.UUID, elementsToBeAdded);
                }

                #region add backward edges

                if (aAddToListAttrUpdateNode.Attribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                {
                    Dictionary<AttributeUUID, AObject> userdefinedAttributes = new Dictionary<AttributeUUID, AObject>();
                    userdefinedAttributes.Add(aAddToListAttrUpdateNode.Attribute.UUID, elementsToBeAdded);                   

                    var setBackEdgesExcept = SetBackwardEdges(_Type, userdefinedAttributes, aDBObject.ObjectUUID, dbContext);

                    if (setBackEdgesExcept.Failed)
                        return new Exceptional<Tuple<string, TypeAttribute, AListEdgeType>>(setBackEdgesExcept);
                }

                #endregion

                return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(new Tuple<String, TypeAttribute, AListEdgeType>(aAddToListAttrUpdateNode.Attribute.Name, aAddToListAttrUpdateNode.Attribute, elementsToBeAdded));

                #endregion
            }
            else
            {
                if (aTask is RemoveFromListAttrUpdateNode)
                {
                    #region data

                    RemoveFromListAttrUpdateNode aRemoveFromListAttrUpdateNode = (RemoveFromListAttrUpdateNode)aTask;
                    AListEdgeType _elementsToBeRemoved;
                    EdgeTypeListOfBaseObjects undefAttrList;

                    #endregion

                    #region undefined attributes

                    if (aRemoveFromListAttrUpdateNode.Attribute == null)
                    {
                        var loadExcept = LoadUndefAttributes(aRemoveFromListAttrUpdateNode.AttributeName, dbContext, aDBObject);

                        if (loadExcept.Failed)
                            return new Exceptional<Tuple<string, TypeAttribute, AListEdgeType>>(loadExcept);

                        if (!loadExcept.Value.ContainsKey(aRemoveFromListAttrUpdateNode.AttributeName))
                            return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(new Tuple<String, TypeAttribute, AListEdgeType>(aRemoveFromListAttrUpdateNode.AttributeName, null, null));
                        
                        if (!(loadExcept.Value[aRemoveFromListAttrUpdateNode.AttributeName] is EdgeTypeListOfBaseObjects))
                            return new Exceptional<Tuple<string, TypeAttribute, AListEdgeType>>(new Error_InvalidAttributeKind());

                        undefAttrList = (EdgeTypeListOfBaseObjects)loadExcept.Value[aRemoveFromListAttrUpdateNode.AttributeName];

                        foreach (var tuple in aRemoveFromListAttrUpdateNode.TupleNode.Tuple)
                        {
                            undefAttrList.Remove(GraphDBTypeMapper.GetBaseObjectFromCSharpType(tuple.Value));
                        }

                        dbContext.DBObjectManager.AddUndefinedAttribute(aRemoveFromListAttrUpdateNode.AttributeName, undefAttrList, aDBObject);

                        return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(new Tuple<String, TypeAttribute, AListEdgeType>(aRemoveFromListAttrUpdateNode.AttributeName, null, null));
                    }

                    #endregion

                    #region get elements

                    try
                    {
                        _elementsToBeRemoved = (AListEdgeType)(aRemoveFromListAttrUpdateNode.GetCorrespondigDBObjectGuidAsList(_Type, dbContext, aRemoveFromListAttrUpdateNode.TupleNode, aRemoveFromListAttrUpdateNode.Attribute.EdgeType.GetNewInstance(), aRemoveFromListAttrUpdateNode.Attribute.GetDBType(dbContext.DBTypeManager)).Value);
                    }
                    catch (Exception e)
                    {
                        return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(new Error_UnknownDBError(e));
                    }

                    #endregion

                    #region remove elements from list

                    if (_elementsToBeRemoved == null)
                    {
                        return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(new Error_UpdateAttributeNoElements(aRemoveFromListAttrUpdateNode.Attribute));
                    }

                    if (aDBObject.HasAttribute(aRemoveFromListAttrUpdateNode.Attribute.UUID, aRemoveFromListAttrUpdateNode.Attribute.GetRelatedType(dbContext.DBTypeManager), dbContext.SessionSettings))
                    {
                        ASetReferenceEdgeType edge = (ASetReferenceEdgeType)aDBObject.GetAttribute(aRemoveFromListAttrUpdateNode.Attribute.UUID);

                        edge.RemoveWhere(item => (_elementsToBeRemoved as ASetReferenceEdgeType).Contains(item));

                        #region remove backward edges

                        if (aRemoveFromListAttrUpdateNode.Attribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                        {
                            Dictionary<AttributeUUID, object> userdefinedAttributes = new Dictionary<AttributeUUID, object>();
                            userdefinedAttributes.Add(aRemoveFromListAttrUpdateNode.Attribute.UUID, _elementsToBeRemoved);

                            RemoveBackwardEdges(_Type.UUID, userdefinedAttributes, aDBObject.ObjectUUID, dbContext);
                        }

                        #endregion

                        return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(new Tuple<String, TypeAttribute, AListEdgeType>(aRemoveFromListAttrUpdateNode.Attribute.Name, aRemoveFromListAttrUpdateNode.Attribute, edge));
                    }

                    #endregion
                }
                else
                {
                    return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }
            }

            return new Exceptional<Tuple<String, TypeAttribute, AListEdgeType>>(null as Tuple<String, TypeAttribute, AListEdgeType>);
        }

        public Exceptional<List<TypeAttribute>> ApplyRemoveAttribute(List<string> attrsToRemoved, DBContext dbContext, DBObjectStream aDBObject)
        {
            #region data

            List<TypeAttribute> removedAttributes = new List<TypeAttribute>();

            #endregion

            var MandatoryTypeAttrib = _Type.GetMandatoryAttributesUUIDs(dbContext.DBTypeManager);
            foreach (String aAttribute in attrsToRemoved)
            {
                TypeAttribute typeAttribute = _Type.GetTypeAttributeByName(aAttribute);

                if (aDBObject.HasAttribute(typeAttribute.UUID, _Type, null))
                {
                    if (!MandatoryTypeAttrib.Contains(typeAttribute.UUID))
                    {
                        #region remove backward edges

                        if (typeAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                        {
                            var userdefinedAttributes = new Dictionary<AttributeUUID, object>();
                            userdefinedAttributes.Add(typeAttribute.UUID, aDBObject.GetAttribute(typeAttribute.UUID));

                            RemoveBackwardEdges(_Type.UUID, userdefinedAttributes, aDBObject.ObjectUUID, dbContext);
                        }

                        #endregion

                        aDBObject.RemoveAttribute(typeAttribute.UUID);
                        removedAttributes.Add(typeAttribute);
                    }
                    else
                    {
                        return new Exceptional<List<TypeAttribute>>(new Error_MandatoryConstraintViolation("Error in update statement. The attribute \"" + typeAttribute.Name + "\" is mandatory and can not be removed."));
                    }
                }
            }


            return new Exceptional<List<TypeAttribute>>(removedAttributes);
        }
        

        private Exceptional<Boolean> RemoveBackwardEdgesOnReferences(AttributeAssignNode aTaskNode, IReferenceEdge myReference, DBObjectStream aDBObject, DBContext dbContext)
        {
            foreach (var item in myReference.GetAllUUIDs())
            {
                var streamExcept = dbContext.DBObjectCache.LoadDBObjectStream(aTaskNode.AttributeIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager), (ObjectUUID)item);

                if (!streamExcept.Success)
                    return new Exceptional<Boolean>(streamExcept.Errors.First());

                var removeExcept = dbContext.DBObjectManager.RemoveBackwardEdge(streamExcept.Value, aTaskNode.AttributeIDNode.LastAttribute.RelatedGraphDBTypeUUID, aTaskNode.AttributeIDNode.LastAttribute.UUID, aDBObject.ObjectUUID);

                if (!removeExcept.Success)
                    return new Exceptional<Boolean>(removeExcept.Errors.First());
            }

            return new Exceptional<Boolean>(true);
        }

        public Exceptional<Tuple<String, TypeAttribute, AObject>> ApplyAssignAttribute(object aTask, DBContext dbContext, DBObjectStream aDBObject)
        {
            #region Data

            AttributeAssignNode aTaskNode = (AttributeAssignNode)aTask;
            Exceptional<AObject> aValue = null;

            #endregion

            //get value for assignement
            aValue = GetValueForAttribute(aTaskNode, aDBObject, dbContext);

            if (aValue.Failed)
            {
                return new Exceptional<Tuple<String, TypeAttribute, AObject>>(aValue);
            }

            object oldValue = null;

            if (aDBObject.HasAttribute(aTaskNode.AttributeIDNode.LastAttribute.UUID, _Type, dbContext.SessionSettings))
            {
                oldValue = aDBObject.GetAttribute(aTaskNode.AttributeIDNode.LastAttribute.UUID);
                
                switch (aTaskNode.AttributeIDNode.LastAttribute.KindOfType)
                {
                    case KindsOfType.SetOfReferences:
                        var typeOfCollection = ((CollectionOfDBObjectsNode)aTaskNode.AttributeValue).CollectionType;

                        if (typeOfCollection == CollectionType.List)
                            return new Exceptional<Tuple<String, TypeAttribute, AObject>>(new Error_InvalidAssignOfSet(aTaskNode.AttributeIDNode.LastAttribute.Name));

                        var removeRefExcept = RemoveBackwardEdgesOnReferences(aTaskNode, (IReferenceEdge)oldValue, aDBObject, dbContext);

                        if (!removeRefExcept.Success)
                            return new Exceptional<Tuple<String, TypeAttribute, AObject>>(removeRefExcept.Errors.First());

                        oldValue = (ASetReferenceEdgeType)aValue.Value;
                        break;

                    case KindsOfType.SetOfNoneReferences:
                    case KindsOfType.ListOfNoneReferences:
                        oldValue = (AListBaseEdgeType)aValue.Value;
                        break;

                    case KindsOfType.SingleReference:
                        if (aValue.Value is ASingleReferenceEdgeType)
                        {
                            removeRefExcept = RemoveBackwardEdgesOnReferences(aTaskNode, (IReferenceEdge)oldValue, aDBObject, dbContext);

                            if (!removeRefExcept.Success)
                                return new Exceptional<Tuple<String, TypeAttribute, AObject>>(removeRefExcept.Errors.First());

                            ((ASingleReferenceEdgeType)oldValue).Merge((ASingleReferenceEdgeType)aValue.Value);
                            aValue.Value = (ASingleReferenceEdgeType)oldValue;
                        }
                        break;

                    default:
                        return new Exceptional<Tuple<String, TypeAttribute, AObject>>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }
            }

            var alterExcept = aDBObject.AlterAttribute(aTaskNode.AttributeIDNode.LastAttribute.UUID, aValue.Value);

            if(alterExcept.Failed)
                return new Exceptional<Tuple<string, TypeAttribute, AObject>>(alterExcept);

            if (!alterExcept.Value)
            {
                aDBObject.AddAttribute(aTaskNode.AttributeIDNode.LastAttribute.UUID, aValue.Value);
            }

            #region add backward edges

            if (aTaskNode.AttributeIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
            {
                Dictionary<AttributeUUID, AObject> userdefinedAttributes = new Dictionary<AttributeUUID, AObject>();
                userdefinedAttributes.Add(aTaskNode.AttributeIDNode.LastAttribute.UUID, aValue.Value);

                var setBackEdges = SetBackwardEdges(_Type, userdefinedAttributes, aDBObject.ObjectUUID, dbContext);

                if (setBackEdges.Failed)
                    return new Exceptional<Tuple<string, TypeAttribute, AObject>>(setBackEdges);
            }

            #endregion

            return new Exceptional<Tuple<String, TypeAttribute, AObject>>(new Tuple<String, TypeAttribute, AObject>(aTaskNode.AttributeIDNode.LastAttribute.Name, aTaskNode.AttributeIDNode.LastAttribute, aValue.Value));
        }

        /// <summary>
        /// This method extracts the value for an attribute assignement.
        /// </summary>
        /// <param name="aTaskNode">The actual assignement task.</param>
        /// <param name="aDBObject">The current DBObject.</param>
        /// <param name="dbContext">The Typemanager of the database.</param>
        /// <returns>The value as an object.</returns>
        private Exceptional<AObject> GetValueForAttribute(AttributeAssignNode aTaskNode, DBObjectStream aDBObject, DBContext dbContext)
        {
            #region data

            AObject value = null;

            #endregion

            switch (aTaskNode.AttributeType)
            {
                case TypesOfOperatorResult.SetOfDBObjects:

                    #region ListOfDBObjects

                    CollectionOfDBObjectsNode aSetNode = (CollectionOfDBObjectsNode)aTaskNode.AttributeValue;

                    if (aTaskNode.AttributeIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                    {
                        //userdefined
                        //value = aSetNode.GetCorrespondigDBObjectUUIDs(aTaskNode.AttributeIDNodee, typeManager, dbObjectCache, mySessionToken);

                        if (aSetNode.CollectionType == CollectionType.SetOfUUIDs)
                        {
                            var retVal = aSetNode.TupleNodeElement.GetAsUUIDEdge(dbContext, aTaskNode.AttributeIDNode.LastAttribute);
                            if (!retVal.Success)
                            {
                                return new Exceptional<AObject>(retVal);
                            }
                            else
                            {
                                value = retVal.Value;
                            }
                        }
                        else
                        {
                            value = (AEdgeType)(aTaskNode.GetCorrespondigDBObjectGuidAsList(_Type, dbContext, aSetNode.TupleNodeElement, aTaskNode.AttributeIDNode.LastAttribute.EdgeType.GetNewInstance(), aTaskNode.AttributeIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager)).Value);
                        }
                    }
                    else
                    {
                        //not userdefined

                        var edge = GetBasicList(dbContext, aTaskNode, aTaskNode.AttributeIDNode.LastAttribute, aSetNode, (AListBaseEdgeType)aTaskNode.AttributeIDNode.LastAttribute.EdgeType.GetNewInstance(), _Type);

                        // If the collection was declared as a SETOF insert
                        if (aSetNode.CollectionType == CollectionType.Set)
                            edge.Distinction();

                        value = edge;
                    }

                    break;

                    #endregion

                case TypesOfOperatorResult.Expression:

                    #region Expression

                    value = ((BinaryExpressionNode)aTaskNode.AttributeValue).SimpleExecution(aDBObject, dbContext, dbContext.SessionSettings);

                    break;

                    #endregion

                #region no references

                case TypesOfOperatorResult.Unknown:
                case TypesOfOperatorResult.Int64:
                case TypesOfOperatorResult.UInt64:
                case TypesOfOperatorResult.Double:
                case TypesOfOperatorResult.DateTime:
                case TypesOfOperatorResult.Boolean:
                case TypesOfOperatorResult.String:

                    if (GraphDBTypeMapper.IsAValidAttributeType(aTaskNode.AttributeIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager), aTaskNode.AttributeType, dbContext, aTaskNode.AttributeValue))
                    {
                        value = GraphDBTypeMapper.GetPandoraObjectFromType(aTaskNode.AttributeType, aTaskNode.AttributeValue);
                    }
                    else
                    {
                        return new Exceptional<AObject>(new Error_InvalidAttributeValue(aTaskNode.AttributeIDNode.LastAttribute.Name, aTaskNode.AttributeValue));
                    }

                    break;

                #endregion

                #region Reference

                case TypesOfOperatorResult.Reference:

                    #region reference

                    SetRefNode setRefNode = (SetRefNode)aTaskNode.AttributeValue;

                    var validationResult = aTaskNode.AttributeIDNode.ValidateMe(_Type, dbContext);

                    if (validationResult.Failed)
                    {
                        return new Exceptional<AObject>(validationResult);
                    }

                    // if we have a Userdefined Type, than all assignments will work on this type
                    if (!aTaskNode.AttributeIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                    {
                        //attributeType = _Type;
                    }

                    value = aTaskNode.AttributeIDNode.LastAttribute.EdgeType.GetNewInstance();

                    var dbos = setRefNode.GetCorrespondigDBObjects(aTaskNode.AttributeIDNode.LastAttribute.GetDBType(dbContext.DBTypeManager), dbContext, aTaskNode.AttributeIDNode.LastAttribute.GetRelatedType(dbContext.DBTypeManager));

                    foreach (var dbo in dbos)
                    {
                        if (dbo.Failed)
                            return new Exceptional<AObject>(dbo);

                        (value as ASingleReferenceEdgeType).Set(dbo.Value.ObjectUUID, setRefNode.Params);
                    }

                    #endregion

                    break;

                #endregion

                #region not implemented

                case TypesOfOperatorResult.NotABasicType:
                default:

                    return new Exceptional<AObject>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                #endregion
            }

            return new Exceptional<AObject>(value);
        }

        private AListBaseEdgeType GetBasicList(DBContext dbContext, AttributeAssignNode aAttributeAssignNode, TypeAttribute attr, CollectionOfDBObjectsNode aListObject, AListBaseEdgeType edge, GraphDBType myType)
        {
            //add some basic elements like FavoriteNumbers
            foreach (TupleElement aTupleElement in aListObject.TupleNodeElement.Tuple)
            {
                if (GraphDBTypeMapper.IsAValidAttributeType(attr.GetDBType(dbContext.DBTypeManager), aTupleElement.TypeOfValue, dbContext, aTupleElement.Value))
                {
                    edge.Add(GraphDBTypeMapper.GetPandoraObjectFromTypeName(attr.GetDBType(dbContext.DBTypeManager).Name, aTupleElement.Value), aTupleElement.Parameters.ToArray());
                }
                else
                {
                    if (aTupleElement.Value is BinaryExpressionNode
                    && ((BinaryExpressionNode)aTupleElement.Value).ResultValue.Value is AtomValue
                    && GraphDBTypeMapper.IsAValidAttributeType(attr.GetDBType(dbContext.DBTypeManager), ((AtomValue)((BinaryExpressionNode)aTupleElement.Value).ResultValue.Value).TypeOfValue, dbContext, aTupleElement.Value))
                    {
                        ((AListBaseEdgeType)edge).Add(((AtomValue)((BinaryExpressionNode)aTupleElement.Value).ResultValue.Value).Value, aTupleElement.Parameters.ToArray());
                    }
                    else if (!(aTupleElement.Value is BinaryExpressionNode))
                    {
                        throw new GraphDBException(new Error_DataTypeDoesNotMatch(attr.GetDBType(dbContext.DBTypeManager).Name, aTupleElement.Value.GetType().Name));
                    }
                    else
                    {
                        //throw new GraphDBException(new Error_SetOfAssignment("Invalid type (" + aAttributeAssignNode.AttributeType + ") of attribute (" + attr.Name + ") for PandoraType \"" + myType.Name + "\"."));
                        throw new GraphDBException(new Error_InvalidAssignOfSet(attr.Name));
                    }
                }
            }

            return edge;
        }

        #endregion

        #region insert methods

        public QueryResult Insert(DBContext dbContext, Boolean checkUniqueness = true)
        {
            if (_Type.IsAbstract)
            {
                return new QueryResult(new Error_InvalidType(_Type, "It is not possible to insert DBObjects of an abstract type"));
            }

            #region Data

            Exceptional<DBObjectStream> dbObjectStream;

            #endregion

            #region create new DBObject

            try
            {
                dbObjectStream = dbContext.DBObjectManager.CreateNewDBObject(_Type, _Attributes.ToDictionary(key => key.Key.Definition.Name, value => value.Value), _UndefinedAttributes, _SpecialTypeAttributes, dbContext.SessionSettings, checkUniqueness);
            }
            catch (GraphDBException pe)
            {
                return new QueryResult(pe.GraphDBErrors);
            }
            catch (Exception e)
            {
                GraphDBError aError = new Error_UnknownDBError(e);

                return new QueryResult(aError);
            }

            if (dbObjectStream.Failed)
            {
                return new QueryResult(dbObjectStream.Errors);
            }

            #endregion

            #region set backward edges for reference attributes

            var userdefinedAttributes = from attr in _Attributes where attr.Key.TypeOfAttribute.IsUserDefined select attr;

            if (userdefinedAttributes.CountIsGreater(0))
            {
                var setBackEdges = SetBackwardEdges(_Type, userdefinedAttributes.ToDictionary(key => key.Key.Definition.UUID, value => value.Value), dbObjectStream.Value.ObjectUUID, dbContext);

                if (setBackEdges.Failed)
                    return new QueryResult(setBackEdges.Errors);
            }

            #endregion

            var readOut = GetManipulationResultSet(dbContext, dbObjectStream, _Attributes, _UndefinedAttributes, _SpecialTypeAttributes);
            var selResultSet = new SelectionResultSet(_Type, new List<DBObjectReadout> { readOut.Value });
            var selectionListElementResultList = new List<SelectionResultSet> { selResultSet };
            var queryResult = new QueryResult(selectionListElementResultList);

            #region If readout creation failed, this is a readon to fail the query result at all

            if (readOut.Failed)
            {
                queryResult.AddErrors(readOut.Errors);
            }

            #endregion

            return queryResult;

        }

        public void CheckMandatoryAttributes(DBContext dbContext)
        {
            Boolean MandatoryDefaults = (Boolean)dbContext.DBSettingsManager.GetSettingValue((new SettingDefaultsOnMandatory()).ID, dbContext, TypesSettingScope.TYPE, _Type).Value.Value;
            TypeAttribute TypeAttr = null;
            GraphDBType TypeOfAttr = null;

            if ((_MandatoryStmtAttr.Count < _TypeMandatoryAttribs.Count()) && !MandatoryDefaults)
                throw new GraphDBException(new Error_MandatoryConstraintViolation(_Type.Name));

            foreach (var attrib in _TypeMandatoryAttribs)
            {
                if (!_MandatoryStmtAttr.Contains(attrib))
                {
                    //if we have mandatory attributes in type but not in the current statement and USE_DEFAULTS_ON_MANDATORY is true then do this
                    if (MandatoryDefaults)
                    {
                        TypeAttr = _Type.GetTypeAttributeByUUID(attrib);

                        if (TypeAttr == null)
                            throw new GraphDBException(new Error_AttributeDoesNotExists(_Type.Name, TypeAttr.Name));

                        TypeOfAttr = dbContext.DBTypeManager.GetTypeByUUID(TypeAttr.DBTypeUUID);

                        AObject defaultValue = TypeAttr.DefaultValue;

                        if (defaultValue == null)
                            defaultValue = TypeAttr.GetDefaultValue(dbContext);

                        _Attributes.Add(new AttributeDefinition(TypeAttr, TypeOfAttr), defaultValue);
                    }
                    else
                    {
                        throw new GraphDBException(new Error_MandatoryConstraintViolation("Attribute \"" + _Type.GetTypeAttributeByUUID(attrib).Name + "\" of Type \"" + _Type.Name + "\" is mandatory."));
                    }
                }
            }

        }

        /// <summary>
        /// Get attribute assignments for new DBObjects.
        /// </summary>
        /// <param name="parseNode">The interesting ParseTreeNode.</param>
        /// <param name="typeManager">The TypeManager of the PandoraDB.</param>
        /// <returns>A Dictionary of AttributeAssignments</returns>
        public Exceptional GetRecursiveAttributes(ParseTreeNode parseNode, DBContext dbContext)
        {

            #region Data

            _Attributes = new Dictionary<AttributeDefinition, AObject>();
            _MandatoryStmtAttr = new HashSet<AttributeUUID>();
            _AssignNodeList = new HashSet<AttributeAssignNode>();
            AttributeAssignNode aAttributeAssignNode;
            TypeAttribute attr;
            ADBBaseObject typedAttributeValue;
            TypesOfOperatorResult correspondingCSharpType;
            _SpecialTypeAttributes = new Dictionary<ASpecialTypeAttribute, object>();
            _UndefinedAttributes = new Dictionary<String, AObject>();

            #endregion

            #region get Data

            #region proceed list

            foreach (ParseTreeNode aAttributeAssignment in parseNode.ChildNodes)
            {

                aAttributeAssignNode = (AttributeAssignNode)aAttributeAssignment.AstNode;
               
                #region Undefined attributes

                //in this case we have an undefined attribute
                if (aAttributeAssignNode.AttributeIDNode == null)
                {

                    #region in this case we have an undefined attribute

                    if (aAttributeAssignment.HasChildNodes())
                    {
                        if (aAttributeAssignment.ChildNodes[0].HasChildNodes())
                        {
                            if (aAttributeAssignment.ChildNodes[0].ChildNodes.Count > 1)
                                return new Exceptional(new Error_InvalidUndefinedAttributeName());

                            String UndefAttrName = aAttributeAssignment.ChildNodes[0].ChildNodes[0].Token.ValueString;

                            if (aAttributeAssignment.ChildNodes[0].ChildNodes.Count > 1)
                            {
                                return new Exceptional(new Error_InvalidUndefinedAttributeName());
                            }

                            if (aAttributeAssignment.ChildNodes[2] != null)
                            {
                                if (!_UndefinedAttributes.ContainsKey(UndefAttrName))
                                {
                                    if (aAttributeAssignment.ChildNodes[2].AstNode is SetRefNode)
                                        return new Exceptional(new Error_InvalidReferenceAssignmentOfUndefAttr());

                                    if (aAttributeAssignment.ChildNodes[2].AstNode is CollectionOfDBObjectsNode)
                                    {
                                        CollectionOfDBObjectsNode colNode = (CollectionOfDBObjectsNode)aAttributeAssignment.ChildNodes[2].AstNode;
                                        EdgeTypeListOfBaseObjects valueList = new EdgeTypeListOfBaseObjects();

                                        try
                                        {
                                            foreach (var tuple in colNode.TupleNodeElement.Tuple)
                                            {
                                                if (tuple.TypeOfValue == TypesOfOperatorResult.Unknown)
                                                    valueList.Add(GraphDBTypeMapper.GetBaseObjectFromCSharpType(tuple.Value));
                                                else
                                                    return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                                            }

                                            if (colNode.CollectionType == CollectionType.Set)
                                                valueList.UnionWith(valueList);

                                            _UndefinedAttributes.Add(UndefAttrName, valueList);
                                        }
                                        catch (GraphDBException e)
                                        {
                                            throw e;
                                        }
                                    }
                                    else
                                    {
                                        var typeOfExpression = aAttributeAssignment.ChildNodes[2].Term.GetType();
                                        var value = GraphDBTypeMapper.GetBaseObjectFromCSharpType(aAttributeAssignment.ChildNodes[2].Token.Value);
                                        _UndefinedAttributes.Add(UndefAttrName, value);
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    continue;
                }

                #endregion

                attr = aAttributeAssignNode.AttributeIDNode.LastAttribute;
                _AssignNodeList.Add(aAttributeAssignNode);

                #region checks

                if (aAttributeAssignNode.AttributeIDNode.LastType != _Type)
                {
                    return new Exceptional(new Error_InvalidAttribute(aAttributeAssignNode.AttributeIDNode.ToString()));
                }

                if (attr.GetDBType(dbContext.DBTypeManager).IsBackwardEdge)
                {
                    return new Exceptional(new Error_Logic("Adding values to BackwardEdges Attributes are not allowed! (" + attr.Name + ") is an BackwardEdge Attribute of PandoraType \"" + _Type.Name + "\"."));
                }

                #endregion

                #region SpecialTypeAttribute

                if (attr is ASpecialTypeAttribute)
                {
                    _SpecialTypeAttributes.Add((attr as ASpecialTypeAttribute), aAttributeAssignNode.AttributeValue);

                    continue;
                }

                #endregion

                #region check & add

                if (aAttributeAssignNode.AttributeIDNode.Edges.Count > 1)
                {
                    //in case of those statements: INSERT INTO Flower VALUES (Colors.Name = 'red')
                    //Colors.Name is an IDNode with 2 Edges. This is not possible.

                    return new Exceptional(new Error_Logic("Invalid attribute assignment : " + aAttributeAssignNode.AttributeIDNode.ToString() + " = " + aAttributeAssignNode.AttributeValue));
                }

                //checking the PandoraType each AttributeAssignment
                switch (aAttributeAssignNode.AttributeType)
                {

                    case TypesOfOperatorResult.SetOfDBObjects:
                        
                        #region SetOfDBObjects
		
                        #region Check, whether this is valid for SetOfDBObjects

                        if (attr.KindOfType != KindsOfType.SetOfReferences)
                        {
                            if (attr.KindOfType != KindsOfType.ListOfNoneReferences)
                            {
                                if (attr.KindOfType != KindsOfType.SetOfNoneReferences)
                                {
                                    if (aAttributeAssignNode.AttributeValue is CollectionOfDBObjectsNode)
                                    {
                                        return new Exceptional(new Error_ReferenceAssignmentExpected(attr));//"Please use SETREF keyword instead of REF/REFERENCE or LISTOF."));
                                    }
                                }
                            }
                        } 

	                    #endregion

                        #region list stuff

                        #region process tuple

                        if (aAttributeAssignNode.AttributeValue is CollectionOfDBObjectsNode)
                        {
                            #region process as list

                            var aListObject = (CollectionOfDBObjectsNode)aAttributeAssignNode.AttributeValue;
                            var dbType = attr.GetDBType(dbContext.DBTypeManager);

                            if (dbType.IsUserDefined)
                            {

                                #region List of references

                                var uuids = aListObject.GetEdge(attr, dbType, dbContext);
                                if (uuids.Failed)
                                {
                                    return new Exceptional(uuids);
                                }

                                _Attributes.Add(new AttributeDefinition(attr, dbType), uuids.Value);

                                if (_TypeMandatoryAttribs.Contains(attr.UUID))
                                    _MandatoryStmtAttr.Add(attr.UUID);

                                #endregion

                            }
                            else
                            {

                                #region List of ADBBaseObjects (Integer, String, etc)

                                var edge = GetBasicList(dbContext, aAttributeAssignNode, attr, aListObject, (AListBaseEdgeType)attr.EdgeType.GetNewInstance(), _Type);

                                // If the collection was declared as a SETOF insert
                                if (aListObject.CollectionType == CollectionType.Set)
                                    edge.Distinction();

                                _Attributes.Add(new AttributeDefinition(attr, attr.GetDBType(dbContext.DBTypeManager)), edge);

                                if (_TypeMandatoryAttribs.Contains(attr.UUID))
                                    _MandatoryStmtAttr.Add(attr.UUID);

                                #endregion

                            }

                            #endregion
                        }

                        #endregion

                        #endregion
 
	                    #endregion

                        break;

                    case TypesOfOperatorResult.Reference:

                        #region reference

                        SetRefNode aSetRefNode = (SetRefNode)aAttributeAssignNode.AttributeValue;

                        var singleedge = aSetRefNode.GetEdge(attr, dbContext, attr.GetRelatedType(dbContext.DBTypeManager));
                        if (singleedge.Failed)
                        {
                            return new Exceptional(singleedge);
                        }

                        if (attr.GetRelatedType(dbContext.DBTypeManager).IsUserDefined)
                        {
                            //a list which carries elements of userdefined types consists of GUIDS
                            _Attributes.Add(new AttributeDefinition(attr, attr.GetDBType(dbContext.DBTypeManager)), singleedge.Value);

                            if (_TypeMandatoryAttribs.Contains(attr.UUID))
                                _MandatoryStmtAttr.Add(attr.UUID);

                        }
                        else
                        {
                            return new Exceptional(new Error_UnknownDBError("Reference types cannot be basic types."));
                        }

                        #endregion

                        break;

                    case TypesOfOperatorResult.Expression:
                        
                        #region Expression
		
                        var value = (aAttributeAssignNode.AttributeValue as BinaryExpressionNode).ResultValue;

                        if (value.Failed)
                        {
                            return new Exceptional(value.Errors.First());
                        }


                        if (value.Value is AtomValue)
                        {

                            #region AtomValue

                            if (attr.KindOfType == KindsOfType.SetOfReferences || attr.KindOfType == KindsOfType.ListOfNoneReferences || attr.KindOfType == KindsOfType.SetOfNoneReferences)
                                return new Exceptional(new Error_InvalidTuple(aAttributeAssignNode.AttributeValue.ToString()));

                            var val = (value.Value as AtomValue).Value.Value;

                            correspondingCSharpType = GraphDBTypeMapper.ConvertPandora2CSharp(attr, attr.GetDBType(dbContext.DBTypeManager));
                            if (GraphDBTypeMapper.GetEmptyPandoraObjectFromType(correspondingCSharpType).IsValidValue(val))
                            {
                                typedAttributeValue = GraphDBTypeMapper.GetPandoraObjectFromType(correspondingCSharpType, val);
                                _Attributes.Add(new AttributeDefinition(attr, attr.GetDBType(dbContext.DBTypeManager)), typedAttributeValue);

                                if (_TypeMandatoryAttribs.Contains(attr.UUID))
                                    _MandatoryStmtAttr.Add(attr.UUID);
                            }
                            else
                            {
                                return new Exceptional(new Error_InvalidAttributeValue(attr.Name, aAttributeAssignNode.AttributeValue));
                            }

                            #endregion

                        }
                        else // TupleValue!
                        {

                            #region TupleValue

                            if (attr.KindOfType != KindsOfType.SetOfReferences)
                                return new Exceptional(new Error_InvalidTuple(aAttributeAssignNode.AttributeValue.ToString()));

                            if (!(attr.EdgeType is AListBaseEdgeType))
                                return new Exceptional(new Error_InvalidEdgeType(attr.EdgeType.GetType(), typeof(AListBaseEdgeType)));

                            correspondingCSharpType = GraphDBTypeMapper.ConvertPandora2CSharp(attr, attr.GetDBType(dbContext.DBTypeManager));

                            if ((value.Value as TupleValue).TypeOfValue != correspondingCSharpType)
                                return new Exceptional(new Error_DataTypeDoesNotMatch(correspondingCSharpType.ToString(), (value.Value as TupleValue).TypeOfValue.ToString()));

                            var edge = attr.EdgeType.GetNewInstance() as IBaseEdge;
                            edge.AddRange((value.Value as TupleValue).Values);
                            _Attributes.Add(new AttributeDefinition(attr, attr.GetDBType(dbContext.DBTypeManager)), edge as AListEdgeType);

                            #endregion

                        }
 
	                    #endregion

                        break;

                    default:

                        if (attr.KindOfType == KindsOfType.ListOfNoneReferences)
                            return new Exceptional(new Error_InvalidTuple(aAttributeAssignNode.AttributeValue.ToString()));

                        correspondingCSharpType = GraphDBTypeMapper.ConvertPandora2CSharp(attr, attr.GetDBType(dbContext.DBTypeManager));
                        if (GraphDBTypeMapper.GetEmptyPandoraObjectFromType(correspondingCSharpType).IsValidValue(aAttributeAssignNode.AttributeValue))
                        {
                            typedAttributeValue = GraphDBTypeMapper.GetPandoraObjectFromType(correspondingCSharpType, aAttributeAssignNode.AttributeValue);
                            _Attributes.Add(new AttributeDefinition(attr, attr.GetDBType(dbContext.DBTypeManager)), typedAttributeValue);

                            if (_TypeMandatoryAttribs.Contains(attr.UUID))
                                _MandatoryStmtAttr.Add(attr.UUID);
                        }
                        else
                        {
                            return new Exceptional(new Error_InvalidAttributeValue(attr.Name, aAttributeAssignNode.AttributeValue));
                        }

                        break;

                }

                #endregion

            }

            #endregion

            #endregion

            return Exceptional.OK;
        }

        #endregion
        
        #region delete methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myPandoraType"></param>
        /// <param name="attributes">Pass null if you want to delete the whole object with all attributes</param>
        /// <param name="myListOfAffectedDBObjectUUIDs"></param>
        /// <param name="dbContext"></param>
        /// <param name="myToken"></param>
        /// <param name="myDBObjectCache"></param>
        /// <returns></returns>
        public Exceptional<SelectionResultSet> DeleteDBObjects(GraphDBType myPandoraType, List<TypeAttribute> attributes, IEnumerable<Exceptional<DBObjectStream>> myListOfAffectedDBObjects, DBContext dbContext)
        {
            //some stuff for queryResult
            UInt64 deletionCounter = 0;

            foreach (var aDBO in myListOfAffectedDBObjects)
            {
                if (aDBO.Failed)
                {
                    return new Exceptional<SelectionResultSet>(new Error_LoadObject(aDBO.Value.ObjectUUID.ToString()));
                }

                deletionCounter++;
                
                if (!attributes.IsNullOrEmpty())
                {
                    foreach (var Attr in attributes)
                    {
                        if (Attr.GetRelatedType(dbContext.DBTypeManager).GetMandatoryAttributesUUIDs(dbContext.DBTypeManager).Contains(Attr.UUID))
                        {
                            if ((Boolean)dbContext.DBSettingsManager.GetSetting(SettingDefaultsOnMandatory.UUID, dbContext, TypesSettingScope.TYPE, Attr.GetRelatedType(dbContext.DBTypeManager)).Value.Value.Value)
                            {
                                var defaultExcept = SetDefaultValue(Attr, aDBO, dbContext);

                                if(defaultExcept.Failed)
                                    return new Exceptional<SelectionResultSet>(defaultExcept);

                                if (!defaultExcept.Value)
                                    return new Exceptional<SelectionResultSet>(new Error_UpdateAttributeValue(Attr));
                            }
                            else
                                return new Exceptional<SelectionResultSet>(new Error_MandatoryConstraintViolation(Attr.Name));
                        }
                        else
                        {
                            if (!aDBO.Value.RemoveAttribute(Attr.UUID))                        
                                return new Exceptional<SelectionResultSet>(new Error_RemoveTypeAttribute(myPandoraType, Attr));                            
                        }
                    }
                    
                    var saveResult = aDBO.Value.Save();

                    if (!saveResult.Success)
                    {
                        return new Exceptional<SelectionResultSet>(saveResult);
                    }
                }
                else
                {
                    var objID = aDBO.Value.ObjectUUID;
                    var backwarEdges = aDBO.Value.BackwardEdges;

                    if (backwarEdges == null)
                    {
                        var backwardEdgeLoadExcept = dbContext.DBObjectManager.LoadBackwardEdge(aDBO.Value.ObjectLocation);
                        
                        if (!backwardEdgeLoadExcept.Success)
                            return new Exceptional<SelectionResultSet>(backwardEdgeLoadExcept.Errors.First());

                        backwarEdges = backwardEdgeLoadExcept.Value;
                    }
                    
                    var removeResult = dbContext.DBObjectManager.RemoveDBObject(myPandoraType, aDBO.Value, dbContext.DBObjectCache, dbContext.SessionSettings);

                    if (!removeResult.Success)
                    {
                        return new Exceptional<SelectionResultSet>(removeResult);
                    }

                    if (backwarEdges != null)
                    {
                        var delEdgeRefs = DeleteObjectReferences(objID, backwarEdges, dbContext);

                        if (!delEdgeRefs.Success)
                            return new Exceptional<SelectionResultSet>(delEdgeRefs.Errors.First());
                    }
                }                
            }

            if (deletionCounter == 0)
            {
                var result = new Exceptional<SelectionResultSet>(new SelectionResultSet(myPandoraType, deletionCounter));
                return result.PushT(new Warning_NoObjectsToDelete());
            }

            return new Exceptional<SelectionResultSet>(new SelectionResultSet(myPandoraType, deletionCounter));

        }

        public Exceptional<Boolean> DeleteObjectReferences(ObjectUUID myObjectUUID, BackwardEdgeStream myObjectBackwardEdges, DBContext dbContext)
        {            
            foreach (var item in myObjectBackwardEdges)
            {
                var type = dbContext.DBTypeManager.GetTypeByUUID(item.Key.TypeUUID);

                if (type == null)
                    return new Exceptional<Boolean>(new Error_TypeDoesNotExist(""));
                
                foreach (var objID in item.Value.GetAllUUIDs())
                {
                    var dbObj = dbContext.DBObjectCache.LoadDBObjectStream(type, objID);

                    if (!dbObj.Success)
                        return new Exceptional<Boolean>(dbObj.Errors.First());

                    var attr = dbObj.Value.GetAttribute(item.Key.AttrUUID);

                    if (attr != null)
                    {
                        if (attr is ASetReferenceEdgeType)
                            ((IReferenceEdge)attr).RemoveUUID(myObjectUUID);
                        else if (attr is ASingleReferenceEdgeType && ((IReferenceEdge)attr).GetAllUUIDs().Contains(myObjectUUID))
                            dbObj.Value.RemoveAttribute(item.Key.AttrUUID); 
                    }

                    var flushExcept = dbContext.DBObjectManager.FlushDBObject(dbObj.Value);

                    if (!flushExcept.Success)
                        return new Exceptional<bool>(flushExcept.Errors.First());
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
            if (myDBO.Success)
            {
                AObject defaultValue = myAttr.DefaultValue;

                if (defaultValue == null)
                    defaultValue = myAttr.GetDefaultValue(dbContext);
                
                var alterExcept = myDBO.Value.AlterAttribute(myAttr.UUID, defaultValue);

                if (alterExcept.Failed)
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

        public QueryResult Delete(BinaryExpressionNode _WhereExpression, DBContext dbContext, Dictionary<GraphDBType, List<string>> _TypeWithUndefAttrs, Dictionary<string, List<TypeAttribute>> _DBTypeAttributeToDelete, Dictionary<String, GraphDBType> _ReferenceTypeLookup)
        {

            List<SelectionResultSet> _SelectionListElementResultList = new List<SelectionResultSet>();
            QueryResult result = new QueryResult(_SelectionListElementResultList);

            try
            {
                #region get UUIDs

                if (_WhereExpression != null)
                {
                    #region _WhereExpression

                    var resultGraph = _WhereExpression.Calculon(dbContext, new CommonUsageGraph(dbContext), false);
                    if (resultGraph.Failed)
                    {
                        return new QueryResult(resultGraph.Errors);
                    }

                    IEnumerable<Exceptional<DBObjectStream>> _dbobjects;

                    #region undefined attributes

                    foreach (var type in _TypeWithUndefAttrs)
                    {
                        _dbobjects = resultGraph.Value.Select(new LevelKey(type.Key), null, false);

                        foreach (var dbObj in _dbobjects)
                        {
                            foreach (var undefAttr in type.Value)
                            {
                                var removeExcept = dbContext.DBObjectManager.RemoveUndefinedAttribute(undefAttr, dbObj.Value);

                                if (removeExcept.Failed)
                                    return new QueryResult(removeExcept.Errors);
                            }
                        }
                    }

                    #endregion

                    Boolean generateLevel = true;

                    foreach (var aToBeDeletedAttribute in _DBTypeAttributeToDelete)
                    {
                        if (!resultGraph.Value.ContainsRelevantLevelForType(_ReferenceTypeLookup[aToBeDeletedAttribute.Key]))
                        {
                            generateLevel = false;
                        }
                        else
                        {
                            generateLevel = true;
                        }

                        var deleteResult = DeleteDBObjects(_ReferenceTypeLookup[aToBeDeletedAttribute.Key], aToBeDeletedAttribute.Value, resultGraph.Value.Select(new LevelKey(_ReferenceTypeLookup[aToBeDeletedAttribute.Key]), null, generateLevel), dbContext);

                        if (deleteResult.Failed)
                        {
                            return new QueryResult(deleteResult.Errors);
                        }
                        else
                        {

                            if (!deleteResult.Success)
                            {
                                result.AddWarnings(deleteResult.Warnings);
                            }

                            result.AddResult(deleteResult.Value);

                        }

                    }

                    #region expressionGraph error handling

                    result.AddWarnings(resultGraph.Value.GetWarnings());

                    #endregion

                    #endregion
                }
                else
                {

                    if (_DBTypeAttributeToDelete.Count == 0)
                    {

                        #region delete only undefined attributes of types

                        foreach (var type in _TypeWithUndefAttrs)
                        {
                            var listOfAffectedDBObjects = dbContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(type.Key), dbContext);
                            RemoveUndefAttrs(listOfAffectedDBObjects, type.Value, dbContext);
                        }

                        #endregion

                    }
                    else
                    {
                        #region get guids via guid-idx

                        foreach (var _PandoraType in _DBTypeAttributeToDelete)
                        {
                            var listOfAffectedDBObjects = dbContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(_ReferenceTypeLookup[_PandoraType.Key]), dbContext).ToList();

                            if (_TypeWithUndefAttrs.ContainsKey(_ReferenceTypeLookup[_PandoraType.Key]))
                                RemoveUndefAttrs(listOfAffectedDBObjects, _TypeWithUndefAttrs[_ReferenceTypeLookup[_PandoraType.Key]], dbContext);

                            var Result = DeleteDBObjects(_ReferenceTypeLookup[_PandoraType.Key], _DBTypeAttributeToDelete[_PandoraType.Key], listOfAffectedDBObjects, dbContext);

                            if (Result.Failed)
                            {
                                return new QueryResult(Result.Errors);
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

        private Exceptional<Boolean> RemoveUndefAttrs(IEnumerable<Exceptional<DBObjectStream>> myDBObjStream, List<String> myUndefAttrs, DBContext dbContext)
        {
            #region delete undefined attributes

            foreach (var objects in myDBObjStream)
            {
                if (objects.Value.UndefAttributes != null)
                {
                    var undefAttrsStream = myUndefAttrs.Where(item => objects.Value.GetUndefinedAttributes(dbContext.DBObjectManager).Value.ContainsKey(item));

                    foreach (var undefAttr in undefAttrsStream)
                    {
                        var removeExcept = dbContext.DBObjectManager.RemoveUndefinedAttribute(undefAttr, objects.Value);

                        if (removeExcept.Failed)
                            return new Exceptional<bool>(removeExcept);
                    }
                }
            }

            #endregion

            return new Exceptional<bool>(true);
        }

        #endregion

        #region GetManipulationResultSet

        /// <summary>
        /// Create a readout based on the passed <paramref name="attributes"/>, <paramref name="undefinedAttributes"/>, <paramref name="specialTypeAttributes"/> which are all optional
        /// </summary>
        /// <param name="dbObjectStream"></param>
        /// <param name="attributes"></param>
        /// <param name="undefinedAttributes"></param>
        /// <param name="specialTypeAttributes"></param>
        /// <returns></returns>
        private Exceptional<DBObjectReadout> GetManipulationResultSet(DBContext dbContext, Exceptional<DBObjectStream> dbObjectStream, Dictionary<AttributeDefinition, AObject> attributes = null, Dictionary<String, AObject> undefinedAttributes = null, Dictionary<ASpecialTypeAttribute, Object> specialTypeAttributes = null)
        {
            DBObjectReadout readOut = null;

            #region Return inserted attributes

            #region attributes

            if (!attributes.IsNullOrEmpty())
            {

                readOut = new DBObjectReadout(attributes.ToDictionary(key => key.Key.Definition.UUID, value => value.Value), _Type);

            }
            else
            {
                readOut = new DBObjectReadout();
            }

            #endregion

            #region UndefinedAttributes

            if (!undefinedAttributes.IsNullOrEmpty())
            {

                foreach (var undefAttr in undefinedAttributes)
                {
                    readOut.Attributes.Add(undefAttr.Key, undefAttr.Value.GetReadoutValue());
                }

            }

            #endregion

            #region SpecialTypeAttributes

            if (!specialTypeAttributes.IsNullOrEmpty())
            {

                foreach (var specAttr in specialTypeAttributes)
                {
                    readOut.Attributes.Add(specAttr.Key.Name, specAttr.Value);
                }

            }

            #endregion

            #region UUID

            if (!readOut.Attributes.ContainsKey(SpecialTypeAttribute_UUID.AttributeName)) // If it was updated by SpecialTypeAttributes we do not need to add them again
            {

                var extractedValue = new SpecialTypeAttribute_UUID().ExtractValue(dbObjectStream.Value, _Type, dbContext);
                if (extractedValue.Failed)
                {
                    return new Exceptional<DBObjectReadout>(extractedValue);
                }
                readOut.Attributes.Add(SpecialTypeAttribute_UUID.AttributeName, extractedValue.Value.GetReadoutValue());

            }

            #endregion

            #region REVISION

            if (!readOut.Attributes.ContainsKey(SpecialTypeAttribute_REVISION.AttributeName)) // If it was updated by SpecialTypeAttributes we do not need to add them again
            {

                var extractedValue = new SpecialTypeAttribute_REVISION().ExtractValue(dbObjectStream.Value, _Type, dbContext);
                if (extractedValue.Failed)
                {
                    return new Exceptional<DBObjectReadout>(extractedValue);
                }
                readOut.Attributes.Add(SpecialTypeAttribute_REVISION.AttributeName, extractedValue.Value.GetReadoutValue());

            }

            #endregion

            #endregion

            return new Exceptional<DBObjectReadout>(readOut);

        }

        #endregion

        #region BackwardEdges

        public Exceptional SetBackwardEdges(GraphDBType aType, Dictionary<AttributeUUID, AObject> userdefinedAttributes, ObjectUUID reference, DBContext dbContext)
        {

            var returnVal = new Exceptional();

            #region process attributes

            foreach (var aUserDefinedAttribute in userdefinedAttributes)
            {
                #region Data

                GraphDBType typeOFAttribute = null;
                TypeAttribute attributesOfType = null;

                #endregion

                #region get PandoraType of Attribute

                //attributesOfType = aType.Attributes[aUserDefinedAttribute.Key];
                attributesOfType = aType.GetTypeAttributeByUUID(aUserDefinedAttribute.Key);

                typeOFAttribute = dbContext.DBTypeManager.GetTypeByUUID(attributesOfType.DBTypeUUID);

                #endregion

                //if (attributesOfType.KindOfType == KindsOfType.SetOfReferences)
                //{
                    /* The DBO independent version */
                    var beEdge = new EdgeKey(aType.UUID, attributesOfType.UUID);
                    
                    foreach (var uuid in ((IReferenceEdge)aUserDefinedAttribute.Value).GetAllUUIDs())
                    {
                        var addExcept = dbContext.DBObjectManager.AddBackwardEdge(uuid, attributesOfType.DBTypeUUID, beEdge, reference);

                        if (addExcept.Failed)
                            return new Exceptional<bool>(addExcept);
                    }
                    /**/
                    /* The parallel version
                    Parallel.ForEach(((ASetReferenceEdgeType)aUserDefinedAttribute.Value).GetAllUUIDs(), (uuid) =>
                    {
                        var addExcept = dbContext.DBObjectManager.AddBackwardEdge(uuid, attributesOfType.DBTypeUUID, beEdge, reference);

                        if (!addExcept.Success)
                        {
                            returnVal.AddErrorsAndWarnings(addExcept);
                        }
                    });

                    if (!returnVal.Success)
                    {
                        return returnVal;
                    }
                    */
                    /* The DBO dependent version
                    var listOfObjects = dbContext.DBObjectCache.LoadListOfDBObjectStreams(typeOFAttribute, ((ASetReferenceEdgeType)aUserDefinedAttribute.Value).GetAllUUIDs().Distinct());

                    foreach (var aDBObject in listOfObjects)
                    {
                        if (aDBObject.Failed)
                        {
                            return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                        }

                        var setExcept = dbContext.DBObjectManager.AddBackwardEdge(aDBObject.Value, aType.UUID, attributesOfType.UUID, reference);

                        if (setExcept.Failed)
                            return new Exceptional<bool>(setExcept);
                    }
                    */
                //}
                //else
                //{
                //    var aDBObject = dbContext.DBObjectCache.LoadDBObjectStream(typeOFAttribute, ((ASingleReferenceEdgeType)aUserDefinedAttribute.Value).GetUUID());
                //    if (aDBObject.Failed)
                //    {
                //        return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                //    }

                //    var setExcept = dbContext.DBObjectManager.AddBackwardEdge(aDBObject.Value, aType.UUID, aUserDefinedAttribute.Key, reference);

                //    if (setExcept.Failed)
                //        return new Exceptional(setExcept);
                //}
            }

            #endregion

            return Exceptional.OK;
        }

        public Exceptional<Boolean> RemoveBackwardEdges(TypeUUID uUIDofType, Dictionary<AttributeUUID, object> userdefinedAttributes, ObjectUUID reference, DBContext dbContext)
        {
            #region data

            GraphDBType aType = null;

            #endregion

            #region get type that carries the attributes

            aType = dbContext.DBTypeManager.GetTypeByUUID(uUIDofType);

            #endregion

            #region process attributes

            foreach (KeyValuePair<AttributeUUID, object> aUserDefinedAttribute in userdefinedAttributes)
            {

                #region Data

                GraphDBType typeOFAttribute = null;
                TypeAttribute attributesOfType = null;

                #endregion

                #region get PandoraType of Attribute

                attributesOfType = aType.Attributes[aUserDefinedAttribute.Key];

                typeOFAttribute = dbContext.DBTypeManager.GetTypeByUUID(attributesOfType.DBTypeUUID);

                #endregion


                IEnumerable<Exceptional<DBObjectStream>> listOfObjects;

                if (aUserDefinedAttribute.Value is IReferenceEdge)
                {
                    listOfObjects = dbContext.DBObjectCache.LoadListOfDBObjectStreams(typeOFAttribute, ((IReferenceEdge)aUserDefinedAttribute.Value).GetAllUUIDs());
                }
                else
                {
                    listOfObjects = dbContext.DBObjectCache.LoadListOfDBObjectStreams(typeOFAttribute, (HashSet<ObjectUUID>)aUserDefinedAttribute.Value);
                }

                foreach (var aDBObject in listOfObjects)
                {
                    if (aDBObject.Failed)
                    {
                        return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    var removeExcept = dbContext.DBObjectManager.RemoveBackwardEdge(aDBObject.Value, uUIDofType, aUserDefinedAttribute.Key, reference);

                    if (removeExcept.Failed)
                        return new Exceptional<bool>(removeExcept);
                }

            }

            #endregion

            return new Exceptional<bool>(true);
        }

        #endregion
    }
}
