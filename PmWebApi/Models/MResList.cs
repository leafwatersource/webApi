using PmWebApi.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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
            DataTable canresdata = sysresdata.Clone();
            foreach (DataRow item in userdata.Rows)
            {
                DataRow[] rows = sysresdata.Select("username = '" + item[0].ToString() + "'");
                if (rows.Count() > 0)
                {
                    foreach (DataRow selectrows in rows)
                    {
                        selectrows[6] = GetDayShift(selectrows[0].ToString());
                        DataRow addrows = canresdata.NewRow();
                        addrows.ItemArray = selectrows.ItemArray;
                        canresdata.Rows.Add(addrows);
                    }
                }
            }             
            return canresdata;
        }
        public int GetDayShift(string resName)
        {
            DataTable dayshiftTable = new DataTable();
            SqlCommand dayshiftCmd = PmConnections.SchCmd();
            dayshiftCmd.CommandText = "select distinct(dayShift) as dayshift,shiftStartEndTime from User_MesDailyData where pmResName = '" + resName + "'";
            SqlDataAdapter dayshiftDa = new SqlDataAdapter(dayshiftCmd);
            dayshiftDa.Fill(dayshiftTable);
            dayshiftDa.Dispose();
            dayshiftCmd.Connection.Close();
            int now = DateTime.Now.Hour;
            int shift = 0;
            for (int i = 0; i < dayshiftTable.Rows.Count; i++)
            {
                int start = Convert.ToInt32(dayshiftTable.Rows[i]["shiftStartEndTime"].ToString().Split(",")[0].Split(":")[0]);
                int end = Convert.ToInt32(dayshiftTable.Rows[i]["shiftStartEndTime"].ToString().Split(",")[1].Split(":")[0]);
                if (now > start && now < end)
                {
                    shift = Convert.ToInt32(dayshiftTable.Rows[i]["dayshift"]);
                    //sysresdata.
                }
            }
            return shift;
        }
    }
}
