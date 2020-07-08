using Newtonsoft.Json;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Data.SqlClient;

namespace PmWebApi.Models
{
    public class MReport
    {
        public bool Report_Call(string jsonedata)
        {
            COrderList mesEvent = JsonConvert.DeserializeObject<COrderList>(jsonedata);
            WriteMesEvent writeMes = new WriteMesEvent();
            if(writeMes.WriteMesEvent_Call(mesEvent,2))
            {
                //更新mesdailydata
                SqlCommand cmd = PmConnections.SchCmd();
                cmd.CommandText = "SELECT finishedQty,AllfinishedQty,AllJobTaskQty FROM User_MesDailyData WHERE UID = '" + mesEvent.OrderUID + "'";
                SqlDataReader rd = cmd.ExecuteReader();
                rd.Read();
                double serverallfinishedQty = Convert.ToDouble(rd[1]);
                double serverfinishedqty = Convert.ToDouble(rd[0]);
                double alljobtaskqty = Convert.ToDouble(rd[2]);
                rd.Close();
                int taskfinishedstate;
                if ((serverallfinishedQty + mesEvent.FinishedQty + mesEvent.FailedQty) == alljobtaskqty)
                {
                    taskfinishedstate = 4;
                }
                else
                {
                    if ((serverfinishedqty + mesEvent.FinishedQty + mesEvent.FailedQty) >= mesEvent.Plannedqty)
                    {
                        taskfinishedstate = 4;
                    }
                    else
                    {
                        taskfinishedstate = 2;
                    }
                }
               
                cmd.CommandText = "UPDATE User_MesDailyData SET mesResName = '" + mesEvent.MesResName + "', mesOpName = '" + mesEvent.MesOpName + "',finishedQty = finishedQty + '" + (mesEvent.FinishedQty + mesEvent.FailedQty) +
                    "', failQty = failQty + '" + mesEvent.FailedQty + "', endDateTime = '" + mesEvent.ReportTime + "', updateDateTime = '" + DateTime.Now + "', bgPerson = '" + mesEvent.MesOperator +
                    "',ScrappedQty = ScrappedQty + '" + mesEvent.ScrappedQty + "', TaskFinishState = '" +taskfinishedstate + "' WHERE UID = '" + mesEvent.OrderUID + "'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE User_MesDailyData SET AllFinishedQty = AllFinishedQty + '" + (mesEvent.FinishedQty + mesEvent.FailedQty) + "', updateDateTime ='" + DateTime.Now + "' WHERE workID = '" + mesEvent.WorkID +
                    "' and productID = '" + mesEvent.ProductID + "' and pmOpName = '" + mesEvent.PmOpName + "'";
                int result = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                if(result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
