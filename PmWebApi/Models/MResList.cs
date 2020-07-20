using PmWebApi.Classes;
using System.Data.SqlClient;
using System.Linq;
using PmWebApi.Classes.StaticClasses;
using System.Data;

namespace PmWebApi.Models
{
    public class MResList
    {
        public string GetDefaultRes(string empid)
        {
            SqlCommand cmd = PmConnections.ModCmd();
            string schdbname = PmConnections.SchCmd().Connection.Database;
            cmd.CommandText = "SELECT a.resourceName  FROM  wapUser_ResPMS a,wapEmpUserMap b,wapEmpList c  where  (a.userName = b.userName and b.empID = c.empID and c.empID = " + empid + ") and (a.resourceName not in (select resname from " + schdbname + ".dbo.wapResLockState))";
            SqlDataReader rd = cmd.ExecuteReader();
            string defresname;
            if(rd.Read())
            {
                defresname = rd[0].ToString();
            }
            else
            {
                defresname = string.Empty;
            }
            rd.Close();
            cmd.Connection.Close();
            return defresname;
        }
        public DataTable GetResList(string sysid)
        {
            SqlCommand cmd = PmConnections.ModCmd();
            cmd.CommandText = "SELECT a.userName FROM wapUser a,wapEmpUserMap b where  a.userName = b.userName and a.sysID = '" + sysid + "' and b.empID ='" + CUserToken.UserEmpID + "'";
            DataTable userdata = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(userdata);
            da.Dispose();
            cmd.CommandText = "SELECT resourceName,canRep,canCfm,canQC,isDefaultRes,username FROM wapUser_ResPMS WHERE sysid = '" + sysid + "'";
            da = new SqlDataAdapter(cmd);
            DataTable sysresdata = new DataTable();
            da.Fill(sysresdata);
            da.Dispose();
            sysresdata.Columns.Add("dayshift");
            sysresdata.Columns.Add("ResEventType");
            sysresdata.Columns.Add("LockedStartTime");
            sysresdata.Columns.Add("LockedEndTime");
            sysresdata.Columns.Add("LockedPerson");
            sysresdata.Columns.Add("ResEventComment");
            DataTable canresdata = sysresdata.Clone();
            cmd.Connection.Close();

            cmd = PmConnections.SchCmd();
            cmd.CommandText = "SELECT * FROM wapResLockState";
            da = new SqlDataAdapter(cmd);
            DataTable dtlockres = new DataTable();
            da.Fill(dtlockres);
            da.Dispose();
            cmd.Connection.Close();

            foreach (DataRow item in userdata.Rows)
            {
                DataRow[] rows = sysresdata.Select("username = '" + item[0].ToString() + "'");
                if (rows.Count() > 0)
                {
                    foreach (DataRow selectrows in rows)
                    {
                        string resname = selectrows["resourceName"].ToString();
                        DataRow[] resstate = dtlockres.Select("ResName = '" + resname + "'");
                        if (resstate.Count() > 0)
                        {
                            selectrows["ResEventType"] = resstate[0]["ResEventType"].ToString();
                            selectrows["LockedStartTime"] = resstate[0]["LockedStartTime"].ToString();
                            selectrows["LockedEndTime"] = resstate[0]["LockedEndTime"].ToString();
                            selectrows["LockedPerson"] = resstate[0]["LockedPerson"].ToString();
                            selectrows["ResEventComment"] = resstate[0]["ResEventComment"].ToString();
                        }
                        else
                        {
                            selectrows["ResEventType"] = "Y";
                            selectrows["LockedStartTime"] = string.Empty;
                            selectrows["LockedEndTime"] = string.Empty;
                            selectrows["LockedPerson"] = string.Empty;
                            selectrows["ResEventComment"] = string.Empty;
                        }
                        DataRow addrows = canresdata.NewRow();
                        selectrows["dayshift"] = PublicFunc.GetDayShift(resname);
                        addrows.ItemArray = selectrows.ItemArray;
                        canresdata.Rows.Add(addrows);
                    }
                }
            }             
            return canresdata;
        }
    }
}
