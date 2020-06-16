using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PmWebApi.Classes;
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
                return Ok(changeRes.GetCanResList_Call(bean));
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