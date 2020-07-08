using PmWebApi.Classes.StaticClasses;

namespace PmWebApi.Models
{
    public class MAppConfig
    {
        public string GetAppVersion(string appguid)
        {
            if(appguid == PmSettings.VersionGuid)
            {
                return PmSettings.AppVersion + "%" + PmSettings.VersionMsg;
            }
            else
            {
                return "-1";
            }
        }
    }
   
}
