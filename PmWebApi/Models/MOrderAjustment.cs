using Newtonsoft.Json;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System.Data.SqlClient;

namespace PmWebApi.Models
{
    public class MOrderAjustment
    {
        public bool OrderAjustmest_Call(string jsonEdata) 
        {
            COrderList mesEvent = JsonConvert.DeserializeObject<COrderList>(jsonEdata);
            SqlCommand cmd = PmConnections.SchCmd();
            cmd.CommandText = "UPDATE User_MesDailyData SET adjustment = '1' WHERE UID = '" + mesEvent.OrderUID + "'";
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
    }
}
