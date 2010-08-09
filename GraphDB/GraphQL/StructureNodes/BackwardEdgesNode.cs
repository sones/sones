/* <id name="GraphDB – BackwardEdgesNode" />
 * <copyright file="BackwardEdgesNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>A list of single BackwardEdge definition nodes.</summary>
 */

#region Usings

using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class BackwardEdgesNode : AStructureNode
    {

        #region Data

        /// <summary>
        /// The information about the BackwardEdge: &lt;Type, Attribute, Visible AttributeName&gt;
        /// </summary>
        public List<BackwardEdgeDefinition> BackwardEdgeInformation
        {
            get { return _BackwardEdgeInformation; }
        }
        private List<BackwardEdgeDefinition> _BackwardEdgeInformation;

        #endregion

        #region constructor

        public BackwardEdgesNode()
        {
            _BackwardEdgeInformation = new List<BackwardEdgeDefinition>();
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes())
            {
                foreach (var _ParseTreeNode in parseNode.ChildNodes[1].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode as BackwardEdgeNode != null)
                    {
                        _BackwardEdgeInformation.Add(((BackwardEdgeNode)_ParseTreeNode.AstNode).BackwardEdgeDefinition);
                    }
                }
            }

        }

    }

}
