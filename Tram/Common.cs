using Microsoft.AspNetCore.Http;
using System;
using System.IO;


namespace Tram
{
    
    public static class Common
    {
        public static DataTransportStructure[] Get_dts_from_request_body(HttpContext cont)
        {
            using (StreamReader sr = new StreamReader(cont.Request.Body))
            {
                try
                {
                    
                    return new JsonSerialization().deserializeJSON<DataTransportStructure[]>(sr.ReadToEnd());
                    
                }
                catch(Exception e)
                {
                    return new DataTransportStructure[0];
                }
            }
        }

        public static string GetPFromTS(string Name, DataTransportStructure[] dts_arr)
        {
            foreach (DataTransportStructure dts in dts_arr)
            {
                if (dts.p_name == Name)
                {
                    return dts.p_value;
                }
            }

            return "";
        }
    }

    [Serializable]
    public class DataTransportStructure
    {
        public string p_name = "";
        public string p_value = "";
    }

    [Serializable]
    public class Result
    {
        public bool Status = true;
        public string Description = "";
    }
}
