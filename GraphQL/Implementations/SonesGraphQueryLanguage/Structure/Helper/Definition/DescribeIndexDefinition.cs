using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphDB.Request.GetIndex;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
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



        public override QueryResult GetResult(
                                                GQLPluginManager myPluginManager,
                                                IGraphDB myGraphDB,
                                                SecurityToken mySecurityToken,
                                                TransactionToken myTransactionToken)
        {
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

            if(error != null)
                return new QueryResult("", "GQL", 0L, ResultType.Failed, resultingVertices, error);
            else
                return new QueryResult("", "GQL", 0L, ResultType.Successful, resultingVertices);
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
            var _Prop = new Dictionary<String, Object>();

            foreach (var index in myIndices)
            {

                _Index.Add("Name", myName);
                _Index.Add("ID", index.ID);
                _Index.Add("Edition", index.Edition);
                _Index.Add("IndexTypeName", index.IndexTypeName);
                _Index.Add("IsUserDefinied", index.IsUserdefined);

                foreach (var prop in index.IndexedProperties)
                {
                    _Prop.Add("Name", prop.Name);
                    _Prop.Add("DefaultValue", prop.DefaultValue);
                    _Prop.Add("IsUserDefined", prop.IsUserDefinedType);
                    _Prop.Add("Multiplicity", prop.Multiplicity);
                }

                var temp = new Dictionary<String, IEdgeView> { {"IndexedProperty", new EdgeView(_Prop, null)} };
            }

            return new VertexView(_Index, new Dictionary<String, IEdgeView>());
        }

        #endregion
    }
}
