using Newtonsoft.Json;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Data.SqlClient;

namespace PmWebApi.Models
{
    public class MEndChange
    {
        public bool EndChange_Call(string jsonedata)
        {
            COrderList mesEvent = JsonConvert.DeserializeObject<COrderList>(jsonedata);
            //写队列
            WriteMesEvent writeMes = new WriteMesEvent();
            if(writeMes.WriteMesEvent_Call(mesEvent,1))
            {
                //成功更新mesdailydata
                SqlCommand cmd = PmConnections.SchCmd();
                cmd.CommandText = "UPDATE User_MesDailyData SET TaskFinishState = '2',setupEndTime = '" + DateTime.Now + "', startDateTime = '" + DateTime.Now + "', mesResName = '" + mesEvent.MesResName +
                    "', mesOpName = '" + mesEvent.MesOpName + "', updateDateTime = '" + DateTime.Now + "', bgPerson = '" + mesEvent.MesOperator + "' WHERE UID ='" + mesEvent.OrderUID + "'";
                int result = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                if (result == 1)
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
