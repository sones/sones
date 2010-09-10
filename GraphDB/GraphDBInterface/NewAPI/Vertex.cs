/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/*
 * sones GraphDS API - DBObject
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

#endregion

namespace sones.GraphDB.NewAPI
{

    /// <summary>
    /// The DBObject class for user-defined graph data base types
    /// </summary>

    public class DBVertex : DBObject, IEquatable<DBVertex>
    {


        #region Properties

        [HideFromDatabase]
        public IDictionary<String, Object> Attributes { get; protected set; }

        #endregion

        #region Constructor(s)

        #region DBVertex()

        public DBVertex()
        {
            UUID        = null;
            Edition     = null;
            RevisionID  = null;
            Attributes  = new Dictionary<String, Object>();
        }

        #endregion

        #region DBVertex(myAttributes)

        public DBVertex(IDictionary<String, Object> myAttributes)
        {
            Attributes = myAttributes;
        }

        #endregion

        #endregion



        #region this[myAttributeName]

        [HideFromDatabase]
        public Object this[String myAttributeName]
        {

            get
            {

                Object _Object = null;

                Attributes.TryGetValue(myAttributeName, out _Object);
                
                return _Object;

            }

        }

        #endregion
        


        #region Graph Operations

        #region Link()

        public Exceptional Link(DBVertex myDBVertex)
        {

            if (myDBVertex == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Link()

        public Exceptional Link(params DBVertex[] myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Link()

        public Exceptional Link(IEnumerable<DBVertex> myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region Unlink()

        public Exceptional Unlink(DBVertex myDBVertex)
        {

            if (myDBVertex == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Unlink()

        public Exceptional Unlink(params DBVertex[] myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region Unlink()

        public Exceptional Unlink(IEnumerable<DBVertex> myDBVertices)
        {

            if (myDBVertices == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region GetNeighbors(myDBVertexQualifier = null, myDepth = 0)

        public IEnumerable<DBVertex> GetNeighbors(Func<DBVertex, Boolean> myDBVertexQualifier = null, UInt64 myDepth = 0)
        {

            if (myDBVertexQualifier == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region GetNeighborCount(myDBVertexQualifier = null, myDepth = 0)

        public UInt64 GetNeighborCount(Func<DBVertex, Boolean> myDBVertexQualifier = null, UInt64 myDepth = 0)
        {

            if (myDBVertexQualifier == null)
                // count all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region GetEdge(myEdgeName)

        public IEnumerable<DBObject> GetEdge(String myEdgeName)
        {

            //ToDo: Rethink me!

            var prop = this.GetType().GetProperty(myEdgeName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                return null;

            var edgeProp = this.GetType().GetProperty(myEdgeName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(this, null) as IList;
            if (edgeProp == null)
                return null;
            //yield break;


            var retVal = new List<DBObject>();

            foreach (var edge in edgeProp)
            {
                retVal.Add(edge as DBObject);
                //yield return edge as DBObject;
            }

            return retVal;

        }

        #endregion

        #region GetEdges(myDBEdgeQualifier = null, myDepth = 0)

        public IEnumerable<DBEdge> GetEdges(Func<DBEdge, Boolean> myDBEdgeQualifier = null, UInt64 myDepth = 0)
        {

            if (myDBEdgeQualifier == null)
                // return all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion

        #region GetEdgeCount(myDBEdgeQualifier = null, myDepth = 0)

        public UInt64 GetEdgeCount(Func<DBEdge, Boolean> myDBEdgeQualifier = null, UInt64 myDepth = 0)
        {

            if (myDBEdgeQualifier == null)
                // count all!
                throw new NotImplementedException();

            throw new NotImplementedException();

        }

        #endregion


        #region Traverse(...)

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
        public T TraversePath<T>(   TraversalOperation                       TraversalOperation  = TraversalOperation.BreathFirst,
                                    Func<DBPath, DBEdge, Boolean>            myFollowThisEdge    = null,
                                    Func<DBPath, DBEdge, DBVertex, Boolean>  myFollowThisPath    = null,
                                    Func<DBPath, Boolean>                    myMatchEvaluator    = null,
                                    Action<DBPath>                           myMatchAction       = null,
                                    Func<TraversalState, Boolean>            myStopEvaluator     = null,
                                    Func<IEnumerable<DBPath>, T>             myWhenFinished      = null)
        {
            return GraphDBInterface.TraversePath(SessionToken, this, TraversalOperation, myFollowThisEdge, myFollowThisPath, myMatchEvaluator, myMatchAction, myStopEvaluator, myWhenFinished);
        }

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
        public T TraverseVertex<T>(TraversalOperation TraversalOperation = TraversalOperation.BreathFirst,
                                    Func<DBVertex, DBEdge, Boolean> myFollowThisEdge = null,
                                    Func<DBVertex, Boolean> myMatchEvaluator = null,
                                    Action<DBVertex> myMatchAction = null,
                                    Func<TraversalState, Boolean> myStopEvaluator = null,
                                    Func<IEnumerable<DBVertex>, T> myWhenFinished = null)
        {
            return GraphDBInterface.TraverseVertex(SessionToken, this, TraversalOperation, myFollowThisEdge, myMatchEvaluator, myMatchAction, myStopEvaluator, myWhenFinished);
        }

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (myDBObject1, myDBVertex2)

        public static Boolean operator == (DBVertex myDBVertex1, DBVertex myDBVertex2)
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

        public static Boolean operator != (DBVertex myDBVertex1, DBVertex myDBVertex2)
        {
            return !(myDBVertex1 == myDBVertex2);
        }

        #endregion

        #endregion

        #region IEquatable<DBObject> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject == null)
                return false;

            var _Object = myObject as DBVertex;
            if (_Object == null)
                return (Equals(_Object));

            return false;

        }

        #endregion

        #region Equals(myDBVertex)

        public Boolean Equals(DBVertex myDBVertex)
        {

            if ((object) myDBVertex == null)
            {
                return false;
            }

            //TODO: Here it might be good to check all attributes of the UNIQUE constraint!
            return (this.UUID == myDBVertex.UUID);

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

            var _ReturnValue = new StringBuilder(Attributes.Count + " Attributes: ");

            foreach (var _KeyValuePair in Attributes)
                _ReturnValue.Append(_KeyValuePair.Key + " = '" + _KeyValuePair.Value + "', ");

            _ReturnValue.Length = _ReturnValue.Length - 2;

            return _ReturnValue.ToString();

        }

        #endregion

    }

}
