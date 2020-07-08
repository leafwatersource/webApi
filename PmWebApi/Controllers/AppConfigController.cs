using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PmWebApi.Models;

namespace PmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetAppVersionController : ControllerBase
    {
        [EnableCors]
        [HttpPost]
        public IActionResult GetServerVersion([FromForm]string AppGuid) 
        {
            MAppConfig config = new MAppConfig();
            return Ok(config.GetAppVersion(appguid: AppGuid));
        }
    }
}
