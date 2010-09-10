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
                                        DBVertex                                 myStartVertex,
                                        TraversalOperation                       TraversalOperation  = TraversalOperation.BreathFirst,
                                        Func<DBPath, DBEdge, Boolean>            myFollowThisEdge    = null,
                                        Func<DBPath, DBEdge, DBVertex, Boolean>  myFollowThisPath    = null,
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
                                          DBVertex                        myStartVertex,
                                          TraversalOperation              TraversalOperation  = TraversalOperation.BreathFirst,
                                          Func<DBVertex, DBEdge, Boolean> myFollowThisEdge    = null,
                                          Func<DBVertex, Boolean>         myMatchEvaluator    = null,
                                          Action<DBVertex>                myMatchAction       = null,
                                          Func<TraversalState, Boolean>   myStopEvaluator     = null,
                                          Func<IEnumerable<DBVertex>, T>  myWhenFinished      = null)
        {
            //TODO call method on GraphDBSession

            throw new NotImplementedException();
        }

        #endregion


    }

}
