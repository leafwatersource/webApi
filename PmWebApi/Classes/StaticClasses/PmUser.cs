using System.Collections.Generic;

namespace PmWebApi.Classes.StaticClasses
{
    public static class PmUser
    {
        public static int EmpID { get; set; }
        public static string UserName { get; set; }
        public static string UserPass { get; set; }
        public static string EmpWorkID { get; set; }
        public static string UserDesc { get; set; }
        public static string UserIpAdress { get; set; }
        public static string PhoneNumber { get; set; }
        public static string Email { get; set; }
        public static string UserGuid { get; set; }
        public static bool IsAdmin { get; set; }
        public static int UserSysID { get; set; }
        public static int UsercusID { get; set; }
        public static string UserShopUserGroupID { get; set; }
        public static string UserSysName { get; set; }
        public static List<string> FunctionList;
    }
}
