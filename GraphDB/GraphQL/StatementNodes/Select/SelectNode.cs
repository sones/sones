/*
 * SelectNode
 * (c) Stefan Licht, 2009-2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Select;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.GraphDB.Settings;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Select
{

    public class SelectNode : AStatement
    {

        #region AStatement Properties

        public override string StatementName
        {
            get { return "Select"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// List of selected types
        /// </summary>
        public List<TypeReferenceDefinition> TypeList { get; private set; }

        /// <summary>
        /// AExpressionDefinition, Alias, SelectValueAssignment - PBI 527
        /// </summary>
        //public Dictionary<AExpressionDefinition, String> SelectedElements { get; private set; }
        public List<Tuple<AExpressionDefinition, String, SelectValueAssignment>> SelectedElements { get; private set; }

        /// <summary>
        /// Group by definitions
        /// </summary>
        public List<IDChainDefinition> GroupByIDs { get; private set; }

        /// <summary>
        /// Having definition
        /// </summary>
        public BinaryExpressionDefinition Having { get; private set; }

        /// <summary>
        /// OrderBy section
        /// </summary>
        public OrderByDefinition OrderByDefinition { get; private set; }

        /// <summary>
        /// Limit section
        /// </summary>
        public UInt64? Limit { get; private set; }

        /// <summary>
        /// Offset section
        /// </summary>
        public UInt64? Offset { get; private set; }

        /// <summary>
        /// Resolution depth
        /// </summary>
        public Int64 ResolutionDepth { get; private set; }

        public BinaryExpressionDefinition WhereExpressionDefinition { get; private set; }

        #endregion

        #region Data

        /// <summary>
        /// The type of the output
        /// </summary>
        SelectOutputTypes _SelectOutputType = SelectOutputTypes.Tree;

        SelectResultManager _SelectResultManager;

        #endregion

        #region override AStatement.GetContent

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region Data

            TypeList = new List<TypeReferenceDefinition>();
            GroupByIDs = new List<IDChainDefinition>();
            SelectedElements = new List<Tuple<AExpressionDefinition, string, SelectValueAssignment>>();
            Limit = null;
            Offset = null;
            WhereExpressionDefinition = null;
            ResolutionDepth = -1;

            #endregion

            #region TypeList

            foreach (ParseTreeNode aNode in parseNode.ChildNodes[1].ChildNodes)
            {
                ATypeNode aType = (ATypeNode)aNode.AstNode;

                // use the overrides equals to check duplicated references
                if (!TypeList.Contains(aType.ReferenceAndType))
                {
                    TypeList.Add(aType.ReferenceAndType);
                }
                else
                {
                    throw new GraphDBException(new Error_DuplicateReferenceOccurence(aType.ReferenceAndType));
                }
            }

            #endregion

            #region selList

            foreach (ParseTreeNode aNode in parseNode.ChildNodes[3].ChildNodes)
            {
                SelectionListElementNode aColumnItemNode = (SelectionListElementNode)aNode.AstNode;
                String typeName                          = null;

                if (aColumnItemNode.SelType != TypesOfSelect.None)
                {
                    if (aColumnItemNode.SelType == TypesOfSelect.Ad)
                    {
                        typeName = aColumnItemNode.TypeName;
                    }

                    foreach (var reference in GetTypeReferenceDefinitions(context))
                    {
                        //SelectedElements.Add(new IDChainDefinition(new ChainPartTypeOrAttributeDefinition(reference.TypeName), aColumnItemNode.SelType, typeName), null);
                        SelectedElements.Add(new Tuple<AExpressionDefinition, string, SelectValueAssignment>(
                            new IDChainDefinition(new ChainPartTypeOrAttributeDefinition(reference.TypeName), aColumnItemNode.SelType, typeName), null, aColumnItemNode.ValueAssignment));
                    }
                    continue;
                }

                SelectedElements.Add(new Tuple<AExpressionDefinition, string, SelectValueAssignment>(
                    aColumnItemNode.ColumnSourceValue, aColumnItemNode.AliasId, aColumnItemNode.ValueAssignment));

            }

            #endregion

            #region whereClauseOpt

            if (parseNode.ChildNodes[4].HasChildNodes())
            {
                WhereExpressionNode tempWhereNode = (WhereExpressionNode)parseNode.ChildNodes[4].AstNode;
                if (tempWhereNode.BinaryExpressionDefinition != null)
                {
                    WhereExpressionDefinition = tempWhereNode.BinaryExpressionDefinition;
                }
            }

            #endregion

            #region groupClauseOpt

            if (parseNode.ChildNodes[5].HasChildNodes() && parseNode.ChildNodes[5].ChildNodes[2].HasChildNodes())
            {
                foreach (ParseTreeNode node in parseNode.ChildNodes[5].ChildNodes[2].ChildNodes)
                {
                    GroupByIDs.Add(((IDNode)node.AstNode).IDChainDefinition);
                }
            }

            #endregion

            #region havingClauseOpt

            if (parseNode.ChildNodes[6].HasChildNodes())
            {
                Having = ((BinaryExpressionNode)parseNode.ChildNodes[6].ChildNodes[1].AstNode).BinaryExpressionDefinition;
            }

            #endregion

            #region orderClauseOpt

            if (parseNode.ChildNodes[7].HasChildNodes())
            {
                OrderByDefinition = ((OrderByNode)parseNode.ChildNodes[7].AstNode).OrderByDefinition;
            }

            #endregion

            //#region MatchingClause

            //if (parseNode.ChildNodes[8].HasChildNodes())
            //{
            //    throw new NotImplementedException();
            //}

            //#endregion

            #region Offset

            if (parseNode.ChildNodes[9].HasChildNodes())
            {
                Offset = ((OffsetNode)parseNode.ChildNodes[9].AstNode).Count;
            }

            #endregion

            #region Limit

            if (parseNode.ChildNodes[10].HasChildNodes())
            {
                Limit = ((LimitNode)parseNode.ChildNodes[10].AstNode).Count;
            }

            #endregion

            #region Depth

            if (parseNode.ChildNodes[11].HasChildNodes())
            {
                ResolutionDepth = Convert.ToUInt16(parseNode.ChildNodes[11].ChildNodes[1].Token.Value);
            }

            #endregion

            #region Select Output

            if (parseNode.ChildNodes[12].HasChildNodes())
            {
                _SelectOutputType = (parseNode.ChildNodes[12].AstNode as SelectOutputOptNode).SelectOutputType;
            }

            #endregion

        }

        #endregion

        #region override AStatement.Execute

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {

            #region Start select

            var runThreaded = DBConstants.UseThreadedSelect;
#if DEBUG
            runThreaded = false;
#endif

            var qresult = graphDBSession.Select(SelectedElements, TypeList, WhereExpressionDefinition, GroupByIDs, Having, OrderByDefinition, Limit, Offset, ResolutionDepth, runThreaded);
            qresult.PushIExceptional(ParsingResult);
            return qresult;

            #endregion

        }

        #endregion

    }

}
