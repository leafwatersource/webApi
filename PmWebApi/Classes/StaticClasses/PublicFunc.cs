using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace PmWebApi.Classes.StaticClasses
{
    public static class GetUserLoginState
    {
        public static bool LoginState(IHeaderDictionary header)
        {
            JObject jObject;
            if (header.ContainsKey("token"))
            {
                jObject = JsonConvert.DeserializeObject<JObject>(header["token"]);
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
        public static int GetMaxUID(string serverdb, string tablename, string key)
        {
            SqlCommand cmd = null;
            if (serverdb.ToUpper() == "S")
            {
                cmd = PmConnections.SchCmd();
            }
            else if (serverdb.ToUpper() == "M")
            {
                cmd = PmConnections.ModCmd();
            }
            else if (serverdb.ToUpper() == "C")
            {
                cmd = PmConnections.CtrlCmd();
            }
            //判断数据库是否为空
            cmd.CommandText = "SELECT COUNT(" + key + ") as R FROM " + tablename;
            SqlDataReader rd = cmd.ExecuteReader();
            rd.Read();
            int R = Convert.ToInt32(rd[0]);
            rd.Close();
            if (R == 0)
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
            cmd.CommandText = "SELECT " + PmSettings.SysNameColName + " FROM pmSysList WHERE sysID = '" + sysID + "'";
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
        /// 获取用户名称
        /// </summary>
        /// <param name="empID">用户ID</param>
        /// <returns>用户名称</returns>
        public static string GetEmpName(int empID)
        {
            SqlCommand cmd = PmConnections.ModCmd();
            cmd.CommandText = "SELECT empName FROM wapEmpList WHERE empID = '" + empID + "'";
            SqlDataReader rd = cmd.ExecuteReader();
            string empname = string.Empty;
            if (rd.Read())
            {
                empname = rd[0].ToString();
            }
            rd.Close();
            cmd.Connection.Close();
            return empname;
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
                if (datatype == "SYSTEM.INT32" || datatype == "SYSTEM.INT64")
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
                if (datatype == "SYSTEM.DATETIME")
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
                if (datatype == "SYSTEM.DECIMAL")
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
                if (datatype == "SYSTEM.BOOLEAN")
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
        /// <summary>
        /// 获取这个设备当前时间的Dailydate
        /// </summary>
        /// <param name="resname"></param>
        /// <returns></returns>
        public static DateTime GetDailyDate(string resname)
        {
            if (string.IsNullOrWhiteSpace(resname))
            {
                return DateTime.MinValue;
            }
            else
            {
                string realResname;
                if (resname.Contains(":"))
                {
                    realResname = resname.Split(':')[0];
                }
                else
                {
                    realResname = resname;
                }
                SqlCommand cmd = PmConnections.ModCmd();
                cmd.CommandText = "SELECT a.resourceName,b.shiftStartTime,c.fromHour,c.toHour,c.workdayInWeek,c.workshift " +
                                  "FROM objResource a,objCalendar b,objCalendarWorking c " +
                                  "WHERE a.calendarUID = c.calendarUID and b.calendarUID = c.calendarUID  and a.resourceName = '" + realResname + "' and a.sysID = '" + PmUser.UserSysID + "'" +
                                  "ORDER by c.workdayInWeek,c.workShift";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dtshift = new DataTable();
                da.Fill(dtshift);
                cmd.Connection.Close();
                if (dtshift.Rows.Count > 0)
                {
                    int calstarthour = Convert.ToInt32(dtshift.Rows[0]["shiftStartTime"].ToString().Split(',')[0].Split(':')[0]);
                    int nowhour = DateTime.Now.Hour;
                    int[] Day = new int[] { 0, 1, 2, 3, 4, 5, 6 };
                    int week = Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))];
                    DataRow[] drselect = dtshift.Select("workdayInWeek = '" + week + "' and workshift = 1");
                    int realstarthour = calstarthour + Convert.ToInt32(drselect[0]["fromHour"]);
                    if (nowhour < realstarthour)
                    {
                        return DateTime.Now.AddDays(-1);
                    }
                    else
                    {
                        return DateTime.Now;
                    }
                }
                else
                {
                    return DateTime.MinValue;
                }
            }

        }
        /// <summary>
        /// 获取当前设备的DayShift
        /// </summary>
        /// <param name="resName"></param>
        /// <returns></returns>
        public static int GetDayShift(string resName)
        {
            SqlCommand cmd = PmConnections.SchCmd();
            cmd.CommandText = "SELECT pmResName,shiftStartEndTime,dayShift FROM User_MesDailyData where sysID = " + PmUser.UserSysID + " and pmResName = '" + resName + "' and  mesdailydate ='" + GetDailyDate(resName).Date + "' order by dayShift";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dtshift = new DataTable();
            da.Fill(dtshift);
            dtshift = dtshift.DefaultView.ToTable(true);
            cmd.Connection.Close();
            if (dtshift.Rows.Count > 0)
            {
                if (dtshift.Rows.Count == 1)
                {
                    return Convert.ToInt32(dtshift.Rows[0]["dayShift"]);
                }
                else
                {
                    DataTable dtstarthours = new DataTable();
                    dtstarthours.Columns.Add("WorkShift");
                    dtstarthours.Columns.Add("StartHour");
                    dtstarthours.Columns.Add("EndHour");
                    for (int i = 0; i < dtshift.Rows.Count; i++)
                    {
                        DataRow dr = dtstarthours.NewRow();
                        if (i < dtshift.Rows.Count - 1)
                        {
                            dr[0] = dtshift.Rows[i]["dayShift"];
                            dr[1] = dtshift.Rows[i]["shiftStartEndTime"].ToString().Split(',')[0].Split(':')[0];
                            dr[2] = dtshift.Rows[i + 1]["shiftStartEndTime"].ToString().Split(',')[0].Split(':')[0];
                        }
                        else
                        {
                            dr[0] = dtshift.Rows[i]["dayShift"];
                            dr[1] = dtshift.Rows[i]["shiftStartEndTime"].ToString().Split(',')[0].Split(':')[0];
                            dr[2] = dtshift.Rows[0]["shiftStartEndTime"].ToString().Split(',')[0].Split(':')[0];
                        }
                        dtstarthours.Rows.Add(dr);
                    }
                    int nowhour = DateTime.Now.Hour;
                    int dayshift = -1;
                    for (int i = 0; i < dtstarthours.Rows.Count; i++)
                    {
                        int calstart = Convert.ToInt32(dtstarthours.Rows[i][1]);
                        int calend = Convert.ToInt32(dtstarthours.Rows[i][2]);
                        if (calstart < calend)
                        {
                            if (calstart <= nowhour && calend > nowhour)
                            {
                                dayshift = Convert.ToInt32(dtstarthours.Rows[i][0]);
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (calstart <= nowhour || calend > nowhour)
                            {
                                dayshift = Convert.ToInt32(dtstarthours.Rows[i][0]);
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    return dayshift;

                }
            }
            else
            {
                return -1;
            }
        }
        public static void WriteUserLog(string userempid, string ip, string model, string logmessage, string webinfo)
        {
            SqlCommand cmd = PmConnections.CtrlCmd();
            string empname = PublicFunc.GetEmpName(Convert.ToInt32(userempid));
            cmd.CommandText = "INSERT INTO wapUserlog (empid,empname,ipAddress,model,logTime,logMessage,webinfomation) VALUES ('"
                + userempid + "','" + empname + "','" + ip + "','" + model + "','" + DateTime.Now + "','" + logmessage + "','" + webinfo + "')";
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
    }
}
