/* <id name="GraphDB – EdgeTypeDefNode" />
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
    public class EdgeTypeDefNode : AStructureNode
    {

        #region Data

        public KindsOfType Type { get; private set; }

        public String Name { get; private set; }
        
        /// <summary>
        /// The characteristics of an edge (Backward, Mandatory, Unique - valid for a combination of typeattributes
        /// </summary>
        public TypeCharacteristics TypeCharacteristics { get; private set; }

        /// <summary>
        /// An edge type definition - resolved by the name and found in the typeManager EdgeTypesLookup table
        /// </summary>
        public String EdgeType{ get; private set; }

        public EdgeTypeParamDefinition[] Parameters { get; private set; }

        #endregion

        #region constructor

        public EdgeTypeDefNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            #region TypeEdge class

            //if (!dbContext.DBPluginManager.HasEdgeType(parseNode.ChildNodes[2].Token.ValueString))
            //    throw new GraphDBException(new Error_EdgeTypeDoesNotExist(parseNode.ChildNodes[2].Token.ValueString));

            //EdgeType = dbContext.DBPluginManager.GetEdgeType(parseNode.ChildNodes[2].Token.ValueString);
            EdgeType = parseNode.ChildNodes[2].Token.ValueString;

            if (parseNode.ChildNodes[3].AstNode != null)
            {
                Parameters = ((EdgeTypeParamsNode)parseNode.ChildNodes[3].AstNode).Parameters;
            }

            #endregion

            if (parseNode.FirstChild.Term.Name.Equals("id_simple"))
            {
                #region simple id

                Type = KindsOfType.SingleNoneReference;

                Name = parseNode.ChildNodes[0].Token.ValueString;

                #endregion
            }

            else if (parseNode.FirstChild.Term.Name.ToUpper().Equals(DBConstants.DBBackwardEdge.ToUpper()))
            {
                Name = ((IDNode)parseNode.ChildNodes[2].AstNode).IDChainDefinition.Reference.Item2.Name;
                TypeCharacteristics = new TypeCharacteristics();
                TypeCharacteristics.IsBackwardEdge = true;
            }

            else
            {

                Name = parseNode.ChildNodes[5].Token.ValueString;

                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.SET)
                {
                    Type = KindsOfType.UnknownSet;
                }

                if (parseNode.ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.LIST)
                {
                    Type = KindsOfType.ListOfNoneReferences;
                }


            }

        }

    }

}
