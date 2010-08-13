/*
 * AttributeAssignOrUpdateValue
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.Enums;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Errors;


namespace sones.GraphDB.Managers.Structures
{

    #region AttributeAssignOrUpdateValue

    public class AttributeAssignOrUpdateValue : AAttributeAssignOrUpdate
    {

        #region Properties

        public Object Value { get; private set; }
        public TypesOfOperatorResult AttributeAssignType { get; private set; }

        #endregion

        #region Ctor

        public AttributeAssignOrUpdateValue(IDChainDefinition myIDChainDefinition, TypesOfOperatorResult myAttributeAssignType, Object myValue)
            : base(myIDChainDefinition)
        {
            this.Value = myValue;
            AttributeAssignType = myAttributeAssignType;
        }

        #endregion

        #region override AAttributeAssignOrUpdate.GetValueForAttribute

        public override Exceptional<IObject> GetValueForAttribute(DBObjectStream myDBObject, DBContext myDBContext, GraphDBType myGraphDBType)
        {

            if (AttributeIDChain.IsUndefinedAttribute)
            {
                return new Exceptional<IObject>(GraphDBTypeMapper.GetBaseObjectFromCSharpType(Value));
            }


            #region Simple value

            if (GraphDBTypeMapper.IsAValidAttributeType(AttributeIDChain.LastAttribute.GetDBType(myDBContext.DBTypeManager), AttributeAssignType, myDBContext, Value))
            {
                return new Exceptional<IObject>(GraphDBTypeMapper.GetGraphObjectFromType(AttributeAssignType, Value)); ;
            }
            else
            {
                return new Exceptional<IObject>(new Error_InvalidAttributeValue(AttributeIDChain.LastAttribute.Name, Value));
            }

            #endregion

        }

        #endregion

    }

    #endregion
}
