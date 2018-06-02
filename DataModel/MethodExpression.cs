using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataModel
{
    [Serializable]
    public class MethodInformation
    {
        public string Name { get; set; }
        public List<Object> List { get; set; }

        public MethodInformation()
        {
            List = new List<object>();
        }
        
        public static MethodInformation FromLambdaExpression(LambdaExpression expr)
        {
            switch (expr.Body)
            {
                case MethodCallExpression methodcall:
                    return FromMethodCallExpression(methodcall);
                case UnaryExpression unary:
                    switch (unary.Operand)
                    {
                        case MethodCallExpression mc1:
                            return FromMethodCallExpression(mc1);
                        default:
                            throw new NotSupportedException(unary.Operand.GetType().ToString());
                    }
                default:
                    throw new NotSupportedException(expr.Body.GetType().ToString());
            }
        }

        private static MethodInformation FromMethodCallExpression(MethodCallExpression methodCallExpression)
        {
            var result = new MethodInformation
            {
                Name = methodCallExpression.Method.Name,
                List = new List<object>()
            };
            foreach (var exp in methodCallExpression.Arguments)
            {
                result.List.Add(exp.Evaluate());
            }
            return result;
        }

        public static MethodInformation FromBase64(string base64string)
        {
            return BinarySerialization.FromBase64BinaryString<MethodInformation>(base64string);
        }
        
        public object Excute(object obj)
        {
            var call = obj.GetType().GetMethod(Name);
            return call.Invoke(obj, List.ToArray());
        }
        
    }

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
