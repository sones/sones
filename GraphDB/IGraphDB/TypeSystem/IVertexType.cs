/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;

namespace sones.GraphDB.TypeSystem
{

    /// <summary>
    /// The interface for all vertex types
    /// </summary>
    public interface IVertexType: IBaseType
    {

        #region Inheritance

        /// <summary>
        /// Returns the descendant of this IVertexType.
        /// </summary>
        /// <returns>An enumeration of IVertexType that are descendant of this IVertexType.</returns>
        /// <seealso cref="IBaseType.GetDescendantTypes"/>
        IEnumerable<IVertexType> GetDescendantVertexTypes();

        /// <summary>
        /// Returns the descendant of this IVertexType and this IVertexType in one enumeration.
        /// </summary>
        /// <returns>An enumeration of IVertexType that are descendant of this IVertexType and this IVertexType itself.</returns>
        /// <seealso cref="IBaseType.GetDescendantTypesAndSelf"/>
        IEnumerable<IVertexType> GetDescendantVertexTypesAndSelf();

        /// <summary>
        /// Returns the ancestor of this IVertexType.
        /// </summary>
        /// <returns>An enumeration of IVertexType that are ancestors of this IVertexType.</returns>
        /// <seealso cref="IBaseType.GetAncestorTypes"/>
        IEnumerable<IVertexType> GetAncestorVertexTypes();

        /// <summary>
        /// Returns the ancestor of this IVertexType and this IVertexType in one enumeration.
        /// </summary>
        /// <returns>An enumeration of IVertexType that are ancestors of this IVertexType and this IVertexType itself.</returns>
        /// <seealso cref="IBaseType.GetAncestorTypesAndSelf"/>
        IEnumerable<IVertexType> GetAncestorVertexTypesAndSelf();

        /// <summary>
        /// Returns all descendant and ancestors of this IVertexType.
        /// </summary>
        /// <returns>An enumeration of all IVertexType that are ancestors or descendant of this IVertexType.</returns>
        /// <seealso cref="IBaseType.GetKinsmenTypes"/>
        IEnumerable<IVertexType> GetKinsmenVertexTypes();

        /// <summary>
        /// Returns all descendant and ancestors of this IVertexType and this IVertexType in one enumeration. 
        /// </summary>
        /// <returns>An enumeration of all IVertexType that are ancestors or descendant of this IVertexType and this IVertexType itself.</returns>
        /// <seealso cref="IBaseType.GetKinsmenTypesAndSelf"/>
        IEnumerable<IVertexType> GetKinsmenVertexTypesAndSelf();

        /// <summary>
        /// Returns the direct children of this IVertexType.
        /// </summary>
        /// <seealso cref="IBaseType.ChildrenTypes"/>
        IEnumerable<IVertexType> ChildrenVertexTypes { get; }

        /// <summary>
        /// Gets the parent of this IVertexType.
        /// </summary>
        /// <seealso cref="IBaseType.ParentType"/>
        IVertexType ParentVertexType { get; }

        #endregion

        #region Incoming

        /// <summary>
        /// Has this vertex type a certain binary property?
        /// </summary>
        /// <param name="myEdgeName">The name of the binary property.</param>
        /// <returns>True, if a binary property with the given name exists, otherwise false.</returns>
        bool HasBinaryProperty(String myEdgeName);

        /// <summary>
        /// Gets a certain binary property definition.
        /// </summary>
        /// <param name="myAttributeName">The name of the interesting binary property.</param>
        /// <returns>A binary property definition, if existing otherwise <c>NULL</c>.</returns>
        IBinaryPropertyDefinition GetBinaryPropertyDefinition(String myEdgeName);

        /// <summary>
        /// Has this vertex type any binary property.
        /// </summary>
        /// <param name="myIncludeAncestorDefinitions">If true, the ancestor vertex types are included, otherwise false.</param>
        /// <returns>True if a binary property exists, otherwise false.</returns>
        bool HasBinaryProperties(bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Get all binary properties.
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent vertex type(s).</param>
        /// <returns>An enumerable of binary property definitions.</returns>
        IEnumerable<IBinaryPropertyDefinition> GetBinaryProperties(bool myIncludeAncestorDefinitions);

        #endregion


        #region Edges

        #region Incoming

        /// <summary>
        /// Has this vertex type a certain incoming IncomingEdge?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasIncomingEdge(String myEdgeName);

        /// <summary>
        /// Gets a certain incoming IncomingEdge definition
        /// </summary>
        /// <param name="myAttributeName">The name of the interesting incoming IncomingEdge</param>
        /// <returns>An incoming IncomingEdge definition</returns>
        IIncomingEdgeDefinition GetIncomingEdgeDefinition(String myEdgeName);

        /// <summary>
        /// Has this vertex type any visible incoming edges?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasIncomingEdges(bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Get all incoming edges
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent vertex type(s)</param>
        /// <returns>An enumerable of incoming IncomingEdge attributes</returns>
        IEnumerable<IIncomingEdgeDefinition> GetIncomingEdgeDefinitions(bool myIncludeAncestorDefinitions);


        #endregion

        #region Outgoing

        /// <summary>
        /// Has this vertex type a certain outgoing IncomingEdge?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasOutgoingEdge(String myEdgeName);

        /// <summary>
        /// Gets a certain outgoing IncomingEdge definition
        /// </summary>
        /// <param name="myAttributeName">The name of the interesting outgoing IncomingEdge</param>
        /// <returns>An outgoing IncomingEdge definition</returns>
        IOutgoingEdgeDefinition GetOutgoingEdgeDefinition(String myEdgeName);

        /// <summary>
        /// Has this vertex type any outgoing edges?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasOutgoingEdges(bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Get all outgoing edges
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent vertex type(s)</param>
        /// <returns>An enumerable of outgoing IncomingEdge attributes</returns>
        IEnumerable<IOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(bool myIncludeAncestorDefinitions);

        #endregion

        #endregion

        #region Uniques

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myIncludeAncestorDefinitions"></param>
        /// <returns></returns>
        bool HasUniqueDefinitions(bool myIncludeAncestorDefinitions);

        /// <summary>
        /// A set of uniqueness definitions.
        /// </summary>
        /// <returns>An enumerable of uniqueness definitions. Never <c>NULL</c>.</returns>
        IEnumerable<IUniqueDefinition> GetUniqueDefinitions(bool myIncludeAncestorDefinitions);

        #endregion

        #region Indices

        bool HasIndexDefinitions(bool myIncludeAncestorDefinitions);

        /// <summary>
        /// A set of index definitions.
        /// </summary>
        /// <returns>An enumerable of index definitions. Never <c>NULL</c>.</returns>
        IEnumerable<IIndexDefinition> GetIndexDefinitions(bool myIncludeAncestorDefinitions);

        #endregion


    }
}