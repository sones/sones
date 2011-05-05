using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Expression;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.Expression.Tree;
using sones.GraphDB.Request;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Request.Insert;
using sones.GraphDB.TypeManagement.Base;

namespace sones.GraphDB.Manager.Vertex
{
    internal abstract class AVertexHandler: IManager
    {
        protected IManagerOf<IVertexTypeHandler> _vertexTypeManager;


        protected IVertexType GetVertexType(String myVertexTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            try
            {
                //check if the vertex type exists.
                return _vertexTypeManager.ExecuteManager.GetVertexType(myVertexTypeName, myTransaction, mySecurity);
            }
            catch (KeyNotFoundException)
            {
                throw new VertexTypeDoesNotExistException(myVertexTypeName);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new EmptyVertexTypeNameException();
            }
        }

        protected void CheckMandatoryConstraint(IPropertyProvider myPropertyProvider, IBaseType myType)
        {
            var mandatories = myType.GetPropertyDefinitions(true).Where(IsMustSetProperty).ToArray();

            foreach (var mand in mandatories)
            {
                if ( myPropertyProvider == null || myPropertyProvider.StructuredProperties == null || !myPropertyProvider.StructuredProperties.Any(x => mand.Name.Equals(x.Key)))
                {
                    throw new MandatoryConstraintViolationException(mand.Name);
                }
            }
        }

        private static bool IsMustSetProperty(IPropertyDefinition myPropertyDefinition)
        {
            return IsMandatoryProperty(myPropertyDefinition) && !HasDefaultValue(myPropertyDefinition) && myPropertyDefinition.RelatedType.ID != (long)BaseTypes.Vertex;
        }

        private static bool IsMandatoryProperty(IPropertyDefinition myPropertyDefinition)
        {
            return myPropertyDefinition.IsMandatory;
        }

        private static bool HasDefaultValue(IPropertyDefinition myPropertyDefinition)
        {
            return myPropertyDefinition.DefaultValue != null;
        }



        #region IManager Members

        public virtual void Initialize(IMetaManager myMetaManager)
        {
            _vertexTypeManager = myMetaManager.VertexTypeManager;
        }

        public virtual void Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
        }

        #endregion
    }
}
