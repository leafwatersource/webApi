using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PmWebApi.Classes.StaticClasses;
using PmWebApi.Models;

namespace PmWebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BeginChangeController : ControllerBase
    {
        public IActionResult Result([FromForm] string bean)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                string UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserAgent = Request.Headers["User-Agent"].ToString();
                string UserEmpID = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                string OrderUID = JsonConvert.DeserializeObject<JObject>(bean).GetValue("orderUID").ToString();
                string Opname = JsonConvert.DeserializeObject<JObject>(bean).GetValue("pmOpName").ToString();
                PublicFunc.WriteUserLog(UserEmpID, UserIP, "开始切换", "OrderUID:" + OrderUID + ",工序名称:" + Opname, UserAgent);
                MBeginChange beginChange = new MBeginChange();
                return Ok(beginChange.BeginChange_Call(bean));
            }
            else
            {
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class EndChangeController : ControllerBase
    {
        public IActionResult Result([FromForm] string bean)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                string UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserAgent = Request.Headers["User-Agent"].ToString();
                string UserEmpID = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                string OrderUID = JsonConvert.DeserializeObject<JObject>(bean).GetValue("orderUID").ToString();
                string Opname = JsonConvert.DeserializeObject<JObject>(bean).GetValue("pmOpName").ToString();
                PublicFunc.WriteUserLog(UserEmpID, UserIP, "结束切换", "OrderUID:" + OrderUID + ",工序名称:" + Opname, UserAgent);
                MEndChange endChange = new MEndChange();
                return Ok(endChange.EndChange_Call(bean));
            }
            else
            {               
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        public IActionResult Result([FromForm] string bean)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                string UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserAgent = Request.Headers["User-Agent"].ToString();
                string UserEmpID = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                string OrderUID = JsonConvert.DeserializeObject<JObject>(bean).GetValue("orderUID").ToString();
                string Opname = JsonConvert.DeserializeObject<JObject>(bean).GetValue("pmOpName").ToString();
                PublicFunc.WriteUserLog(UserEmpID, UserIP, "报工", "OrderUID:" + OrderUID + ",工序名称:" + Opname, UserAgent);
                MReport report = new MReport();
                return Ok(report.Report_Call(bean));
            }
            else
            {                 
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class PauseOrderController : ControllerBase
    {
        public IActionResult Result([FromForm] string bean)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                string UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserAgent = Request.Headers["User-Agent"].ToString();
                string UserEmpID = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                string OrderUID = JsonConvert.DeserializeObject<JObject>(bean).GetValue("orderUID").ToString();
                string Opname = JsonConvert.DeserializeObject<JObject>(bean).GetValue("pmOpName").ToString();
                PublicFunc.WriteUserLog(UserEmpID, UserIP, "订单暂停", "OrderUID:" + OrderUID + ",工序名称:" + Opname, UserAgent);
                MPauseOrder pauseOrder = new MPauseOrder();
                return Ok(pauseOrder.PauseOrder_Call(bean));
            }
            else
            {                
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeOrderController : ControllerBase
    {
        public IActionResult Result([FromForm] string bean)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                string UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserAgent = Request.Headers["User-Agent"].ToString();
                string UserEmpID = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                string OrderUID = JsonConvert.DeserializeObject<JObject>(bean).GetValue("orderUID").ToString();
                string Opname = JsonConvert.DeserializeObject<JObject>(bean).GetValue("pmOpName").ToString();
                PublicFunc.WriteUserLog(UserEmpID, UserIP, "恢复生产", "OrderUID:" + OrderUID + ",工序名称:" + Opname, UserAgent);
                MResume resume = new MResume();
                return Ok(resume.Resume_Call(bean));
            }
            else
            {               
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class EndWorkingController : ControllerBase
    {
        public IActionResult Result([FromForm] string bean)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                string UserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string UserAgent = Request.Headers["User-Agent"].ToString();
                string UserEmpID = JsonConvert.DeserializeObject<JObject>(Request.Headers["token"]).GetValue("UserEmpID").ToString();
                PublicFunc.WriteUserLog(UserEmpID, UserIP, "换班", "换班", UserAgent);
                MEndWork mEnd = new MEndWork();
                bool result = mEnd.EndWork(bean);
                return Ok(result);
            }
            else
            {
                return Ok(-1);
            }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class OrderAjustmentController : ControllerBase
    {
        public IActionResult Result([FromForm] string bean)
        {
            if (GetUserLoginState.LoginState(Request.Headers))
            {
                MOrderAjustment ajustment = new MOrderAjustment();                
                return Ok(ajustment.OrderAjustmest_Call(bean));
            }
            else
            {
                return Ok(-1);
            }
        }
    }
}