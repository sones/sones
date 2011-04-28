using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Manager.Select;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class SelectNode : AStatement, IAstNodeInit
    {
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

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
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
                    throw new DuplicateReferenceOccurrenceException(aType.ReferenceAndType.TypeName);
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

            if (parseNode.ChildNodes[4].ChildNodes != null && parseNode.ChildNodes[4].ChildNodes.Count > 0)
            {
                WhereExpressionNode tempWhereNode = (WhereExpressionNode)parseNode.ChildNodes[4].AstNode;
                if (tempWhereNode.BinaryExpressionDefinition != null)
                {
                    WhereExpressionDefinition = tempWhereNode.BinaryExpressionDefinition;
                }
            }

            #endregion

            #region groupClauseOpt

            if (HasChildNodes(parseNode.ChildNodes[5]) && HasChildNodes(parseNode.ChildNodes[5].ChildNodes[2]))
            {
                foreach (ParseTreeNode node in parseNode.ChildNodes[5].ChildNodes[2].ChildNodes)
                {
                    GroupByIDs.Add(((IDNode)node.AstNode).IDChainDefinition);
                }
            }

            #endregion

            #region havingClauseOpt

            if (HasChildNodes(parseNode.ChildNodes[6]))
            {
                Having = ((BinaryExpressionNode)parseNode.ChildNodes[6].ChildNodes[1].AstNode).BinaryExpressionDefinition;
            }

            #endregion

            #region orderClauseOpt

            if (HasChildNodes(parseNode.ChildNodes[7]))
            {
                OrderByDefinition = ((OrderByNode)parseNode.ChildNodes[7].AstNode).OrderByDefinition;
            }

            #endregion

            #region Offset

            if (HasChildNodes(parseNode.ChildNodes[9]))
            {
                Offset = ((OffsetNode)parseNode.ChildNodes[9].AstNode).Count;
            }

            #endregion

            #region Limit

            if (HasChildNodes(parseNode.ChildNodes[10]))
            {
                Limit = ((LimitNode)parseNode.ChildNodes[10].AstNode).Count;
            }

            #endregion

            #region Depth

            if (HasChildNodes(parseNode.ChildNodes[11]))
            {
                ResolutionDepth = Convert.ToUInt16(parseNode.ChildNodes[11].ChildNodes[1].Token.Value);
            }

            #endregion

            #region Select Output

            if (HasChildNodes(parseNode.ChildNodes[12]))
            {
                _SelectOutputType = (parseNode.ChildNodes[12].AstNode as SelectOutputOptNode).SelectOutputType;
            }

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Select"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
