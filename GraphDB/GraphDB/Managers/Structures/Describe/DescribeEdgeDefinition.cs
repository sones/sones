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

/*
 * DescribeEdgeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{

    public class DescribeEdgeDefinition : ADescribeDefinition
    {

        #region Data

        private String _EdgeName;

        #endregion

        #region Ctor

        public DescribeEdgeDefinition(string myEdgeName = null)
        {
            _EdgeName = myEdgeName;
        }

        #endregion

        #region ADescribeDefinition

        public override Exceptional<IEnumerable<Vertex>> GetResult(DBContext myDBContext)
        {
            if (!String.IsNullOrEmpty(_EdgeName))
            {

                #region Specific edge

                var edge = myDBContext.DBPluginManager.GetEdgeType(_EdgeName);
                if (edge != null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new List<Vertex>(){GenerateOutput(edge, _EdgeName)});
                }
                else
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_EdgeTypeDoesNotExist(_EdgeName));
                }

                #endregion

            }
            else
            {

                #region All edges

                var resultingReadouts = new List<Vertex>();

                foreach (var edge in myDBContext.DBPluginManager.GetAllEdgeTypes())
                {
                    resultingReadouts.Add(GenerateOutput(edge.Value, edge.Key));
                }


                return new Exceptional<IEnumerable<Vertex>>(resultingReadouts);
                #endregion

            }
        }

        #endregion

        #region Output

        /// <summary>
        /// generates an output for an edgetype
        /// </summary>
        /// <param name="myEdge">the edge</param>
        /// <param name="myEdgeName">edge name</param>
        /// <returns>list of readouts with the information</returns>
        private Vertex GenerateOutput(IEdgeType myEdge, String myEdgeName)
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

            Edge.Add("EdgeType", Convert.ToUInt32(Temp.Substring(0, Pos)));

            return new Vertex(Edge);

        }

        #endregion

    }

}
