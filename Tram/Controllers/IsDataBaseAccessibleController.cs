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
            object ret_data;

            string Name = "";
            string Login = "";
            string Password = "";
            string Data = "";
            string db_path = "";

            logger.RecordEntry("Tram: запрос 'Exec' от " + HttpContext.Connection.RemoteIpAddress.ToString());

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
                    logger.RecordEntry("Tram: пытаюсь создать экземпляр сом-сервера 1с 7.7....");
                    ret = v77inst.CreateV77Instance();
                    if (ret.Status)
                    {
                        ret = v77inst.IsDatabaseAccesible(Path.Combine(db_path, Name), Login, Password);
                        if (ret.Status)
                        {
                            ret_data = Ok(js.Serialize(ret));
                        }
                        else
                        {
                            ret_data = Problem(js.Serialize(ret),null,500);
                            logger.RecordEntry("Tram: " + ret.Description);
                        }
                    }
                    else
                    {
                        ret_data = Problem(js.Serialize(ret), null,500);
                        logger.RecordEntry("Tram: " + ret.Description);
                    }
                }
            }
            return (ObjectResult)ret_data;

        }
    }
}


