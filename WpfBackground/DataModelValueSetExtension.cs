using CommonLibrary;
using System;
using Windows.Foundation.Collections;

namespace DataModel
{
    public static class ValueSetExtension
    {
        public static ValueSet ToValueSet(this RequestData requestData)
        {
            var set = new ValueSet
            {
                { ConnectionDataStruct.C_REQUEST, requestData.Request.ToString() },
                { ConnectionDataStruct.C_DATA, requestData.Data.ToXmlText() }
            };
            return set;
        }

        public static AnswerData ToAnswerData(this ValueSet set)
        {
            if (set.ContainsKey(ConnectionDataStruct.C_REQUEST))
            {
                var req = (Request)Enum.Parse(typeof(Request), set[ConnectionDataStruct.C_REQUEST] as string);
                var dat = XmlSerialization.FromXmlText<ExChangeDataBase>(set[ConnectionDataStruct.C_DATA] as string);
                return new AnswerData
                {
                    Request = req,
                    Data = dat,
                };
            }
            else
            {
                return new AnswerData();
            }
        }
    }
}
