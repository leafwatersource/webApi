using System.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<DataTable> ActionResult([FromForm] string empid)
        {

            if (GetUserLoginState.LoginState(Request.Headers))
            {
                User user = new User();
                return user.GetUserLog(empid, "2019/08/10 00:00:00", "2020/05/25 00:00:00");
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