using PmWebApi.Classes.StaticClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PmWebApi.Models
{
    public class MSetResUsed
    {
        public void SetResUsed(string empid, string resName, string useType, string lockedStartTime, string lockedEndTime, string eventMessage)
        {
            int pmuid = PublicFunc.GetMaxUID("S", "wapResLockState", "PMUID");
            string username = PublicFunc.GetEmpName(Convert.ToInt32(empid));
            SqlCommand cmd = PmConnections.SchCmd();
            cmd.CommandText = "INSERT INTO wapResLockState (PMUID,ResName,LockedStartTime,LockedEndTime,LockedPerson,ResEventType,ResEventComment) VALUES ('"
                + pmuid + "','" + resName + "','" + lockedStartTime + "','" + lockedEndTime + "','" + username + "','" + useType + "','" + eventMessage + "')";
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        public void SetResUnused(string resName,string useType)
        {
            SqlCommand cmd = PmConnections.SchCmd();
            cmd.CommandText = "DELETE FROM wapResLockState WHERE ResName = '" + resName + " and ResEventType = '" + useType + "'";
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
    }
}
