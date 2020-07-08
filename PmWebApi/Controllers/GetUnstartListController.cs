using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PmWebApi.Models;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;

namespace PmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetUnstartListController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public ActionResult<List<COrderList>> Result([FromForm]string resName)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {                                
                MUnStartList mUnStart = new MUnStartList();
                List<COrderList> cOrderLists = mUnStart.GetUnStartOrderList(resName);
                if(cOrderLists == null)
                {
                    return Ok(-2);
                }
                else
                {
                    return Ok(cOrderLists);
                }
            }
            else
            {               
                return Ok(-1);
            }

        }
    }

}