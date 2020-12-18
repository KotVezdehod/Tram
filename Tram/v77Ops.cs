using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;


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

                    return new Result { Status = false, Description = "Can't retrieve 'RMTrade' variable from com-server!" };
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
                    return new Result { Status = false, Description = "Fail to connect to database." };
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
                        return new Result { Status = false, Description = "Fail to invoke 1c 77 method 'CreateObject(\"Query\")' (reason: unknown)" };
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
                        return new Result { Status = false, Description = "Fail to invoke 1c 77 method 'ОткрытьОтчет' (" + e.InnerException.Message + ")" };
                    }
                    else
                    {
                        return new Result { Status = false, Description = "Fail to invoke 1c 77 method 'ОткрытьОтчет' (" + e.Message + ")" };
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
