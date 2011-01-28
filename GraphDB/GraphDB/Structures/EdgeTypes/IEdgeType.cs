/* <id name="GraphDB – the base abstract class for all edges" />
 * <copyright file="AEdgeType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This abstract class defines all methods which should be at least implemented of an edge - either a list or single or whatever.</summary>
 */

using System;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement;
using System.Collections.Generic;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// This abstract class defines all methods which should be at least implemented of an edge - either a list or single or whatever.
    /// </summary>    
    public interface IEdgeType : IObject 
    {

        /// <summary>
        /// The name of the edgeType. Identified by the reflector and QL
        /// </summary>
        String EdgeTypeName { get; }

        /// <summary>
        /// A unique ID. Currently we can't use this ID for recognition of the edge type during reload because there is no typemanager available.
        /// </summary>
        EdgeTypeUUID EdgeTypeUUID { get; }

        /// <summary>
        /// Apply the given <paramref name="myParams"/> to the EdgeType
        /// </summary>
        /// <param name="myParams">A array ob parameters</param>
        void ApplyParams(params EdgeTypeParamDefinition[] myParams);

        /// <summary>
        /// Apply the given <paramref name="myParams"/> to the EdgeType
        /// </summary>
        /// <param name="myParams">A array ob parameters</param>
        IEnumerable<EdgeTypeParamDefinition> GetParams();

        String ToString();

        /// <summary>
        /// Returns information for the describe command
        /// </summary>
        /// <returns></returns>
        String GetDescribeOutput(GraphDBType myGraphDBType);

        /// <summary>
        /// Return the Graph Data Definition Lanuage statement
        /// </summary>
        /// <param name="myGraphDBType"></param>
        /// <returns></returns>
        String GetGDDL(GraphDBType myGraphDBType);

        /// <summary>
        /// Returns a new instance of this EdgeType including all settings but without values
        /// </summary>
        /// <returns>A new instance with all settings</returns>
        IEdgeType GetNewInstance();

    }  
}
