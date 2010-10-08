/*
 * GraphDSSharp - TraverserExtensions
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using sones.GraphDB.NewAPI;
using System.Collections.Generic;

#endregion

namespace sones.GraphDS.API.CSharp.TraverserExtensions
{

    /// <summary>
    /// Extension classes for the GraphDSSharp API to provide graph traversals
    /// </summary>
    public static class TraverserExtensions
    {


        #region TraversePath(this myAGraphDSSharp, ...)

        /// <summary>
        /// Starts a traversal and returns the found paths or an aggreagted result
        /// </summary>
        /// <typeparam name="T">The resulttype after applying the result transformation</typeparam>
        /// <param name="myStartVertex">The starting vertex</param>
        /// <param name="TraversalOperation">BreathFirst|DepthFirst</param>
        /// <param name="myFollowThisEdge">Follow this edge? Based on its TYPE or any other property/characteristic...</param>
        /// <param name="myFollowThisPath">Follow this path (== actual path + NEW edge + NEW dbobject? Based on edge/object UUID, TYPE or any other property/characteristic...</param>
        /// <param name="myMatchEvaluator">Mhm, this vertex/path looks interesting!</param>
        /// <param name="myMatchAction">Hey! I have found something interesting!</param>
        /// <param name="myStopEvaluator">Will stop the traversal on a condition</param>
        /// <param name="myWhenFinished">Finish this traversal by calling (a result transformation method and) an external method...</param>
        /// <returns></returns>
        public static T TraversePath<T>(this AGraphDSSharp                       myAGraphDSSharp,
                                        Vertex                                 myStartVertex,
                                        TraversalOperation                       TraversalOperation  = TraversalOperation.BreathFirst,
                                        Func<DBPath, Edge, Boolean> myFollowThisEdge = null,
                                        Func<DBPath, Edge, Vertex, Boolean> myFollowThisPath = null,
                                        Func<DBPath, Boolean>                    myMatchEvaluator    = null,
                                        Action<DBPath>                           myMatchAction       = null,
                                        Func<TraversalState, Boolean>            myStopEvaluator     = null,
                                        Func<IEnumerable<DBPath>, T>             myWhenFinished      = null)
        {
            //TODO call method on GraphDBSession

            throw new NotImplementedException();
        }

        #endregion

        #region TraverseVertex(this myAGraphDSSharp, ...)

        /// <summary>
        /// Starts a traversal and returns the found vertices or an aggreagted result
        /// </summary>
        /// <typeparam name="T">The resulttype after applying the result transformation</typeparam>
        /// <param name="myStartVertex">The starting vertex</param>
        /// <param name="TraversalOperation">BreathFirst|DepthFirst</param>
        /// <param name="myFollowThisEdge">Follow this edge? Based on its TYPE or any other property/characteristic...</param>
        /// <param name="myMatchEvaluator">Mhm, this vertex/path looks interesting!</param>
        /// <param name="myMatchAction">Hey! I have found something interesting!</param>
        /// <param name="myStopEvaluator">Will stop the traversal on a condition</param>
        /// <param name="myWhenFinished">Finish this traversal by calling (a result transformation method and) an external method...</param>
        /// <returns></returns>
        public static T TraverseVertex<T>(this AGraphDSSharp              myAGraphDSSharp,
                                          Vertex                        myStartVertex,
                                          TraversalOperation              TraversalOperation  = TraversalOperation.BreathFirst,
                                          Func<Vertex, Edge, Boolean> myFollowThisEdge = null,
                                          Func<Vertex, Boolean>         myMatchEvaluator    = null,
                                          Action<Vertex>                myMatchAction       = null,
                                          Func<TraversalState, Boolean>   myStopEvaluator     = null,
                                          Func<IEnumerable<Vertex>, T>  myWhenFinished      = null)
        {
            //TODO call method on GraphDBSession

            throw new NotImplementedException();
        }

        #endregion


    }

}
