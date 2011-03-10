using System;
using System.Collections.Generic;

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// The interface for all hyper edges
    /// </summary>
    public interface IHyperEdge : IEdge
    {
        /// <summary>
        /// Gets all contained edges
        /// </summary>
        /// <param name="myFilterFunction">A function to filter those edges</param>
        /// <returns>An IEnumerable of edges</returns>
        IEnumerable<ISingleEdge> GetEdges(Func<ISingleEdge, bool> myFilterFunction = null);

        /// <summary>
        /// Invokes a function on a hyper edge
        /// </summary>
        /// <typeparam name="TResult">The type of the result aka the function output</typeparam>
        /// <param name="myHyperEdgeFunction">A function that is executed on a hyper edge</param>
        /// <returns>A TResult</returns>
        TResult InvokeHyperEdgeFunc<TResult>(Func<IEnumerable<ISingleEdge>, TResult> myHyperEdgeFunction);
    }
}