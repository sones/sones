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
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.DataStructures;
using sones.Library.ErrorHandling;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;
using sones.Plugins.SonesGQL.DBImport;

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
    /// 1) The GraphDB only accepts ID values in long format. If the given IDs contain any
    /// non-numerical characters they will be removed and only the contained numbers
    /// will be used as an ID. If there are no numerical chars included in the id string 
    /// an exception will be thrown.
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
		/// The vertexIDs which were read.
		/// </summary>
		private HashSet<long> _Vertices;
		
		/// <summary>
		/// Maps the vertex ID in the document to the vertex ID in the GraphDB.
		/// </summary>
		private Dictionary<string, long> _VertexMapping;
		
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
				return _Vertices.Count;
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
			_VertexMapping 			= new Dictionary<string, long>();
			_AttributeDefinitions 	= new Dictionary<string, Tuple<string, string, string, string>>();
			
			_Vertices 		= new HashSet<long>();
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
			FileStream stream;
			
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
					stream = new FileStream(new Uri(myLocation).LocalPath, FileMode.Open);
					
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
					stream.Close();
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
			var outEdgePreDef 		= new OutgoingEdgePredefinition(_EdgeTypeName);
			
			// weighted multi-edge
			outEdgePreDef.SetEdgeTypeAsWeighted();
			// set inner edge type to weighted
			outEdgePreDef.SetMultiplicityAsMultiEdge("Weighted");
			// set type of vertices at edges
			outEdgePreDef.SetAttributeType(vertexTypePreDef);
			
			vertexTypePreDef.AddOutgoingEdge(outEdgePreDef);			
			
			var requestCreateVertexType = new RequestCreateVertexType(vertexTypePreDef);
			
			_GraphDB.CreateVertexType(_SecurityToken,
			                         _TransactionToken,
			                         requestCreateVertexType,
			                         (stats, vType) => vType);
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
		/// GraphDB only accepts IDs of type 'long' as ID
		/// 
		/// If the id contains any non-numeric characters,
		/// the method will try to extract all numbers and
		/// use this as value.
		/// 
		/// If the ID doesn't contain any numbers or there is
		/// a duplicate id, exceptions will be thrown.
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
		private long ReadVertexID(XmlReader myReader, String myToken)
		{
			var vertexIDString = myReader.GetAttribute(myToken);
			
			// get numbers from string
			var vertexIDStringNumbers = String.Join(null, Regex.Split(vertexIDString, "[^\\d]"));
			
			long res;
			
			if(long.TryParse(vertexIDStringNumbers, out res))
			{	
				// store the mapping between external and internal vertex expressions
				if(!_VertexMapping.ContainsKey(vertexIDString))
				{
					_VertexMapping.Add(vertexIDString, res);
				}
				return res;	
			} else
			{
				throw new ArgumentException(String.Format(
					"ID format in graphml file was invalid: {0}", 
					vertexIDString));
			}
		}
		
		/// <summary>
		/// Inserts a vertex and its attributes into the GraphDB instance.
		/// </summary>
		/// <param name='myVertexID'>
		/// the vertex id
		/// </param>
		private void InsertVertex(XmlReader myReader, long myVertexID)
		{
			IVertex addedVertex = null;
			
			if(!_Vertices.Contains(myVertexID))
			{
				var insertRequest = new RequestInsertVertex(_VertexTypeName);
				insertRequest.SetUUID(myVertexID);
				
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
					_Vertices.Add(myVertexID);
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
										(IComparable)ParseValue(attrType, value));
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
			
			if(!_Vertices.Contains(sourceID) || !_Vertices.Contains(targetID))
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
			
			var edgePreDef = new EdgePredefinition(_EdgeTypeName);
			edgePreDef.AddVertexID(_VertexTypeName, targetID);
			edgePreDef.AddStructuredProperty(GraphMLTokens.EDGE_WEIGHT, edgeWeight);
			
			var requestUpdate = new RequestUpdate(new RequestGetVertices(_VertexTypeName, new List<long>() { sourceID }));
			requestUpdate.AddElementsToCollection(_EdgeTypeName, edgePreDef);
			
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
		private double ReadEdgeWeight(XmlReader myReader)
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
										return (float)ParseValue(attrType, value);	
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
				
				var propertyPreDefinition = new PropertyPredefinition(attrName);
				
				propertyPreDefinition.SetAttributeType(attrType);
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
				//mh..had to use invariant culture or he made 10.0 out of 1.0
                return float.Parse(myValue, CultureInfo.InvariantCulture);
            }
            else if (myType.Equals(GraphMLTokens.DOUBLE))
            {
                //mh..had to use invariant culture or he made 10.0 out of 1.0
                return Double.Parse(myValue, CultureInfo.InvariantCulture);
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
		
		#endregion
			
		#endregion
	}
}

