using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tram.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IsDataBaseAccessibleController : ControllerBase
    {
        
        [HttpPost]
        public async Task<ActionResult> Post()
        {
        
            List<string> lst_db_folders = new List<string>();

            JsonSerialization js = new JsonSerialization();

            DataTransportStructure[] dts_arr = Common.Get_dts_from_request_body(HttpContext);

            if (dts_arr.Length == 0)
            {
                return BadRequest(js.Serialize(new Result { Status = false, Description = "Input JSON is invalid." }));
            }

            string Name = Common.GetPFromTS("Name", dts_arr);
            string Login = Common.GetPFromTS("Login", dts_arr);
            string Password = Common.GetPFromTS("Password", dts_arr);
                        
            if (Name.Trim() == "")
            {
                return BadRequest(js.Serialize(new Result { Status = false, Description = "No 'Name' member specyfied." }));
            }

    
            string fullPath = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(IsDataBaseAccessibleController)).Location);

            using (StreamReader sr = new StreamReader(Path.Combine(fullPath,"settings.txt")))
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
            string db_path = "";
            
            foreach (string folder in lst_db_folders)
            {
                if (Directory.Exists(Path.Combine(Path.Combine(folder, Name))))
                {

                    if (db_path.Trim() != "")
                    {
                        return Conflict(js.Serialize(new Result { Status = false, Description = "Bad settings file: more then one database in different folders!" }));
                    }

                    db_path = folder;
                }
            }

            using (v77Ops v77inst = new v77Ops())
            {
                v77inst.CreateV77Instance();

                //v77inst.ExecuteBatch(Convert.ToBase64String(b), @"C:\программист\77_test_db", "", "");
                Result ret = v77inst.IsDatabaseAccesible(Path.Combine(db_path, Name), Login, Password);
                return new ObjectResult(js.Serialize(ret));
                
            }

        }

    }

}


