/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System.Linq;
using sones.Library.ErrorHandling;
using sones.Library.CollectionWrapper;
using sones.GraphDB.Request;

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

        public DescribeIndexDefinition(String myTypeName, String myIndexName, String myIndexEdition)
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
                                                Int64 myTransactionToken)
        {
            IEnumerable<IVertexView> resultingVertices = new List<IVertexView>();
            ASonesException error = null;

            #region Specific index

            var request = new RequestDescribeIndex(_TypeName, _IndexName, _IndexEdition);

            IEnumerable<IIndexDefinition> indices = null;

            if (request.IndexName == String.Empty && request.TypeName == String.Empty && request.Edition == String.Empty)
            {
                indices = myGraphDB.DescribeIndices<IEnumerable<IIndexDefinition>>(mySecurityToken, myTransactionToken, request, (stats, definitions) => definitions);                
            }
            else
            {
                indices = myGraphDB.DescribeIndex<IEnumerable<IIndexDefinition>>(mySecurityToken, myTransactionToken, request, (stats, definitions) => definitions);
            }

            if (indices == null)
            {
                error = new IndexTypeDoesNotExistException(_TypeName, _IndexName);
            }

            if (String.IsNullOrEmpty(_IndexEdition))
            {
                //_IndexEdition = DBConstants.DEFAULTINDEX;
            }

            resultingVertices = GenerateOutput(indices, _IndexName);

            #endregion

            if(error != null)
                return new QueryResult("", SonesGQLConstants.GQL, 0L, ResultType.Failed, resultingVertices, error);
            else
                return new QueryResult("", SonesGQLConstants.GQL, 0L, ResultType.Successful, resultingVertices);
        }

        #region Output

        /// <summary>
        /// Generate an output for an index
        /// </summary>
        /// <param name="myIndex">The index</param>
        /// <param name="myName">The index name</param>
        /// <returns>List of readouts which contain the index information</returns>
        private IEnumerable<IVertexView> GenerateOutput(IEnumerable<IIndexDefinition> myIndices, String myName)
        {

            var _Index = new Dictionary<String, Object>();
            var result = new List<IVertexView>();

            foreach (var index in myIndices)
            {

                _Index.Add("Name", index.Name);
                _Index.Add("ID", index.ID);
                _Index.Add("Edition", index.Edition);
                _Index.Add("IndexTypeName", index.IndexTypeName);
                _Index.Add("IsUserDefinied", index.IsUserdefined);

                ListCollectionWrapper list = new ListCollectionWrapper();

                foreach (var prop in index.IndexedProperties)
                {
                    list.Add(prop.Name);
                }

                _Index.Add("IndexedProperties", list);
                
                result.Add(new VertexView(new Dictionary<String, Object>(_Index), new Dictionary<String, IEdgeView>()));
                _Index.Clear();
            }

            return result;
        }

        #endregion
    }
}
