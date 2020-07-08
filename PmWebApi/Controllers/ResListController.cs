using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PmWebApi.Classes.StaticClasses;
using PmWebApi.Models;

namespace PmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResListController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult ActionResult([FromForm]string usersysid)
        {

            if (GetUserLoginState.LoginState(Request.Headers))
            {
                MResList resList = new MResList();
                return Ok(resList.GetResList(usersysid));
            }
            else
            {               
                return Ok(-1);
            }
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class GetCanChangeResListController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult ActionResult([FromForm]string bean)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                MChangeRes changeRes = new MChangeRes();
                List<string> reslist = changeRes.GetCanResList_Call(bean);
                if(reslist == null)
                {
                    return Ok(-2);
                }
                else
                {
                    if(reslist.Count <1)
                    {
                        return Ok(-2);
                    }
                    else
                    {
                        return Ok(reslist);
                    }
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
    public class ChangeResourceController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult ActionResult([FromForm]string bean)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                MChangeRes changeRes = new MChangeRes();
                return Ok(changeRes.ChangeResource_Call(bean));
            }
            else
            {               
                return Ok(-1);
            }
        }
    }
}