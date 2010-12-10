/*
 * sones GraphDS API - IVertex
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.NewAPI
{

    public interface IVertex : IEquatable<IVertex>, IEnumerable<KeyValuePair<String, Object>>
    {

        #region IVertex Properties

        ObjectUUID           UUID                   { get; set; }
        String               TYPE                   { get; }
        String               EDITION                { get; set; }
        ObjectRevisionID     REVISIONID             { get; set; }
        String               Comment                { get; set; }

        #endregion
        
        #region Attributes ( == Properties + Edges)

        Boolean              HasAttribute           (String myAttributeName);
        Boolean              HasAttribute           (Func<String, Boolean> myAttributeNameFilter = null);

        IEnumerable<KeyValuePair<String, Object>>
                             Attributes             (Func<String, Object, Boolean> myAttributeFilter = null);

        UInt64               Count                  { get; }

        #endregion

        #region Properties

        Boolean              HasProperty            (String myPropertyName);
        Boolean              HasProperty            (Func<String, Object, Boolean> myPropertyFilter = null);

        Object               GetProperty            (String myPropertyName);
        IEnumerable<Object>  GetProperties          (Func<String, Object, Boolean> myPropertyFilter = null);

        T                    GetProperty<T>         (String myPropertyName);

        IEnumerable<T>       GetProperties<T>       (Func<String, Object, Boolean> myPropertyFilter = null);

        String               GetStringProperty      (String myPropertyName);
        IEnumerable<String>  GetStringProperty      (Func<String, String, Boolean> myPropertyFilter = null);

        #endregion

        #region Edges

        Boolean              HasEdge                (String myEdgeName);
        Boolean              HasEdge                (Func<String, IEdge, Boolean> myEdgeFilter = null);

        IEdge                GetEdge                (String myEdgeName);
        IEnumerable<IEdge>   GetEdges               (String myEdgeName);
        IEnumerable<IEdge>   GetEdges               (Func<String, IEdge, Boolean> myEdgeFilter = null);

        TEdge                GetEdge<TEdge>         (String myEdgeName) where TEdge : class, IEdge;
        IEnumerable<TEdge>   GetEdges<TEdge>        (String myEdgeName) where TEdge : class, IEdge;
        IEnumerable<TEdge>   GetEdges<TEdge>        (Func<String, IEdge, Boolean> myEdgeFilter = null) where TEdge : class, IEdge;

        #endregion

        #region Neighbors

        IVertex              GetNeighbor            (String myEdgeName);
        IEnumerable<IVertex> GetNeighbors           (String myEdgeName);
        IEnumerable<IVertex> GetNeighbors           (Func<String, IVertex, Boolean> myVertexFilter = null);
        IEnumerable<IVertex> GetNeighbors           (Func<String, IEdge,   Boolean> myEdgeFilter   = null);

        TVertex              GetNeighbor<TVertex>   (String myEdgeName) where TVertex : class, IVertex;
        IEnumerable<TVertex> GetNeighbors<TVertex>  (String myEdgeName) where TVertex : class, IVertex;
        IEnumerable<TVertex> GetNeighbors<TVertex>  (Func<String, IVertex, Boolean> myVertexFilter = null) where TVertex : class, IVertex;
        IEnumerable<TVertex> GetNeighbors<TVertex>  (Func<String, IEdge,   Boolean> myEdgeFilter   = null) where TVertex : class, IVertex;

        #endregion

        #region Link/Unlink

        Exceptional Link     (IVertex              myTargetVertex);
        Exceptional Link     (params IVertex[]     myTargetVertices);
        Exceptional Link     (IEnumerable<IVertex> myTargetVertices);


        Exceptional Unlink   (IVertex              myTargetVertex);
        Exceptional Unlink   (params IVertex[]     myTargetVertices);
        Exceptional Unlink   (IEnumerable<IVertex> myTargetVertices);

        #endregion

    }

}
