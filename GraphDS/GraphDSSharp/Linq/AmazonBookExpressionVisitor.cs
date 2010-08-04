using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using sones.GraphDS.API.CSharp.Reflection;

namespace sones.GraphDS.API.CSharp.Linq
{
    public class AmazonBookExpressionVisitor
    {

            //AmazonBookQueryCriteria _Criteria;

        //http://github.com/samus/mongodb-csharp/tree/master/source/MongoDB/Linq/

        //    public AmazonBookQueryCriteria ProcessExpression(Expression expression)
            public Object ProcessExpression(Expression expression)
            {
                //_Criteria = new AmazonBookQueryCriteria();
                VisitExpression(expression);
                //return _Criteria;
                return null;
            }

            private void VisitExpression(Expression expression)
            {

                if (expression.NodeType == ExpressionType.AndAlso)
                {
                    VisitAndAlso((BinaryExpression)expression);
                }

                else if (expression.NodeType == ExpressionType.Equal)
                {
                    VisitEqual((BinaryExpression)expression);
                }

                else if (expression.NodeType == ExpressionType.LessThanOrEqual)
                {
                    VisitLessThanOrEqual((BinaryExpression)expression);
                }

                else if (expression is MethodCallExpression)
                {
                    VisitMethodCall((MethodCallExpression)expression);
                }

                else if (expression is LambdaExpression)
                {
                    VisitExpression(((LambdaExpression)expression).Body);
                }

            }

            private void VisitAndAlso(BinaryExpression andAlso)
            {
                VisitExpression(andAlso.Left);
                VisitExpression(andAlso.Right);
            }

            private void VisitEqual(BinaryExpression expression)
            {

                // Handle book.Publisher == "xxx" 
                if ((expression.Left.NodeType == ExpressionType.MemberAccess) &&
                  (((MemberExpression)expression.Left).Member.Name == "Publisher"))
                {
                    //if (expression.Right.NodeType == ExpressionType.Constant)
                        //_Criteria.Publisher = (String)((ConstantExpression)expression.Right).Value;
                    //else if (expression.Right.NodeType == ExpressionType.MemberAccess)
                        //_Criteria.Publisher = (String)GetMemberValue((MemberExpression)expression.Right);
                    //else
                    //    throw new NotSupportedException("Expression type not supported for publisher: " + expression.Right.NodeType.ToString());
                }

                // Handle book.Condition == BookCondition.* 
                //else if ((expression.Left is UnaryExpression) &&
                //  (((UnaryExpression)expression.Left).Operand.Type == typeof(BookCondition)))
                //{
                    //if (expression.Right.NodeType == ExpressionType.Constant)
                    //    _Criteria.Condition = (BookCondition)((ConstantExpression)expression.Right).Value;
                    //else if (expression.Right.NodeType == ExpressionType.MemberAccess)
                    //    _Criteria.Condition = (BookCondition)GetMemberValue((MemberExpression)expression.Right);
                    //else
                    //    throw new NotSupportedException("Expression type not supported for book condition: " + expression.Right.NodeType.ToString());
                //}

            }

            private void VisitLessThanOrEqual(BinaryExpression expression)
            {
                // Handle book.Price <= xxx 
                if ((expression.Left.NodeType == ExpressionType.MemberAccess) &&
                  (((MemberExpression)expression.Left).Member.Name == "Price"))
                {
                    //if (expression.Right.NodeType == ExpressionType.Constant)
                    //    _Criteria.MaximumPrice = (Decimal)((ConstantExpression)expression.Right).Value;
                    //else if (expression.Right.NodeType == ExpressionType.MemberAccess)
                    //    _Criteria.MaximumPrice = (Decimal)GetMemberValue((MemberExpression)expression.Right);
                    //else
                    //    throw new NotSupportedException("Expression type not supported for price: " + expression.Right.NodeType.ToString());
                }
            }

            private void VisitMethodCall(MethodCallExpression expression)
            {

                if ((expression.Method.DeclaringType == typeof(Queryable)) &&
                  (expression.Method.Name == "Where"))
                {
                    //this.Visit(m.Arguments[0]); 
                    //LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]); 
                    //this.Visit(lambda.Body); 
                    VisitExpression(((UnaryExpression)expression.Arguments[1]).Operand);
                }

                else if ((expression.Method.DeclaringType == typeof(String)) &&
                  (expression.Method.Name == "Contains"))
                {
                    // Handle book.Title.Contains("xxx") 
                    if (expression.Object.NodeType == ExpressionType.MemberAccess)
                    {
                        MemberExpression memberExpr = (MemberExpression)expression.Object;
                        if (memberExpr.Expression.Type == typeof(DBVertex))
                        {
                            if (memberExpr.Member.Name == "Title")
                            {
                                Expression argument;

                                argument = expression.Arguments[0];
                                if (argument.NodeType == ExpressionType.Constant)
                                {
                                    //_Criteria.Title = (String)((ConstantExpression)argument).Value;
                                }
                                else if (argument.NodeType == ExpressionType.MemberAccess)
                                {
                                    //_Criteria.Title = (String)GetMemberValue((MemberExpression)argument);
                                }
                                else
                                {
                                    throw new NotSupportedException("Expression type not supported: " + argument.NodeType.ToString());
                                }
                            }
                        }
                    }
                }

                else
                {
                    throw new NotSupportedException("Method not supported: " + expression.Method.Name);
                }

            }

            #region Helpers

            private Object GetMemberValue(MemberExpression memberExpression)
            {
                MemberInfo memberInfo;
                Object obj;

                if (memberExpression == null)
                    throw new ArgumentNullException("memberExpression");

                // Get object 
                if (memberExpression.Expression is ConstantExpression)
                    obj = ((ConstantExpression)memberExpression.Expression).Value;
                else if (memberExpression.Expression is MemberExpression)
                    obj = GetMemberValue((MemberExpression)memberExpression.Expression);
                else
                    throw new NotSupportedException("Expression type not supported: " + memberExpression.Expression.GetType().FullName);

                // Get value 
                memberInfo = memberExpression.Member;
                if (memberInfo is PropertyInfo)
                {
                    PropertyInfo property = (PropertyInfo)memberInfo;
                    return property.GetValue(obj, null);
                }
                else if (memberInfo is FieldInfo)
                {
                    FieldInfo field = (FieldInfo)memberInfo;
                    return field.GetValue(obj);
                }
                else
                {
                    throw new NotSupportedException("MemberInfo type not supported: " + memberInfo.GetType().FullName);
                }
            }

            #endregion Helpers
        } 


}
