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


/* <id name="sones GraphDB – AlterTypeNode astnode" />
 * <copyright file="AlterTypeNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an Alter Type statement.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;

using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Frameworks.Irony.Scripting.Ast;

using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.Lib.DataStructures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphFS.Transactions;
using sones.Lib;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements
{
    /// <summary>
    /// This node is requested in case of an Alter Type statement.
    /// </summary>
    class AlterTypeNode : AStatement
    {

        #region Data

        String _TypeName = ""; //the name of the type that should be altered
        AlterCommandNode _AlterCmdNode = null; //what should be done with the type
        UniqueAttributesOptNode _UniqueAttributes = null;
        MandatoryOptNode _MandatoryAttributes = null;
        TypesOfAlterCmd _AlterCmdType;

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "AlterType"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region constructor

        public AlterTypeNode()
        {
            
        }

        #endregion

        #region public AStatement methods

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession, DBContext dbContext)
        {

            #region Data

            GraphDBType atype = null;
            QueryResult qr = new QueryResult();

            #endregion

            using (var transaction = graphDBSession.BeginTransaction())
            {
                var dbInnerContext = transaction.GetDBContext();
                SelectionResultSet resultReadout = null;

                #region check pandoraType

                atype = dbInnerContext.DBTypeManager.GetTypeByName(_TypeName);
                if (atype == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(_TypeName));
                }

                #endregion

                #region switch _AlterCmdType

                switch (_AlterCmdType)
                {
                    case TypesOfAlterCmd.Add:

                        #region add

                        var result = ProcessAddAttribute(atype, dbInnerContext);
                        if (!result.Success)
                        {
                            return new QueryResult(result);
                        }
                        else
                        {
                            #region generate result
                            Dictionary<String, Object> payload = new Dictionary<string, object>();
                            List<DBObjectReadout> attributeReadouts = new List<DBObjectReadout>();
                            payload.Add("TYPE", _TypeName);

                            if (_AlterCmdNode.BackwardEdgeInformation.IsNotNullOrEmpty())
                            {
                                #region backwardEdge

                                payload.Add("ACTION", "ADD BACKWARDEDGES");

                                foreach (var aAddedBackwardEdge in _AlterCmdNode.BackwardEdgeInformation)
                                {

                                    var payloadPerBackwardEdge = new Dictionary<string, object>();
                                    var type = dbInnerContext.DBTypeManager.GetTypeByName(aAddedBackwardEdge.TypeName);
                                    
                                    payloadPerBackwardEdge.Add("NAME", aAddedBackwardEdge.AttributeName);
                                    payloadPerBackwardEdge.Add("TYPE", type);
                                    payloadPerBackwardEdge.Add("ATTRIBUTE", type.GetTypeAttributeByName(aAddedBackwardEdge.TypeAttributeName));

                                    attributeReadouts.Add(new DBObjectReadout(payloadPerBackwardEdge));

                                }

                                payload.Add("BACKWARDEDGES", new Edge(attributeReadouts));

                                #endregion
                            }
                            else
                            {
                                #region attributes

                                payload.Add("ACTION", "ADD ATTRIBUTES");

                                foreach (var aAddedAttribute in (List<AttributeDefinitionNode>)_AlterCmdNode.Value)
                                {
                                    var payloadPerAttribute = new Dictionary<String, Object>();

                                    payloadPerAttribute.Add("NAME", aAddedAttribute.Name);
                                    payloadPerAttribute.Add("TYPE", dbInnerContext.DBTypeManager.GetTypeByName(aAddedAttribute.Type));

                                    attributeReadouts.Add(new DBObjectReadout(payloadPerAttribute));
                                }

                                payload.Add("ATTRIBUTES", new Edge(attributeReadouts));

                                #endregion
                            }

                            resultReadout = new SelectionResultSet(new DBObjectReadout(payload));


                            #endregion
                        }

                        #endregion

                        break;

                    case TypesOfAlterCmd.Drop:

                        qr = ProcessDropAttribute(atype, dbInnerContext);

                        break;

                    case TypesOfAlterCmd.RenameAttribute:

                        qr = new QueryResult(ProcessRenameAttribute(atype, dbInnerContext));

                        if (qr.ResultType == ResultType.Successful)
                        {
                            qr.AddResult(CreateRenameResult("RENAME ATTRIBUTE", ((KeyValuePair<String, String>)_AlterCmdNode.Value).Key, ((KeyValuePair<String, String>)_AlterCmdNode.Value).Value, atype));
                        }

                        break;

                    case TypesOfAlterCmd.RenameType:                        
                        
                        qr = new QueryResult(ProcessRenameType(atype, dbInnerContext));

                        if (qr.ResultType == ResultType.Successful)
                        {
                            qr.AddResult(CreateRenameResult("RENAME TYPE", _TypeName, (String)_AlterCmdNode.Value, atype));
                        }

                        break;

                    case TypesOfAlterCmd.RenameBackwardedge:

                        qr = new QueryResult(ProcessRenameBackwardEdge(atype, dbInnerContext));

                        if (qr.ResultType == ResultType.Successful)
                        {
                            qr.AddResult(CreateRenameResult("RENAME BACKWARDEDGE", ((KeyValuePair<String, String>)_AlterCmdNode.Value).Key, ((KeyValuePair<String, String>)_AlterCmdNode.Value).Value, atype));
                        }

                        break;

                    case TypesOfAlterCmd.Unqiue:

                        qr = ProcessUniqueAttributes(atype, dbInnerContext.DBTypeManager);

                        break;

                    case TypesOfAlterCmd.DropUnqiue:

                        qr = ProcessDropUniqueAttributes(atype, dbInnerContext.DBTypeManager);

                        break;

                    case TypesOfAlterCmd.Mandatory:

                        qr = ProcessMandatoryAttributes(atype, dbInnerContext.DBTypeManager);

                        break;

                    case TypesOfAlterCmd.DropMandatory:

                        qr = ProcessDropMandatoryAttributes(atype, dbInnerContext.DBTypeManager);

                        break;

                    case TypesOfAlterCmd.ChangeComment:

                        qr = new QueryResult(ProcessChangeCommentOnType(atype, dbInnerContext));

                        if (qr.ResultType == ResultType.Successful)
                        {
                            qr.AddResult(CreateChangeCommentResult("CHANGE COMMENT", (String)_AlterCmdNode.Value, atype));
                        }

                        break;

                    default:

                        throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }
                
                #endregion

                #region Commit transaction and add all Warnings and Errors

                qr.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                if ((qr.ResultType == ResultType.Successful) && (resultReadout != null))
                {
                    qr.AddResult(resultReadout);
                }

                return qr;
            }
        }

        private SelectionResultSet CreateChangeCommentResult(string myAlterAction, string myNewComment, GraphDBType myType)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add("TYPE", myType);
            payload.Add("ACTION", myAlterAction);
            payload.Add("NEW COMMENT", myNewComment);

            return new SelectionResultSet(new DBObjectReadout(payload));

        }

        private SelectionResultSet CreateRenameResult(string myAlterAction, string myFromString, string myToString, GraphDBType myType)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add("TYPE", myType);
            payload.Add("ACTION", myAlterAction);
            payload.Add("FROM", myFromString);
            payload.Add("TO", myToString);

            return new SelectionResultSet(new DBObjectReadout(payload));

        }

        private Exceptional ProcessChangeCommentOnType(GraphDBType atype, DBContext dbContext)
        {

            var newName = (String)_AlterCmdNode.Value;

            return dbContext.DBTypeManager.ChangeCommentOnType(atype, newName);

        }

        /// <summary>
        /// Gets the content of a AlterTypeNode.
        /// </summary>
        /// <param name="myCompilerContext">CompilerContext of Irony.</param>
        /// <param name="myParseTreeNode">The current ParseNode.</param>
        /// <param name="myTypeManager">The TypeManager of the PandoraDB.</param>
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            var dbContext = myCompilerContext.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            try 
            {
                _TypeName = myParseTreeNode.ChildNodes[2].Token.ValueString;

                if(myParseTreeNode.ChildNodes[3].HasChildNodes())
                {
                    _AlterCmdNode = (AlterCommandNode)myParseTreeNode.ChildNodes[3].AstNode;
                    _AlterCmdType = _AlterCmdNode.TypeOfAlterCmd;

                    if (_AlterCmdType == TypesOfAlterCmd.Add)
                        CheckForSetConstraint(myParseTreeNode.ChildNodes[3].ChildNodes[2], _TypeName, dbContext);
                }

                if(myParseTreeNode.ChildNodes[4].HasChildNodes())
                {
                    _UniqueAttributes = (UniqueAttributesOptNode)myParseTreeNode.ChildNodes[4].AstNode;
                    _AlterCmdType = TypesOfAlterCmd.Unqiue;
                }

                if (myParseTreeNode.ChildNodes[5].HasChildNodes())
                {
                    _MandatoryAttributes = (MandatoryOptNode)myParseTreeNode.ChildNodes[5].AstNode;
                    _AlterCmdType = TypesOfAlterCmd.Mandatory;
                }
            }catch(GraphDBException e)
            {
                throw new GraphDBException(e.GraphDBErrors);
            }

        }

        #endregion

        #region private helper methods

        private Boolean CheckForSet(ParseTreeNode aChildNode)
        {
            if (aChildNode.HasChildNodes())
            {
                if (aChildNode.ChildNodes[0] != null)
                {
                    if (aChildNode.ChildNodes[0].HasChildNodes())
                    {
                        if (aChildNode.ChildNodes[0].ChildNodes[0] != null)
                        {
                            if (aChildNode.ChildNodes[0].ChildNodes[0].HasChildNodes())
                            {
                                if (aChildNode.ChildNodes[0].ChildNodes[0].ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.SET)
                                    return true;
                            }
                            else if (aChildNode.ChildNodes[0].ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.SET)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// check for set constrain attributes in the alter type add attribute statement
        /// </summary>
        /// <param name="aAttrDefNode">the interessting attribute</param>
        /// <param name="dbContext">the typemanager</param>
        /// <param name="myTypeName">current type name</param>
        private void CheckForSetConstraint(ParseTreeNode aAttrDefNode, string myTypeName, DBContext dbContext)
        {
            GraphDBType dbType = null;
            AttributeDefinitionNode attrDef = null;

            foreach (var item in aAttrDefNode.ChildNodes)
            {
                if (item.AstNode is AttributeDefinitionNode)
                {
                    attrDef = (AttributeDefinitionNode)item.AstNode;

                    if (attrDef.Type != myTypeName)
                    {
                        dbType = dbContext.DBTypeManager.GetTypeByName(attrDef.Type);

                        if (dbType == null)
                            throw new GraphDBException(new Error_TypeDoesNotExist(attrDef.Type));

                        if (attrDef.TypeAttribute.KindOfType == KindsOfType.SetOfReferences)
                        {
                            if (!CheckForSet(item))
                                throw new GraphDBException(new Error_ListAttributeNotAllowed(myTypeName));
                        }

                    }
                    else if (attrDef.Type == myTypeName && (attrDef.TypeAttribute.KindOfType == KindsOfType.ListOfNoneReferences) || (attrDef.TypeAttribute.KindOfType == KindsOfType.SetOfNoneReferences))
                    {
                        if (!CheckForSet(item))
                            throw new GraphDBException(new Error_ListAttributeNotAllowed(myTypeName));
                    }
                }
            }
        }

        private QueryResult ProcessDropUniqueAttributes(GraphDBType atype, DBTypeManager myTypeManager)
        {
            var pResult = atype.DropUniqueAttributes(myTypeManager);

            return new QueryResult(pResult);

        }

        private QueryResult ProcessUniqueAttributes(GraphDBType atype, DBTypeManager myTypeManager)
        {
            List<String> attributes = (List<String>)_UniqueAttributes.UniqueAttributes;

            var pResult = atype.ChangeUniqueAttributes(attributes, myTypeManager);

            return new QueryResult(pResult);
            
        }

        private QueryResult ProcessMandatoryAttributes(GraphDBType atype, DBTypeManager myTypeManager)
        {
            List<string> attributes = (List<string>)_MandatoryAttributes.MandatoryAttribs;

            var pResult = atype.ChangeMandatoryAttributes(attributes, myTypeManager);

            return new QueryResult(pResult);
        }

        private QueryResult ProcessDropMandatoryAttributes(GraphDBType atype, DBTypeManager myTypeManager)
        {
            var pResult = atype.DropUniqueAttributes(myTypeManager);

            return new QueryResult(pResult);
        }

        /// <summary>
        /// Executes the renaming of the attribute of a given type.
        /// </summary>
        /// <param name="atype">The corresponding PandoraType.</param>
        /// <param name="dbContext">The TypeManager of the PandoraDB.</param>
        /// <returns>A QueryResult.</returns>
        private Exceptional<Boolean> ProcessRenameAttribute(GraphDBType atype, DBContext dbContext)
        {
            #region data

            KeyValuePair<String, String> oldAndNewName = (KeyValuePair<String, String>)_AlterCmdNode.Value;

            #endregion

            return dbContext.DBTypeManager.RenameAttributeOfType(atype, oldAndNewName.Key, oldAndNewName.Value);
        }

        /// <summary>
        /// Execute the renaming of the backwardedge of a given type.
        /// </summary>
        /// <param name="atype">The corresponding PandoraType.</param>
        /// <param name="dbContext">The TypeManager of the PandoraDB.</param>
        /// <returns></returns>
        private Exceptional<Boolean> ProcessRenameBackwardEdge(GraphDBType atype, DBContext dbContext)
        {
            KeyValuePair<String, String> oldNewName = (KeyValuePair<String, String>)_AlterCmdNode.Value;

            TypeAttribute Attribute = atype.GetTypeAttributeByName(oldNewName.Key);

            if (Attribute == null)
                return new Exceptional<Boolean>(new Error_AttributeDoesNotExists(oldNewName.Key));

            return atype.RenameBackwardedge(Attribute, oldNewName.Value, dbContext.DBTypeManager);
        }

        /// <summary>
        /// Execute the renaming of a given type.
        /// </summary>
        /// <param name="atype">The corresponding PandoraType.</param>
        /// <param name="dbContext">typeManager</param>
        /// <returns>A QueryResult</returns>
        private Exceptional<Boolean> ProcessRenameType(GraphDBType atype, DBContext dbContext)
        {         
            String newName = (String)_AlterCmdNode.Value;

            return dbContext.DBTypeManager.RenameType(atype, newName);
        }

        /// <summary>
        /// Executes the removal of certain myAttributes.
        /// </summary>
        /// <param name="atype">The PandoraType that should be cleaned of some myAttributes.</param>
        /// <param name="typeManager">The TypeManager of the PandoraDB.</param>
        /// <returns>A QueryResult.</returns>
        private QueryResult ProcessDropAttribute(GraphDBType myPandoraType, DBContext dbContext)
        {

            #region Data

            List<String> listOfToBeDroppedAttributes = (List<String>)_AlterCmdNode.Value;

            #endregion

            foreach (String aAttributeName in listOfToBeDroppedAttributes)
            {

                //Hack: remove myAttributes in DBObjects
                var aTempResult = dbContext.DBTypeManager.RemoveAttributeFromType(_TypeName, aAttributeName, dbContext.DBTypeManager);

                if (aTempResult.Value != ResultType.Successful)
                {
                    return new QueryResult(new Exceptional<ResultType>(aTempResult));
                }
            }

            return new QueryResult(new Exceptional<ResultType>(ResultType.Successful));

        }


        /// <summary>
        /// Adds myAttributes to a certain PandoraType.
        /// </summary>
        /// <param name="atype">The PandoraType that should be added some myAttributes.</param>
        /// <param name="dbContext">The TypeManager of the PandoraDB.</param>
        /// <returns>A QueryResult.</returns>
        private Exceptional<ResultType> ProcessAddAttribute(GraphDBType atype, DBContext dbContext)
        {
            #region Data

            List<AttributeDefinitionNode> listOfToBeAddedAttributes = (List<AttributeDefinitionNode>)_AlterCmdNode.Value;
            Exceptional<ResultType> aResult = new Exceptional<ResultType>(ResultType.Successful);

            #endregion

            #region check type for myAttributes

            foreach (AttributeDefinitionNode aAttributeDefinition in listOfToBeAddedAttributes)
            {
                if (atype.GetTypeAttributeByName(aAttributeDefinition.Name) != null)
                {
                    var aError = new Error_AttributeAlreadyExists(aAttributeDefinition.Name);

                    return new Exceptional<ResultType>(aError);
                }
            }

            #endregion

            #region add myAttributes

            foreach (AttributeDefinitionNode aAttributeDefinition in listOfToBeAddedAttributes)
            {
                try
                {
                    var theType = dbContext.DBTypeManager.GetTypeByName(aAttributeDefinition.Type);
                    aAttributeDefinition.TypeAttribute.DBTypeUUID = theType.UUID;
                    aAttributeDefinition.TypeAttribute.RelatedGraphDBTypeUUID = atype.UUID;

                    #region EdgeType
                    if (aAttributeDefinition.TypeAttribute.EdgeType == null)
                    {
                        #region we had not defined a special EdgeType - for single reference attributes we need to set the EdgeTypeSingle NOW!
                        if (aAttributeDefinition.TypeAttribute.KindOfType == KindsOfType.SingleReference)
                            aAttributeDefinition.TypeAttribute.EdgeType = new EdgeTypeSingleReference();
                        
                        if (aAttributeDefinition.TypeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                            aAttributeDefinition.TypeAttribute.EdgeType = new EdgeTypeListOfBaseObjects();

                        if (aAttributeDefinition.TypeAttribute.KindOfType == KindsOfType.SetOfReferences || aAttributeDefinition.TypeAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                            aAttributeDefinition.TypeAttribute.EdgeType = new EdgeTypeSetOfReferences();

                        #endregion
                    }
                    #endregion

                    var aTempResult = dbContext.DBTypeManager.AddAttributeToType(_TypeName, aAttributeDefinition.Name, aAttributeDefinition.TypeAttribute);

                    if (aTempResult.Value != ResultType.Successful)
                    {
                        return aTempResult;
                    }
                }
                catch (Exception e)
                {
                    var aError = new Error_UnknownDBError(e);

                    return new Exceptional<ResultType>(aError);
                }
            }

            #endregion


            if (_AlterCmdNode.BackwardEdgeInformation != null)
            {
                foreach (var tuple in _AlterCmdNode.BackwardEdgeInformation)
                {
                    //GraphDBType edgeType = typeManager.GetTypeByName(tuple.TypeName);
                    //TypeAttribute edgeAttribute = edgeType.GetTypeAttributeByName(tuple.TypeAttributeName);

                    var typeAttribute = dbContext.DBTypeManager.CreateBackwardEdgeAttribute(tuple, atype);

                    if (typeAttribute.Failed)
                    {
                        aResult = new Exceptional<ResultType>(typeAttribute);
                        return aResult;
                    }

                    // TODO: go through all DB Objects of the _TypeName and change the implicit backward edge type to the new ta.EdgeType
                    //       if the DBObject has one!

                    try
                    {
                        var aTempResult = dbContext.DBTypeManager.AddAttributeToType(_TypeName, tuple.AttributeName, typeAttribute.Value);

                        if (aTempResult.Value != ResultType.Successful)
                        {
                            return aTempResult;
                        }
                    }
                    catch (Exception e)
                    {
                        return new Exceptional<ResultType>(new Error_UnknownDBError(e));
                    }
                }
            }

            return aResult;
        }

        /// <summary>
        /// Gets an attributeList from a ParseTreeNode.
        /// </summary>
        /// <param name="aChildNode">The interesting ParseTreeNode.</param>
        /// <returns>A Dictionary with attribute definitions.</returns>
        private Dictionary<String, String> GetAttributeList(ParseTreeNode aChildNode)
        {
            #region Data

            Dictionary<String, String> attributes = new Dictionary<string, string>();

            #endregion

            foreach (ParseTreeNode aAttrDefNode in aChildNode.ChildNodes)
            {
                AttributeDefinitionNode aAttrDef = (AttributeDefinitionNode)aAttrDefNode.AstNode;
                attributes.Add(aAttrDef.Name, aAttrDef.Type);
            }

            return attributes;
        }

        #endregion

    }
}
