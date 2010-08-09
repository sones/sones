/* <id name="PandoraDB – ListPropertyAssign_WeightedNode" />
 * <copyright file="ListPropertyAssign_WeightedNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>The weighted value.</summary>
 */

#region Usings

using System;

using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// The weighted value
    /// </summary>
    public class ListPropertyAssign_WeightedNode : AStructureNode, IAstNodeInit
    {

        private Boolean _IsWeighted = false;
        public Boolean IsWeighted
        {
            get { return _IsWeighted; }
        }

        private DBNumber _WeightedValue = new DBUInt64(1UL);
        public DBNumber WeightedValue
        {
            get { return _WeightedValue; }
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (!parseNode.HasChildNodes())
                return;

            _IsWeighted = true;

            if (parseNode.ChildNodes[2].HasChildNodes())
            {
                String type = ((GraphDBTypeNode)parseNode.ChildNodes[2].AstNode).DBTypeDefinition.Name;
                _WeightedValue = (DBNumber)GraphDBTypeMapper.GetPandoraObjectFromTypeName(type); //Convert.ToDouble(parseNode.ChildNodes[2].Token.Value);
            }
            else if (parseNode.ChildNodes.Count == 3)
            {
                _WeightedValue.SetValue(parseNode.ChildNodes[2].Token.Value);
            }

            if (parseNode.ChildNodes.Count == 4)
            {
                _WeightedValue.SetValue(parseNode.ChildNodes[2].Token.Value);
            }
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
