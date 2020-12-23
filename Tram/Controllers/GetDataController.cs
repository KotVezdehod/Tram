using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tram.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GetDataController : ControllerBase
    {
        object ret_data;

        [HttpPost]
        public async Task<ActionResult> Post()
        {

            string Name = "";
            string Login = "";
            string Password = "";
            string Data = "";
            string db_path = "";

            Result ret = new CommonControllerProcs().PrepareEnvironmentForRequestHandling(HttpContext, ref Name, ref Login, ref Password, ref Data, ref db_path);
            JsonSerialization js = new JsonSerialization();

            if (!ret.Status)
            {
                ret_data = Problem(js.Serialize(ret), null, ret.HttpCode);
            }
            else
            {
                using (v77Ops v77inst = new v77Ops())
                {
                    ret = v77inst.CreateV77Instance();
                    if (!ret.Status)
                    {
                        ret_data = Problem(js.Serialize(ret), null, 500);
                    }
                    else
                    {
                        ret = v77inst.GetData(Data, Path.Combine(db_path, Name), Login, Password);
                        if (ret.Status)
                        {
                            ret_data = Ok(js.Serialize(ret));
                        }
                        else
                        {
                            ret_data = Problem(js.Serialize(ret), null, 500);
                        }
                    }
                }
            }
            return (ObjectResult)ret_data;
        }
    }
}
