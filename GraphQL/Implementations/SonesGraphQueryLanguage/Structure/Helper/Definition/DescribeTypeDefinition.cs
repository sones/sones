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
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphDB.Request.GetVertexType;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    public sealed class DescribeTypeDefinition : ADescribeDefinition
    {
        #region Data

        /// <summary>
        /// The type name
        /// </summary>
        private String _TypeName;

        #endregion

        #region Ctor

        public DescribeTypeDefinition(string myTypeName = null)
        {
            _TypeName = myTypeName;
        }

        #endregion

        #region ADescribeDefinition

        /// <summary>
        /// <seealso cref=" ADescribeDefinition"/>
        /// </summary>
        public override QueryResult GetResult(
                                                GQLPluginManager myPluginManager,
                                                IGraphDB myGraphDB,
                                                SecurityToken mySecurityToken,
                                                TransactionToken myTransactionToken)
        {
            var resultingVertices = new List<IVertexView>();
            ASonesException error = null;

            if (!String.IsNullOrEmpty(_TypeName))
            {

                #region Specific type

                var request = new RequestGetVertexType(_TypeName);
                var type = myGraphDB.GetVertexType<IVertexType>(mySecurityToken, myTransactionToken, request, (stats, vertexType) => vertexType);

                if (type != null)
                {
                    resultingVertices = new List<IVertexView>() { (GenerateOutput(type, 1)) };
                }
                else
                {
                    error = new VertexTypeDoesNotExistException(_TypeName, "");
                }

                #endregion

            }
            else
            {

                #region All types

                foreach (var type in myGraphDB.GetAllVertexTypes<IEnumerable<IVertexType>>(mySecurityToken, 
                                                                                            myTransactionToken, 
                                                                                            new RequestGetAllVertexTypes(), 
                                                                                            (stats, vertexTypes) => vertexTypes))
                {
                    resultingVertices.Add(GenerateOutput(type));
                }

                #endregion
            }

            if (error != null)
                return new QueryResult("", "GQL", 0L, ResultType.Failed, resultingVertices, error);
            else
                return new QueryResult("", "GQL", 0L, ResultType.Successful, resultingVertices);

        }

        #endregion

        #region GenerateOutput(myDBContext, myType)

        /// <summary>
        /// Generate an output for an type with the attributes of the types and all parent types
        /// </summary>         
        /// <param name="myDBContext">The db context</param>
        /// <param name="myType">The db type</param>
        /// <param name="myDepth">If depth == 0 only the type basic attributes will be returned</param>
        private IVertexView GenerateOutput(IVertexType myType, Int32 myDepth = 0)
        {

            var retVal = new Dictionary<String, object>();

            List<IVertexView> result = new List<IVertexView>();
            var edges = new Dictionary<String, IEdgeView>();

            retVal.Add("UUID", myType.ID);
            retVal.Add("TYPE", myType.GetType());
            retVal.Add("Name", myType.Name);

            if (!string.IsNullOrWhiteSpace(myType.Comment))
                
            {
                retVal.Add("Comment", myType.Comment);
            }

            if (myDepth > 0)
            {               
                
                edges.Add("Properties", new HyperEdgeView(null, GeneratePropertiesOutput(myType, myType.GetPropertyDefinitions(true))));

                edges.Add("Edges", new HyperEdgeView(null, GenerateEdgesOutput(myType, myType.GetOutgoingEdgeDefinitions(true))));

                edges.Add("Incomingedges", new HyperEdgeView(null, GenerateEdgesOutput(myType, myType.GetIncomingEdgeDefinitions(true))));

                edges.Add("UniqueAttributes", new HyperEdgeView(null, GenerateUniquePropertiesOutput(myType, myType.GetUniqueDefinitions(true))));

                edges.Add("Attributes", new HyperEdgeView(null, GenerateAttributesOutput(myType, myType.GetAttributeDefinitions(true))));

                edges.Add("Indices", new HyperEdgeView(null, GenerateIndicesOutput(myType)));

                if (myType.HasParentType)
                {
                    var _ParentType = myType.ParentVertexType;
                    edges.Add("Extends", new SingleEdgeView(null, GenerateOutput(_ParentType, myDepth - 1)));
                }

            }

            return new VertexView(retVal, edges);

        }

        /// <summary>
        /// output for the type properties
        /// </summary>
        /// <param name="myType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myProperties">The propertyDefinitions</param>
        /// <returns>a list of readouts, contains the properties</returns>
        private IEnumerable<ISingleEdgeView> GeneratePropertiesOutput(IVertexType myType, IEnumerable<IPropertyDefinition> myProperties)
        {

            var _AttributeReadout = new List<ISingleEdgeView>();

            foreach (var property in myProperties)
            {

                var Attributes = new Dictionary<String, Object>();

                Attributes.Add("ID", property.ID);
                Attributes.Add("TYPE", property.BaseType);
                Attributes.Add("Name", property.Name);
                Attributes.Add("UserDefined", property.IsUserDefinedType);

                if (property.DefaultValue != null)
                    Attributes.Add("DefaultValue", property.DefaultValue);
                
                _AttributeReadout.Add(new SingleEdgeView(null, new VertexView(Attributes, new Dictionary<String, IEdgeView>())));

            }

            return _AttributeReadout;

        }

        /// <summary>
        /// output for the type attributes
        /// </summary>
        /// <param name="myType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myAttributes">The attributeDefinitions</param>
        /// <returns>a list of readouts, contains the attributes</returns>
        private IEnumerable<ISingleEdgeView> GenerateAttributesOutput(IVertexType myType, IEnumerable<IAttributeDefinition> myAttributes)
        {

            var _AttributeReadout = new List<ISingleEdgeView>();

            foreach (var attr in myAttributes)
            {

                var Attributes = new Dictionary<String, Object>();

                Attributes.Add("ID", attr.ID);
                Attributes.Add("TYPE", attr.Kind);
                Attributes.Add("Name", attr.Name);
                
                _AttributeReadout.Add(new SingleEdgeView(null, new VertexView(Attributes, new Dictionary<String, IEdgeView>())));

            }

            return _AttributeReadout;

        }

        /// <summary>
        /// output for the type uniques
        /// </summary>
        /// <param name="myType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myUniques">The uniqueDefinitions</param>
        /// <returns>a list of readouts, contains the attributes</returns>
        private IEnumerable<ISingleEdgeView> GenerateUniquePropertiesOutput(IVertexType myType, IEnumerable<IUniqueDefinition> myUniques)
        {

            var _AttributeReadout = new List<ISingleEdgeView>();

            foreach (var unique in myUniques)
            {

                var Attributes = new Dictionary<String, Object>();
                
                Attributes.Add("CorrespondingIndex", unique.CorrespondingIndex);
                Attributes.Add("DefiningVertexType", unique.DefiningVertexType);
                Attributes.Add("UniqueProperties", GeneratePropertiesOutput(myType, unique.UniquePropertyDefinitions));
                
                _AttributeReadout.Add(new SingleEdgeView(null, new VertexView(Attributes, new Dictionary<String, IEdgeView>())));

            }

            return _AttributeReadout;

        }

        /// <summary>
        /// output for the type indices
        /// </summary>
        /// <param name="myType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>a list of readouts, contains the attributes</returns>
        private IEnumerable<ISingleEdgeView> GenerateIndicesOutput(IVertexType myType)
        {

            var _AttributeReadout = new List<ISingleEdgeView>();

            foreach (var idx in myType.GetIndexDefinitions(true))
            {

                var Attributes = new Dictionary<String, Object>();

                Attributes.Add("UUID", idx.ID);
                Attributes.Add("TYPE", idx.IndexTypeName);
                Attributes.Add("Name", idx.Name);
                Attributes.Add("Edition", idx.Edition);

                _AttributeReadout.Add(new SingleEdgeView(null, new VertexView(Attributes, new Dictionary<String, IEdgeView>())));

            }

            return _AttributeReadout;

        }

        /// <summary>
        /// output for the edges
        /// </summary>
        /// <param name="myType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myEdges">The EdgeDefinitions</param>
        /// <returns>a list of readouts, contains the attributes</returns>
        private IEnumerable<ISingleEdgeView> GenerateEdgesOutput(IVertexType myType, IEnumerable<IAttributeDefinition> myEdges)
        {

            var _AttributeReadout = new List<ISingleEdgeView>();

            foreach (var edge in myEdges)
            {

                var Attributes = new Dictionary<String, Object>();

                if (edge.Kind == AttributeType.IncomingEdge)
                {
                    Attributes.Add("TYPE", (edge as IIncomingEdgeDefinition).RelatedEdgeDefinition.EdgeType);
                }
                else if (edge.Kind == AttributeType.OutgoingEdge)
                {
                    Attributes.Add("TYPE", (edge as IOutgoingEdgeDefinition).EdgeType);
                    Attributes.Add("SourceVertexType", (edge as IOutgoingEdgeDefinition).SourceVertexType);
                    Attributes.Add("TargetVertexType", (edge as IOutgoingEdgeDefinition).TargetVertexType);
                }

                Attributes.Add("UUID", edge.ID);
                Attributes.Add("Name", edge.Name);
                
                _AttributeReadout.Add(new SingleEdgeView(null, new VertexView(Attributes, new Dictionary<String, IEdgeView>())));

            }

            return _AttributeReadout;

        }

        #endregion
    }
}
