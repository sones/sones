using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;
using Irony.Parsing;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphDB;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphDB.TypeSystem;
using System.Diagnostics;
using sones.GraphDB.Request.GetIndex;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    public sealed class DescribeIndexDefinition : ADescribeDefinition
    {
        #region Data

        private String _IndexEdition;
        private String _TypeName;
        private String _IndexName;

        #endregion

        #region Ctor

        public DescribeIndexDefinition(String myTypeName, string myIndexName, String myIndexEdition)
        {
            _TypeName = myTypeName;
            _IndexName = myIndexName;
            _IndexEdition = myIndexEdition;
        }

        #endregion



        public override QueryResult GetResult(ParsingContext myContext,
                                                GQLPluginManager myPluginManager,
                                                IGraphDB myGraphDB,
                                                SecurityToken mySecurityToken,
                                                TransactionToken myTransactionToken)
        {
            var sw = new Stopwatch();

            sw.Reset();
            sw.Start();

            var resultingVertices = new List<IVertexView>();
            ASonesException error = null;

            #region Specific index

            var request = new RequestDescribeIndex(_TypeName, _IndexName, _IndexEdition);
            
            var indices = myGraphDB.DescribeIndex<IEnumerable<IIndexDefinition>>(mySecurityToken, myTransactionToken, request, (stats, definitions) => definitions);

            if (indices == null)
            {
                error = new IndexTypeDoesNotExistException(_TypeName, _IndexName);
            }

            if (String.IsNullOrEmpty(_IndexEdition))
            {
                //_IndexEdition = DBConstants.DEFAULTINDEX;
            }

            resultingVertices = new List<IVertexView>() { GenerateOutput(indices, _IndexName) };

            #endregion

            sw.Stop();

            return new QueryResult("", "GQL", (ulong)sw.ElapsedMilliseconds, ResultType.Successful, resultingVertices, error);
        }

        #region Output

        /// <summary>
        /// Generate an output for an index
        /// </summary>
        /// <param name="myIndex">The index</param>
        /// <param name="myName">The index name</param>
        /// <returns>List of readouts which contain the index information</returns>
        private IVertexView GenerateOutput(IEnumerable<IIndexDefinition> myIndices, String myName)
        {

            var _Index = new Dictionary<String, Object>();

            foreach (var index in myIndices)
            {

                _Index.Add("Name", myName);
                _Index.Add("ID", index.ID);
                _Index.Add("Edition", index.Edition);
                _Index.Add("IndexTypeName", index.IndexTypeName);
                _Index.Add("IsUserDefinied", index.IsUserdefined);

                foreach (var prop in index.IndexedProperties)
                {
                    _Index.Add("IndexedProperty", prop);
                }

            }

            return new VertexView(_Index, new Dictionary<String, IEdgeView>());

        }

        #endregion
    }
}
