/*
 * AttributeAssignOrUpdateSetRef
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Enums;
using sones.Lib.ErrorHandling;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.EdgeTypes;

using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Managers.Structures
{

    #region AttributeAssignOrUpdateSetRef

    /// <summary>
    /// Assign or update reference values for attributes
    /// </summary>
    public class AttributeAssignOrUpdateSetRef : AAttributeAssignOrUpdate
    {

        #region Properties

        /// <summary>
        /// The reference definition
        /// </summary>
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

        /// <summary>
        /// <seealso cref=" AAttributeAssignOrUpdateOrRemove"/>
        /// </summary>
        public override Exceptional<IObject> GetValueForAttribute(DBObjectStream myDBObject, DBContext myDBContext, GraphDBType myDBType)
        {

            #region reference

            var validationResult = AttributeIDChain.Validate(myDBContext, true, myDBType);
            if (validationResult.Failed())
            {
                return new Exceptional<IObject>(validationResult);
            }

            if (AttributeIDChain.IsUndefinedAttribute)
            {
                return new Exceptional<IObject>(new Error_InvalidReferenceAssignmentOfUndefAttr());
            }

            // if we have a Userdefined Type, than all assignments will work on this type
            if (!AttributeIDChain.LastAttribute.GetDBType(myDBContext.DBTypeManager).IsUserDefined)
            {
                //attributeType = _Type;
            }

            var value = AttributeIDChain.LastAttribute.EdgeType.GetNewInstance();

            var dbos = SetRefDefinition.GetCorrespondigDBObjects(AttributeIDChain.LastAttribute.GetDBType(myDBContext.DBTypeManager), myDBContext, AttributeIDChain.LastAttribute.GetRelatedType(myDBContext.DBTypeManager));

            foreach (var dbo in dbos)
            {
                if (dbo.Failed())
                    return new Exceptional<IObject>(dbo);

                (value as ASingleReferenceEdgeType).Set(dbo.Value.ObjectUUID, AttributeIDChain.LastAttribute.DBTypeUUID, SetRefDefinition.Parameters);
            }

            #endregion

            return new Exceptional<IObject>(value);

        } 

        #endregion

    }

    #endregion

}
