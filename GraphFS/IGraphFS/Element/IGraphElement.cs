using System;

namespace GraphFS.Element
{
    /// <summary>
    /// The interface for graph elements like vertices or edges
    /// </summary>
    public interface IGraphElement
    {
        /// <summary>
        /// Returns the property of a vertex
        /// </summary>
        /// <typeparam name="T">The type of the interesting property</typeparam>
        /// <param name="myPropertyName">The name of the interesing property</param>
        /// <returns>A Property</returns>
        T GetProperty<T>(string myPropertyName);
    }
}
