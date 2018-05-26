using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class ConnectionDataStruct
    {
        public Request Request { get; protected set; }
        public ExChangeDataBase Data { get; protected set; }

        internal ConnectionDataStruct()
        {

        }
    }
    
    public class ExChangeDataBase { }

}
