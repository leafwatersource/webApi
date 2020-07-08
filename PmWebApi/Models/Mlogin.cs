using System;
using System.Collections.Generic;
using PmWebApi.Classes.StaticClasses;
using PmWebApi.Classes;
using System.Data.SqlClient;
using System.Data;

namespace PmWebApi.Models
{
    /// <summary>
    /// 用户登录处理类
    /// </summary>
    public class Mlogin
    {
        public CLogin ForceOut(string userName,string userpass)
        {

            SqlCommand  cmd = PmConnections.CtrlCmd();
            cmd.CommandText = "SELECT online FROM wapUserstate WHERE empid = '" + GetUpdateVal() + "'";
            SqlDataReader rd = cmd.ExecuteReader();
            if (rd.Read())
            {
                if(Convert.ToBoolean(rd[0]))
                {
                    rd.Close();
                    cmd.CommandText = "DELETE FROM wapUserstate WHERE empid = '" + GetUpdateVal() + "'";
                    cmd.ExecuteNonQuery();
                }                
                rd.Close();
                cmd.Connection.Close();
                return Login(userName, userpass);
            }
            else
            {
                return Login(userName, userpass);
            }
        }
        /// <summary>
        /// 登录方法
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userPass">密码</param>
        /// <returns>Login信息类Clogin</returns>
        readonly int pwderrTimes = 3; //用于记录密码错误次数
        public string UserIP { get; set; }
        public CLogin Login(string userName, string userPass)
        {
            if(CUserInfo.LogedUserInfo == null)
            {
                CUserInfo.LogedUserInfo = new Dictionary<string, string>();
            }
            // 获取用户的IP地址和浏览器信息
            SqlCommand cmd = PmConnections.ModCmd();
            cmd.CommandText = "SELECT *  FROM wapEmpList WHERE " + PmSettings.LoginColName +" = '" + userName + "'";
            DataTable dtuserdata = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            dataAdapter.Fill(dtuserdata);
            dataAdapter.Dispose();
            cmd.Connection.Close();
            if(dtuserdata.Rows.Count > 0)
            {
                DataRow data = PublicFunc.CheckEmptyVal(dtuserdata.Clone(), dtuserdata.Rows[0]);
                PmUser.EmpID = Convert.ToInt32(data["empID"]);
                PmUser.UserName = data["empName"].ToString();
                PmUser.EmpWorkID = data["empWorkID"].ToString();
                PmUser.UserDesc = data["dept"].ToString();
                PmUser.PhoneNumber = data["phoneNum"].ToString();
                PmUser.Email = data["email"].ToString();
                PmUser.UserSysID = Convert.ToInt32(data["sysID"]);
                PmUser.UsercusID = Convert.ToInt32(data["cusID"]);
                PmUser.UserShopUserGroupID = data["shopUserGroupID"].ToString();
                PmUser.UserSysName = PublicFunc.GetSysName(PmUser.UserSysID);
                PmUser.UserIpAdress = UserIP;
                PmUser.UserPass = userPass;
            }           
            CLogin login = new CLogin();
            //查询是否有这个用户名
            if (dtuserdata.Rows.Count == 0)
            {
                login.LoginState = 0;
                login.Message = "没有这个用户名.";
            }
            //判断密码是否正确
            else
            {
                //服务器获取登录状态
                cmd = PmConnections.CtrlCmd();
                cmd.CommandText = "SELECT * FROM wapUserstate WHERE empid = '" + userName + "'";
                DataTable dtuserstate = new DataTable();
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dtuserstate);
                dataAdapter.Dispose();
                cmd.Connection.Close();
                //判断是否是登录状态
                if (dtuserstate.Rows.Count > 0)
                {
                    //查询用户是否在线
                    if (Convert.ToBoolean(dtuserstate.Rows[0]["online"]))
                    {
                        //用户在线,返回在线消息
                        login.LoginState = 2;
                        login.Message = "用户已经在IP:" + dtuserstate.Rows[0]["userIpaddress"] + " 上登陆.";
                    }
                    else
                    {
                        //用户不在线,但是密码错误
                        DateTime lasterrortime = Convert.ToDateTime(dtuserstate.Rows[0]["errorTime"]);
                        int errortimes = Convert.ToInt32(dtuserstate.Rows[0]["errorTimes"]);
                        int haslogintimes = pwderrTimes - errortimes;
                        if (haslogintimes > 0)
                        {
                            //可以再次登录
                            if (userPass == dtuserdata.Rows[0]["password"].ToString())
                            {
                                //密码正确,返回登录信息,记录用户为正常登录
                                login.LoginState = 1;
                                login.Message = "登录成功.";
                                login.UserGuiD = Guid.NewGuid().ToString();
                                PmUser.UserGuid = login.UserGuiD;
                                login.EmpID = PmUser.EmpID;
                                if (CUserInfo.LogedUserInfo.ContainsKey(PmUser.EmpID.ToString()))
                                {
                                    CUserInfo.LogedUserInfo[PmUser.EmpID.ToString()] = PmUser.UserGuid;
                                }
                                else
                                {
                                    CUserInfo.LogedUserInfo.Add(PmUser.EmpID.ToString(), PmUser.UserGuid);
                                }
                                //更新用户信息类                                       
                                PmUser.UserGuid = login.UserGuiD;
                                //查询用户是不是管理员
                                PmUser.IsAdmin = IsAdmin(PmUser.EmpID, PmUser.UserSysID);
                                //获取用户的功能列表
                                PmUser.FunctionList = new List<string>();
                                if (PmUser.IsAdmin == true)
                                {
                                    PmUser.FunctionList.Add("systemsetting");
                                    PmUser.FunctionList.Add("reportsystem");
                                    PmUser.FunctionList.Add("datacenter");
                                    PmUser.FunctionList.Add("planboard");
                                }
                                else
                                {
                                    cmd = PmConnections.ModCmd();
                                    cmd.CommandText = "SELECT shopUsergroupid FROM wapUser WHERE userName in (SELECT USERNAME FROM wapEmpUserMap WHERE empid = '" + PmUser.EmpID + "')";
                                    SqlDataReader rd = cmd.ExecuteReader();
                                    while (rd.Read())
                                    {
                                        if (rd[0].ToString().ToUpper() == "CFM")
                                        {
                                            PmUser.FunctionList.Add("reportsystem");
                                        }
                                        if (rd[0].ToString().ToUpper() == "REP")
                                        {
                                            PmUser.FunctionList.Add("reportsystem");
                                        }
                                        if (rd[0].ToString().ToUpper() == "VIEW")
                                        {
                                            PmUser.FunctionList.Add("datacenter");
                                        }
                                        if (rd[0].ToString().ToUpper() == "BOARD")
                                        {
                                            PmUser.FunctionList.Add("planboard");
                                        }
                                    }
                                    rd.Close();
                                    cmd.Connection.Close();
                                }
                                //更新登录状态,这里是update
                                cmd = PmConnections.CtrlCmd();
                                cmd.CommandText = "UPDATE wapUserstate SET userIpaddress = '" + PmUser.UserIpAdress + "',onLine = '1',errorTimes='0',errorTime='" + DateTime.Now + "',message = '登录成功',userGuid = '" + PmUser.UserGuid + "'WHERE empid = '" + GetUpdateVal() + "'";
                                cmd.ExecuteNonQuery();
                                cmd.Connection.Close();
                            }
                            else
                            {
                                //密码错误,返回登录信息,记录用户密码错误
                                errortimes ++;
                                login.LoginState = 0;
                                login.Message = "用户密码错误！再输入" + (3 - errortimes).ToString() + "次错误密码后，账号将被锁定5分钟.";
                                //更新登录状态,这里是update
                                cmd = PmConnections.CtrlCmd();
                                cmd.CommandText = "UPDATE wapUserstate SET userIpaddress = '" + PmUser.UserIpAdress + "',onLine = '0',errorTimes='" + errortimes + "',errorTime='" + DateTime.Now + "',message = '密码错误',userGuid = '" + PmUser.UserGuid + "'WHERE empid = '" + GetUpdateVal() + "'";
                                cmd.ExecuteNonQuery();
                                cmd.Connection.Close();                              
                            }
                        }
                        else
                        {
                            if ((DateTime.Now - lasterrortime).TotalSeconds > 300)
                            {
                                //可以验证密码登录
                                if (userPass == dtuserdata.Rows[0]["password"].ToString())
                                {
                                    login.LoginState = 1;
                                    login.Message = "登录成功.";
                                    login.UserGuiD = Guid.NewGuid().ToString();
                                    PmUser.UserGuid = login.UserGuiD;
                                    login.EmpID = PmUser.EmpID;
                                    if (CUserInfo.LogedUserInfo.ContainsKey(PmUser.EmpID.ToString()))
                                    {
                                        CUserInfo.LogedUserInfo[PmUser.EmpID.ToString()] = PmUser.UserGuid;
                                    }
                                    else
                                    {
                                        CUserInfo.LogedUserInfo.Add(PmUser.EmpID.ToString(), PmUser.UserGuid);
                                    }
                                    //更新用户信息类                                
                                    PmUser.UserGuid = login.UserGuiD;
                                    //查询用户是不是管理员
                                    PmUser.IsAdmin = IsAdmin(PmUser.EmpID, PmUser.UserSysID);
                                    //获取用户的功能列表
                                    PmUser.FunctionList = new List<string>();
                                    if (PmUser.IsAdmin == true)
                                    {
                                        PmUser.FunctionList.Add("systemsetting");
                                        PmUser.FunctionList.Add("reportsystem");
                                        PmUser.FunctionList.Add("datacenter");
                                        PmUser.FunctionList.Add("planboard");
                                    }
                                    else
                                    {
                                        cmd = PmConnections.ModCmd();
                                        cmd.CommandText = "SELECT shopUSergroupid FROM wapUser WHERE userName in (SELECT USERNAME FROM wapEmpUserMap WHERE empid = '" + PmUser.EmpID + "') and sysid = '" + PmUser.UserSysID + "'";
                                        SqlDataReader rd = cmd.ExecuteReader();
                                        while (rd.Read())
                                        {
                                            if (rd[0].ToString().ToUpper() == "CFM")
                                            {
                                                PmUser.FunctionList.Add("reportsystem");
                                            }
                                            if (rd[0].ToString().ToUpper() == "REP")
                                            {
                                                PmUser.FunctionList.Add("reportsystem");
                                            }
                                            if (rd[0].ToString().ToUpper() == "VIEW")
                                            {
                                                PmUser.FunctionList.Add("datacenter");
                                            }
                                            if (rd[0].ToString().ToUpper() == "BOARD")
                                            {
                                                PmUser.FunctionList.Add("planboard");
                                            }
                                        }
                                        rd.Close();
                                        cmd.Connection.Close();
                                    }
                                    //更新登录状态,这里是update
                                    cmd = PmConnections.CtrlCmd();
                                    cmd.CommandText = "UPDATE wapUserstate SET userIpaddress = '" + PmUser.UserIpAdress + "',onLine = '1',errorTimes='0',errorTime='" + DateTime.Now + "',message = '登录成功',userGuid = '" + PmUser.UserGuid + "' WHERE empid = '" + GetUpdateVal() + "'";
                                    cmd.ExecuteNonQuery();
                                    cmd.Connection.Close();
                                }
                                //用户密码错误记录密码错误次数
                                else
                                {
                                    //删除登录状态,重新登录
                                    cmd = PmConnections.CtrlCmd();
                                    cmd.CommandText = "DELETE FROM wapUserstate WHERE empid = '" + GetUpdateVal() + "'";
                                    cmd.ExecuteNonQuery();
                                    cmd.Connection.Close();
                                    //正常退出的用户,判断用户密码是否正确
                                    if (userPass == dtuserdata.Rows[0]["password"].ToString())
                                    {
                                        //密码正确,返回登录信息,记录用户为正常登录
                                        login.LoginState = 1;
                                        login.Message = "登录成功.";
                                        login.UserGuiD = Guid.NewGuid().ToString();
                                        PmUser.UserGuid = login.UserGuiD;
                                        login.EmpID = PmUser.EmpID;
                                        if (CUserInfo.LogedUserInfo.ContainsKey(PmUser.EmpID.ToString()))
                                        {
                                            CUserInfo.LogedUserInfo[PmUser.EmpID.ToString()] = PmUser.UserGuid;
                                        }
                                        else
                                        {
                                            CUserInfo.LogedUserInfo.Add(PmUser.EmpID.ToString(), PmUser.UserGuid);
                                        }
                                        //更新用户信息类                       
                                        PmUser.UserGuid = login.UserGuiD;
                                        //查询用户是不是管理员
                                        PmUser.IsAdmin = IsAdmin(PmUser.EmpID, PmUser.UserSysID);
                                        //获取用户的功能列表
                                        PmUser.FunctionList = new List<string>();
                                        if (PmUser.IsAdmin == true)
                                        {
                                            PmUser.FunctionList.Add("systemsetting");
                                            PmUser.FunctionList.Add("reportsystem");
                                            PmUser.FunctionList.Add("datacenter");
                                            PmUser.FunctionList.Add("planboard");
                                        }
                                        else
                                        {
                                            cmd = PmConnections.ModCmd();
                                            cmd.CommandText = "SELECT shopUSergroupid FROM wapUser WHERE userName in (SELECT USERNAME FROM wapEmpUserMap WHERE empid = '" + PmUser.EmpID + "') and sysid = '" + PmUser.UserSysID + "'";
                                            SqlDataReader rd = cmd.ExecuteReader();
                                            while (rd.Read())
                                            {
                                                if (rd[0].ToString().ToUpper() == "CFM")
                                                {
                                                    PmUser.FunctionList.Add("reportsystem");
                                                }
                                                if (rd[0].ToString().ToUpper() == "REP")
                                                {
                                                    PmUser.FunctionList.Add("reportsystem");
                                                }
                                                if (rd[0].ToString().ToUpper() == "VIEW")
                                                {
                                                    PmUser.FunctionList.Add("datacenter");
                                                }
                                                if (rd[0].ToString().ToUpper() == "BOARD")
                                                {
                                                    PmUser.FunctionList.Add("planboard");
                                                }
                                            }
                                            rd.Close();
                                            cmd.Connection.Close();
                                        }
                                        //更新登录状态
                                        cmd = PmConnections.CtrlCmd();
                                        cmd.CommandText = "INSERT wapUserstate (empID,empName,userPass,userIpaddress,onLine,errorTimes,errorTime,message,userGuid) VALUES ('" + GetUpdateVal() + "','"
                                            + PmUser.UserName + "','" + PmUser.UserPass + "','" + PmUser.UserIpAdress + "','1','0','" + DateTime.Now + "','登录成功','" + PmUser.UserGuid + "')";
                                        cmd.ExecuteNonQuery();
                                        cmd.Connection.Close();
                                        if (CUserInfo.LogedUserInfo.ContainsKey(PmUser.UserName))
                                        {
                                            CUserInfo.LogedUserInfo[PmUser.UserName] = PmUser.UserGuid;
                                        }
                                        else
                                        {
                                            CUserInfo.LogedUserInfo.Add(PmUser.UserName, PmUser.UserGuid);
                                        }
                                    }
                                    else
                                    {
                                        //密码错误,返回登录信息,记录用户密码错误
                                        login.LoginState = 0;
                                        login.Message = "用户密码错误！再输入2次错误密码后，账号将被锁定5分钟.";
                                        //更新登录状态
                                        cmd = PmConnections.CtrlCmd();
                                        cmd.CommandText = "INSERT wapUserstate (empID,empName,userPass,userIpaddress,onLine,errorTimes,errorTime,message,userGuid) VALUES ('" + GetUpdateVal() + "','"
                                          + PmUser.UserName + "','" + PmUser.UserPass + "','" + PmUser.UserIpAdress + "','0','1','" + DateTime.Now + "','密码错误','" + PmUser.UserGuid + "')";
                                        cmd.ExecuteNonQuery();
                                        cmd.Connection.Close();
                                    };

                                }
                            }
                            else
                            {
                                login.LoginState = 0;
                                login.Message = "用户被锁定，请在" + (300 - (DateTime.Now - lasterrortime).TotalSeconds).ToString("0") + "秒后登陆.";
                            }                           
                        }
                    }
                }
                else
                {
                    //正常退出的用户,判断用户密码是否正确
                    if (userPass == dtuserdata.Rows[0]["password"].ToString())
                    {
                        //密码正确,返回登录信息,记录用户为正常登录
                        login.LoginState = 1;
                        login.Message = "登录成功.";
                        login.UserGuiD = Guid.NewGuid().ToString();
                        PmUser.UserGuid = login.UserGuiD;
                        login.EmpID = PmUser.EmpID;
                        if (CUserInfo.LogedUserInfo.ContainsKey(PmUser.EmpID.ToString()))
                        {
                            CUserInfo.LogedUserInfo[PmUser.EmpID.ToString()] = PmUser.UserGuid;
                        }
                        else
                        {
                            CUserInfo.LogedUserInfo.Add(PmUser.EmpID.ToString(), PmUser.UserGuid);
                        }
                        //更新用户信息类                       
                        PmUser.UserGuid = login.UserGuiD;
                        //查询用户是不是管理员
                        PmUser.IsAdmin = IsAdmin(PmUser.EmpID,PmUser.UserSysID);
                        //获取用户的功能列表
                        PmUser.FunctionList = new List<string>();
                        if(PmUser.IsAdmin == true)
                        {
                            PmUser.FunctionList.Add("systemsetting");
                            PmUser.FunctionList.Add("reportsystem");
                            PmUser.FunctionList.Add("datacenter");
                            PmUser.FunctionList.Add("planboard");
                        }
                        else
                        {
                            cmd = PmConnections.ModCmd();
                            cmd.CommandText = "SELECT shopUSergroupid FROM wapUser WHERE userName in (SELECT USERNAME FROM wapEmpUserMap WHERE empid = '" + PmUser.EmpID + "') and sysid = '" + PmUser.UserSysID + "'";
                            SqlDataReader rd = cmd.ExecuteReader();
                            while (rd.Read())
                            {
                                if (rd[0].ToString().ToUpper() == "CFM")
                                {
                                    PmUser.FunctionList.Add("reportsystem");
                                }
                                if (rd[0].ToString().ToUpper() == "REP")
                                {
                                    PmUser.FunctionList.Add("reportsystem");
                                }
                                if (rd[0].ToString().ToUpper() == "VIEW")
                                {
                                    PmUser.FunctionList.Add("datacenter");
                                }
                                if (rd[0].ToString().ToUpper() == "BOARD")
                                {
                                    PmUser.FunctionList.Add("planboard");
                                }
                            }
                            rd.Close();
                            cmd.Connection.Close();
                        }
                        //更新登录状态
                        cmd = PmConnections.CtrlCmd();
                        cmd.CommandText = "INSERT wapUserstate (empID,empName,userPass,userIpaddress,onLine,errorTimes,errorTime,message,userGuid) VALUES ('" + GetUpdateVal() + "','"
                            + PmUser.UserName + "','"+PmUser.UserPass+"','" + PmUser.UserIpAdress + "','1','0','" + DateTime.Now + "','登录成功','" + PmUser.UserGuid + "')";
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();       
                    }
                    else
                    {
                        //密码错误,返回登录信息,记录用户密码错误
                        login.LoginState = 0;
                        login.Message = "用户密码错误！再输入2次错误密码后，账号将被锁定5分钟.";
                        //更新登录状态
                        cmd = PmConnections.CtrlCmd();
                        cmd.CommandText = "INSERT wapUserstate (empID,empName,userPass,userIpaddress,onLine,errorTimes,errorTime,message,userGuid) VALUES ('" + GetUpdateVal() + "','"
                          + PmUser.UserName + "','"+PmUser.UserPass+"','" + PmUser.UserIpAdress + "','0','1','" + DateTime.Now + "','密码错误','" + PmUser.UserGuid + "')";
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
            }
            return login;
        }
        
        private bool IsAdmin (int empid,int userSysID)
        {
            SqlCommand cmd = PmConnections.ModCmd();
            cmd.CommandText = "SELECT userName,shopUSergroupid,sysid FROM wapUser WHERE userName in (SELECT USERNAME FROM wapEmpUserMap WHERE empid = '" + empid + "') and sysid = '" + userSysID + "'";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dtusergroup = new DataTable();
            adapter.Fill(dtusergroup);
            adapter.Dispose();
            bool isadmin = false;
            if(dtusergroup.Rows.Count > 0)
            {
                foreach (DataRow item in dtusergroup.Rows)
                {
                    if(item["shopUSergroupid"].ToString().ToUpper() != "ADMIN")
                    {
                        continue;
                    }
                    else
                    {
                        isadmin = true;
                        break;
                    }
                }
            }
            else
            {
                isadmin = false;
            }
            return isadmin;
        }
        private string GetUpdateVal()
        {
            string val = string.Empty;
            if(PmSettings.LoginColName.ToUpper() =="EMPID")
            {
                val= PmUser.EmpID.ToString();
            }
            if(PmSettings.LoginColName.ToUpper() == "EMPWORKID")
            {
                val = PmUser.EmpWorkID;
            }
            return val;
        }
    }
}
