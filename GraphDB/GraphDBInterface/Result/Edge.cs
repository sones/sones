/*
 * sones GraphDS API - Edge
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Result
{

    /// <summary>
    /// Old and maybe new Edge class for graphdb and user-defined edge types.
    /// May get more and more functionality from DBEdgeNew.
    /// </summary>
    public class Edge : IEnumerable<Vertex>, IEnumerable<EdgeLabel>, IEquatable<Edge>
    {

        #region Properties

        #region SourceVertex

        [HideFromDatabase]
        public Vertex SourceVertex { get; private set; }

        #endregion

        #region TargetVertex

        [HideFromDatabase]
        public Vertex TargetVertex
        {

            get
            {
                return _TargetVertices.FirstOrDefault();
            }

            //set
            //{
            //    _TargetVertices.Clear();
            //    _TargetVertices.Add(value);
            //}

        }

        #endregion

        #region TargetVertices

        //ToDo: Maybe better use a HashSet<Vertex>!
        private IEnumerable<Vertex> _TargetVertices;

        [HideFromDatabase]
        public IEnumerable<Vertex> TargetVertices
        {
            get
            {
                return _TargetVertices;
            }
        }

        #endregion

        #region EdgeTypeName

        public String EdgeTypeName { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        #region Edge(mySourceVertex, myTargetVertex, myEdgeTypeName = "")

        public Edge(Vertex mySourceVertex, Vertex myTargetVertex, String myEdgeTypeName = "")
        {
            SourceVertex    = mySourceVertex;
            _TargetVertices = new List<Vertex> { myTargetVertex };
            EdgeTypeName    = myEdgeTypeName;
        }

        #endregion

        #region Edge(mySourceVertex, myTargetVertices, myEdgeTypeName = "")

        public Edge(Vertex mySourceVertex, IEnumerable<Vertex> myTargetVertices, String myEdgeTypeName = "")
        {
            SourceVertex    = mySourceVertex;
            _TargetVertices = myTargetVertices;
            EdgeTypeName    = myEdgeTypeName;
        }

        #endregion

        #endregion


        #region this[myElementAt]

        public Vertex this[Int32 myElementAt]
        {
            get
            {
                return _TargetVertices.ElementAtOrDefault(myElementAt);
            }
        }

        #endregion

        #region CompareVertices(myVertices1, myVertices2)

        private Boolean CompareVertices(IEnumerable<Vertex> myVertices1, IEnumerable<Vertex> myVertices2)
        {

            if (myVertices1.Count() != myVertices2.Count())
            {
                return false;
            }

            if (myVertices1.Count() == 0)
            {
                return true;
            }

            foreach (var aOuterResult in myVertices1)
            {
                var equals = myVertices2.Contains(aOuterResult);

                if (!equals)
                {
                    return false;
                }
            }

            return true;

        }

        #endregion



        #region Operator overloading

        #region Operator == (myEdge1, myEdge2)

        public static Boolean operator == (Edge myEdge1, Edge myEdge2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(myEdge1, myEdge2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myEdge1 == null) || ((Object) myEdge2 == null))
                return false;

            return myEdge1.Equals(myEdge2);

        }

        #endregion

        #region Operator != (myEdge1, myEdge2)

        public static Boolean operator != (Edge myEdge1, Edge myEdge2)
        {
            return !(myEdge1 == myEdge2);
        }

        #endregion

        #endregion

        #region IEnumerable.GetEnumerator()

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _TargetVertices.GetEnumerator();
        }

        #endregion

        #region IEnumerable<Vertex> Members

        IEnumerator<Vertex> IEnumerable<Vertex>.GetEnumerator()
        {
            return _TargetVertices.GetEnumerator();
        }

        #endregion

        #region IEnumerable<EdgeLabel> Members

        IEnumerator<EdgeLabel> IEnumerable<EdgeLabel>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<Edge> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject == null)
                return false;

            var _Object = myObject as Edge;
            if (_Object == null)
                return (Equals(_Object));

            return false;

        }

        #endregion

        #region Equals(myDBObject)

        public Boolean Equals(Edge myEdge)
        {

            if ((Object) myEdge == null)
                return false;

            //TODO: Here it might be good to check all vertices and
            //      all edge attributes of the UNIQUE constraint!
            //return (this.UUID == myDBEdge.UUID);

            return (EdgeTypeName == myEdge.EdgeTypeName) &&
                CompareVertices(_TargetVertices, myEdge._TargetVertices);

        }

        #endregion

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return EdgeTypeName.GetHashCode();
        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            String _SourceVertexString = "<null>";
            
            if (SourceVertex != null)
                _SourceVertexString = SourceVertex.ToString();

            var _ReturnValue = String.Format("[{0}] SourceVertex {1} -> ", EdgeTypeName, _SourceVertexString);

            foreach (var _Vertex in _TargetVertices)
                _ReturnValue += _Vertex.ToString() + ", ";

            _ReturnValue.Substring(0, _ReturnValue.Length - 2);

            return _ReturnValue;

        }

        #endregion

    }

}
