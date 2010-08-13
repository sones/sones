/* 
 * GraphDB
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace sones.GraphDB.NewAPI
{

    public class DBPath
    {

        #region Properties

        public DBVertex              StartVertex    { get; protected set; }
        public DBVertex              EndVertex      { get; protected set; }
        public UInt64                Length         { get; protected set; }
        public IEnumerable<DBVertex> Vertices       { get; protected set; }
        public IEnumerable<DBEdge>   Edges          { get; protected set; }

        #endregion

        public DBPath(DBVertex myStartVertex, DBVertex myEndVertex, IEnumerable<DBVertex> myVertices, IEnumerable<DBEdge> myEdges)
        {

            #region Initial Checks

            if (myStartVertex == null)
                throw new ArgumentNullException();

            if (myEndVertex == null)
                throw new ArgumentNullException();

            if (myVertices == null)
                throw new ArgumentNullException();

            if (myEdges == null)
                throw new ArgumentNullException();

            if (myVertices.Count() != myEdges.Count() - 1)
                throw new ArgumentException();

            #endregion

            StartVertex = myStartVertex;
            EndVertex   = myEndVertex;
            Vertices    = myVertices;
            Edges       = myEdges;
            Length      = (UInt64) myEdges.LongCount();

        }

    }

}
