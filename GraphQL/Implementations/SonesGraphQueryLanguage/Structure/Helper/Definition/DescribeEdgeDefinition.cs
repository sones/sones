using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphDB.Request.GetEdgeType;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    public sealed class DescribeEdgeDefinition : ADescribeDefinition
    {
        #region Data

        /// <summary>
        /// The name of the edge
        /// </summary>
        private String _EdgeName;

        #endregion

        #region Ctor

        public DescribeEdgeDefinition(string myEdgeName = null)
        {
            _EdgeName = myEdgeName;
        }

        #endregion

        public override QueryResult GetResult(
                                                GQLPluginManager myPluginManager,
                                                IGraphDB myGraphDB,
                                                SecurityToken mySecurityToken,
                                                TransactionToken myTransactionToken)
        {
            var resultingVertices = new List<IVertexView>();
            ASonesException error = null;

            if (!String.IsNullOrEmpty(_EdgeName))
            {

                #region Specific edge

                var request = new RequestGetEdgeType(_EdgeName);
                var edge = myGraphDB.GetEdgeType<IEdgeType>(mySecurityToken, myTransactionToken, request, (stats, edgeType) => edgeType);

                if (edge != null)
                {
                    resultingVertices = new List<IVertexView>() { GenerateOutput(edge, _EdgeName) };
                }
                else
                {
                    error = new EdgeTypeDoesNotExistException(_EdgeName, "");
                }

                #endregion

            }
            else
            {

                #region All edges

                var resultingReadouts = new List<IVertexView>();

                var request = new RequestGetAllEdgeTypes();
                foreach (var edge in myGraphDB.GetAllEdgeTypes<IEnumerable<IEdgeType>>(mySecurityToken, myTransactionToken, request, (stats, edgeTypes) => edgeTypes))
                {
                    resultingReadouts.Add(GenerateOutput(edge, edge.Name));
                }


                #endregion

            }

            return new QueryResult("", "GQL", 0L, ResultType.Successful, resultingVertices, error);
        }

        #region Output

        /// <summary>
        /// generates an output for an edgetype
        /// </summary>
        /// <param name="myEdge">the edge</param>
        /// <param name="myEdgeName">edge name</param>
        /// <returns>list of readouts with the information</returns>
        private IVertexView GenerateOutput(IEdgeType myEdge, String myEdgeName)
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

            return new VertexView(Edge, new Dictionary<String,IEdgeView>());

        }

        #endregion
    }
}
