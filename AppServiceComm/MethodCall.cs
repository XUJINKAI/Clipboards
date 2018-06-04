using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MethodWrapper
{
    [Serializable]
    public class MethodCall
    {
        public string Name { get; set; }
        public List<Object> Args { get; set; }
        public object Result { get; set; }

        public MethodCall()
        {
            Args = new List<object>();
        }

        public object Excute(object obj)
        {
            var method = obj.GetType().GetMethod(Name);
            var invokeresult = method.Invoke(obj, Args.ToArray());
            if(invokeresult is Task)
            {
                Task.Run(async () => { await (Task)invokeresult; });
                return invokeresult.GetType().GetMethod("get_Result").Invoke(invokeresult, null);
            }
            return invokeresult;
        }
    }
}
