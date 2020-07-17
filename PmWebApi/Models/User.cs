using Newtonsoft.Json;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace PmWebApi.Models
{
    public class User
    {
        public bool UpdateUserinfo(string userinfo)
        {
            CUserInfo mesEvent = JsonConvert.DeserializeObject<CUserInfo>(userinfo);
            SqlCommand cmd = PmConnections.ModCmd();
            cmd.CommandText = "UPDATE wapEmpList SET phoneNum = '" + mesEvent.PhoneNumber + "',email = '" + mesEvent.Email + "' WHERE SYSID = '" + mesEvent.UserSysID + "' and empid = '" + mesEvent.EmpID + "'";
            int result = cmd.ExecuteNonQuery();
            if(result == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public DataTable GetUserLog(string empid,int logtype)
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PmConnections.CtrlCmd();
            DateTime start;
            DateTime end;
            if (logtype == 0)
            {
                end = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                start = DateTime.Now.Date;
                cmd.CommandText = "select * from wapUserlog where empID = '" + empid + "'and logTime >= '" + start + "' and logTime <= '" + end + "' ORDER BY logTime DESC";
            }
            else if (logtype == 1)
            {
                end = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                start = DateTime.Now.Date.AddDays(-3);
                cmd.CommandText = "select * from wapUserlog where empID = '" + empid + "' and logTime >= '" + start + "' and logTime <= '" + end + "' ORDER BY logTime DESC";
            }
            else if (logtype == 2)
            {
                end = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                start = DateTime.Now.Date.AddDays(-7);
                cmd.CommandText = "select * from wapUserlog where empID = '" + empid + "' and logTime >= '" + start + "' and logTime <= '" + end + "' ORDER BY logTime DESC";
            }
            else if (logtype == 3)
            {
                end = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                start = DateTime.Now.Date.AddMonths(-1);
                cmd.CommandText = "select * from wapUserlog where empID = '" + empid + "' and logTime >= '" + start + "' and logTime <= '" + end + "' ORDER BY logTime DESC";
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            da.Dispose();
            cmd.Connection.Close();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string mobietype = CreateMobelType(table.Rows[i]["webinfomation"].ToString());
                table.Rows[i]["webinfomation"] = mobietype;
            }
            table.AcceptChanges();
            return table;
        }

        private string CreateMobelType(string webstr)
        {
            string str = string.Empty; ;
            if(webstr.Contains("Mozilla"))
            {
                for (int i = webstr.IndexOf("(") + 1; i < webstr.Length; i++)
                {
                    if (webstr[i].ToString() == ")")
                    {
                        break;
                    }
                    else
                    {
                        str += webstr[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < webstr.Length; i++)
                {
                    if (webstr[i].ToString() == ")")
                    {
                        str += webstr[i];
                        break;
                    }
                    else
                    {
                        str += webstr[i];
                    }
                }
            }
            return str;
        }
        public DataTable Operate(string empName)
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PmConnections.SchCmd();
            cmd.CommandText = "select * from wapMesEventRec where MesOperator = '" + empName + "'";
            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            ad.Fill(table);
            ad.Dispose();
            cmd.Connection.Close();
            return table;
        }
        public Boolean SignOut(string empid)
        {
            SqlCommand cmd = PmConnections.CtrlCmd();
            cmd.CommandText = "delete from wapUserstate where Empid = '" + empid + "'";
            int count = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            if (count > 0 )
            {
                return true;
            }
            return false;
        }
        public Boolean ChangePass(string empid, string oldpass,string newPass)
        {
            string pass = GetSafePass(empid, newPass);
            string olpass = GetSafePass(empid, oldpass);
            Boolean canChange = CanChangePass(empid,olpass);
            if (canChange) {
                SqlCommand cmd = PmConnections.ModCmd();
                cmd.CommandText = "update wapEmpList set password ='" + pass + "' where empID = '" + empid + "'";
                int count = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                if (count > 0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }

        }
        public Boolean HasLogin(string userName,string userGuid)
        {
            SqlCommand cmd = PmConnections.CtrlCmd();
            cmd.CommandText = "select count(empID) from wapUserstate where empID = '" + userName + "' and userGuid = '" + userGuid + "'";
            //int count = Convert.ToInt32(cmd.ExecuteReader());
            int count = Convert.ToInt32( cmd.ExecuteScalar());
            cmd.Connection.Close();
            if (count > 0)
            {
                return true;
            }
            return false;
        }
        public bool CanChangePass(string empid, string password) {
            SqlCommand cmd = PmConnections.ModCmd();
            cmd.CommandText = "select count(*) from wapEmpList where empID = '" + empid + "' and password = '" + password + "'";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Connection.Close();
            if (count > 0)
            {
                return true;
            }
            return false;
        }
        public string GetSafePass(string username,string userpass)
        {
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
            return userPass;
        }
    }
}
