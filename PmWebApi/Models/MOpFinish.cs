using Newtonsoft.Json;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PmWebApi.Models
{
    public class MOpFinish
    {
        public List<CFinishHistory> GetOpFinishHistory(string orderuid)
        {
            SqlCommand cmd = PmConnections.SchCmd();
            cmd.CommandText = "SELECT eventMessage,eventTime,finishedQty,failedQty,planQty,jobQty,mesOperator FROM wapMesEventRec WHERE OrderUID = '" + orderuid + "' ORDER BY EventTime";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dthistory = new DataTable();
            da.Fill(dthistory);
            da.Dispose();
            cmd.Connection.Close();
            List<CFinishHistory> cFinishHistories = new List<CFinishHistory>();
            foreach (DataRow item in dthistory.Rows)
            {
                CFinishHistory cFinish = new CFinishHistory
                {
                    EventMessage = item["eventMessage"].ToString(),
                    EventTime = PublicFunc.ForMatDateTimeStr(Convert.ToDateTime(item["eventTime"]),1),
                    FinishedQty = Convert.ToDouble(item["finishedQty"]),
                    FailedQty = Convert.ToDouble(item["failedQty"]),
                    PlannedQty = Convert.ToDouble(item["planQty"]),
                    JobQty = Convert.ToDouble(item["jobQty"]),
                    MesOperator = item["mesOperator"].ToString()
                };
                cFinishHistories.Add(cFinish);
            }
            return cFinishHistories;
        }

        public COpFinish GetOpFinishedData(string jsonedata)
        {
            COrderList order = JsonConvert.DeserializeObject<COrderList>(jsonedata);
            SqlCommand cmd = PmConnections.SchCmd();
            //获取计划数据
            cmd.CommandText = "SELECT * FROM User_MesDailyData WHERE UID = '" + order.OrderUID + "'";
            SqlDataReader rd = cmd.ExecuteReader();
            rd.Read();
            DateTime planstarttime = Convert.ToDateTime(rd["planStartTime"]);
            DateTime planendtime = Convert.ToDateTime(rd["planEndTime"]);
            double plannedsetuphours = Convert.ToDouble(rd["setupTime"]);
            int setuptime = Convert.ToInt32(plannedsetuphours * 60);  
            double plannedqty = Convert.ToDouble(rd["plannedQty"]);
            double plannedhours = Convert.ToDouble(rd["workHour"]);
            double failedqty = Convert.ToDouble(rd["failQty"]);
            double finishedqty = Convert.ToDouble(rd["finishedqty"]);
            rd.Close();
            DateTime plansetupstarttime = planstarttime.AddMinutes(-setuptime);
            DateTime plansetupendtime = planstarttime;
            //获取实际数据
            cmd.CommandText = "SELECT * FROM wapMesEventRec WHERE OrderUID = '" + order.OrderUID + "' order by EventTime";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dtmesdata = new DataTable();
            da.Fill(dtmesdata);
            da.Dispose();
            cmd.Connection.Close();      
            //计算数据
            COpFinish returndata = new COpFinish
            {
                PlanStartTime = PublicFunc.ForMatDateTimeStr(planstarttime, 1),
                PlanEndTime = PublicFunc.ForMatDateTimeStr(planendtime, 1),
                PlannedHours = plannedhours * 3600,
                PlannedQty = plannedqty,
                PlanSetupStartTime = PublicFunc.ForMatDateTimeStr(plansetupstarttime, 1),
                PlanSetupEndTime = PublicFunc.ForMatDateTimeStr(plansetupendtime, 1),
                PlannedSetupHours = plannedsetuphours * 3600,
                FinishedQty = finishedqty,
                FailedQty = failedqty
            };
            //开始计算mes数据
            //获取mes切换开始时间
            double messetuphours = 0;
            double meshours = 0;
            for (int i = 0; i < dtmesdata.Rows.Count; i++)
            {
                if(dtmesdata.Rows[i]["EventType"].ToString().Equals("S"))
                {
                    messetuphours += (Convert.ToDateTime(dtmesdata.Rows[i + 1]["EventTime"]) - Convert.ToDateTime(dtmesdata.Rows[i]["EventTime"])).TotalSeconds;
                    if(dtmesdata.Rows[i + 1]["EventType"].ToString().Equals("E"))
                    {
                        returndata.MesSetupStartTime = PublicFunc.ForMatDateTimeStr(Convert.ToDateTime(dtmesdata.Rows[i]["EventTime"]), 1);
                        returndata.MesSetupEndTime = PublicFunc.ForMatDateTimeStr(Convert.ToDateTime(dtmesdata.Rows[i + 1]["EventTime"]), 1);
                    }

                }
                if(dtmesdata.Rows[i]["EventType"].ToString().Equals("E"))
                {
                    if(dtmesdata.Rows[i + 1]["EventType"].ToString().Equals("R"))
                    {
                        meshours += (Convert.ToDateTime(dtmesdata.Rows[i + 1]["EventTime"]) - Convert.ToDateTime(dtmesdata.Rows[i]["EventTime"])).TotalSeconds;    
                        returndata.MesStartTime = PublicFunc.ForMatDateTimeStr(Convert.ToDateTime(dtmesdata.Rows[i]["EventTime"]), 1);
                        returndata.MesEndTime = PublicFunc.ForMatDateTimeStr(Convert.ToDateTime(dtmesdata.Rows[i + 1]["EventTime"]), 1);
                    }
                }
            }
            returndata.MesSetupHours = Convert.ToDouble(messetuphours.ToString("0.00"));
            returndata.MesHours = Convert.ToDouble(meshours.ToString("0.00"));
            return returndata;
        }
    }
}
