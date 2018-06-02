using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Windows.Foundation.Collections;

namespace DataModel
{
    public static class ValueSetExtension
    {
        public const string FuncMethodBase64String = "Func";
        public const string ReturnObjectString = "Ret";

        public static ValueSet SetLambda(LambdaExpression lambdaExpression)
        {
            var method = MethodInformation.FromLambdaExpression(lambdaExpression);
            var set = new ValueSet
            {
                { FuncMethodBase64String, method.ToBase64BinaryString() },
            };
            return set;
        }

        public static MethodInformation GetMethod(ValueSet set)
        {
            string b64 = set[FuncMethodBase64String] as string;
            var method = MethodInformation.FromBase64(b64);
            return method;
        }

        public static ValueSet SetResponse(object obj)
        {
            var set = new ValueSet
            {
                { ReturnObjectString, obj.ToBase64BinaryString() },
            };
            return set;
        }

        public static object GetResponse(ValueSet set)
        {
            string ret = set[ReturnObjectString] as string;
            return BinarySerialization.FromBase64BinaryString<object>(ret);
        }
    }
}
