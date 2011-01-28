/*
 * DescribeTypeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{
    /// <summary>
    /// Describes an db type an all its derived types
    /// </summary>
    public class DescribeTypeDefinition : ADescribeDefinition
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
        public override Exceptional<IEnumerable<Vertex>> GetResult(DBContext myDBContext)
        {

            if (!String.IsNullOrEmpty(_TypeName))
            {

                #region Specific type

                var type = myDBContext.DBTypeManager.GetTypeByName(_TypeName);
                if (type != null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new List<Vertex>(){(GenerateOutput(myDBContext, type, 1))});
                }
                else
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(_TypeName));
                }

                #endregion

            }
            else
            {

                #region All types

                var resultingReadouts = new List<Vertex>();

                foreach (var type in myDBContext.DBTypeManager.GetAllTypes())
                {
                    resultingReadouts.Add(GenerateOutput(myDBContext, type));
                }

                return new Exceptional<IEnumerable<Vertex>>(resultingReadouts);

                #endregion
            }
            
        }
        
        #endregion

        #region GenerateOutput(myDBContext, myGraphDBType)

        /// <summary>
        /// Generate an output for an type with the attributes of the types and all parent types
        /// </summary>         
        /// <param name="myGraphDBType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myDepth">If depth == 0 only the type basic attributes will be returned</param>
        private Vertex GenerateOutput(DBContext myDBContext, GraphDBType myGraphDBType, Int32 myDepth = 0)
        {

            var retVal = new Vertex();

            if (myGraphDBType.ParentTypeUUID != null)

            retVal.AddAttribute("UUID",        myGraphDBType.UUID);
            retVal.AddAttribute("TYPE",        myGraphDBType.GetType().Name);
            retVal.AddAttribute("Name",        myGraphDBType.Name);
            retVal.AddAttribute("Comment",     myGraphDBType.Comment);

            if (myDepth > 0)
            {

                retVal.AddAttribute("Properties", new Edge(retVal, GeneratePropertiesOutput(myGraphDBType, myDBContext,
                    myGraphDBType.Attributes.Where(a => !(a.Value.EdgeType is IReferenceEdge)).Select(kv => kv.Value)))
                {
                    EdgeTypeName = "Property"
                });

                retVal.AddAttribute("Edges", new Edge(retVal, GenerateEdgesOutput(myGraphDBType, myDBContext, myGraphDBType.Attributes.Where(a => (a.Value.EdgeType is IReferenceEdge) && !a.Value.IsBackwardEdge).Select(kv => kv.Value)))
                {
                    EdgeTypeName = "Edge"
                });

                retVal.AddAttribute("BackwardEdges", new Edge(retVal, GenerateEdgesOutput(myGraphDBType, myDBContext, myGraphDBType.Attributes.Where(a => (a.Value.EdgeType is IReferenceEdge) && a.Value.IsBackwardEdge).Select(kv => kv.Value)))
                {
                    EdgeTypeName = "Edge"
                });
                
                retVal.AddAttribute("UniqueAttributes", new Edge(retVal, GeneratePropertiesOutput(myGraphDBType, myDBContext,
                    myGraphDBType.GetUniqueAttributes().Select(a => myGraphDBType.GetTypeAttributeByUUID(a))))
                {
                    EdgeTypeName = "Unique"
                });

                retVal.AddAttribute("MandatoryAttributes", new Edge(retVal, GeneratePropertiesOutput(myGraphDBType, myDBContext,
                    myGraphDBType.GetMandatoryAttributes().Select(a => myGraphDBType.GetTypeAttributeByUUID(a))))
                {
                    EdgeTypeName = "Mandatory"
                });

                retVal.AddAttribute("Indices", new Edge(retVal, GenerateIndicesOutput(myGraphDBType, myDBContext))
                {
                    EdgeTypeName = "Index"
                });

                if (myGraphDBType.ParentTypeUUID != null)
                {
                    var _ParentType = myDBContext.DBTypeManager.GetTypeByUUID(myGraphDBType.ParentTypeUUID);
                    retVal.AddAttribute("Extends", new Edge(retVal, GenerateOutput(myDBContext, _ParentType, myDepth - 1))
                    {
                        EdgeTypeName = "VertexType"
                    });
                }

            }

            return retVal;

        }

        /// <summary>
        /// output for the type attributes
        /// </summary>
        /// <param name="myGraphDBType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>a list of readouts, contains the attributes</returns>
        private IEnumerable<IVertex> GeneratePropertiesOutput(GraphDBType myGraphDBType, DBContext myDBContext, IEnumerable<TypeAttribute> myAttributes)
        {

            var _AttributeReadout = new List<Vertex>();

            foreach (var attr in myAttributes)
            {

                var Attributes = new Dictionary<String, Object>();
                Attributes.Add("UUID", attr.UUID);
                Attributes.Add("TYPE", attr.GetDBType(myDBContext.DBTypeManager));
                //Attributes.Add("UUID", new ObjectUUID(_KeyValuePair.Value.UUID as UUID));
                Attributes.Add("Name", attr.Name);
                if (attr.EdgeType != null)
                {
                    Attributes.Add("Collection", attr.EdgeType.EdgeTypeName);
                }

                if (attr.DefaultValue != null)
                {
                    Attributes.Add("DefaultValue", attr.DefaultValue.GetReadoutValue());
                }

                _AttributeReadout.Add(new Vertex(Attributes));

            }

            return _AttributeReadout;

        }

        /// <summary>
        /// output for the type attributes
        /// </summary>
        /// <param name="myGraphDBType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>a list of readouts, contains the attributes</returns>
        private IEnumerable<IVertex> GenerateIndicesOutput(GraphDBType myGraphDBType, DBContext myDBContext)
        {

            var _AttributeReadout = new List<Vertex>();

            foreach (var idx in myGraphDBType.GetAllAttributeIndices(myDBContext, false))
            {

                var Attributes = new Dictionary<String, Object>();
                Attributes.Add("UUID", idx.ObjectUUID);
                Attributes.Add("TYPE", idx.IndexType);
                Attributes.Add("Name", idx.IndexName);
                Attributes.Add("Edition", idx.IndexEdition);
           
                _AttributeReadout.Add(new Vertex(Attributes));

            }

            return _AttributeReadout;

        }

        /// <summary>
        /// output for the type attributes
        /// </summary>
        /// <param name="myGraphDBType">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>a list of readouts, contains the attributes</returns>
        private IEnumerable<Vertex> GenerateEdgesOutput(GraphDBType myGraphDBType, DBContext myDBContext, IEnumerable<TypeAttribute> myAttributes)
        {

            var _AttributeReadout = new List<Vertex>();

            foreach (var attr in myAttributes)
            {

                var Attributes = new Dictionary<String, Object>();

                if (attr.IsBackwardEdge)
                {
                    Attributes.Add("TYPE", attr.BackwardEdgeDefinition.GetTypeAndAttributeInformation(myDBContext.DBTypeManager).Item1);
                    Attributes.Add("ATTRIBUTE", attr.BackwardEdgeDefinition.GetTypeAndAttributeInformation(myDBContext.DBTypeManager).Item2);
                }
                else
                {
                    Attributes.Add("TYPE", attr.GetDBType(myDBContext.DBTypeManager));
                }

                //Attributes.Add("UUID", new ObjectUUID(_KeyValuePair.Value.UUID as UUID));
                Attributes.Add("UUID", attr.UUID);
                Attributes.Add("Name", attr.Name);
                Attributes.Add("EdgeType", attr.EdgeType.EdgeTypeName);

                foreach (var par in attr.EdgeType.GetParams())
                {
                    Attributes.Add(par.Type.ToString(), par.Param);
                }
                _AttributeReadout.Add(new Vertex(Attributes));

            }

            return _AttributeReadout;

        }

        #endregion

    }
}
