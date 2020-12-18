
namespace Tram
{
    using System;
    using System.Runtime.Serialization.Json;
    using System.Xml.Serialization;
    using System.IO;


    public class JsonSerialization
    {
        public T deserializeJSON<T>(string json)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(json);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));
                    return (T)deserializer.ReadObject(ms);
                }
            }
        }

        public string Serialize(Object Obj)
        {
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(Obj.GetType());
            ser.WriteObject(stream1, Obj);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            return sr.ReadToEnd();

        }
    }

    public class XmlSerialization
    {
        public T deserializeXml<T>(string xml)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(xml);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(ms);
                }
            }
        }

        public string Serialize(Object Obj)
        {
            string str_out = "";
            XmlSerializer formatter = new XmlSerializer(Obj.GetType());

            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, Obj);
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                {
                    str_out = sr.ReadToEnd();
                }
            }
            return str_out;

        }
    }


}
