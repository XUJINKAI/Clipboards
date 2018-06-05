using Windows.Foundation.Collections;

namespace MethodWrapper.Extensions
{
    public static class ValueSetExtension
    {
        public const string FuncMethodBase64String = "MethodCall";

        public static ValueSet ToValueSet(this MethodCallInfo methodCall)
        {
            var set = new ValueSet
            {
                { FuncMethodBase64String, methodCall.ToBase64BinaryString() },
            };
            return set;
        }

        public static MethodCallInfo ToMethodCall(this ValueSet set)
        {
            string b64 = set[FuncMethodBase64String] as string;
            var method = BinarySerialization.FromBase64BinaryString<MethodCallInfo>(b64);
            return method;
        }
    }
}
