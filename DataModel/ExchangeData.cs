using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataModel
{
    public enum Request
    {
        None,
        ShowWpfWindow,
        ShutDown,
        AskClipboardList,
        AnsClipboardList,
    }

    public class RequestData : ConnectionDataStruct
    {
        public RequestData()
            : this(Request.None, null)
        {

        }

        public RequestData(Request request)
            : this(request, null)
        {

        }

        public RequestData(Request request, ExChangeDataBase data)
        {
            Request = request;
            if (data != null)
            {
                Data = data;
            }
        }
    }

    public class AnswerData : ConnectionDataStruct
    {

    }

    [XmlInclude(typeof(ExClipboardList))]
    public class ExChangeDataBase { }

    public class ExClipboardList : ExChangeDataBase
    {
        public List<string> Data;

        public ExClipboardList()
        {
            Data = new List<string>();
        }

        public ExClipboardList(List<string> data)
        {
            Data = data;
        }
    }
}
