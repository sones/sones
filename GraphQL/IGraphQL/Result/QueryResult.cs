using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using sones.GraphDB.Result;

namespace sones.GraphQL.Result
{
    /// <summary>
    /// This class hold all the data that comes out of the database after a query is run
    /// </summary>
    public sealed class QueryResult : IEnumerable<IVertexView>
    {
        #region Data

        /// <summary>
        /// The vertices that are contained in this QueryResult
        /// </summary>
        public IEnumerable<IVertexView> Vertices { get; set; }

        /// <summary>
        /// The query that has been executed
        /// </summary>
        public readonly String Query;

        /// <summary>
        /// The name of the query language that has been executed
        /// </summary>
        public readonly String NameOfQuerylanguage;

        /// <summary>
        /// The time that was spent on executing the query
        /// </summary>
        public readonly UInt64 Duration;

        /// <summary>
        /// The number of affected vertices
        /// </summary>
        public UInt64 NumberOfAffectedVertices
        {
            get
            {

                var _NumberOfAffectedVertices = 0UL;

                if (Vertices != null)
                    _NumberOfAffectedVertices = (UInt64)Vertices.Count();

                return _NumberOfAffectedVertices;

            }
        }

        #endregion

        #region Constructor(s)
        
        /// <summary>
        /// Creates a new query result
        /// </summary>
        /// <param name="myQuery">The query that has been executed</param>
        /// <param name="myQLName">The name of the query language that has been executed</param>
        /// <param name="myDuration">The time that was spent on executing the query</param>
        /// <param name="myVertices">The vertices that should be available within the query result</param>
        public QueryResult(String myQuery, String myQLName, UInt64 myDuration, IEnumerable<IVertexView> myVertices = null)
        {
            Vertices = myVertices != null ? myVertices : new List<IVertexView>();
            Query = myQuery;
            NameOfQuerylanguage = myQLName;
            Duration = myDuration;
        }

        #endregion

        #region IEnumerable<Vertex> Members

        public IEnumerator<IVertexView> GetEnumerator()
        {
            return Vertices.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Vertices.GetEnumerator();
        }

        #endregion

        #region ToString()

        #region ToString()

        public override String ToString()
        {
            return String.Format("Query: {0}{1}Count of affected vertices: {2}", Query, Environment.NewLine, NumberOfAffectedVertices);
        }

        #endregion

        #endregion
    }
}