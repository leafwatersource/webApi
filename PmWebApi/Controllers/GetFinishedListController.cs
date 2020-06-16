using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
                return Ok(mUnStart.GetFinishedOrderList(resName));
            }
            else
            {                
                return Ok(-1);
            }

        }
    }
}
