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

namespace sones.Plugins.SonesGQL.GraphMLImport
{
	/// <summary>
    /// Class defines the XML tokens which are used for parsing.
    /// 
    /// author:         Martin Junghanns (martin@sones.com)
    /// copyright (C):  2007-2010 sones GmbH (www.sones.com)
    /// </summary>
    public class GraphMLTokens
    {
        #region graph data

        public const String GRAPHML = "graphml";
        public const String GRAPH = "graph";
        public const String VERTEX = "node";
        public const String EDGE = "edge";
        public const String EDGEDEFAULT = "edgedefault";
        public const String DIRECTED = "directed"; 
        public const String UNDIRECTED = "undirected";
        public const String SOURCE = "source";
        public const String TARGET = "target";

        #region attributes

        public const String DATA = "data";
        public const String ID = "id";
        public const String KEY = "key";
        public const String FOR = "for";
        public const String ATTRIBUTE_NAME = "attr.name";
        public const String ATTRIBUTE_TYPE = "attr.type";
        public const String DEFAULT = "default";

        #endregion

        #endregion

        #region base types

        public const String STRING 				= "String";
        public const String INT 				= "Int";
		public const String LONG 				= "Long";
        public const String BOOLEAN 			= "Boolean";
        public const String FLOAT 				= "Float";
        public const String DOUBLE 				= "Double";
        
		public const String STRING_INTERNAL 	= "String";
		public const String INT_INTERNAL 		= "Int32";
		public const String LONG_INTERNAL 		= "Int64";
		public const String BOOLEAN_INTERNAL 	= "Boolean";
		public const String FLOAT_INTERNAL		= "Double";
        public const String DOUBLE_INTERNAL		= "Double";
		
        #endregion
		
		#region GraphDB specific
		
		public const String EDGE_WEIGHT = "Weight";
		public const float DEFAULT_EDGE_WEIGHT = 1.0f;
		
		#endregion
    }
}

