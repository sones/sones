/* <id name="GraphLib – Autocompletion" />
 * <copyright file="GraphTypeAC.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements the autocompletion
 * for actual GraphTypes.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphDS.API.CSharp;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// This class implements the autocompletion
    /// for actual GraphTypes.
    /// </summary>

    public class StringLiteralGraphType : ADBCLIAutocompletions
    {

        #region properties

        public override String Name { get { return "stringLiteralGraphType"; } }

        #endregion

        #region completion method

        public override List<String> Complete(AGraphDSSharp myGraphDSSharp, ref String CurrentPath, string CurrentStringLiteral)
        {

            var possibleGraphTypes = new List<String>();

            if (myGraphDSSharp.IGraphDBSession != null)
            {

                using (var transaction = myGraphDSSharp.IGraphDBSession.BeginTransaction())
                {

                    foreach (var _GraphType in ((DBContext)transaction.GetDBContext()).DBTypeManager.GetAllTypes())
                    {
                        if (_GraphType.Name.StartsWith(CurrentStringLiteral))
                        {
                            possibleGraphTypes.Add(_GraphType.Name);
                        }
                    }

                }

            }

            return possibleGraphTypes;
            
        }

        #endregion

    }

}
