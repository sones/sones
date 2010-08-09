/*
 * AttributeAssignOrUpdateSetRef
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.EdgeTypes;

using sones.GraphDB.Errors;

namespace sones.GraphDB.Managers.Structures
{

    #region AttributeAssignOrUpdateSetRef

    public class AttributeAssignOrUpdateSetRef : AAttributeAssignOrUpdate
    {

        #region Properties

        public SetRefDefinition SetRefDefinition { get; private set; }

        #endregion

        #region Ctor

        public AttributeAssignOrUpdateSetRef(IDChainDefinition myIDChainDefinition, SetRefDefinition mySetRefDefinition)
            : base(myIDChainDefinition)
        {
            SetRefDefinition = mySetRefDefinition;
        }

        #endregion

        #region override ToString

        public override string ToString()
        {
            return "SetRefNode";
        }
        
        #endregion

        #region override AAttributeAssignOrUpdate.GetValueForAttribute

        public override Exceptional<AObject> GetValueForAttribute(DBObjectStream aDBObject, DBContext dbContext, GraphDBType _Type)
        {

            #region reference

            var validationResult = AttributeIDChain.Validate(dbContext, true, _Type);
            if (validationResult.Failed)
            {
                return new Exceptional<AObject>(validationResult);
            }

            if (AttributeIDChain.IsUndefinedAttribute)
            {
                return new Exceptional<AObject>(new Error_InvalidReferenceAssignmentOfUndefAttr());
            }

            // if we have a Userdefined Type, than all assignments will work on this type
            if (!AttributeIDChain.LastAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
            {
                //attributeType = _Type;
            }

            var value = AttributeIDChain.LastAttribute.EdgeType.GetNewInstance();

            var dbos = SetRefDefinition.GetCorrespondigDBObjects(AttributeIDChain.LastAttribute.GetDBType(dbContext.DBTypeManager), dbContext, AttributeIDChain.LastAttribute.GetRelatedType(dbContext.DBTypeManager));

            foreach (var dbo in dbos)
            {
                if (dbo.Failed)
                    return new Exceptional<AObject>(dbo);

                (value as ASingleReferenceEdgeType).Set(dbo.Value.ObjectUUID, AttributeIDChain.LastAttribute.DBTypeUUID, SetRefDefinition.Parameters);
            }

            #endregion

            return new Exceptional<AObject>(value);

        } 

        #endregion

    }

    #endregion

}
