/* <id name="PandoraDB – InsertOrUpdateNode" />
 * <copyright file="InsertOrUpdateNode.cs"
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
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Structures.Result;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.InsertOrUpdate
{

    public class InsertOrUpdateNode : AStatement
    {

        #region Constructor

        public InsertOrUpdateNode()
        { }

        #endregion

        private List<AAttributeAssignOrUpdate> _AttributeAssignList;
        private BinaryExpressionDefinition _WhereExpression;
        private String _Type;

        public override string StatementName
        {
            get { return "InsertOrUpdate"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes())
            {

                //get type
                if (parseNode.ChildNodes[1] != null && parseNode.ChildNodes[1].AstNode != null)
                {
                    _Type = ((ATypeNode)(parseNode.ChildNodes[1].AstNode)).ReferenceAndType.TypeName;
                }
                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }


                if (parseNode.ChildNodes[3] != null && parseNode.ChildNodes[3].HasChildNodes())
                {

                    _AttributeAssignList = (parseNode.ChildNodes[3].AstNode as AttrAssignListNode).AttributeAssigns;

                }

                if (parseNode.ChildNodes[4] != null && ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinExprNode != null)
                {
                    _WhereExpression = ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinExprNode.BinaryExpressionDefinition;
                }

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

            return graphDBSession.InsertOrUpdate(_Type, _AttributeAssignList, _WhereExpression);

        }

    }

}
