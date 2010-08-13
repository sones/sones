/* <id name="GraphDB – ID node" />
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

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.TypeManagement;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an ID statement. This might be something like U.Name or Name or U.$GUID or U.Friends.Name.
    /// It is necessary to execute an AType node (or TypeWrapper) in previous.
    /// </summary>
    public class IDNode : AStructureNode
    {

        public IDChainDefinition IDChainDefinition { get; private set; }
        private String _IDNodeString;

        #region constructor

        public IDNode()
        {
        
        }

        public IDNode(GraphDBType myType, String myReference)
        {

            IDChainDefinition = new Managers.Structures.IDChainDefinition();
            IDChainDefinition.AddPart(new ChainPartTypeOrAttributeDefinition(myType.Name));
            var listOfRefs = new Dictionary<String, GraphDBType>();
            listOfRefs.Add(myReference, myType);

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

                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Start here integrating edgeinfos"));

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
                                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                            }

                        }

                    }

                }

            }

        }

        #endregion

    }

}
