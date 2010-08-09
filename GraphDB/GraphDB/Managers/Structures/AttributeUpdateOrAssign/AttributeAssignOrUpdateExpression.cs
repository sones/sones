/*
 * AttributeAssignOrUpdateExpression
 * (c) Stefan Licht, 2010
 */


#region Usings

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.Structures
{

    #region AttributeAssignOrUpdateExpression

    public class AttributeAssignOrUpdateExpression : AAttributeAssignOrUpdate
    {

        #region Properties

        public BinaryExpressionDefinition BinaryExpressionDefinition { get; private set; }

        #endregion

        #region Ctor

        public AttributeAssignOrUpdateExpression(IDChainDefinition myIDChainDefinition, BinaryExpressionDefinition binaryExpressionDefinition)
            : base(myIDChainDefinition)
        {
            BinaryExpressionDefinition = binaryExpressionDefinition;
        }

        #endregion

        #region override AAttributeAssignOrUpdate.GetValueForAttribute

        public override Exceptional<AObject> GetValueForAttribute(DBObjectStream aDBObject, DBContext dbContext, GraphDBType _Type)
        {

            #region Expression

            var validateResult = BinaryExpressionDefinition.Validate(dbContext, _Type);
            if (validateResult.Failed)
            {
                return new Exceptional<AObject>(validateResult);
            }

            var value = BinaryExpressionDefinition.SimpleExecution(aDBObject, dbContext);

            #endregion

            return new Exceptional<AObject>(value);

        }
        
        #endregion

    }

    #endregion

}
