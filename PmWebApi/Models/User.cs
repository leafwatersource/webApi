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
        public DataTable GetUserLog(string empid,string startTime,string endTime)
        {
            DataTable table = new DataTable();
            DateTime start =  Convert.ToDateTime(startTime);
            DateTime end = Convert.ToDateTime(endTime);
            SqlCommand cmd = PmConnections.CtrlCmd();
            cmd.CommandText = "select * from wapUserlog where empID = '" + empid + "' and logTime > '" + start + "' and logTime < '" + end + "'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            cmd.Connection.Close();
            da.Dispose();
            return table;
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
        //public void WriteLog(string empID, string empName)
        //{
        //    写入登录日志
        //    SqlCommand cmd = PmConnections.CtrlCmd();
        //    cmd.Parameters.Add("@empID", SqlDbType.VarChar).Value = empID;
        //    cmd.Parameters.Add("@empName", SqlDbType.VarChar).Value = empName;
        //    cmd.Parameters.Add("@ipaddress", SqlDbType.VarChar).Value = ipaddress;
        //    cmd.Parameters.Add("@model", SqlDbType.VarChar).Value = model;
        //    cmd.Parameters.Add("@time", SqlDbType.DateTime).Value = time;
        //    cmd.Parameters.Add("@message", SqlDbType.VarChar).Value = message;
        //    cmd.Parameters.Add("@webinfo", SqlDbType.VarChar).Value = webinfo;
        //    cmd.CommandText = "insert into wapUserlog (empID,empName,ipAddress,model,logtime,logmessage,webinfomation) values (@empID,@empName,@ipaddress,@model,@time,@message,@webinfo)";
        //    cmd.ExecuteNonQuery();
        //    cmd.Connection.Close();
        //}
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
