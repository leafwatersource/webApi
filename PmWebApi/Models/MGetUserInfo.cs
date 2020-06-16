using System.Data.SqlClient;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;

namespace PmWebApi.Models
{
    public class MGetUserInfo
    {        
        public  CUserInfo GetInformation(string localGuid,string userEmpid)
        {
            CUserInfo cUser = new CUserInfo();
            SqlCommand cmd = PmConnections.CtrlCmd();
            cmd.CommandText = "SELECT userGUID FROM wapUserstate WHERE empID = '" + userEmpid + "'";
            SqlDataReader rd = cmd.ExecuteReader();
            if(rd.Read())
            {
                string serverUserguid = rd[0].ToString();
                if(serverUserguid == localGuid)
                {
                    cUser.Email = PmUser.Email;
                    cUser.EmpID = PmUser.EmpID;
                    cUser.EmpWorkID = PmUser.EmpWorkID;
                    cUser.FunctionList = PmUser.FunctionList;
                    cUser.IsAdmin = PmUser.IsAdmin;
                    cUser.PhoneNumber = PmUser.PhoneNumber;
                    cUser.UsercusID = PmUser.UsercusID;
                    cUser.UserDesc = PmUser.UserDesc;
                    cUser.UserGuid = PmUser.UserGuid;
                    cUser.UserIpAdress = PmUser.UserIpAdress;
                    cUser.UserName = PmUser.UserName;
                    cUser.UserShopUserGroupID = PmUser.UserShopUserGroupID;
                    cUser.UserSysID = PmUser.UserSysID;
                    cUser.UserSysName = PmUser.UserSysName;
                }
                else
                {
                    cUser.AlertMessage = "请不要越权操作!";
                }
            }
            else
            {
                cUser.AlertMessage = "请不要越权操作!";
            }
            return cUser;
        }
    }
}
