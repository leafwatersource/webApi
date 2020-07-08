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
            DataTable canresdata = sysresdata.Clone();
            foreach (DataRow item in userdata.Rows)
            {
                DataRow[] rows = sysresdata.Select("username = '" + item[0].ToString() + "'");
                if (rows.Count() > 0)
                {
                    foreach (DataRow selectrows in rows)
                    {
                        DataRow addrows = canresdata.NewRow();
                        selectrows["dayshift"] = PublicFunc.GetDayShift(selectrows["resourceName"].ToString());
                        addrows.ItemArray = selectrows.ItemArray;
                        canresdata.Rows.Add(addrows);
                    }
                }
            }             
            return canresdata;
        }
    }
}
