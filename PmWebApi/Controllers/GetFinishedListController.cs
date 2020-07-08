using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PmWebApi.Classes.StaticClasses;
using PmWebApi.Models;

namespace PmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetFinishedListController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult Result([FromForm] string resName)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                MFinishedList mUnStart = new MFinishedList();
                if(mUnStart.GetFinishedOrderList(resName) != null)
                {
                    return Ok(mUnStart.GetFinishedOrderList(resName));
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
}
