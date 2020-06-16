using Newtonsoft.Json;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PmWebApi.Models
{
    public class MPauseOrder
    {
        public bool PauseOrder_Call(string jsonedata)
        {
            COrderList mesEvent = JsonConvert.DeserializeObject<COrderList>(jsonedata);
            //写队列
            WriteMesEvent writeMes = new WriteMesEvent();
            if (writeMes.WriteMesEvent_Call(mesEvent, 3))
            {
                SqlCommand cmd = PmConnections.SchCmd();
                cmd.CommandText = "UPDATE User_MesDailyData SET TaskFinishState = '3',MesResName = '" + mesEvent.MesResName + "', MesOpName = '" + mesEvent.MesOpName + "', bgPerson = '" +
                    mesEvent.MesOperator + "', updateDateTime = '" + DateTime.Now + "' WHERE UID = '" + mesEvent.OrderUID + "'";
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
