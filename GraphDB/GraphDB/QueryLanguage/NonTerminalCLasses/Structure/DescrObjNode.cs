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


/* <id name="sones GraphDB – DescrObjNode" />
 * <copyright file="DescrObjNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class DescribeObjectNode : ADescrNode
    {
        #region Data
        private List<SelectionResultSet> _ObjectValues;
        private ADBBaseObject _Object = null;
        #endregion

        #region constructor
        public DescribeObjectNode()
        {
        }
        #endregion

        #region AStructureNode        

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _ObjectValues = new List<SelectionResultSet>();
            
            try
            {
                if (parseNode.HasChildNodes())
                {
                    _Object = PandoraTypeMapper.GetPandoraObjectFromTypeName(parseNode.ChildNodes[1].Token.ValueString);

                   // _ObjectValues.Add("ID", _Object.ID.ToString());

                    /*NameValue = new Dictionary<string, object>();
                    NameValue.Add("ModificationTime", _Object.ModificationTime.ToString());
                    Readout.Add(new DBObjectReadout(NameValue));*/

                   /* _ObjectValues.Add("Type", _Object.Type.ToString());
                    _ObjectValues.Add("Value", _Object.Value.ToString());*/
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }        
        #endregion

        #region Accessor
        public override List<SelectionResultSet> Result
        { get { return _ObjectValues; } }
        #endregion
    }
}
