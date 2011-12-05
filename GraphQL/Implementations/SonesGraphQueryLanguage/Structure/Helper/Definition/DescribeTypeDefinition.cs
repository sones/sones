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
using System.Linq;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.Library.CollectionWrapper;
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
        public override IEnumerable<IVertexView> GetResult(GQLPluginManager myPluginManager,
                                                IGraphDB myGraphDB,
                                                SecurityToken mySecurityToken,
                                                Int64 myTransactionToken)
        {
            var resultingVertices = new List<IVertexView>();

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
                    throw new VertexTypeDoesNotExistException(_TypeName, "");
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

            return resultingVertices;
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

            //base output
            retVal.Add("VertexID", myType.ID);
            retVal.Add("Type", myType.GetType().Name);
            retVal.Add("Name", myType.Name);
            retVal.Add("IsUserDefined", myType.IsUserDefined);
             
            //additional output
            if (myDepth > 0)
            {
                retVal.Add("IsAbstract", myType.IsAbstract);
                
                edges.Add("Properties", new HyperEdgeView(null, GeneratePropertiesOutput(myType, myType.GetPropertyDefinitions(true), myDepth)));

                edges.Add("Edges", new HyperEdgeView(null, GenerateEdgesOutput(myType, myType.GetOutgoingEdgeDefinitions(true))));

                edges.Add("Incomingedges", new HyperEdgeView(null, GenerateEdgesOutput(myType, myType.GetIncomingEdgeDefinitions(true))));

                edges.Add("UniqueAttributes", new HyperEdgeView(null, GenerateUniquePropertiesOutput(myType, myType.GetUniqueDefinitions(true))));

                edges.Add("Indices", new HyperEdgeView(null, GenerateIndicesOutput(myType)));

                if (myType.HasParentType)
                    edges.Add("Extends", new SingleEdgeView(null, GenerateOutput(myType.ParentVertexType)));

                if (myType.HasChildTypes)
                {
                    List<ISingleEdgeView> list = new List<ISingleEdgeView>();

                    foreach (var child in myType.ChildrenVertexTypes)
                        list.Add(new SingleEdgeView(null, GenerateOutput(child)));
                    
                    edges.Add("ChildrenVertexTypes", new HyperEdgeView(null, list));
                }

                if (!string.IsNullOrWhiteSpace(myType.Comment))
                    retVal.Add("Comment", myType.Comment);
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
        private IEnumerable<ISingleEdgeView> GeneratePropertiesOutput(IVertexType myType, IEnumerable<IPropertyDefinition> myProperties, Int32 myDepth = 0)
        {

            var _AttributeReadout = new List<ISingleEdgeView>();

            foreach (var property in myProperties)
            {

                var Attributes = new Dictionary<String, Object>();

                Attributes.Add("ID", property.ID);
                Attributes.Add("Type", property.BaseType.Name);
                Attributes.Add("Name", property.Name);
                Attributes.Add("IsUserDefined", property.IsUserDefined);

                if (myDepth > 0)
                {
                    Attributes.Add("Multiplicity", property.Multiplicity);
                    Attributes.Add("IsMandatory", property.IsMandatory);

                    if (property.DefaultValue != null)
                        Attributes.Add("DefaultValue", property.DefaultValue);
                }

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

                Attributes.Add("CorrespondingIndex", unique.CorrespondingIndex.Name);
                Attributes.Add("DefiningVertexType", unique.DefiningVertexType.Name);

                var list = new ListCollectionWrapper();
                foreach (var item in unique.UniquePropertyDefinitions)
                    list.Add(item.Name);

                Attributes.Add("UniqueProperties", list);

                _AttributeReadout.Add(new SingleEdgeView(null, new VertexView(Attributes, new Dictionary<String, IEdgeView>())));

            }

            return _AttributeReadout;

        }

        /// <summary>
        /// output for the type mandatories
        /// </summary>
        /// <param name="myType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myMadatories">The uniqueDefinitions</param>
        /// <returns>a list of readouts, contains the attributes</returns>
        private IEnumerable<ISingleEdgeView> GenerateMandatoryPropertiesOutput(IVertexType myType, IEnumerable<IPropertyDefinition> myMandatories)
        {

            var _AttributeReadout = new List<ISingleEdgeView>();

            foreach (var mandatory in myMandatories)
            {

                var Attributes = new Dictionary<String, Object>();

                Attributes.Add("ID", mandatory.ID);
                Attributes.Add("Type", mandatory.Kind);
                Attributes.Add("Name", mandatory.Name);
                Attributes.Add("IsMandatory", mandatory.IsMandatory);

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

                Attributes.Add("ID", idx.ID);
                Attributes.Add("Type", idx.IndexTypeName);
                Attributes.Add("Name", idx.Name);
                Attributes.Add("Edition", idx.Edition);

                var list = new ListCollectionWrapper();
                foreach (var item in idx.IndexedProperties)
                    list.Add(item.Name);

                Attributes.Add("IndexedProperties", list);

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
                    Attributes.Add("RelatedEdgeName", (edge as IIncomingEdgeDefinition).RelatedEdgeDefinition.Name);
                    Attributes.Add("RelatedEdgeSourceVertexType", (edge as IIncomingEdgeDefinition).RelatedEdgeDefinition.SourceVertexType.Name);
                    Attributes.Add("RelatedEdgeTargetVertexType", (edge as IIncomingEdgeDefinition).RelatedEdgeDefinition.TargetVertexType.Name);
                }
                else if (edge.Kind == AttributeType.OutgoingEdge)
                {
                    Attributes.Add("Type", (edge as IOutgoingEdgeDefinition).EdgeType.Name);
                    Attributes.Add("SourceVertexType", (edge as IOutgoingEdgeDefinition).SourceVertexType.Name);
                    Attributes.Add("TargetVertexType", (edge as IOutgoingEdgeDefinition).TargetVertexType.Name);
                }

                Attributes.Add("ID", edge.ID);
                Attributes.Add("Name", edge.Name);

                _AttributeReadout.Add(new SingleEdgeView(null, new VertexView(Attributes, null)));

            }

            return _AttributeReadout;

        }

        #endregion
    }
}
