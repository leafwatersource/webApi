using System.Data;
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
    public class UserLog : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public ActionResult<DataTable> ActionResult([FromForm] string empid,[FromForm] int logtype)
        {

            if (GetUserLoginState.LoginState(Request.Headers))
            {
                User user = new User();
                return user.GetUserLog(empid, logtype);
            }
            else
            {
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class UserOperating : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public ActionResult<DataTable> ActionResult([FromForm] string empName)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                User user = new User();
                return user.Operate(empName);
            }
            else
            {
                return Ok(-1);
            }
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ChangeUserInfo : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult Result([FromForm]string userinfo)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                User user = new User();
                return Ok(user.UpdateUserinfo(userinfo));
            }
            else
            {
                return Ok(-1);
            }
        }
    }
    /// <summary>
    /// 返回值为true表示成功退出
    /// 返回值为false表示退出失败
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserSignOut : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult Result([FromForm] string empid)
        {
            //退出登录

            if (GetUserLoginState.LoginState(Request.Headers))
            {
                string UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserAgent = Request.Headers["User-Agent"].ToString();
                string UserEmpID = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                PublicFunc.WriteUserLog(UserEmpID, UserIP, "退出登陆", "退出登陆", UserAgent);
                User user = new User();
                return Ok(user.SignOut(empid));
            }
            else
            {
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ChangePass : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public ActionResult Action([FromForm]string empid ,[FromForm]string oldPass, [FromForm]string newPass)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                string UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserAgent = Request.Headers["User-Agent"].ToString();
                string UserEmpID = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                PublicFunc.WriteUserLog(UserEmpID, UserIP, "更改密码", "更改密码为:" + newPass, UserAgent);
                User user = new User();
                return Ok(user.ChangePass(empid,oldPass, newPass));
            }
            else
            {
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class UserHaveLogin : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public ActionResult Action([FromForm]string username,[FromForm]string userGuid)
        {
            User user = new User();
            return Ok(user.HasLogin(username,userGuid));
        }
    }
}