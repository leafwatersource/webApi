using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PmWebApi.Models
{
    public class MBeginChange
    {
        public bool BeginChange_Call(string jsonedata)
        {
            
            COrderList mesEvent = JsonConvert.DeserializeObject<COrderList>(jsonedata);
            //写队列
            WriteMesEvent writeMes = new WriteMesEvent();
            if(writeMes.WriteMesEvent_Call(mesEvent,0))
            {
                //成功之后，更新订单状态
                SqlCommand cmd = PmConnections.SchCmd();
                cmd.CommandText = "UPDATE User_MesDailyData SET TaskFinishState = '1',setupStartTime = '" + DateTime.Now + "', mesresname = '" + mesEvent.MesResName + "', mesopname = '" + mesEvent.MesOpName
                    + "', updateDateTime = '" + DateTime.Now + "', bgPerson = '" + mesEvent.MesOperator + "' WHERE UID = '" + mesEvent.OrderUID + "'";
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
            else
            {
                return false;
            }
        }
    }
}
