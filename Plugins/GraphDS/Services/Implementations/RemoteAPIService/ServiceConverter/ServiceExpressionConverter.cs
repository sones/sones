using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests.Expression;
using sones.GraphDB.Expression;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceConverter
{
    public static class ServiceExpressionConverter
    {
        public static IExpression ConvertExpression(ServiceBaseExpression myExpression)
        {
            IExpression ResultExpression = null;

            if (myExpression is ServiceBinaryExpression)
            {
                var Expression = myExpression as ServiceBinaryExpression;
                var Left = ServiceExpressionConverter.ConvertExpression(Expression.Left);
                var Right = ServiceExpressionConverter.ConvertExpression(Expression.Right);
                var Operator = (BinaryOperator)Expression.Operator;
                return new BinaryExpression(Left, Operator, Right);
            }
            else if (myExpression is 
                ServicePropertyExpression)
            {
                var Expression = myExpression as ServicePropertyExpression;
                return new PropertyExpression(Expression.NameOfVertexType, Expression.NameOfProperty, Expression.Edition);
            }
            else if (myExpression is 
                ServiceSingleLiteralExpression)
            {
                var Expression = myExpression as ServiceSingleLiteralExpression;
                return new SingleLiteralExpression((IComparable)Expression.Constant);
            }
            else if (myExpression is
                ServiceCollectionLiteralExpression)
            {
                var Expression = myExpression as ServiceCollectionLiteralExpression;
                return new CollectionLiteralExpression(Expression.CollectionLiteral.Select(
                    x => (IComparable)x).ToList());
            }
            else if (myExpression is
                ServiceRangeLiteralExpression)
            {
                var Expression = myExpression as ServiceRangeLiteralExpression;
                return new RangeLiteralExpression((IComparable)Expression.Lower, (IComparable)Expression.Upper,
                    Expression.IncludeBorders);
            }
            else if (myExpression is ServiceUnaryExpression)
            {
                var Expression = myExpression as ServiceUnaryExpression;
                return new UnaryExpression((UnaryLogicOperator)Expression.Operator,
                    ServiceExpressionConverter.ConvertExpression(Expression.Expression));
            }

            return ResultExpression;
        }
    }
}
