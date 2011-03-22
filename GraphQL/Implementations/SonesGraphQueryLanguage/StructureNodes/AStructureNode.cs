/* <id name="GraphDB – abstract structure node" />
 * <copyright file="AStructureNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Abstract class for all structure nodes.</summary>
 */

#region Usings

using System.Collections.Generic;
using sones.GraphQL.StructureNodes;
using Irony.Parsing;
using sones.GraphQL.Structures;

#endregion

namespace sones.GraphQL.StructureNodes
{

    /// <summary>
    /// Abstract class for all structure nodes.
    /// </summary>
    public abstract class AStructureNode
    {

        #region Protected properties

        //public Exceptional ParsingResult { get; private set; }

        #endregion

        #region protected helper methods

        /// <summary>
        /// Extracts a AExpressionDefinition from the ParseTreeNode
        /// </summary>
        /// <param name="myParseTreeNode"></param>
        /// <returns></returns>
        protected AExpressionDefinition GetExpressionDefinition(ParseTreeNode myParseTreeNode)
        {
            AExpressionDefinition retVal = null;
            if (myParseTreeNode.Term is NonTerminal)
            {
                #region left is NonTerminal

                if (myParseTreeNode.AstNode is IDNode)
                {
                    retVal = (myParseTreeNode.AstNode as IDNode).IDChainDefinition;
                }
                //else if (myParseTreeNode.AstNode is TupleNode)
                //{
                //    retVal = (myParseTreeNode.AstNode as TupleNode).TupleDefinition;
                //}
                //else if (myParseTreeNode.AstNode is BinaryExpressionNode)
                //{
                //    retVal = (myParseTreeNode.AstNode as BinaryExpressionNode).BinaryExpressionDefinition;
                //}
                //else if (myParseTreeNode.AstNode is UnaryExpressionNode)
                //{
                //    retVal = (myParseTreeNode.AstNode as UnaryExpressionNode).UnaryExpressionDefinition;
                //}
                //else if (myParseTreeNode.AstNode is AggregateNode)
                //{
                //    retVal = (myParseTreeNode.AstNode as AggregateNode).AggregateDefinition;
                //}

                #endregion
            }
            else
            {
                //#region No NonTerminal

                //retVal = new ValueDefinition(myParseTreeNode.Token.Value);

                //#endregion
            }

            return retVal;
        }

        protected List<TypeReferenceDefinition> GetTypeReferenceDefinitions(ParsingContext context)
        {
            //return ((List<TypeReferenceDefinition>)context.GraphListOfReferences);
            return null;
        }

        #endregion

        #region public methods

        public GraphQL.GraphQueryLanguage GetGraphQLGrammar(ParsingContext context)
        {
            return context.Parser.Language.Grammar as GraphQL.GraphQueryLanguage;
        }

        #endregion

        #region Ctor

        //public AStructureNode()
        //{
        //    ParsingResult = new Exceptional();
        //}

        #endregion

    }

}
