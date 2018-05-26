using System;
using System.IO;
using System.Xml.Serialization;

namespace DataModel
{
    public static class XmlSerialization
    {
        public static string ToXmlText<T>(this T o) where T : new()
        {
            var serializer = new XmlSerializer(typeof(T));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, o);
                return writer.ToString();
            }
        }

        public static T FromXmlText<T>(string xml, bool FailCreateNew = false) where T : ExChangeDataBase, new()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (StringReader reader = new StringReader(xml))
                {
                    T obj = (T)serializer.Deserialize(reader);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                if (FailCreateNew)
                {
                    return new T();
                }
                throw ex;
            }
        }
    }
}
