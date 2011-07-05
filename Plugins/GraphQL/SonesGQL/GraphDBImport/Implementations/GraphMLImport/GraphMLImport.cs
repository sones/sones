using System;
using sones.Plugins.SonesGQL.DBImport;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.GraphQL;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.DataStructures;
using sones.Library.VersionedPluginManager;
using System.Collections.Generic;
using System.Xml;
using sones.GraphDB.Request;
using sones.Library.PropertyHyperGraph;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using sones.Library.ErrorHandling;

namespace sones.Plugins.SonesGQL.GraphMLImport
{
	/// <summary>
    /// Class deserializes a graph stored in GraphML format.
    /// 
    /// GraphML description can be found here: http://graphml.graphdrawing.org/primer/graphml-primer.html
    /// 
    /// implemented:
    /// + attribute definition
    /// + vertex parsing
    /// + edge parsing
    /// + attribute parsing
    /// 
    /// not implemented:
    /// - hyperedges
    /// - nested graphs
    /// - graph meta
    /// - xml header stuff
    /// - ports
    /// - extends
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
		
		public const string PARAM_EDGETYPENAME		= "EdgeTypeName";
		
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
		
		public GraphMLImport (String myVertexTypeName, String myEdgeTypeName)
		{
			_VertexTypeName 		= myVertexTypeName;
			_EdgeTypeName 			= myEdgeTypeName;
			_VertexMapping 			= new Dictionary<string, long>();
			_AttributeDefinitions 	= new Dictionary<string, Tuple<string, string, string, string>>();
			
			_Vertices 		= new HashSet<long>();
			_EdgeCount 		= 0;
		}
		
		#endregion

		#region IGraphDBImport implementation
		
		public QueryResult Import (string myLocation, 
			IGraphDB myGraphDB,
			IGraphQL myGraphQL,
			SecurityToken mySecurityToken,
			TransactionToken myTransactionToken,
			UInt32 myParallelTasks = 1U,
			IEnumerable<string> myComments = null,
			UInt64? myOffset = null,
			UInt64? myLimit = null,
			VerbosityTypes myVerbosityTypes = VerbosityTypes.Silent)
		{
			#region data
			
			if (myGraphDB == null)
			{
				return new QueryResult("", 
						ImportFormat, 
						0, 
						ResultType.Failed, 
						null, 
						new UnknownException(new ArgumentNullException("myGraphDB")));
			}
			if(mySecurityToken == null)
			{
				return new QueryResult("", 
						ImportFormat, 
						0, 
						ResultType.Failed, 
						null, 
						new UnknownException(new ArgumentNullException("mySecurityToken")));
			}
			if(myTransactionToken == null)
			{
				return new QueryResult("", 
						ImportFormat, 
						0, 
						ResultType.Failed, 
						null, 
						new UnknownException(new ArgumentNullException("myTransactionToken")));	
			}
			
			_GraphDB 			= myGraphDB;
			_SecurityToken 		= mySecurityToken;
			_TransactionToken 	= myTransactionToken;
			
			var sw = new Stopwatch();
			
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
	                var reader = XmlReader.Create(new FileStream(myLocation,FileMode.Open));
					
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
	                return new QueryResult("", 
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
			
			return new QueryResult("", ImportFormat, (ulong)sw.ElapsedMilliseconds, ResultType.Successful, null, null);
		}

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
			if(myParameters == null || myParameters.Count == 0)
			{
				throw new ArgumentException("myParameters was either null or empty.");
			}
			if(!myParameters.ContainsKey(PARAM_VERTEXTYPENAME))
			{
				throw new ArgumentException("VertexTypeName has not been defined.");
			}
			if(!myParameters.ContainsKey(PARAM_EDGETYPENAME))
			{
				throw new ArgumentException("EdgeTypeName has not been defined.");	
			}
			
			var vertexTypeName 	= (string)myParameters[PARAM_VERTEXTYPENAME];
			var edgeTypeName 	= (string)myParameters[PARAM_EDGETYPENAME];
			
			if(vertexTypeName == null || "".Equals(vertexTypeName))
			{
				throw new ArgumentException("VertexTypeName is invalid");	
			}
			
			if(edgeTypeName == null || "".Equals(edgeTypeName))
			{
				throw new ArgumentException("EdgeTypeName is invalid");
			}
			
			myParameters.Add("test", null);
			
			return new GraphMLImport(vertexTypeName, edgeTypeName);
		}

		public string PluginName 
		{
			get { return "sones.graphmlimport"; }
		}

		public PluginParameters<Type> SetableParameters 
		{
			get 
			{ 
				return new PluginParameters<Type>()
				{
					{PARAM_VERTEXTYPENAME, 	typeof(string)},
					{PARAM_EDGETYPENAME, 	typeof(string)}
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
			
			// weighted edge
			outEdgePreDef.SetEdgeTypeAsWeighted();
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
			#region vertex info
			
            var vertexID = ReadVertexID(myReader, GraphMLTokens.ID);
			
            #endregion

            #region vertex attributes
			
			InsertVertex(myReader, vertexID);

            #endregion
		}
		
		/// <summary>
		/// Reads the vertexID from the GraphML File
		/// 
		/// GraphDB only acceps long values as ID.
		/// 
		/// If the id contains any non-numeric characters,
		/// the method will try to extract all numbers and
		/// use this as value.
		/// 
		/// If the ID doesn't contain any numbers, an
		/// exception will be thrown.
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
				throw new ArgumentException("ID format in graphml file was invalid: {0}", vertexIDString);
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
									myRequestInsertVertex.AddStructuredProperty(attrName, 
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
		
		private void ReadEdge(XmlReader myReader)
		{
			#region read edge data
			
			
            var sourceID = ReadVertexID(myReader, GraphMLTokens.SOURCE);
            var targetID = ReadVertexID(myReader, GraphMLTokens.TARGET);
			
			if(100576000000 == sourceID)
				Console.WriteLine ();
			
			if(!_Vertices.Contains(sourceID) || !_Vertices.Contains(targetID))
			{
				throw new InvalidDataException(String.Format(
					"Source or Target vertexID for edge ({0},{1}) doesn't exist",
					sourceID,
					targetID));
			}
			
			var edgeWeight = ReadEdgeWeight(myReader);
			
			#endregion
			
			#region create edge (update vertex)
			
			var edgePreDef = new EdgePredefinition(_EdgeTypeName);
			edgePreDef.AddVertexID(_VertexTypeName, targetID);
			edgePreDef.AddStructuredProperty(GraphMLTokens.EDGE_WEIGHT, edgeWeight);
			
			var requestUpdate = new RequestUpdate(new RequestGetVertices(_VertexTypeName, new List<long>() { sourceID }));
			requestUpdate.AddElementsToCollection(_EdgeTypeName, edgePreDef);
			
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
		/// Reads the edge weight from the data field of an edge definition
		/// </summary>
		/// <returns>
		/// The edge weight or the default edge weight if no weight is defined.
		/// </returns>
		/// <param name='myReader'>
		/// My reader.
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
		/// Reads the attribute definitions.
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

            _AttributeDefinitions.Add(attrId, new Tuple<string, string, string, string>(attrFor, attrName, attrType, attrDefault));
			
			#endregion
			
			#region alter vertexType
			
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
		/// Casts the given value to the given type.
		/// 
		/// The Type must implement IComparable
		/// </summary>
		/// <returns>
		/// The parsed value value.
		/// </returns>
		/// <param name='myType'>
		/// My type.
		/// </param>
		/// <param name='myValue'>
		/// My value.
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
			
		#endregion
			
		#endregion
	}
}

