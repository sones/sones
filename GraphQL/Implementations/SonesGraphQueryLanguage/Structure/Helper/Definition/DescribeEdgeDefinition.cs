using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;
using Irony.Parsing;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphDB.Request.GetEdgeType;

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

        public override IEnumerable<IVertexView> GetResult(ParsingContext myContext,
                                                            GQLPluginManager myPluginManager,
                                                            IGraphDB myGraphDB,
                                                            SecurityToken mySecurityToken,
                                                            TransactionToken myTransactionToken)
        {
            if (!String.IsNullOrEmpty(_EdgeName))
            {

                #region Specific edge

                var request = new RequestGetEdgeType(_EdgeName);
                var edge = myGraphDB.GetEdgeType<IEdgeType>(mySecurityToken, myTransactionToken, request, (stats, edgeType) => edgeType);

                if (edge != null)
                {
                    return new List<IVertexView>() { GenerateOutput(edge, _EdgeName) };
                }
                else
                {
                    throw new EdgeTypeDoesNotExistException(_EdgeName, "");
                }

                #endregion

            }
            else
            {

                #region All edges

                var resultingReadouts = new List<VertexView>();

                var request = new RequestGetAllEdgeTypes();
                foreach (var edge in myGraphDB.GetAllEdgeTypes<IEnumerable<IEdgeType>>(mySecurityToken, myTransactionToken, request, (stats, edgeTypes) => edgeTypes))
                {
                    //resultingReadouts.Add(GenerateOutput(edge.Value, edge.Key));
                }


                return resultingReadouts;
                #endregion

            }
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
