using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PmWebApi.Classes.StaticClasses
{
    public static class GetUserLoginState
    {        
        public static bool LoginState(IHeaderDictionary header)
        {
            JObject jObject;
            if (header.ContainsKey("token"))
            {
                jObject = JsonConvert.DeserializeObject<JObject>(header["token"].ToString());
                CUserToken.UserEmpID = jObject.GetValue("UserEmpID").ToString();
                CUserToken.UserGuid = jObject.GetValue("UserGuid").ToString();
            }
            else
            {
                //EmpID=2; UserGuid=36bafb5b-bb8c-4c3e-8b7f-be8b23af8bf6
                string[] sss = header["Cookie"].ToString().Split(';');
                CUserToken.UserEmpID = sss[0].Split('=')[1];
                CUserToken.UserGuid = sss[1].Split('=')[1];
            }           

            if (CUserInfo.LogedUserInfo == null)
            {
                return false;
            }
            else
            {
                if (CUserInfo.LogedUserInfo[CUserToken.UserEmpID] == CUserToken.UserGuid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    public static class PublicFunc
    {
        /// <summary>
        /// 获取当前表的最大ID，适用于表格不具有自增长字段
        /// </summary>
        /// <param name="serverdb">S:schedule数据库;M:modeler数据库;C:Control数据库</param>
        /// <param name="tablename">数据库名称</param>
        /// <param name="key">数据库的KEY字段</param>
        /// <returns></returns>
        public static int GetMaxUID(string serverdb,string tablename,string key)
        {
            SqlCommand cmd = null;
            if(serverdb.ToUpper() == "S")
            {
                cmd = PmConnections.SchCmd();
            }
            else if (serverdb.ToUpper() == "M")
            {
                cmd = PmConnections.ModCmd();
            }
            else if(serverdb.ToUpper() == "C")
            {
                cmd = PmConnections.CtrlCmd();
            }
            //判断数据库是否为空
            cmd.CommandText = "SELECT COUNT(" + key + ") as R FROM " + tablename;
            SqlDataReader rd = cmd.ExecuteReader();
            rd.Read();
            int R = Convert.ToInt32(rd[0]);
            rd.Close();
            if(R == 0)
            {
                return 1;
            }
            else
            {
                cmd.CommandText = "SELECT MAX(" + key + ") AS maxid FROM " + tablename;
                rd = cmd.ExecuteReader(0);
                rd.Read();
                int maxid = Convert.ToInt32(rd[0]) + 1;
                return maxid;
            }            
        }
        /// <summary>
        /// 对字符串生成MD5码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMd5(string str)
        {
            MD5 md5str = MD5.Create();
            byte[] s = md5str.ComputeHash(Encoding.UTF8.GetBytes(str));
            md5str.Dispose();
            return Convert.ToBase64String(s);
        }
        /// <summary>
        /// 获取当前sysid的显示名称
        /// </summary>
        /// <param name="sysID"></param>
        /// <returns></returns>
        public static string GetSysName(int sysID)
        {
            SqlCommand cmd = PmConnections.ModCmd();
            cmd.CommandText = "SELECT "+PmSettings.SysNameColName+" FROM pmSysList WHERE sysID = '" + sysID + "'";
            SqlDataReader rd = cmd.ExecuteReader();
            string sysname = string.Empty;
            if (rd.Read())
            {
                sysname = rd[0].ToString();
            }
            rd.Close();
            cmd.Connection.Close();
            return sysname;
        }
        /// <summary>
        /// 转换数据库中空值的表
        /// </summary>
        /// <param name="table">空表获得这条数据的数据格式</param>
        /// <param name="data">这个表中的某条数据</param>
        /// <returns></returns>
        public static DataRow CheckEmptyVal(DataTable table, DataRow data)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string datatype = table.Columns[i].DataType.ToString().ToUpper();
                if(datatype == "SYSTEM.INT32" || datatype == "SYSTEM.INT64")
                {
                    try
                    {
                        Convert.ToInt32(data[i]);
                    }
                    catch (Exception)
                    {

                        data[i] = -1;
                    }
                }
                if(datatype == "SYSTEM.DATETIME")
                {
                    try
                    {
                        Convert.ToDateTime(data[i]);
                    }
                    catch (Exception)
                    {

                        data[i] = Convert.ToDateTime("1900-01-01 00:00:00");
                    }
                }
                if(datatype == "SYSTEM.DECIMAL")
                {
                    try
                    {
                        Convert.ToDecimal(data[i]);
                    }
                    catch (Exception)
                    {

                        data[i] = -0.00;
                    }
                }
                if(datatype == "SYSTEM.BOOLEAN")
                {
                    try
                    {
                        Convert.ToBoolean(data[i]);
                    }
                    catch (Exception)
                    {
                        data[i] = false;
                    }
                }
            }
            return data;
        }
        /// <summary>
        /// 返回非标准时间字符串
        /// </summary>
        /// <param name="time">timetype = 0,2020/03/06 08:00:06;timetype = 1,03/06 08:00:06;timetype = 2,03/06 08:00:06;</param>
        /// <param name="timetype"></param>
        /// <returns></returns>
        public static string ForMatDateTimeStr(DateTime time, int timetype)
        {
            string returndata;
            string monthstr, daystr, hourstr, minutestr, secondstr;
            monthstr = time.Month.ToString();
            if (monthstr.Length < 2)
            {
                monthstr = "0" + monthstr;
            }
            daystr = time.Day.ToString();
            if (daystr.Length < 2)
            {
                daystr = "0" + daystr;
            }
            hourstr = time.Hour.ToString();
            if (hourstr.Length < 2)
            {
                hourstr = "0" + hourstr;
            }
            minutestr = time.Minute.ToString();
            if (minutestr.Length < 2)
            {
                minutestr = "0" + minutestr;
            }
            secondstr = time.Second.ToString();
            if (secondstr.Length < 2)
            {
                secondstr = "0" + secondstr;
            }
            if (timetype == 0)
            {
                returndata = time.Year + "/" + monthstr + "/" + daystr + " " + hourstr + ":" + minutestr + ":" + secondstr;
            }
            else if (timetype == 1)
            {
                returndata = monthstr + "/" + daystr + " " + hourstr + ":" + minutestr + ":" + secondstr;
            }
            else if (timetype == 2)
            {

                returndata = monthstr + "/" + daystr + " " + hourstr + ":" + minutestr;
            }
            else
            {
                returndata = "1900/01/01 00:00:00";
            }
            return returndata;
        }
    }
}
