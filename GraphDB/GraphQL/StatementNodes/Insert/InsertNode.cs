/* <id name="GraphDB – Insert astnode" />
 * <copyright file="InsertNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of an Insert statement.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.Managers;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StructureNodes;


using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Insert
{

    /// <summary>
    /// This node is requested in case of an Insert statement.
    /// </summary>
    class InsertNode : AStatement
    {

        private String _TypeName;
        private List<AAttributeAssignOrUpdate> _AttributeAssignList;
        
        #region Properties - Statement information

        public override String StatementName { get { return "Insert"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region constructor

        public InsertNode()
        {
        }

        #endregion

        #region public AStatement methods

        /// <summary>
        /// Gets the content of a InsertStatement.
        /// </summary>
        /// <param name="context">CompilerContext of Irony.</param>
        /// <param name="parseNode">The current ParseNode.</param>
        /// <param name="typeManager">The TypeManager of the GraphDB.</param>
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            #region get type for name

            _TypeName = GetTypeReferenceDefinitions(myCompilerContext).First().TypeName;

            #endregion

            #region get myAttributes

            if (myParseTreeNode.ChildNodes[3].HasChildNodes())
            {

                _AttributeAssignList = ((myParseTreeNode.ChildNodes[3].ChildNodes[1].AstNode as AttrAssignListNode).AttributeAssigns);

            }

            #endregion

        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {

            var qresult = graphDBSession.Insert(_TypeName, _AttributeAssignList);
            qresult.PushIExceptional(ParsingResult);
            return qresult;

        }


        #endregion

    }

}
