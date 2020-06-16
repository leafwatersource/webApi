using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PmWebApi.Classes.StaticClasses;
using PmWebApi.Models;

namespace PmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownOrderController : ControllerBase
    {
        public IActionResult Result([FromForm]string resName)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                MCanDownThisRes downThisRes = new MCanDownThisRes();
                return Ok(downThisRes.CanDownThisRes_Call(resName));
            }
            else
            {
                JObject jObject = new JObject
                {
                    { "LoginState", "0" },
                    { "ErrMsg", "Please Login At Now" }
                };
                return Ok(jObject);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class BeginDownController : ControllerBase
    {
        public IActionResult Result([FromForm] string resName,[FromForm]string orderUID,[FromForm]string dayshift)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
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