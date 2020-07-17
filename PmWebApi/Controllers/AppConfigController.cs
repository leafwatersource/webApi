using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PmWebApi.Classes.StaticClasses;
using PmWebApi.Models;

namespace PmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetAppVersionController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult GetServerVersion([FromForm]string AppGuid) 
        {
            MAppConfig config = new MAppConfig();
            return Ok(config.GetAppVersion(appguid: AppGuid));
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class WriteLogController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult Result([FromForm] string logmodel,[FromForm] string logmessage)
        {
            if(GetUserLoginState.LoginState(Request.Headers))
            {
                string UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserAgent = Request.Headers["User-Agent"].ToString();
                string UserEmpID = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                PublicFunc.WriteUserLog(UserEmpID, UserIP, logmodel, logmessage, UserAgent);
                return Ok(1);
            }
            else
            {
                return Ok(-1);
            }
        }
    }
}
