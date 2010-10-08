
using System;
using System.Collections.Generic;
using sones.GraphDB.TypeManagement.BasicTypes;


namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// Interface for List and Set of base object
    /// </summary>
    public interface IBaseEdge : IEdgeType, IListOrSetEdgeType, IEnumerable<ADBBaseObject>
    {

        /// <summary>
        /// Adds a new value with some optional parameters
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myParameters"></param>
        void Add(ADBBaseObject myValue, params ADBBaseObject[] myParameters);

        /// <summary>
        /// Adds a new value with some optional parameters
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myParameters"></param>
        void AddRange(IEnumerable<ADBBaseObject> myValue, params ADBBaseObject[] myParameters);

        /// <summary>
        /// Remove a value
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        Boolean Remove(ADBBaseObject myValue);


        /// <summary>
        /// Check for a containing element
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        Boolean Contains(ADBBaseObject myValue);

        /// <summary>
        /// Returns all values. Use this for all not reference ListEdgeType.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Object> GetReadoutValues();

        /// <summary>
        /// Get all data and their edge infos
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tuple<ADBBaseObject, ADBBaseObject>> GetEdges();

        /// <summary>
        /// Get all data and their edge infos
        /// </summary>
        /// <returns></returns>
        IEnumerable<ADBBaseObject> GetBaseObjects();

    }
}
