/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphDB.TypeSystem;
using System.Collections.Generic;
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.Misc
{
    /// <summary>
    /// This node is requested in case of an ID statement. This might be something like U.Name or Name or U.$GUID or U.Friends.Name.
    /// It is necessary to execute an AType node (or TypeWrapper) in previous.
    /// </summary>
    public sealed class IDNode : AStructureNode, IAstNodeInit
    {
        public IDChainDefinition IDChainDefinition { get; private set; }
        private String _IDNodeString;

        #region constructor

        public IDNode()
        {

        }

        public IDNode(IVertexType myType, String myReference)
        {

            IDChainDefinition = new IDChainDefinition();
            IDChainDefinition.AddPart(new ChainPartTypeOrAttributeDefinition(myType.Name));
            var listOfRefs = new Dictionary<String, IVertexType>();
            listOfRefs.Add(myReference, myType);

        }

        #endregion


        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            ExtractIDNodeString(parseNode);

            IDChainDefinition = new IDChainDefinition(_IDNodeString, GetTypeReferenceDefinitions(context));

            foreach (var child in parseNode.ChildNodes)
            {

                if (child.AstNode is EdgeTraversalNode)
                {

                    #region EdgeTraversalNode

                    var edgeTraversal = (child.AstNode as EdgeTraversalNode);
                    if (edgeTraversal.FuncCall != null)
                    {
                        IDChainDefinition.AddPart(edgeTraversal.FuncCall.FuncDefinition, new IDChainDelemiter(edgeTraversal.Delimiter.GetKindOfDelimiter()));
                    }
                    else
                    {
                        IDChainDefinition.AddPart(new ChainPartTypeOrAttributeDefinition(edgeTraversal.AttributeName), new IDChainDelemiter(edgeTraversal.Delimiter.GetKindOfDelimiter()));
                    }

                    #endregion

                }

                else if (child.AstNode is EdgeInformationNode)
                {

                    #region EdgeInformation

                    var aEdgeInformation = (EdgeInformationNode)child.AstNode;

                    throw new NotImplementedQLException("Start here integrating edgeinfos");

                    #endregion

                }

                else if (child.AstNode is FuncCallNode)
                {
                    IDChainDefinition.AddPart((child.AstNode as FuncCallNode).FuncDefinition);
                }

                else
                {
                    IDChainDefinition.AddPart(new ChainPartTypeOrAttributeDefinition(child.Token.ValueString));
                }

            }
        }

        #endregion

        #region private helper

        #region ExtractIDNodeString(parseNode)

        /// <summary>
        /// This method extracts the IDNodeString from the irony parse tree
        /// </summary>
        /// <param name="parseNode">A ParseTree node.</param>
        /// <returns>The IDNode String.</returns>
        private void ExtractIDNodeString(ParseTreeNode parseNode)
        {

            foreach (var aChildNode in parseNode.ChildNodes)
            {

                if (aChildNode.Term is IdentifierTerminal)
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
                            _IDNodeString += aEdgeTraversalNode.FuncCall.FuncDefinition.SourceParsedString;
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
                            _IDNodeString += ((FuncCallNode)aChildNode.AstNode).FuncDefinition.SourceParsedString;
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
                                throw new NotImplementedQLException("");
                            }

                        }

                    }

                }

            }

        }

        #endregion

        #endregion
    }
}
