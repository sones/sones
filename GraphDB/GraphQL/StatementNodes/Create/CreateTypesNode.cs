/* <id name="GraphDB – CreateTypes astnode" />
 * <copyright file="CreateTypesNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of an Create Types statement.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Structures.Result;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes
{

    /// <summary>
    /// This node is requested in case of an Create Types statement.
    /// </summary>
    public class CreateTypesNode : AStatement
    {

        #region Data

        private List<GraphDBTypeDefinition> _TypeDefinitions = new List<GraphDBTypeDefinition>();

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "CreateTypes"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region constructor

        public CreateTypesNode()
        {
            
        }

        #endregion

        #region Execute

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {

            var qresult = graphDBSession.CreateTypes(_TypeDefinitions);
            qresult.AddErrorsAndWarnings(ParsingResult);
            return qresult;
        }

        #endregion

        #region GetContent

        /// <summary>
        /// Gets the content of a CreateTypeStatement.
        /// </summary>
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            if (myParseTreeNode.ChildNodes.Count > 3)
            {

                #region Single type

                BulkTypeNode aTempNode = (BulkTypeNode)myParseTreeNode.ChildNodes[3].AstNode;
                ParsingResult.Push(aTempNode.ParsingResult);

                Boolean isAbstract = false;

                if (myParseTreeNode.ChildNodes[1].HasChildNodes())
                {
                    isAbstract = true;
                }

                _TypeDefinitions.Add(new GraphDBTypeDefinition(aTempNode.TypeName, aTempNode.Extends, isAbstract, aTempNode.Attributes, aTempNode.BackwardEdges, aTempNode.Indices, aTempNode.Comment));
                
                #endregion

            }

            else
            {

                #region Multi types

                foreach (var _ParseTreeNode in myParseTreeNode.ChildNodes[2].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode != null)
                    {
                        BulkTypeListMemberNode aTempNode = (BulkTypeListMemberNode)_ParseTreeNode.AstNode;
                        ParsingResult.Push(aTempNode.ParsingResult);
                        _TypeDefinitions.Add(new GraphDBTypeDefinition(aTempNode.TypeName, aTempNode.Extends, aTempNode.IsAbstract, aTempNode.Attributes, aTempNode.BackwardEdges, aTempNode.Indices, aTempNode.Comment));
                    }
                }

                #endregion

            }


        }

        #endregion
    
    }

}
