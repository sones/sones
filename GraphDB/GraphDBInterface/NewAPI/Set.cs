/*
 * sones GraphDS API - Set
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace sones.GraphDB.NewAPI
{

    #region Set<TVertexType>

    public class Set<TVertexType> : HashSet<TVertexType> 
    { }

    #endregion

    #region Set<TVertexType, TEdgeType>

    public class Set<TVertexType, TEdgeType> : HashSet<TVertexType>
        where TVertexType : Vertex
        where TEdgeType   : Edge
    { }

    #endregion

}
