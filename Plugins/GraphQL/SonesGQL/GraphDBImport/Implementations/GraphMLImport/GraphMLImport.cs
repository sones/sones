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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.DataStructures;
using sones.Library.ErrorHandling;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;
using sones.Plugins.SonesGQL.DBImport;
using System.Net;

namespace sones.Plugins.SonesGQL
{
	/// <summary>
    /// Class deserializes a graph stored in GraphML format.
    /// 
    /// GraphML description can be found here: http://graphml.graphdrawing.org/primer/graphml-primer.html
    /// 
    /// implemented:
    /// + attribute definition parsing
    /// + vertex parsing
    /// + edge parsing
    /// + (vertex/edge)attribute parsing
    /// 
    /// not implemented:
    /// - hyperedges
    /// - nested graphs
    /// - graph meta
    /// - xml header stuff
    /// - ports
    /// - extends
    /// 
    /// supported attribute types:
    /// - String
    /// - Int
    /// - Float
    /// - Double
    /// - Boolean
    /// - Long
    /// 
    /// Notes:
    /// 
    /// 1) Defined VertexIDs are stored in a property called "ID" as String and the database creates 
    /// it's own internal IDs (VertexID) of type "Long"
    /// 
    /// 2) Currently the parser only supports one attribute at edges: "weight". If you
    /// want to use weighted edges, the attr.name has to be "weight" or the parser will
    /// ignore it.
    /// 
    /// 3) fell free to add more functionality :)
    /// 
    /// author:         Martin Junghanns (martin@sones.com)
    /// copyright (C):  2007-2011 sones GmbH (www.sones.com)
    /// </summary>
	public class GraphMLImport : IGraphDBImport, IPluginable
	{
		#region private members
		
		/// <summary>
		/// The database instance used for import.
		/// </summary>
		private IGraphDB _GraphDB;
		
		/// <summary>
		/// The security token.
		/// </summary>
		private SecurityToken _SecurityToken;
		
		/// <summary>
		/// The transaction token.
		/// </summary>
		private TransactionToken _TransactionToken;
		
		/// <summary>
		/// The vertexType which shall be used for storing the vertex data.
		/// </summary>
		private string _VertexTypeName;
		
		/// <summary>
		/// The edgeType which shall be used for storing the edge data.
		/// </summary>
		private string _EdgeTypeName;
		
		/// <summary>
        /// Maps attribute keys to attribute (for, name, type, default)
        /// </summary>
		private Dictionary<string, Tuple<string, string, string, string>> _AttributeDefinitions;
		
		/// <summary>
		/// Maps the GraphML VertexIDs to the internal GraphDB VertexIDs
		/// </summary>
		private Dictionary<String, long> _VertexIDMapper;
		
		/// <summary>
		/// The number of edges which were read.
		/// </summary>
		private long _EdgeCount;
		
		/// <summary>
        /// The lock to avoid multiple concurrent calls to this plugin.
        /// </summary>
        private object _Lock = new Object();
		
		#endregion
		
		#region constants
		
		public const string PARAM_VERTEXTYPENAME 	= "VertexTypeName";
		
		public const string PARAM_EDGENAME			= "EdgeName";
		
		#endregion
		
		#region properties
		
		/// <summary>
		/// Number of vertices read from GraphML File.
		/// </summary>
		/// <value>
		/// The vertex count.
		/// </value>
		public Int64 VertexCount
		{
			get 
			{
				return _VertexIDMapper.Count;
			}
		}
		
		/// <summary>
		/// Number of edges read from GraphML File.
		/// </summary>
		/// <value>
		/// The edge count.
		/// </value>
		public Int64 EdgeCount
		{
			get
			{
				return _EdgeCount;	
			}
		}
		
		#endregion
		
		#region constructors
		
		public GraphMLImport ()
		{}
		
		#endregion

		#region IGraphDBImport implementation
		
		private void InitVertexSettings(Dictionary<string, string> myOptions)
		{
			if(!myOptions.ContainsKey(PARAM_VERTEXTYPENAME))
			{
				throw new ArgumentException(String.Format("{0} has not been defined.", PARAM_VERTEXTYPENAME));
			}
			if(!myOptions.ContainsKey(PARAM_EDGENAME))
			{
				throw new ArgumentException(String.Format("{0} has not been defined.", PARAM_EDGENAME));	
			}
			
			var vertexTypeName 	= (string)myOptions[PARAM_VERTEXTYPENAME];
			var edgeTypeName 	= (string)myOptions[PARAM_EDGENAME];
			
			if(vertexTypeName == null || "".Equals(vertexTypeName))
			{
				throw new ArgumentException(String.Format("{0} is invalid (null or empty)", PARAM_VERTEXTYPENAME));	
			}
			
			if(edgeTypeName == null || "".Equals(edgeTypeName))
			{
				throw new ArgumentException(String.Format("{0} is invalid (null or empty)", PARAM_EDGENAME));	
			}
			
			_VertexTypeName 		= vertexTypeName;
			_EdgeTypeName 			= edgeTypeName;
			_AttributeDefinitions 	= new Dictionary<string, Tuple<string, string, string, string>>();
			
			_VertexIDMapper 		= new Dictionary<String, long>();
			_EdgeCount 		= 0;
		}
		
		public QueryResult Import (string myLocation, 
			IGraphDB myGraphDB,
			IGraphQL myGraphQL,
			SecurityToken mySecurityToken,
			TransactionToken myTransactionToken,
			UInt32 myParallelTasks = 1U,
			IEnumerable<string> myComments = null,
			UInt64? myOffset = null,
			UInt64? myLimit = null,
			VerbosityTypes myVerbosityTypes = VerbosityTypes.Silent,
			Dictionary<string, string> myOptions = null)
		{
			#region data
			
			if (myGraphDB == null)
			{
				return new QueryResult("", 
						ImportFormat, 
						0, 
						ResultType.Failed, 
						null, 
						new UnknownException(new ArgumentNullException("Missing GraphDB object")));
			}
			
			if(myLocation == null)
			{
				return new QueryResult("", 
						ImportFormat, 
						0, 
						ResultType.Failed, 
						null, 
						new UnknownException(new ArgumentNullException("Missing Location object")));	
			}
			
//			if(mySecurityToken == null)
//			{
//				return new QueryResult("", 
//						ImportFormat, 
//						0, 
//						ResultType.Failed, 
//						null, 
//						new UnknownException(new ArgumentNullException("mySecurityToken")));
//			}
//			if(myTransactionToken == null)
//			{
//				return new QueryResult("", 
//						ImportFormat, 
//						0, 
//						ResultType.Failed, 
//						null, 
//						new UnknownException(new ArgumentNullException("myTransactionToken")));	
//			}
			
			if(myOptions == null)
			{
				return new QueryResult("", 
						ImportFormat, 
						0, 
						ResultType.Failed, 
						null, 
						new UnknownException(
					new ArgumentNullException(
						String.Format("Missing Options {0}, {1}",
							PARAM_VERTEXTYPENAME,
							PARAM_EDGENAME
						))));
			}
			
			_GraphDB 			= myGraphDB;
			_SecurityToken 		= mySecurityToken;
			_TransactionToken 	= myTransactionToken;
			InitVertexSettings(myOptions);
			
			var sw = new Stopwatch();
			Stream stream = null;
			
			#endregion
			
			#region create vertex type
			
			CreateVertexType();
			
			#endregion
			
			#region import
			
            lock (_Lock)
            {
				#region read elements

	            try
	            {					
					if(myLocation.ToLower().StartsWith("file://") || myLocation.ToLower().StartsWith("file:\\"))
					{
						stream = GetStreamFromFile(myLocation.Substring("file://".Length));
					} 
					else if(myLocation.StartsWith("http://"))
					{
						stream = GetStreamFromHttp(myLocation);	
					} 
					else
					{
						throw new FormatException("Given file location is invalid.");
					}
					
					var reader = XmlReader.Create(stream);
					
					sw.Start();
					
	                while (reader.Read())
	                {
	                    if (reader.NodeType == XmlNodeType.Element)
	                    {
	                        switch (reader.Name)
	                        {
	                            case GraphMLTokens.GRAPHML:
	                                break;
	                            case GraphMLTokens.GRAPH:
	                                break;
	                            case GraphMLTokens.KEY:
	                                ReadAttributeDefinition(reader);
	                                break;
	                            case GraphMLTokens.VERTEX:
	                                ReadVertex(reader);
	                                break;
	                            case GraphMLTokens.EDGE:
	                                ReadEdge(reader);
	                                break;
	                            default:
	                                throw new XmlException(String.Format("Unsupported Node Type in GraphML File: {0}",
									reader.Name));
	                        }
	                    }
	                }
	            }
	            catch (Exception ex)
	            {
					if(stream != null)
					{
						stream.Close();
					}
					// drop vertex type in case of exception
					DropVertexType();
					
	                return new QueryResult("VertexType has been removed", 
						ImportFormat, 
						(ulong)sw.ElapsedMilliseconds, 
						ResultType.Failed, 
						null, 
						new UnknownException(ex));
	            }
				finally
				{
					sw.Stop();
				}

            	#endregion
			}
			
			#endregion
			
			return new QueryResult("", 
				ImportFormat, 
				(ulong)sw.ElapsedMilliseconds, 
				ResultType.Successful,
				null, 
				null);
		}
		
		/// <summary>
		/// Returns the import format.
		/// </summary>
		/// <value>
		/// The import format.
		/// </value>
		public string ImportFormat 
		{
			get { return "GRAPHML"; }
		}
		
		#endregion

		#region IPluginable implementation
		
		/// <summary>
		/// Initializes the plugin.
		/// 
		/// GraphMLImport needs two parameters:
		/// 
		/// VertexTypeName 	- The VertexType to use for import (will be created)
		/// EdgeTypeName 	- The EdgeType to use for import (will be created)
		/// 
		/// </summary>
		/// <returns>
		/// The plugin.
		/// </returns>
		/// <param name='UniqueString'>
		/// Unique string.
		/// </param>
		/// <param name='myParameters'>
		/// My parameters.
		/// </param>
		/// <exception cref='ArgumentException'>
		/// Is thrown when an argument passed to a method is invalid.
		/// </exception>
		public IPluginable InitializePlugin (string UniqueString, Dictionary<string, object> myParameters)
		{			
			return new GraphMLImport();
		}

		public string PluginName 
		{
			get { return "sones.graphmlimport"; }
		}
		
		/// <summary>
		/// Returns the setable parameters for this plugin.
		/// </summary>
		/// <value>
		/// The setable parameters.
		/// </value>
		public PluginParameters<Type> SetableParameters 
		{
			get 
			{ 
				return new PluginParameters<Type>()
				{
					{PARAM_VERTEXTYPENAME, 	typeof(string)},
					{PARAM_EDGENAME, 	typeof(string)}
				}; 
			}
		}
		
		#endregion

		#region IDisposable implementation
		
		public void Dispose ()
		{ }
		
		#endregion
		
		#region private methods
		
		/// <summary>
		/// Creates the vertexType based on the VertexTypeName.
		/// 
		/// The vertexType contains one Outgoing Edge Defintion, the edge
		/// is weighted and can't contain any other attributes.
		/// </summary>
		private void CreateVertexType()
		{
			#region create vertex type
			
			var vertexTypePreDef 	= new VertexTypePredefinition(_VertexTypeName);
			var outEdgePreDef 		= new OutgoingEdgePredefinition(_EdgeTypeName, vertexTypePreDef);
			
			#region create edge definition
			
			// weighted multi-edge
			outEdgePreDef.SetEdgeTypeAsWeighted();
			// set inner edge type to weighted
			outEdgePreDef.SetMultiplicityAsMultiEdge("Weighted");
			
			vertexTypePreDef.AddOutgoingEdge(outEdgePreDef);			
			
			#endregion
			
			#region create id definition
			
			var idPreDefinition = new PropertyPredefinition(GraphMLTokens.VERTEX_ID_NAME , GraphMLTokens.VERTEX_ID_TYPE);

			idPreDefinition.SetDefaultValue(GraphMLTokens.VERTEX_ID_DEF_VAL);
			
			vertexTypePreDef.AddProperty(idPreDefinition);
			
			#endregion
			
			#region create vertex type
			
			var requestCreateVertexType = new RequestCreateVertexType(vertexTypePreDef);
			
			_GraphDB.CreateVertexType(_SecurityToken,
			                         _TransactionToken,
			                         requestCreateVertexType,
			                         (stats, vType) => vType);
			
			#endregion
			
			#endregion
		}
		
		private void DropVertexType()
		{
			_GraphDB.DropType(_SecurityToken,
				_TransactionToken,
				new RequestDropVertexType(_VertexTypeName),
				(stats, removedIDs) => removedIDs);
		}
		
		#region vertex stuff
		
		/// <summary>
		/// Reads the vertex and all attributes from the GraphML File
		/// and inserts the data into the GraphDB instance.
		/// </summary>
		/// <param name='myReader'>
		/// XMLReader
		/// </param>
		private void ReadVertex(XmlReader myReader)
		{
			#region VertexID
			
            var vertexID = ReadVertexID(myReader, GraphMLTokens.ID);
			
            #endregion

            #region Read Attributes and insert vertex into GraphDB
			
			InsertVertex(myReader, vertexID);

            #endregion
		}
		
		/// <summary>
		/// Reads the vertexID from the GraphML File.
		/// 
		/// </summary>
		/// <returns>
		/// The vertex ID.
		/// </returns>
		/// <param name='myReader'>
		/// XMLReader
		/// </param>
		/// <param name='myToken'>
		/// My token.
		/// </param>
		/// <exception cref='ArgumentException'>
		/// Is thrown when an argument passed to a method is invalid.
		/// </exception>
		private String ReadVertexID(XmlReader myReader, String myToken)
		{
			var vertexIDString = myReader.GetAttribute(myToken);
			
			if(vertexIDString == null != "".Equals(vertexIDString))
			{
				throw new ArgumentException(String.Format(
					"Vertex ID was invalid: {0}", 
					vertexIDString));
			} else
			{
				return vertexIDString;
			}
		}
		
		/// <summary>
		/// Inserts a vertex and its attributes into the GraphDB instance.
		/// </summary>
		/// <param name='myExternalVertexID'>
		/// the vertex id
		/// </param>
		private void InsertVertex(XmlReader myReader, String myExternalVertexID)
		{
			IVertex addedVertex = null;
			
			if(!_VertexIDMapper.ContainsKey(myExternalVertexID))
			{
				var insertRequest = new RequestInsertVertex(_VertexTypeName);
//				insertRequest.SetUUID(myVertexID);
				
				#region store Graphml VertexID
				
				insertRequest.AddStructuredProperty(GraphMLTokens.VERTEX_ID_NAME, myExternalVertexID);
				
				#endregion
				
				#region read vertex attributes
				
				ReadVertexAttributes(myReader, insertRequest);
				
				#endregion
				
				// insert vertex
				addedVertex = _GraphDB.Insert<IVertex>(
					_SecurityToken,
					_TransactionToken,
					insertRequest,
					(stats, v) => v
					);
				
				if(addedVertex != null)
				{
					// create mapping between external and internal VertexID
					_VertexIDMapper.Add(myExternalVertexID, addedVertex.VertexID);
				}
			}
		}
		
		/// <summary>
		/// Reads the attributes of a vertex based on the attribute definitions.
		/// 
		/// Updates the RequestInsertVertex with the read attributes.
		/// </summary>
		/// <param name='myReader'>
		/// XmlReader
		/// </param>
		/// <param name='myRequestInsertVertex'>
		/// RequestInsertVertex where the attributes will be added.
		/// </param>
		private void ReadVertexAttributes(XmlReader myReader, RequestInsertVertex myRequestInsertVertex)
		{
			using (var vertexDataReader = myReader.ReadSubtree())
            {
                //read attributes
                while (vertexDataReader.Read())
                {
                    if (vertexDataReader.Name == GraphMLTokens.DATA)
                    {
						var key = myReader.GetAttribute(GraphMLTokens.KEY);
			            var value = myReader.ReadElementContentAsString();
			
			            if (key != null)
			            {
			                var tupel = _AttributeDefinitions[key];
			
			                if (tupel != null)
			                {
			                    var attrName = tupel.Item2;
			                    var attrType = tupel.Item3;
			
			                    if (value == null) //use default value
			                    {
			                        value = tupel.Item4;
			                    }
			
			                    if (attrType != null && value != null)
			                    {
//									myRequestInsertVertex.AddStructuredProperty(attrName, 
//										(IComparable)ParseValue(attrType, value));
									myRequestInsertVertex.AddUnknownProperty(attrName,
										Convert.ChangeType(ParseValue(attrType, value), 
										typeof(String), 
										CultureInfo.GetCultureInfo("en-us")));
			                    }
			                }
			            }
                    }
                }
            }
		}
		
		#endregion
		
		#region edge stuff
		
		/// <summary>
		/// Reads an edge information from the GraphML File and inserts
		/// the edge in the GraphDB by altering the existing source vertex.
		/// 
		/// Currently only the "weight" attribute will be considered and
		/// has to be annotated correctly (see class documentation).
		/// </summary>
		/// <param name='myReader'>
		/// XmlReader
		/// </param>
		private void ReadEdge(XmlReader myReader)
		{
			#region read edge data
			
            var sourceID = ReadVertexID(myReader, GraphMLTokens.SOURCE);
            var targetID = ReadVertexID(myReader, GraphMLTokens.TARGET);
			
			if(!_VertexIDMapper.ContainsKey(sourceID) || !_VertexIDMapper.ContainsKey(targetID))
			{
				throw new InvalidDataException(String.Format(
					"Source or Target vertexID for edge ({0},{1}) doesn't exist",
					sourceID,
					targetID));
			}
			
			// get the weight
			var edgeWeight = ReadEdgeWeight(myReader);
			
			#endregion
			
			#region create edge (update vertex)
			
			var hyperEdge = new EdgePredefinition(_EdgeTypeName);
			hyperEdge.AddEdge(new EdgePredefinition()
				.AddVertexID(_VertexTypeName, _VertexIDMapper[targetID])
				.AddUnknownProperty(
					GraphMLTokens.EDGE_WEIGHT, 
					Convert.ChangeType(edgeWeight, typeof(String), CultureInfo.GetCultureInfo("en-us"))
				));
			
			
			var requestUpdate = new RequestUpdate(new RequestGetVertices(_VertexTypeName, new List<long>() { _VertexIDMapper[sourceID] }));
			requestUpdate.AddElementsToCollection(_EdgeTypeName, hyperEdge);
			
			// process the update
			_GraphDB.Update<IEnumerable<IVertex>>(
				_SecurityToken,
				_TransactionToken,
				requestUpdate,
				(stats, v) => v
				);
			
			_EdgeCount++;
			
			#endregion
		}
		
		/// <summary>
		/// Reads the edge weight from the data field of an edge definition.
		/// 
		/// The attribute name must be equal to the value in 
		/// GraphMLTokens.EDGE_WEIGHT or it won't be considered by the parsing.
		/// </summary>
		/// <returns>
		/// The edge weight or the default edge weight if no weight is defined.
		/// </returns>
		/// <param name='myReader'>
		/// XmlReader
		/// </param>
		private IComparable ReadEdgeWeight(XmlReader myReader)
		{
			using (var edgeDataReader = myReader.ReadSubtree())
            {
                while (edgeDataReader.Read())
                {
                    if (edgeDataReader.Name == GraphMLTokens.DATA)
                    {
                        var key = myReader.GetAttribute(GraphMLTokens.KEY);
			            var value = myReader.ReadElementContentAsString();
			
			            if (key != null)
			            {
			                var tupel = _AttributeDefinitions[key];
			
			                if (tupel != null)
			                {
			                    var attrName = tupel.Item2;
			                    var attrType = tupel.Item3;
			
			                    if (value == null) //use default value
			                    {
			                        value = tupel.Item4;
			                    }
			
			                    if (attrType != null && value != null)
			                    {
									if(attrName.Equals(GraphMLTokens.EDGE_WEIGHT.ToLower()))
									{
										return ParseValue(attrType, value);	
									}
			                    }
			                }
			            }
                    }
                }
            }
			// no weight defined, use default value
			return GraphMLTokens.DEFAULT_EDGE_WEIGHT;
		}
		
		#endregion
		
		#region attribute [definitions]
		
		/// <summary>
		/// Reads an attribute definition from the GraphML File and stores
		/// it internal for later usage on vertex / edge reading.
		/// </summary>
		/// <param name='myReader'>
		/// XmlReader
		/// </param>
		private void ReadAttributeDefinition(XmlReader myReader)
        {
			#region data
			
            var attrId 		= myReader.GetAttribute(GraphMLTokens.ID);
            var attrFor 	= myReader.GetAttribute(GraphMLTokens.FOR);
			var attrName 	= myReader.GetAttribute(GraphMLTokens.ATTRIBUTE_NAME);
            var attrType 	= myReader.GetAttribute(GraphMLTokens.ATTRIBUTE_TYPE).ToLower();

            string attrDefault = null;

            using (var readerAttribute = myReader.ReadSubtree())
            {
                while (readerAttribute.Read())
                {
                    if (readerAttribute.Name == GraphMLTokens.DEFAULT)
                    {
                        attrDefault = readerAttribute.ReadElementContentAsString();
                    }
                }
            }
			
			// make attribute type DB conform (capitalize first letter)
			attrType = char.ToUpper(attrType[0]) + attrType.Substring(1).ToLower();
			// and store the whole definition
            _AttributeDefinitions.Add(attrId, new Tuple<string, string, string, string>(attrFor, attrName, attrType, attrDefault));
			// get GraphDB internal type
			attrType = GetInternalTypeName(attrType);
			
			#endregion
			
			#region alter vertex type with new attribute
			
			if(attrFor.Equals(GraphMLTokens.VERTEX))
			{
				var requestAlterVertexType = new RequestAlterVertexType(_VertexTypeName);
				
				var propertyPreDefinition = new PropertyPredefinition(attrName, attrType);
				
				propertyPreDefinition.SetDefaultValue(attrDefault);
				
				requestAlterVertexType.AddProperty(propertyPreDefinition);
				
				_GraphDB.AlterVertexType(_SecurityToken,
					_TransactionToken,
					requestAlterVertexType,
					(stats, vType) => vType);
			}
			
			#endregion
        }		
		
		#endregion
		
		#region helper
		
		/// <summary>
		/// Parses the given string value to the given type.
		/// 
		/// The type must implement IComparable!
		/// </summary>
		/// <returns>
		/// The parsed value.
		/// </returns>
		/// <param name='myType'>
		/// The type whose Parse() method will be used.
		/// </param>
		/// <param name='myValue'>
		/// The value as string.
		/// </param>
		private IComparable ParseValue(String myType, String myValue)
        {
            if (myType.Equals(GraphMLTokens.INT))
            {
                return Int32.Parse(myValue);
            }
            else if (myType.Equals(GraphMLTokens.FLOAT))
            {
               	return float.Parse(myValue, CultureInfo.GetCultureInfo("en-us"));
            }
            else if (myType.Equals(GraphMLTokens.DOUBLE))
            {
                return Double.Parse(myValue, CultureInfo.GetCultureInfo("en-us"));
            }
            else if (myType.Equals(GraphMLTokens.LONG))
            {
                return Int64.Parse(myValue);
            }
			else if (myType.Equals(GraphMLTokens.BOOLEAN))
            {
                return Boolean.Parse(myValue);
            }
			else if(myType.Equals(GraphMLTokens.STRING))
			{
				return myValue;	
			}
            else
            {
                throw new ArgumentException(String.Format("Attribute Type {0} not supported by GraphML Parser",
					myType));
            }
        }
		
		private String GetInternalTypeName(String myExternalTypeName)
		{
			if(myExternalTypeName.Equals(GraphMLTokens.STRING))
			{
				return GraphMLTokens.STRING_INTERNAL;	
			} 
			else if(myExternalTypeName.Equals(GraphMLTokens.INT))
			{
				return GraphMLTokens.INT_INTERNAL;	
			}
			else if(myExternalTypeName.Equals(GraphMLTokens.LONG))
			{
				return GraphMLTokens.LONG_INTERNAL;	
			}
			else if(myExternalTypeName.Equals(GraphMLTokens.FLOAT))
			{
				return GraphMLTokens.FLOAT_INTERNAL;
			}
			else if(myExternalTypeName.Equals(GraphMLTokens.DOUBLE))
			{
				return GraphMLTokens.DOUBLE_INTERNAL;
			}
			else if(myExternalTypeName.Equals(GraphMLTokens.BOOLEAN))
			{
				return GraphMLTokens.BOOLEAN_INTERNAL;
			}
			else
            {
                throw new ArgumentException(String.Format("Attribute Type {0} not supported by GraphML Parser",
					myExternalTypeName));
            }
				
		}
		
		#region Get streams

        /// <summary>
        /// Reads from file
        /// </summary>
        /// <param name="myLocation"></param>
        /// <returns></returns>
        private Stream GetStreamFromFile(String myLocation)
        {
            return File.OpenRead(myLocation);
        }

        /// <summary>
        /// Reads from web resource
        /// </summary>
        /// <param name="myLocation"></param>
        /// <returns></returns>
        private Stream GetStreamFromHttp(String myLocation)
        {
            var request = (HttpWebRequest)WebRequest.Create(myLocation);
            var response = request.GetResponse();
            return response.GetResponseStream();
        }

        #endregion
		
		#endregion
			
		#endregion
	}
}

