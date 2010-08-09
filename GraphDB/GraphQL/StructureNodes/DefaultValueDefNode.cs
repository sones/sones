/* <id name="GraphDB – DefaultValueDefNode" />
 * <copyright file="DefaultValueDefNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This node just hold the value of a DEFAULT parameter of a list definition.</summary>
 */

#region Usings

using System;

using sones.Lib.Frameworks.Irony.Parsing;

using sones.GraphDB.GraphQL.StructureNodes;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node just hold the value of a DEFAULT parameter of a list definition.
    /// </summary>
    public class DefaultValueDefNode : AStructureNode, IAstNodeInit
    {

        /// <summary>
        /// The default value.
        /// </summary>
        public Object Value { get; private set; }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            Value = parseNode.ChildNodes[2].Token.Value;
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    
    }

}
