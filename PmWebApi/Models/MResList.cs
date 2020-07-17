using PmWebApi.Classes;
using System.Data.SqlClient;
using System.Linq;
using PmWebApi.Classes.StaticClasses;
using System.Data;

namespace PmWebApi.Models
{
    public class MResList
    {
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
                            selectrows["LockedStartTime"] = "";
                            selectrows["LockedEndTime"] = "";
                            selectrows["LockedPerson"] = "";
                            selectrows["ResEventComment"] = "";
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
