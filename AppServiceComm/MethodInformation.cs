using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AppServiceComm
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

        public object Excute(object obj)
        {
            var call = obj.GetType().GetMethod(Name);
            return call.Invoke(obj, List.ToArray());
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
    }
}
