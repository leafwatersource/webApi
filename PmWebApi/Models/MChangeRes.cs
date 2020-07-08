using Newtonsoft.Json;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PmWebApi.Models
{
    public class MChangeRes
    {
        public List<string> GetCanResList_Call(string jsonedata)
        {
            COrderList mesEvent = JsonConvert.DeserializeObject<COrderList>(jsonedata);
            List<string> canuseres = new List<string>();
            SqlCommand cmd = PmConnections.ModCmd();
            string thisresname = mesEvent.MesResName;
            string thisopname = mesEvent.PmOpName;
            string thisproduct = mesEvent.ProductID;
            string usersysid = PmUser.UserSysID.ToString();
            cmd.CommandText = "SELECT resourceName,capacity FROM vResPrcsOpItem WHERE itemname ='" + thisproduct + "' and operationName = '" + thisopname + "' and sysid = '" + usersysid + "'";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable canress = new DataTable();
            da.Fill(canress);
            da.Dispose();
            cmd.Connection.Close();
            foreach (DataRow dr in canress.Rows)
            {
                int rescount = Convert.ToInt32(dr[1]);
                string resname = dr[0].ToString();
                if (resname == thisresname)
                {
                    continue;
                }
                if(rescount > 1)
                {                 
                    for (int i = 1; i <= rescount; i++)
                    {
                        if (resname + ":" + i.ToString() == thisresname)
                        {
                            continue;
                        }
                        else
                        {
                            canuseres.Add(resname + ":" + i.ToString());
                        }
                    }
                }
                else if(rescount == 0)
                {
                    continue;
                }
                else
                {
                    canuseres.Add(resname);
                }
            }
            return canuseres;
        }

        public bool ChangeResource_Call(string jsonedata)
        {
            COrderList mesEvent = JsonConvert.DeserializeObject<COrderList>(jsonedata);
            WriteMesEvent writeMes = new WriteMesEvent();
            if(writeMes.WriteMesEvent_Call(mesEvent,5))
            {
                SqlCommand cmd = PmConnections.SchCmd();
                cmd.CommandText = "UPDATE User_MesDailyData SET pmResName  = '" + mesEvent.ChangeResName + "' WHERE UID = '" + mesEvent.OrderUID + "'";
                int result = cmd.ExecuteNonQuery();
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
