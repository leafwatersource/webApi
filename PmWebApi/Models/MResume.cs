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
    public class MResume
    {
        public bool Resume_Call(string jsonedata)
        {
            COrderList mesEvent = JsonConvert.DeserializeObject<COrderList>(jsonedata);
            WriteMesEvent writeMes = new WriteMesEvent();
            if(writeMes.WriteMesEvent_Call(mesEvent,4))
            {
                SqlCommand cmd = PmConnections.SchCmd();
                cmd.CommandText = "UPDATE User_MesDailyData SET TaskFinishState = '0' WHERE UID = '" + mesEvent.OrderUID + "'";
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
