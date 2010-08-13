/* <id name="GraphDB – ReplaceNode" />
 * <copyright file="ReplaceNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <developer>Stefan Licht</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.Managers;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Structures.Result;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Replace
{

    public class ReplaceNode : AStatement
    {

        private BinaryExpressionDefinition _whereExpression;
        private String _TypeName;
        private List<AAttributeAssignOrUpdate> _AttributeAssignList;

        public override string StatementName
        {
            get { return "Replace"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes())
            {
                _TypeName = ((ATypeNode)(parseNode.ChildNodes[1].AstNode)).ReferenceAndType.TypeName;

                if (parseNode.ChildNodes[3] != null && parseNode.ChildNodes[3].HasChildNodes())
                {
                    _AttributeAssignList = (parseNode.ChildNodes[3].AstNode as AttrAssignListNode).AttributeAssigns;
                }

                _whereExpression = ((BinaryExpressionNode)parseNode.ChildNodes[5].AstNode).BinaryExpressionDefinition;

                System.Diagnostics.Debug.Assert(_whereExpression != null);
            }
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {

            var qresult = graphDBSession.Replace(_TypeName, _AttributeAssignList, _whereExpression);
            qresult.AddErrorsAndWarnings(ParsingResult);
            return qresult;

        }

    }

}
