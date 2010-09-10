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

/* <id name="PandoraDB – ID node" />
 * <copyright file="IDNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an ID statement. This might be something like U.Name or Name or U.$GUID or U.Friends.Name. It is necessary to execute an AType node (or TypeWrapper) in previous.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.Lib.Session;
using sones.Lib;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.Enums;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{

    public abstract class AIDNodePart
    {

    }

    public class IDNodeEdge : AIDNodePart
    {
        public EdgeKey EdgeKey { get; set; }
    }
    public class IDNodeFunc : AIDNodePart
    {
        public FuncCallNode FuncCallNode { get; set; }
    }

    /// <summary>
    /// This node is requested in case of an ID statement. This might be something like U.Name or Name or U.$GUID or U.Friends.Name.
    /// It is necessary to execute an AType node (or TypeWrapper) in previous.
    /// </summary>
    public class IDNode : AStructureNode
    {

        #region Data

        #region Level

        private int _level = 0;

        /// <summary>
        /// Shows the level of the IDNode.
        /// "U.Name" would be 0 and "U.Friends.Name" is 1.
        /// Something like U.Friends is still level 0 because all hereinafter algorythms work
        /// on the list-attribute "Friends" instead of the real User-Objects in 
        /// the Friends list. The other case is "U.Friends.Name" where one explicitly wants to
        /// process the User-objects in the Friends-List.
        /// </summary>
        public int Level
        {
            get 
            { 
                //if(_isValidated)
                //{
                    return _level; 
                //}
                //else
                //{
                //    if (!String.IsNullOrEmpty(_IDNodeString))
                //        throw new GraphDBException(new Error_InvalidIDNode(_IDNodeString));
                //    else
                //        throw new GraphDBException(new Error_InvalidIDNode("It is not allowed to access informations on an IDNode that has not been validated."));
                //}
            }
        }

        #endregion

        #region Edges

        private List<EdgeKey> _edges;

        /// <summary>
        /// List of EdgeKeys
        /// </summary>
        public List<EdgeKey> Edges
        {
            get 
            {
                if (_isValidated)
                {
                    return _edges;
                }
                else
                {
                    if (!String.IsNullOrEmpty(_IDNodeString))
                    {
                        if (_lastError != null)
                            throw new GraphDBException(_lastError);
                        else
                            throw new GraphDBException(new Error_InvalidIDNode(_IDNodeString));
                    }
                    else
                        throw new GraphDBException(new Error_InvalidIDNode("It is not allowed to access informations on an IDNode that has not been validated."));
                }
            }
        }

        #endregion

        #region Depth

        public Int32 Depth
        {
            get
            {
                if (Edges.IsNullOrEmpty())
                    return 0;
                else if (Edges.Count == 1 && Edges[0].AttrUUID == null)
                    return 0;
                else
                    return Edges.Count;
            }
        }

        #endregion

        private int _hashCode;

        private String _IDNodeString = "";

        #region Reference

        private Tuple<String, GraphDBType> _reference;

        /// <summary>
        /// The reference of the IDNode.
        /// TupleElement1 is sth like "U" or "User" and TupleElement2 is the DBTypeStream of "User").
        /// </summary>
        public Tuple<String, GraphDBType> Reference
        {
            get 
            {
                //if(_isValidated)
                //{
                    return _reference; 
                //}
                //else
                //{
                //    if (!String.IsNullOrEmpty(_IDNodeString))
                //    {
                //        if(_lastError != null)
                //            throw new GraphDBException(_lastError);
                //        else
                //            throw new GraphDBException(new Error_InvalidIDNode(_IDNodeString));
                //    }
                //    else
                //        throw new GraphDBException(new Error_InvalidIDNode("It is not allowed to access informations on an IDNode that has not been validated."));
                //}
            }
        }

        #endregion

        #region LastType

        private GraphDBType _lastType = null;

        /// <summary>
        /// This is the last type of the IDNode. This might be "User" 
        /// in case of "U.Friends.Age" and means that the attribute "Age" is 
        /// realted to the type "User". This is important for index operations on 
        /// inheritated attributes.
        /// </summary>
        public GraphDBType LastType
        {
            get { return _lastType; }
        }

        #endregion

        #region LastInvalidAttribute

        private GraphDBError _lastError;

        public GraphDBError LastError
        {
            get { return _lastError; }
        }

        #endregion

        #region LastAttribute

        private TypeAttribute _lastAttribute = null;

        /// <summary>
        /// The TypeAttribute of the last attribute in the IDNode
        /// </summary>
        public TypeAttribute LastAttribute
        {
            get 
            {
                if (_isValidated)
                {
                    return _lastAttribute;
                }
                else
                {
                    if (!String.IsNullOrEmpty(_IDNodeString))
                    {
                        if(_lastError != null)
                            throw new GraphDBException(_lastError);
                        else
                            throw new GraphDBException(new Error_InvalidIDNode(_IDNodeString));
                    }
                    else
                        throw new GraphDBException(new Error_InvalidIDNode("It is not allowed to access informations on an IDNode that has not been validated."));
                }
            }
            set
            {
                _lastAttribute = value;

                #region recalculate hashcode and set edges

                _edges.Last().SetAttributeUUID(_lastAttribute.UUID);

                _hashCode = 0;

                _edges.ForEach(item => _hashCode = _hashCode ^ item.GetHashCode());

                #endregion
            }
        }

        #endregion

        #region IsValidated

        private Boolean _isValidated = true;

        /// <summary>
        /// Determines if the IDNode is a valid one (valid edges, level, lastAttribute...)
        /// </summary>
        public Boolean IsValidated
        {
            get { return _isValidated; }
            set { _isValidated = value; }
        }

        #endregion

        #region IsAsteriskSet

        private Boolean _isAsteriskSet = false;

        /// <summary>
        /// Determines if the IDNode is a valid one (valid edges, level, lastAttribute...)
        /// </summary>
        public Boolean IsAsteriskSet
        {
            get { return _isAsteriskSet; }
        }

        #endregion

        #region IDNodeParts

        private List<AIDNodePart> _IDNodeParts;
        /// <summary>
        /// This will store all parts of the IDNode
        /// U.Friends.TOP(3).* will result in 4 entries.
        /// </summary>
        public List<AIDNodePart> IDNodeParts
        {
            get { return _IDNodeParts; }
        }

        #endregion

        private List<ParseTreeNode> _invalidNodeParts = new List<ParseTreeNode>();

        private SessionSettings _sessionToken;

        #endregion

        #region constructor

        public IDNode()
        {
            _hashCode = 0;
            _edges = new List<EdgeKey>();
            _IDNodeParts = new List<AIDNodePart>();
        
        }

        public IDNode(GraphDBType myType, String myReference, SessionSettings mySessionToken)
        {
            _isValidated = true;
            _sessionToken = mySessionToken;
            _lastAttribute = null;
            _lastType = myType;
            _reference = new Tuple<string, GraphDBType>(myReference, myType);
            _edges = new List<EdgeKey>();
            _edges.Add(new EdgeKey(myType.UUID, null));
            _IDNodeParts = new List<AIDNodePart>();
            _IDNodeParts.Add(new IDNodeEdge() { EdgeKey = new EdgeKey(myType.UUID, null) });
            _level = 0;
            _isAsteriskSet = true;
            _hashCode = _hashCode ^ _edges[0].GetHashCode();
        }

        #endregion
        
        /// <summary>
        /// This method extracts information of irony child nodes.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parseNode"></param>
        /// <param name="myTypeManager"></param>
        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            //extract _IDNOdeString
            ExtractIDNodeString(parseNode);

            //get the session token from context. this is necessary for setting-attribute extraction.
            _sessionToken = dbContext.SessionSettings;

            //there has to be at least one element, otherwise the IDNode would not have been called.

            //U.Name or U.Friends.Age or Name
            AddFirstEdge(context, parseNode, dbContext);

            if (_isValidated)
            {
                //add the the rest of the edges
                AddEdges(parseNode.ChildNodes.Skip(1), dbContext, _lastType);
            }
        }

        /// <summary>
        /// This method validates an invalid IDNode and in case of a valid one the edges are extracted.
        /// </summary>
        /// <param name="myStartingType">The starting type.</param>
        /// <param name="myTypeManager">The TypeManager of the database.</param>
        public Exceptional<bool> ValidateMe(GraphDBType myStartingType, DBContext myTypeManager)
        {
            #region input exceptions

            if ((myStartingType == null) || (myTypeManager == null))
            {
                throw new GraphDBException(new Error_ArgumentNullOrEmpty("myStartingType or TypeManager"));
            }

            #endregion

            if (!_isValidated)
            {
                //Case 1: No validation happend before.
                #region Case 1

                #region data

                String attributeName = String.Empty;
                TypeAttribute tempTypeAttribute;

                #endregion

                var nodePart = _invalidNodeParts.First();

                if (nodePart.AstNode != null)
                {
                    #region traversal

                    if (!(nodePart.AstNode is EdgeTraversalNode))
                    {
                        return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    attributeName = ((EdgeTraversalNode)nodePart.AstNode).AttributeName;

                    #endregion
                }
                else
                {
                    #region simple string

                    attributeName = nodePart.Token.ValueString;

                    #endregion
                }

                tempTypeAttribute = myStartingType.GetTypeAttributeByName(attributeName);

                if (tempTypeAttribute != null)
                {
                    _level = 0;
                    _edges.Add(new EdgeKey(myStartingType.UUID, tempTypeAttribute.UUID));
                    _IDNodeParts.Add(new IDNodeEdge() { EdgeKey = new EdgeKey(myStartingType.UUID, tempTypeAttribute.UUID) });
                    _lastAttribute = tempTypeAttribute;
                    _lastType = myStartingType;

                    _reference = new Tuple<string, GraphDBType>(myStartingType.Name, myStartingType); //T1 -->key in context dictionary
                }
                else
                {
                    //we are done. INVALID IDNODE
                    return new Exceptional<bool>(new Error_AttributeDoesNotExists(myStartingType.Name, attributeName));
                }

                AddEdges(_invalidNodeParts.Skip(1), myTypeManager, myStartingType);

                _isValidated = true;


                #endregion
            }
            // even if it was validate it could be wrong. In the case, the attribute exist in an PandoraListOfReferences type AND in the type of an
            // CollectionofDBObjectsNode than the wrong type is picked! In this case the type needs to be updated
            // check out [NewQLtest_IDNode_DifferentTypesWithSameAttributeName] test query "UPDATE SimpsonProfile SET (ADD TO  Auftritt SETOF(Name='HABF22',Name='HABF23')) WHERE Name='Bart Simpson'"
            // the type of "Name='HABF22'" should be "Auftritt" but "SimpsonProfile" is used!
            else
            {
                var tempTypeAttribute = myStartingType.GetTypeAttributeByName(_lastAttribute.Name);

                if (myStartingType != _lastType && tempTypeAttribute != null)
                {
                    _level = 0;
                    _edges = new List<EdgeKey>();
                    _edges.Add(new EdgeKey(myStartingType.UUID, tempTypeAttribute.UUID));
                    _IDNodeParts = new List<AIDNodePart>();
                    _IDNodeParts.Add(new IDNodeEdge() { EdgeKey = new EdgeKey(myStartingType.UUID, tempTypeAttribute.UUID) });
                    _lastAttribute = tempTypeAttribute;
                    _lastType = myStartingType;

                    _reference = new Tuple<string, GraphDBType>(myStartingType.Name, myStartingType); //T1 -->key in context dictionary
                }
            }

            return new Exceptional<bool>(true);
        }

        public override string ToString()
        {
            return _IDNodeString;
        }

        #region private helper methods

        /// <summary>
        /// This method extracts the IDNodeString from the irony parse tree
        /// </summary>
        /// <param name="parseNode">A ParseTree node.</param>
        /// <returns>The IDNode String.</returns>
        private void ExtractIDNodeString(ParseTreeNode parseNode)
        {
            foreach (var aChildNode in parseNode.ChildNodes)
            {
                if (aChildNode.AstNode == null)
                {
                    _IDNodeString += aChildNode.Token.ValueString;
                }
                else
                {
                    if (aChildNode.AstNode is EdgeTraversalNode)
                    {
                        var aEdgeTraversalNode = (EdgeTraversalNode)aChildNode.AstNode;

                        _IDNodeString += aEdgeTraversalNode.Delimiter.GetDelimiterString();

                        if (aEdgeTraversalNode.FuncCall != null)
                        {
                            _IDNodeString += aEdgeTraversalNode.FuncCall.SourceParsedString;
                        }
                        else
                        {
                            _IDNodeString += aEdgeTraversalNode.AttributeName;
                        }
                    }
                    else
                    {
                        if (aChildNode.AstNode is FuncCallNode)
                        {
                            _IDNodeString += ((FuncCallNode)aChildNode.AstNode).SourceParsedString;
                        }
                        else
                        {
                            if (aChildNode.AstNode is EdgeInformationNode)
                            {
                                var aEdgeInformationNode = (EdgeInformationNode)aChildNode.AstNode;

                                _IDNodeString += aEdgeInformationNode.Delimiter.GetDelimiterString();

                                _IDNodeString += aEdgeInformationNode.EdgeInformationName;
                            }
                            else
                            {
                                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method adds and validates the first edge.
        /// </summary>
        /// <param name="context">The irony compiler context.</param>
        /// <param name="parseNode">The first irony parse node.</param>
        /// <param name="dbContext">The TypeManager of the database.</param>
        private void AddFirstEdge(CompilerContext context, ParseTreeNode parseNode, DBContext dbContext)
        {
            if (_edges.Count == 0)
            {
                #region Data

                #region functions without a caller like CurrentDate()

                if (parseNode.ChildNodes[0].AstNode is FuncCallNode)
                {
                    if (IsValidAggregateOrFunction(dbContext.DBTypeManager, parseNode.ChildNodes[0].AstNode as FuncCallNode))
                    {
                        //validated
                        _IDNodeParts.Add(new IDNodeFunc() { FuncCallNode = (parseNode.ChildNodes[0].AstNode as FuncCallNode) });
                        return;
                    }
                }

                #endregion

                String tempTypeOrAttributeName = String.Empty;

                tempTypeOrAttributeName = parseNode.ChildNodes[0].Token.ValueString;

                GraphDBType tempType = null;
                TypeAttribute tempTypeAttribute = null;

                #endregion

                //there always have to be an type context for this node
                if (context.PandoraListOfReferences == null)
                {
                    if (!String.IsNullOrEmpty(_IDNodeString))
                        throw new GraphDBException(new Error_InvalidIDNode(_IDNodeString));
                    else
                        throw new GraphDBException(new Error_InvalidIDNode("The IDNode has been called without calling a type in previous."));
                }

                if (context.PandoraListOfReferences.ContainsKey(tempTypeOrAttributeName))
                {

                    //Case 1: sth like "U" (where U stands for User)
                    #region Case 1

                    tempType = (GraphDBType)((ATypeNode)context.PandoraListOfReferences[tempTypeOrAttributeName]).DBTypeStream;

                    _lastType = tempType;
                    _reference = new Tuple<string, GraphDBType>(tempTypeOrAttributeName, tempType);

                    _level = 0;//we are in level 0

                    #endregion

                    //Case 2: an attribute like "Car" (of "User") is used and in parallel a similar written Type exists. In this case
                    // it is necessary to throw an error.
                    #region Case 2

                    var typesWithAttibute = (from aContextType in context.PandoraListOfReferences where (aContextType.Key != tempTypeOrAttributeName) && (((GraphDBType)(((ATypeNode)aContextType.Value).DBTypeStream)).GetTypeAttributeByName("tempTypeOrAttributeName") != null) select aContextType.Key).FirstOrDefault();

                    if (typesWithAttibute != null)
                    {
                        _lastError = new Error_AttributeDoesNotExists(tempTypeOrAttributeName);
                        IsValidated = false;
                    }

                    #endregion

                    _edges.ForEach(item => _hashCode = _hashCode ^ item.GetHashCode());
                }
                else
                {

                    //Case 3: 
                    //  (3.1)In this case it must be an attribute. If it is used in an ambigous way, an exception would be thrown.
                    //  (3.2)In this case it can be an attribute of another type (i.e. the Color attribute in 
                    //  "INSERT INTO User VALUES (Name = 'Fry', UserID = 12, Age = 22, Car = SETREF ( Color = 'red' ))" --> it is not an attribute of User but of Car).
                    //  Because of the adverse fact that it is not possible to find out about the real type in this time/place we have 
                    //  to validate the IDNode again :(

                    #region case 3

                    //sth like Name --> we have to find out the corresponding type
                    Boolean foundSth = false;
                    String reference = null;
                    foreach (var contextElement in context.PandoraListOfReferences)
                    {
                        if (contextElement.Value is ATypeNode)
                        {
                            if (((ATypeNode)contextElement.Value).DBTypeStream is GraphDBType)
                            {
                                tempType = (GraphDBType)((ATypeNode)contextElement.Value).DBTypeStream;
                                reference = contextElement.Key;


                                tempTypeAttribute = (from aAttribute in tempType.AttributeLookupTable where aAttribute.Value.Name == tempTypeOrAttributeName select aAttribute.Value).FirstOrDefault();

                                if (tempTypeAttribute != null)
                                {
                                    if (foundSth == true)
                                    {
                                        throw new GraphDBException(new Error_UnknownDBError("The attribute or type \"" + tempTypeOrAttributeName + "\" has been used ambigous."));
                                    }
                                    else
                                    {
                                        foundSth = true;

                                        if (tempTypeAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined || tempTypeAttribute.IsBackwardEdge)
                                        {
                                            _level++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }

                    if (foundSth)
                    {

                        #region 3.1

                        _edges.Add(new EdgeKey(tempType.UUID, tempTypeAttribute.UUID));
                        _IDNodeParts.Add(new IDNodeEdge() { EdgeKey = new EdgeKey(tempType.UUID, tempTypeAttribute.UUID) });
                        _lastAttribute = tempTypeAttribute;
                        _lastType = tempType;
                        _reference = new Tuple<string, GraphDBType>(reference, tempType); //T1 -->key in context dictionary

                        _edges.ForEach(item => _hashCode = _hashCode ^ item.GetHashCode());

                        #endregion

                    }
                    else
                    {

                        #region 3.2

                        _isValidated = false;

                        _invalidNodeParts.AddRange(parseNode.ChildNodes);

                        #endregion

                    }

                    #endregion
                }
            }
            else
            {
                throw new GraphDBException(new Error_UnknownDBError("Do not use the AddFirstEdge method with an aready filled EdgeKey-list"));
            }
        }

        private bool IsValidAggregateOrFunction(DBTypeManager dBTypeManager, FuncCallNode myFunc)
        {
            if (myFunc is AggregateNode)
            {
                #region validate aggregate

                return true;

                #endregion
            }
            else
            {
                if (myFunc.Function == null)
                {
                    return true;
                }

                #region validate function

                var lastAttribute = GetLastAttribute(dBTypeManager);

                if (myFunc.Function.ValidateWorkingBase(lastAttribute, dBTypeManager))
                {
                    //validated
                    return true;
                }
                else
                {
                    //invalid
                    throw new GraphDBException(new Error_InvalidFunctionBase(lastAttribute, myFunc.Function.FunctionName));
                }
            }

                #endregion
        }

        /// <summary>
        /// This method adds and validates edges.
        /// </summary>
        /// <param name="myEdges">The Edges as String.</param>
        /// <param name="dbContext">The TypeManager of the database.</param>
        /// <param name="myStartingType">The first type of the edges.</param>
        private void AddEdges(IEnumerable<ParseTreeNode> myEdges, DBContext dbContext, GraphDBType myStartingType)
        {
            #region data

            TypeAttribute tempTypeAttribute = _lastAttribute;

            #endregion

            foreach (var aEdge in myEdges)
            {
                if (tempTypeAttribute != null)
                {
                    if (tempTypeAttribute.IsBackwardEdge)
                    {
                        myStartingType = dbContext.DBTypeManager.GetTypeByUUID(tempTypeAttribute.BackwardEdgeDefinition.TypeUUID);
                    }
                    else if (tempTypeAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                    {
                        myStartingType = tempTypeAttribute.GetDBType(dbContext.DBTypeManager);
                    }
                }

                if (aEdge.AstNode is EdgeTraversalNode)
                {
                    #region EdgeTraversal

                    var aEdgeTraversal = (EdgeTraversalNode)aEdge.AstNode;

                    if (aEdgeTraversal.FuncCall != null)
                    {
                        #region function

                        if(IsValidAggregateOrFunction(dbContext.DBTypeManager, aEdgeTraversal.FuncCall))
                        {
                            _IDNodeParts.Add(new IDNodeFunc() { FuncCallNode = aEdgeTraversal.FuncCall });
                        }

                        #endregion
                    }
                    else
                    {
                        #region attribute

                        tempTypeAttribute = myStartingType.GetTypeAttributeByName(aEdgeTraversal.AttributeName);

                        if (tempTypeAttribute == null)
                        {
                            _lastError = new Error_AttributeDoesNotExists(aEdgeTraversal.AttributeName);
                            _invalidNodeParts.Add(aEdge);
                            IsValidated = false;
                        }
                        else
                        {
                            AddNewEdgeKey(myStartingType, tempTypeAttribute.UUID);

                            if (tempTypeAttribute.IsBackwardEdge)
                            {
                                //TODO: try to avoid this special case
                                myStartingType = tempTypeAttribute.GetRelatedType(dbContext.DBTypeManager);

                                _level++;
                            }
                            else
                            {
                                myStartingType = tempTypeAttribute.GetDBType(dbContext.DBTypeManager);

                                if (tempTypeAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                                {
                                    _level++;
                                }
                            }
                        }

                        #endregion
                    }

                    #endregion
                }
                else
                {
                    if (aEdge.AstNode is EdgeInformationNode)
                    {
                        #region EdgeInformation

                        var aEdgeInformation = (EdgeInformationNode)aEdge.AstNode;

                        throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Start here integrating edgeinfos"));

                        #endregion
                    }
                    else
                    {
                        throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }
                }
            }

            _lastAttribute = tempTypeAttribute;

            if (_level == 0)
            {
                if (tempTypeAttribute == null)
                {
                    if (_isValidated)
                    {
                        _isAsteriskSet = true;

                        if (myStartingType != null)
                            _edges.Add(new EdgeKey(myStartingType.UUID, null));
                    }
                }
            }
        }

        private TypeAttribute GetLastAttribute(DBTypeManager dbTypeManager)
        {
            if (_edges.Count > 0)
            {
                if (_edges.Last().AttrUUID != null)
                {
                    return dbTypeManager.GetTypeByUUID(_edges.Last().TypeUUID).GetTypeAttributeByUUID(_edges.Last().AttrUUID);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (_invalidNodeParts.Count > 0)
                {
                    var lastInvalid = _invalidNodeParts.Last();

                    if(lastInvalid.AstNode is EdgeTraversalNode)
                    {
                        var lastInvalidTraversalNode = (EdgeTraversalNode)lastInvalid.AstNode;

                        throw new GraphDBException(new Error_AttributeDoesNotExists(_lastType.Name, lastInvalidTraversalNode.AttributeName));
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// This method adds a new EdgeKey to the IDNode
        /// </summary>
        /// <param name="myStartingType">The type that corresponds to the attribute.</param>
        /// <param name="tempTypeAttributeUUID">The attribute uuid.</param>
        private void AddNewEdgeKey(GraphDBType myStartingType, AttributeUUID tempTypeAttributeUUID)
        {
            EdgeKey tempEdgeKey = new EdgeKey(myStartingType.UUID, tempTypeAttributeUUID);

            _lastType = myStartingType;

            _edges.Add(tempEdgeKey);

            _IDNodeParts.Add(new IDNodeEdge() { EdgeKey = tempEdgeKey });

            _hashCode = _hashCode ^ tempEdgeKey.GetHashCode();
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
            IDNode p = obj as IDNode;
            if ((System.Object)p == null)
            {
                return false;
            }

            return this._hashCode == p.GetHashCode();
        }

        public Boolean Equals(IDNode p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return this._hashCode == p.GetHashCode();
        }

        public static Boolean operator ==(IDNode a, IDNode b)
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

        public static Boolean operator !=(IDNode a, IDNode b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        #endregion

        public List<ParseTreeNode> GetInvalidIDNodeParts()
        {
            return _invalidNodeParts;
        }
    }
}
