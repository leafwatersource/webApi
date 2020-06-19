using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    public class UserLog : ControllerBase
    {
        /// <summary>
        /// 查看用户的登录日志
        /// </summary>
        /// <param name="userUUID"></param>
        /// <param name="empid"></param>
        /// <returns></returns>
        [EnableCors]
        [HttpPost]
        public ActionResult<DataTable> ActionResult([FromForm]string userUUID, [FromForm] string empid)
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
        /// <summary>
        /// 用户操作记录
        /// </summary>
        /// <param name="empName">操作人的名称</param>
        /// <returns>json</returns>
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
    public class UserSignOut : ControllerBase
    {
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="empid"></param>
        /// <returns>bool</returns>
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
        /// <summary>
        /// 用户修改面
        /// </summary>
        /// <param name="oldPass">旧密码</param>
        /// <param name="newPass">新密码</param>
        /// <returns>bool表示修改成功或失败,-1是没有登录</returns>
        [EnableCors]
        [HttpPost]
        public ActionResult Action([FromForm]string oldPass, [FromForm]string newPass)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                User user = new User();
                return Ok(user.ChangePass(oldPass, newPass));
            }
            else
            {
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeUserMessage : ControllerBase
    {
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="userobj">{empName:"",dept:"",phoneNum:"",email:""}</param>
        /// <returns>bool</returns>
        [EnableCors]
        [HttpPost]
        public ActionResult Action([FromForm]string userobj)
        {
            //用户修改信息
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                User user = new User();
                return Ok(user.UserMessage(userobj));
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
        /// <summary>
        /// 判断用户是否被强制登陆
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        [EnableCors]
        [HttpPost]
        public ActionResult Action([FromForm]string username, [FromForm]string userGuid)
        {
            User user = new User();
            return Ok(user.HasLogin(username,userGuid));
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeAllPass : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public ActionResult Action()
        {
            User user = new User();
            return Ok(user.updateAllUser());
        }
    }
}