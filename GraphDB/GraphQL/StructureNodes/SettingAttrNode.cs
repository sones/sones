/* <id name="PandoraDB – ASettingAttrNode" />
 * <copyright file="ASettingAttrNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class SettingAttrNode : AStructureNode
    {

        /// <summary>
        /// TypeName,Attribute
        /// </summary>
        public Dictionary<String, List<IDChainDefinition>> Attributes { get; private set; }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            Attributes = new Dictionary<String, List<IDChainDefinition>>();

            if (parseNode == null)
                return;

            if (parseNode.ChildNodes[1].HasChildNodes())
            {
                foreach (var Node in parseNode.ChildNodes[1].ChildNodes)
                {
                    if (Node.HasChildNodes())
                    {

                        if (!Attributes.ContainsKey((Node.ChildNodes[0].AstNode as ATypeNode).ReferenceAndType.TypeName))
                        {
                            Attributes.Add((Node.ChildNodes[0].AstNode as ATypeNode).ReferenceAndType.TypeName, new List<IDChainDefinition>());
                        }
                        Attributes[(Node.ChildNodes[0].AstNode as ATypeNode).ReferenceAndType.TypeName].Add((Node.ChildNodes[2].AstNode as IDNode).IDChainDefinition);

                    }
                }
            }

        }

    }

}
