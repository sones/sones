/*
 * CreateIndexNode
 * (c) Achim Friedland, 2009 - 2010
 * refactored: Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.GraphDB.Structures;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes
{

    /// <summary>
    /// This node is requested in case of an create index statement.
    /// </summary>
    public class CreateIndexNode : AStatement
    {

        #region Data

        String                          _IndexName          = null;
        String                          _IndexEdition       = null;
        String                          _DBType             = null;
        List<IndexAttributeDefinition>  _AttributeList      = null;
        String                          _IndexType;

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "CreateIndex"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region Constructors

        public CreateIndexNode()
        { }

        #endregion

        #region GetContent

        /// <summary>
        /// Gets the content of an UpdateStatement.
        /// </summary>
        /// <param name="myCompilerContext">CompilerContext of Irony.</param>
        /// <param name="myParseTreeNode">The current ParseNode.</param>
        /// <param name="typeManager">The TypeManager of the GraphDB.</param>
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            var grammar = GetGraphQLGrammar(myCompilerContext);

            var childNum = 0;
            foreach (var child in myParseTreeNode.ChildNodes)
            {
                if (child.AstNode != null)
                {
                    if (child.AstNode is IndexNameOptNode)
                    {
                        _IndexName = (child.AstNode as IndexNameOptNode).IndexName;
                    }
                    else if (child.AstNode is EditionOptNode)
                    {
                        _IndexEdition = (child.AstNode as EditionOptNode).IndexEdition;
                    }
                    else if (child.AstNode is ATypeNode)
                    {
                        _DBType = (child.AstNode as ATypeNode).ReferenceAndType.TypeName;

                        if (myParseTreeNode.ChildNodes[childNum - 1].Token.AsSymbol == grammar.S_ON)
                        {
                            ParsingResult.PushIWarning(new Warnings.Warning_ObsoleteGQL("CREATE INDEX ... ON", "CREATE INDEX ... ON VERTEX"));
                        }
                    }
                    else if (child.AstNode is IndexAttributeListNode)
                    {
                        ParsingResult.PushIExceptional((child.AstNode as IndexAttributeListNode).ParsingResult);
                        _AttributeList = (child.AstNode as IndexAttributeListNode).IndexAttributes;
                    }
                    else if (child.AstNode is IndexTypeOptNode)
                    {
                        _IndexType = (child.AstNode as IndexTypeOptNode).IndexType;
                    }

                }
                childNum++;
            }


        }

        #endregion

        #region Execute

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="myIGraphDBSession">The DBSession to start new transactions</param>
        /// <param name="transactionContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="myIGraphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession myIGraphDBSession)
        {

            var createIdxResult = myIGraphDBSession.CreateIndex(_DBType, _IndexName, _IndexEdition, _IndexType, _AttributeList);
            createIdxResult.PushIExceptional(ParsingResult);
            return createIdxResult;

        }

        #endregion

    }

}
