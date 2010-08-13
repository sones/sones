/*
 * DescribeEdgeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;

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

        public override Exceptional<List<SelectionResultSet>> GetResult(DBContext myDBContext)
        {

            var result = new List<SelectionResultSet>();

            if (!String.IsNullOrEmpty(_EdgeName))
            {

                #region Specific edge

                var edge = myDBContext.DBPluginManager.GetEdgeType(_EdgeName);
                if (edge != null)
                {
                    result.Add(new SelectionResultSet(GenerateOutput(edge, _EdgeName)));
                }
                else
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_EdgeTypeDoesNotExist(_EdgeName));
                }

                #endregion

            }
            else
            {

                #region All edge

                foreach (var edge in myDBContext.DBPluginManager.GetAllEdgeTypes())
                {
                    result.Add(new SelectionResultSet(GenerateOutput(edge.Value, edge.Key)));
                }

                #endregion

            }

            return new Exceptional<List<SelectionResultSet>>(result);

        }

        #endregion

        #region Output

        /// <summary>
        /// generates an output for an edgetype
        /// </summary>
        /// <param name="myEdge">the edge</param>
        /// <param name="myEdgeName">edge name</param>
        /// <returns>list of readouts with the information</returns>
        private IEnumerable<DBObjectReadout> GenerateOutput(IEdgeType myEdge, String myEdgeName)
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
