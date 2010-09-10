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
 * AAlterTypeCommand and implementations
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.Errors;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;

using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.ExpressionGraph;
using sones.GraphDB.Errors.AttributeAssignmentErrors;
using sones.GraphDB.Indices;
using sones.GraphDBInterface.Result;
using sones.GraphDBInterface.TypeManagement;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    #region AAlterTypeCommand

    public abstract class AAlterTypeCommand
    {
        public abstract TypesOfAlterCmd AlterType { get; }

        public virtual Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            return new Exceptional(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true), GetType().Name));
        }

        /// <summary>
        /// Create a command specific readout. May return null if no readout applies.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="graphDBType"></param>
        /// <returns>A readout or null</returns>
        public virtual SelectionResultSet CreateReadout(DBContext dbContext, GraphDBType graphDBType)
        {
            //throw new GraphDBException(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true), GetType().Name));
            return null;
        }

        protected SelectionResultSet CreateRenameResult(string myAlterAction, string myFromString, string myToString, GraphDBType myType)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add("TYPE", myType);
            payload.Add("ACTION", myAlterAction);
            payload.Add("FROM", myFromString);
            payload.Add("TO", myToString);

            return new SelectionResultSet(new DBObjectReadout(payload));

        }
    }

    #endregion

    #region SetUnique

    public class AlterType_SetUnique : AAlterTypeCommand
    {

        public List<String> UniqueAttributes { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Unqiue; }
        }

        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            return graphDBType.ChangeUniqueAttributes(UniqueAttributes, dbContext);
        }

    }

    #endregion

    #region SetMandatory

    public class AlterType_SetMandatory : AAlterTypeCommand
    {

        public List<String> MandatoryAttributes { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Mandatory; }
        }

        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            return graphDBType.ChangeMandatoryAttributes(MandatoryAttributes, dbContext.DBTypeManager);
        }

    }

    #endregion

    #region DropUnique

    public class AlterType_DropUnique : AAlterTypeCommand
    {

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.DropUnqiue; }
        }

        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            return graphDBType.DropUniqueAttributes(dbContext.DBTypeManager);
        }

    }

    #endregion

    #region DropMandatory

    public class AlterType_DropMandatory : AAlterTypeCommand
    {

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.DropMandatory; }
        }

        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            /// Why the hack DropUnique???? copied from ObjectManipulationManager: private QueryResult ProcessDropMandatoryAttributes(GraphDBType atype, DBTypeManager myTypeManager)
            return graphDBType.DropUniqueAttributes(dbContext.DBTypeManager);
            //return graphDBType.DropMandatoryAttributes(dbContext.DBTypeManager);
        }

    }

    #endregion

    #region RenameBackwardedge

    public class AlterType_RenameBackwardedge : AAlterTypeCommand
    {

        public String OldName { get; set; }
        public String NewName { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.RenameBackwardedge; }
        }

        /// <summary>
        /// Execute the renaming of the backwardedge of a given type.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="graphDBType"></param>
        /// <returns></returns>
        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            TypeAttribute Attribute = graphDBType.GetTypeAttributeByName(OldName);

            if (Attribute == null)
            {
                return new Exceptional(new Error_AttributeIsNotDefined(OldName));
            }

            return graphDBType.RenameBackwardedge(Attribute, NewName, dbContext.DBTypeManager);
        }

        public override SelectionResultSet CreateReadout(DBContext dbContext, GraphDBType graphDBType)
        {
            return CreateRenameResult("RENAME BACKWARDEDGE", OldName, NewName, graphDBType);
        }

    }

    #endregion

    #region RenameAttribute

    public class AlterType_RenameAttribute : AAlterTypeCommand
    {

        public String OldName { get; set; }
        public String NewName { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.RenameAttribute; }
        }

        /// <summary>
        /// Executes the renaming of the attribute of a given type.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="graphDBType"></param>
        /// <returns></returns>
        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            return dbContext.DBTypeManager.RenameAttributeOfType(graphDBType, OldName, NewName);
        }

        public override SelectionResultSet CreateReadout(DBContext dbContext, GraphDBType graphDBType)
        {
            return CreateRenameResult("RENAME ATTRIBUTE", OldName, NewName, graphDBType);
        }

    }

    #endregion

    #region RenameType

    public class AlterType_RenameType : AAlterTypeCommand
    {

        public String OldName { get; set; }
        public String NewName { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.RenameType; }
        }

        /// <summary>
        /// Execute the renaming of a given type.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="graphDBType"></param>
        /// <returns></returns>
        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            OldName = graphDBType.Name;
            return dbContext.DBTypeManager.RenameType(graphDBType, NewName);
        }

        public override SelectionResultSet CreateReadout(DBContext dbContext, GraphDBType graphDBType)
        {
            return CreateRenameResult("RENAME TYPE", OldName, graphDBType.Name, graphDBType);
        }

    }

    #endregion

    #region ChangeComment

    public class AlterType_ChangeComment : AAlterTypeCommand
    {

        public String NewComment { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.ChangeComment; }
        }

        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            return dbContext.DBTypeManager.ChangeCommentOnType(graphDBType, NewComment);
        }

        public override SelectionResultSet CreateReadout(DBContext dbContext, GraphDBType graphDBType)
        {
            var payload = new Dictionary<String, Object>();

            payload.Add("TYPE", graphDBType);
            payload.Add("ACTION", "CHANGE COMMENT");
            payload.Add("NEW COMMENT", NewComment);

            return new SelectionResultSet(new DBObjectReadout(payload));
        }

    }

    #endregion

    #region DropAttributes

    public class AlterType_DropAttributes : AAlterTypeCommand
    {

        List<String> _ListOfAttributes = new List<string>();

        public AlterType_DropAttributes(List<String> listOfAttributes)
        {
            _ListOfAttributes = listOfAttributes;
        }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Drop; }
        }

        /// <summary>
        /// Executes the removal of certain myAttributes.
        /// </summary>
        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {

            foreach (String aAttributeName in _ListOfAttributes)
            {

                //Hack: remove myAttributes in DBObjects
                var aTempResult = dbContext.DBTypeManager.RemoveAttributeFromType(graphDBType.Name, aAttributeName, dbContext.DBTypeManager);

                if (aTempResult.Failed())
                {
                    return aTempResult;
                }
            }

            return Exceptional.OK;
        }

        public override SelectionResultSet CreateReadout(DBContext dbContext, GraphDBType graphDBType)
        {
            return null;
        }

    }

    #endregion

    #region DropIndices

    public class AlterType_DropIndices : AAlterTypeCommand
    {
        private Dictionary<String, String> _IdxDropList;

        public AlterType_DropIndices(Dictionary<String, String> listOfIndices)
        {
            _IdxDropList = listOfIndices;
        }

        public override TypesOfAlterCmd AlterType
        {
            get { throw new NotImplementedException(); }
        }

        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            var retExceptional = new Exceptional();

            foreach (var index in _IdxDropList)
            {
                var dropIdxExcept = graphDBType.RemoveIndex(index.Key, index.Value, dbContext.DBTypeManager);

                if (!dropIdxExcept.Success())
                {
                    retExceptional.AddErrorsAndWarnings(dropIdxExcept);
                }
            }

            return retExceptional;
        }

        public override SelectionResultSet CreateReadout(DBContext dbContext, GraphDBType graphDBType)
        {
            return base.CreateReadout(dbContext, graphDBType);
        }
    }

    #endregion

    #region AddIndices

    public class AlterType_AddIndices : AAlterTypeCommand
    {

        private List<IndexDefinition> _IdxDefinitionList;

        public AlterType_AddIndices(List<IndexDefinition> listOfIndices)
        {
            _IdxDefinitionList = listOfIndices;
        }

        public override TypesOfAlterCmd AlterType
        {
            get { throw new NotImplementedException(); }
        }

        private Exceptional CheckIndexTypeReference(List<IndexAttributeDefinition> idxDef, GraphDBType type)
        {
            foreach (var idx in idxDef)
            {
                if (idx.IndexType != null && idx.IndexType != type.Name)
                {
                    return new Exceptional(new Error_CouldNotAlterIndexOnType(idx.IndexType));
                }                
            }

            return Exceptional.OK;
        }
        
        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {

            var retExceptional = new Exceptional();

            foreach (var idxDef in _IdxDefinitionList)
            {

                var checkIdx = CheckIndexTypeReference(idxDef.IndexAttributeDefinitions, graphDBType);

                if (!checkIdx.Success())
                {
                    retExceptional.AddErrorsAndWarnings(checkIdx);
                }
                else
                {
                    var result = dbContext.DBIndexManager.CreateIndex(dbContext, graphDBType.Name, idxDef.IndexName, idxDef.Edition, idxDef.IndexType, idxDef.IndexAttributeDefinitions);

                    if (!result.Success())
                    {
                        retExceptional.AddErrorsAndWarnings(result);
                    }
                }

            }
            
            return retExceptional;

        }

        public override SelectionResultSet CreateReadout(DBContext dbContext, GraphDBType graphDBType)
        {

            Dictionary<String, Object> payload = new Dictionary<string, object>();
            List<DBObjectReadout> indicesReadouts = new List<DBObjectReadout>();

            payload.Add("TYPE", graphDBType.Name);

            if (_IdxDefinitionList.IsNotNullOrEmpty())
            {
                payload.Add("ACTION", "ADD INDICES");

                foreach (var idxDef in _IdxDefinitionList)
                {
                    var payloadPerIndex = new Dictionary<string, object>();                   

                    payloadPerIndex.Add("NAME", idxDef.IndexName);
                    payloadPerIndex.Add("EDITION", idxDef.Edition);
                    payloadPerIndex.Add("INDEXTYPE", idxDef.IndexType);

                    if (idxDef.IndexAttributeDefinitions.Count == 1)
                    {
                        payloadPerIndex.Add("ATTRIBUTE", idxDef.IndexAttributeDefinitions[0].IndexAttribute);
                    }
                    else
                    {
                        String attributes = String.Empty;

                        idxDef.IndexAttributeDefinitions.ForEach(item => attributes += item.IndexAttribute + " ");
                        payloadPerIndex.Add("ATTRIBUTES", attributes);
                    }

                    indicesReadouts.Add(new DBObjectReadout(payloadPerIndex));
                }
                
                payload.Add("INDICES", indicesReadouts);
            }

            return new SelectionResultSet(new DBObjectReadout(payload));

        }

    }


    #endregion

    #region AddAttributes

    public class AlterType_AddAttributes : AAlterTypeCommand
    {

        private List<AttributeDefinition>           _ListOfAttributes;
        private List<BackwardEdgeDefinition>        _BackwardEdgeInformation;

        public AlterType_AddAttributes(List<AttributeDefinition> listOfAttributes, List<BackwardEdgeDefinition> backwardEdgeInformation)
        {
            _ListOfAttributes           = listOfAttributes;
            _BackwardEdgeInformation    = backwardEdgeInformation;
        }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Add; }
        }

        /// <summary>
        /// Adds myAttributes to a certain graphDBType.
        /// </summary>
        /// <param name="graphDBType">The type that should be added some myAttributes.</param>
        /// <param name="dbContext">The DBContext.</param>
        /// <returns>A Exceptional.</returns>
        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            
            #region check type for myAttributes

            foreach (var aAttributeDefinition in _ListOfAttributes)
            {
                if (graphDBType.GetTypeAttributeByName(aAttributeDefinition.AttributeName) != null)
                {
                    var aError = new Error_AttributeAlreadyExists(aAttributeDefinition.AttributeName);

                    return new Exceptional(aError);
                }
            }

            #endregion

            #region add myAttributes

            foreach (var aAttributeDefinition in _ListOfAttributes)
            {

                var typeAttributeExceptional = aAttributeDefinition.CreateTypeAttribute(dbContext);
                if (typeAttributeExceptional.Failed())
                {
                    return typeAttributeExceptional;
                }
                var typeAttribute = typeAttributeExceptional.Value;

                try
                {
                    var theType = dbContext.DBTypeManager.GetTypeByName(aAttributeDefinition.AttributeType.Name);
                    typeAttribute.DBTypeUUID = theType.UUID;
                    typeAttribute.RelatedGraphDBTypeUUID = graphDBType.UUID;

                    #region EdgeType
                    if (typeAttribute.EdgeType == null) // should never happen - delete me!
                    {
                        #region we had not defined a special EdgeType - for single reference attributes we need to set the EdgeTypeSingle NOW!
                        if (typeAttribute.KindOfType == KindsOfType.SingleReference)
                            typeAttribute.EdgeType = new EdgeTypeSingleReference(null, theType.UUID);

                        if (typeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                            typeAttribute.EdgeType = new EdgeTypeListOfBaseObjects();

                        if (typeAttribute.KindOfType == KindsOfType.SetOfReferences || typeAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                            typeAttribute.EdgeType = new EdgeTypeSetOfReferences(null, theType.UUID);

                        #endregion
                    }
                    #endregion

                    var aTempResult = dbContext.DBTypeManager.AddAttributeToType(graphDBType, typeAttribute);

                    if (!aTempResult.Success())
                    {
                        return aTempResult;
                    }
                }
                catch (Exception e)
                {
                    var aError = new Error_UnknownDBError(e);

                    return new Exceptional(aError);
                }
            }

            #endregion


            #region add BackwardEdges

            if (_BackwardEdgeInformation != null)
            {
                foreach (var beDef in _BackwardEdgeInformation)
                {
                    //GraphDBType edgeType = typeManager.GetTypeByName(tuple.TypeName);
                    //TypeAttribute edgeAttribute = edgeType.GetTypeAttributeByName(tuple.TypeAttributeName);

                    var typeAttribute = dbContext.DBTypeManager.CreateBackwardEdgeAttribute(beDef, graphDBType);

                    if (typeAttribute.Failed())
                    {
                        return new Exceptional(typeAttribute);
                    }

                    // TODO: go through all DB Objects of the _TypeName and change the implicit backward edge type to the new ta.EdgeType
                    //       if the DBObject has one!

                    try
                    {
                        var aTempResult = dbContext.DBTypeManager.AddAttributeToType(graphDBType, typeAttribute.Value);

                        if (aTempResult.Failed())
                        {
                            return aTempResult;
                        }
                    }
                    catch (Exception e)
                    {
                        return new Exceptional(new Error_UnknownDBError(e));
                    }
                }
            }

            #endregion            

            return Exceptional.OK;

        }

        public override SelectionResultSet CreateReadout(DBContext dbContext, GraphDBType graphDBType)
        {

            #region generate result

            Dictionary<String, Object> payload = new Dictionary<string, object>();
            List<DBObjectReadout> attributeReadouts = new List<DBObjectReadout>();
            payload.Add("TYPE", graphDBType.Name);

            if (_BackwardEdgeInformation.IsNotNullOrEmpty())
            {
                #region backwardEdge

                payload.Add("ACTION", "ADD BACKWARDEDGES");

                foreach (var aAddedBackwardEdge in _BackwardEdgeInformation)
                {

                    var payloadPerBackwardEdge = new Dictionary<string, object>();
                    var type = dbContext.DBTypeManager.GetTypeByName(aAddedBackwardEdge.TypeName);

                    payloadPerBackwardEdge.Add("NAME", aAddedBackwardEdge.AttributeName);
                    payloadPerBackwardEdge.Add("TYPE", type);
                    payloadPerBackwardEdge.Add("ATTRIBUTE", type.GetTypeAttributeByName(aAddedBackwardEdge.TypeAttributeName));

                    attributeReadouts.Add(new DBObjectReadout(payloadPerBackwardEdge));

                }

                payload.Add("BACKWARDEDGES", new Edge(attributeReadouts, "BACKWARDEDGE"));

                #endregion
            }
            else
            {
                #region attributes

                payload.Add("ACTION", "ADD ATTRIBUTES");

                foreach (var aAddedAttribute in _ListOfAttributes)
                {
                    var payloadPerAttribute = new Dictionary<String, Object>();

                    payloadPerAttribute.Add("NAME", aAddedAttribute.AttributeName);
                    payloadPerAttribute.Add("TYPE", dbContext.DBTypeManager.GetTypeByName(aAddedAttribute.AttributeType.Name));

                    attributeReadouts.Add(new DBObjectReadout(payloadPerAttribute));
                }

                payload.Add("ATTRIBUTES", new Edge(attributeReadouts, "ATTRIBUTE"));

                #endregion
            }

            #endregion

            return new SelectionResultSet(new DBObjectReadout(payload));

        }

    }

    #endregion

    #region Defined Attributes

    public class AlterType_DefineAttributes : AAlterTypeCommand
    {
        #region data

        private List<AttributeDefinition> _ListOfAttributes;

        #endregion

        public override TypesOfAlterCmd AlterType
        {
            get { throw new NotImplementedException(); }
        }

        public AlterType_DefineAttributes(List<AttributeDefinition> listOfAttributes)
        {
            _ListOfAttributes = listOfAttributes;
        }

        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {
            var listOfTypeAttributes    = new Dictionary<TypeAttribute, GraphDBType>();
            var retExcept               = new Exceptional();
            var existingTypeAttributes  = graphDBType.GetAllAttributes(dbContext);

            foreach (var attr in _ListOfAttributes)
            {
                var createExcept = attr.CreateTypeAttribute(dbContext);

                if (!createExcept.Success())
                {
                    retExcept.AddErrorsAndWarnings(createExcept);
                }

                if (existingTypeAttributes.Exists(item => item.Name == createExcept.Value.Name))
                {
                    retExcept.AddErrorsAndWarnings(new Exceptional(new Error_AttributeAlreadyExists(createExcept.Value.Name)));
                }

                var attrType = dbContext.DBTypeManager.GetTypeByName(attr.AttributeType.Name);

                if(attrType == null)
                {
                    retExcept.AddErrorsAndWarnings(new Exceptional(new Error_TypeDoesNotExist(attr.AttributeType.Name)));
                    return retExcept;
                }
                
                if(attrType.IsUserDefined)
                {
                    retExcept.AddErrorsAndWarnings(new Exceptional(new Error_InvalidReferenceAssignmentOfUndefAttr()));
                    return retExcept;
                }

                createExcept.Value.DBTypeUUID = attrType.UUID;
                createExcept.Value.RelatedGraphDBTypeUUID = graphDBType.UUID;                

                graphDBType.AddAttribute(createExcept.Value, dbContext.DBTypeManager, true);

                var flushExcept = dbContext.DBTypeManager.FlushType(graphDBType);

                if (!flushExcept.Success())
                {
                    retExcept.AddErrorsAndWarnings(flushExcept);
                }

                listOfTypeAttributes.Add(createExcept.Value, attrType);
            }

            var dbobjects = dbContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(graphDBType, dbContext.DBTypeManager), dbContext);

            foreach (var item in dbobjects)
            {
                if (!item.Success())
                {
                    retExcept.AddErrorsAndWarnings(item);
                }
                else
                {
                    var undefAttrExcept = item.Value.GetUndefinedAttributes(dbContext.DBObjectManager);

                    if (!undefAttrExcept.Success())
                    {
                        retExcept.AddErrorsAndWarnings(undefAttrExcept);
                    }

                    foreach (var attr in listOfTypeAttributes)
                    { 
                        IObject value;

                        if (undefAttrExcept.Value.TryGetValue(attr.Key.Name, out value))
                        {
                            var typeOfOperator = GraphDBTypeMapper.ConvertGraph2CSharp(attr.Value.Name);

                            if (GraphDBTypeMapper.IsAValidAttributeType(attr.Value, typeOfOperator, dbContext, value))
                            {
                                item.Value.AddAttribute(attr.Key.UUID, value);

                                var removeExcept = item.Value.RemoveUndefinedAttribute(attr.Key.Name, dbContext.DBObjectManager);

                                if (!removeExcept.Success())
                                {
                                    retExcept.AddErrorsAndWarnings(removeExcept);
                                }

                                var flushExcept = dbContext.DBObjectManager.FlushDBObject(item.Value);

                                if (!flushExcept.Success())
                                {
                                    retExcept.AddErrorsAndWarnings(flushExcept);
                                }
                            }
                            else 
                            {
                               retExcept.AddErrorsAndWarnings(new Exceptional(new Error_InvalidUndefAttrType(attr.Key.Name, attr.Value.Name)));
                            }
                        }
                    }
                }
            }

            return Exceptional.OK;
        }

        public override SelectionResultSet CreateReadout(DBContext dbContext, GraphDBType graphDBType)
        {
            return base.CreateReadout(dbContext, graphDBType);
        }
    }


    #endregion

    #region Undefined Attributes

    public class AlterType_UndefineAttributes : AAlterTypeCommand
    {
        #region data

        private List<String> _ListOfAttributes;

        #endregion

        public AlterType_UndefineAttributes(List<String> listOfAttributes)
        {
            _ListOfAttributes = listOfAttributes;
        }
        
        public override TypesOfAlterCmd AlterType
        {
            get { throw new NotImplementedException(); }
        }

        public override Exceptional Execute(DBContext dbContext, GraphDBType graphDBType)
        {            
            var listOfTypeAttrs                     = new List<TypeAttribute>();
            var retExcept                           = new Exceptional();
            
            foreach (var attr in _ListOfAttributes)
            {
                var typeAttr = graphDBType.GetTypeAttributeByName(attr);
                
                if (typeAttr == null)
                {
                    return new Exceptional(new Error_AttributeIsNotDefined(attr));
                }

                var attrType = dbContext.DBTypeManager.GetTypeByUUID(typeAttr.DBTypeUUID);

                if (attrType == null)
                    return new Exceptional(new Error_TypeDoesNotExist(""));

                if (attrType.IsUserDefined)
                    return new Exceptional(new Error_InvalidReferenceAssignmentOfUndefAttr());
                
                listOfTypeAttrs.Add(typeAttr);
            }

            var dbobjects = dbContext.DBObjectCache.SelectDBObjectsForLevelKey(new LevelKey(graphDBType, dbContext.DBTypeManager), dbContext);

            foreach (var item in dbobjects)
            {
                if (!item.Success())
                {
                    retExcept.AddErrorsAndWarnings(item);
                }
                else
                {
                    foreach (var attr in listOfTypeAttrs)
                    {
                        if (item.Value.HasAttribute(attr.UUID, graphDBType))
                        {
                            var attrVal = item.Value.GetAttribute(attr.UUID);
                            
                            var addExcept = item.Value.AddUndefinedAttribute(attr.Name, attrVal, dbContext.DBObjectManager);

                            if (!addExcept.Success())
                                retExcept.AddErrorsAndWarnings(addExcept);
                            
                            item.Value.RemoveAttribute(attr.UUID);

                            var saveExcept = dbContext.DBObjectManager.FlushDBObject(item.Value);

                            if (!saveExcept.Success())
                                retExcept.AddErrorsAndWarnings(saveExcept);
                        }
                    }
                }
            }


            var indices = graphDBType.GetAllAttributeIndices();
            List<AAttributeIndex> idxToDelete = new List<AAttributeIndex>();

            //remove attributes from type
            foreach (var attr in listOfTypeAttrs)
            {   
                foreach(var item in indices)
                {
                    var index =  item.IndexKeyDefinition.IndexKeyAttributeUUIDs.Find(idx => idx == attr.UUID);

                    if (index != null && item.IndexKeyDefinition.IndexKeyAttributeUUIDs.Count == 1)
                    {
                        idxToDelete.Add(item);
                    }
                }

                //remove indices
                foreach(var idx in idxToDelete)
                {
                    var remExcept = graphDBType.RemoveIndex(idx.IndexName, idx.IndexEdition, dbContext.DBTypeManager);

                    if(!remExcept.Success())
                    {
                        retExcept.AddErrorsAndWarnings(remExcept);
                    }
                }

                idxToDelete.Clear();
                graphDBType.RemoveAttribute(attr.UUID);
                
                var flushExcept = dbContext.DBTypeManager.FlushType(graphDBType);

                if (!flushExcept.Success())
                {
                    retExcept.AddErrorsAndWarnings(flushExcept);
                }
            }

            return retExcept;
        }

        public override SelectionResultSet CreateReadout(DBContext dbContext, GraphDBType graphDBType)
        {
            return base.CreateReadout(dbContext, graphDBType);
        }
    }


    #endregion    

}
