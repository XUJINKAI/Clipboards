using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AppServiceComm
{
    public static class ExpressionExtension
    {
        public static object Evaluate(this Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Constant:
                    return ((ConstantExpression)expr).Value;
                case ExpressionType.New:
                    var newexpr = (NewExpression)expr;
                    var arguments = newexpr.Arguments.Select(Evaluate).ToArray();
                    return newexpr.Constructor.Invoke(arguments);
                case ExpressionType.ListInit:
                    var listexpr = (ListInitExpression)expr;
                    var listobj = Evaluate(listexpr.NewExpression);
                    foreach (var element in listexpr.Initializers)
                    {
                        element.AddMethod.Invoke(listobj, element.Arguments.Select(Evaluate).ToArray());
                    }
                    return listobj;
                case ExpressionType.MemberAccess:
                    var me = (MemberExpression)expr;
                    object target = Evaluate(me.Expression);
                    switch (me.Member.MemberType)
                    {
                        case MemberTypes.Field:
                            return ((FieldInfo)me.Member).GetValue(target);
                        case MemberTypes.Property:
                            return ((PropertyInfo)me.Member).GetValue(target, null);
                        default:
                            throw new NotSupportedException(me.Member.MemberType.ToString());
                    }
                default:
                    throw new NotSupportedException(expr.NodeType.ToString());
            }
        }
    }
}
