using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Expression;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Expression.Tree;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Request.Insert;
using sones.GraphDB.TypeManagement.Base;

namespace sones.GraphDB.Manager.Vertex
{
    class CheckVertexManager: AVertexHandler, IVertexHandler
    {
        #region IVertexHandler Members

        public IEnumerable<IVertex> GetVertices(IExpression myExpression, bool myIsLongrunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            return null;
        }

        public IEnumerable<IVertex> GetVertices(long myTypeID, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            _vertexTypeManager.CheckManager.GetVertexType(myTypeID, myTransaction, mySecurity);
            return null;
        }

        public IEnumerable<IVertex> GetVertices(string myVertexType, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            _vertexTypeManager.CheckManager.GetVertexType(myVertexType, myTransaction, mySecurity);
            return null;
        }

        public IVertex GetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        public IVertex GetVertex(long myVertexTypeID, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        public IVertex GetSingleVertex(IExpression myExpression, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        public IVertex AddVertex(RequestInsertVertex myInsertDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            IVertexType vertexType = GetVertexType(myInsertDefinition.VertexTypeName, myTransaction, mySecurity);

            if (vertexType.IsAbstract)
                throw new AbstractConstraintViolationException(myInsertDefinition.VertexTypeName);

            ConvertUnknownProperties(myInsertDefinition, vertexType);


            var mandatoryProps = vertexType.GetPropertyDefinitions(true).Where(IsMustSetProperty).ToArray();


            if (myInsertDefinition.StructuredProperties != null)
            {
                CheckAddStructuredProperties(myInsertDefinition, vertexType);
                CheckMandatoryConstraint(myInsertDefinition, mandatoryProps);
            }
            else
            {
                if (mandatoryProps.Length > 0)
                {
                    throw new MandatoryConstraintViolationException(String.Join(",", mandatoryProps.Select(x => x.Name)));
                }
            }

            if (myInsertDefinition.BinaryProperties != null)
                CheckAddBinaryProperties(myInsertDefinition, vertexType);

            return null;
        }

        public IVertexStore VertexStore
        {
            get { return null; }
        }

        #endregion

        private static bool IsMustSetProperty(IPropertyDefinition myPropertyDefinition)
        {
            return IsMandatoryProperty(myPropertyDefinition) && !HasDefaultValue(myPropertyDefinition);
        }

        private static bool IsMandatoryProperty(IPropertyDefinition myPropertyDefinition)
        {
            return myPropertyDefinition.IsMandatory;
        }

        private static bool HasDefaultValue(IPropertyDefinition myPropertyDefinition)
        {
            return myPropertyDefinition.DefaultValue != null;
        }

        private static void ConvertUnknownProperties(IPropertyProvider myPropertyProvider, IBaseType myBaseType)
        {
            if (myPropertyProvider.UnknownProperties != null)
            {
                foreach (var unknownProp in myPropertyProvider.UnknownProperties)
                {
                    if (myBaseType.HasProperty(unknownProp.Key))
                    {
                        if (unknownProp.Value is IComparable)
                        {
                            myPropertyProvider.AddStructuredProperty(unknownProp.Key, (IComparable)unknownProp.Value);
                        }
                        else
                        {
                            //TODO: better exception
                            throw new Exception("Type of property does not match.");
                        }
                    }
                    else
                    {
                        myPropertyProvider.AddUnstructuredProperty(unknownProp.Key, unknownProp.Value);
                    }
                }
                myPropertyProvider.ClearUnknown();
            }
        }

        private static void CheckMandatoryConstraint(RequestInsertVertex myInsertDefinition, IEnumerable<IPropertyDefinition> myMandatoryProperties)
        {
            foreach (var mand in myMandatoryProperties)
            {
                if (mand.RelatedType.ID == (long)BaseTypes.Vertex)
                    if (!myInsertDefinition.StructuredProperties.Any(x => mand.Name.Equals(x)))
                    {
                        throw new MandatoryConstraintViolationException(mand.Name);
                    }
            }
        }

        private static void CheckAddStructuredProperties(RequestInsertVertex myInsertDefinition, IVertexType vertexType)
        {
            foreach (var prop in myInsertDefinition.StructuredProperties)
            {
                var propertyDef = vertexType.GetPropertyDefinition(prop.Key);
                if (propertyDef == null)
                    throw new AttributeDoesNotExistException(prop.Key, myInsertDefinition.VertexTypeName);

                if (propertyDef.Multiplicity == PropertyMultiplicity.Single)
                {
                    CheckPropertyType(myInsertDefinition, prop.Key, prop.Value, propertyDef);
                }
                else
                {
                    IEnumerable<IComparable> items = prop.Value as IEnumerable<IComparable>;
                    if (items == null)
                    {
                        throw new PropertyHasWrongTypeException(myInsertDefinition.VertexTypeName, prop.Key, propertyDef.Multiplicity, propertyDef.BaseType.Name);
                    }

                    foreach (var item in items)
                    {
                        CheckPropertyType(myInsertDefinition, prop.Key, item, propertyDef);
                    }
                }
            }
        }

        private static void CheckPropertyType(RequestInsertVertex myInsertDefinition, String myAttributeName, IComparable myValue, IPropertyDefinition propertyDef)
        {
            //Assign safty should be suffice.
            if (!propertyDef.BaseType.IsAssignableFrom(myValue.GetType()))
                throw new PropertyHasWrongTypeException(myInsertDefinition.VertexTypeName, myAttributeName, propertyDef.BaseType.Name, myValue.GetType().Name);
        }

        private static void CheckAddBinaryProperties(RequestInsertVertex myInsertDefinition, IVertexType vertexType)
        {
            foreach (var prop in myInsertDefinition.BinaryProperties)
            {
                var propertyDef = vertexType.GetBinaryPropertyDefinition(prop.Key);
                if (propertyDef == null)
                    throw new AttributeDoesNotExistException(prop.Key, myInsertDefinition.VertexTypeName);
            }
        }



    }
}
