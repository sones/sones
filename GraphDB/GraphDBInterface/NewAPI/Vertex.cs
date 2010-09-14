/*
 * sones GraphDS API - Vertex
 * (c) Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Structures;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.NewAPI
{

    /// <summary>
    /// The Vertex class for all user-defined vertex types
    /// </summary>
    public class Vertex : DBObject, IEquatable<Vertex>
    {

        #region Properties

        #region Count

        public UInt64 Count
        {
            get
            {
                return (UInt64) _Attributes.Count();
            }
        }

        #endregion

        #endregion

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


        #region Graph operations

        #region IsAttribute(myAttributeName)

        public Boolean IsAttribute(String myAttributeName)
        {
            return _Attributes.ContainsKey(myAttributeName);
        }

        #endregion

        #region IsProperty(myAttributeName)

        public Boolean IsProperty(String myAttributeName)
        {

            var _Attribute = GetProperty(myAttributeName);

            if (_Attribute == null)
                return false;

            if (_Attribute as Vertex != null)
                return false;

            if (_Attribute as IEnumerable<Vertex> != null)
                return false;

            return true;

        }

        #endregion

        #region IsEdge(myAttributeName)

        public Boolean IsEdge(String myAttributeName)
        {

            var _Attribute = GetProperty(myAttributeName);

            if (_Attribute == null)
                return false;

            if (_Attribute as Vertex != null)
                return true;

            if (_Attribute as IEnumerable<Vertex> != null)
                return true;

            return false;

        }

        #endregion


        #region GetProperty(myAttributeName)

        public Object GetProperty(String myAttributeName, Func<Object, Boolean> myPropertyQualifier = null)
        {

            Object _Object = null;

            _Attributes.TryGetValue(myAttributeName, out _Object);

            return _Object;

        }

        #endregion

        #region GetProperty<T>(myAttributeName)

        public T GetProperty<T>(String myAttributeName, Func<T, Boolean> myPropertyQualifier = null)
        {

            Object _Object = null;

            _Attributes.TryGetValue(myAttributeName, out _Object);

            return (T)_Object;

        }

        #endregion

        #region GetStringProperty(myAttributeName)

        public String GetStringProperty(String myAttributeName, Func<Object, Boolean> myPropertyQualifier = null)
        {

            Object _Object = null;

            _Attributes.TryGetValue(myAttributeName, out _Object);

            return _Object as String;

        }

        #endregion


        #region GetNeighbor(myAttributeName)

        public Vertex GetNeighbor(String myAttributeName)
        {

            Object _Object = null;

            _Attributes.TryGetValue(myAttributeName, out _Object);

            var _Edge = _Object as Edge;
            if (_Edge != null)
            {
                var _Vertices = _Edge as IEnumerable<Vertex>;
                if (_Vertices != null)
                    return _Vertices.First();
            }

            return null;

        }

        #endregion

        #region GetNeighbor<T>(myAttributeName)

        public T GetNeighbor<T>(String myAttributeName)
            where T : Vertex
        {

            Object _Object = null;

            _Attributes.TryGetValue(myAttributeName, out _Object);

            var _Edge = _Object as Edge;
            if (_Edge != null)
            {
                var _Vertices = _Edge as IEnumerable<Vertex>;
                if (_Vertices != null)
                    return _Vertices.First() as T;
            }

            return null;

        }

        #endregion

        #region GetNeighbors(myAttributeName)

        public IEnumerable<Vertex> GetNeighbors(String myAttributeName)
        {

            Object _Object = null;

            _Attributes.TryGetValue(myAttributeName, out _Object);

            return _Object as Edge;

        }

        #endregion

        #region GetNeighbors<T>(myAttributeName)

        public IEnumerable<T> GetNeighbors<T>(String myAttributeName)
            where T : Vertex
        {

            Object _Object = null;

            _Attributes.TryGetValue(myAttributeName, out _Object);

            return _Object as Edge as IEnumerable<T>;

        }

        #endregion


        #region GetAllNeighbors(myVertexQualifier = null)

        public IEnumerable<Vertex> GetAllNeighbors(Func<Vertex, Boolean> myVertexQualifier = null)
        {

            if (myVertexQualifier == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region GetEdge(myAttributeName)

        public IEnumerable<EdgeLabel> GetEdgeInfo(String myAttributeName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetEdges(myDBEdgeQualifier = null)

        public IEnumerable<EdgeLabel> GetEdgeInfo(Func<EdgeLabel, Boolean> myDBEdgeQualifier = null)
        {

            if (myDBEdgeQualifier == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #endregion

        #region Special Graph Operations

        #region Link()

        public Exceptional Link(Vertex myDBVertex)
        {

            if (myDBVertex == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Link()

        public Exceptional Link(params Vertex[] myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Link()

        public Exceptional Link(IEnumerable<Vertex> myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region Unlink()

        public Exceptional Unlink(Vertex myDBVertex)
        {

            if (myDBVertex == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Unlink()

        public Exceptional Unlink(params Vertex[] myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Unlink()

        public Exceptional Unlink(IEnumerable<Vertex> myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region TraversePath(...)

        /// <summary>
        /// Starts a traversal and returns the found paths or an aggreagted result
        /// </summary>
        /// <typeparam name="T">The resulttype after applying the result transformation</typeparam>
        /// <param name="TraversalOperation">BreathFirst|DepthFirst</param>
        /// <param name="myFollowThisEdge">Follow this edge? Based on its TYPE or any other property/characteristic...</param>
        /// <param name="myFollowThisPath">Follow this path (== actual path + NEW edge + NEW dbobject? Based on edge/object UUID, TYPE or any other property/characteristic...</param>
        /// <param name="myMatchEvaluator">Mhm, this vertex/path looks interesting!</param>
        /// <param name="myMatchAction">Hey! I have found something interesting!</param>
        /// <param name="myStopEvaluator">Will stop the traversal on a condition</param>
        /// <param name="myWhenFinished">Finish this traversal by calling (a result transformation method and) an external method...</param>
        /// <returns></returns>
        public T TraversePath<T>(TraversalOperation                         TraversalOperation = TraversalOperation.BreathFirst,
                                 Func<DBPath, EdgeLabel, Boolean>           myFollowThisEdge = null,
                                 Func<DBPath, EdgeLabel, Vertex, Boolean>   myFollowThisPath = null,
                                 Func<DBPath, Boolean>                      myMatchEvaluator = null,
                                 Action<DBPath>                             myMatchAction = null,
                                 Func<TraversalState, Boolean>              myStopEvaluator = null,
                                 Func<IEnumerable<DBPath>, T>               myWhenFinished = null)
        {
            return GraphDBInterface.TraversePath(SessionToken, this, TraversalOperation, myFollowThisEdge, myFollowThisPath, myMatchEvaluator, myMatchAction, myStopEvaluator, myWhenFinished);
        }

        #endregion

        #region TraverseVertex(...)

        /// <summary>
        /// Starts a traversal and returns the found vertices or an aggreagted result
        /// </summary>
        /// <typeparam name="T">The resulttype after applying the result transformation</typeparam>
        /// <param name="TraversalOperation">BreathFirst|DepthFirst</param>
        /// <param name="myFollowThisEdge">Follow this edge? Based on its TYPE or any other property/characteristic...</param>
        /// <param name="myMatchEvaluator">Mhm, this vertex/path looks interesting!</param>
        /// <param name="myMatchAction">Hey! I have found something interesting!</param>
        /// <param name="myStopEvaluator">Will stop the traversal on a condition</param>
        /// <param name="myWhenFinished">Finish this traversal by calling (a result transformation method and) an external method...</param>
        /// <returns></returns>
        public T TraverseVertex<T>(TraversalOperation                       TraversalOperation = TraversalOperation.BreathFirst,
                                    Func<Vertex, EdgeLabel, Boolean>        myFollowThisEdge = null,
                                    Func<Vertex, Boolean>                   myMatchEvaluator = null,
                                    Action<Vertex>                          myMatchAction = null,
                                    Func<TraversalState, Boolean>           myStopEvaluator = null,
                                    Func<IEnumerable<Vertex>, T>            myWhenFinished = null)
        {
            return GraphDBInterface.TraverseVertex(SessionToken, this, TraversalOperation, myFollowThisEdge, myMatchEvaluator, myMatchAction, myStopEvaluator, myWhenFinished);
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

        #region IEquatable<Vertex> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject == null)
                return false;

            var _Object = myObject as Vertex;
            if (_Object == null)
                return (Equals(_Object));

            return false;

        }

        #endregion

        #region Equals(myVertex)

        public Boolean Equals(Vertex myVertex)
        {

            if ((object) myVertex == null)
            {
                return false;
            }

            //TODO: Here it might be good to check all attributes of the UNIQUE constraint!
            return (this.UUID == myVertex.UUID);

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
