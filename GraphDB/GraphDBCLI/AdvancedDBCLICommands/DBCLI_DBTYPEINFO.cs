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

/* <id name="PandoraDB – DBTYPEINFO CLI command" />
 * <copyright file="DBCLI_DBTYPEINFO.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This command enables the user to get informations concerning 
 * the myAttributes of a given type.</summary>
 */



#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.Lib.CLI;
using sones.Lib.Frameworks.CLIrony.Compiler;

using sones.GraphDB;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// Returns myAttributes of a given PandoraType
    /// </summary>

    public class DBCLI_DBTYPEINFO : AllAdvancedDBCLICommands
    {

        #region Constructor

        public DBCLI_DBTYPEINFO()
        {

            // Command name and description
            InitCommand("DBTYPEINFO",
                        "Returns myAttributes of a given PandoraType",
                        "Returns myAttributes of a given PandoraType");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + PandoraTypeNT);

        }

        #endregion

        #region Execute Command

        public override void Execute(ref object myIGraphFS2Session, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            _CancelCommand = false;
            //var _IGraphFS2Session = myIGraphFS2Session as IGraphFSSession;
            var _IPandoraDBSession = myIPandoraDBSession as IGraphDBSession;

            if (_IPandoraDBSession == null)
            {
                WriteLine("No database instance started...");
                return;
            }

            var typeInterestedIn = myOptions.ElementAt(1).Value[0].Option;


            var _ActualType = _IPandoraDBSession.GetDBContext().DBTypeManager.GetTypeByName(typeInterestedIn);

            while (_ActualType != null)
            {

                if (_ActualType.UUID.ToString().Length > 4)
                    WriteLine("(by " + _ActualType.Name + " [UUID: " + _ActualType.UUID.ToString().Substring(0, 4) + "..])");
                else
                    WriteLine("(by " + _ActualType.Name + " [UUID: " + _ActualType.UUID.ToString() + "])");

                if (!_ActualType.Attributes.Count.Equals(0))
                {
                    var maxLengthName = _ActualType.Attributes.Values.Max(a => a.Name.Length);
                    foreach (var _TypeAttribute in _ActualType.Attributes.Values)
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.Append(_TypeAttribute.Name.PadRight(maxLengthName + 3));
                        sb.Append("");

                        if (_TypeAttribute.TypeCharacteristics.IsBackwardEdge)
                        {
                            sb.Append("BACKWARDEDGE<");
                            var beTypeAttr = _IPandoraDBSession.GetDBContext().DBTypeManager.GetTypeAttributeByEdge(_TypeAttribute.BackwardEdgeDefinition);
                            sb.Append(beTypeAttr.GetRelatedType(_IPandoraDBSession.GetDBContext().DBTypeManager).Name + "." + beTypeAttr.Name);
                            sb.Append(">");
                        }
                        else
                        {

                            if (_TypeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                                sb.Append("LIST<");

                            if (_TypeAttribute.KindOfType == KindsOfType.SetOfReferences || _TypeAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                                sb.Append("SET<");

                            sb.Append(_IPandoraDBSession.GetDBContext().DBTypeManager.GetTypeByUUID(_TypeAttribute.DBTypeUUID).Name);

                            if (_TypeAttribute.KindOfType == KindsOfType.ListOfNoneReferences || _TypeAttribute.KindOfType == KindsOfType.SetOfNoneReferences || _TypeAttribute.KindOfType == KindsOfType.SetOfReferences)
                                sb.Append(">");
                        }

                        sb.Append(" ");
                        sb.Append(_TypeAttribute.TypeCharacteristics.ToString());

                        sb.Append(" [UUID: ");
                        sb.Append(_TypeAttribute.UUID.ToString().Substring(0, 4));
                        sb.Append("..]");

                        WriteLine(sb.ToString());
                    }
                }

                else
                    WriteLine("No myAttributes for this type.");

                WriteLine(Environment.NewLine);

                if (_ActualType.ParentTypeUUID == null)
                    break;

                _ActualType = _IPandoraDBSession.GetDBContext().DBTypeManager.GetTypeByUUID(_ActualType.ParentTypeUUID);

            }

        }

        #endregion

    }

}
