/* <id name="GraphDB – ParametersNode node" />
 * <copyright file="ParametersNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>A list of parameters passed to a add method of a AListEdgeType.</summary>
 */

#region Usings

using System.Collections.Generic;

using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.Frameworks.Irony.Parsing;


#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// A list of parameters passed to a add method of a AListEdgeType.
    /// </summary>
    public class ParametersNode : AStructureNode, IAstNodeInit
    {

        private List<ADBBaseObject> _ParameterValues;
        public List<ADBBaseObject> ParameterValues
        {
            get { return _ParameterValues; }
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _ParameterValues = new List<ADBBaseObject>();
            if (parseNode.HasChildNodes() && parseNode.ChildNodes[1].HasChildNodes())
            {
                foreach (var child in parseNode.ChildNodes[1].ChildNodes)
                {
                    _ParameterValues.Add(GraphDBTypeMapper.GetGraphObjectFromTypeName(child.Token.Terminal.GetType().Name, child.Token.Value));
                }
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
