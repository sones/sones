/*
 * sones GraphDS API - Vertex
 * (c) Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphDB.Result;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.NewAPI
{

    /// <summary>
    /// The vertex class for all user-defined vertex types.
    /// </summary>
    public class Vertex : DBObject, IVertex
    {


        #region Constructor(s)

        #region Vertex()

        public Vertex()
        {
        }

        #endregion

        #region Vertex(myAttributes)

        public Vertex(IDictionary<String, Object> myAttributes)
            : this()
        {

            if (myAttributes != null && myAttributes.Any())
                AddAttribute(myAttributes);

        }

        #endregion

        #endregion


        #region Attributes ( == Properties + Edges)

        #region HasAttribute(myAttributeName)

        public Boolean HasAttribute(String myAttributeName)
        {
            return _Attributes.ContainsKey(myAttributeName);
        }

        #endregion

        #region HasAttribute(myAttributeNameFilter)

        public Boolean HasAttribute(Func<String, Boolean> myAttributeNameFilter)
        {

            if (myAttributeNameFilter == null)
                return _Attributes.Any();

            foreach (var _Key in _Attributes.Keys)
                if (myAttributeNameFilter(_Key))
                    return true;

            return false;

        }

        #endregion


        #region Attributes(myAttributeFilter = null)

        public IEnumerable<KeyValuePair<String, Object>> Attributes(Func<String, Object, Boolean> myAttributeFilter = null)
        {

            foreach (var _KeyValuePair in _Attributes)
            {

                if (myAttributeFilter == null)
                    yield return _KeyValuePair;

                else if (myAttributeFilter(_KeyValuePair.Key, _KeyValuePair.Value))
                    yield return _KeyValuePair;

            }

        }

        #endregion


        #region Count

        public UInt64 Count
        {
            get
            {
                return (UInt64) _Attributes.Count;
            }
        }

        #endregion

        #endregion

        #region Properties

        #region HasProperty(myPropertyName)

        public Boolean HasProperty(String myPropertyName)
        {

            var _Property = GetProperty(myPropertyName);

            if (_Property == null)
                return false;

            if (_Property as Vertex != null)
                return false;

            if (_Property as IEnumerable<Vertex> != null)
                return false;

            return true;

        }

        #endregion

        #region HasProperty(myPropertyFilter = null)

        public Boolean HasProperty(Func<String, Object, Boolean> myPropertyFilter = null)
        {
            return GetProperties(myPropertyFilter).Any();
        }

        #endregion


        #region GetProperty(myPropertyName)

        public Object GetProperty(String myPropertyName)
        {

            Object _Object = null;

            _Attributes.TryGetValue(myPropertyName, out _Object);

            return _Object;

        }

        #endregion

        #region GetProperties(myPropertyFilter = null)

        public IEnumerable<Object> GetProperties(Func<String, Object, Boolean> myPropertyFilter = null)
        {

            foreach (var _KeyValuePair in _Attributes)
            {

                if (_KeyValuePair.Value != null)
                {

                    if (myPropertyFilter == null)
                        yield return _KeyValuePair.Value;

                    else if (myPropertyFilter(_KeyValuePair.Key, _KeyValuePair.Value))
                        yield return _KeyValuePair.Value;

                }

            }

        }

        #endregion


        #region GetProperty<T>(myPropertyName)

        public T GetProperty<T>(String myPropertyName)
        {

            Object _Object = null;

            _Attributes.TryGetValue(myPropertyName, out _Object);

            return (T) _Object;

        }

        #endregion

        #region GetProperties<T>(myPropertyFilter = null)

        public IEnumerable<T> GetProperties<T>(Func<String, Object, Boolean> myPropertyFilter = null)
        {

            Boolean _ExceptionOccured = false;
            T       _T                = default(T);

            foreach (var _KeyValuePair in _Attributes)
            {

                _ExceptionOccured = false;

                try
                {
                    _T = (T) _KeyValuePair.Value;
                }
                catch (Exception)
                {
                    _ExceptionOccured = true;
                }

                if (_ExceptionOccured == false && _T != null)
                {

                    if (myPropertyFilter == null)
                        yield return _T;

                    else if (myPropertyFilter(_KeyValuePair.Key, _T))
                        yield return _T;

                }

            }

        }

        #endregion


        #region GetStringProperty(myPropertyName)

        public String GetStringProperty(String myPropertyName)
        {

            Object _Object = null;

            _Attributes.TryGetValue(myPropertyName, out _Object);

            return _Object as String;

        }

        #endregion

        #region GetStringProperty(myPropertyFilter = null)

        public IEnumerable<String> GetStringProperty(Func<String, String, Boolean> myPropertyFilter = null)
        {

            foreach (var _KeyValuePair in _Attributes)
            {

                var _String = _KeyValuePair.Value as String;

                if (_String != null)
                {

                    if (myPropertyFilter == null)
                        yield return _String;

                    else if (myPropertyFilter(_KeyValuePair.Key, _String))
                        yield return _String;

                }

            }

        }

        #endregion

        #endregion

        #region Edges

        #region HasEdge(myEdgeName)

        public Boolean HasEdge(String myEdgeName)
        {

            var _Edge = GetProperty(myEdgeName);

            if (_Edge == null)
                return false;

            if (_Edge as Vertex != null)
                return true;

            if (_Edge as IEnumerable<Vertex> != null)
                return true;

            return false;

        }

        #endregion

        #region HasEdge(myEdgeFilter = null)

        public Boolean HasEdge(Func<String, IEdge, Boolean> myEdgeFilter = null)
        {
            return GetEdges(myEdgeFilter).Any();
        }

        #endregion


        #region GetEdge(myEdgeName)

        public IEdge GetEdge(String myEdgeName)
        {
            return GetProperty(myEdgeName) as Edge;
        }

        #endregion

        #region GetEdges(myEdgeName)

        public IEnumerable<IEdge> GetEdges(String myEdgeName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetEdges(myEdgeFilter = null)

        public IEnumerable<IEdge> GetEdges(Func<String, IEdge, Boolean> myEdgeFilter = null)
        {

            foreach (var _KeyValuePair in _Attributes)
            {

                var _Edge = _KeyValuePair.Value as Edge;

                if (_Edge != null)
                {

                    if (myEdgeFilter == null)
                        yield return _Edge;

                    else if (myEdgeFilter(_KeyValuePair.Key, _Edge))
                        yield return _Edge;

                }

            }

        }

        #endregion


        #region GetEdge<TEdge>(myEdgeName)

        public TEdge GetEdge<TEdge>(String myEdgeName)
            where TEdge : class, IEdge
        {
            return GetEdge(myEdgeName) as TEdge;
        }

        #endregion

        #region GetEdges<TEdge>(myEdgeName)

        public IEnumerable<TEdge> GetEdges<TEdge>(String myEdgeName)
            where TEdge : class, IEdge
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetEdges<TEdge>(myEdgeFilter = null)

        public IEnumerable<TEdge> GetEdges<TEdge>(Func<String, IEdge, Boolean> myEdgeFilter = null)
            where TEdge : class, IEdge
        {

            foreach (var _KeyValuePair in _Attributes)
            {

                var _Edge = _KeyValuePair.Value as Edge;

                if (_Edge != null)
                {

                    if (myEdgeFilter == null)
                        yield return _Edge as TEdge;

                    else if (myEdgeFilter(_KeyValuePair.Key, _Edge))
                        yield return _Edge as TEdge;

                }

            }

        }

        #endregion

        #endregion

        #region Neighbors

        #region GetNeighbor(myEdgeName)

        public IVertex GetNeighbor(String myEdgeName)
        {

            Object _Object = null;

            _Attributes.TryGetValue(myEdgeName, out _Object);

            var _Edge = _Object as Edge;
            if (_Edge != null)
                return _Edge.TargetVertices.First();

            return null;

        }

        #endregion

        #region GetNeighbors(myEdgeName)

        public IEnumerable<IVertex> GetNeighbors(String myEdgeName)
        {

            Object _Object = null;

            _Attributes.TryGetValue(myEdgeName, out _Object);

            var _Edge = _Object as Edge;
            if (_Edge != null)
                return _Edge.TargetVertices;

            return null;

        }

        #endregion

        #region GetNeighbors(myVertexFilter = null)

        public IEnumerable<IVertex> GetNeighbors(Func<String, IVertex, Boolean> myVertexFilter = null)
        {

            foreach (var _KeyValuePair in _Attributes)
            {

                var _Edge = _KeyValuePair.Value as Edge;

                if (_Edge != null)
                {

                    foreach (var _TargetVertex in _Edge.TargetVertices)
                    {

                        if (myVertexFilter == null)
                            yield return _TargetVertex;

                        else if (myVertexFilter(_KeyValuePair.Key, _TargetVertex))
                            yield return _TargetVertex;

                    }

                }

            }

        }

        #endregion

        #region GetNeighbors(myEdgeFilter = null)

        public IEnumerable<IVertex> GetNeighbors(Func<String, IEdge, Boolean> myEdgeFilter = null)
        {

            foreach (var _KeyValuePair in _Attributes)
            {

                var _Edge = _KeyValuePair.Value as Edge;

                if (_Edge != null)
                {

                    if (myEdgeFilter == null)
                        foreach (var _TargetVertex in _Edge.TargetVertices)
                            yield return _TargetVertex;

                    else if (myEdgeFilter(_KeyValuePair.Key, _Edge as IEdge))
                        foreach (var _TargetVertex in _Edge.TargetVertices)
                            yield return _TargetVertex;

                }

            }

        }

        #endregion


        #region GetNeighbor<TVertex>(myEdgeName)

        public TVertex GetNeighbor<TVertex>(String myEdgeName)
            where TVertex : class, IVertex
        {

            Object _Object = null;

            _Attributes.TryGetValue(myEdgeName, out _Object);

            var _Edge = _Object as Edge;
            if (_Edge != null)
            {
                var _Vertices = _Edge as IEnumerable<IVertex>;
                if (_Vertices != null)
                    return _Vertices.First() as TVertex;
            }

            return default(TVertex);

        }

        #endregion

        #region GetNeighbors<TVertex>(myEdgeName)

        public IEnumerable<TVertex> GetNeighbors<TVertex>(String myEdgeName)
            where TVertex : class, IVertex
        {

            Object _Object = null;

            _Attributes.TryGetValue(myEdgeName, out _Object);

            return _Object as Edge as IEnumerable<TVertex>;

        }

        #endregion

        #region GetNeighbors<TVertex>(myVertexFilter = null)

        public IEnumerable<TVertex> GetNeighbors<TVertex>(Func<String, IVertex, Boolean> myVertexFilter = null)
            where TVertex : class, IVertex
        {

            if (myVertexFilter == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region GetNeighbors<TVertex>(myEdgeFilter = null)

        public IEnumerable<TVertex> GetNeighbors<TVertex>(Func<String, IEdge, Boolean> myEdgeFilter = null)
            where TVertex : class, IVertex
        {

            if (myEdgeFilter == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #endregion

        #region Link/Unlink

        #region Link(myTargetVertex)

        public Exceptional Link(IVertex myTargetVertex)
        {

            if (myTargetVertex == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Link(myIVertices)

        public Exceptional Link(params IVertex[] myTargetVertices)
        {

            if (myTargetVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Link(myTargetVertices)

        public Exceptional Link(IEnumerable<IVertex> myTargetVertices)
        {

            if (myTargetVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region Unlink(myTargetVertex)

        public Exceptional Unlink(IVertex myTargetVertex)
        {

            if (myTargetVertex == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Unlink(myTargetVertices)

        public Exceptional Unlink(params IVertex[] myTargetVertices)
        {

            if (myTargetVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Unlink(myTargetVertices)

        public Exceptional Unlink(IEnumerable<IVertex> myTargetVertices)
        {

            if (myTargetVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #endregion

        #region Operator overloading

        #region Operator == (myDBObject1, myDBVertex2)

        public static Boolean operator == (Vertex myDBVertex1, Vertex myDBVertex2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(myDBVertex1, myDBVertex2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myDBVertex1 == null) || ((Object) myDBVertex2 == null))
                return false;

            return myDBVertex1.Equals(myDBVertex2);

        }

        #endregion

        #region Operator != (myDBVertex1, myDBVertex2)

        public static Boolean operator != (Vertex myDBVertex1, Vertex myDBVertex2)
        {
            return !(myDBVertex1 == myDBVertex2);
        }

        #endregion

        #endregion

        #region IEquatable<IVertex> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject == null)
                return false;

            var _Object = myObject as IVertex;
            if (_Object != null)
                return (Equals(_Object));

            return false;

        }

        #endregion

        #region Equals(myIVertex)

        public Boolean Equals(IVertex myIVertex)
        {

            if ((object) myIVertex == null)
            {
                return false;
            }

            //TODO: Here it might be good to check all attributes of the UNIQUE constraint!
            return (this.UUID == myIVertex.UUID);

        }

        #endregion

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            var _ReturnValue = new StringBuilder(_Attributes.Count + " Attributes: ");

            foreach (var _KeyValuePair in _Attributes)
                _ReturnValue.Append(_KeyValuePair.Key + " = '" + _KeyValuePair.Value + "', ");

            _ReturnValue.Length = _ReturnValue.Length - 2;

            return _ReturnValue.ToString();

        }

        #endregion
       
    }

}
