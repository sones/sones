/* <id name="DescribeNode" />
 * <copyright file="DescribeNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;

using sones.GraphDB.Managers.Structures.Describe;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.Structures.Result;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class DescribeNode : AStatement
    {

        #region Data

        private ADescribeDefinition _DescribeDefinition;

        #endregion

        #region Constructor

        public DescribeNode()
        {            
        }

        #endregion        

        #region AStatement

        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            _DescribeDefinition = ((ADescrNode)myParseTreeNode.ChildNodes[1].AstNode).DescribeDefinition;

        }

        public override String StatementName
        {
            get { return "Describe"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="myIGraphDBSession">The DBSession to start new transactions</param>
        /// <param name="myDBContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="myIGraphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession myIGraphDBSession)
        {

            var qresult = myIGraphDBSession.Describe(_DescribeDefinition);
            qresult.AddErrorsAndWarnings(ParsingResult);
            return qresult;
        }

        #endregion
        
    }

}
