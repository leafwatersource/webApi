using Newtonsoft.Json.Linq;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PmWebApi.Models
{
    public class User
    {
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
        public void WriteLog(string empID,string empName)
        {
            //写入登录日志
            //SqlCommand cmd = PmConnections.CtrlCmd();
            //cmd.Parameters.Add("@empID", SqlDbType.VarChar).Value = empID;
            //cmd.Parameters.Add("@empName", SqlDbType.VarChar).Value = empName;
            //cmd.Parameters.Add("@ipaddress", SqlDbType.VarChar).Value = ipaddress;
            //cmd.Parameters.Add("@model", SqlDbType.VarChar).Value = model;
            //cmd.Parameters.Add("@time", SqlDbType.DateTime).Value = time;
            //cmd.Parameters.Add("@message", SqlDbType.VarChar).Value = message;
            //cmd.Parameters.Add("@webinfo", SqlDbType.VarChar).Value = webinfo;
            //cmd.CommandText = "insert into wapUserlog (empID,empName,ipAddress,model,logtime,logmessage,webinfomation) values (@empID,@empName,@ipaddress,@model,@time,@message,@webinfo)";
            //cmd.ExecuteNonQuery();
            //cmd.Connection.Close();
        }
        public Boolean ChangePass(string oldpass,string newPass)
        {
            string pass = GetSafePass(PmUser.EmpID.ToString(), newPass);
            string olpass = GetSafePass(PmUser.EmpID.ToString(), oldpass);
            Boolean canChange = CanChangePass(olpass);
            if (canChange) {
                SqlCommand cmd = PmConnections.ModCmd();
                cmd.CommandText = "update wapEmpList set password ='" + pass + "' where empID = '" + PmUser.EmpID + "'";
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
            string aa = cmd.ExecuteScalar().ToString();
            int count = Convert.ToInt32( cmd.ExecuteScalar());
            cmd.Connection.Close();
            if (count > 0)
            {
                return true;
            }
            return false;
        }
        public bool CanChangePass(string password) {
            SqlCommand cmd = PmConnections.ModCmd();
            cmd.CommandText = "select count(*) from wapEmpList where empID = '" + PmUser.EmpID + "' and password = '" + password + "'";
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
        public Boolean UserMessage(string userobj) {
            try
            {
                SqlCommand cmd = PmConnections.ModCmd();
                JObject userObject = JObject.Parse(userobj);
                cmd.CommandText = "update wapEmpList set empName = '" + userObject["empName"] + "' , dept = '" + userObject["dept"] + "' , phoneNum = '" + userObject["phoneNum"] + "' , email = '" + userObject["email"] + "' where empID = '" + PmUser.EmpID + "'";
                int count = cmd.ExecuteNonQuery();
                if (count > 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }
            
            return false;
        }
        public Boolean updateAllUser()
        {
            DataTable table = new DataTable();
            SqlCommand cmd = PmConnections.ModCmd();
            cmd.CommandText = "select * from wapEmpList where password=''";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(table);
            cmd.Connection.Close();
            da.Dispose();
            string pass = "111";
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string aaa = table.Rows[i]["empID"].ToString();
                string passworld = GetSafePass(table.Rows[i]["empID"].ToString(), pass);
                table.Rows[i]["password"] = passworld;
            }
            for (int i = 0; i < table.Rows.Count; i++)
            {
                SqlCommand change = PmConnections.ModCmd();
                string newPass = table.Rows[i]["password"].ToString();
                string userName = table.Rows[i]["empID"].ToString();
                change.CommandText = "update wapEmpList set password = '"+ newPass + "' where empID = '"+ userName + "'";
                int count = change.ExecuteNonQuery();
                change.Connection.Close();
            }
            return true;
        }
    }
}
