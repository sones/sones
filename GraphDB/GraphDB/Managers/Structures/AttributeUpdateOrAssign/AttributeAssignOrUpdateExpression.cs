/*
 * AttributeAssignOrUpdateExpression
 * (c) Stefan Licht, 2010
 */


#region Usings

using sones.GraphDB.ObjectManagement;

using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Managers.Structures
{

    #region AttributeAssignOrUpdateExpression

    /// <summary>
    /// A class that contains the BinaryExpressionDefinition of an attribute with his values
    /// </summary>
    public class AttributeAssignOrUpdateExpression : AAttributeAssignOrUpdate
    {

        #region Properties

        /// <summary>
        /// The BinaryExpressionDefinition of an attribute
        /// </summary>
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

        /// <summary>
        /// Return the value of an attribute
        /// <seealso cref=" AAttributeAssignOrUpdateOrRemove"/>
        /// </summary>        
        public override Exceptional<IObject> GetValueForAttribute(DBObjectStream myDBObject, DBContext dbContext, GraphDBType myGraphDBType)
        {

            #region Expression

            var validateResult = BinaryExpressionDefinition.Validate(dbContext, myGraphDBType);
            if (validateResult.Failed())
            {
                return new Exceptional<IObject>(validateResult);
            }

            var value = BinaryExpressionDefinition.SimpleExecution(myDBObject, dbContext);

            #endregion

            return new Exceptional<IObject>(value);

        }
        
        #endregion

    }

    #endregion

}
