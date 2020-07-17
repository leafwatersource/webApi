using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PmWebApi.Classes.StaticClasses;
using PmWebApi.Models;

namespace PmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpFinishController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult Result([FromForm]string bean)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                MOpFinish opFinish = new MOpFinish();
                return Ok(opFinish.GetOpFinishedData(bean));
            }
            else
            {
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class OpFinishHistoryController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult Result([FromForm] string orderuid)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                MOpFinish opFinish = new MOpFinish();
                return Ok(opFinish.GetOpFinishHistory(orderuid));
            }
            else
            {
                return Ok(-1);
            }
        }
    }
}
