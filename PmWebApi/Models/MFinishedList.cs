using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PmWebApi.Models
{
    public class MFinishedList
    {
        public List<COrderList> GetFinishedOrderList(string resName)
        {
            SqlCommand cmd = PmConnections.SchCmd();

            cmd.CommandText = "SELECT * FROM User_MesDailyData WHERE pmResName = '" + resName + "' and datatype = 'P' and mesdailyDate ='" + PublicFunc.GetDailyDate(resName) + "' and TaskFinishState = '4' and dayShift = '" + PublicFunc.GetDayShift(resName) + "' ORDER BY mesdailyDate,planStartTime";
            //cmd.CommandText = "SELECT * FROM User_MesDailyData WHERE pmResName = '" + resName + "' and datatype = 'P'and TaskFinishState < '4' ORDER BY dailyDate,planStartTime";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable returndata = new DataTable();
            da.Fill(returndata);
            da.Dispose();
            cmd.Connection.Close();
            if (returndata.Rows.Count < 1)
            {
                return null;
            }
            else
            {
                List<COrderList> LiReturnData = new List<COrderList>();
                foreach (DataRow item in returndata.Rows)
                {
                    DataRow checkeddata = PublicFunc.CheckEmptyVal(returndata, item);
                    COrderList li = new COrderList
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
                        FinishedQty = Convert.ToDouble(checkeddata["FinishedQty"]),
                        Plannedqty = Convert.ToDouble(checkeddata["Plannedqty"]),
                        FailedQty = Convert.ToDouble(checkeddata["FailQty"]),
                        AllFinishedQty = Convert.ToDouble(checkeddata["AllFinishedQty"]),
                        JobQty = Convert.ToDouble(checkeddata["JobQty"]),
                        ItemAttr1 = checkeddata["ItemAttr1"].ToString(),
                        ItemAttr2 = checkeddata["ItemAttr2"].ToString(),
                        ItemAttr3 = checkeddata["ItemAttr3"].ToString(),
                        ItemAttr4 = checkeddata["ItemAttr4"].ToString(),
                        DayShift = Convert.ToInt32(checkeddata["DayShift"]),
                        ItemDesp = checkeddata["itemDesp"].ToString(),
                        WorkHours = Convert.ToDouble(checkeddata["workHour"]),
                        SetupTime = Convert.ToDouble(checkeddata["setupTime"]),
                        OrderUID = Convert.ToInt32(checkeddata["UID"]),
                        BomComused = Convert.ToDouble(checkeddata["AllJobTaskqty"]) / Convert.ToDouble(checkeddata["JobQty"]),
                        CanReport = true,
                        CanReportQty = Convert.ToDouble(checkeddata["AllJobTaskqty"]) - Convert.ToDouble(checkeddata["AllFinishedQty"]) - Convert.ToDouble(checkeddata["FailQty"]),
                        ChangeResName = string.Empty,
                        ReportTime = Convert.ToDateTime(checkeddata["updateDatetime"]),
                        JobDemandDay = Convert.ToDateTime(checkeddata["jobDemandDay"]),
                        ScrappedQty = Convert.ToDouble(checkeddata["ScrappedQty"]),
                        Ajustment = Convert.ToBoolean(checkeddata["adjustment"]),
                        UnitPrice = Convert.ToDouble(checkeddata["unitPrice"])
                    };
                    LiReturnData.Add(li);

                }
                bool hasoutput = false;
                foreach (COrderList c in LiReturnData)
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
                    foreach (COrderList c in LiReturnData)
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
                return LiReturnData;
            }
        }
    }
}
