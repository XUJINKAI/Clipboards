using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MethodWrapper
{
    public interface IInvokerProxy
    {
        Task<object> InvokeAsync(MethodInfo targetMethod, object[] args);
    }
}
