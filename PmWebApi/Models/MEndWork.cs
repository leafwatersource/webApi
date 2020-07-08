using Newtonsoft.Json;
using PmWebApi.Classes;
using PmWebApi.Classes.StaticClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PmWebApi.Models
{
    public class MEndWork
    {
        string thisresname;
        public bool EndWork(string jsonedata)
        {

            COrderList mesEvent = JsonConvert.DeserializeObject<COrderList>(jsonedata);
            WriteMesEvent writeMes = new WriteMesEvent();
            if (writeMes.WriteMesEvent_Call(mesEvent, 6))
            {
                thisresname = mesEvent.MesResName;
                SqlDataReader rd;
                SqlCommand cmd;
                int taskfinishedstate = -1;
                if (mesEvent.TaskFinishState == 2)
                {
                    //更新mesdailydata
                    cmd = PmConnections.SchCmd();
                    cmd.CommandText = "SELECT finishedQty,AllfinishedQty,AllJobTaskQty FROM User_MesDailyData WHERE UID = '" + mesEvent.OrderUID + "'";
                    rd = cmd.ExecuteReader();
                    rd.Read();
                    double serverallfinishedQty = Convert.ToDouble(rd[1]);
                    double serverfinishedqty = Convert.ToDouble(rd[0]);
                    double alljobtaskqty = Convert.ToDouble(rd[2]);
                    rd.Close();
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
                        "',ScrappedQty = ScrappedQty + '" + mesEvent.ScrappedQty + "', TaskFinishState = '" + taskfinishedstate + "' WHERE UID = '" + mesEvent.OrderUID + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE User_MesDailyData SET AllFinishedQty = AllFinishedQty + '" + (mesEvent.FinishedQty + mesEvent.FailedQty) + "', updateDateTime ='" + DateTime.Now + "' WHERE workID = '" + mesEvent.WorkID +
                        "' and productID = '" + mesEvent.ProductID + "' and pmOpName = '" + mesEvent.PmOpName + "'";
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }

                //查询下一个班次是哪天,第几个班次
                cmd = PmConnections.SchCmd();
                cmd.CommandText = "SELECT MAX(dayShift) FROM User_MesDailyData WHERE pmResName = '" + mesEvent.PmResName + "' and mesDailyDate ='" + PublicFunc.GetDailyDate(thisresname) + "'";
                rd = cmd.ExecuteReader();
                rd.Read();
                int maxdayshift = Convert.ToInt32(rd[0]);
                rd.Close();
                cmd.Connection.Close();
                int nextdayshift;
                DateTime nextdailydate;
                if (mesEvent.DayShift < maxdayshift)
                {
                    nextdayshift = mesEvent.DayShift + 1;
                    nextdailydate = PublicFunc.GetDailyDate(thisresname);
                }
                else
                {
                    nextdayshift = 1;
                    nextdailydate = PublicFunc.GetDailyDate(thisresname).AddDays(1);
                }
                //先查找这个设备上的下个班次是否有这个订单
                cmd = PmConnections.SchCmd();
                //SELECT COUNT(UID) FROM [sch_test].[dbo].[User_MesDailyData] where pmResName = '纽威立式车床17:2' and  mesDailyDate = '2020-6-20' and workID = '0021905001' and dayShift = 1 and productID = '8311280776' and pmOpName = '精车一'
                cmd.CommandText = "SELECT COUNT(UID) FROM User_MesDailyData WHERE pmResName = '" + thisresname + "' and mesDailyDate = '" + nextdailydate + "' and dayshift = '" + nextdayshift +
                    "' and workid = '" + mesEvent.WorkID + "' and productid = '" + mesEvent.ProductID + "' and pmOpName = '" + mesEvent.PmOpName + "'";
                rd = cmd.ExecuteReader();
                rd.Read();
                int hasthisorder = Convert.ToInt32(rd[0]);
                rd.Close();
                cmd.Connection.Close();
                if (taskfinishedstate == 4)
                {
                    EndOtherOrder(thisresname);
                    return true;
                }
                else
                {
                    if (hasthisorder < 1)
                    {
                        cmd = PmConnections.SchCmd();
                        cmd.CommandText = "UPDATE User_MesDailyData SET mesDailyDate = '" + nextdailydate + "', dayshift = '" + nextdayshift + "' WHERE UID = '" + mesEvent.OrderUID + "'";
                        int result = cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                        if (result > 0)
                        {
                            EndOtherOrder(thisresname);
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else
                    {
                        cmd = PmConnections.SchCmd();
                        cmd.CommandText = "UPDATE User_MesDailyData SET TaskFinishState = '5' WHERE UID = '" + mesEvent.OrderUID + "'";
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                        EndOtherOrder(thisresname);
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        //查询是否有其他订单是否换班
        private void EndOtherOrder(string resname)
        {
            MUnStartList unStartList = new MUnStartList();
            List<COrderList> cOrders = unStartList.GetUnStartOrderList(resname);
            if (cOrders.Count > 0)
            {
                foreach (COrderList item in cOrders)
                {
                    //查询下一个班次是哪天,第几个班次
                    SqlCommand cmd = PmConnections.SchCmd();
                    cmd.CommandText = "SELECT MAX(dayShift) FROM User_MesDailyData WHERE pmResName = '" + item.PmResName + "' and mesDailyDate ='" + PublicFunc.GetDailyDate(thisresname) + "'";
                    SqlDataReader rd = cmd.ExecuteReader();
                    rd.Read();
                    int maxdayshift = Convert.ToInt32(rd[0]);
                    rd.Close();
                    cmd.Connection.Close();
                    int nextdayshift;
                    DateTime nextdailydate;
                    if (item.DayShift < maxdayshift)
                    {
                        nextdayshift = item.DayShift + 1;
                        nextdailydate = PublicFunc.GetDailyDate(thisresname);
                    }
                    else
                    {
                        nextdayshift = 1;
                        nextdailydate = PublicFunc.GetDailyDate(thisresname).AddDays(1);
                    }
                    //先查找这个设备上的下个班次是否有这个订单
                    cmd = PmConnections.SchCmd();
                    //SELECT COUNT(UID) FROM [sch_test].[dbo].[User_MesDailyData] where pmResName = '纽威立式车床17:2' and  mesDailyDate = '2020-6-20' and workID = '0021905001' and dayShift = 1 and productID = '8311280776' and pmOpName = '精车一'
                    cmd.CommandText = "SELECT COUNT(UID) FROM User_MesDailyData WHERE pmResName = '" + thisresname + "' and mesDailyDate = '" + nextdailydate + "' and dayshift = '" + nextdayshift +
                        "' and workid = '" + item.WorkID + "' and productid = '" + item.ProductID + "' and pmOpName = '" + item.PmOpName + "'";
                    rd = cmd.ExecuteReader();
                    rd.Read();
                    int hasthisorder = Convert.ToInt32(rd[0]);
                    rd.Close();
                    cmd.Connection.Close();

                    if (hasthisorder < 1)
                    {
                        cmd = PmConnections.SchCmd();
                        cmd.CommandText = "UPDATE User_MesDailyData SET mesDailyDate = '" + nextdailydate + "', dayshift = '" + nextdayshift + "' WHERE UID = '" + item.OrderUID + "'";
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        cmd = PmConnections.SchCmd();
                        cmd.CommandText = "UPDATE User_MesDailyData SET TaskFinishState = '5' WHERE UID = '" + item.OrderUID + "'";
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
            }
        }
    }
}
