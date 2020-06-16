using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PmWebApi.Models
{
    public class MCanDownThisRes
    {
        public List<COrderList> CanDownThisRes_Call (string resname)
        {
            SqlCommand cmd = PmConnections.SchCmd();
            cmd.CommandText = "SELECT * FROM User_MesDailyData WHERE  resCanUsed like '%"+ resname.Split(':')[0] +"%' and datatype = 'P' and dailyDate >='" + DateTime.Now + "' and TaskFinishState < '4' and pmresname != '"+ resname +"' ORDER BY pmresname, dailyDate,planStartTime";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable data = new DataTable();
            da.Fill(data);
            da.Dispose();
            cmd.Connection.Close();
            List<COrderList> orderlist = new List<COrderList>();
            if(data.Rows.Count > 0)
            {
                foreach (DataRow item in data.Rows)
                {
                    DataRow checkeddata = PublicFunc.CheckEmptyVal(data, item);
                    COrderList cOrder = new COrderList()
                    {
                        MesResName = checkeddata["MesResName"].ToString(),
                        MesOpName = checkeddata["MesOpName"].ToString(),
                        MesOperator = checkeddata["bgPerson"].ToString(),
                        WorkID = checkeddata["WorkID"].ToString(),
                        PlanStartTime = PublicFunc.ForMatDateTimeStr(Convert.ToDateTime(checkeddata["PlanStartTime"]), 1),
                        Planendtime = PublicFunc.ForMatDateTimeStr(Convert.ToDateTime(checkeddata["Planendtime"]), 1),
                        PmResName = checkeddata["PmResName"].ToString(),
                        PmOpName = checkeddata["PmOpName"].ToString(),
                        ProductID = checkeddata["ProductID"].ToString(),
                        TaskFinishState = Convert.ToInt32(checkeddata["TaskFinishState"]),
                        FinishedQty = Convert.ToDecimal(checkeddata["FinishedQty"]),
                        Plannedqty = Convert.ToDecimal(checkeddata["Plannedqty"]),
                        FailedQty = Convert.ToDecimal(checkeddata["FailQty"]),
                        AllFinishedQty = Convert.ToDecimal(checkeddata["AllFinishedQty"]),
                        JobQty = Convert.ToDecimal(checkeddata["JobQty"]),
                        ItemAttr1 = checkeddata["ItemAttr1"].ToString(),
                        ItemAttr2 = checkeddata["ItemAttr2"].ToString(),
                        ItemAttr3 = checkeddata["ItemAttr3"].ToString(),
                        ItemAttr4 = checkeddata["ItemAttr4"].ToString(),
                        DayShift = Convert.ToInt32(checkeddata["DayShift"]),
                        ItemDesp = checkeddata["itemDesp"].ToString(),
                        WorkHours = Convert.ToDecimal(checkeddata["workHour"]),
                        SetupTime = Convert.ToDecimal(checkeddata["setupTime"]),
                        OrderUID = Convert.ToInt32(checkeddata["UID"]),
                        BomComused = Convert.ToDecimal(checkeddata["AllJobTaskqty"]) / Convert.ToDecimal(checkeddata["JobQty"]),
                        CanReport = true,
                        CanReportQty = Convert.ToDecimal(checkeddata["AllJobTaskqty"]) - Convert.ToDecimal(checkeddata["AllFinishedQty"]),
                        ChangeResName = string.Empty
                    };
                    orderlist.Add(cOrder);
                }
                bool hasoutput = false;
                foreach (COrderList c in orderlist)
                {
                    if (c.TaskFinishState != 0 && c.TaskFinishState != 3)
                    {
                        hasoutput = true;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (hasoutput == true)
                {
                    foreach (COrderList c in orderlist)
                    {
                        if (c.TaskFinishState != 0)
                        {
                            c.CanReport = true;
                        }
                        else
                        {
                            c.CanReport = false;
                        }
                    }
                }
            }
            return orderlist;
        }

        public bool BeginDown_Call(string orderuid,string resName,string dayshift)
        {
            SqlCommand cmd = PmConnections.SchCmd();
            cmd.CommandText = "UPDATE User_MesDailyData SET pmresname = '" + resName + "', updateDatetime ='" + DateTime.Now + "', bgPerson = '" + PmUser.UserName+ "',dayShift = '"+dayshift + "' WHERE  UID = '" + orderuid+"'";
            int result = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            if(result == 1)
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
