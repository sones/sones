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

/* <id name="PandoraDB – the base abstract class for all edges" />
 * <copyright file="AEdgeType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This abstract class defines all methods which should be at least implemented of an edge - either a list or single or whatever.</summary>
 */

using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.Structures.EdgeTypes
{
    /// <summary>
    /// This abstract class defines all methods which should be at least implemented of an edge - either a list or single or whatever.
    /// </summary>    
    public abstract class AEdgeType : AObject, IFastSerialize, IFastSerializationTypeSurrogate 
    {

        /// <summary>
        /// The name of the edgeType. Identified by the reflector and QL
        /// </summary>
        public abstract String EdgeTypeName { get; }

        /// <summary>
        /// A unique ID. Currently we can't use this ID for recognition of the edge type during reload because there is no typemanager available.
        /// </summary>
        public abstract EdgeTypeUUID EdgeTypeUUID { get; }

        /// <summary>
        /// Apply the given <paramref name="myParams"/> to the EdgeType
        /// </summary>
        /// <param name="myParams">A array ob parameters</param>
        public abstract void ApplyParams(params EdgeTypeParamNode[] myParams);

        /// <summary>
        /// Returns a new instance of this EdgeType including all settings but without values
        /// </summary>
        /// <returns>A new instance with all settings</returns>
        public abstract AEdgeType GetNewInstance();

        public new abstract String ToString();

        /// <summary>
        /// Returns information for the describe command
        /// </summary>
        /// <returns></returns>
        public abstract String GetDescribeOutput(GraphDBType myGraphDBType);

        /// <summary>
        /// Return the Graph Data Definition Lanuage statement
        /// </summary>
        /// <param name="myGraphDBType"></param>
        /// <returns></returns>
        public abstract String GetGDDL(GraphDBType myGraphDBType);

        /// <summary>
        /// Creates a new instance of this edge from the passed data.
        /// </summary>
        /// <param name="iEnumerable"></param>
        /// <returns></returns>
        public abstract AEdgeType GetNewInstance(IEnumerable<Exceptional<DBObjectStream>> iEnumerable);

        /// <summary>
        /// Creates a new instance of this edge from the passed data.
        /// </summary>
        /// <param name="iEnumerable"></param>
        /// <returns></returns>
        public abstract AEdgeType GetNewInstance(IEnumerable<ObjectUUID> iEnumerable);


    }  
}
