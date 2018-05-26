using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DataModel
{
    public class ConnectionDataStruct
    {
        public const string C_REQUEST = "Request";
        public const string C_DATA = "Data";

        public Request Request { get; set; }
        public ExChangeDataBase Data { get; set; } = new ExChangeDataBase();

        internal ConnectionDataStruct()
        {

        }

        public override string ToString()
        {
            return $"Request: {Request}";
        }
    }

}
