/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* <id name="sones GraphDB – DescrIdxsNode" />
 * <copyright file="DescrIdxsNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings
using System;
using System.Collections.Generic;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure.Helper;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class DescribeIndicesNode : ADescrNode
    {
        #region Data
        private List<SelectionResultSet> _Indices;
        #endregion

        #region constructors
        public DescribeIndicesNode()
        {}
        #endregion

        #region AStructureNode
        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;
            
            _Indices = new List<SelectionResultSet>();
            List<AttributeIndex> attrList = new List<AttributeIndex>();

            try
            {
                DescrIdxOutput Output = new DescrIdxOutput();
                var typeList = typeManager.GetAllTypes();


                foreach (var type in typeList)
                {
                    if (type.IsUserDefined)
                    {
                        foreach (var Index in type.GetAllAttributeIndices())
                        {
                            _Indices.Add(new SelectionResultSet(Output.GenerateOutput(Index, Index.IndexName)));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new GraphDBException(new Error_UnknownDBError(e));
            }
        }
        #endregion


        #region ADescrNode
        public override List<SelectionResultSet> Result
        {
            get { return _Indices; }
        }
        #endregion
    }
}
