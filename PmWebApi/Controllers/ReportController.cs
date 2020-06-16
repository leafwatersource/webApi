﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                MResume resume = new MResume();
                return Ok(resume.Resume_Call(bean));
            }
            else
            {               
                return Ok(-1);
            }
        }
    }
}