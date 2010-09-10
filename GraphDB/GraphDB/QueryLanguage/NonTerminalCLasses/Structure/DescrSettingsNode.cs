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

/* <id name="PandoraDB – DescrSettNode" />
 * <copyright file="DescrSettNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
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
using sones.Lib.ErrorHandling;
using sones.GraphDB.Settings;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class DescribeSettingsNode : ADescrNode
    {

        #region Data
        private List<SelectionResultSet> _Settings;
        #endregion

        #region constructors
        public DescribeSettingsNode()
        { }
        #endregion
        

        /// <summary>
        /// generate a output result
        /// </summary>
        /// <param name="mySetting">the setting</param>
        /// <param name="myReadOutList">list of dbreadouts</param>
        private void GenerateResult(ADBSettingsBase mySetting, List<DBObjectReadout> myReadOutList, DBTypeManager typeManager)
        {

            var Setting = new Dictionary<String, Object>();
            Setting.Add("Name", mySetting.Name);
            Setting.Add("ID", mySetting.ID);
            Setting.Add("Type", typeManager.GetTypeByUUID(mySetting.Type));
            Setting.Add("Desc", mySetting.Description);
            Setting.Add("Default", mySetting.Default);
            Setting.Add("Value", mySetting.Value);

            myReadOutList.Add(new DBObjectReadout(Setting));

        }
        
        #region AStructureNode
        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            _Settings = new List<SelectionResultSet>();
            var ReadOut = new List<DBObjectReadout>();

            try
            {
                foreach (var Item in dbContext.DBSettingsManager.AllSettingsByName)
                {
                    GenerateResult(Item.Value, ReadOut, typeManager);
                }

                _Settings.Add(new SelectionResultSet(ReadOut));

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
            get { return _Settings; }
        }
        #endregion
    }
}
