/* <id name="GraphDB – EdgeTypeParamsNode" />
 * <copyright file="EdgeTypeParamsNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>A collection of all params for a EdgeType definition</summary>
 */

#region Usings

using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// A collection of all params for a EdgeType definition
    /// </summary>
    public class EdgeTypeParamsNode : AStructureNode, IAstNodeInit
    {

        private List<EdgeTypeParamDefinition> _Parameters;
        public EdgeTypeParamDefinition[] Parameters
        {
            get {
                if (_Parameters == null)
                    return new EdgeTypeParamDefinition[0];
                return _Parameters.ToArray(); 
            }
        }

        public EdgeTypeParamsNode()
        {
            _Parameters = new List<EdgeTypeParamDefinition>();
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            foreach (var child in parseNode.ChildNodes)
            {
                _Parameters.Add((child.AstNode as EdgeTypeParamNode).EdgeTypeParamDefinition);

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
