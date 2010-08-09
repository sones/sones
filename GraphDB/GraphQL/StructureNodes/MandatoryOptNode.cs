/* <id name="PandoraDB – Mandatory OptNode" />
 * <copyright file="MandatoryOptNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class MandatoryOptNode : AStructureNode, IAstNodeInit
    {

        #region Data
        private List<string> _MandAttribs;
        #endregion

        #region constructor
        public MandatoryOptNode()
        {
            _MandAttribs = new List<string>();
        }
        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            try
            {
                if (parseNode.HasChildNodes())
                {
                    if (parseNode.ChildNodes[1].HasChildNodes())
                    {
                        _MandAttribs = (from Attr in parseNode.ChildNodes[1].ChildNodes select Attr.Token.ValueString).ToList();
                    }
                }
            }

            catch(Exception ex)
            {
                throw new GraphDBException(new Error_UnknownDBError(ex));
            }

        }
        
        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

        #region Accessor
        public List<string> MandatoryAttribs
        { 
            get { return _MandAttribs; } 
        }
        #endregion

    }

}
