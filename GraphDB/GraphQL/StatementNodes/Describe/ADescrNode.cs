/* <id name="PandoraDB – ADescrNode" />
 * <copyright file="ADescrNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using sones.GraphDB.Managers.Structures.Describe;
using sones.GraphDB.GraphQL.StructureNodes;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public abstract class ADescrNode : AStructureNode
    {
        public abstract ADescribeDefinition DescribeDefinition { get; }
    }

}
