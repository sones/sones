/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/* <id name="GraphDB – Mandatory OptNode" />
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
