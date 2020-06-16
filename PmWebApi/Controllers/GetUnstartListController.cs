using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PmWebApi.Models;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetUnstartListController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public ActionResult<List<COrderList>> Result([FromForm]string resName,[FromForm]string dayShift)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {                                
                MUnStartList mUnStart = new MUnStartList();
                List<COrderList> cOrderLists = mUnStart.GetUnStartOrderList(resName, dayShift);
                string str = JsonConvert.SerializeObject(cOrderLists);
                return Ok(cOrderLists);
            }
            else
            {               
                return Ok(-1);
            }

        }
    }

}