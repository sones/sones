/* <id name="PandoraDB – EdgeTypeDefNode" />
 * <copyright file="EdgeTypeDefNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This node is the edgeType definition. A simple EdgyTypeList, BackwardEdge (make an implicit backward edge visible to the user)
 *          or a own defined edge derived from AListEdgeType, AListBaseEdgeType, ASingleEdgeType
 * </summary>
 */

#region Usings

using System;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is the edgeType definition. A simple EdgyTypeList, BackwardEdge (make an implicit backward edge visible to the user)
    /// or a own defined edge derived from AListEdgeType, AListBaseEdgeType, ASingleEdgeType
    /// </summary>
    public class SingleEdgeTypeDefNode : AStructureNode
    {

        #region Data

        public KindsOfType Type { get { return _Type; } }
        KindsOfType _Type;

        public String Name { get { return _Name; } }
        String _Name = null;
        
        /// <summary>
        /// The characteristics of an edge (Backward, Mandatory, Unique - valid for a combination of typeattributes
        /// </summary>
        public TypeCharacteristics TypeCharacteristics { get { return _TypeCharacteristics; } }
        TypeCharacteristics _TypeCharacteristics = null;

        /// <summary>
        /// An edge type definition - resolved by the name and found in the typeManager EdgeTypesLookup table
        /// </summary>
        public String EdgeType { get; private set; }

        public EdgeTypeParamDefinition[] Parameters { get; private set; }

        #endregion

        #region Constructor

        public SingleEdgeTypeDefNode()
        {
            
        }

        #endregion

        #region GetContent(myCompilerContext, myParseTreeNode)

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            #region TypeEdge class

            //if (!dbContext.DBPluginManager.HasEdgeType(myParseTreeNode.ChildNodes[0].Token.ValueString))
            //    throw new GraphDBException(new Error_EdgeTypeDoesNotExist(myParseTreeNode.ChildNodes[0].Token.ValueString));

            //_EdgeType = dbContext.DBPluginManager.GetEdgeType(myParseTreeNode.ChildNodes[0].Token.ValueString);

            EdgeType = myParseTreeNode.ChildNodes[0].Token.ValueString;

            if (myParseTreeNode.ChildNodes[1].AstNode != null)
            {
                Parameters = ((EdgeTypeParamsNode)myParseTreeNode.ChildNodes[1].AstNode).Parameters;
            }

            #endregion

            _Name = myParseTreeNode.ChildNodes[3].Token.ValueString;

            _Type = KindsOfType.UnknownSingle;
            
        }

        #endregion

    }

}
