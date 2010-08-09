/* <id name="GraphDB – AttrRemove Node" />
 * <copyright file="AttrRemoveNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of an AttrRemove node.</summary>
 */

#region usings

using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class AttrRemoveNode : AStructureNode, IAstNodeInit
    {

        #region properties

        public AttributeRemove AttributeRemove { get; private set; }

        #endregion

        #region constructor

        public AttrRemoveNode()
        {

        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var _toBeRemovedAttributes = new List<string>();

            foreach (ParseTreeNode aParseTreeNode in parseNode.ChildNodes[2].ChildNodes)
            {
                _toBeRemovedAttributes.Add(aParseTreeNode.Token.ValueString);
            }

            AttributeRemove = new AttributeRemove(_toBeRemovedAttributes);
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    
    }

}
