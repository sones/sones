using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Manager.Vertex;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.GraphDB.Manager;
using sones.GraphDB.Expression;
using sones.GraphDB.TypeManagement.BaseTypes;
using System.Collections;

namespace sones.GraphDB.TypeManagement
{
    internal class VertexType: IVertexType
    {
        /// <summary>
        /// This is the initialization count of the result list of a GetChildVertices method
        /// </summary>
        private const int ExpectedChildTypes = 50;

        IVertex _Vertex;
        

        #region c'tor

        private VertexType(IVertex myVertex)
        {
            //_Expr = new BinaryExpression(new ConstantExpression(_ID), BinaryOperator.Equals, new AttributeExpression("VertexType", "ID"));
            _Vertex = myVertex;
        }

        #endregion

        #region IVertexType Members

        long IVertexType.ID
        {
            get { return GetVertex().GetProperty<long>(AttributeDefinitions.ID.AttributeID); }
        }

        string IVertexType.Name
        {
            get { return GetVertex().GetPropertyAsString(AttributeDefinitions.Name.AttributeID); }
        }

        IBehaviour IVertexType.Behaviour
        {
            get 
            {
                throw new NotImplementedException();
            }
        }

        string IVertexType.Comment
        {
            get 
            {
                return GetVertex().Comment;
            }
        }

        bool IVertexType.IsAbstract
        {
            get 
            {
                return GetVertex().GetProperty<bool>(AttributeDefinitions.IsAbstractOnBaseType.AttributeID);
            }
        }

        bool IVertexType.IsSealed
        {
            get
            {
                return GetVertex().GetProperty<bool>(AttributeDefinitions.IsSealedOnBaseType.AttributeID);
            }
        }

        bool IVertexType.HasParentVertexType
        {
            /// All vertices are at least inherit from Vertex
            get { return true; }
        }

        IVertexType IVertexType.GetParentVertexType
        {
            get 
            {
                var parentVertex = GetVertex().GetOutgoingSingleEdge(AttributeDefinitions.ParentOnVertexType.AttributeID).GetTargetVertex();
                return new VertexType(parentVertex);
            }
        }

        bool IVertexType.HasChildVertexTypes
        {
            get 
            {
                return GetVertex().HasIncomingEdge((long)BaseVertexType.VertexType, AttributeDefinitions.ParentOnVertexType.AttributeID);
            }
        }

        IEnumerable<IVertexType> IVertexType.GetChildVertexTypes
        {
            get 
            { 
                var vertices = GetVertex().GetIncomingVertices((long)BaseVertexType.VertexType, AttributeDefinitions.ParentOnVertexType.AttributeID);

                //Perf: initialize the result list with a size
                List<IVertexType> result = (vertices is ICollection)
                    ? new List<IVertexType>((vertices as ICollection).Count)
                    : new List<IVertexType>(ExpectedChildTypes);

                foreach (var vertex in vertices)
                    result.Add(new VertexType(vertex));

                return result;
            }
        }

        IEnumerable<IAttributeDefinition> IVertexType.GetAttributeDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IPropertyDefinition> IVertexType.GetPropertyDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IIncomingEdgeDefinition IVertexType.GetIncomingEdgeDefinition(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        bool IVertexType.HasVisibleIncomingEdges(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IIncomingEdgeDefinition> IVertexType.GetIncomingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IOutgoingEdgeDefinition IVertexType.GetOutgoingEdgeDefinition(string myEdgeName)
        {
            throw new NotImplementedException();
        }

        bool IVertexType.HasOutgoingEdges(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IOutgoingEdgeDefinition> IVertexType.GetOutgoingEdgeDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IUniqueDefinition> IVertexType.GetUniqueDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IIndexDefinition> IVertexType.GetIndexDefinitions(bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        IAttributeDefinition IVertexType.GetAttributeDefinition(string myAttributeName)
        {
            throw new NotImplementedException();
        }

        IPropertyDefinition IVertexType.GetPropertyDefinition(string myPropertyName)
        {
            throw new NotImplementedException();
        }

        #endregion

        private IVertex GetVertex()
        {
            return _Vertex;
        }

    }
}
