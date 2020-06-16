using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PmWebApi.Models;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using Microsoft.AspNetCore.Cors;
using System.Security.Cryptography;
using System.Text;

namespace PmWebApi.Controllers
{
    /// <summary>
    /// 登入接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {        
        [EnableCors]
        [HttpPost]
        public ActionResult<CLogin> Result([FromForm]string username,[FromForm]string userpass)
        {
            Mlogin mlogin = new Mlogin
            {
                //这个信息只能在controller里获取,model里不能获取
                UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString()
            };
            MD5 md5 = MD5.Create();
            //PMStaticModels.UserModels.PMUser.UserSysID
            userpass += username;
            string userPass = "";
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(userpass.Trim()));
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                userPass += s[i].ToString("X");
            }
            CLogin cLogin = mlogin.Login(username, userPass);
            if(cLogin.LoginState == 1)
            {
                Response.Cookies.Append("EmpID", PmUser.EmpID.ToString(), new CookieOptions() { IsEssential = true });
                Response.Cookies.Append("UserGuid", PmUser.UserGuid, new CookieOptions() { IsEssential = true });
                if(PmUser.IsAdmin == true)
                {
                    Response.Cookies.Append("MD5", PublicFunc.GetMd5("ADMIN" + Guid.NewGuid().ToString()), new CookieOptions() { IsEssential = true });
                }
            }
            return cLogin;
        }
    }
    /// <summary>
    /// 强制登出接口,包含强制登出后登录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ForceOut : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public CLogin Result([FromForm]string username, [FromForm]string userpass)
        {
            Mlogin mlogin = new Mlogin
            {
                //这个信息只能在controller里获取,model里不能获取
                UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString()
            };
            MD5 md5 = MD5.Create();
            //PMStaticModels.UserModels.PMUser.UserSysID
            userpass += username;
            string userPass = "";
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(userpass.Trim()));
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                userPass += s[i].ToString("X");
            }
            CLogin cLogin = mlogin.ForceOut(username, userPass);
            if (cLogin.LoginState == 1)
            {
                Response.Cookies.Append("EmpID", PmUser.EmpID.ToString(), new CookieOptions() { IsEssential = true });
                Response.Cookies.Append("UserGuid", PmUser.UserGuid, new CookieOptions() { IsEssential = true });
                if (PmUser.IsAdmin == true)
                {
                    Response.Cookies.Append("MD5", PublicFunc.GetMd5("ADMIN" + Guid.NewGuid().ToString()), new CookieOptions() { IsEssential = true });
                }
            }
            return cLogin;
        }
    }
    /// <summary>
    /// 获取登陆后的用户信息
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GetUserInfo : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public ActionResult<CUserInfo> Result([FromForm]string empid,[FromForm]string userguid)
        {
            MGetUserInfo mGetUser = new MGetUserInfo();
            return mGetUser.GetInformation(userguid, empid);
        }
    }
}