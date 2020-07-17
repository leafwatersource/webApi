using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using PmWebApi.Models;
using System.Collections.Generic;

namespace PmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownOrderController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult Result([FromForm] string resName,[FromForm] int dayshift)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                MCanDownThisRes downThisRes = new MCanDownThisRes();
                List<COrderList> cOrder = downThisRes.CanDownThisRes_Call(resName,dayshift);
                if (cOrder != null)
                {
                    return Ok(cOrder);
                }
                else
                {
                    return Ok(-2);
                }
            }
            else
            {               
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class BeginDownController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult Result([FromForm] string resName,[FromForm]string orderUID,[FromForm]string dayshift)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                string UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserAgent = Request.Headers["User-Agent"].ToString();
                string UserEmpID = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                PublicFunc.WriteUserLog(UserEmpID, UserIP, "拉取订单", "拉取订单:" + orderUID + ",到设备:" + resName , UserAgent);
                MCanDownThisRes downThisRes = new MCanDownThisRes();
                return Ok(downThisRes.BeginDown_Call(orderuid: orderUID, resName: resName, dayshift:dayshift));
            }
            else
            {               
                return Ok(-1);
            }
        }
    }
}