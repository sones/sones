/* <id name="GraphDB – Partial select statement node" />
 * <copyright file="PartialSelectStmtNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of a Partial select statement.</summary>
 */

#region Usings

using sones.GraphDB.GraphQL.StatementNodes.Select;


using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Managers.Structures;
using System.Collections.Generic;
using System;
using sones.GraphDB.Managers.Select;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of a Partial select statement.
    /// </summary>
    public class PartialSelectStmtNode : AStructureNode
    {

        #region Data

        QueryResult _queryResult = null;

        #endregion

        #region Properties
        public SelectDefinition SelectDefinition { get; private set; }

        #endregion

        #region constructor

        public PartialSelectStmtNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var aSelectNode = (SelectNode)parseNode.ChildNodes[0].AstNode;

            SelectDefinition = new Managers.Structures.SelectDefinition(aSelectNode.TypeList, aSelectNode.SelectedElements, aSelectNode.WhereExpressionDefinition, 
                aSelectNode.GroupByIDs, aSelectNode.Having, aSelectNode.Limit, aSelectNode.Offset, aSelectNode.OrderByDefinition, aSelectNode.ResolutionDepth);

        }

        public QueryResult ExecuteQuery(IGraphDBSession myGraphDBSession)
        {
            return myGraphDBSession.Select(SelectDefinition.SelectedElements, SelectDefinition.TypeList, SelectDefinition.WhereExpressionDefinition, SelectDefinition.GroupByIDs, SelectDefinition.Having, SelectDefinition.OrderByDefinition, SelectDefinition.Limit, SelectDefinition.Offset, SelectDefinition.ResolutionDepth);
        }

    }

}
