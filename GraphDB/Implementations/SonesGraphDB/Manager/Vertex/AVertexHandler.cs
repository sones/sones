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
using sones.GraphDB.Manager.QueryPlan;
using sones.Library.LanguageExtensions;

namespace sones.GraphDB.Manager.Vertex
{
    internal abstract class AVertexHandler: IManager
    {
        protected IManagerOf<IVertexTypeHandler> _vertexTypeManager;
        protected IQueryPlanManager _queryPlanManager;


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

        protected static void ConvertUnknownProperties(IPropertyProvider myPropertyProvider, IBaseType myBaseType)
        {
            if (myPropertyProvider.UnknownProperties != null)
            {
                foreach (var unknownProp in myPropertyProvider.UnknownProperties)
                {   
                    //ASK: What's about binary properties?
                    if (myBaseType.HasProperty(unknownProp.Key))
                    {
                        var propDef = myBaseType.GetPropertyDefinition(unknownProp.Key);

                        try
                        {
                            var converted = unknownProp.Value.ConvertToIComparable(propDef.BaseType);
                            myPropertyProvider.AddStructuredProperty(unknownProp.Key, converted);
                        }
                        catch (InvalidCastException)                 
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

        protected static void CheckAddStructuredProperties(IDictionary<String, IComparable> myProperties, IVertexType vertexType)
        {
            foreach (var prop in myProperties)
            {
                var propertyDef = vertexType.GetPropertyDefinition(prop.Key);
                if (propertyDef == null)
                    throw new AttributeDoesNotExistException(prop.Key, vertexType.Name);

                if (propertyDef.Multiplicity == PropertyMultiplicity.Single)
                {
                    CheckPropertyType(vertexType.Name, prop.Value, propertyDef);
                }
                else
                {
                    IEnumerable<IComparable> items = prop.Value as IEnumerable<IComparable>;
                    if (items == null)
                    {
                        throw new PropertyHasWrongTypeException(vertexType.Name, prop.Key, propertyDef.Multiplicity, propertyDef.BaseType.Name);
                    }

                    foreach (var item in items)
                    {
                        CheckPropertyType(vertexType.Name, item, propertyDef);
                    }
                }
            }
        }

        protected static void CheckPropertyType(String myVertexTypeName, IComparable myValue, IPropertyDefinition propertyDef)
        {
            //Assign safty should be suffice.
            if (!propertyDef.BaseType.IsAssignableFrom(myValue.GetType()))
                throw new PropertyHasWrongTypeException(myVertexTypeName, propertyDef.Name, propertyDef.BaseType.Name, myValue.GetType().Name);
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
            _queryPlanManager = myMetaManager.QueryPlanManager;
        }

        public virtual void Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
        }

        #endregion
    }
}
