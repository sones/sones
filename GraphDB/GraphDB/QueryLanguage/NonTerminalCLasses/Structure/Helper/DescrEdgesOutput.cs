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


#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures.EdgeTypes;
#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure.Helper
{
    /// <summary>
    /// this class generate an output for the describe edge(s) commands
    /// </summary>
    public class DescrEdgesOutput
    {

        #region constructor
        
        public DescrEdgesOutput()
        { }

        #endregion

        #region Output

        /// <summary>
        /// generates an output for an edgetype
        /// </summary>
        /// <param name="myEdge">the edge</param>
        /// <param name="myEdgeName">edge name</param>
        /// <returns>list of readouts with the information</returns>
        public IEnumerable<DBObjectReadout> GenerateOutput(AEdgeType myEdge, String myEdgeName)
        {

            string Temp = "";
            int Pos = 0;

            Temp = myEdge.ToString();
            Pos = Temp.IndexOf(",");
                        
            var Edge = new Dictionary<String, Object>();

            if (myEdgeName != "")
                Edge.Add("Name", myEdgeName);
            else
                Edge.Add("Name", Temp.Substring(Pos + 1, Temp.Length - (Pos + 1)));

            Edge.Add("EdgeType", System.Convert.ToUInt32(Temp.Substring(0, Pos)));

            return new List<DBObjectReadout>() { new DBObjectReadout(Edge) };

        }

        #endregion

    }
}
