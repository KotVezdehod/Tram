using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
                catch (Exception e)
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

    public class CommonControllerProcs
    {
        public Result PrepareEnvironmentForRequestHandling(HttpContext in_http, ref string Name, ref string Login, ref string Password, ref string Data, ref string db_path)
        {
            Result ret = new Result();

            List<string> lst_db_folders = new List<string>();
            DataTransportStructure[] dts_arr = Common.Get_dts_from_request_body(in_http);

            Name = Common.GetPFromTS("Name", dts_arr);
            Login = Common.GetPFromTS("Login", dts_arr);
            Password = Common.GetPFromTS("Password", dts_arr);
            Data = Common.GetPFromTS("Data", dts_arr);

            if (dts_arr.Length == 0)
            {

                ret.Status = false;
                ret.Description = "Входной пакет JSON ошибочен.";
                ret.HttpCode = 400;
            }
            else
            {

                if (Name.Trim() == "")
                {
                    ret.Status = false;
                    ret.Description = "Не определен член 'Name' структуры запроса.";
                    ret.HttpCode = 400;

                }
                else
                {
                    string fullPath = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(CommonControllerProcs)).Location);
                    if (!File.Exists(Path.Combine(fullPath, "settings.txt")))
                    {
                        ret.Status = false;
                        ret.Description = "В каталоге программы не обнаружен файл настроек 'Settings.txt'.";
                        ret.HttpCode = 500;
                    }
                    else
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        using (StreamReader sr = new StreamReader(Path.Combine(fullPath, "settings.txt"),true))
                        {
                            string SettingsLine = "";
                            List<string> lst_setting = new List<string>();
                            string curr_section = "";

                            while (!sr.EndOfStream)
                            {
                                SettingsLine = sr.ReadLine();
                                switch (SettingsLine)
                                {
                                    case "[folders]":

                                        curr_section = SettingsLine;
                                        break;
                                    default:

                                        switch (curr_section)
                                        {
                                            case "[folders]":

                                                lst_db_folders.Add(SettingsLine);

                                                break;
                                            default:

                                                break;
                                        }
                                        break;
                                }
                            }
                        }

                        //try to lock database in folders
                        db_path = "";

                        foreach (string folder in lst_db_folders)
                        {
                            if (Directory.Exists(Path.Combine(Path.Combine(folder, Name))))
                            {
                                if (db_path.Trim() != "")
                                {
                                    ret.Status = false;
                                    ret.Description = "Файл настроек с ошибками. Обнаружено 2 базы данных с одним именем.";
                                    ret.HttpCode = 500;
                                    break;
                                }
                                db_path = folder;
                            }
                        }

                        if (db_path.Trim() == "")
                        {
                            ret.Status = false;
                            ret.Description = "В файле настроек не указан каталог баз данных, либо база данных по указанному пути не существует!";
                            ret.HttpCode = 500;
                        }
                    }

                }
            }

            return ret;
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
        public int HttpCode = 200;
    }


}
