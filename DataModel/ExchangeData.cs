using System;
using System.Collections.Generic;

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
        public RequestData(string arg1, string arg2)
        {
            Request = (Request)Enum.Parse(typeof(Request), arg1);
            switch (Request)
            {
                case Request.AnsClipboardList:
                    Data = XmlSerialization.FromXmlText<ExClipboardList>(arg2);
                    break;
                default:
                    Data = new ExChangeDataBase();
                    break;
            }
        }
    }

    public class AnswerData : ConnectionDataStruct
    {
        public AnswerData(Request request, ExChangeDataBase data)
        {
            Request = Request;
            Data = data;
        }
    }

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
