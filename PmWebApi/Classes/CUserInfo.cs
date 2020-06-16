using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmWebApi.Classes
{
    public class CUserInfo
    {
        public  int EmpID { get; set; }
        public  string UserName { get; set; }
        public  string EmpWorkID { get; set; }
        public  string UserDesc { get; set; }
        public  string UserIpAdress { get; set; }
        public  string PhoneNumber { get; set; }
        public  string Email { get; set; }
        public  string UserGuid { get; set; }
        public  bool IsAdmin { get; set; }
        public  int UserSysID { get; set; }
        public  int UsercusID { get; set; }
        public  string UserShopUserGroupID { get; set; }
        public  string UserSysName { get; set; }
        public string AlertMessage { get; set; }
        public List<string> FunctionList { get; set; }
        public static Dictionary<string, string> LogedUserInfo ;
    }
}
