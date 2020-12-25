using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;


namespace Tram
{
    public class v77Ops : IDisposable
    {
        public Type v7_1c_comm_conn_cls_type;
        public object v77_instance;
        public Result IsDatabaseAccesible(string DataBasePath, string Login, string Password)
        {
            Result ret = new Result();

            try
            {
                

                string СтрокаЛогина = Login == "" ? "" : " /n" + "\"" + Login + "\"";
                string СтрокаПароля = Password == "" ? "" : " /p" + "\"" + Password + "\"";
                //string conn = "\"" + Каталог77 + "\"" + " /D" + "\"" + DataBasePath + "\"" + СтрокаЛогина + СтрокаПароля;
                string conn = "/D" + "\"" + DataBasePath + "\"" + СтрокаЛогина + СтрокаПароля;
                dynamic RMTrade = v7_1c_comm_conn_cls_type.InvokeMember("RMTrade", BindingFlags.Public | BindingFlags.GetProperty, null, v77_instance, null);

                if (RMTrade == null)
                {
                    ret.Status = false;
                    ret.Description = "Can't retrieve 'RMTrade' variable from com-server!";
                    return ret;
                }

                object[] init_params = new object[3];
                init_params[0] = RMTrade;
                init_params[1] = conn;
                init_params[2] = "NO_SPLASH_SHOW";

                if (!(bool)v7_1c_comm_conn_cls_type.InvokeMember("Initialize", BindingFlags.Public | BindingFlags.InvokeMethod,
                    null, v77_instance, init_params))
                {
                    ret.Status = false;
                    ret.Description = "Fail to connect to database.";
                    return ret;
                }

                string Batch = "CreateObject(\"Query\")";
                string[] invoke_params = new string[1];
                invoke_params[0] = Batch;

                try
                {

                    bool r = (bool)v7_1c_comm_conn_cls_type.InvokeMember("ExecuteBatch", BindingFlags.Public | BindingFlags.InvokeMethod,
                        null, v77_instance, invoke_params);

                    if (r)
                    {
                        return ret;
                    }
                    else
                    {
                        ret.Status = false;
                        ret.Description = "Fail to invoke 1c 77 method 'CreateObject(\"Query\")' (reason: unknown)";
                        return ret;
                    }

                }
                catch (Exception e)
                {
                    ret.Status = false;
                    if (e.InnerException != null)
                    {
                        ret.Description = "Fail to invoke 1c 77 method 'CreateObject(\"Query\")' (" + e.InnerException.Message + ")";
                    }
                    else
                    {
                        ret.Description = "Fail to invoke 1c 77 method 'CreateObject(\"Query\")' (" + e.Message + ")";
                    }

                    ret.Status = false;
                    return ret;
                }

            }
            catch (Exception e)
            {

                ret.Status = false;
                if (e.InnerException != null)
                {
                    ret.Description = e.InnerException.Message;
                }
                else
                {
                    ret.Description = e.Message;
                }
            }

            return ret;
        }
        public Result ExecuteBatch(string B64Data, string DataBasePath, string Login, string Password)
        {
            byte[] buf;

            try
            {
                buf = Convert.FromBase64String(B64Data);
            }
            catch (Exception e)
            {
                return new Result { Status = false, Description = e.Message };
            }

            string tmp_fn = Path.GetTempFileName();

            using (FileStream fs = new FileStream(tmp_fn, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(buf, 0, buf.Length);
            }

            try
            {
                string Каталог77 = Assembly.GetAssembly(v7_1c_comm_conn_cls_type).Location;
                string СтрокаЛогина = Login == "" ? "" : " /n" + "\"" + Login + "\"";
                string СтрокаПароля = Password == "" ? "" : " /p" + "\"" + Password + "\"";
                string conn = "\"" + Каталог77 + "\"" + " /D" + "\"" + DataBasePath + "\"" + СтрокаЛогина + СтрокаПароля;

                dynamic RMTrade = v7_1c_comm_conn_cls_type.InvokeMember("RMTrade", BindingFlags.Public | BindingFlags.GetProperty, null, v77_instance, null);

                if (RMTrade == null)
                {
                    try
                    {
                        File.Delete(tmp_fn);
                    }
                    catch { };

                    return new Result { Status = false, Description = "Не удалось получить переменную 'RMTrade' из com-сервера 1с 7.7!" };
                }

                object[] init_params = new object[3];
                init_params[0] = RMTrade;
                init_params[1] = conn;
                init_params[2] = "NO_SPLASH_SHOW";

                if (!(bool)v7_1c_comm_conn_cls_type.InvokeMember("Initialize", BindingFlags.Public | BindingFlags.InvokeMethod,
                    null, v77_instance, init_params))
                {
                    try
                    {
                        File.Delete(tmp_fn);
                    }
                    catch { };
                    return new Result { Status = false, Description = "Не удалось подключиться к базе данных." };
                }

                string Batch = "ОткрытьФорму(\"Отчет\"" + ", \"\", \"" + tmp_fn + "\")";

                string[] invoke_params = new string[1];
                invoke_params[0] = Batch;

                try
                {
                    
                    bool r = (bool)v7_1c_comm_conn_cls_type.InvokeMember("ExecuteBatch", BindingFlags.Public | BindingFlags.InvokeMethod,
                    null, v77_instance, invoke_params);

                    try
                    {
                        File.Delete(tmp_fn);
                    }
                    catch
                    {
                    }

                    if (r)
                    {
                        return new Result { Status = true };
                    }
                    else
                    {
                        return new Result { Status = false, Description = "Не удалось вызвать метод 1c 77 'CreateObject(\"Query\")' (это проверка com-соединения) (причина не понятна)" };
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        File.Delete(tmp_fn);
                    }
                    catch { };

                    if (e.InnerException != null)
                    {
                        return new Result { Status = false, Description = "Не удалось вызвать 1c 77 метод 'ОткрытьОтчет' (" + e.InnerException.Message + ")" };
                    }
                    else
                    {
                        return new Result { Status = false, Description = "Не удалось вызвать 1c 77 метод 'ОткрытьОтчет' (" + e.Message + ")" };
                    }

                }

            }

            catch (Exception e)
            {

                try
                {
                    File.Delete(tmp_fn);
                }
                catch
                {
                }

                Result ret = new Result { Status = false };
                if (e.InnerException != null)
                {
                    ret.Description = e.InnerException.Message;
                }
                else
                {
                    ret.Description = e.Message;
                }

                return ret;
            }

            
        }
        public Result GetData(string Code, string DataBasePath, string Login, string Password)
        {
            Dictionary<string, string> fn_to_kill = new Dictionary<string, string>();

            fn_to_kill.Add("fn_empty", Path.GetTempFileName());

            fn_to_kill.Add("fn_CodeListing", Path.Combine(DataBasePath, "CodeListing.txt"));

            fn_to_kill.Add("fn_result", Path.GetTempFileName());

            File.WriteAllBytes(fn_to_kill.GetValueOrDefault("fn_empty"), Properties.Resources.Empty);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (StreamWriter sw = new StreamWriter(fn_to_kill.GetValueOrDefault("fn_CodeListing"), false, Encoding.GetEncoding("windows-1251")))
            {
                sw.WriteLine("Процедура ПриОткрытии()");
                sw.WriteLine("");
                sw.WriteLine("Попытка");
                sw.WriteLine("");
                sw.Write(Code);
                sw.WriteLine("");
                sw.WriteLine(Properties.Resources.FlushCode.ToString());
                sw.WriteLine("");
                sw.WriteLine("КонецПроцедуры");
            }


            try
            {
                string Каталог77 = Assembly.GetAssembly(v7_1c_comm_conn_cls_type).Location;

                string СтрокаЛогина = Login == "" ? "" : " /n" + "\"" + Login + "\"";
                string СтрокаПароля = Password == "" ? "" : " /p" + "\"" + Password + "\"";

                string conn = "\"" + Каталог77 + "\"" + " /D" + "\"" + DataBasePath + "\"" + СтрокаЛогина + СтрокаПароля;

                dynamic RMTrade = v7_1c_comm_conn_cls_type.InvokeMember("RMTrade", BindingFlags.Public | BindingFlags.GetProperty, null, v77_instance, null);

                if (RMTrade == null)
                {
                    KillTmpFiles(fn_to_kill);

                    return new Result { Status = false, Description = "Не удалось получить переменную 'RMTrade' из com-сервера 1с 7.7!" };
                }

                object[] init_params = new object[3];
                init_params[0] = RMTrade;
                init_params[1] = conn;
                init_params[2] = "NO_SPLASH_SHOW";

                if (!(bool)v7_1c_comm_conn_cls_type.InvokeMember("Initialize", BindingFlags.Public | BindingFlags.InvokeMethod,
                    null, v77_instance, init_params))
                {
                    KillTmpFiles(fn_to_kill);
                    return new Result { Status = false, Description = "Не удалось подключиться к базе данных." };
                }

                string Batch = "ОткрытьФорму(\"Отчет\"" + ", \"" + fn_to_kill.GetValueOrDefault("fn_result") + "\", \"" + fn_to_kill.GetValueOrDefault("fn_empty") + "\")";

                string[] invoke_params = new string[1];
                invoke_params[0] = Batch;

                try
                {

                    bool res = (bool)v7_1c_comm_conn_cls_type.InvokeMember("ExecuteBatch", BindingFlags.Public | BindingFlags.InvokeMethod,
                        null, v77_instance, invoke_params);

                    if (res)
                    {
                        if (File.Exists(fn_to_kill.GetValueOrDefault("fn_result")))
                        {
                            bool IsError = false;

                            using (StreamReader sr = new StreamReader(fn_to_kill.GetValueOrDefault("fn_result"), Encoding.GetEncoding("windows-1251")))
                            {
                                if (sr.EndOfStream)
                                {
                                    KillTmpFiles(fn_to_kill);

                                    return new Result { Status = false, Description = "Файл результатов пустой." };

                                }

                                string first_line = sr.ReadLine();

                                if (!first_line.Contains("||||||"))
                                {
                                    IsError = true;

                                }
                            }

                            using (StreamReader sr = new StreamReader(fn_to_kill.GetValueOrDefault("fn_result"), Encoding.GetEncoding("windows-1251")))
                            {

                                Result ret = new Result { Status = !IsError, Description = sr.ReadToEnd() };

                                KillTmpFiles(fn_to_kill);

                                return ret;

                            }

                        }
                        else
                        {
                            KillTmpFiles(fn_to_kill);

                            return new Result { Status = false, Description = "После выполнения кода на стороне 1c 7.7 - файл результатов не обнаружен. Возможно процесс 1с упал." };
                        }

                    }
                    else
                    {
                        KillTmpFiles(fn_to_kill);

                        return new Result { Status = false, Description = "Не удалось выполнить 'ExecuteBatch' " };

                    }

                }
                catch (Exception e)
                {
                    KillTmpFiles(fn_to_kill);

                    return new Result { Status = false, Description = "Не удалось выполнить метод 'ОткрытьОтчет' " };
                }

            }
            catch (Exception e)
            {

                Result ret = new Result { Status = false };
                if (e.InnerException != null)
                {
                    ret.Description = e.InnerException.Message;
                }
                else
                {
                    ret.Description = e.Message;
                }

                return ret;
            }


        }

        public Result CreateV77Instance()
        {

            Result res = new Result();

            try
            {
                v7_1c_comm_conn_cls_type = Type.GetTypeFromProgID("V77.Application");
                if (v7_1c_comm_conn_cls_type == null)
                {
                    res.Status = false;
                    res.Description = "Не удалось получить ком-сервер из идентификатора 'V77.Application'.";
                    return res;
                }
            }
            catch (Exception e)
            {
                res.Status = false;
                if (e.InnerException != null)
                {
                    res.Description = "Не удалось получить ком-сервер из идентификатора 'V77.Application': " + e.InnerException.Message;
                }
                else
                {
                    res.Description = "Не удалось получить ком-сервер из идентификатора 'V77.Application': " + e.Message;
                }
                return res;
            }


            try
            {
                v77_instance = Activator.CreateInstance(v7_1c_comm_conn_cls_type);
                
                if (v77_instance == null)
                {
                    res.Status = false;
                    res.Description = "Не удалось получить экземпляр ком-сервера из идентификатора 'V77.Application'.";
                }
            }
            catch (Exception e)
            {
                res.Status = false;
                if (e.InnerException != null)
                {
                    res.Description = "Не удалось получить экземпляр ком-сервера из идентификатора 'V77.Application': " + e.InnerException.Message;
                }
                else
                {
                    res.Description = "Не удалось получить экземпляр ком-сервера из идентификатора 'V77.Application': " + e.Message;
                }

            }

            return res;
        }

        public void KillTmpFiles(Dictionary<string, string> in_dict)
        {
            foreach (KeyValuePair<string, string> fn in in_dict)
            {
                try
                {
                    if (File.Exists(fn.Value))
                    {
                        File.Delete(fn.Value);
                    }
                    else if (Directory.Exists(fn.Value))
                    {
                        Directory.Delete(fn.Value, true);
                    }
                }
                catch { }
            }

        }

        public void Dispose()
        {
            if (v77_instance != null)
            {
                Marshal.FinalReleaseComObject(v77_instance);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
