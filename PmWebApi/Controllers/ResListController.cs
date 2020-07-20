using System.Collections.Generic;
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
    public class GetDefaultResController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult ActionResult()
        {

            if (GetUserLoginState.LoginState(Request.Headers))
            {
                string empid = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                MResList resList = new MResList();
                string defresname = resList.GetDefaultRes(empid);
                if(string.IsNullOrWhiteSpace(defresname))
                {
                    return Ok(-1);
                }
                else
                {
                    return Ok(defresname);
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
    public class ResListController : ControllerBase
    {
        /// <summary>
        ///  ResEventType:  u：正在使用  s：停机   y:没有占用
        /// </summary>
        /// <param name="usersysid"></param>
        /// <returns></returns>
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
                string UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserAgent = Request.Headers["User-Agent"].ToString();
                string UserEmpID = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                JObject obj = JsonConvert.DeserializeObject<JObject>(bean);
                string changeresname = obj.GetValue("changeResName").ToString();
                string thisresname = obj.GetValue("mesResName").ToString();
                string orderUID = obj.GetValue("orderUID").ToString();
                PublicFunc.WriteUserLog(UserEmpID, UserIP, "订单推送", "orderUID: " + orderUID + ";订单推送:" + thisresname + "=>" + changeresname, UserAgent);
                MChangeRes changeRes = new MChangeRes();
                return Ok(changeRes.ChangeResource_Call(bean));
            }
            else
            {               
                return Ok(-1);
            }
        }
    }

    [Route("api/[controller]")]
    [ApiController]    
    public class SetResUsedController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        [EnableCors]
        [HttpPost]
        public IActionResult ActionResult([FromForm]string resname,[FromForm]string usetype,[FromForm]string starttime,[FromForm]string endtime,[FromForm]string eventmessage)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                string empid = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"].ToString()).GetValue("UserEmpID").ToString();
                MSetResUsed setResUsed = new MSetResUsed();
                setResUsed.SetResUsed(empid, resname, usetype, starttime, endtime, eventmessage);
                return Ok(1);
            }
            else
            {
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class SetResUnusedController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        [EnableCors]
        [HttpPost]
        public IActionResult ActionResult([FromForm] string resname, [FromForm] string usetype)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                MSetResUsed setResUsed = new MSetResUsed();
                setResUsed.SetResUnused(resname,usetype);
                return Ok(1);
            }
            else
            {
                return Ok(-1);
            }
        }
    }
}