using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB.Request;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request.GetVertexType;
using sones.GraphQL.GQL.ErrorHandling;
using System.Diagnostics;
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
        public override QueryResult GetResult(ParsingContext myContext,
                                                GQLPluginManager myPluginManager,
                                                IGraphDB myGraphDB,
                                                SecurityToken mySecurityToken,
                                                TransactionToken myTransactionToken)
        {
            var sw = new Stopwatch();

            sw.Reset();
            sw.Start();

            var resultingVertices = new List<IVertexView>();
            ASonesException error = null;

            if (!String.IsNullOrEmpty(_TypeName))
            {

                #region Specific type

                var request = new RequestGetVertexType(_TypeName);
                var type = myGraphDB.GetVertexType<IVertexType>(mySecurityToken, myTransactionToken, request, (stats, vertexType) => vertexType);

                if (type != null)
                {
                    resultingVertices = new List<IVertexView>() { (GenerateOutput(myContext, type, 1)) };
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
                    resultingVertices.Add(GenerateOutput(myContext, type));
                }

                #endregion
            }

            sw.Stop();

            return new QueryResult("", "GQL", (ulong)sw.ElapsedMilliseconds, ResultType.Successful, resultingVertices, error);

        }

        #endregion

        #region GenerateOutput(myDBContext, myType)

        /// <summary>
        /// Generate an output for an type with the attributes of the types and all parent types
        /// </summary>         
        /// <param name="myDBContext">The db context</param>
        /// <param name="myType">The db type</param>
        /// <param name="myDepth">If depth == 0 only the type basic attributes will be returned</param>
        private IVertexView GenerateOutput(ParsingContext myDBContext, IVertexType myType, Int32 myDepth = 0)
        {

            var retVal = new Dictionary<String, object>();

            if (myType.HasParentType)

            retVal.Add("UUID", myType.ID);
            retVal.Add("TYPE", myType.GetType());
            retVal.Add("Name", myType.Name);
            retVal.Add("Comment", myType.Comment);

            if (myDepth > 0)
            {

                retVal.Add("Properties", GeneratePropertiesOutput(myType, myDBContext, myType.GetPropertyDefinitions(true)));

                retVal.Add("Edges", GenerateEdgesOutput(myType, myDBContext, myType.GetOutgoingEdgeDefinitions(true)));

                retVal.Add("BackwardEdges", GenerateEdgesOutput(myType, myDBContext, myType.GetIncomingEdgeDefinitions(true)));

                retVal.Add("UniqueAttributes", GenerateUniquePropertiesOutput(myType, myDBContext, myType.GetUniqueDefinitions(true)));

                retVal.Add("Attributes", GenerateAttributesOutput(myType, myDBContext, myType.GetAttributeDefinitions(true)));

                retVal.Add("Indices", GenerateIndicesOutput(myType, myDBContext));

                if (myType.HasParentType)
                {
                    var _ParentType = myType.GetParentVertexType;
                    retVal.Add("Extends", GenerateOutput(myDBContext, _ParentType, myDepth - 1));
                }

            }

            return new VertexView(retVal, new Dictionary<String, IEdgeView>());

        }

        /// <summary>
        /// output for the type properties
        /// </summary>
        /// <param name="myType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myProperties">The propertyDefinitions</param>
        /// <returns>a list of readouts, contains the properties</returns>
        private IEnumerable<IVertexView> GeneratePropertiesOutput(IVertexType myType, ParsingContext myContext, IEnumerable<IPropertyDefinition> myProperties)
        {

            var _AttributeReadout = new List<IVertexView>();

            foreach (var property in myProperties)
            {

                var Attributes = new Dictionary<String, Object>();

                Attributes.Add("ID", property.AttributeID);
                Attributes.Add("TYPE", property.BaseType);
                Attributes.Add("Name", property.Name);
                Attributes.Add("UserDefined", property.IsUserDefinedType);
                Attributes.Add("DefaultValue", property.DefaultValue);
                
                _AttributeReadout.Add(new VertexView(Attributes, new Dictionary<String, IEdgeView>()));

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
        private IEnumerable<IVertexView> GenerateAttributesOutput(IVertexType myType, ParsingContext myContext, IEnumerable<IAttributeDefinition> myAttributes)
        {

            var _AttributeReadout = new List<IVertexView>();

            foreach (var attr in myAttributes)
            {

                var Attributes = new Dictionary<String, Object>();

                Attributes.Add("ID", attr.AttributeID);
                Attributes.Add("TYPE", attr.Kind);
                Attributes.Add("Name", attr.Name);
                Attributes.Add("RelatedType", attr.RelatedType);

                _AttributeReadout.Add(new VertexView(Attributes, new Dictionary<String, IEdgeView>()));

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
        private IEnumerable<IVertexView> GenerateUniquePropertiesOutput(IVertexType myType, ParsingContext myContext, IEnumerable<IUniqueDefinition> myUniques)
        {

            var _AttributeReadout = new List<IVertexView>();

            foreach (var unique in myUniques)
            {

                var Attributes = new Dictionary<String, Object>();
                
                Attributes.Add("CorrespondingIndex", unique.CorrespondingIndex);
                Attributes.Add("DefiningVertexType", unique.DefiningVertexType);
                Attributes.Add("UniqueProperties", GeneratePropertiesOutput(myType, myContext, unique.UniquePropertyDefinitions));
                
                _AttributeReadout.Add(new VertexView(Attributes, new Dictionary<String, IEdgeView>()));

            }

            return _AttributeReadout;

        }

        /// <summary>
        /// output for the type indices
        /// </summary>
        /// <param name="myType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>a list of readouts, contains the attributes</returns>
        private IEnumerable<IVertexView> GenerateIndicesOutput(IVertexType myType, ParsingContext myContext)
        {

            var _AttributeReadout = new List<IVertexView>();

            foreach (var idx in myType.GetIndexDefinitions(true))
            {

                var Attributes = new Dictionary<String, Object>();

                Attributes.Add("UUID", idx.ID);
                Attributes.Add("TYPE", idx.IndexTypeName);
                Attributes.Add("Name", idx.Name);
                Attributes.Add("Edition", idx.Edition);

                _AttributeReadout.Add(new VertexView(Attributes, new Dictionary<String, IEdgeView>()));

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
        private IEnumerable<IVertexView> GenerateEdgesOutput(IVertexType myType, ParsingContext myDBContext, IEnumerable<IAttributeDefinition> myEdges)
        {

            var _AttributeReadout = new List<IVertexView>();

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

                Attributes.Add("UUID", edge.AttributeID);
                Attributes.Add("Name", edge.Name);
                
                _AttributeReadout.Add(new VertexView(Attributes, new Dictionary<String, IEdgeView>()));

            }

            return _AttributeReadout;

        }

        #endregion
    }
}
